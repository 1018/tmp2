using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace CommonClassLibrary
{
    
    public static class SerializeSupport
    {

        ///<summary>
        /// オブジェクトを指定の型に変換する。
        ///</summary>
        public static object ConvertTo(object Source, Type ConvType)
        {
            if (Source == null) { return null; }
            Type SourceType = Source.GetType();

            //一旦元をString型に変換する
            string strVal = TypeDescriptor.GetConverter(SourceType).ConvertToString(Source);

            TypeConverter Converter = TypeDescriptor.GetConverter(ConvType);

            bool CanConv = Converter.CanConvertFrom(typeof(string));

            if (CanConv)
            {
                return Converter.ConvertFromString(strVal);
            }


            //変換できない場合はnullを返す。
            return null;
        }

        /// <summary>
        /// オブジェクトをシリアル化クラスへ格納する
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static object Serialize(object target)
        {
            object ret = null;
            
            if(target is IBaseForm)
            {
                ret = SerializeBaseForm((IBaseForm)target);
            }
            else if (target is Symbol_Draw)
            {
                ret = SerializeSymbol((Symbol_Draw)target);
            }
            else if(target is IShapeObject)
            {
                ret = SerializeShape((IShapeObject)target);
            }

            return ret;
        }

        private static object SerializeBaseForm(IBaseForm target)
        {
            //保存するクラスのインスタンスを作成
            BaseFormDataSerializeFormat baseFormData = new BaseFormDataSerializeFormat();            

            FontConverter fc = new FontConverter();
            ColorConverter cc = new ColorConverter();
            SizeConverter sc = new SizeConverter();

            baseFormData.TitleName = target.TitleName;
            baseFormData.Font = fc.ConvertToString(target.Font);
            baseFormData.BackColor = cc.ConvertToString(target.BackColor);
            baseFormData.Number = target.Number.ToString();
            baseFormData.Text = target.Text;
            baseFormData.Size = sc.ConvertToString(target.BaseFormSize);

            return baseFormData;


        }

        private static object SerializeSymbol(Symbol_Draw target)
        {
            SymbolDataSerializeFormat symbolData = new SymbolDataSerializeFormat();
            symbolData.Type = Path.GetFileNameWithoutExtension(target.GetType().Assembly.ManifestModule.ToString());
            symbolData.SymbolName = target.SymbolName;


            #region PropertyData取得

            List<PropertyDataSerializeFormat> PropertyDatas = new List<PropertyDataSerializeFormat>();

            PropertyDescriptorCollection Properties = TypeDescriptor.GetProperties(target, null, true);

            foreach (PropertyDescriptor Property in Properties)
            {

                if (Global.GetVisibleAttribute(target, Property))
                {
                    PropertyDataSerializeFormat PropertyData = new PropertyDataSerializeFormat();

                    //プロパティ名
                    PropertyData.Name = Property.Name;

                    //プロパティ値 
                    object PropertyValue = Property.GetValue(target);
                   
                    TypeConverter tc = Property.Converter;
                    if (tc.GetType() == typeof(TypeConverter) || tc.GetType() == typeof(ArrayConverter)) { tc = new NestConverter(); }
                    PropertyData.Value = tc.ConvertTo(PropertyValue, typeof(string));                    

                    PropertyDatas.Add(PropertyData);
                }
            }

            #endregion

            symbolData.PropertyData = PropertyDatas;


            return symbolData;            


        }

        private static object SerializeShape(IShapeObject target)
        {
            ShapeDataSerializeFormat shapeData = new ShapeDataSerializeFormat();

            shapeData.Type = target.ShapeType;

            List<PropertyDataSerializeFormat> PropertyDatas = new List<PropertyDataSerializeFormat>();

            PropertyDescriptorCollection Properties = TypeDescriptor.GetProperties(target, null, true);

            foreach (PropertyDescriptor Property in Properties)
            {
                if (Property.IsBrowsable && Global.GetVisibleAttribute(target, Property))
                {
                    PropertyDataSerializeFormat PropertyData = new PropertyDataSerializeFormat();

                    //プロパティ名
                    PropertyData.Name = Property.Name;

                    //プロパティ値
                    PropertyData.Value = Property.Converter.ConvertTo(Property.GetValue(target), typeof(string));

                    PropertyDatas.Add(PropertyData);
                }
            }

            shapeData.PropertyData = PropertyDatas;

            return shapeData; 

        }
        
        /// <summary>
        /// シリアル化クラスから指定した型のオブジェクトを生成する。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object Deserialize(object target,Type targetType)
        {
            object ret = null;            

            if(target.GetType() == typeof(BaseFormDataSerializeFormat))
            {
                ret = DeserializeBaseForm((BaseFormDataSerializeFormat)target, targetType);
            }
            else if (target.GetType() == typeof(SymbolDataSerializeFormat))
            {
                ret = DeserializeSymbol((SymbolDataSerializeFormat)target);
            }
            else if (target.GetType() == typeof(ShapeDataSerializeFormat))
            {
                ret = DeserializeShape((ShapeDataSerializeFormat)target, targetType);
            }

            return ret;
        }

        /// <summary>
        /// シリアル化クラスからシンボル型のオブジェクトを生成する。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static object Deserialize(object target)
        {
            object ret = null;

            if (target.GetType() == typeof(SymbolDataSerializeFormat))
            {
                ret = DeserializeSymbol((SymbolDataSerializeFormat)target);
            }
            return ret;
        }

        private static object DeserializeBaseForm(BaseFormDataSerializeFormat target, Type targetType)
        {            
            IBaseForm instance = (IBaseForm)Activator.CreateInstance(targetType);
            instance.SendToBack();
            FontConverter fc = new FontConverter();
            ColorConverter cc = new ColorConverter();
            SizeConverter sc = new SizeConverter();

            instance.TitleName = (string)target.TitleName;
            instance.Font = (Font)fc.ConvertFromString((string)target.Font);
            instance.BackColor = (Color)cc.ConvertFromString((string)target.BackColor);
            instance.Number = (int)Convert.ToInt32(target.Number);
            instance.Text = (string)target.Text;
            instance.BaseFormSize = (Size)sc.ConvertFrom(target.Size);

            return instance;

        }

        private static object DeserializeSymbol(SymbolDataSerializeFormat target)
        {
            #region シンボルのインスタンスを生成する。

            Symbol_Draw symbol = null;            

            try
            {
                Assembly Asm = Assembly.LoadFrom(AppSetting.SymbolPath + "\\" + target.Type + ".dll");

                Type SymbolType = Asm.GetType(AppSetting.SymbolNameSpace + "." + target.Type + "_Draw");

                symbol = (Symbol_Draw)Activator.CreateInstance(SymbolType);
            }
            catch
            {
                // dllが存在しない、等の例外処理
                //MessageBox.Show("DLLが存在しません。" + target.Type + ".dll",
                //                "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Global.LogManager.Write("Dll Not Found " + target.Type + ".dll");

                return null;
            }
            #endregion

            Symbol_Draw deviceFormatSaveSymbol = SaveDeviceFormatInfo(symbol);

            #region シンボルのプロパティを生成する。

            symbol.IsDeserializing = true;

            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(symbol, null, true);

            foreach (PropertyDataSerializeFormat pdsf in target.PropertyData)
            {
                PropertyDataSerializeFormat propertyData = pdsf;

                //旧データコンバート
                OldSymbolPropertyConvert(symbol, ref propertyData);

                PropertyDescriptor pd = pdc[propertyData.Name];

                //シリアライズされたプロパティが型に存在しない場合
                if (pd == null)
                {
                    Global.LogManager.Write("Property Not Found " + symbol.Name + "." + propertyData.Name);
                    continue;
                }

                //Visible属性がない場合
                if (!Global.GetVisibleAttribute(symbol, pd)) { continue; }

                //try
                //{
                    TypeConverter tc = pd.Converter;

                    if (tc.GetType() == typeof(TypeConverter) || tc.GetType() == typeof(ArrayConverter)) { tc = new NestConverter(); }
                                        
                    TypeDescriptorContext context = new TypeDescriptorContext(pd);
                    object convValue = tc.ConvertFrom(context, null, propertyData.Value);
                    pd.SetValue(symbol, convValue);
                    
                //}
                //catch
                //{
                //    Global.LogManager.Write("プロパティの型変換ができませんでした。 " + Symbol.Name + "." + pd.Name);
                //}
            }

            symbol.IsDeserializing = false;

            #endregion

            LoadDeviceFormatInfo(deviceFormatSaveSymbol, symbol);

            symbol.OnRedrawOrder(EventArgs.Empty);

            return symbol;
        }

        private static object DeserializeShape(ShapeDataSerializeFormat target, Type targetType)
        {
            IShapeObject instance = (IShapeObject)Activator.CreateInstance(targetType);
            
            //プロパティコピー
            PropertyDescriptorCollection TypeProperties = TypeDescriptor.GetProperties(instance, null, true);

            foreach (PropertyDataSerializeFormat PropertyData in target.PropertyData)
            {
                PropertyDescriptor TypeProperty = null;
                
                TypeProperty = TypeProperties[PropertyData.Name];

                object ConvValue = TypeProperty.Converter.ConvertFrom(PropertyData.Value);

                TypeProperty.SetValue(instance, ConvValue);
            }

            return instance;

        }

        private static Symbol_Draw SaveDeviceFormatInfo(Symbol_Draw target)
        {
            return (Symbol_Draw)Activator.CreateInstance(target.GetType());
        }

        private static void LoadDeviceFormatInfo(object infoSymbol, object target)
        {
            PropertyDescriptorCollection infoPdc = TypeDescriptor.GetProperties(infoSymbol);
            PropertyDescriptorCollection targetPdc = TypeDescriptor.GetProperties(target);

            for(int i =0; i< infoPdc.Count; i++)
            {
                PropertyDescriptor infoPd = infoPdc[i];
                PropertyDescriptor targetPd = targetPdc[i];

                if (infoPd.Converter.GetType() == typeof(DeviceFormatConverter))
                {
                    DeviceFormat targetsource = (DeviceFormat)targetPd.GetValue(target);
                    DeviceFormat infoSource = (DeviceFormat)infoPd.GetValue(infoSymbol);

                    targetsource.DataCount = infoSource.DataCount;
                    targetsource.InputType = infoSource.InputType;
                    targetsource.Io = infoSource.Io;
                    targetsource.Value = infoSource.Value;
                }
                else if (infoPd.PropertyType == typeof(DeviceFormat[]))
                {
                    DeviceFormat[] targetsources = (DeviceFormat[])targetPd.GetValue(target);
                    DeviceFormat[] infoSources = (DeviceFormat[])infoPd.GetValue(infoSymbol);

                    for (int j = 0; j < targetsources.Count(); j++)
                    {
                        targetsources[j].DataCount = infoSources[j].DataCount;
                        targetsources[j].InputType = infoSources[j].InputType;
                        targetsources[j].Io = infoSources[j].Io;
                        targetsources[j].Value = infoSources[j].Value;
                    }
                }

                else if (infoPd.Converter.GetType() == typeof(NestConverter))
                {
                    object targetsource = targetPd.GetValue(target);
                    object infoSource = infoPd.GetValue(infoSymbol);

                    LoadDeviceFormatInfo(infoSource, targetsource);

                }

            }

        }

        private static void OldSymbolPropertyConvert(Symbol_Draw symbol, ref PropertyDataSerializeFormat PropertyData)
        {
            if (symbol.SymbolType == "水槽")
            {
                if (PropertyData.Name.Contains("ChargePomp"))
                    PropertyData.Name = PropertyData.Name.Replace("ChargePomp", "ChargePump");

                if (PropertyData.Name.Contains("DischargePomp"))
                    PropertyData.Name = PropertyData.Name.Replace("DischargePomp", "DischargePump");
            }
        }
    }

    //public static class SerializeInterlock
    //{
    //    public static void ToFile(string FilePath, Interlock SerializeObject)
    //    {
    //        List<Type> typ = new List<Type>();
    //        typ.Add(typeof(DeviceFormat));
    //        typ.AddRange(InterlockInfo.ConditionTypes);
    //        typ.AddRange(InterlockInfo.ExpressionTypes);

    //        XmlSerializer serializer = new XmlSerializer(typeof(Interlock), typ.ToArray());

    //        using (FileStream fs = new FileStream(FilePath, FileMode.Create))
    //        {
    //            serializer.Serialize(fs, SerializeObject);
    //        }
    //    }

    //    public static Interlock FromFile(string FilePath)
    //    {
    //        List<Type> typ = new List<Type>();
    //        typ.Add(typeof(DeviceFormat));
    //        typ.AddRange(InterlockInfo.ConditionTypes);
    //        typ.AddRange(InterlockInfo.ExpressionTypes);

    //        XmlSerializer serializer = new XmlSerializer(typeof(Interlock), typ.ToArray());

    //        using (FileStream fs = new FileStream(FilePath, FileMode.Open))
    //        {
    //            return (Interlock)serializer.Deserialize(fs);
    //        }
    //    }

    //}

    public class ProjectDataSerializeFormat
    {
        public string ProjectName { get; set; }

        public List<BaseFormDataSerializeFormat> BaseFormData { get; set; }

        //旧バージョン互換用　リリース時削除要        
        public SystemSettingFormat SystemSetting
        {
            get
            {
                return null;
            }
            set
            {               
            }
        }

        public string MakerName { get; set; }
        public List<object> ConnectSetting { get; set; }

        public List<IOMonitorSymbolFormat> IOMonitorData { get; set; }

        public static ProjectDataSerializeFormat Load(string filePath)
        {
            Type[] et = new Type[] { typeof(string[]) };
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectDataSerializeFormat), et);

            ProjectDataSerializeFormat projectData = new ProjectDataSerializeFormat();

            if (!File.Exists(filePath)) { return projectData; }
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                try
                {
                    projectData = (ProjectDataSerializeFormat)serializer.Deserialize(fs);

                    if (projectData.ConnectSetting == null) { projectData.ConnectSetting = projectData.SystemSetting.ConnectSetting; }
                    if (projectData.MakerName == null) { projectData.MakerName = projectData.SystemSetting.MakerName; }


                }
                catch
                {                    
                    return null;
                }
            }

            return projectData;
        }

        public static bool SaveMainProgram(string filePath, ProjectDataSerializeFormat saveData)
        {
            //一旦現在のXMLデータを読み込む
            ProjectDataSerializeFormat serializeData = Load(filePath);
            if (serializeData == null) { return false; }

            serializeData.IOMonitorData = saveData.IOMonitorData;
            serializeData.ConnectSetting = saveData.ConnectSetting;

            Type[] et = new Type[] { typeof(string[]) };

            //XmlSerializerオブジェクトを作成。書き込むオブジェクトの型を指定する            
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectDataSerializeFormat), et);
                        
            if (File.Exists(filePath) == false) { return false; }

            //ファイルを開く
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                //シリアル化し、XMLファイルに保存する
                serializer.Serialize(fs, serializeData);
            }

            return true;


        }

        public static bool SaveProjectMaker(string filePath, ProjectDataSerializeFormat saveData, bool addFlag)
        {
            ProjectDataSerializeFormat serializeData = new ProjectDataSerializeFormat();
            if (addFlag)
            {
                //一旦現在のXMLデータを読み込む
                serializeData = Load(filePath);
                if (serializeData == null) { return false; }
            }
                serializeData.ProjectName = saveData.ProjectName;
                serializeData.MakerName = saveData.MakerName;
                serializeData.BaseFormData = saveData.BaseFormData;
            

            Type[] et = new Type[] { typeof(string[]) };

            //XmlSerializerオブジェクトを作成。書き込むオブジェクトの型を指定する            
            XmlSerializer serializer = new XmlSerializer(typeof(ProjectDataSerializeFormat), et);

            //ファイルを開く
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                //シリアル化し、XMLファイルに保存する
                serializer.Serialize(fs, serializeData);
            }

            return true;

        }



    }

    [Serializable]
    public class BaseFormDataSerializeFormat
    {
        public object TitleName { get; set; }
        public object Number { get; set; }
        public object Size { get; set; }
        public object Text { get; set; }
        public object BackColor { get; set; }
        public object Font { get; set; }

        public List<SymbolDataSerializeFormat> SymbolData { get; set; }

        public List<ShapeDataSerializeFormat> ShapeData { get; set; }

    }

    [Serializable]
    public class SymbolDataSerializeFormat
    {
        public string Type { get; set; }
        public string SymbolName { get; set; }
        public List<PropertyDataSerializeFormat> PropertyData { get; set; }
    }

    [Serializable]
    public class PropertyDataSerializeFormat
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    [Serializable]
    public class ShapeDataSerializeFormat
    {
        public string Type { get; set; }
        public List<PropertyDataSerializeFormat> PropertyData { get; set; }

    }

    [Serializable]
    public class SystemSettingFormat
    {
        public string MakerName { get; set; }
        public List<object> ConnectSetting { get; set; }
    }
   
}
