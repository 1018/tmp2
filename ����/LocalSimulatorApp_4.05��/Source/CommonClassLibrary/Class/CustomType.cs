using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;


namespace CommonClassLibrary
{
    #region PlcCommunication

    [Serializable]
    public delegate void PlcErrorEventHandler(object sender, PlcErrorEventArgs e);

    public interface IPlcUniqueData
    {
        /// <summary>
        /// PLCメーカー名
        /// </summary>
        /// <remarks>
        /// このプロパティはメーカー毎にDLLを区別するために存在します。
        /// したがって、このプロパティにより返される値は、
        /// いかなる状況においても変化しないことが期待されます。
        /// 
        /// （要は定数プロパティ）
        /// </remarks>
        string MakerName { get; }
    }    

    public interface IPlcCommunication : IPlcUniqueData
    {
        /// <summary>
        /// PLCとの通信を開始します。
        /// </summary>
        void Start();       
    }

    public interface IPlcSetting
    {
        /// <summary>
        /// 設定フォーム表示
        /// </summary>
        /// <param name="setting">設定データ</param>
        /// <returns>押下したボタンの種類</returns>
        ConnectionMode FormShow(ref object[] setting);       


    }

    [Serializable]
    public class PlcErrorEventArgs : EventArgs
    {
        public PlcErrorEventArgs() : this(string.Empty)
        {
        }

        public PlcErrorEventArgs(string errorContent)
        {
            this.errorText = errorContent;
        }


        private string errorText;


        public string Message
        {
            get
            {
                return this.errorText;
            }
            set
            {
                errorText = value;
            }
        }
    }

    public enum ConnectionMode
    {
        Cancel,Normal,Auto
    }

    #endregion


    #region カスタムデータフォーマット

    [TypeConverter(typeof(DeviceFormatConverter))]
    public class DeviceFormat : ICloneable
    {

        int _DataCount = 1;
        UInt16[] _Value = new UInt16[1];
        IoType _Io = IoType.Io;
        SetType _InputType = SetType.Word | SetType.Bit;

        #region コンストラクタ

        public DeviceFormat() { }

        //受信デバイス初期化時使用コンストラクタ
        public DeviceFormat(string _Address, int _DataCount)
        {
            Address = _Address;
            DataCount = _DataCount;
            Value = new UInt16[DataCount];
        }
        public DeviceFormat(int _DataCount)
        {
            DataCount = _DataCount;
            Value = new UInt16[DataCount];
        }

        //送信デバイス初期化使用時コンストラクタ
        public DeviceFormat(string _Address, UInt16[] _Value)
        {
            Address = _Address;
            DataCount = _Value.Length;
            Value = _Value;
        }

        //アドレス変更用コンストラクタ
        public DeviceFormat(string _Address) { Address = _Address; }

        public DeviceFormat(string _Address, IoType _Io, SetType _Type)
        {
            Address = _Address;
            Io = _Io;
            InputType = _Type;
        }

        public DeviceFormat(IoType _Io, SetType _Type)
        {
            Io = _Io;
            InputType = _Type;
        }

        public DeviceFormat(int _DataCount, IoType _Io, SetType _Type)
        {
            DataCount = _DataCount;
            Io = _Io;
            InputType = _Type;

            Value = new ushort[DataCount];
        }

        public DeviceFormat(string _Address, int _DataCount, IoType _Io, SetType _Type)
        {
            Address = _Address;
            DataCount = _DataCount;
            Io = _Io;
            InputType = _Type;

            Value = new ushort[DataCount];
        }

        #endregion

        [DisplayName("アドレス")]
        public string Address { get; set; }

        [DisplayName("データ数")]
        [Browsable(false), ReadOnly(true)]
        public int DataCount
        {
            get { return _DataCount; }
            set { _DataCount = value; }
        }

        [DisplayName("値")]
        [Browsable(false)]
        public UInt16[] Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        [DisplayName("IN/OUT")]
        [Browsable(false), ReadOnly(true)]
        public IoType Io
        {
            get { return _Io; }
            set { _Io = value; }
        }

        [DisplayName("WORD/BIT")]
        [Browsable(false), ReadOnly(true)]
        public SetType InputType
        {
            get { return _InputType; }
            set { _InputType = value; }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
 
    }

    [TypeConverter(typeof(NestConverter))]
    public class SpanFormat : ICloneable
    {
        public SpanFormat() { }

