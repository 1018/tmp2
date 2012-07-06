using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace CommonClassLibrary
{
    [Serializable, TypeConverter(typeof(PropertyConverter))]
    public class Symbol_Draw : UserControl, ICloneable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Symbol_Draw()
        {
            // 派生クラスにてSymbolTypeが設定されていなければ、
            // SymbolTypeは「型名 - "_Draw"」とする。
            if (String.IsNullOrEmpty(SymbolType))
            {
                string typeName = this.GetType().Name;
                SymbolType = typeName.Substring(0, typeName.IndexOf("_Draw"));
            }
        }



        private bool _WidthStretch = true;
        private bool _HeightStretch = true;
        private string _SymbolName = null;
        private Size _Size;
        private Point _Location;
        private double _Zoom = 1;
        private bool _IsDeserializing = false;
        //private Thread _CyclicThread = null;
        //private bool _CyclicRunning = false;

        //サイクリック処理の有無指定
        protected bool CyclicFlag = false;

        public event EventHandler RedrawOrder;

        public void OnRedrawOrder(EventArgs e)
        {
            if (RedrawOrder != null)
            {
                RedrawOrder(this, e);
            }
        }
 

        public bool Cyclic
        {
            get
            {
                return CyclicFlag;
            }

        }

        #region プロパティ
        
        [Category("00 種別")]
        [DisplayName("名称")]
        [Visible(true)]
        public string SymbolName
        {
            get { return _SymbolName; }
            set
            {
                _SymbolName = value;
            }
        }

        [Category("90 変形許可")]
        [Visible(true)]
        [DisplayName("横変形許可")]
        public bool WidthStretch
        {
            get { return _WidthStretch; }
            set { _WidthStretch = value; }
        }

        [Category("90 変形許可")]
        [Visible(true)]
        [DisplayName("縦変形許可")]
        public bool HeightStretch
        {
            get { return _HeightStretch; }
            set { _HeightStretch = value; }

        }

        [Category("95 配置")]
        [Visible(true)]
        [DisplayName("サイズ")]
        public new Size Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _Size = value;
                DrawSize = ZoomManager.MagnifySize(_Size, Zoom);
            }

        }

        [Category("95 配置")]
        [Visible(true)]
        [DisplayName("位置")]
        public new Point Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
                DrawLocation = ZoomManager.MagnifyPoint(_Location, Zoom);
            }
        }

        [Category("00 種別")]
        [Visible(true)]
        [DisplayName("シンボルタイプ")]
        public string SymbolType { get; protected set; }

        [Category("00 種別")]
        [Visible(true)]
        [DisplayName("カテゴリ")]
        public string Category { get; protected set; }
        
        public double Zoom
        {
            get
            {
                return _Zoom;
            }
            set
            {
                if (_Zoom != value)
                {
                    _Zoom = value;

                    ZoomChanged();

                    DrawLocation = ZoomManager.MagnifyPoint(Location, _Zoom);
                    DrawSize = ZoomManager.MagnifySize(Size, _Zoom);
                }
            }
        }

        [Visible(false)]
        public Size DrawSize
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        [Visible(false)]
        public Point DrawLocation
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
            }
        }

        [Visible(false)]
        public bool IsDeserializing
        {
            get
            {
                return this._IsDeserializing;
            }
            set
            {
                this._IsDeserializing = value;
            }
        }

        #endregion

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        //private void BaseCyclicMethod()
        //{
        //  //while(true)
        //  while(_CyclicRunning)
        //  {
        //      this.Invoke((MethodInvoker)delegate()
        //      {
        //          CyclicMethod();
        //      });
        //      Thread.Sleep(100);
        //  }
        //}
        public void BaseCyclicMethod()
        {            
            this.Invoke((MethodInvoker)delegate()
            {
                CyclicMethod();
            });
            
        }

        /// <summary>
        /// 定周期で実行されるメソッドです。
        /// </summary>
        /// <remarks>
        /// このメソッドを使用するには、メンバ変数「CyclicFlag」にtrueをセットする必要があります。
        /// </remarks>
        public virtual void CyclicMethod()
        {

        }       

        /// <summary>
        /// 指定のアドレスに値を書き込みます。
        /// </summary>
        /// <param name="Address">値を書き込むアドレス</param>
        /// <param name="Value">書き込む値</param>
        protected void SetDeviceData(string Address, UInt16 Value)
        {
            SetDeviceData(Address, new UInt16[] { Value });
        }
        /// <summary>
        /// 先頭アドレスからデータ数分の値を書込みます。
        /// </summary>
        /// <param name="Address">値を書き込む先頭アドレス</param>
        /// <param name="Values">書き込む値</param>
        protected void SetDeviceData(string Address, UInt16[] Values)
        {
            Remoting Common = Remoting.Instance;
            Common.SetSendData(this, Address, Values.Length, new CalculatorSet(Values));           
        }
        /// <summary>
        /// 指定のアドレスに対し演算を行い、その結果を書込みます。
        /// </summary>
        /// <param name="Address">値を書き込む先頭アドレス</param>
        /// <param name="Operator">アドレスに対し、実行する演算子</param>
        /// <param name="Volume">演算する値</param>
        protected void SetDeviceCalculation(string Address, string Operator, UInt16 Volume)
        {
            Remoting Common = Remoting.Instance;

            switch (Operator)
            {
                case "+":
                case "＋":
                {
                    // 加算
                    Common.SetSendData(this, Address, 1, new CalculatorAdd(Volume));

                    break;
                }

                case "-":
                case "－":
                {
                    // 減算
                    Common.SetSendData(this, Address, 1, new CalculatorMinus(Volume));

                    break;
                }

                default:
                    throw new ArgumentException("SetSendOperation");
            }
        }
        /// <summary>
        /// デバイスリストを更新します。
        /// </summary>
        /// <remarks>
        /// シンボル内のDeviceFormatのアドレスを変更した時、デバイスリストを更新する必要があります。
        /// デバイスリストはSymbol_Draw.Initialが実行された後、一度だけ実行されます。
        /// Initial以降、アドレスの変更を適用するにはこの関数をシンボル側で呼び出して下さい。
        /// </remarks>
        protected void DataListMake()
        {
            Global.DataListMake();
        }

        //public void BaseInitial()
        //{
        //    if (CyclicFlag == true)
        //    {
        //        //Thread task = new Thread(new ThreadStart(BaseCyclicMethod));
        //        //task.IsBackground = true;
        //        //task.Start();

        //        _CyclicThread = new Thread(new ThreadStart(BaseCyclicMethod));
        //        _CyclicThread.IsBackground = true;

        //        _CyclicRunning = true;
        //        _CyclicThread.Start();
        //    }
        //    Initial();
        //}
        /// <summary>
        /// シンボルの初期化処理です。
        /// </summary>
        /// <remarks>
        /// ProjectMakerで設定されたプロパティをセット後、PLCとの通信を開始する前に一度だけ呼び出されます。
        /// </remarks>
        public virtual void Initial()
        {

        }        
        /// <summary>
        /// シンボルのズーム処理です。
        /// </summary>
        /// <remarks>
        /// ズーム設定が変更された時に呼び出されます。
        /// シンボルの外観に文字を表示させている場合、この関数内にフォントサイズを変更する処理を記述して下さい。
        /// 現在のズーム倍率はSymbol_Draw.Zoomに格納されます。
        /// </remarks>
        protected virtual void ZoomChanged()
        {
            
        }

        //public void BaseAbort()
        //{
        //    if (_CyclicRunning)
        //    {
        //        _CyclicRunning = false;

        //        // 停止待ちを入れると固まります(Invoke呼び出しなので)

        //        _CyclicThread = null;
        //    }
        //    Abort();
        //}
        public void BaseAbort()
        {
            Abort();
        }
        /// <summary>
        /// シンボルのアボート処理です。
        /// </summary>
        /// <remarks>
        /// PLCとの通信が切断された時に呼び出されます。
        /// </remarks>
        protected virtual void Abort()
        {
        }
    }

    public class PropertyConverter : TypeConverter
    {

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] filter)
        {
            
            List<PropertyDescriptor> Properties = new List<PropertyDescriptor>();
            PropertyDescriptorCollection BaseProperties = TypeDescriptor.GetProperties(value, filter, true);

             //"その他" が後ろにくるようにする
            foreach (PropertyDescriptor BaseProperty in BaseProperties)
            {
                if (BaseProperty.Category != "その他")
                {
                    Properties.Add(BaseProperty);
                }
            }
            foreach (PropertyDescriptor BaseProperty in BaseProperties)
            {
                if (BaseProperty.Category == "その他")
                {
                    Properties.Add(BaseProperty);
                }
            }

            return new PropertyDescriptorCollection(Properties.ToArray());
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
            //return false;
        }

        
    }    
 
}
