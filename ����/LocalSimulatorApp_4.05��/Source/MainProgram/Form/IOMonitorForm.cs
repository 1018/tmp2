using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CommonClassLibrary;

namespace LocalSimulator.MainProgram
{
    public sealed partial class IOMonitorForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IOMonitorForm()
        {
            InitializeComponent();

            #region メンバ初期化

            this._AutoUpDateTimer = new System.Windows.Forms.Timer();
            this._AutoUpDateTimer.Tick += new EventHandler(this.AutoUpdateTimer_Tick);
            this._AutoUpDateTimer.Interval = 1000;

            #endregion

            // イメージリスト作成
            // (イメージが存在しないとTreeListViewの表示がズレる)
            ImageList imageList = new ImageList();
            Bitmap iconBmp = new Bitmap(16, 16);
            imageList.Images.Add(iconBmp);
            treeListIOMonitor.SmallImageList = imageList;

            // 子コントロールにDropイベント追加
            foreach (Control child in this.Controls)
            {
                child.DragEnter += new DragEventHandler(this.IOMonitorForm_DragEnter);
                child.DragDrop += new DragEventHandler(this.IOMonitorForm_DragDrop);
            }

            displayFormatDelegate = WordFormatter.FormatInt16;

            //デバイスログ開始
            string LogPath = AppSetting.LogPath + Path.DirectorySeparatorChar + "DeviceLog.log";

            logManager = KssClassLibrary.LogManager.CreateInstance(this, LogPath, true);
        }

        static KssClassLibrary.LogManager logManager;

        delegate string WordDisplayFormatDelegate(ushort[] value, int index, int numberFormat);

        static WordDisplayFormatDelegate displayFormatDelegate;
        static int numberFormat = 10;

        System.Windows.Forms.Timer _AutoUpDateTimer = null;       


        Setter_Draw _DeviceSetter = new Setter_Draw();           // デバイスセッター


        #region IOMonitorForm イベント