        public SpanFormat(double _Min, double _Max)
        {
            Min = _Min;
            Max = _Max;
        }

        public double Min { get; set; }

        public double Max { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Serializable]
    public class RemotingDataFormat
    {
        public DeviceElement Device;
        public int DataCount;
        public UInt16[] Value;

        public RemotingDataFormat() { }
        public RemotingDataFormat(DeviceElement _Device, int _DataCount, UInt16[] _Value)
        {
            Device = _Device;
            DataCount = _DataCount;
            Value = _Value;
        }

        public override string ToString()
        {
            return Device + "=" + Value;
        }

    }

    public class RemotingMessageFormat : MarshalByRefObject
    {
        public string DeviceString;
        public int DataCount;
        public ICalculator Calculator;

        public RemotingMessageFormat() { }
        public RemotingMessageFormat(string deviceString, int dataCount, ICalculator calculator)
        {
            DeviceString = deviceString;
            DataCount = dataCount;
            Calculator = calculator;
        }
    }

    [Serializable]
    public class RecvDeviceFormat
    {
        public List<DataFormat> BitDevice_Block = new List<DataFormat>();
        public List<DataFormat> BitDevice_Single = new List<DataFormat>();
        public List<DataFormat> WordDevice_Block = new List<DataFormat>();
        public List<DataFormat> WordDevice_Single = new List<DataFormat>();
        public List<DataFormat> Buf_Block = new List<DataFormat>();
        public List<DataFormat> Buf_Single = new List<DataFormat>();
        public List<DataFormat> Virtual_Block = new List<DataFormat>();
        public List<DataFormat> Virtual_Single = new List<DataFormat>();
    }

    [Serializable]
    public class DataFormat
    {
        public DeviceElement Device;
        public int DataCount;
        public DataFormat() { }
        public DataFormat(DeviceElement _Device, int _DataCount)
        {
            Device = _Device;
            DataCount = _DataCount;
        }
    }    

    public class EventSymbolFormat
    {
        public DeviceElement Device;
        public int ArrayNumber;
        public int DataCount;
        public Symbol_Draw Symbol;
        public PropertyInfo Property;
        public EventSymbolFormat() { }
        public EventSymbolFormat(DeviceElement _Device, int _ArrayNumber, int _DataCount, Symbol_Draw _Symbol, PropertyInfo _Property)
        {
            Device = _Device;
            ArrayNumber = _ArrayNumber;
            DataCount = _DataCount;
            Symbol = _Symbol;
            Property = _Property;
        }
    }

    public class ShapeTypeFormat
    {
        public enum DrawType { Line, Circle, Square, Text, View, Lines, Polygon }

        public DrawType Type { get; set; }

        public string ShapeType { get; set; }

        public Point Location { get; set; }

        public Size Size { get; set; }

        public Point StartPoint { get; set; }

        public Point EndPoint { get; set; }

        public int BorderWidth { get; set; }

        public Color BorderColor { get; set; }

        public DashStyle BorderStyle { get; set; }

        public int LineWidth { get; set; }

        public Color LineColor { get; set; }

        public DashStyle LineStyle { get; set; }

        public Color FillColor { get; set; }

        public string TextString { get; set; }

        public Font TextFont { get; set; }

        public ContentAlignment TextAlignment { get; set; }


        public Color TextColor { get; set; }


        public MyList<Point> Points { get; set; }


        public int InsideBorderWidth { get; set; }


        public Color InsideBorderColor { get; set; }


        public DashStyle InsideBorderStyle { get; set; }



    }


    [TypeConverter(typeof(GenericListConverter))]
    public class MyList<T> : IList
    {
        private List<T> items = new List<T>();

        public MyList()
        {

        }

        public MyList(MyList<T> collection)
        {
            T[] temp = collection.Items.ToArray();

            this.Items.Clear();
            foreach (T item in collection)
            {
                items.Add(item);
            }
        }



        public T this[int i]
        {
            get
            {
                return items[i];
            }
            set
            {
                items[i] = value;
            }

        }

        public void Add(T item)
        {
            items.Add(item);
        }

        public void AddRange(IEnumerable<T> item)
        {
            items.AddRange(item);
        }

        public int Count
        {
            get { return items.Count; }
        }

        public List<T> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }

        #region IList メンバ

