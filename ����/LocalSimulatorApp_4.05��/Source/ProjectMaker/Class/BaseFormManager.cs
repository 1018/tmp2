using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using CommonClassLibrary;
using System.Drawing;
using System.Collections;

namespace LocalSimulator.ProjectMaker
{
    public class BaseFormManager
    {
        private BaseFormManager() { }

        static BaseFormManager Instance { get; set; }
        public static BaseFormManager CreateInstance()
        {
            if (Instance == null)
            {
                Instance = new BaseFormManager();
            }
            return Instance;
        }

        public void SetProperty(IBaseForm baseFrm, string propertyRoot, object setValue)
        {
            //設定するプロパティまでのルート
            string[] nameRoot = propertyRoot.Split(',');
            for (int i = 0; i < nameRoot.Length; i++)
            {
                nameRoot[i] = nameRoot[i].Trim();
            }

            //値を設定するオブジェクトを取得
            object[] instanceRoot = new object[nameRoot.Length];
            object component = baseFrm;

            for (int i = 0; i < nameRoot.Length; i++)
            {
                Type componentType = component.GetType();

                // 配列アクセス
                if (nameRoot[i].IndexOf('[') == 0)
                {
                    string arrayIndexStr = nameRoot[i].Trim('[', ']');
                    int arrayIndex = int.Parse(arrayIndexStr);

                    instanceRoot[i] = ((IList)component)[arrayIndex];
                }
                else
                {
                    PropertyInfo pi = componentType.GetProperty(nameRoot[i]);
                    instanceRoot[i] = pi.GetValue(component, null);
                }

                component = instanceRoot[i];
            }

            //新しい値を設定
            instanceRoot[instanceRoot.Length - 1] = setValue;

            //反対向きにセットしていく(ルートに値型が含まれる可能性があるため)
            for (int i = nameRoot.Length - 1; i >= 0; i--)
            {
                if (i == 0)
                    component = baseFrm;
                else
                    component = instanceRoot[i - 1];


                Type componentType = component.GetType();

                // 配列アクセス
                if (nameRoot[i].IndexOf('[') == 0)
                {
                    string arrayIndexStr = nameRoot[i].Trim('[', ']');
                    int arrayIndex = int.Parse(arrayIndexStr);

                    ((IList)component)[arrayIndex] = instanceRoot[i];
                }
                else
                {
                    PropertyInfo pi = componentType.GetProperty(nameRoot[i]);
                    pi.SetValue(component, instanceRoot[i], null);
                }
            }
        }
    }

    //class PropertyChangedEventArgs : EventArgs
    //{
    //    public PropertyDescriptor ChangeProperty { get; set; }
    //    public object NewValue { get; set; }
    //    public object OldValue { get; set; }

    //    public string PropertyName
    //    {
    //        get
    //        {
    //            if (this.ChangeProperty != null)
    //            {
    //                return this.ChangeProperty.Name;
    //            }
    //            return string.Empty;
    //        }
    //    }
    //    public Type PropertyType
    //    {
    //        get
    //        {
    //            if (ChangeProperty != null)
    //            {
    //                return this.ChangeProperty.PropertyType;
    //            }
    //            return null;
    //        }
    //    }

    //    public PropertyChangedEventArgs(PropertyDescriptor changeProperty, object newValue, object oldValue)
    //    {
    //        this.ChangeProperty = changeProperty;
    //        this.NewValue = this.NewValue;
    //        this.OldValue = this.OldValue;
    //    }
    //}
}
