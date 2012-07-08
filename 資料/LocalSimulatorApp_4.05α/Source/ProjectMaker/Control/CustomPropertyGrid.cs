using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using CommonClassLibrary;
using System.Reflection;
using System.Collections;
using System.Drawing.Design;

namespace LocalSimulator.ProjectMaker
{
    public class CustomPropertyGrid : PropertyGrid
    {
        public delegate void PropertyValueValidatingEventHandler(object sender, PropertyValueValidatingEventArgs e);
        public delegate void PropertyValueValidatedEventHandler(object sender, PropertyValueChangedEventArgs e);

        /// <summary>
        /// 概要：
        ///     変更された値を検証しているときに発生します。
        /// </summary>
        public event PropertyValueValidatingEventHandler PropertyValueValidating;
        /// <summary>
        /// 概要：
        ///     変更された値が検証された後に発生します。
        /// </summary>
        public event PropertyValueValidatedEventHandler PropertyValueValidated;

        protected override PropertyTab CreatePropertyTab(Type tabType)
        {
            return new MyPropertyTab();     // MyPropertyTab型のタブを返す
        }

        protected override void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
        {
            #region
            /*
             * PropertyValueValidatingイベントは入力された値の検証イベント。
             * 入力をキャンセルすることが可能。
             * 
             * PropertyValueValidatedイベントはPropertyValueValidatingイベントにて
             * 検証が終了したことを通知するイベント。
             * 
             * 
             * イベントの発生の順序は以下のようになる。
             * 
             * PropertyValueChanged
             *        ↓
             * PropertyValueValidating
             *        ↓
             * PropertyValueValidated (入力がキャンセルされなければ)
             */
            #endregion

            // PropertyValueChangedイベント
            base.OnPropertyValueChanged(e);

            // 検証中イベントを発生させる
            PropertyValueValidatingEventArgs args =
                new PropertyValueValidatingEventArgs(e.ChangedItem, e.OldValue);

            this.OnPropertyValueValidating(args);


            // 変更をキャンセルしない
            if (!args.Cancel)
            {
                // 検証完了イベントを発生させる
                this.OnPropertyValueValidated(e);
            }
            else
            {
                // 値を元に戻す

                RedoValue(e.ChangedItem, e.OldValue);

                this.Refresh();
            }
        }

        /// <summary>
        /// 検証中イベント発生
        /// </summary>
        /// <param name="e"></param>
        protected void OnPropertyValueValidating(PropertyValueValidatingEventArgs e)
        {
            if (this.PropertyValueValidating != null)
            {
                this.PropertyValueValidating(this, e);
            }
        }

        /// <summary>
        /// 検証完了イベント発生
        /// </summary>
        /// <param name="e"></param>
        protected void OnPropertyValueValidated(PropertyValueChangedEventArgs e)
        {
            if (this.PropertyValueValidated != null)
            {
                this.PropertyValueValidated(this, e);
            }
        }


        /// <summary>
        /// 指定の値を変更前の値に戻す
        /// </summary>
        /// <remarks>
        /// OnChangedValueValidatingイベントにて入力をキャンセルされた時、
        /// 入力前の値に戻すために呼び出す。
        /// </remarks>
        /// <param name="gridItem">変更されたGridItem</param>
        /// <param name="oldValue">変更前の値</param>
        private void RedoValue(GridItem gridItem, object oldValue)
        {
            GridItem item = gridItem;
            object parentObject;
            object setValue = oldValue;
            PropertyDescriptor setProperty = item.PropertyDescriptor;

            while (item.Parent != null)
            {
                // カテゴリは飛ばす
                if (item.Parent.GridItemType != GridItemType.Category)
                {
                    // setValue の親オブジェクト
                    parentObject = item.Parent.Value;

                    // (parentObject.○○○ == setValue) な関係

                    // 設定前値をセットする
                    setProperty.SetValue(parentObject, setValue);

                    // 上に遡る
                    setValue = parentObject;
                    setProperty = item.Parent.PropertyDescriptor;
                }

                item = item.Parent;
            }
        }


        public class MyPropertyTab : PropertyTab
        {
            public MyPropertyTab()
            {
            }

            public override string TabName
            {
                get { return "Array Tab"; }
            }

            public override Bitmap Bitmap
            {
                get
                {
                    return new Bitmap(16, 16);
                }
            }


