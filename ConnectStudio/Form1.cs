using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Management;

namespace ConnectStudio
{
    public partial class Form1 : Form
    {
        IEnumerable<tmpImageItem> appListFromReg;

        public Form1()
        {
            InitializeComponent();
            ClearEdgeDetail();
            processComboBox.Visible = false;
            canvas.AllowDrop = true;
            keyComboBoxHotKey.Items.AddRange(Enum.GetNames(typeof(Keys)).OrderBy(s => s).ToArray());

            appListFromReg = GetAppList();
            UpdateAppListView();
        }
        ManagementObjectCollection moc;

        void UpdateAppListView()
        {
            ImageList ls = new ImageList();

            string query = "SELECT ExecutablePath FROM Win32_Process";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            moc = searcher.Get();

            var ps = Process.GetProcesses()
                .Where(p => string.IsNullOrWhiteSpace(p.MainWindowTitle) == false)
                .GroupBy(p => p.ProcessName)
                .Select(g => g.First())
                .Select(p =>
                    {
                        SHFILEINFO shinfo = new SHFILEINFO();
                        string appPath = ProcessExecutablePath(p);
                        IntPtr hSuccess = SHGetFileInfo(appPath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                        if (hSuccess != IntPtr.Zero)
                        {
                            Icon appIcon = Icon.FromHandle(shinfo.hIcon);
                            return new tmpImageItem()
                            {
                                Path = appPath,
                                appIcon = appIcon
                            };
                        }
                        else
                        {
                            // TODO
                            Icon appIcon = new Icon("../../../Resources/dummy.ico");
                            return new tmpImageItem()
                            {
                                Path = appPath,
                                appIcon = appIcon
                            };
                        }
                    })
                .Where(item => item != null).ToArray();

            ls.Images.Add("Any", new Bitmap("../../../Resources/ANYIcon.png"));
            foreach (var item in ps)
            {
                ls.Images.Add(item.Path, item.appIcon);
            }
            foreach (var item in appListFromReg.OrderBy(item => Path.GetFileName(item.Path)))
            {
                if (!ls.Images.ContainsKey(item.Path))
                {
                    ls.Images.Add(item.Path, item.appIcon);
                }
            }

            appListView.LargeImageList = ls;
            appListView.LargeImageList.ImageSize = new Size(32, 32);
            for (int i = 0; i < appListView.LargeImageList.Images.Count; i++)
            {
                appListView.Items.Add(new ListViewItem()
                {
                    Text = System.IO.Path.GetFileNameWithoutExtension(appListView.LargeImageList.Images.Keys[i]),
                    ImageIndex = i
                });
            }
        }

        string ProcessExecutablePath(Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch
            {
                try
                {
                    if (moc != null)
                    {
                        foreach (ManagementObject item in moc)
                        {
                            object path = item["ExecutablePath"];
                            if (path != null)
                            {
                                return path.ToString();
                            }
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Error: ProcessExecutablePath()");
                }
            }

            return "";
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            UnhookWindowsHookEx(mouseHook);
            UnhookWindowsHookEx(keyHook);
        }

        //--------------------------------------------------------
        // メニュー操作
        //--------------------------------------------------------

        private void openOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog.Filter = "ConnectStudioスクリプト(*.con)|*.con|すべてのファイル(*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                    BinaryFormatter f = new BinaryFormatter();
                    //読み込んで逆シリアル化する
                    appGraph = f.Deserialize(fs) as AppGraph;
                    fs.Close();

                    Debug.Assert(appGraph.edges[0].Src == appGraph.nodes[0] || appGraph.edges[0].Dst == appGraph.nodes[0]);

                    foreach (var node in appGraph.nodes)
                    {
                        // アイコン取得
                        if (appListView.LargeImageList.Images.ContainsKey(node.Path))
                        {
                            node.Icon = appListView.LargeImageList.Images[appListView.LargeImageList.Images.IndexOfKey(node.Path)];
                        }
                        // プロセス情報はデフォルトに設定しなおす
                        node.Any = true;
                        node.Process = new Process();
                    }
                    canvas.Invalidate();
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
        }

        private void saveSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog.Filter = "ConnectStudioスクリプト(*.con)|*.con|すべてのファイル(*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, appGraph);
                    fs.Close();                    
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
        }

        private void exitQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void startAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Text = MainTitleText + "(Running)";
            Hook();
        }

