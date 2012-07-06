using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CommonClassLibrary
{
    /// <summary>
    /// Parameterクラス用コンバーター
    /// </summary>
    public static class ParameterConverter
    {
        public static object FromConnectionSetting(Type convertType, object value)
        {
            //try
            //{
                object paramInstance = Activator.CreateInstance(convertType);
                object[] sourceValue = (object[])value;
                PropertyDescriptorCollection paramClassProperties = TypeDescriptor.GetProperties(paramInstance.GetType());

                // PropertyDescriptor配列に変換
                PropertyDescriptor[] arrayParamProperties = paramClassProperties.OfType<PropertyDescriptor>().ToArray();

                // ParameterID順にソート
                SortPropertyWithParameterID(arrayParamProperties);

                // ID順に値を設定していく
                int loopCnt = (int)Math.Min(sourceValue.Length, arrayParamProperties.Length);
                for (int i = 0; i < loopCnt; i++)
                {
                    PropertyDescriptor property = arrayParamProperties[i];
                    object setValue = sourceValue.GetValue(i);

                    TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);

                    if(converter.CanConvertFrom(setValue.GetType()))
                    {
                        property.SetValue(paramInstance, converter.ConvertFrom(setValue));
                    }
                    else
                    {
                        property.SetValue(paramInstance, setValue);
                    }
                }

                return paramInstance;
            //}
            //catch
            //{
            //    Global.LogManager.Write("設定内容が一致しません");
            //    return null;
            //}
        }        

        public static object ToConnectionSetting(object value)
        {
            List<object> listInstance = new List<object>();
            
            PropertyDescriptorCollection paramClassProperties = TypeDescriptor.GetProperties(value.GetType());

            // PropertyDescripor配列に変換
            PropertyDescriptor[] arrayParamProperties = paramClassProperties.OfType<PropertyDescriptor>().ToArray();

            // ParameterID順にソート
            SortPropertyWithParameterID(arrayParamProperties);

            // ID順に配列化
            int loopCnt = paramClassProperties.Count;
            for (int i = 0; i < loopCnt; i++)
            {
                PropertyDescriptor property = arrayParamProperties[i];
                object setValue = property.GetValue(value);

                listInstance.Add(setValue);
            }

            return listInstance.ToArray();
        
            
        }

        private static void SortPropertyWithParameterID(PropertyDescriptor[] properties)
        {
            // パラメータIDを比較するデリゲート
            Comparison<PropertyDescriptor> comparison =
                (p1, p2) =>
                {
                    ParameterIDAttribute idAttr1 = p1.Attributes[typeof(ParameterIDAttribute)] as ParameterIDAttribute;
                    ParameterIDAttribute idAttr2 = p2.Attributes[typeof(ParameterIDAttribute)] as ParameterIDAttribute;

                    int id1 = (idAttr1 != null) ? idAttr1.ID : int.MaxValue;
                    int id2 = (idAttr2 != null) ? idAttr2.ID : int.MaxValue;

                    return Comparer<int>.Default.Compare(id1, id2);
                };

            // 配列をソート
            Array.Sort<PropertyDescriptor>(properties, comparison);
        }
    }

    /// <summary>
    /// パラメータID属性
    /// </summary>
    public class ParameterIDAttribute : Attribute
    {
        public ParameterIDAttribute(int id)
        {
            this.myID = id;
        }

        private int myID;

        public int ID
        {
            get
            {
                return this.myID;
            }
        }
    }
}
