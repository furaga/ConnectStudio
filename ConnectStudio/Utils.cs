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
                row["NAME"] = ProcessTitle(p);// p.MainWindowTitle + "(" + p.Id + ")";
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
        ImageList GetAppList()
        {
            var ls1 = GetAppListInAppPaths();
            var ls2 = GetAppListInApplication();
            ImageList ls = new ImageList();
            ls.Images.Add("Any", new Bitmap("../../../../Resources/ANYIcon.png"));
            foreach (var item in ls1.Concat(ls2).OrderBy(item => Path.GetFileName(item.Path)))
            {
                if (!ls.Images.ContainsKey(item.Path))
                {
                    ls.Images.Add(item.Path, item.appIcon);
                }
            }
            return ls;
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
                        // プロセスが動いてなければ
                        if (appEdge.Src.Process == null || appEdge.Dst.Process == null)
                        {
                            continue;
                        }
                        if (appEdge.ActionHandler.TriggerType != TriggerType.HotKey)
                        {
                            continue;
                        }
                        Process activeProcess = GetActiveProcess();
                        if (appEdge.Src.Path != "Any" && activeProcess.Id != appEdge.Src.Process.Id)
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
        
        int OnMouseLLHook(int code, WM message, IntPtr state)
        {
            try
            {
                if (message == WM.LBUTTONDOWN)
                {
                    var clientPos = virtualContextMenu.PointToClient(Cursor.Position);
                    if (virtualContextMenu.Visible && virtualContextMenu.ClientRectangle.Contains(clientPos) == false)
                    {
                    }
                }
                if (message == WM.RBUTTONUP)
                {
                    // コンテキストメニューを表示
                    virtualContextMenu.Items.Clear();

                    //

                    for (int i = 0; i < appGraph.edges.Count; i++)
                    {

                    }





                    if (virtualContextMenu.Items.Count >= 1)
                    {
                        virtualContextMenu.Hide();
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
                Stopwatch sw = Stopwatch.StartNew();
                ScriptEngine engine = new ScriptEngine();
                Session session = engine.CreateSession();

                session.AddReference("System");
                session.AddReference("System.Drawing");
                session.AddReference("System.Windows.Forms");
                session.Execute("using System;");
                session.Execute("using System.Collections.Generic;");
                session.Execute("using System.Windows.Forms;");
                Console.WriteLine("Init: " + sw.Elapsed.TotalSeconds + " s");
                sw.Restart();

                session.Execute(appEdge.ActionHandler.srcScript);
                Console.WriteLine("Execute Src: " + sw.Elapsed.TotalSeconds + " s");
                sw.Restart();

                ActivateProcess(appEdge.Dst.Process);
                Console.WriteLine("Activate Dst: " + sw.Elapsed.TotalSeconds + " s");
                sw.Restart();

                session.Execute(appEdge.ActionHandler.dstScript);
                Console.WriteLine("Execute Dst: " + sw.Elapsed.TotalSeconds + " s");
                sw.Restart();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //----------------------------------------------------------
        // アプリケーション間の連携を表すグラフ構造
        //----------------------------------------------------------

        public class AppGraph
        {
            public List<AppNode> nodes = new List<AppNode>();
            public List<AppEdge> edges = new List<AppEdge>();
        }

        public class AppNode
        {
            public string Text { get; set; }
            public string Path { get; set; }
            [System.Xml.Serialization.XmlIgnoreAttribute]
            public Image Icon { get; set; }
            public Point Position { get; set; }
            [System.Xml.Serialization.XmlIgnoreAttribute]
            public Process Process { get; set; }
        }

        public class AppEdge
        {
            public string Text { get; set; }
            public AppNode Src { get; set; }
            public AppNode Dst { get; set; }
            public ActionHandler ActionHandler { get; set; }
            public AppEdge()
            {
                ActionHandler = new ActionHandler();
            }
        }

        // スクリプト実行のためのハンドラ情報
        public class ActionHandler
        {
            public TriggerType TriggerType { get; set; }
            public string ContextMenuText { get; set; }
            public bool Ctrl { get; set; }
            public bool Alt { get; set; }
            public bool Win { get; set; }
            public bool Shift { get; set; }
            public bool Fn { get; set; }
            public string Key { get; set; }
            public bool BlockKeyStroke { get; set; }
            public string srcScript { get; set; }
            public string dstScript { get; set; }

            public ActionHandler()
            {
                TriggerType = TriggerType.HotKey;
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

        void ClearEdgeDetail(AppEdge edge)
        {
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

        string ProcessTitle(Process p)
        {
            try
            {
                if (p != null)
                {
                    if (string.IsNullOrEmpty(p.MainWindowTitle))
                    {
                        return "";
                    }
                    else
                    {
                        return p.MainWindowTitle;
                    }
                }
            }
            catch (Exception ee)
            {
                return "Any";
                //                Console.WriteLine(ee);
            }
            return "";
        }
        
        void ShowEdgeDetail(AppEdge edge)
        {
            if (edge == null) return;

            allowEditEdgeDetail = false;

            editingEdge = edge;

            srcIconTitle.Image = edge.Src.Icon;
            srcIconTitle.Text = edge.Src.Text + ": " + ProcessTitle(edge.Src.Process);
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
            dstIconTitle.Text = edge.Dst.Text + ": " + ProcessTitle(edge.Dst.Process);;
            dstScript.Text = edge.ActionHandler.dstScript;

            allowEditEdgeDetail = true;
        }
    }
}