        private void stopAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Text = MainTitleText;
            Unhook();
        }

        //-------------------------------------------------------------
        // キャンバスの描画
        //-------------------------------------------------------------

        AppGraph appGraph = new AppGraph();
        AppNode appendingNode = null;
        readonly Pen edgePen = new Pen(Brushes.Red, 3) { CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(8, 8) };
        readonly Size nodeSize = new Size(64, 64);
        const int AppNodeTitleCharLimit = 10;

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(Color.White);

            // 頂点の描画
            if (appGraph != null)
            {
                // appGraph, appendingNodeの描画
                for (int i = 0; i < appGraph.nodes.Count; i++)
                {
                    AppNode node = appGraph.nodes[i];
                    g.DrawImage(node.Icon, new Rectangle(node.Position, nodeSize));
                    try
                    {
                        if (node.Process != null)
                        {
                            string text = NodeTitle(node);
                            if (text.Length >= AppNodeTitleCharLimit)
                            {
                                text = text.Remove(AppNodeTitleCharLimit - 4) + "...";
                            }
                            SizeF textSize = g.MeasureString(text, fnt);
                            float textX = node.Position.X + nodeSize.Width / 2 - textSize.Width / 2;
                            float textY = node.Position.Y + nodeSize.Height;
                            g.DrawString(text, fnt, Brushes.Black, new PointF(textX, textY));
                        }
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(ee);
                    }
                }
            }

            if (appendingNode != null)
            {
                g.DrawImage(appendingNode.Icon, new Rectangle(appendingNode.Position, nodeSize));
            }

            // 枝の描画
            if (appGraph != null)
            {
                for (int i = 0; i < appGraph.edges.Count; i++)
                {
                    AppEdge edge = appGraph.edges[i];
                    Point p1, p2, p3, p4;
                    EdgePoints(edge.Src, edge.Dst, out p1, out p2, out p3, out p4);

                    if (edge.Src != edge.Dst)
                    {
                        g.DrawLine(edgePen, p3, p4);
                    }
                    else
                    {
                        g.DrawArc(edgePen, new Rectangle(p1.X, p1.Y - 64, 64, 64), 90, -270);
                    }
                    // TODO
                    string text = "";
                    if (edge.ActionHandler.TriggerType == TriggerType.ContextMenu)
                    {
                        text = "\"" + edge.ActionHandler.ContextMenuText + "\"";
                    }
                    else
                    {
                        text += edge.ActionHandler.Ctrl ? "Ctrl + " : "";
                        text += edge.ActionHandler.Win ? "Ctrl + " : "";
                        text += edge.ActionHandler.Fn ? "Fn + " : "";
                        text += edge.ActionHandler.Shift ? "Sfhift + " : "";
                        text += edge.ActionHandler.Alt ? "Alt + " : "";
                        text += edge.ActionHandler.Key;
                    }
                    g.DrawString(text, fnt, Brushes.Black, new PointF((p1.X + p4.X) / 2, (p1.Y + p4.Y) / 2));
                }
            }

