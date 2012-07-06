using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonClassLibrary;

namespace LocalSimulator.MainProgram
{
    public partial class BaseForm : Form, IBaseForm
    {
        public BaseForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        #region IBaseForm メンバ

        public string TitleName
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        private int number;
        public int Number
        {
            get
            {
                return this.number;
            }
            set
            {
                this.number = value;
            }
        }
        private Size _BaseFormSize;
        public Size BaseFormSize
        {
            get
            {
                return _BaseFormSize;
            }
            set
            {
                _BaseFormSize = value;
                this.Size = new Size((int)(_BaseFormSize.Width * MainForm.Instance.Zoom), (int)(_BaseFormSize.Height * MainForm.Instance.Zoom));
            }
        }

        #endregion


        public List<IDrawing> shapeCollection = new List<IDrawing>();


        #region イベント処理

        /// <summary>
        /// BaseForm.FormClosingイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                //this.Hide();
            }            
        }

        /// <summary>
        /// BaseForm.MouseDownイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IOMonitorSymbolFormat monitorForm = new IOMonitorSymbolFormat();
                monitorForm.BaseFormNumber = this.Number;               // BaseForm番号
                monitorForm.SymbolName = null;                          // モニタ対象シンボル名

                // MonitorSymbolFormat.SymbolNameがnullのとき、
                // シンボル選択フォームが表示される。

                DoDragDrop(monitorForm, DragDropEffects.Copy);

                return;
            }
        }

        /// <summary>
        /// BaseForm.Paintイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(this.DisplayRectangle.X, this.DisplayRectangle.Y);
            foreach (IDrawing ShapeData in this.shapeCollection)
            {
                ShapeData.Draw(e.Graphics, MainForm.Instance.Zoom);
            }
        }

        /// <summary>
        /// BaseForm.Scrollイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseForm_Scroll(object sender, ScrollEventArgs e)
        {
            this.Refresh();
        }

        /// <summary>
        /// BaseForm.SizeChangedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseForm_SizeChanged(object sender, EventArgs e)
        {
            // 自身が最大化表示の時のみ、AutoScrollを行う
            // (通常表示時にはスクロールバーを出したくないため)
            bool isAutoScroll = (this.WindowState == FormWindowState.Maximized);
            this.AutoScroll = isAutoScroll;
        }

        /// <summary>
        /// Symbol_Draw.MouseDownイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Symbol_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Symbol_Draw symbol = GetTopLevelControl((Control)sender) as Symbol_Draw;

                if (symbol != null)
                {
                    IOMonitorSymbolFormat monitorSymbol = new IOMonitorSymbolFormat();
                    monitorSymbol.BaseFormNumber = this.Number;                 // BaseForm番号
                    monitorSymbol.SymbolName = symbol.SymbolName;               // モニタ対象シンボル名

                    DoDragDrop(monitorSymbol, DragDropEffects.Copy);

                    return;
                }
            }
        }

        /// <summary>
        /// Symbol_Draw.MouseHoverイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Symbol_MouseHover(object sender, EventArgs e)
        {
            Symbol_Draw symbol = GetTopLevelControl((Control)sender) as Symbol_Draw;

            // Symbol_Draw以外
            if (symbol == null) { return; }

            // ToolTip表示文字列の作成
            string toolTipString = "[" + symbol.SymbolName + "]" + "\n\n";

            ToolTipViewData t = new ToolTipViewData(symbol);
            toolTipString += t.GetToolTipStringFromDevice();

            // 最後の改行を削除
            toolTipString = toolTipString.TrimEnd(new char[] { '\n' });

            toolTip1.SetToolTip((Control)sender, toolTipString);
        }

        /// <summary>
        /// toolTip1.Drawイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolTip1_Draw(object sender, DrawToolTipEventArgs e)
        {
            Font f = new Font("ＭＳ ゴシック", 8);

            e.DrawBackground();
            e.Graphics.DrawString(e.ToolTipText, f, Brushes.Black, new Point(5, 5));
            e.DrawBorder();
        }

        /// <summary>
        /// toolTip1.Popupイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            Graphics g = this.CreateGraphics();
            Font f = new Font("ＭＳ ゴシック", 8);
            string text = toolTip1.GetToolTip(e.AssociatedControl);
            StringFormat sf = new StringFormat();
            SizeF stringSize = g.MeasureString(text, f, 1000, sf);
            e.ToolTipSize = new Size((int)stringSize.Width + 10, (int)stringSize.Height + 10);

            g.Dispose();

        }

        #endregion


        public void Deserizlie(BaseFormDataSerializeFormat baseFormData)
        {
            #region ShapeObjectロード

            shapeCollection = new List<IDrawing>();

            foreach (ShapeDataSerializeFormat shapeData in baseFormData.ShapeData)
            {
                ShapeTypeFormat myShape = StoreShapeData(shapeData.PropertyData);

                switch (shapeData.Type)
                {
                    case "CircleObject":
                        shapeCollection.Add(new CircleDrawing(
                            myShape.BorderColor, myShape.BorderWidth, myShape.BorderStyle, myShape.FillColor,
                            new List<Point>(new Point[] { myShape.Location, myShape.Location.Add(myShape.Size) }), false));
                        break;

                    case "LineObject":
                        shapeCollection.Add(new LineDrawing(
                            myShape.BorderColor, myShape.BorderWidth, myShape.BorderStyle,
                            myShape.LineColor, myShape.LineWidth, myShape.LineStyle,
                            new List<Point>(new Point[] { myShape.StartPoint, myShape.EndPoint })));
                        break;

                    case "LinesObject":
                        shapeCollection.Add(new LinesDrawing(
                            myShape.BorderColor, myShape.BorderWidth, myShape.BorderStyle,
                            myShape.LineColor, myShape.LineWidth, myShape.LineStyle, myShape.Points.Items));
                        break;

                    case "PolygonObject":
                        shapeCollection.Add(new PolygonDrawing(
                            myShape.BorderColor, myShape.BorderWidth, myShape.BorderStyle,
                            myShape.FillColor, myShape.Points.Items));
                        break;

                    case "SquareObject":
                        shapeCollection.Add(new SquareDrawing(
                            myShape.BorderColor, myShape.BorderWidth, myShape.BorderStyle, myShape.FillColor,
                            new List<Point>(new Point[] { myShape.Location, myShape.Location.Add(myShape.Size) })));
                        break;

                    case "TextObject":
                        shapeCollection.Add(new TextDrawing(
                            myShape.BorderColor, myShape.BorderWidth, myShape.BorderStyle,
                            myShape.FillColor, myShape.TextColor, myShape.TextString,
                            myShape.TextFont, myShape.TextAlignment,
                            new List<Point>(new Point[] { myShape.Location, myShape.Location.Add(myShape.Size) })));
                        break;
                }
            }

            #endregion

            #region SymbolObjectロード

            IEnumerable<SymbolDataSerializeFormat> serializeSymbols = baseFormData.SymbolData;

            foreach (SymbolDataSerializeFormat symbolData in serializeSymbols.Reverse())
            {
                // シンボルのインスタンスを生成する
                Symbol_Draw symbol = SerializeSupport.Deserialize(symbolData) as Symbol_Draw;

                // 生成に成功したシンボルを配置する
                if (symbol != null)
                {
                    // マウスイベントの登録
                    RegisterMouseEvent(symbol);

                    this.Controls.Add(symbol);
                }
            }

            #endregion
        }


        private ShapeTypeFormat StoreShapeData(List<PropertyDataSerializeFormat> shapeObjectPropertyList)
        {
            ShapeTypeFormat myShape = new ShapeTypeFormat();
            PropertyDescriptorCollection myShapeProperties = TypeDescriptor.GetProperties(myShape);

            foreach (PropertyDataSerializeFormat property in shapeObjectPropertyList)
            {
                if (property.Name == "Type") { continue; }

                PropertyDescriptor myShapeProperty = myShapeProperties[property.Name];
                TypeConverter converter = TypeDescriptor.GetConverter(myShapeProperty.PropertyType);
                if (myShapeProperty != null)
                {
                    myShapeProperty.SetValue(myShape, converter.ConvertFrom(property.Value));
                }
            }

            return myShape;
        }

        private void RegisterMouseEvent(Control control)
        {
            control.MouseDown += new MouseEventHandler(Symbol_MouseDown);
            control.MouseHover += new EventHandler(Symbol_MouseHover);

            foreach (Control childControl in control.Controls)
            {
                RegisterMouseEvent(childControl);
            }
        }

        private Control GetTopLevelControl(Control control)
        {
            System.Diagnostics.Debug.Assert(control != null);

            if ((control.Parent == null) || (control.Parent == this))
            {
                return control;
            }
            else
            {
                return GetTopLevelControl(control.Parent);
            }
        }


        private class ToolTipViewData
        {
            private Symbol_Draw Symbol;
            private string NestPropertyName = null;
            private Dictionary<string, DeviceFormat> DeviceFormatPropertyList = new Dictionary<string, DeviceFormat>(); //プロパティ名，デバイス名

            public ToolTipViewData(Symbol_Draw symbol)
            {
                this.Symbol = symbol;

                //デバイスフォーマットプロパティリスト作成
                PropertyDescriptorCollection allProperties = TypeDescriptor.GetProperties(this.Symbol);

                foreach (PropertyDescriptor pd in allProperties)
                {
                    if (!Global.GetVisibleAttribute(this.Symbol, pd)) { continue; }
                    NestPropertyName = pd.Category + ".";
                    object nestObj = pd.GetValue(this.Symbol);
                    MakeDeviceFormatPropertyList(nestObj, pd.DisplayName);
                }

            }

            public void MakeDeviceFormatPropertyList(object obj, string name)
            {
                if (obj == null) { return; }

                if (obj.GetType() == typeof(DeviceFormat))
                {
                    NestPropertyName += name;
                    DeviceFormatPropertyList.Add(NestPropertyName, ((DeviceFormat)obj));
                }
                //配列型
                else if (obj.GetType().IsArray)
                {
                    string arrayName = NestPropertyName += name;

                    IList list = ((IList)(obj));

                    for (int i = 0; i < list.Count; i++)
                    {
                        NestPropertyName = arrayName;
                        //判定再帰呼び出し
                        MakeDeviceFormatPropertyList(list[i], "[" + i.ToString() + "]");
                    }


                }

                //クラス型
                else if (obj.GetType().IsClass)
                {
                    string className = NestPropertyName += name + ".";

                    PropertyDescriptorCollection allProperties = TypeDescriptor.GetProperties(obj);

                    foreach (PropertyDescriptor pd in allProperties)
                    {
                        NestPropertyName = className;
                        object nestObj = pd.GetValue(obj);

                        MakeDeviceFormatPropertyList(nestObj, pd.DisplayName);
                    }
                }
                else
                {
                    NestPropertyName = null;
                }


            }

            public string GetToolTipStringFromDevice()
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

                //プロパティ名最大文字数算出
                int keyMaxLength = 0;
                foreach (string key in DeviceFormatPropertyList.Keys)
                {
                    if (sjisEnc.GetByteCount(key) > keyMaxLength) { keyMaxLength = sjisEnc.GetByteCount(key); }
                }

                //デバイス名最大文字数算出
                int valueMaxLength = 0;
                foreach (DeviceFormat value in DeviceFormatPropertyList.Values)
                {
                    if (value.Address != null)
                    {
                        if (sjisEnc.GetByteCount(value.Address) > valueMaxLength) { valueMaxLength = sjisEnc.GetByteCount(value.Address); }
                    }
                }

                string totalString = null;
                foreach (KeyValuePair<string, DeviceFormat> pair in DeviceFormatPropertyList)
                {
                    if (String.IsNullOrEmpty(pair.Value.Address) == true) { continue; }

                    //プロパティ名空白作成
                    int keySpaceLenB = keyMaxLength - sjisEnc.GetByteCount(pair.Key);
                    string keySpace = "";
                    for (int i = 0; i < keySpaceLenB; i++) { keySpace += " "; }

                    //デバイス名空白作成
                    int valueSpaceLenB = valueMaxLength - sjisEnc.GetByteCount(pair.Value.Address);
                    string valueSpace = "";
                    for (int i = 0; i < valueSpaceLenB; i++) { valueSpace += " "; }

                    if (pair.Value.Io != IoType.Out)
                    {
                        totalString += pair.Key + keySpace + ":" + pair.Value.Address + valueSpace + " ･･･ " + pair.Value.Value[0] + "\n";
                    }
                    else
                    {
                        totalString += pair.Key + keySpace + ":" + pair.Value.Address + valueSpace + " ･･･ ---" + "\n";
                    }
                }
                return totalString;
            }

        }
    }
}