        public int Add(object value)
        {
            items.Add((T)value);
            return this.items.Count - 1;
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection メンバ

        public void CopyTo(Array array, int index)
        {
            T[] cvArray = null;
            Array.Copy(array, cvArray, array.Length);

            items.CopyTo(cvArray);

        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        #endregion

        #region IEnumerable メンバ

        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion


    }

    //微分クラス
    public class Dif
    {
        public class DifEventArgs : EventArgs
        {
            public bool Value;
        }

        public Dif()
        {
            Address = null;
            this.ValueChanged += new DifEventhandler(Dif_ValueChanged);
        }
        public Dif(string Address)
        {
            this.Address = Address;
            this.ValueChanged += new DifEventhandler(Dif_ValueChanged);
        }
        void Dif_ValueChanged(object sender, DifEventArgs e)
        {
            if (Address != null)
            {
                ushort[] values = new ushort[1];
                values[0] = (ushort)((e.Value == true) ? 1 : 0);
                Remoting Common = Remoting.Instance;
                Common.SetSendData(null, Address, 1, new CalculatorSet(values));
            }
        }
        public delegate void DifEventhandler(object sender, DifEventArgs e);
        public event DifEventhandler ValueChanged;
        private bool _Value;
        public bool Value
        {
            get { return _Value; }
            set
            {
                if (value != _Value)
                {
                    DifEventArgs e = new DifEventArgs();
                    e.Value = value;
                    OnValueChanged(e);
                }
                _Value = value;
            }
        }
        public string Address { get; set; }
        protected virtual void OnValueChanged(DifEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

    }



    

    #endregion


    #region 型コンバータ

    public class GenericListConverter : CollectionConverter
    {
        //コンバータがオブジェクトを指定した型に変換できるか
        //（変換できる時はTrueを返す）
        //ここでは、CustomClass型のオブジェクトには変換可能とする
        public override bool CanConvertTo(
            ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(DeviceFormat))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        //指定した値オブジェクトを、指定した型に変換する
        //CustomClass型のオブジェクトをString型に変換する方法を提供する
        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object obj, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string NewString = "";

                IList GenericList = (IList)obj;

                foreach (object Data in GenericList)
                {
                    string sObj = (string)CommonClassLibrary.SerializeSupport.ConvertTo(Data, typeof(string));

                    if (NewString == "")
                    {
                        NewString = "[" + Data.GetType().Name + "]" + sObj;
                    }
                    else
                    {
                        NewString = NewString + " , " + sObj;
                    }

                }


                return NewString;
            }
            return base.ConvertTo(context, culture, obj, destinationType);
        }

        //コンバータが特定の型のオブジェクトをコンバータの型に変換できるか
        //（変換できる時はTrueを返す
        //ここでは、String型のオブジェクトなら変換可能とする
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        //指定した値をコンバータの型に変換する
        //String型のオブジェクトをCustomClass型に変換する方法を提供する
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] ss = value.ToString().Split(new string[] { " , " }, System.StringSplitOptions.None);

                //タイプ抜き出し
                int TypeStart = ss[0].IndexOf("[") + 1;
                int TypeEnd = ss[0].IndexOf("]");
                string TypeString = ss[0].Substring(TypeStart, TypeEnd - TypeStart);
                string DataString = ss[0].Substring(TypeEnd + 1);
                ss[0] = DataString;

                Type ConvertType = null;

                if (TypeString == "Point")
                {
                    ConvertType = typeof(Point);
                }
                else
                {
                    ConvertType = Type.GetType(TypeString);
                }

                System.Type genericBaseType = typeof(MyList<>);
                Type genericType = genericBaseType.MakeGenericType(ConvertType);

                IList NewObject = Activator.CreateInstance(genericType) as IList;

                foreach (string s in ss)
                {
                    object obj = CommonClassLibrary.SerializeSupport.ConvertTo(s, ConvertType);

                    NewObject.Add(obj);

                }

                return NewObject;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class DeviceFormatConverter : TypeConverter
    {
        //コンバータがオブジェクトを指定した型に変換できるか
        //（変換できる時はTrueを返す）
        //ここでは、CustomClass型のオブジェクトには変換可能とする
        public override bool CanConvertTo(
            ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(DeviceFormat))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        //指定した値オブジェクトを、指定した型に変換する
        //CustomClass型のオブジェクトをString型に変換する方法を提供する
        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object obj, Type destinationType)
        {
            if (destinationType == typeof(string) && obj is DeviceFormat)
            {
                DeviceFormat cc = (DeviceFormat)obj;

                if (string.IsNullOrEmpty(cc.Address))
                {
                    cc.Address = string.Empty;
                }

                return cc.Address;
            }
            return base.ConvertTo(context, culture, obj, destinationType);
        }

