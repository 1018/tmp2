using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CommonClassLibrary;
using CustomBindingList;

namespace LocalSimulator.ProjectMaker
{
    public partial class IoListForm : Form
    {
        IoDataSource dataSource;

        private int prevSelectRow;

        static IoListForm _Instance;
        static object creatingInstanceLock;
        public static IoListForm Instance
        {
            get
            {
                lock (creatingInstanceLock)
                {
                    if (_Instance == null)
                    {
                        _Instance = new IoListForm();
                    }
                }
                return _Instance;
            }
        }

        static IoListForm()
        {
            creatingInstanceLock = new object();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private IoListForm()
        {
            // メンバ初期化
            dataSource = new IoDataSource();

            InitializeComponent();

            dgvIOView.DataSource = dataSource.GetDataSource();
            dgvIOView.AutoGenerateColumns = true;
        }

        /// <summary>
        /// Form.Closingイベント
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }
        /// <summary>
        /// Form.Loadイベント
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            PropertyDescriptorCollection listItemProperties =
                TypeDescriptor.GetProperties(typeof(IIoListItem));

            PropertyDescriptor addressProperty = listItemProperties["Address"];

            dataSource.RegistSortable(addressProperty, new AddressComparer());
        }
        /// <summary>
        /// Form.OnKeyDownイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.C:
                    if (dgvIOView.SelectedRows.Count == 0)
                        break;

                    break;

                case Keys.V:
                    break;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// DataGridView データ操作中例外発生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvIOView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Error"
                , MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// ツールメニュー [オプション]→[常に手前に表示]
        /// </summary>
        private void DlgTopMost_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem MenuItem = (ToolStripMenuItem)sender;

