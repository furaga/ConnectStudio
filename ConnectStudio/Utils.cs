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
using System.Runtime.InteropServices;
using System.Management;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ConnectStudio
{
    public partial class Form1 : Form
    {
        IntPtr mouseHook;
        IntPtr keyHook;
        MouseHookHandler mouseHandler;
        KeyHookHandler keyHandler;
        bool onCtrl = false;
        bool onAlt = false;
        bool onShift = false;
        bool onFn = false;
        bool onWin = false;


        //----------------------------------------------------
        // プロセスへの処理
        //----------------------------------------------------

        static Process GetActiveProcess()
        {
            int id;
            IntPtr hWnd = GetForegroundWindow();
            GetWindowThreadProcessId(hWnd, out id);
            Process process = Process.GetProcessById(id);
            return process;
        }

        void ActivateProcess(Process process)
        {
            try
            {
                SetForegroundWindow(process.MainWindowHandle);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // プロセスの種類がprocessNameなものの一覧
        DataTable ProcessTable(string processName)
        {
            Process[] ps = Process.GetProcessesByName(processName);
            DataTable table = new DataTable();
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("ID", typeof(int));
            DataRow row = table.NewRow();
            row["NAME"] = "Any";
            row["ID"] = -1; // ダミーのプロセス
            table.Rows.Add(row);
            foreach (Process p in ps.Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)))
            {
                row = table.NewRow();
                row["NAME"] = p.MainWindowTitle;// p.MainWindowTitle + "(" + p.Id + ")";
                row["ID"] = p.Id;
                table.Rows.Add(row);
            }
            table.AcceptChanges();
            return table;
        }

        //-------------------------------------------------------
        // パソコンに入っているソフトウェアの一覧を取得
        //-------------------------------------------------------

        class tmpImageItem
        {
            public string Path;
            public Icon appIcon;
        }

        // レジストリ内の "App Paths", "Application" キーから出奥
        IEnumerable<tmpImageItem> GetAppList()
        {
            var ls1 = GetAppListInAppPaths();
            var ls2 = GetAppListInApplication();
            return ls1.Concat(ls2);
        }

        List<tmpImageItem> GetAppListInAppPaths()
        {
            var ls = new List<tmpImageItem>();
            string rootPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
            var rootRegKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(rootPath, false);
            string[] keyNames = rootRegKey.GetSubKeyNames();
            SHFILEINFO shinfo = new SHFILEINFO();
            foreach (string keyName in keyNames)
            {
                var appRegKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(Path.Combine(rootPath, keyName));
                if (appRegKey == null)
                {
                    continue;
                }
                try
                {
                    string appPath = appRegKey.GetValue("").ToString();
                    IntPtr hSuccess = SHGetFileInfo(appPath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                    if (hSuccess != IntPtr.Zero)
                    {
                        Icon appIcon = Icon.FromHandle(shinfo.hIcon);
                        ls.Add(new tmpImageItem()
                        {
                            Path = appPath,
                            appIcon = appIcon
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return ls;
        }

        List<tmpImageItem> GetAppListInApplication()
        {
            var ls = new List<tmpImageItem>();
            string rootPath = @"Applications";
            var rootRegKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(rootPath, false);
            string[] keyNames = rootRegKey.GetSubKeyNames();
            SHFILEINFO shinfo = new SHFILEINFO();
            foreach (string keyName in keyNames)
            {
                var appRegKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Path.Combine(rootPath, keyName, @"shell\open\command"));
                if (appRegKey != null)
                {
                    try
                    {
                        string command = appRegKey.GetValue("").ToString();
                        string appPath = command.Split('"')
                            .Where(t => string.IsNullOrWhiteSpace(t) == false)
                            .First();

                        IntPtr hSuccess = SHGetFileInfo(appPath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                        if (hSuccess != IntPtr.Zero)
                        {
                            Icon appIcon = Icon.FromHandle(shinfo.hIcon);
                            ls.Add(new tmpImageItem()
                            {
                                Path = appPath,
                                appIcon = appIcon
                            });
                            Console.WriteLine("\nApp Path = " + appPath);
                            Console.WriteLine(keyName);
                            Console.WriteLine(Path.Combine(rootPath, keyName));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            return ls;
        }

        //-----------------------------------------------------
        // フック開始/終了
        //-----------------------------------------------------

        // キー入力とマウス入力をフック
        public void Hook()
        {
            IntPtr hMod = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);

            UnhookWindowsHookEx(mouseHook);
            UnhookWindowsHookEx(keyHook);
            mouseHandler = new MouseHookHandler(OnMouseLLHook);
            keyHandler = new KeyHookHandler(OnKeyLLHook);
            mouseHook = SetWindowsHookEx(WH.MOUSE_LL, mouseHandler, hMod, 0);
            keyHook = SetWindowsHookEx(WH.KEYBOARD_LL, keyHandler, hMod, 0);

            if (mouseHook == IntPtr.Zero || keyHook == IntPtr.Zero)
            {
                UnhookWindowsHookEx(mouseHook);
                UnhookWindowsHookEx(keyHook);
                int errorCode = Marshal.GetLastWin32Error();
                Console.WriteLine(new Win32Exception(errorCode));
            }
        }

        // キー・マウス入力をアンフック
        public void Unhook()
        {
            UnhookWindowsHookEx(mouseHook);
            UnhookWindowsHookEx(keyHook);
        }

        //-----------------------------------------------------
        // キーのフック処理
        //-----------------------------------------------------
        int OnKeyLLHook(int code, WM wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            UpdateSysKeyStates(wParam, ref lParam);
            switch (wParam)
            {
                case WM.KEYDOWN:
                case WM.SYSKEYDOWN:
                    //                case WM.KEYUP:
                    //                case WM.SYSKEYUP:
                    for (int i = 0; i < appGraph.edges.Count; i++)
                    {
                        AppEdge appEdge = appGraph.edges[i];
                        if (!IsValidEdge(appEdge, TriggerType.HotKey))
                        {
                            continue;
                        }
                        Keys keyCode;
                        if (Enum.TryParse(appEdge.ActionHandler.Key, out keyCode))
                        {
                            // TODO: fnなど
                            if (
                                    (!appEdge.ActionHandler.Ctrl || onCtrl) &&
                                    (!appEdge.ActionHandler.Alt || onAlt) &&
                                    (!appEdge.ActionHandler.Shift || onShift) &&
                                    (!appEdge.ActionHandler.Win || onWin) &&
                                //                              (!appEdge.ActionHandler.Fn || (lParam.vkCode & MOD_WIN) != 0) && 
                                    ((lParam.vkCode & (uint)keyCode) == (uint)keyCode))
                            {
                                try
                                {
                                    ExecuteActionHandler(appEdge);
                                    if (appEdge.ActionHandler.BlockKeyStroke)
                                    {
                                        return 1;
                                    }
                                    else
                                    {
                                        return CallNextHookEx(0, code, wParam, ref lParam);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            }
                        }
                    }
                    break;
            }
            return CallNextHookEx(0, code, wParam, ref lParam);
        }

        void UpdateSysKeyStates(WM wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            switch (wParam)
            {
                case WM.KEYDOWN:
                case WM.SYSKEYDOWN:
                    if (lParam.vkCode == 160 || lParam.vkCode == 161) onShift = true;
                    if (lParam.vkCode == 162 || lParam.vkCode == 163) onCtrl = true;
                    if (lParam.vkCode == 164 || lParam.vkCode == 165) onAlt = true;
                    break;
                case WM.KEYUP:
                case WM.SYSKEYUP:
                    if (lParam.vkCode == 160 || lParam.vkCode == 161) onShift = false;
                    if (lParam.vkCode == 162 || lParam.vkCode == 163) onCtrl = false;
                    if (lParam.vkCode == 164 || lParam.vkCode == 165) onAlt = false;
                    break;
            }
        }
        
        //-----------------------------------------------------
        // マウスのフック処理
        //-----------------------------------------------------

        bool IsValidEdge(AppEdge appEdge, TriggerType triggerType)
        {
            if (appEdge.Src.Process == null || appEdge.Dst.Process == null)
            {
                return false;
            }
            if (appEdge.ActionHandler.TriggerType != triggerType)
            {
                return false;
            }
            Process activeProcess = GetActiveProcess();
            if (appEdge.Src.Path != "Any")
            {
                // 
                if (appEdge.Src.Any)
                {
                    // ソフトの種類のみ見る
                    if (activeProcess.ProcessName != appEdge.Src.ProcessName)
                    {
                        return false;
                    }
                }
                else
                {
                    // プロセスIDまで見る
                    if (activeProcess.Id != appEdge.Src.Process.Id)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        int OnMouseLLHook(int code, WM message, IntPtr state)
        {
            try
            {
                if (message == WM.LBUTTONDOWN)
                {
                    var clientPos = virtualContextMenu.PointToClient(Cursor.Position);
                    if (virtualContextMenu.Visible && virtualContextMenu.ClientRectangle.Contains(clientPos) == false)
                    {
                        virtualContextMenu.Hide();
                    }
                }
                if (message == WM.RBUTTONUP)
                {
                    // コンテキストメニューを表示
                    virtualContextMenu.Items.Clear();

                    for (int i = 0; i < appGraph.edges.Count; i++)
                    {
                        AppEdge appEdge = appGraph.edges[i];
                        if (!IsValidEdge(appEdge, TriggerType.ContextMenu))
                        {
                            continue;
                        }
                        var item = new ToolStripMenuItem(appEdge.ActionHandler.ContextMenuText);
                        item.Click += (s, ee) => ExecuteActionHandler(appEdge);
                        item.Invalidate();
                        virtualContextMenu.Items.Add(item);
                    }

                    if (virtualContextMenu.Items.Count >= 1)
                    {
                        ToolStripMenuItem[] itemList = new ToolStripMenuItem[virtualContextMenu.Items.Count];
                        for (int j = 0; j < virtualContextMenu.Items.Count; j++)
                        {
                            itemList[j] = virtualContextMenu.Items[j] as ToolStripMenuItem;
                        }
                        virtualContextMenu.Show(new Point(Cursor.Position.X, Cursor.Position.Y - 22 * virtualContextMenu.Items.Count));
                    }
                }
                return CallNextHookEx(mouseHook, code, message, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return CallNextHookEx(mouseHook, code, message, state);
            }
        }

        //------------------------------------------------
        // スクリプトの実行
        //------------------------------------------------

        void ExecuteActionHandler(AppEdge appEdge)
        {
            try
            {
                ScriptEngine engine = new ScriptEngine();
                Session session = engine.CreateSession();
                ExecuteBeforeTransition(session, appEdge);
                ExecuteAfterTransition(session, appEdge);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void ExecuteBeforeTransition(Session session, AppEdge appEdge)
        {
            session.AddReference("System");
            session.AddReference("System.Drawing");
            session.AddReference("System.Windows.Forms");
            session.Execute("using System;");
            session.Execute("using System.Drawing;");
            session.Execute("using System.Collections.Generic;");
            session.Execute("using System.Windows.Forms;");
            session.Execute(appEdge.ActionHandler.srcScript);
        }

        void ExecuteAfterTransition(Session session, AppEdge appEdge)
        {
            Process targetProcess = null;

            if (appEdge.Dst.Any)
            {
                Process[] ps = Process.GetProcessesByName(appEdge.Dst.ProcessName).Where(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle)).ToArray();
                if (ps.Length >= 1)
                {
                    targetProcess = ps.First();
                }
            }
            else
            {
                targetProcess = appEdge.Dst.Process;
            }

            if (targetProcess != null)
            {
                ActivateProcess(targetProcess);
            }

            if (appEdge.Dst.Path == "Any" || targetProcess != null)
            {
                session.Execute(appEdge.ActionHandler.dstScript);
            }
        }
            

        //----------------------------------------------------------
        // アプリケーション間の連携を表すグラフ構造
        //----------------------------------------------------------
        [Serializable()]
        public class AppGraph
        {
            public List<AppNode> nodes = new List<AppNode>();
            public List<AppEdge> edges = new List<AppEdge>();
        }
        [Serializable()]
        public class AppNode
        {
            public string ProcessName;
            public string Path;
            [NonSerialized()]
            public Image Icon;
            public Point Position;
            [NonSerialized()]
            public Process Process;
            public bool Any;
            public AppNode()
            {
                Any = true;
                Process = new Process();
            }
        }
        [Serializable()]
        public class AppEdge
        {
            public string Text;
            public AppNode Src;
            public AppNode Dst;
            public ActionHandler ActionHandler;
            public AppEdge()
            {
                ActionHandler = new ActionHandler();
            }
        }

        // スクリプト実行のためのハンドラ情報
        [Serializable()]
        public class ActionHandler
        {
            public TriggerType TriggerType;
            public string ContextMenuText;
            public bool Ctrl;
            public bool Alt;
            public bool Win;
            public bool Shift;
            public bool Fn;
            public string Key;
            public bool BlockKeyStroke;
            public string srcScript;
            public string dstScript;

            public ActionHandler()
            {
                TriggerType = TriggerType.HotKey;
                ContextMenuText = "";
                Key = "";
                srcScript = "";
                dstScript = "";
            }

            public ActionHandler(Form1 mainForm)
            {
                TriggerType = mainForm.radioContextMenu.Checked ? TriggerType.ContextMenu : TriggerType.HotKey;
                ContextMenuText = mainForm.textBoxContextMenu.Text;
                Ctrl = mainForm.CtrlCheckBoxHotKey.Checked;
                Alt = mainForm.AltCheckBoxHotKey.Checked;
                Win = mainForm.WinCheckBoxHotKey.Checked;
                Shift = mainForm.ShiftCheckBoxHotKey.Checked;
                Fn = mainForm.FnCheckBoxHotKey.Checked;
                Key = mainForm.keyComboBoxHotKey.Text;
                BlockKeyStroke = mainForm.BlockKeyStrokeCheckBox.Checked;
                srcScript = mainForm.srcScript.Text;
                dstScript = mainForm.dstScript.Text;
            }
        }

        // AppEdgeの内容をGUIに表示

        void ClearEdgeDetail()
        {
            flowLayoutPanel1.Hide();

            allowEditEdgeDetail = false;

            editingEdge = null;

            srcIconTitle.Image = null;
            srcIconTitle.Text = "";
            radioContextMenu.Checked = true;
            textBoxContextMenu.Text = "";
            radioHotKey.Checked = false;
            CtrlCheckBoxHotKey.Checked = false;
            AltCheckBoxHotKey.Checked = false; 
            WinCheckBoxHotKey.Checked = false; 
            ShiftCheckBoxHotKey.Checked = false; 
            FnCheckBoxHotKey.Checked = false; 
            keyComboBoxHotKey.Text = "";
            BlockKeyStrokeCheckBox.Checked = false;
            srcScript.Text = "";

            dstIconTitle.Image = null;
            dstIconTitle.Text = "";
            dstScript.Text = "";

            allowEditEdgeDetail = true;
        }

        string NodeTitle(AppNode appNode)
        {
            try
            {
                if (appNode.Any)
                {
                    return "Any";
                }
                Process p = appNode.Process;
                return p.MainWindowTitle;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
            return "";
        }
        
        void ShowEdgeDetail(AppEdge edge)
        {
            flowLayoutPanel1.Show();
            if (edge == null) return;

            allowEditEdgeDetail = false;

            editingEdge = edge;

            srcIconTitle.Image = edge.Src.Icon;
            srcIconTitle.Text = edge.Src.ProcessName + ": " + NodeTitle(edge.Src);
            radioContextMenu.Checked = edge.ActionHandler.TriggerType == TriggerType.ContextMenu;
            textBoxContextMenu.Text = edge.ActionHandler.ContextMenuText;
            radioHotKey.Checked = edge.ActionHandler.TriggerType == TriggerType.HotKey;
            CtrlCheckBoxHotKey.Checked = edge.ActionHandler.Ctrl;
            AltCheckBoxHotKey.Checked = edge.ActionHandler.Alt;
            WinCheckBoxHotKey.Checked = edge.ActionHandler.Win;
            ShiftCheckBoxHotKey.Checked = edge.ActionHandler.Shift;
            FnCheckBoxHotKey.Checked = edge.ActionHandler.Fn;
            keyComboBoxHotKey.Text = edge.ActionHandler.Key;
            BlockKeyStrokeCheckBox.Checked = edge.ActionHandler.BlockKeyStroke;
            srcScript.Text = edge.ActionHandler.srcScript;

            dstIconTitle.Image = edge.Dst.Icon;
            dstIconTitle.Text = edge.Dst.ProcessName + ": " + NodeTitle(edge.Dst);;
            dstScript.Text = edge.ActionHandler.dstScript;

            allowEditEdgeDetail = true;
        }
    }
}