        //コンバータが特定の型のオブジェクトをコンバータの型に変換できるか
        //（変換できる時はTrueを返す
        //ここでは、String型のオブジェクトなら変換可能とする
        public override bool CanConvertFrom(
            ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        //指定した値をコンバータの型に変換する
        //String型のオブジェクトをCustomClass型に変換する方法を提供する
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is string)
            {               
                int commaIndex = ((string)value).IndexOf(',');
                DeviceFormat returnDevice = new DeviceFormat();

                if (commaIndex == -1)
                    returnDevice.Address = (string)value;
                else
                    returnDevice.Address = ((string)value).Substring(0, commaIndex);

                if (context != null && context.PropertyDescriptor != null && context.Instance != null)
                {
                    DeviceFormat baseDeviceFormat = (DeviceFormat)context.PropertyDescriptor.GetValue(context.Instance);

                    returnDevice.DataCount = baseDeviceFormat.DataCount;
                    returnDevice.Value = baseDeviceFormat.Value;
                    returnDevice.Io = baseDeviceFormat.Io;
                    returnDevice.InputType = baseDeviceFormat.InputType;
                }

                return returnDevice;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class SpanFormatConverter : ExpandableObjectConverter
    {
        //コンバータがオブジェクトを指定した型に変換できるか
        //（変換できる時はTrueを返す）
        //ここでは、CustomClass型のオブジェクトには変換可能とする
        public override bool CanConvertTo(
            ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(SpanFormat))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        //指定した値オブジェクトを、指定した型に変換する
        //CustomClass型のオブジェクトをString型に変換する方法を提供する
        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) &&
                value is SpanFormat)
            {
                SpanFormat cc = (SpanFormat)value;
                return cc.Min.ToString() + "," + cc.Max;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        //コンバータが特定の型のオブジェクトをコンバータの型に変換できるか
        //（変換できる時はTrueを返す）
        //ここでは、String型のオブジェクトなら変換可能とする
        public override bool CanConvertFrom(
            ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        //指定した値をコンバータの型に変換する
        //String型のオブジェクトをCustomClass型に変換する方法を提供する
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] ss = value.ToString().Split(new char[] { ',' }, 2);
                SpanFormat cc = new SpanFormat();
                //cc.Min = Convert.ToDouble(ss[0]);
                //cc.Max = Convert.ToDouble(ss[1]);
                cc.Min = Convert.ToDouble(ss[0].Trim());
                cc.Max = Convert.ToDouble(ss[1].Trim());
                return cc;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }    

    public class NestConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            //try
            //{
            if (context != null && value is string)
            {
                return ConvertFromString(context, context.PropertyDescriptor.PropertyType, (string)value);
            }
            
