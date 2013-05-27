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
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId); public const uint MOD_ALT = 0x1;
        // SHGetFileInfo関数
        [DllImport("shell32.dll")]
        static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        // SHGetFileInfo関数で使用する構造体
        struct SHFILEINFO
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

        public enum TriggerType { ContextMenu, HotKey }

        delegate int MouseHookHandler(int code, WM message, IntPtr state);
        delegate int KeyHookHandler(int nCode, WM wParam, ref KBDLLHOOKSTRUCT lParam);

        // SHGetFileInfo関数で使用するフラグ
        const uint SHGFI_ICON = 0x100; // アイコン・リソースの取得
        const uint SHGFI_LARGEICON = 0x0; // 大きいアイコン
        const uint SHGFI_SMALLICON = 0x1; // 小さいアイコン

        const string MainTitleText = "Connect Studio";
    }
}