        private void IOMonitorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // フォームはCloseせず、非表示で待機
            e.Cancel = true;
            this.Hide();
        }

        private void IOMonitorForm_Load(object sender, EventArgs e)
        {
            // パネルのボーダーライン（デザイン用に表示）を削除
            this.DummyPanel1.BorderStyle = BorderStyle.None;
            this.DummyPanel2.BorderStyle = BorderStyle.None;
        }

        private void IOMonitorForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(IOMonitorSymbolFormat)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void IOMonitorForm_DragDrop(object sender, DragEventArgs e)
        {
            IOMonitorSymbolFormat dropData = (IOMonitorSymbolFormat)e.Data.GetData(typeof(IOMonitorSymbolFormat));     // ドロップされたシンボル情報
            IEnumerable<IOMonitorSymbolFormat> monitorSymbols = null;

            // シンボル名がnullなら、シンボル選択フォームを表示する
            if (dropData.SymbolName == null)
            {
                SelectSymbolForm selectForm = new SelectSymbolForm();

                #region シンボル選択フォーム表示

                // ドロップされたBaseForm
                BaseForm dropBaseForm = SupportMethods.GetBaseFormInstance(dropData.BaseFormNumber);

                // BaseFormが存在しないことはありえない
                System.Diagnostics.Debug.Assert(dropBaseForm != null);

                // I/O Monitor未表示のシンボル一覧を作成
                List<Symbol_Draw> unmonitorSymbolList = new List<Symbol_Draw>();
                foreach (Control ctrl in dropBaseForm.Controls)
                {
                    Symbol_Draw symbol = ctrl as Symbol_Draw;
                    if (symbol == null) { continue; }

                    bool isFind = false;
                    foreach (TreeListViewItem item in this.treeListIOMonitor.Items)
                    {
                        MonitorSymbolItem monitorItem = item as MonitorSymbolItem;
                        if (monitorItem == null) { continue; }

                        if (monitorItem.SymbolInstance == symbol)
                        {
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        unmonitorSymbolList.Add(symbol);
                    }
                }

                // シンボル選択画面表示
                selectForm.StartPosition = FormStartPosition.Manual;
                selectForm.SourceList = unmonitorSymbolList;
                selectForm.Location = PointToScreen(this.treeListIOMonitor.Location);
                selectForm.ShowDialog();

                #endregion

                IEnumerable<Symbol_Draw> selectedSymbols =
                                            selectForm.SelectedSymbolCollection;    // 選択されたシンボル一覧

                // 選択されたシンボルを書式化する
                List<IOMonitorSymbolFormat> symbolDataList = new List<IOMonitorSymbolFormat>();
                foreach (Symbol_Draw symbol in selectedSymbols)
                {
                    IOMonitorSymbolFormat monitorSymbolData = new IOMonitorSymbolFormat();
                    monitorSymbolData.BaseFormNumber = dropData.BaseFormNumber;
                    monitorSymbolData.SymbolName = symbol.SymbolName;

                    symbolDataList.Add(monitorSymbolData);
                }

                monitorSymbols = symbolDataList;
            }
            else
            {
                monitorSymbols = new IOMonitorSymbolFormat[] { dropData };
            }

            // モニタリストにシンボルを追加する
            SetMonitorSymbols(monitorSymbols);
        }

        private void IOMonitorForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                // Timer ON
                this._AutoUpDateTimer.Start();
                Console.WriteLine("I/O Moniter Timer ON");
            }
            else
            {
                // Timer OFF
                this._AutoUpDateTimer.Stop();
                Console.WriteLine("I/O Monitor Timer OFF");
            }
        }

        private void IOMonitorForm_SizeChanged(object sender, EventArgs e)
        {
            this.treeListIOMonitor.Size =
                new Size(this.Width, this.ClientRectangle.Height - this.treeListIOMonitor.Top);
        }

        #endregion

        #region TreeListIOMonitor（TreeListViwe） イベント

        private void TreeListIOMonitor_AfterExpand(object sender, TreeListViewEventArgs e)
        {
            // 列幅調節
            treeListIOMonitor.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void TreeListIOMonitor_DoubleClick(object sender, EventArgs e)
        {
            // カーソル位置のアイテム
            Point clientCursorPosition = treeListIOMonitor.PointToClient(Cursor.Position);
            ListViewHitTestInfo hitInfo = treeListIOMonitor.HitTest(clientCursorPosition);

            if (hitInfo.Item != null)
            {
                TreeListViewItem hitItem = (TreeListViewItem)hitInfo.Item;

                if (hitItem.Items.Count == 0)
                {
                    string address = hitItem.SubItems[1].Text;

                    if (!string.IsNullOrEmpty(address) && (address != "未使用"))
                    {
                        SetDeviceForm setDeviceForm = new SetDeviceForm(address);
                        if (setDeviceForm.ShowDialog() == DialogResult.OK)
                        {
                            ushort shortValue = (ushort)(setDeviceForm.InputValue & 0x0FFFF);
                            this._DeviceSetter.Public_SetDeviceData(address, shortValue);
                        }
                    }
                }
            }
        }

        private void TreeListIOMonitor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                #region [Delete]

                int selectedItemIndex;

                // アイテムが一つも選択されていない
                int selectedItemCount = this.treeListIOMonitor.SelectedItems.Count;
                if (selectedItemCount == 0) { return; }

                // 選択されたアイテム
                TreeListViewItem selectedItem = this.treeListIOMonitor.SelectedItems[0];

                // 最上位のアイテム（シンボル名が表示されたアイテム）
                TreeListViewItem symbolItem = selectedItem;
                while (symbolItem.Parent != null)
                {
                    symbolItem = symbolItem.Parent;
                }

                //selectedItemIndex = symbolItem.Index;   // インデックスを保持
                selectedItemIndex = this.treeListIOMonitor.Items.GetIndexOf(symbolItem);

                // シンボルを削除
                this.treeListIOMonitor.Items.Remove(symbolItem);


                // アイテム選択状態変更
                if (treeListIOMonitor.Items.Count != 0)
                {
                    int selectItemIndex = selectedItemIndex;

                    // 一番下のアイテムが削除された
                    if (this.treeListIOMonitor.Items.Count == selectedItemIndex)
                    {
                        // 一番下のアイテムを選択状態にする
                        int bottomMostItemIndex = this.treeListIOMonitor.Items.Count - 1;
                        selectItemIndex = bottomMostItemIndex;
                    }

                    this.treeListIOMonitor.Items[selectItemIndex].Selected = true;
                }

                #endregion

            }
        }

        private void TreeListIOMonitor_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // 全ての子アイテムのチェック状態を変化させる

            TreeListViewItem checkedItem = (TreeListViewItem)e.Item;
            bool checkState = checkedItem.Checked;

            SetCheckAllChildItem(checkedItem, checkState);
        }

        #endregion

        #region 表示変更RadioButton イベント

        private void ValuePattern_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Checked)
            {
                if (false) { }

                else if (object.Equals(radioButton, this.rbInt16))
                {
                    displayFormatDelegate = WordFormatter.FormatInt16;
                }
                else if (object.Equals(radioButton, this.rbInt32))
                {
                    displayFormatDelegate = WordFormatter.FormatInt32;
                }
                else if (object.Equals(radioButton, this.rbFloat))
                {
                    displayFormatDelegate = WordFormatter.FormatFloat;
                }
                else if (object.Equals(radioButton, this.rbDouble))
                {
                    displayFormatDelegate = WordFormatter.FormatDouble;
                }
            }
        }

        private void NumberPattern_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Checked)
            {
                if (false) { }

                else if (object.Equals(radioButton, this.rbDecNumber))
                {
                    numberFormat = 10;
                }
                else if (object.Equals(radioButton, this.rbHexNumber))
                {
                    numberFormat = 16;
                }
                else if (object.Equals(radioButton, this.rbBinNumber))
                {
                    numberFormat = 2;
                }
            }
        }

        #endregion

        private void AutoUpdateTimer_Tick(object sender, EventArgs e)
        {
            MethodInvoker updateCallBack =
                () =>
                {
                    // 各TreeListItemに対し、Updateメソッドを呼び出す
                    foreach (TreeListViewItem item in this.treeListIOMonitor.Items)
                    {
                        MonitorSymbolItem monitorItem = item as MonitorSymbolItem;
                        if (monitorItem != null)
                        {
                            monitorItem.Update();
                        }
                    }

                };

            if (this.IsHandleCreated)
            {
                this.Invoke(updateCallBack);
            }
        }


        public static void LogOut(string symbolName, string propertyName, string address, string oldValue, string newValue)
        {
            string logOutString = string.Format(
                "{0}.{1} {2} : {3} → {4}", symbolName, propertyName, address, oldValue, newValue);


            logManager.Write(logOutString);

        }


        /// <summary>
        /// I/O Monitorに表示するシンボルを設定します。
        /// </summary>
        /// <param name="monitorSymbols">
        /// 表示するシンボル情報
        /// </param>
        public void SetMonitorSymbols(IEnumerable<IOMonitorSymbolFormat> monitorSymbols)
        {
            foreach (IOMonitorSymbolFormat symbolData in monitorSymbols)
            {
                // 重複なし
                if (!IsOverlapSymbol(symbolData))
                {
                    MonitorSymbolItem monitorItem = new MonitorSymbolItem(symbolData);

                    // シンボルデータ無効
                    if (monitorItem.SymbolInstance == null)
                    {
                        continue;
                    }

                    monitorItem.BackColor = Color.LightGreen;     // 最上位ノードには色を付ける

                    // サブアイテム追加
                    for (int i = 0; i < this.treeListIOMonitor.Columns.Count; i++)
                    {
                        monitorItem.SubItems.Add(string.Empty);
                    }

                    monitorItem.SubItems[3].Text = symbolData.BaseFormNumber.ToString().PadLeft(4,'0');


                    this.treeListIOMonitor.Items.Add(monitorItem);
                    monitorItem.Update();
                }
            }

            // カラム幅調節
            this.treeListIOMonitor.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private bool IsOverlapSymbol(IOMonitorSymbolFormat symbolData)
        {
            bool isOverlap = false;
            foreach (TreeListViewItem item in this.treeListIOMonitor.Items)
            {
                MonitorSymbolItem monitorItem = item as MonitorSymbolItem;
                if (monitorItem == null) { continue; }

                if ((symbolData.BaseFormNumber == monitorItem.BaseFormNumber) &&
                    (symbolData.SymbolName == monitorItem.SymbolInstance.SymbolName))
                {
                    isOverlap = true;
                    break;
                }
            }
            return isOverlap;
        }

        /// <summary>
        /// I/O Monitorに表示されているシンボルを取得します。
        /// </summary>
        /// <returns>
        /// I/O Monitorに表示しているシンボル情報
        /// </returns>
        public IEnumerable<IOMonitorSymbolFormat> GetMonitorSymbols()
        {
            List<IOMonitorSymbolFormat> monitorSymbolList = new List<IOMonitorSymbolFormat>();

            foreach (TreeListViewItem item in this.treeListIOMonitor.Items)
            {
                MonitorSymbolItem monitorItem = item as MonitorSymbolItem;
                if (monitorItem == null) { continue; }

                IOMonitorSymbolFormat symbolData = new IOMonitorSymbolFormat();
                symbolData.BaseFormNumber = monitorItem.BaseFormNumber;
                symbolData.SymbolName = monitorItem.SymbolInstance.SymbolName;
                symbolData.LogOutProperties = (
                    from ioProperty in monitorItem.MonitorIO
                    where ioProperty.IsLogOut
                    select ioProperty.Property)
                    .ToArray();

                monitorSymbolList.Add(symbolData);
            }

            return monitorSymbolList;
        }

        /// <summary>
        /// I/O Monitorに表示されている全てのシンボルをクリアします。
        /// </summary>
        public void ClearMonitorSymbols()
        {
            while (this.treeListIOMonitor.Items.Count != 0)
            {
                this.treeListIOMonitor.Items.RemoveAt(0);
            }
        }

        /// <summary>
        /// 全ての子アイテムのチェック状態を変化させる
        /// </summary>
        /// <param name="baseItem">子アイテムを持つ親アイテム</param>
        /// <param name="isChecked">設定するチェック状態</param>
        private void SetCheckAllChildItem(TreeListViewItem baseItem, bool isChecked)
        {
            foreach (TreeListViewItem item in baseItem.Items)
            {
                item.Checked = isChecked;
                SetCheckAllChildItem(item, isChecked);
            }
        }


        #region Setter_Draw

        /// <summary>
        /// デバイスセット用 Symbol_Draw
        /// </summary>
        private class Setter_Draw : Symbol_Draw
        {
            public Setter_Draw()
            {
                base.SymbolName = "I/O Monitor";
            }

            public void Public_SetDeviceData(string address, ushort value)
            {
                base.SetDeviceData(address, value);
            }
        }

        #endregion


        #region MonitorIOInfo

        /// <summary>
        /// ログアウトするデバイスを表す
        /// </summary>
        [TypeConverter(typeof(MonitorIOInfoConverter))]
        private class MonitorIOInfo
        {
            public MonitorIOInfo()
            {
                this.Property = string.Empty;
                this.ArrayIndex = -1;
                this.Offset = 0;
                this.IsLogOut = false;
            }


            public string Property { get; set; }

            public int ArrayIndex { get; set; }

            public int Offset { get; set; }

            public bool IsLogOut { get; set; }
        }

        #endregion


        #region MonitorIOInfoConverter

        /// <summary>
        /// MonitorIOInfoの文字列相互変換をサポートするコンバーター
        /// </summary>
        private class MonitorIOInfoConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                string sourceValue = value as string;

                if (sourceValue != null)
                {
                    MonitorIOInfo ioInfo = new MonitorIOInfo();
                    string[] param = sourceValue.Split(',');

                    try
                    {
                        ioInfo.Property = (string)param[0];
                        ioInfo.ArrayIndex = Int32.Parse(param[1]);
                        ioInfo.Offset = Int32.Parse(param[2]);
                        ioInfo.IsLogOut = Boolean.Parse(param[3]);

                        return ioInfo;
                    }
                    catch
                    {
                        return null;
                    }
                }

                return base.ConvertFrom(context, culture, value);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                MonitorIOInfo sourceValue = value as MonitorIOInfo;

                if ((sourceValue != null) && (destinationType == typeof(string)))
                {
                    string result = string.Empty;

                    try
                    {
                        string[] param = new string[] {
                            (string)sourceValue.Property,
                            sourceValue.ArrayIndex.ToString(),
                            sourceValue.Offset.ToString(),
                            sourceValue.IsLogOut.ToString()
                        };

                        return string.Join(",", param);
                    }
                    catch
                    {
                        return null;
                    }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        #endregion


        #region MonitorSymbolItem

        /// <summary>
        /// モニタするシンボルを表すTreeListViewItem
        /// </summary>
        private class MonitorSymbolItem : TreeListViewItem
        {

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="monitorSymbolData"></param>
            public MonitorSymbolItem(IOMonitorSymbolFormat monitorSymbolData)
            {
                int bfNumber = monitorSymbolData.BaseFormNumber;
                string name = monitorSymbolData.SymbolName;
                string[] logOut = monitorSymbolData.LogOutProperties;

                this.baseFormNumber = bfNumber;
                this.symbolInstance = SupportMethods.GetSymbolInstance(bfNumber, name);

                if (this.SymbolInstance != null)
                {
                    this.Text = SymbolInstance.SymbolName;
                }
            }

            /// <summary>
            /// Symbol_Drawの持つ、全てのデバイスプロパティを列挙する。
            /// </summary>
            /// <returns>全てのデバイスプロパティ</returns>
            private PropertyInfo[] EnumeratesDevicePropery()
            {
                Type symbolType = this.SymbolInstance.GetType();
                PropertyInfo[] symbolProperties = symbolType.GetProperties();

                List<PropertyInfo> deviceProperties = new List<PropertyInfo>();

                foreach (PropertyInfo property in symbolProperties)
                {
                    if (IsDeviceProperty(property))
                    {
                        deviceProperties.Add(property);
                    }
                }

                return deviceProperties.ToArray();
            }

            /// <summary>
            /// デバイスのプロパティであるかを判別する。
            /// </summary>
            /// <param name="property">判別するプロパティ</param>
            /// <returns>デバイスのプロパティならtrueを返す</returns>
            private bool IsDeviceProperty(PropertyInfo property)
            {
                Type propertyType = property.PropertyType;

                // DeviceFormat型であるか
                if (propertyType == typeof(DeviceFormat))
                {
                    return true;
                }

                // DeviceFormat型の配列であるか
                if (propertyType == typeof(DeviceFormat[]))
                {
                    return true;
                }

                // それ以外の型
                return false;
            }

            /// <summary>
            /// ネストされるTreeListViewItemを生成します。
            /// </summary>
            /// <returns>TreeListViewItemのインスタンス</returns>
            private TreeListViewItem CreateNestItem()
            {
                TreeListViewItem createItem = new TreeListViewItem();
                int columnCnt = this.TreeListView.Columns.Count;

                for (int i = 0; i < columnCnt; i++)
                {
                    createItem.SubItems.Add(string.Empty);
                }

                return createItem;
            }


            int baseFormNumber;
            public int BaseFormNumber
            {
                get
                {
                    return this.baseFormNumber;
                }
            }

            Symbol_Draw symbolInstance;
            public Symbol_Draw SymbolInstance
            {
                get
                {
                    return symbolInstance;
                }
            }

            public IEnumerable<MonitorIOInfo> MonitorIO
            {
                get
                {
                    string symbolName = this.SymbolInstance.SymbolName;

                    List<MonitorIOInfo> ioList = new List<MonitorIOInfo>();
                    IEnumerable<PropertyInfo> deviceProperties = EnumeratesDevicePropery(); // 全てのデバイスプロパティ

                    foreach (PropertyInfo property in deviceProperties)
                    {
                        MonitorIOInfo ioInfo = new MonitorIOInfo();

                        string propertyName = Global.GetDisplayName(this.SymbolInstance, property);    // プロパティ名
                        bool isLogOut = false;                  // ログ出力を行う

                        // プロパティノード取得
                        TreeListViewItem propertyNode = null;
                        foreach (TreeListViewItem item in this.Items)
                        {
                            if (item.Text == propertyName)
                            {
                                propertyNode = item;
                                break;
                            }
                        }

                        // ノードが存在しないことはありえない
                        System.Diagnostics.Debug.Assert(propertyNode != null);

                        // プロパティはDeviceFormat配列
                        if (property.PropertyType.IsSubclassOf(typeof(Array)))
                        {
                        }
                        // プロパティはDeviceFormat
                        else
                        {
                        }

                        // ノードがチェックされている
                        if (propertyNode.Checked)
                        {
                            isLogOut = true;    // ログ出力ON
                        }
                        else
                        {
                            isLogOut = false;   // ログ出力OFF
                        }

                        ioInfo.Property = propertyName;
                        ioInfo.IsLogOut = isLogOut;

                        ioList.Add(ioInfo);
                    }

                    return ioList;
                }
            }

            public void Update()
            {
                // 全てのデバイスプロパティ
                IEnumerable<PropertyInfo> deviceProperties = EnumeratesDevicePropery();

                foreach (PropertyInfo property in deviceProperties)
                {
                    string propertyName = Global.GetDisplayName(this.SymbolInstance, property);     // プロパティ名称
                    object propertyValue = property.GetValue(this.SymbolInstance, null);            // プロパティ値

                    // プロパティノード取得
                    TreeListViewItem propertyNode = null;
                    foreach (TreeListViewItem item in this.Items)
                    {
                        if (item.Text == propertyName)
                        {
                            propertyNode = item;
                        }
                    }

                    // ノードが生成されていない
                    if (propertyNode == null)
                    {
                        propertyNode = CreateNestItem();
                        propertyNode.Text = propertyName;
                        this.Items.Add(propertyNode);
                    }

                    // プロパティはDeviceFormat配列
                    if ((propertyValue as Array) != null)
                    {
                        #region DeviceFormat配列処理

                        DeviceFormat[] deviceArray = propertyValue as DeviceFormat[];

                        //配列子ノード作成
                        CreateArrayNestItem(deviceArray, propertyNode);


                        // 各デバイスの更新
                        for (int i = 0; i < deviceArray.Length; i++)
                        {
                            TreeListViewItem arrayIndexNode = propertyNode.Items[i];
                            DeviceFormat indexValue = deviceArray[i];

                            //複数データ時子ノード作成
                            CreateDatasNestItem(indexValue, arrayIndexNode);


                            // 更新処理
                            // 配列インデックスノードに最上位の値を表示

                            UpdateValue(arrayIndexNode, indexValue, this.SymbolInstance.SymbolName, propertyNode.Text + arrayIndexNode.Text);

                            // プロパティの値を表示
                            if (indexValue.DataCount != 1)
                            {
                                //複数データ更新
                                UpdateDatasValue(arrayIndexNode, indexValue, this.SymbolInstance.SymbolName, propertyNode.Text);
                            }
                        }

                        #endregion

                    }
                    // プロパティはDeviceFormat
                    else
                    {
                        #region DeviceFormat処理

                        DeviceFormat deviceValue = (DeviceFormat)propertyValue;

                        //複数データ時子ノード作成
                        CreateDatasNestItem(deviceValue, propertyNode);

                        // 更新処理

                        // プロパティノードに最上位の値を表示
                        UpdateValue(propertyNode, deviceValue, this.SymbolInstance.SymbolName, propertyNode.Text);

                        // プロパティの値を表示
                        if (deviceValue.DataCount != 1)
                        {
                            //複数データ更新
                            UpdateDatasValue(propertyNode, deviceValue, this.SymbolInstance.SymbolName, propertyNode.Text);
                        }

                        #endregion

                    }
                }
            }

            private void UpdateValue(TreeListViewItem node, DeviceFormat symbolProperty, string symbolName, string propertyName)
            {
                bool isLowest = node.Items.Count == 0;

                string newData = "";
                string nowData = "";

                #region アドレス
                newData = symbolProperty.Address;
                nowData = node.SubItems[1].Text;

                //アドレスが存在しない場合、何もしない
                if (String.IsNullOrEmpty(newData) && String.IsNullOrEmpty(nowData)) { return; }

                if (newData != nowData)
                {
                    node.SubItems[1].Text = newData;
                }

                #endregion
                #region 値
                nowData = node.SubItems[2].Text;
                if (symbolProperty.DataCount > 0)
                {
                    if (symbolProperty.Io != IoType.Out)
                    {
                        newData = IOMonitorForm.displayFormatDelegate(symbolProperty.Value, 0, IOMonitorForm.numberFormat);
                    }
                    else
                    {
                        newData = "----";
                    }
                }

                if (nowData != newData)
                {
                    node.SubItems[2].Text = newData;
                    if (!String.IsNullOrEmpty(newData) && !String.IsNullOrEmpty(nowData) && node.Checked && isLowest)
                    {
                        LogOut(symbolName, propertyName, node.SubItems[1].Text, nowData, newData);
                    }

                }
                #endregion
                #region 2進表示
                if (symbolProperty.Io != IoType.Out)
                {
                    //nowData = node.SubItems[3].Text;
                    //if (symbolProperty.DataCount > 0)
                    //{
                    //    newData = WordFormatter.FormatBinary(symbolProperty.Value[0]);
                    //}
                    //else
                    //{
                    //    newData = "";
                    //}

                    //if (nowData != newData)
                    //{
                    //    node.SubItems[3].Text = newData;
                    //}
                }
                #endregion
            }

            private void UpdateDatasValue(TreeListViewItem node, DeviceFormat symbolProperty, string symbolName, string propertyName)
            {
                for (int cnt = 0; cnt < symbolProperty.DataCount; cnt++)
                {
                    TreeListViewItem deviceNode = node.Items[cnt];

                    bool isLowest = deviceNode.Items.Count == 0;
                    string dNodeNow;
                    string dNodeNew;

                    #region アドレス
                    dNodeNow = deviceNode.SubItems[1].Text;
                    dNodeNew = DeviceManager.Offset(symbolProperty.Address, cnt);

                    //アドレスが存在しない場合、何もしない
                    if (String.IsNullOrEmpty(dNodeNew) && String.IsNullOrEmpty(dNodeNow)) { continue; }

                    if (dNodeNew != dNodeNow)   // 表示が一致しない
                    {
                        deviceNode.SubItems[1].Text = dNodeNew;
                    }
                    #endregion
                    #region 値

                    if (symbolProperty.Io == IoType.Out)
                    {
                        dNodeNew = "----";
                    }
                    else
                    {
                        dNodeNew = IOMonitorForm.displayFormatDelegate(symbolProperty.Value, cnt, IOMonitorForm.numberFormat);
                    }

                    dNodeNow = deviceNode.SubItems[2].Text;

                    if (dNodeNew != dNodeNow)   // 表示が一致しない
                    {
                        deviceNode.SubItems[2].Text = dNodeNew;
                        if (!String.IsNullOrEmpty(dNodeNew) && !String.IsNullOrEmpty(dNodeNow) && deviceNode.Checked && isLowest)
                        {
                            LogOut(symbolName, propertyName + deviceNode.Text, deviceNode.SubItems[1].Text, dNodeNow, dNodeNew);
                        }
                    }
                    #endregion
                    #region 2進表示
                    if (symbolProperty.Io != IoType.Out)
                    {
                        //dNodeNow = deviceNode.SubItems[3].Text;
                        //dNodeNew = WordFormatter.FormatBinary(symbolProperty.Value[cnt]);

                        //if (dNodeNew != dNodeNow)   // 表示が一致しない
                        //{
                        //    deviceNode.SubItems[3].Text = dNodeNew;
                        //}
                    }
                    #endregion
                }

            }

            private void CreateArrayNestItem(DeviceFormat[] deviceArray, TreeListViewItem propertyNode)
            {
                // 配列長とノード数が一致しない
                if (deviceArray.Length != propertyNode.Items.Count)
                {
                    // 配列長 < ノード数
                    if (deviceArray.Length < propertyNode.Items.Count)
                    {
                        do
                        {
                            propertyNode.Items.RemoveAt(propertyNode.Items.Count - 1);

                        } while (deviceArray.Length < propertyNode.Items.Count);
                    }
                    // 配列長 > ノード数
                    else
                    {
                        do
                        {
                            TreeListViewItem arrayIndexNode = CreateNestItem();
                            arrayIndexNode.Text = string.Format("[{0}]", propertyNode.Items.Count);

                            propertyNode.Items.Add(arrayIndexNode);

                        } while (deviceArray.Length > propertyNode.Items.Count);
                    }
                }
            }

            private void CreateDatasNestItem(DeviceFormat deviceValue, TreeListViewItem propertyNode)
            {
                // データ数とノード数が一致しない AND　データ数が1でない場合
                if (deviceValue.DataCount != propertyNode.Items.Count && deviceValue.DataCount != 1)
                {
                    // データ数 < ノード数
                    if (deviceValue.DataCount < propertyNode.Items.Count)
                    {
                        do
                        {
                            propertyNode.Items.RemoveAt(propertyNode.Items.Count - 1);

                        } while (deviceValue.DataCount < propertyNode.Items.Count);
                    }
                    // データ数 > ノード数
                    else
                    {
                        do
                        {
                            TreeListViewItem deviceNode = CreateNestItem();
                            deviceNode.Text = string.Format("[+{0}]", propertyNode.Items.Count);
                            propertyNode.Items.Add(deviceNode);

                        } while (deviceValue.DataCount > propertyNode.Items.Count);
                    }
                }
            }

        }

        #endregion


        #region WordFormatter

        /// <summary>
        /// ワード値の文字列表示をサポートするメソッド群
        /// </summary>
        private static class WordFormatter
        {
            /// <summary>
            /// 16ﾋﾞｯﾄ整数に変換する
            /// </summary>
            /// <param name="value">変換ワード配列</param>
            /// <param name="index">変換開始位置</param>
            /// <param name="numberFormat">進数変換</param>
            /// <returns>
            /// 16ﾋﾞｯﾄ整数を表す文字列
            /// </returns>
            public static string FormatInt16(ushort[] value, int index, int numberFormat)
            {
                if (numberFormat == 16)
                {
                    return value[index].ToString("X4");
                }
                else if (numberFormat == 10)
                {
                    return value[index].ToString("D");
                }
                else
                {
                    return FormatBinary(value[index]);
                }
            }

            /// <summary>
            /// 32ﾋﾞｯﾄ整数に変換する
            /// </summary>
            /// <remarks>
            /// 変換開始位置が2の倍数でないとき、このメソッドはstring.Emptyを返します。
            /// IOMonitorForm以外で使用する場合は注意してください。
            /// </remarks>
            /// <param name="value">変換ワード配列</param>
            /// <param name="index">変換開始位置</param>
            /// <param name="numberFormat">進数変換</param>
            /// <returns>
            /// 32ﾋﾞｯﾄ整数を表す文字列
            /// </returns>
            public static string FormatInt32(ushort[] value, int index, int numberFormat)
            {
                // 偶数番にのみ表示
                if ((index & 0x01) == 0)
                {
                    uint nowValue;

                    if ((index + 1) >= value.Length)
                        nowValue = value[index];
                    else
                        nowValue = (uint)(value[index + 1] << 16) + value[index];

                    if (numberFormat == 16)
                    {
                        return nowValue.ToString("X8");
                    }
                    else
                    {
                        return nowValue.ToString("D");
                    }
                }
                return string.Empty;
            }

            /// <summary>
            /// 単精度浮動小数点数に変換する
            /// </summary>
            /// <remarks>
            /// 変換開始位置が2の倍数でないとき、このメソッドはstirng.Emptyを返します。
            /// IOMonitorForm以外で使用する場合は注意してください。
            /// 
            /// 符号 31bit
            /// 指数 30bit - 23bit
            /// 仮数 22bit - 0bit
            /// </remarks>
            /// <param name="value">変換ワード配列</param>
            /// <param name="index">変換開始位置</param>
            /// <param name="numberFormat">進数変換</param>
            /// <returns>
            /// 単精度浮動小数点数を表す文字列
            /// </returns>
            public static string FormatFloat(ushort[] value, int index, int numberFormat)
            {
                // 偶数番にのみ表示
                if ((index & 0x01) == 0)
                {
                    if ((index + 1) >= value.Length) return "----------";

                    int nowValue = (value[index + 1] << 16) + value[index];

                    bool s = ((nowValue >> 31 & 0x01) != 0);    // 符号ﾋﾞｯﾄ
                    int exp = nowValue >> 23 & 0x0FF;           // 指数部
                    int frection = nowValue & 0x07FFFFF;        // 仮数部
                    // 算出するのはfloatだが、計算はdoubleで行う
                    double floatValue = 1.0;

                    // ゼロ
                    if (exp == 0 && frection == 0)
                    {
                        return ((double)0.0).ToString();
                    }
                    // 非正規化数
                    else if (exp == 0 && frection != 0)
                    {
                        // 非正規化数の処理が必要ならここに書く。
                        // デフォルトでは表記不可として扱う

                        return "----------";
                    }
                    // 無限大
                    else if (exp == 0xFF && frection == 0)
                    {
                        return "∞";
                    }
                    // NaN
                    else if (exp == 0xFF && frection != 0)
                    {
                        return "----------";
                    }
                    // 正規化数
                    else
                    {
                        exp -= 127;

                        for (int i = 1; i <= 23; i++)
                        {
                            // 左端のﾋﾞｯﾄが立っている
                            if ((frection & 0x400000) != 0)
                            {
                                floatValue += (double)Math.Pow(2, -i);
                            }

                            frection = frection << 1;
                        }

                        floatValue = floatValue * Math.Pow(2, exp);

                        // 負数
                        if (s) floatValue *= -1;

                        return ((float)floatValue).ToString();
                    }
                }
                return string.Empty;
            }

            /// <summary>
            /// 倍精度浮動小数点数に変換する
            /// </summary>
            /// <remarks>
            /// 変換開始位置が4の倍数でないとき、このメソッドはstring.Emptyを返します。
            /// IOMonitorForm以外で使用する場合は注意してください。
            /// 
            /// 符号 63bit
            /// 指数 62bit - 52bit
            /// 仮数 51bit - 0bit
            /// </remarks>
            /// <param name="value">変換ワード配列</param>
            /// <param name="index">変換開始位置</param>
            /// <param name="numberFormat">進数変換</param>
            /// <returns>
            /// 倍精度浮動小数点数を表す文字列
            /// </returns>
            public static string FormatDouble(ushort[] value, int index, int numberFormat)
            {
                // 4の倍数番(0を含む)のみ表示
                if ((index & 0x03) == 0)
                {
                    if ((index + 3) >= value.Length) return "----------";

                    long nowValue = (
                          value[index + 3] << 48
                        + value[index + 2] << 32
                        + value[index + 1] << 16
                        + value[index + 0] << 0);

                    bool s = ((nowValue >> 62 & 0x01) != 0);        // 符号ﾋﾞｯﾄ
                    long exp = nowValue >> 52 & 0x07FF;             // 指数部
                    long frection = nowValue & 0x0FFFFFFFFFFFFF;    // 仮数部
                    double doubleValue = 1.0;

                    // ゼロ
                    if (exp == 0 && frection == 0)
                    {
                        return ((double)0.0).ToString();
                    }
                    // 非正規化数
                    else if (exp == 0 && frection != 0)
                    {
                        // 非正規化数の処理が必要ならここに書く。
                        // デフォルトでは表記不可として扱う

                        return "----------";
                    }
                    // 無限大
                    else if (exp == 0x7FF && frection == 0)
                    {
                        return "∞";
                    }
                    // NaN
                    else if (exp == 0x7FF && frection != 0)
                    {
                        return "----------";
                    }
                    // 正規化数
                    else
                    {
                        exp -= 1023;

                        for (int i = 1; i <= 23; i++)
                        {
                            // 左端のﾋﾞｯﾄが立っている
                            if ((frection & 0x8000000000000) != 0)
                            {
                                doubleValue += (double)Math.Pow(2, -i);
                            }

                            frection = frection << 1;
                        }

                        doubleValue = doubleValue * Math.Pow(2, exp);

                        // 負数
                        if (s) doubleValue *= -1;

                        return doubleValue.ToString();
                    }
                }
                return string.Empty;
            }

            /// <summary>
            /// バイナリに変換する
            /// </summary>
            /// <param name="value">変換するワード値</param>
            /// <returns>
            /// バイナリを表す文字列
            /// </returns>
            public static string FormatBinary(ushort value)
            {
                ushort shiftValue = value;
                string binaryStr = string.Empty;

                for (int i = 0; i < 16; i++)
                {
                    if ((i & 0x03) == 0)
                        binaryStr += "　";

                    if ((shiftValue & 0x8000) != 0)
                        binaryStr += "1";
                    else
                        binaryStr += "0";

                    shiftValue = (ushort)(shiftValue << 1);
                }

                return binaryStr;
            }
        }

        #endregion


        private static class SupportMethods
        {
            /// <summary>
            /// 指定のBaseFormのインスタンスを取得します。
            /// </summary>
            /// <param name="baseFormNumber">インスタンスを取得するBaseFormの番号</param>
            /// <returns>
            /// BaseFormのインスタンスを返す。
            /// 指定されたBaseFormが存在しないとき、nullを返す。
            /// </returns>
            public static BaseForm GetBaseFormInstance(int baseFormNumber)
            {
                if (!ProjectManager.Loaded)
                {
                    return null;
                }

                IEnumerable<IBaseForm> baseForms = ProjectManager.BaseFormList;    // プロジェクトに存在するBaseForm

                // BaseFormを検索する
                IBaseForm baseFormInstance = null;
                foreach (IBaseForm form in ProjectManager.BaseFormList)
                {
                    if (form.Number == baseFormNumber)
                    {
                        baseFormInstance = form;
                        break;
                    }
                }

                if (baseFormInstance == null) { return null; }

                // BaseFormのインスタンスを返す
                return (BaseForm)baseFormInstance;
            }

            /// <summary>
            /// 指定のBaseFormより、指定のシンボルのインスタンスを取得します。
            /// </summary>
            /// <param name="baseFormNumber">検索するBaseFormの番号</param>
            /// <param name="symbolName">インスタンスを取得するシンボルの名称</param>
            /// <returns>
            /// シンボルのインスタンスを返す。
            /// 指定されたBaseForm・シンボルが存在しないとき、nullを返す。
            /// </returns>
            public static Symbol_Draw GetSymbolInstance(int baseFormNumber, string symbolName)
            {
                // シンボルの存在するBaseFormのインスタンス
                BaseForm existSymbolBaseForm = SupportMethods.GetBaseFormInstance(baseFormNumber);
                if (existSymbolBaseForm == null) { return null; }

                // BaseFormに存在するシンボル
                Symbol_Draw symbolInstance = null;
                foreach (Control control in existSymbolBaseForm.Controls)
                {
                    Symbol_Draw symbol = control as Symbol_Draw;

                    if ((symbol != null) && (symbol.SymbolName == symbolName))
                    {
                        symbolInstance = symbol;
                        break;
                    }
                }

                if (symbolInstance == null)
                {
                    return null;
                }

                // シンボルのインスタンスを返す
                return symbolInstance;                
            }
        }       

    }
}