            if (MenuItem.Checked)
            {
                TopMost = true;
            }
            else
            {
                TopMost = false;
            }
        }
        /// <summary>
        /// ツールメニュー [操作]→[クリップボード貼り付け]
        /// </summary>
        private void ToClipboard_Click(object sender, EventArgs e)
        {
            Copy();
        }

        /// <summary>
        /// 表示内容をコピーする
        /// </summary>
        public void Copy()
        {
            string str = "";

            // ヘッダーテキストの取得
            foreach (DataGridViewColumn column in dgvIOView.Columns)
            {
                str += column.HeaderText + "\t";
            }
            str += "\n";

            // セルの値の取得
            foreach (DataGridViewRow row in dgvIOView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    str += cell.Value + "\t";
                }
                str += "\n";
            }

            Clipboard.SetText(str);
        }
        /// <summary>
        /// セルの内容のみコピーする(ヘッダはコピーしない)
        /// </summary>
        public void CopyCellOnly()
        {
            string str = "";

            // セルの値の取得
            foreach (DataGridViewRow row in dgvIOView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    str += cell.Value + "\t";
                }
                str += "\n";
            }

            Clipboard.SetText(str);
        }

        /// <summary>
        /// 表示するシンボルを追加する
        /// </summary>
        public void AddSymbol(BaseForm baseForm, Symbol_Draw symbol)
        {
            RegistProperty(baseForm, symbol, symbol, new NestedProperty[] { });
        }

        /// <summary>
        /// 表示するシンボルを追加する
        /// </summary>
        public void AddSymbol(BaseForm baseForm)
        {
            foreach (ShapeObject symbol in baseForm.ShapeCollection)
            {
                if (symbol.GetType() == typeof(ViewObject))
                {
                    ViewObject vobj = (ViewObject)symbol;
                    AddSymbol(baseForm, vobj.ControlInstance);
                }
            }
        }

        /// <summary>
        /// 表示するプロパティを追加する
        /// </summary>
        private void RegistProperty(BaseForm baseForm, Symbol_Draw symbol, object baseObject, NestedProperty[] nestProperties)
        {
            var properties = TypeDescriptor.GetProperties(baseObject);

            foreach (PropertyDescriptor property in properties)
            {
                // Symbol_Drawのプロパティをチェック中
                if (object.Equals(symbol, baseObject))
                {
                    if (!Global.GetVisibleAttribute(baseObject, property))
                    {
                        continue;
                    }
                }

                // Visible属性が付いている
                //if (Global.GetVisibleAttribute(baseObject, property))
                {
                    // 非表示に設定されているプロパティは表示しない
                    if (property.IsBrowsable == false)
                    {
                        continue;
                    }

                    // DeviceForamt型
                    if (property.PropertyType == typeof(DeviceFormat))
                    {
                        NestedProperty[] nestedProperty = new NestedProperty[nestProperties.Length + 1];
                        nestProperties.CopyTo(nestedProperty, 0);

                        int lastIndex = nestedProperty.Length - 1;
                        nestedProperty[lastIndex] = new NestedProperty(property);

                        IoListItem addItem = new IoListItem(baseForm, symbol, nestedProperty);
                        this.dataSource.Add(addItem);
                    }
                    // DeviceFormat[]型
                    else if (property.PropertyType == typeof(DeviceFormat[]))
                    {
                        DeviceFormat[] dvArray = (DeviceFormat[])property.GetValue(symbol);

                        for (int i = 0; i < dvArray.Length; i++)
                        {
                            NestedProperty[] nestedProperty = new NestedProperty[nestProperties.Length + 1];
                            nestProperties.CopyTo(nestedProperty, 0);

                            int lastIndex = nestedProperty.Length - 1;
                            nestedProperty[lastIndex] = new NestedProperty(property, i);

                            IoListItem addItem = new IoListItem(baseForm, symbol, nestedProperty);
                            this.dataSource.Add(addItem);
                        }
                    }
                    else
                    {
                        object propertyInstance = property.GetValue(baseObject);

                        Array arrayObject = propertyInstance as Array;

                        if (arrayObject == null)
                        {
                            NestedProperty[] nestedProperties = new NestedProperty[nestProperties.Length + 1];
                            nestProperties.CopyTo(nestedProperties, 0);

                            int lastIndex = nestedProperties.Length - 1;
                            nestedProperties[lastIndex] = new NestedProperty(property);

                            RegistProperty(baseForm, symbol, propertyInstance, nestedProperties);
                        }
                        else
                        {
                            for (int i = 0; i < arrayObject.Length; i++)
                            {
                                NestedProperty[] nestedProperties = new NestedProperty[nestProperties.Length + 1];
                                nestProperties.CopyTo(nestedProperties, 0);

                                int lastIndex = nestedProperties.Length - 1;
                                nestedProperties[lastIndex] = new NestedProperty(property, i);

                                RegistProperty(baseForm, symbol, arrayObject.GetValue(i), nestedProperties);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 表示中のシンボルを削除する
        /// </summary>
        public void RemoveSymbol(BaseForm baseForm)
        {
            dataSource.Remove(baseForm);
        }
        /// <summary>
        /// 表示中のシンボルを削除する
        /// </summary>
        public void RemoveSymbol(BaseForm baseForm, Symbol_Draw symbol)
        {
            dataSource.Remove(baseForm, (item) => { return symbol.SymbolName == item.SymbolName; });
        }

        public void ClearSymbol()
        {
            dataSource.Clear();
        }

        private class AddressComparer : IComparer<IIoListItem>
        {
            public int Compare(IIoListItem x, IIoListItem y)
            {
                int result = 0;
                string xAddr = x.Address;
                string yAddr = y.Address;

                // 未入力チェック
                if (string.IsNullOrEmpty(xAddr) || string.IsNullOrEmpty(yAddr))
                {
                    bool xIsNull = string.IsNullOrEmpty(xAddr);
                    bool yIsNull = string.IsNullOrEmpty(yAddr);

                    if (!xIsNull && yIsNull)
                        result = 1;
                    if (xIsNull && !yIsNull)
                        result = -1;

                    goto RESULT;
                }

                DeviceElement xDevice = DeviceManager.ToElement(xAddr);
                DeviceElement yDevice = DeviceManager.ToElement(yAddr);

                if (xDevice == null || yDevice == null)
                {
                    if (xDevice != null && yDevice == null)
                        result = 1;
                    if (xDevice == null && yDevice != null)
                        result = -1;

                    goto RESULT;
                }

                // デバイスタイプ判断
                int PrefixCompResult = string.Compare(xDevice.Prefix, yDevice.Prefix);
                if (PrefixCompResult != 0)
                {
                    result = PrefixCompResult;
                    goto RESULT;
                }

                // デバイスオフセット判断
                int OffsetCompResult = Comparer<int>.Default.Compare(xDevice.AddressOffset, yDevice.AddressOffset);
                if (OffsetCompResult != 0)
                {
                    result = OffsetCompResult;
                    goto RESULT;
                }
                // ビットオフセット判断
                int BitOffsetCompResult = Comparer<int>.Default.Compare(xDevice.BitOffset, yDevice.BitOffset);
                if (BitOffsetCompResult != 0)
                {
                    result = BitOffsetCompResult;
                    goto RESULT;
                }

                // 比較結果
            RESULT:

                return result;
            }

        }

        private void dgvIOView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //列見出しクリック時はそのまま終了
            if (e.RowIndex == -1) { return; }

            //選択列変化なし
            if (e.RowIndex == prevSelectRow) { return; }

            prevSelectRow = e.RowIndex;

            DataGridViewRow rowItems = this.dgvIOView.Rows[e.RowIndex];
            int baseNumber = (int)rowItems.Cells["BaseNumber"].Value;
            string symbolName = (string)rowItems.Cells["SymbolName"].Value;


            foreach (BaseForm bfm in MainForm.Instance.ProjectData.BaseFormData)
            {

                if (bfm.Number == baseNumber)
                {
                    //該当のBaseFormをアクティブにする。
                    bfm.Show();
                    bfm.Focus();
                    foreach (ShapeObject obj in bfm.ShapeCollection)
                    {
                        if (obj.GetType() == typeof(ViewObject))
                        {
                            //該当するシンボルをアクティブにする。
                            if (((ViewObject)obj).ControlInstance.SymbolName == symbolName)
                            {
                                bfm.SelectChange(obj);
                                break;
                            }
                        }
                    }

                    break;
                }
            }

            this.Activate();

        }
    }

    /// <summary>
    /// IOリストのデータソース
    /// </summary>
    /// <remarks>
    /// BindingSourceに対する操作をIIoListItemのメンバーから
    /// 行うようにカプセル化する。
    /// 
    /// (他の用途で使うこともないだろうけど。。。)
    /// </remarks>
    public class IoDataSource
    {
        SortableBindingList<IIoListItem> DataSource;

        public IoDataSource()
        {
            // データソースの要素の型はIIoListItemとする
            DataSource = new SortableBindingList<IIoListItem>();
        }

        /// <summary>
        /// IOリストのデータソースとなるBindingListを取得します。
        /// </summary>
        /// <remarks>
        /// このインスタンスを使って取得したBindingListに対し操作を行わないこと。
        /// カプセル化した意味がなくなる。
        /// </remarks>
        /// <returns></returns>
        public BindingList<IIoListItem> GetDataSource()
        {
            return DataSource;
        }

        public void Add(IIoListItem addItem)
        {
            if (addItem.FormPeculiarObject == null)
            {
                throw new ArgumentException(
                    "IIoListItem.FormPeculiarObject の値が null です。\n\n"
                    + "BaseFormを識別するためのリソースを設定してください。");
            }

            DataSource.Add(addItem);
        }

        public void Remove(object peculiarObject)
        {
            Remove(peculiarObject, (item) => true);
        }

        public void Remove(object peculiarObject, Predicate<IIoListItem> match)
        {
            // 削除するアイテムリスト
            List<IIoListItem> RemoveItems = new List<IIoListItem>();

            // 削除条件に一致するアイテムの検索
            foreach (IIoListItem element in DataSource)
            {
                // 条件に一致する
                if (element.FormPeculiarObject.Equals(peculiarObject) && match(element))
                {
                    RemoveItems.Add(element);
                }
            }

            // アイテムの削除
            foreach (IIoListItem element in RemoveItems)
            {
                DataSource.Remove(element);
            }
        }

        public void RemoveAt(int index)
        {
            DataSource.RemoveAt(index);
        }

        public void Clear()
        {
            DataSource.Clear();
        }

        public void RegistSortable(PropertyDescriptor property, IComparer<IIoListItem> comparer)
        {
            DataSource.RegisterSortable(property, comparer);
        }

        public void UnregistSortable(PropertyDescriptor property)
        {
            DataSource.UnregisterSortable(property);
        }

        public void ResetSortable()
        {
            DataSource.ResetSortable();
        }
    }


    /// <summary>
    /// IOリストに表示する要素を定義するインターフェース
    /// </summary>
    /// <remarks>
    /// インターフェースにした理由は、
    /// 表示する要素とデータソースを分離しておきたかった。
    /// 
    /// インターフェースでは表示する要素を定義している。
    /// </remarks>
    public interface IIoListItem
    {
        /// <summary>
        /// 各Form固有のオブジェクトインスタンス
        /// 
        /// BaseForm番号が被ることを考慮。
        /// 各作成中フォームを識別するためのユニークなインスタンスが必要。
        /// </summary>
        [Browsable(false)]
        object FormPeculiarObject { get; }

        [Browsable(false)]
        string PropertyName { get; }

        /// <summary>
        /// BaseForm番号
        /// </summary>
        [Browsable(true), DisplayName("BaseForm番号")]
        int BaseNumber { get; }

        /// <summary>
        /// シンボル名
        /// </summary>
        [Browsable(true), DisplayName("シンボル名")]
        string SymbolName { get; }

        /// <summary>
        /// プロパティ名
        /// </summary>
        [Browsable(true), DisplayName("プロパティ名")]
        string DisplayPropertyName { get; }

        /// <summary>
        /// デバイスアドレス
        /// </summary>
        [Browsable(true), DisplayName("デバイスアドレス")]
        string Address { get; }

        /// <summary>
        /// シンボルタイプ
        /// </summary>
        [Browsable(true), DisplayName("シンボルタイプ")]
        string SymbolType { get; }

    }

    /// <summary>
    /// IIoListItemを実装したクラス
    /// </summary>
    public class IoListItem : IIoListItem
    {
        // 
        // コンストラクタ
        // 
        public IoListItem(BaseForm baseForm, Symbol_Draw symbol, NestedProperty[] propertyCollection)
        {
            if (baseForm == null) { throw new ArgumentNullException("baseForm"); }
            if (symbol == null) { throw new ArgumentNullException("symbol"); }
            if (propertyCollection == null) { throw new ArgumentNullException("propertyCollection"); }

            if (propertyCollection.Length < 1) { throw new ArgumentException("一つ以上のプロパティが必要です。", "propertyCollection"); }

            BaseForm = baseForm;
            Symbol = symbol;
            PropertyCollection = propertyCollection;


            #region 引数のpropertyCollectionからDeviceFormatにアクセス出来るかのテスト
            object BaseObj = symbol;
            bool IsEnabled = true;

            foreach (NestedProperty pd in propertyCollection)
            {
                object GetObj = pd.Property.GetValue(BaseObj);

                // 取得した値がnull
                if (GetObj == null)
                {
                    IsEnabled = false;
                    break;
                }

                if (pd.ArrayIndex == -1)
                    BaseObj = GetObj;
                else
                    BaseObj = ((Array)GetObj).GetValue(pd.ArrayIndex);
            }

            if (IsEnabled == false || ((BaseObj is DeviceFormat) == false))
            {
                throw new ArgumentException("指定の\"propertyCollection\"からDeviceFormatにアクセスすることが出来ません。");
            }
            #endregion
        }


        [Browsable(false)]
        protected virtual Symbol_Draw Symbol { get; set; }

        [Browsable(false)]
        protected virtual NestedProperty[] PropertyCollection { get; set; }

        [Browsable(false)]
        protected virtual BaseForm BaseForm { get; set; }

        private DeviceFormat Device
        {
            get
            {
                object BaseObj = Symbol;

                foreach (NestedProperty pd in PropertyCollection)
                {
                    object GetObj = pd.Property.GetValue(BaseObj);

                    if (pd.ArrayIndex == -1)
                        BaseObj = GetObj;
                    else
                        BaseObj = ((Array)GetObj).GetValue(pd.ArrayIndex);
                }

                DeviceFormat Device = BaseObj as DeviceFormat;

                if (Device != null)
                {
                    return Device;
                }
                else
                {
                    throw new InvalidOperationException("DeviceFormatにアクセスすることが出来ません。");
                }
            }
        }


        #region IIoListItem メンバ

        public object FormPeculiarObject
        {
            get { return BaseForm; }
        }

        public string PropertyName
        {
            get { return PropertyCollection[0].Property.Name; }
        }

        public int BaseNumber
        {
            get { return BaseForm.Number; }
        }

        public string SymbolName
        {
            get { return Symbol.SymbolName; }
        }

        public string DisplayPropertyName
        {
            get { return Global.GetDisplayName(Symbol, PropertyCollection[0].Property); }
        }

        public string Address
        {
            get
            {
                return this.Device.Address;
            }
        }

        public string SymbolType
        {
            get { return Symbol.SymbolType; }
        }

        #endregion
    }


    public class NestedProperty
    {
        public NestedProperty()
        {
            ArrayIndex = -1;
        }
        public NestedProperty(PropertyDescriptor property)
        {
            Property = property;
            ArrayIndex = -1;
        }
        public NestedProperty(PropertyDescriptor property, int arrayIndex)
        {
            Property = property;
            ArrayIndex = arrayIndex;
        }

        public PropertyDescriptor Property { get; protected set; }

        public int ArrayIndex { get; protected set; }

    }
}