            if (appendingEdge != null)
            {
                Point srcPos = new Point(
                    appendingEdge.Src.Position.X + nodeSize.Width / 2,
                    appendingEdge.Src.Position.Y + nodeSize.Height / 2);
                Point dstPos = canvas.PointToClient(Cursor.Position);
                g.DrawLine(edgePen, srcPos, dstPos);
            }

        }
        Font fnt = new Font("MS UI Gothic", 12);

        //-------------------------------------------------------------
        // キャンバスへのマウス操作
        //-------------------------------------------------------------

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (appendingNode != null)
            {
                appendingNode.Position = e.Location;
                canvas.Invalidate();
            }
            if (appendingEdge != null)
            {
                // 
                canvas.Invalidate();
            }
            if (movingNode != null)
            {
                movingNode.Position = e.Location;
                canvas.Invalidate();
            }
        }


        private void canvas_MouseClick(object sender, MouseEventArgs e)
        {
            // コンボボックスを隠す
            editingNode = null;
            processComboBox.Hide();

            // 1クリック1アクション
            if (appendingNode != null)
            {
                appendingNode.Position = e.Location;
                appGraph.nodes.Add(appendingNode);
                appendingNode = null;
                canvas.Invalidate();
                return;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                canvas_MouseLeftClick(sender, e);
                return;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                canvas_MouseRightClick(sender, e);
                return;
            }
        }
 
        private void canvas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 追加中の枝は削除
            appendingEdge = null;
            canvas.Invalidate();

            // ノードとの当たり判定
            if (canvas_checkNodes(sender, e))
            {
                return;
            }

            // 枝の起点を決める
            if (appendingEdge == null && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (canvas_SetEdgePositions(sender, e))
                {
                    return;
                }
            }
        }
 
        private void canvas_MouseLeftClick(object sender, MouseEventArgs e)
        {
            // 枝の終点を決める
                if (canvas_SetEdgePositions(sender, e))
                {
                    return;
                }
            // 枝との当たり判定。あたったら横のパネルに詳細を表示
            if (canvas_checkEdges(sender, e))
            {
                return;
            }
        }

        AppNode editingNode = null;

        bool canvas_checkNodes(object sender, MouseEventArgs e)
        {
            AppNode hitNode = GetAppNodeInCanvas(e.Location);

            // [要修正] ノードを消す
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (hitNode != null)
                {
                    editingNode = null;
                    processComboBox.Hide();
                    appGraph.edges.RemoveAll(edge => edge.Src == hitNode || edge.Dst == hitNode);
                    appGraph.nodes.Remove(hitNode);
                    return true;
                }
            }

            // コンボボックスが出てないときにノードをクリックしたら
            if (processComboBox.Visible == false && hitNode != null)
            {
                editingNode = hitNode;
                processComboBox.Location = new Point(e.X - processComboBox.Width, e.Y);
                processComboBox.Show();
                return true;
            }
            // コンボボックスを隠す
            editingNode = null;
            processComboBox.Hide();
            return false;
        }

        bool canvas_checkEdges(object sender, MouseEventArgs e)
        {
            AppEdge hitEdge = null;
            // 各ノードと当たり判定
            for (int i = 0; i < appGraph.edges.Count; i++)
            {
                AppEdge edge = appGraph.edges[i];
                Point p1, p2, p3, p4;
                EdgePoints(edge.Src, edge.Dst, out p1, out p2, out p3, out p4);

                if (edge.Src == edge.Dst)
                {
                    if (new Rectangle(p1.X, p1.Y - 64, 64, 64).Contains(e.Location))
                    {
                        hitEdge = edge;
                        break;
                    }
                }
                else
                {
                    PointF n = new PointF(
                        p1.Y - p4.Y,
                        p4.X - p1.X);
                    PointF p = new PointF(
                        e.Location.X - p1.X,
                        e.Location.Y - p1.Y);
                    double d = Math.Abs(n.X * p.X + n.Y * p.Y) / Math.Sqrt(n.X * n.X + n.Y * n.Y);
                    if (d <= 4)
                    {
                        hitEdge = edge;
                        break;
                    }
                }
            }

            if (hitEdge != null)
            {
                ShowEdgeDetail(hitEdge);
                return true;
            }
            return false;
        }

        bool canvas_SetEdgePositions(object sender, MouseEventArgs e)
        {
            AppNode hitNode = GetAppNodeInCanvas(e.Location);

            if (hitNode != null)
            {
                if (appendingEdge == null)
                {
                    appendingEdge = new AppEdge();
                    appendingEdge.Src = hitNode;
                }
                else
                {
                    appendingEdge.Dst = hitNode;
                    appendingEdge.ActionHandler.TriggerType = TriggerType.ContextMenu;
                    appendingEdge.ActionHandler.ContextMenuText = "to " + appendingEdge.Dst.ProcessName;
                    appGraph.edges.Add(appendingEdge);
                    ShowEdgeDetail(appendingEdge);
                    appendingEdge = null;
                    canvas.Invalidate();
                }
                return true;
            }
            else
            {
                appendingEdge = null;
                canvas.Invalidate();
            }
            return false;
        }

        AppNode GetAppNodeInCanvas(Point pos)
        {
            AppNode hitNode = null;
            // 各ノードと当たり判定
            for (int i = 0; i < appGraph.nodes.Count; i++)
            {
                AppNode node = appGraph.nodes[i];
                Rectangle rect = new Rectangle(node.Position, nodeSize);
                if (rect.Contains(pos))
                {
                    hitNode = node;
                    break;
                }
            }
            return hitNode;
        }


        //--------------------------------------------------
        // アプリケーションリストへの操作
        //--------------------------------------------------

        private void appListView_Click(object sender, EventArgs e)
        {
            if (appListView.SelectedItems.Count >= 1)
            {
                var item = appListView.SelectedItems[0];

                if (appendingNode != null && appendingNode.ProcessName == item.Text)
                {
                    // トグル
                    appendingNode = null;
                    return;
                }

                var imgKey = appListView.LargeImageList.Images.Keys[item.Index];
                var img = appListView.LargeImageList.Images[item.Index];
                appendingNode = new AppNode()
                {
                    ProcessName = item.Text,
                    Path = imgKey,
                    Icon = img
                };
            }
        }


        // p1, p2は中心から中心
        // p3, p4はトリミング後
        void EdgePoints(AppNode src, AppNode dst, out Point p1, out Point p2, out Point p3, out Point p4)
        {
            p1 = new Point(
                src.Position.X + nodeSize.Width / 2,
                src.Position.Y + nodeSize.Height / 2);
            p2 = new Point(
                dst.Position.X + nodeSize.Width / 2,
                dst.Position.Y + nodeSize.Height / 2);

            p3 = p1;
            p4 = p2;
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if (dx * dx + dy * dy <= 0.0001)
            {
                return;
            }
            float dl = (float)Math.Sqrt(dx * dx + dy * dy);
            float nx = dx / dl;
            float ny = dy / dl;
            float l = nodeSize.Width / 2;

            p3 = p1;
            p3.X += (int)(nx * l);
            p3.Y += (int)(ny * l);

            p4 = p2;
            p4.X -= (int)(nx * l);
            p4.Y -= (int)(ny * l);
        }

        AppNode movingNode = null;

        // ノードの移動
        private void canvas_MouseRightClick(object sender, MouseEventArgs e)
        {
            if (movingNode != null)
            {
                movingNode = null;
                return;
            }

            AppNode hitNode = null;

            // 各ノードと当たり判定
            for (int i = 0; i < appGraph.nodes.Count; i++)
            {
                AppNode node = appGraph.nodes[i];
                Rectangle rect = new Rectangle(node.Position, nodeSize);
                if (rect.Contains(e.Location))
                {
                    hitNode = node;
                    break;
                }
            }

            if (hitNode != null)
            {
                movingNode = hitNode;
            }
        }

        AppEdge appendingEdge = null;
        AppEdge editingEdge = null;

        private void applyActionHandlerButton_Click(object sender, EventArgs e)
        {
            if (editingEdge != null)
            {
                appGraph.edges.Remove(editingEdge);
                ClearEdgeDetail();
                canvas.Invalidate();
            }
            else
            {
                Console.WriteLine("editingEdge is null (Any Edge is not selected)");
            }
        }

        private void keyComboBoxHotKey_TextChanged(object sender, EventArgs e)
        {
            canvas.Invalidate();
        }

        private void processComboBox_DropDown(object sender, EventArgs e)
        {
            if (editingNode == null) return;
            UpdateProcessComboBox(processComboBox, editingNode.ProcessName);
        }

        void UpdateProcessComboBox(ComboBox pComboBox, string processName)
        {
            pComboBox.DataSource = ProcessTable(processName);
            pComboBox.DisplayMember = "NAME";
            pComboBox.ValueMember = "ID";
        }

        private void processComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editingNode == null) return;
            if (processComboBox.SelectedValue is DataRowView) return;
            if (processComboBox.SelectedIndex >= 0)
            {
                int pid = (int)(processComboBox.SelectedValue);
                if (pid != -1)
                {
                    try
                    {
                        editingNode.Any = false;
                        editingNode.Process = Process.GetProcessById(pid);
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(ee);
                    }
                }
                else
                {
                    editingNode.Any = true;
                    editingNode.Process = new Process();
                }
            }
            canvas.Invalidate();
            ShowEdgeDetail(editingEdge);
        }

        bool allowEditEdgeDetail = true;

        private void EdgeDetailChanged(object sender, EventArgs e)
        {
            if (allowEditEdgeDetail)
            {
                editingEdge.ActionHandler = new ActionHandler(this);
                canvas.Invalidate();
            }
        }

        private void newNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            appGraph = new AppGraph();
            canvas.Invalidate();
            ClearEdgeDetail();
            editingEdge = appendingEdge = null;
            editingNode = appendingNode = null;
        }
    }
}