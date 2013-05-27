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
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(WH hookType, MouseHookHandler hookDelegate, IntPtr module, uint threadId);
        [DllImport("user32.dll", EntryPoint = "SetWindowsHookExA", CharSet = CharSet.Ansi)]
        static extern IntPtr SetWindowsHookEx(WH idHook, KeyHookHandler lpfn, IntPtr module, uint dwThreadId);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hook, int code, WM message, IntPtr state);
        [DllImport("user32.dll", EntryPoint = "CallNextHookEx", CharSet = CharSet.Ansi)]
        static extern int CallNextHookEx(int hook, int code, WM wParam, ref KBDLLHOOKSTRUCT lParam);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId); public const uint MOD_ALT = 0x1;
        // SHGetFileInfo関数
        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        // SHGetFileInfo関数で使用する構造体
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        public enum WH
        {
            KEYBOARD_LL = 13,
            MOUSE_LL = 14,
        }
        public enum WM
        {
            KEYDOWN = 0x0100,
            KEYUP = 0x0101,
            SYSKEYDOWN = 0x0104,
            SYSKEYUP = 0x0105,
            MOUSEMOVE = 0x0200,
            LBUTTONDOWN = 0x0201,
            LBUTTONUP = 0x0202,
            LBUTTONDBLCLK = 0x0203,
            RBUTTONDOWN = 0x0204,
            RBUTTONUP = 0x0205,
            RBUTTONDBLCLK = 0x0206,
            MBUTTONDOWN = 0x0207,
            MBUTTONUP = 0x0208,
            MBUTTONDBLCLK = 0x0209,
            MOUSEWHEEL = 0x020A,
            XBUTTONDOWN = 0x020B,
            XBUTTONUP = 0x020C,
            XBUTTONDBLCLK = 0x020D,
            MOUSEHWHEEL = 0x020E,
        }
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            int scanCode;
            public int flags;
            int time;
            int dwExtraInfo;
        }

        // SHGetFileInfo関数で使用するフラグ
        private const uint SHGFI_ICON = 0x100; // アイコン・リソースの取得
        private const uint SHGFI_LARGEICON = 0x0; // 大きいアイコン
        private const uint SHGFI_SMALLICON = 0x1; // 小さいアイコン

        const uint MOD_CONTROL = 0x2;
        const uint MOD_SHIFT = 0x4;
        const uint MOD_WIN = 0x8;
        const uint MOD_NOREPEAT = 0x4000; // Windows 7 以降
        delegate int MouseHookHandler(int code, WM message, IntPtr state);
        delegate int KeyHookHandler(int nCode, WM wParam, ref KBDLLHOOKSTRUCT lParam);
        IntPtr mouseHook;
        IntPtr keyHook;
        MouseHookHandler mouseHandler;
        KeyHookHandler keyHandler;

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
        public void Unhook()
        {
            UnhookWindowsHookEx(mouseHook);
            UnhookWindowsHookEx(keyHook);
        }

        bool onCtrl = false;
        bool onAlt = false;
        bool onShift = false;
        bool onFn = false;
        bool onWin = false;

        void printSystemKey()
        {
            Console.WriteLine("==================");
            Console.WriteLine("ctrl: " + (onCtrl ? "pushing" : "releasing"));
            Console.WriteLine("alt: " + (onAlt ? "pushing" : "releasing"));
            Console.WriteLine("shift: " + (onShift ? "pushing" : "releasing"));
            Console.WriteLine("fn: " + (onFn ? "pushing" : "releasing"));
            Console.WriteLine("win: " + (onWin ? "pushing" : "releasing"));
            Console.WriteLine("==================");
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

        static Process GetActiveProcess()
        {
            // アクティブなウィンドウハンドルの取得
            IntPtr hWnd = GetForegroundWindow();
            int id;
            // ウィンドウハンドルからプロセスIDを取得
            GetWindowThreadProcessId(hWnd, out id);
            Process process = Process.GetProcessById(id);
            return process;
        }
        // 指定したファイル名のプロセスをアクティブにする
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

        // アプリケーションのパス名とアイコンが入ったListViewItemのリスト

        class tmpImageItem
        {
            public string Path;
            public Icon appIcon;
        }

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
                //Console.WriteLine(keyName);
                //Console.WriteLine(Path.Combine(rootPath, keyName));
                var appRegKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(Path.Combine(rootPath, keyName));
                if (appRegKey != null)
                {
                    try
                    {
                        string appPath = appRegKey.GetValue("").ToString();
                        //                       Console.WriteLine(appPath);
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

        public enum TriggerType { ContextMenu, HotKey }

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

        void ShowEdgeDetail(AppEdge edge)
        {
            editingEdge = edge;

            srcIconTitle.Image = edge.Src.Icon;
            srcIconTitle.Text = edge.Src.Text;
            srcProcessComboBox_DropDown(null, null);
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
            dstIconTitle.Text = edge.Dst.Text;
            dstProcessComboBox_DropDown(null, null);
            dstScript.Text = edge.ActionHandler.dstScript;
        }

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
                row["NAME"] = p.MainWindowTitle + "(" + p.Id + ")";
                row["ID"] = p.Id;
                table.Rows.Add(row);
            }
            table.AcceptChanges();
            return table;
        }
    }
}