                return base.ConvertFrom(null, culture, value);
            //}
            //catch
            //{
            //    return context.Instance;
            //}
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null)
            {
                return ConvertToString(value);
            }
            else if (destinationType == typeof(string) && value == null)
            {
                if (context != null)
                {
                    //GenericList
                    if (context.Instance.GetType().IsGenericType)
                    {
                        string propertyName = context.PropertyDescriptor.Name;
                        int index = ((ElementPropertyDescriptor)context.PropertyDescriptor).Index;

                        IList ar = (IList)context.Instance;

                        return ConvertToString(ar[index]);
                    }
                }
                
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private new string ConvertToString(object value)
        {
            string convertString = "[";

            //配列の場合
            if (value.GetType().IsArray)
            {
                Array arrayValue = (Array)value;

                foreach (object oneValue in arrayValue)
                {
                    string memberString;

                    if (oneValue != null)
                    {
                        TypeConverter tc = TypeDescriptor.GetConverter(oneValue.GetType());

                        if (tc.GetType() == typeof(TypeConverter) || tc.GetType() == typeof(ArrayConverter)) { tc = new NestConverter(); }

                        memberString = tc.ConvertToString(oneValue);
                        //カンマ有り，括弧無しの場合に括弧を追加する。
                        if (memberString.IndexOf(",") != -1 && memberString.IndexOf("[") == -1)
                        {
                            memberString = "[" + memberString + "]";
                        }
                    }
                    else
                    {
                        memberString = "";
                    }
                    convertString += memberString + " ,";


                }
                convertString = convertString.Substring(0, convertString.Length - 2);
            }
            //GenericListの場合
            else if (value.GetType().IsGenericType)
            {
                IList arrayValue = (IList)value;
                
                foreach (object oneValue in arrayValue)
                {
                    string memberString;

                    if (oneValue != null)
                    {
                        TypeConverter tc = TypeDescriptor.GetConverter(oneValue.GetType());

                        if (tc.GetType() == typeof(TypeConverter) || tc.GetType() == typeof(ArrayConverter)) { tc = new NestConverter(); }

                        memberString = tc.ConvertToString(oneValue);
                        //カンマ有り，括弧無しの場合に括弧を追加する。
                        if (memberString.IndexOf(",") != -1 && memberString.IndexOf("[") == -1)
                        {
                            memberString = "[" + memberString + "]";
                        }
                    }
                    else
                    {
                        memberString = "";
                    }
                    convertString += memberString + " ,";
                    
                    
                }
                convertString = convertString.Substring(0, convertString.Length - 2);
                

            }
            else
            {
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value);

                foreach (PropertyDescriptor pd in pdc)
                {
                    if (pd.IsBrowsable == false) { continue; }
                    TypeConverter tc = pd.Converter;

                    if (tc.GetType() == typeof(TypeConverter) || tc.GetType() == typeof(ArrayConverter)) { tc = new NestConverter(); }

                    object memberValue = pd.GetValue(value);
                    string memberString = tc.ConvertToString(memberValue);

                    //カンマ有り，括弧無しの場合に括弧を追加する。
                    if (memberString.IndexOf(",") != -1 && memberString.IndexOf("[") == -1)
                    {
                        memberString = "[" + memberString + "]";
                    }
                    convertString += memberString + " ,";

                }
                if (convertString.Length >= 2)
                {
                    convertString = convertString.Substring(0, convertString.Length - 2);
                }
            }
            
            convertString += "]";
            //System.Diagnostics.Debug.Write(convertString + "\n");
            return convertString;
        }
       

        private object ConvertFromString(ITypeDescriptorContext context, Type destinationType, string source)
        {
            string value = null;
            if (source == "" || source.Substring(0, 1) != "[")
            {
                value = source;
            }
            else
            {
                value = source.Substring(1, source.Length - 2);
            }

            // 配列の終端の値が空白の時、要素数が減るバグの対応
            value += ",";

            #region 配列の場合
            if (destinationType.IsArray || destinationType.IsGenericType)
            {
                int bracketCount = StringCount(value, '[');
                Type elementType;
                if (destinationType.IsArray)
                {
                    elementType = destinationType.GetElementType();
                }
                else
                {
                    Type[] obj = destinationType.GetGenericArguments();
                    elementType = obj[0];
                }

                ArrayList ar = new ArrayList();

                TypeConverter tc = TypeDescriptor.GetConverter(elementType);

                if (tc.GetType() == typeof(TypeConverter) || tc.GetType() == typeof(ArrayConverter)) { tc = new NestConverter(); }
                
                while (true)
                {
                    //1要素取り出し
                    string oneElement = GetOneElement(ref value);

                    if (tc.GetType() != typeof(NestConverter) && oneElement.Length >= 2 && oneElement.Substring(0, 1) == "[")
                    {
                        oneElement = oneElement.Substring(1, oneElement.Length - 2);
                    }

                    object convValue;

                    if (tc.GetType() == typeof(NestConverter))
                    {
                        convValue = ConvertFromString(null, elementType, oneElement);
                    }
                    else
                    {
                        convValue = tc.ConvertFrom(oneElement);
                    }
                    
                    ar.Add(convValue);

                    if (value == "") { break; }
                    
                }
                if (destinationType.IsArray)
                {
                    Array arrayObject = Array.CreateInstance(elementType, ar.Count);
                    Array.Copy(ar.ToArray(), arrayObject, ar.Count);
                    return arrayObject;
                }
                else
                {
                    IList list = Activator.CreateInstance(destinationType) as IList;
                    foreach (object obj in ar)
                    {
                        list.Add(obj);
                    }

                    return list;
                }

                

            }
            #endregion

            else if (destinationType.IsGenericType)
            {
                return null;
            }


