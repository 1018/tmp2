using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CommonClassLibrary;
using WeifenLuo.WinFormsUI.Docking;

namespace LocalSimulator.MainProgram
{
    public class SubFormManager
    {
        public static readonly SubFormManager Instance = new SubFormManager();

        static SubFormManager()
        {
        }

        // レイアウト保存ファイル名
        const string FORM_LAYOUT_FNAME = @".\FormLayout_MainProgram.xml";

        const string BASE_FORM_LIST_NAME = "画面選択";
        const string IO_MONITOR_NAME = "I/Oモニタ";

        delegate Form FormFactory();


        readonly Dictionary<string, SubFormInfo> SubFormDictionary;

        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private SubFormManager()
        {
            this.SubFormDictionary = new Dictionary<string, SubFormInfo>();

            this.SubFormDictionary[BASE_FORM_LIST_NAME] =  new SubFormInfo(CreateBaseFormList);
            this.SubFormDictionary[IO_MONITOR_NAME] = new SubFormInfo(CreateIOMonitor);
        }

        /// <summary>
        /// サブフォームのレイアウトを保存
        /// </summary>
        /// <param name="dockPanel">現在のレイアウト情報を保持するパネルコントロール</param>
        public void SaveLayout(DockPanel dockPanel)
        {
            foreach (var elem in this.SubFormDictionary)
            {
                if (elem.Value.Instance != null)
                {
                    DockContent content = (DockContent)elem.Value.Instance;
                    string key = elem.Key;

                    content.DockHandler.GetPersistStringCallback = new GetPersistStringCallback(() => key);
                }
            }

            // レイアウト情報出力
            //try
            //{
                using (FileStream fStream = new FileStream(this.LayoutFilePath, FileMode.Create))
                {
                    dockPanel.SaveAsXml(fStream, System.Text.Encoding.Unicode);
                }
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine("MainForm.SaveLayout");
            //    System.Diagnostics.Debug.WriteLine(e.Message);
            //}
        }

        /// <summary>
        /// サブフォームのレイアウトを復元
        /// </summary>
        /// <param name="dockPanel">レイアウト情報を復元させるパネルコントロール</param>
        public void LoadLayout(DockPanel dockPanel)
        {
            if (File.Exists(this.LayoutFilePath))
            {
                try
                {
                    using (FileStream fStream = new FileStream(this.LayoutFilePath, FileMode.Open))
                    {
                        dockPanel.LoadFromXml(fStream, RestoreFormLayout);
                    }
                }
                catch (Exception e)
                {
                    Global.LogManager.Write("Error:LoadLayout:" + e.Message);
                }
            }
        }

        /// <summary>
        /// フォームの復元
        /// </summary>
        /// <param name="persistString">復元するフォームのキー文字列</param>
        /// <returns>復元するフォームのインスタンス</returns>
        private IDockContent RestoreFormLayout(string persistString)
        {
            if (this.SubFormDictionary.ContainsKey(persistString))
            {
                Global.LogManager.Write("フォーム復元:" + persistString);

                return (IDockContent)this.SubFormDictionary[persistString].Factory();
            }
            else
            {
                Global.LogManager.Write("フォーム復元失敗:" + persistString);

                return null;
            }
        }

        /// <summary>
        /// I/Oモニターフォーム生成
        /// </summary>
        /// <returns>I/Oモニターフォームのインスタンス</returns>
        private Form CreateIOMonitor()
        {
            IOMonitorForm formIOMonitor = new IOMonitorForm();
            formIOMonitor.ShowHint = DockState.DockRight;

            this.SubFormDictionary[IO_MONITOR_NAME].Instance = formIOMonitor;

            return formIOMonitor;
        }

        /// <summary>
        /// BaseForm選択フォーム生成
        /// </summary>
        /// <returns>BaseForm選択フォームのインスタンス</returns>
        private Form CreateBaseFormList()
        {
            BaseFormListForm formBaseFormList = new BaseFormListForm();
            formBaseFormList.ShowHint = DockState.DockLeft;

            // 選択変更処理をMainFormで処理する
            formBaseFormList.BaseFormList.MouseDoubleClick +=
                new MouseEventHandler(MainForm.Instance.BaseFormList_MouseDoubleClick);

            this.SubFormDictionary[BASE_FORM_LIST_NAME].Instance = formBaseFormList;

            return formBaseFormList;
        }


        #region プロパティ

        /// <summary>
        /// I/Oモニタフォーム
        /// </summary>
        public IOMonitorForm IOMonitorForm
        {
            get
            {
                IOMonitorForm formInstance = this.SubFormDictionary[IO_MONITOR_NAME].Instance as IOMonitorForm;

                if (formInstance == null || formInstance.IsDisposed)
                {
                    formInstance = (IOMonitorForm)CreateIOMonitor();

                    this.SubFormDictionary[IO_MONITOR_NAME].Instance = formInstance;
                }

                return formInstance;
            }
        }

        /// <summary>
        /// BaseForm選択フォーム
        /// </summary>
        public BaseFormListForm BaseFormListForm
        {
            get
            {
                BaseFormListForm formInstance = this.SubFormDictionary[BASE_FORM_LIST_NAME].Instance as BaseFormListForm;

                if (formInstance == null || formInstance.IsDisposed)
                {
                    formInstance = (BaseFormListForm)CreateBaseFormList();

                    this.SubFormDictionary[BASE_FORM_LIST_NAME].Instance = formInstance;
                }

                return formInstance;
            }
        }

        /// <summary>
        /// レイアウト情報を保存するファイルのパス
        /// </summary>
        private string LayoutFilePath
        {
            get
            {
                AbsolutePathGenerator generator = new AbsolutePathGenerator(Application.StartupPath);
                return generator.ToAbsolutePath(FORM_LAYOUT_FNAME);
            }
        }

        #endregion


        private class SubFormInfo
        {
            public SubFormInfo(FormFactory factory)
            {
                _Factory = factory;

                this.Instance = null;
            }

            public Form Instance
            {
                get;
                set;
            }

            readonly FormFactory _Factory;
            public FormFactory Factory
            {
                get { return _Factory; }
            }
        }
    }
}
