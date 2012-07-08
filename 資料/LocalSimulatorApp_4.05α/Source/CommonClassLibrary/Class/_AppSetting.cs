using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Windows.Forms;


namespace CommonClassLibrary
{
    // INIファイルのセクションとキーを定義する
    public static class SettingSection
    {
        // セクション
        public const string SectionName  = "PLC_LocalSimulator";

        // キー
        public const string BaseFormPath = "BaseFormPath";
        public const string LogPath      = "LogPath";
        public const string MakerName    = "MakerName";
        public const string PlcPath      = "PlcPath";
        public const string SymbolPath   = "SymbolPath";
    }


    public static class AppSetting
    {
        const string _SymbolNameSpace    = "SymbolLibrary";
        const int _BaseFormNameMaxLength = 30;
        const string _IniFileName        = "Setting.ini";

        static Dictionary<string, string> DefaultSetting;
        static Dictionary<string, string> Setting;

        static string _IniFilePath;
        static IniFileManager _IniFile;


        static AppSetting()
        {
            // メンバ初期化
            DefaultSetting = new Dictionary<string, string>();
            Setting = new Dictionary<string, string>();

            //各種初期設定
            #region BaseFormデフォルトパス
            {
                Uri u1 = new Uri(Application.StartupPath + "\\");
                Uri u2 = new Uri(u1, "..\\Data\\BaseForm");
                SetDictionary(DefaultSetting, SettingSection.BaseFormPath, u2.LocalPath);
            }
            #endregion
            #region Symbolデフォルトパス
            {
                Uri u1 = new Uri(Application.StartupPath + "\\");
                Uri u2 = new Uri(u1, "..\\Data\\Symbol");
                SetDictionary(DefaultSetting, SettingSection.SymbolPath, u2.LocalPath);
            }
            #endregion
            #region PLCデフォルトパス
            {
                SetDictionary(DefaultSetting, SettingSection.PlcPath, Application.StartupPath);
            }
            #endregion
            #region Logデフォルトパス
            {
                Uri u1 = new Uri(Application.StartupPath + "\\");
                Uri u2 = new Uri(u1, "..\\Log");
                SetDictionary(DefaultSetting, SettingSection.LogPath, u2.LocalPath);
            }
            #endregion

            _IniFilePath = Application.StartupPath + "\\" + _IniFileName;
            _IniFile = new IniFileManager(_IniFilePath);

            InitialRead();
        }


        public static string BaseFormPath
        {
            get
            {
                return GetDictionary(Setting, SettingSection.BaseFormPath);
            }
            set
            {
                SetDictionary(Setting, SettingSection.BaseFormPath, value);
            }
        }

        public static string LogPath
        {
            get
            {
                return GetDictionary(Setting, SettingSection.LogPath);
            }
            set
            {
                SetDictionary(Setting, SettingSection.LogPath, value);
            }
        }

        public static string MakerName
        {
            get
            {
                return GetDictionary(Setting, SettingSection.MakerName);
            }
            set
            {
                SetDictionary(Setting, SettingSection.MakerName, value);
            }
        }

        public static string PlcPath
        {
            get
            {
                return GetDictionary(Setting, SettingSection.PlcPath);
            }
            set
            {
                SetDictionary(Setting, SettingSection.PlcPath, value);
            }
        }

        public static string SymbolPath
        {
            get
            {
                return GetDictionary(Setting, SettingSection.SymbolPath);
            }
            set
            {
                SetDictionary(Setting, SettingSection.SymbolPath, value);
            }
        }


        public static int BaseFormNameMaxLength
        {
            get
            {
                return _BaseFormNameMaxLength;
            }
        }

        public static string SymbolNameSpace
        {
            get
            {
                return _SymbolNameSpace;
            }
        }


        public static string IniFileName
        {
            get
            {
                return _IniFileName;
            }
        }


        public static bool IsExistsIni()
        {
            return File.Exists(_IniFilePath);
        }

        public static bool GetDefault(string keyName, ref string readString)
        {
            string Default = GetDictionary(DefaultSetting, keyName);

            if (Default == null)
            {
                return false;
            }
            else
            {
                readString = Default;
                return true;
            }
        }

        public static bool ReadIni(string sectionName, string keyName, ref string readString)
        {
            return _IniFile.GetStr(sectionName, keyName, ref readString);
        }

        public static bool WriteIni(string sectionName, string keyName, string writeString)
        {
            return _IniFile.SetStr(sectionName, keyName, writeString);
        }

