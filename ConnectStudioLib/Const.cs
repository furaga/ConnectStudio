using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectStudioLib
{
    class Const
    {
        // SHGetFileInfo関数で使用するフラグ
        public const uint SHGFI_ICON = 0x100; // アイコン・リソースの取得
        public const uint SHGFI_LARGEICON = 0x0; // 大きいアイコン
        public const uint SHGFI_SMALLICON = 0x1; // 小さいアイコン
        public const string MainTitleText = "Connect Studio";
    }
}