            #region 配列でない場合
            else
            {
                //インスタンス作成
                object instance = Activator.CreateInstance(destinationType);

                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);

                foreach (PropertyDescriptor pd in properties)
                {
                    if (!pd.IsBrowsable) { continue; }
                    if (pd.IsReadOnly) { continue; }

                    //1要素取り出し
                    string oneElement = GetOneElement(ref value);

                    int bracketCount = StringCount(oneElement, '[');

                    #region [ が1つ有る場合はClass
                    if (bracketCount == 1)
                    {
                        oneElement = oneElement.Substring(1, oneElement.Length - 2);

                        TypeConverter tc = pd.Converter;
                        if (tc.GetType() == typeof(TypeConverter) ||
                            tc.GetType() == typeof(ArrayConverter) ||
                            tc.GetType() == typeof(CollectionConverter))
                        { tc = new NestConverter(); }
                        TypeDescriptorContext elementContext = new TypeDescriptorContext(pd);
                        object convValue = tc.ConvertFrom(elementContext, null, oneElement);
                        pd.SetValue(instance, convValue);
                    }
                    #endregion

                    #region [ が2つ以上有る場合はNest
                    else if (bracketCount > 1)
                    {
                        //Nest処理
                        TypeDescriptorContext elementContext = new TypeDescriptorContext(pd);
                        object valueObject = ConvertFromString(elementContext, pd.PropertyType, oneElement);
                        pd.SetValue(instance, valueObject);

                    }
                    #endregion

                    #region [ が無い場合
                    else
                    {
                        try
                        {
                            TypeConverter tc = pd.Converter;
                            object convValue = tc.ConvertFrom(oneElement);
                            pd.SetValue(instance, convValue);
                        }
                        catch
                        {
                            Global.LogManager.Write("プロパティの変換に失敗しました。 " + instance.ToString() + "." + pd.Name + " value:" + oneElement);
                        }

                    }
                    #endregion

                }

                return instance;
            }
            #endregion
            

        }

        /// <summary>
        /// 文字列内にある指定した文字の数を戻す
        /// </summary>
        /// <param name="target">対象文字列</param>
        /// <param name="findChar">検索文字</param>
        /// <returns>文字の数</returns>
        private int StringCount(string target, char findChar)
        {
            int count = 0;
            for (int i = 0; i < target.Length; i++)
            {
                if (target.Substring(i, 1) == findChar.ToString()) { count++; }
            }            

            return count;
        }

