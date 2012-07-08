using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using CommonClassLibrary;
using System.Collections;

namespace LocalSimulator.ProjectMaker
{
    public partial class PropertyForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public PropertyForm()
        {
            InitializeComponent();
        }
        
        private void SetPropertyChangeCommand(GridItem gridItem, object oldValue)
        {
            GridItem item = gridItem;
            object parentObject;
            object setValue = oldValue;
            PropertyDescriptor setProperty = item.PropertyDescriptor;
            List<string> rootPropertyNameList = new List<string>();

            while (item.Parent != null)
            {
                // カテゴリは飛ばす
                if (item.Parent.GridItemType != GridItemType.Category)
                {
                    // setValue の親オブジェクト
                    parentObject = item.Parent.Value;

                    // (parentObject.○○○ == setValue) な関係

                    if (parentObject is IList)  // List<T>もint[]もIListを実装している
                    {
                        // setValue が配列の要素だった場合、
                        // PropertyDescriptorからインデックスを取得する

                        ElementPropertyDescriptor epd = setProperty as ElementPropertyDescriptor;

                        if (epd != null)
                        {
                            rootPropertyNameList.Insert(0, string.Format("[{0}]", epd.Index));
                        }
                        else
                        {
                            rootPropertyNameList.Insert(0, item.Label);
                        }
                    }
                    else
                    {
                        rootPropertyNameList.Insert(0, setProperty.Name);
                    }

                    // 上に遡る
                    setValue = parentObject;
                    setProperty = item.Parent.PropertyDescriptor;
                }

                item = item.Parent;
            }

            string root = string.Join(",", rootPropertyNameList.ToArray());

            if (item.Value is IBaseForm)
            {
                BaseFormPropertyChangeCommand command = new BaseFormPropertyChangeCommand(
                    BaseFormManager.CreateInstance(), (BaseForm)item.Value, root, gridItem.Value, oldValue);

                CommandManager.UndoCommandStack.Push(command);
            }
            else if (item.Value is ShapeObject)
            {
                ShapeObjectPropertyChangeCommand command = new ShapeObjectPropertyChangeCommand(
                    ShapeManager.CreateInstance(), (ShapeObject)item.Value, root, gridItem.Value, oldValue);

                CommandManager.UndoCommandStack.Push(command);
            }
            else if(item.Value is Symbol_Draw)
            {
                SymbolPropertyChangeCommand command = new SymbolPropertyChangeCommand(
                    ShapeManager.CreateInstance(), (Symbol_Draw)item.Value, root, gridItem.Value, oldValue);

                CommandManager.UndoCommandStack.Push(command);
            }

        }

        private void PropertyView_Validating(object sender, PropertyValueValidatingEventArgs e)
        {
            object SourceObject = PropertyView.SelectedObject;
            ((CustomPropertyGrid)sender).Refresh();
            BaseForm frm = (BaseForm)MainForm.Instance.ActiveMdiChild;
            frm.PropertyChanged(SourceObject, e);
            PropertyView.Refresh();
        }

        private void PropertyView_Validated(object sender, PropertyValueChangedEventArgs e)
        {
            // コマンドスタックへの登録
            SetPropertyChangeCommand(e.ChangedItem, e.OldValue);


            // IOリストへの変更の反映
            IoListForm.Instance.Refresh();

        }        

        private void PropertyView_Enter(object sender, EventArgs e)
        {
            //ShapeToolStrip選択を解除
            MainForm.Instance.LineToolStripButton.Checked = false;
            MainForm.Instance.LinesToolStripButton.Checked = false;
            MainForm.Instance.CircleToolStripButton.Checked = false;
            MainForm.Instance.SquareToolStripButton.Checked = false;
            MainForm.Instance.PolygonToolStripButton.Checked = false;
            MainForm.Instance.TextToolStripButton.Checked = false;

            //ListViewの選択を解除
            if (MainForm.Instance.SymbolListView.FocusedItem != null)
            {
                ListViewItem SelectedItem = MainForm.Instance.SymbolListView.Items[MainForm.Instance.SymbolListView.FocusedItem.Index];
                SelectedItem.Selected = false;

            }
           
        }
        
    }
}
