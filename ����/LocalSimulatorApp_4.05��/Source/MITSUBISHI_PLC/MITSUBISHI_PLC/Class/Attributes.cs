using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITSUBISHI_PLC
{
    //
    // DisplayNameAttributeはフィールドには付加することが出来ないので、
    // フィールドに付加出来る専用の名前属性を定義する。
    //

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class NameAttribute : Attribute
    {
        public NameAttribute(string name)
        {
            this.name = name;
        }

        string name;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public static string GetName(Enum value)
        {
            Type EnumType = value.GetType();
            string Name = Enum.GetName(EnumType, value);

            NameAttribute[] Attributes = (NameAttribute[])
                EnumType.GetField(Name).GetCustomAttributes(typeof(NameAttribute), false);

            return (Attributes.Length != 0) ? Attributes[0].Name : null;
        }
    }
}