        /// <summary>
        /// 1要素取り出し
        /// </summary>
        /// <param name="target">取り出す文字列</param>
        /// <returns>取り出した文字列</returns>
        private string GetOneElement(ref string target)
        {
            string[] answer = new string[2];

            int index = -1; //1要素取り出し位置
            int count = 0; //括弧カウント [ で+1  ]で-1

            for (int i = 0; i < target.Length; i++)
            {
                string oneStr = target.Substring(i, 1);

                if (oneStr == "[") { count++; }
                if (oneStr == "]") { count--; }

                //括弧無しのカンマにて1要素とする。
                if (count == 0 && oneStr == ",")
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                answer[0] = target.Substring(0, index).Trim();
                answer[1] = target.Substring(index + 1);

            }
            else
            {
                answer[0] = target;
                answer[1] = "";
            }

            target = answer[1];

            return answer[0].Trim();
        }

        


    }

    

    // Enumのフィールドに付けられた名前を読み出す
    public class EnumDisplayConverter : EnumConverter
    {
        public EnumDisplayConverter(Type EnumType) : base(EnumType) { }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                string convertName = (string)value;
                IEnumerable<string> enumTypeValueNames = Enum.GetNames(base.EnumType);

                foreach (string valueName in enumTypeValueNames)
                {
                    FieldInfo valueInfo = base.EnumType.GetField(valueName);
                    Attribute attrEnumDisplayName = Attribute.GetCustomAttribute(valueInfo, typeof(EnumDisplayNameAttribute));

                    if (attrEnumDisplayName != null)
                    {
                        string displayName = ((EnumDisplayNameAttribute)attrEnumDisplayName).DisplayName;

                        if (convertName == displayName)
                        {
                            return Enum.Parse(base.EnumType, valueInfo.Name);
                        }
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string name = Enum.GetName(base.EnumType, value);
                FieldInfo fi = base.EnumType.GetField(name);

                Attribute attr = Attribute.GetCustomAttribute(fi, typeof(EnumDisplayNameAttribute));
                if (attr != null)
                {
                    return ((EnumDisplayNameAttribute)attr).DisplayName;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }    

    #endregion

    public class ElementPropertyDescriptor : PropertyDescriptor
    {
        private int index;        
        private IList componentList;
        private object component;
        public ElementPropertyDescriptor(object component, string name, int index) :
            base(name + "[" + index + "]", new Attribute[] { })
        {
            this.index = index;
            this.componentList = ConvertArray(component);
            this.component = component;
        }

        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override Type ComponentType
        {
            get { return componentList.GetType(); }
        }

        public override object GetValue(object component)
        {            
            return this.componentList[index];
        }

        public override bool IsReadOnly
        {
            get
            {
                foreach (Attribute atr in AttributeArray)
                {
                    if (atr.GetType() == typeof(ReadOnlyAttribute)) { return true; }
                }

                return false;
            }
        }

        public override Type PropertyType
        {
            get 
            {
                if (componentList[index] == null)
                {
                    return typeof(string);
                }

                if (component.GetType().IsArray)
                {
                    return component.GetType().GetElementType();
                }
                else
                {                    
                    return componentList[index].GetType(); 
                }                
            }
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            if (component.GetType().IsGenericType)
            {
                ((IList)component)[index] = value;
            }
            else
            {
                ((Array)component).SetValue(value, index);
            }            
            this.componentList[index] = value;            

        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        protected override Attribute[] AttributeArray
        {
            get
            {
                return base.AttributeArray;
            }
            set
            {
                base.AttributeArray = value;
            }
        }

        public Attribute[] AttributeArray2
        {
            get { return AttributeArray; }
            set { AttributeArray = value; }
        }
        public int Index
        {
            get { return index; }
        }

        private IList ConvertArray(object component)
        {
            IList result = new List<object>();

            if (component.GetType().IsGenericType)
            {
                result = (IList)component;
            }
            else
            { 
                Array ar = (Array)component;
                for (int i = 0; i < ar.Length; i++)
                {
                    result.Add(ar.GetValue(i));
                }
            }

            return result;


        }
    
    }


    #region カスタム属性

    // 属性 : PropertyGridに表示する
    [AttributeUsage(AttributeTargets.Property)]
    public class VisibleAttribute : Attribute
    {
        public bool IsVisible { get; set; }

        public VisibleAttribute()
        {
            IsVisible = true;
        }

        public VisibleAttribute(bool _IsVisible)
        {
            IsVisible = _IsVisible;
        }
    }

    // 属性 : 配列エディット許可
    [AttributeUsage(AttributeTargets.Property)]
    public class EditArrayAttribute : Attribute
    {
        // trueならユーザーが配列を操作出来る
        public bool IsEditArray { get; set; }

        public static readonly EditArrayAttribute Yes;
        public static readonly EditArrayAttribute No;
        public static readonly EditArrayAttribute Default;

        static EditArrayAttribute()
        {
            Yes = new EditArrayAttribute(true);
            No = new EditArrayAttribute(false);
            Default = new EditArrayAttribute(false);
        }
        public EditArrayAttribute()
        {
            this.IsEditArray = true;
        }

        public EditArrayAttribute(bool isEditArray)
        {
            this.IsEditArray = isEditArray;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            EditArrayAttribute attribute = obj as EditArrayAttribute;
            return (attribute != null) && (attribute.IsEditArray == this.IsEditArray);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool IsDefaultAttribute()
        {
            return this.IsEditArray == Default.IsEditArray;
        }
    }

    // 属性 : 速度プロパティ
    [AttributeUsage(AttributeTargets.Property)]
    public class SpeedPropertyAttribute : Attribute
    {
        // 搬送可能シンボルに速度プロパティを判別させる
    }

    // 属性 : Enumの各フィールドに名前を付ける
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public EnumDisplayNameAttribute(string displayName)
        {
            this.DisplayName = displayName;
        }
    }

    #endregion


    [Flags]
    public enum IoType
    {
        In  = 0x0001,
        Out = 0x0002,
        Io  = In | Out,
    }

    [Flags]
    public enum SetType
    {
        Bit  = 0x0001,
        Word = 0x0002,
    }

    public enum BitStatus : uint
    {
        OFF = 0,
        ON = 1,
    }

    public enum Sign
    {
        Minus,
        Plus,
    }

    




    public class TypeDescriptorContext : ITypeDescriptorContext
    {
        public TypeDescriptorContext(PropertyDescriptor propertyDescriptor)
        {
            PropertyDescriptor = propertyDescriptor;
        }

        #region ITypeDescriptorContext メンバ

        public IContainer Container
        {
            get { throw new NotImplementedException(); }
        }

        public object Instance
        {
            get { return null; }
        }

        public void OnComponentChanged()
        {
        }

        public bool OnComponentChanging()
        {
            return false;
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get;
            private set;
        }

        #endregion

        #region IServiceProvider メンバ

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 表示するシンボル
    /// </summary>
    [Serializable]
    public class IOMonitorSymbolFormat
    {
        // BaseForm番号
        public int BaseFormNumber { get; set; }
        // シンボル名
        public string SymbolName { get; set; }
        // ログ出力を行うプロパティ
        public string[] LogOutProperties { get; set; }
    }

    [Serializable]
    public enum PlcType
    {
        C,
        CV,
        CS,
        CJ,
    }



    #region Calculator

    // 概要:
    //     デバイスの値に対する操作を表す
    public interface ICalculator
    {
        UInt16[] Calculate(UInt16[] calculateValue);
    }

    // 概要:
    //     全てのCalculatorのベースとなるクラス
    [Serializable]
    public abstract class CalculatorBase : ICalculator
    {
        // 動作の説明
        // (Debug時にこういうのが見れたらいいのでは？)
        public abstract string Direction
        {
            get;
            set;
        }

        public abstract UInt16[] Calculate(UInt16[] calculateValue);
    }

    // 概要:
    //     ユーザー定義の計算を行うCalculatorクラス
    [Serializable]
    public class Calculator : CalculatorBase
    {
        // 計算を行う関数
        private Func<UInt16[], UInt16[]> Calculation
        {
            get;
            set;
        }

        public Calculator(Func<UInt16[], UInt16[]> calculation)
        {
            Calculation = calculation;
        }

        string _direction = string.Empty;
        public override string Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        public override UInt16[] Calculate(UInt16[] calculateValue)
        {
            return Calculation(calculateValue);
        }
    }

    // 概要:
    //     加算を行うCalculatorクラス
    [Serializable]
    public class CalculatorAdd : CalculatorBase
    {
        // 加算する値
        private UInt16 AddValue
        {
            get;
            set;
        }

        public CalculatorAdd(UInt16 volume)
        {
            AddValue = volume;
        }

        public override string Direction
        {
            get
            {
                return "引き渡された値に、指定の値を加算して返します。";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override UInt16[] Calculate(UInt16[] calculateValue)
        {
            UInt16[] CopyValue = (UInt16[])calculateValue.Clone();
            int ArrayLength = CopyValue.Length;

            for (int i = 0; i < ArrayLength; i++)
            {
                CopyValue[i] += AddValue;
            }

            return CopyValue;
        }
    }

    // 概要:
    //     減算を行うCalculatorクラス
    [Serializable]
    public class CalculatorMinus : CalculatorBase
    {
        // 減算する値
        private UInt16 MinusValue
        {
            get;
            set;
        }

        public CalculatorMinus(UInt16 volume)
        {
            MinusValue = volume;
        }

        public override string Direction
        {
            get
            {
                return "引き渡された値から、指定の値を減算して返します。";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override UInt16[] Calculate(UInt16[] calculateValue)
        {
            UInt16[] CopyValue = (UInt16[])calculateValue.Clone();
            int ArrayLength = CopyValue.Length;

            for (int i = 0; i < ArrayLength; i++)
            {
                CopyValue[i] -= MinusValue;
            }

            return CopyValue;
        }
    }

    // 概要:
    //     代入を行うCalculatorクラス
    [Serializable]
    public class CalculatorSet : CalculatorBase
    {
        // 設定する値
        private UInt16[] SetValue
        {
            get;
            set;
        }

        public CalculatorSet(UInt16[] setValue)
        {
            SetValue = setValue;
        }

        public override string Direction
        {
            get
            {
                return "引き渡された値に、指定の値をセットして返します。";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override UInt16[] Calculate(UInt16[] calculateValue)
        {
            UInt16[] CopyValue = new UInt16[calculateValue.Length];

            Array.Copy(SetValue, CopyValue, calculateValue.Length);

            return CopyValue;
        }
    }

    #endregion

}
