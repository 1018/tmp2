using System;
using System.IO;
using System.Web;
using System.Windows.Forms;


namespace CommonClassLibrary
{
    public static class AppSetting
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        static AppSetting()
        {
            AbsolutePathGenerator generator = new AbsolutePathGenerator(Application.StartupPath);
            LogPath = generator.ToAbsolutePath(@"..\Log");
            SymbolPath = generator.ToAbsolutePath(@"..\Data\Symbol");
            ImagePath = generator.ToAbsolutePath(@"..\Data\Bmp");

            // ディレクトリが無ければ作っておく
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }

            if (!Directory.Exists(SymbolPath))
            {
                Directory.CreateDirectory(SymbolPath);
            }

            if (!Directory.Exists(ImagePath))
            {
                Directory.CreateDirectory(ImagePath);
            }
        }

        /// <summary>
        /// 各Symbolのnamespace
        /// </summary>
        public const string SymbolNameSpace = "SymbolLibrary";

        /// <summary>
        /// ログ出力パス
        /// </summary>
        public static string LogPath
        {
            get;
            set;
        }

        /// <summary>
        /// シンボルファイルパス
        /// </summary>
        public static string SymbolPath
        {
            get;
            set;
        }

        /// <summary>
        /// 画像ファイルパス
        /// </summary>
        public static string ImagePath
        {
            get;
            set;
        }
    }
}

