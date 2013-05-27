using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ConnectStudio
{
    public partial class Form1 : Form
    {
        const string MainTitleText = "Connect Studio";

        public Form1()
        {
            InitializeComponent();
            canvas.AllowDrop = true;
            appListView.LargeImageList = GetAppList();
            appListView.LargeImageList.ImageSize = new Size(32, 32);
            for (int i = 0; i < appListView.LargeImageList.Images.Count; i++)
            {
                appListView.Items.Add(new ListViewItem()
                {
                    Text = System.IO.Path.GetFileNameWithoutExtension(appListView.LargeImageList.Images.Keys[i]),
                    ImageIndex = i
                });
            }
            keyComboBoxHotKey.Items.AddRange(Enum.GetNames(typeof(Keys)).OrderBy(s => s).ToArray());
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            UnhookWindowsHookEx(mouseHook);
            UnhookWindowsHookEx(keyHook);
        }

        private void canvas_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void canvas_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            for (int i = 0; i < s.Length; i++)
            {
                Console.WriteLine(s[i]);
            }
        }

        AppGraph appGraph = new AppGraph();
        AppNode appendingNode = null;
        readonly Pen edgePen = new Pen(Brushes.Red, 8)
            {
                EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor
            };
        readonly Size nodeSize = new Size(64, 64);
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
                        g.DrawLine(edgePen, p1, p4);
                    }
                    else
                    {
                        g.DrawArc(edgePen, new Rectangle(p1.X, p1.Y - 64, 64, 64), 90, -270);
                    }
                    // TODO
                    string text = edge.ActionHandler.Ctrl ? "Ctrl + " : "";
                    text += edge.ActionHandler.Win ? "Ctrl + " : "";
                    text += edge.ActionHandler.Fn ? "Fn + " : "";
                    text += edge.ActionHandler.Shift ? "Sfhift + " : "";
                    text += edge.ActionHandler.Alt ? "Alt + " : "";
                    text += edge.ActionHandler.Key;
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

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (appendingNode != null)
            {
                appendingNode.Position = e.Location;
                appGraph.nodes.Add(appendingNode);
                appendingNode = null;
                canvas.Invalidate();
            }
        }

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

        private void openOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //保存元のファイル名
                openFileDialog1.AddExtension = true;
                openFileDialog1.DefaultExt = "*.config;*.config";
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(AppGraph));
                    System.IO.FileStream fs = new System.IO.FileStream(openFileDialog1.FileName, System.IO.FileMode.Open);
                    appGraph = (AppGraph)serializer.Deserialize(fs);
                    fs.Close();

                    // アイコン取得
                    foreach (var node in appGraph.nodes)
                    {
                        if (appListView.LargeImageList.Images.ContainsKey(node.Path))
                        {
                            node.Icon = appListView.LargeImageList.Images[appListView.LargeImageList.Images.IndexOfKey(node.Path)];
                        }
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
                saveFileDialog1.AddExtension = true;
                saveFileDialog1.DefaultExt = "*.config;*.config";
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(AppGraph));
                    using (System.IO.FileStream fs = new System.IO.FileStream(saveFileDialog1.FileName, System.IO.FileMode.Create))
                    {
                        serializer.Serialize(fs, appGraph);
                    }
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
            /*
            for (int i = 0; i < appGraph.nodes.Count; i++)
            {
                AppNode node = appGraph.nodes[i];
                if (node.Path != "Any")
                {
                    Process process = new Process();
                    process.StartInfo.FileName = node.Path;
                    process.StartInfo.UseShellExecute = false;
                    process.EnableRaisingEvents = true;
                    process.Start();
                    node.Process = process;
                }
            }
            */
            Text = MainTitleText + "(Running)";
            Hook();
        }

        private void stopAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Text = MainTitleText;
            Unhook();
        }

        private void appListView_Click(object sender, EventArgs e)
        {
            if (appListView.SelectedItems.Count >= 1)
            {
                var item = appListView.SelectedItems[0];

                if (appendingNode != null && appendingNode.Text == item.Text)
                {
                    // トグル
                    appendingNode = null;
                    return;
                }

                var imgKey = appListView.LargeImageList.Images.Keys[item.Index];
                var img = appListView.LargeImageList.Images[item.Index];
                appendingNode = new AppNode()
                {
                    Text = item.Text,
                    Path = imgKey,
                    Icon = img
                };
            }
        }

        private void canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                canvas_MouseLeftClick(sender, e);
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                canvas_MouseRightClick(sender, e);
            }
        }

        // 枝の起点を決める
        private void canvas_MouseLeftClick(object sender, MouseEventArgs e)
        {
            canvas_checkEdges(sender, e);
            canvas_SetEdgePositions(sender, e);
        }

        private void canvas_SetEdgePositions(object sender, MouseEventArgs e)
        {
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
                if (appendingEdge == null)
                {
                    appendingEdge = new AppEdge();
                    appendingEdge.Src = hitNode;
                }
                else
                {
                    appendingEdge.Dst = hitNode;
                    appGraph.edges.Add(appendingEdge);
                    ShowEdgeDetail(appendingEdge);
                    appendingEdge = null;
                    canvas.Invalidate();
                }
            }
            else
            {
                appendingEdge = null;
                canvas.Invalidate();
            }
        }

        void canvas_checkEdges(object sender, MouseEventArgs e)
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
                    if (d <= edgePen.Width / 2)
                    {
                        hitEdge = edge;
                        break;
                    }
                }
            }

            if (hitEdge != null)
            {
                ShowEdgeDetail(hitEdge);
            }
        }

        void EdgePoints(AppNode src, AppNode dst, out Point p1, out Point p2, out Point p3, out Point p4)
        {

            p1 = new Point(
                src.Position.X + nodeSize.Width / 2,
                src.Position.Y + nodeSize.Height / 2);
            p2 = p3 = Point.Empty;
            p4 = new Point(
                dst.Position.X + nodeSize.Width / 2,
                dst.Position.Y + nodeSize.Height / 2);
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
                editingEdge.ActionHandler = new ActionHandler(this);
            }
            else
            {
                Console.WriteLine("editingEdge is null (Any Edge is not selected)");
            }
        }

        private void keyComboBoxHotKey_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void keyComboBoxHotKey_TextChanged(object sender, EventArgs e)
        {
            canvas.Invalidate();
        }
        
        private void srcProcessComboBox_DropDown(object sender, EventArgs e)
        {
            if (editingEdge == null || editingEdge.Src == null) return;
            string pName = System.IO.Path.GetFileNameWithoutExtension(editingEdge.Src.Path);
            UpdateProcessComboBox(srcProcessComboBox, pName);
        }

        private void dstProcessComboBox_DropDown(object sender, EventArgs e)
        {
            if (editingEdge == null || editingEdge.Dst == null) return;
            string pName = System.IO.Path.GetFileNameWithoutExtension(editingEdge.Dst.Path);
            UpdateProcessComboBox(dstProcessComboBox, pName);
        }

        void UpdateProcessComboBox(ComboBox pComboBox, string processName)
        {
            pComboBox.DataSource = ProcessTable(processName);
            pComboBox.DisplayMember = "NAME";
            pComboBox.ValueMember = "ID";
        }

        private void srcProcessComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editingEdge == null || editingEdge.Src == null) return;
            if (srcProcessComboBox.SelectedValue is DataRowView) return;
            if (srcProcessComboBox.SelectedIndex >= 0)
            {
                int pid = (int)(srcProcessComboBox.SelectedValue);
                if (pid != -1)
                {
                    try
                    {
                        editingEdge.Src.Process = Process.GetProcessById(pid);
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(ee);
                    }
                }
            }
        }

        private void dstProcessComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editingEdge == null || editingEdge.Dst == null) return;
            if (dstProcessComboBox.SelectedValue is DataRowView) return;
            if (dstProcessComboBox.SelectedIndex >= 0)
            {
                int pid = (int)(dstProcessComboBox.SelectedValue);
                if (pid != -1)
                {
                    try
                    {
                        editingEdge.Dst.Process = Process.GetProcessById(pid);
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(ee);
                    }
                }
            }
        }
    }
}