        //
        // INIファイルからアプリケーションの共通の設定をまとめて読み込む。
        //
        //
        // 戻り値：読取りエラーが発生したキー名
        //
        public static string[] ReadSetting()
        {
            string Section = SettingSection.SectionName;
            string ReadString = string.Empty;
            bool Result = false;
            List<string> ErrorKey = new List<string>();

            #region BaseFormPath読取り
            Result = ReadIni(Section, SettingSection.BaseFormPath, ref ReadString);
            if (Result)
            {
                BaseFormPath = ToFullPath(ReadString);
            }
            else
            {
                ErrorKey.Add(SettingSection.BaseFormPath);
            }
            #endregion

            #region SymbolPath読取り
            Result = ReadIni(Section, SettingSection.SymbolPath, ref ReadString);
            if (Result)
            {
                SymbolPath = ToFullPath(ReadString);
            }
            else
            {
                ErrorKey.Add(SettingSection.SymbolPath);
            }
            #endregion

            #region PlcPath読取り
            //Result = ReadIni(Section, StandardSection.PlcPath, ref ReadString);
            //if (Result)
            //{
            //    PlcPath = ToFullPath(ReadString);
            //}
            //else
            //{
            //    ErrorSection.Add(StandardSection.PlcPath);
            //}
            PlcPath = GetDictionary(DefaultSetting, SettingSection.PlcPath);
            #endregion

            #region MakerNeme読取り
            Result = ReadIni(Section, SettingSection.MakerName, ref ReadString);
            if (Result)
            {
                MakerName = ReadString;
            }
            else
            {
                ErrorKey.Add(SettingSection.MakerName);
            }
            #endregion

            #region LogPath読取り
            //Result = ReadIni(Section, StandardSection.LogPath, ref ReadString);
            //if (Result)
            //{
            //    LogPath = ToFullPath(ReadString);
            //}
            //else
            //{
            //    ErrorKey.Add(StandardSection.LogPath);
            //}
            LogPath = GetDictionary(DefaultSetting, SettingSection.LogPath);
            #endregion

            return ErrorKey.ToArray();
        }

        //
        // INIファイルにアプリケーションの共通の設定をまとめて書き込む。
        //
        //
        // 戻り値：書込みエラーが発生したキー名
        //
        public static string[] WriteSetting()
        {
            string Section = SettingSection.SectionName;
            bool Result = false;
            List<string> ErrorKey = new List<string>();

            #region BaseFormPath書込み

            Result = WriteIni(Section, SettingSection.BaseFormPath, ToRelativePath(BaseFormPath));
            if (!Result)
                ErrorKey.Add(SettingSection.BaseFormPath);

            #endregion

            #region SymbolPath書込み

            Result = WriteIni(Section, SettingSection.SymbolPath, ToRelativePath(SymbolPath));
            if (!Result)
                ErrorKey.Add(SettingSection.SymbolPath);

            #endregion

            #region PlcPath書込み

            //Result = WriteIni(Section, StandardSection.PlcPath, ToRelativePath(PlcPath));
            //if (!Result)
            //    ErrorKey.Add(StandardSection.PlcPath);

            #endregion

            #region MakerNeme書込み

            Result = WriteIni(Section, SettingSection.MakerName, MakerName);
            if (!Result)
                ErrorKey.Add(SettingSection.MakerName);

            #endregion

            #region LogPath書込み

            //Result = WriteIni(Section, StandardSection.LogPath, ToRelativePath(LogPath));
            //if (!Result)
            //    ErrorKey.Add(StandardSection.LogPath);

            #endregion

            return ErrorKey.ToArray();
        }


        private static void InitialRead()
        {
            ReadSetting();
        }

        private static string GetDictionary(Dictionary<string, string> dic, string key)
        {
            string value;

            if (dic.TryGetValue(key, out value))
            {
                return value;
            }

            return null;
        }

        private static void SetDictionary(Dictionary<string, string> dic, string key, string value)
        {
            if (value == null)
                throw (new ArgumentNullException("value"));

            dic[key] = value;
        }

        /// <summary>
        /// 指定の Application.StartupPath からの相対パスを、絶対パスに変換します
        /// </summary>
        /// <param name="relativePath">相対パス</param>
        /// <returns>絶対パス</returns>
        private static string ToFullPath(string relativePath)
        {
            Uri u1 = new Uri(Application.StartupPath);
            Uri u2 = new Uri(u1, relativePath);

            return u2.LocalPath;
        }

        /// <summary>
        /// 指定の絶対パスを、Application.StartupPath からの相対パスに変換します。
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private static string ToRelativePath(string fullPath)
        {
            Uri u1 = new Uri(Application.StartupPath);
            Uri u2 = new Uri(u1, fullPath);

            // デコード
            string RelativePath = u1.MakeRelativeUri(u2).ToString();
            RelativePath = HttpUtility.UrlDecode(RelativePath).Replace('/', '\\');

            return RelativePath;
        }
    }
}