            public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
            {
                return GetProperties(null, component, attributes);
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
            {
                //フォント特殊処理
                if (context.PropertyDescriptor!= null && context.PropertyDescriptor.Converter.GetType() == typeof(FontConverter)) { return null; }


                bool isHead = (component is BaseForm ||
                               component is Symbol_Draw ||
                               component is ShapeObject) ? true : false;

                List<PropertyDescriptor> newProperties = new List<PropertyDescriptor>();

                #region 親が配列でない場合
                if (!component.GetType().IsArray && !component.GetType().IsGenericType)
                {

                    PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(component);

                    foreach (PropertyDescriptor pd in pdc)
                    {
                        if (!pd.IsBrowsable) { continue; }
                        if (isHead && !Global.GetVisibleAttribute(context.Instance, pd)) { continue; }

                        //メンバが配列でない場合
                        if (!pd.PropertyType.IsArray && !pd.PropertyType.IsGenericType)
                        {
                            TypeConverter tc = pd.Converter;

                            PropertyInfo pi = component.GetType().GetProperty(pd.Name);

                            if (tc.GetType() != typeof(NestConverter) && pi.CanWrite == true)
                            {
                                List<Attribute> AttributeCollection = new List<Attribute>(pd.Attributes.OfType<Attribute>());
                                PropertyDescriptor newPd = TypeDescriptor.CreateProperty(component.GetType(), pd.Name, pd.PropertyType, AttributeCollection.ToArray());
                                newProperties.Add(newPd);
                            }
                            else
                            {
                                List<Attribute> AttributeCollection = new List<Attribute>(pd.Attributes.OfType<Attribute>());
                                AttributeCollection.Add(new ReadOnlyAttribute(true));
                                PropertyDescriptor newPd = TypeDescriptor.CreateProperty(component.GetType(), pd.Name, pd.PropertyType, AttributeCollection.ToArray());
                                                                      
                                
                                newProperties.Add(newPd);
                            }
                        }
                        //メンバが配列の場合
                        else
                        {

                            List<Attribute> memberAttributes = new List<Attribute>(pd.Attributes.OfType<Attribute>());

                            memberAttributes = DeleteAttribute(memberAttributes, typeof(ReadOnlyAttribute));
                            memberAttributes = DeleteAttribute(memberAttributes, typeof(EditorAttribute));
                            memberAttributes = DeleteAttribute(memberAttributes, typeof(TypeConverterAttribute));

                            memberAttributes.Add(new EditorAttribute(typeof(UITypeEditor), typeof(UITypeEditor)));
                            memberAttributes.Add(new TypeConverterAttribute(typeof(NestConverter)));
                            memberAttributes.Add(new ReadOnlyAttribute(true));
                            PropertyDescriptor newPd = TypeDescriptor.CreateProperty(component.GetType(), pd.Name, pd.PropertyType, memberAttributes.ToArray());
                            newProperties.Add(newPd);
                        }

                    }
                }
                #endregion

                #region 親が配列の場合
                else if (component.GetType().IsArray)
                {
                    Array ar = (Array)component;                    
                    string propertyName = context.PropertyDescriptor.DisplayName;

                    for (int i = 0; i < ar.Length; i++)
                    {
                        ElementPropertyDescriptor pd = new ElementPropertyDescriptor(component, propertyName, i);
      
                        //親の属性から要素の属性を設定する
                        SetElementAttribute(context, ref pd);                        
                        
                        newProperties.Add(pd);
                    }

                }
                #endregion

                #region 親がGenericListの場合
                else
                {
                    IList ar = (IList)component;
                    
                    string propertyName = context.PropertyDescriptor.DisplayName;

                    for (int i = 0; i < ar.Count; i++)
                    {
                        ElementPropertyDescriptor pd = new ElementPropertyDescriptor(component, propertyName, i);

                        //親の属性から要素の属性を設定する
                        SetElementAttribute(context, ref pd);

                        newProperties.Add(pd);
                    }
                }
                #endregion

                return new PropertyDescriptorCollection(newProperties.ToArray());
            }

            private void SetElementAttribute(ITypeDescriptorContext context, ref ElementPropertyDescriptor pd)
            {

                //親の属性を取得
                AttributeCollection parentAttributes = context.PropertyDescriptor.Attributes;

                List<Attribute> attributes = new List<Attribute>();

                //タイプコンバータ
                TypeConverter tc = TypeDescriptor.GetConverter(pd.PropertyType);
                if (tc.GetType() == typeof(TypeConverter)) { tc = new NestConverter(); }

                //エディタ
                object editor = pd.GetEditor(typeof(UITypeEditor));

                foreach (Attribute attr in parentAttributes)
                {
                    if (attr is ReadOnlyAttribute)
                    {
                        
                    }
                    else if (attr is TypeConverterAttribute)
                    {
                    }
                    else if (attr is DisplayNameAttribute)
                    {
                    }
                    else
                    {
                        //親にエディタ属性有り且つ、子にエディタ属性有
                        if (attr is EditorAttribute && editor != null)
                        {
                        }
                        else
                        {
                            attributes.Add(attr);
                        }
                    }                    
                }

                attributes.Add(new TypeConverterAttribute(tc.GetType()));

                if (editor != null)
                {
                    attributes.Add(new EditorAttribute(editor.GetType(), typeof(UITypeEditor)));
                }

                PropertyInfo pi = context.Instance.GetType().GetProperty(context.PropertyDescriptor.Name);

                if (tc.GetType() == typeof(NestConverter) || pi.CanWrite == false) 
                {
                    attributes.Add(new ReadOnlyAttribute(true)); 
                }

                pd.AttributeArray2 = attributes.ToArray();
                
            }

            private List<Attribute> DeleteAttribute(List<Attribute> targetAttributes, Type deleteAttribute)
            {
                List<Attribute> result = new List<Attribute>();

                foreach (Attribute targetAttribute in targetAttributes)
                {
                    if (targetAttribute.GetType() != deleteAttribute)
                    {
                        result.Add(targetAttribute);
                    }
                }
                return result;
            }

        }
    }

    

    public class PropertyValueValidatingEventArgs : CancelEventArgs
    {
        public PropertyValueValidatingEventArgs(GridItem changedItem, object oldValue)
            : base()
        {
            this.ChangedItem = changedItem;
            this.OldValue = oldValue;
        }
        public PropertyValueValidatingEventArgs(GridItem changedItem, object oldValue, bool cancel)
            : base(cancel)
        {
            this.ChangedItem = changedItem;
            this.OldValue = oldValue;
        }

        public GridItem ChangedItem { get; protected set; }

        public object OldValue { get; protected set; }
    }

}
