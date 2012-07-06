using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Reflection;
using CommonClassLibrary;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace LocalSimulator.ProjectMaker
{
    public static class MainCtrl
    {

        public static void PropertyView_Set(object Obj)
        {
            //Debug.WriteLine(DateTime.Now.ToLongTimeString() + "PropertySet");

            if (Obj == null)
            { 
                MainForm.Instance.PropertyView.SelectedObject = null;
                return;
            }

            if (Obj.GetType().Name == "ViewObject")
            {
               ViewObject Vobj = (ViewObject)Obj;
               MainForm.Instance.PropertyView.SelectedObject = Vobj.ControlInstance;
                return;
            }

            MainForm.Instance.PropertyView.SelectedObject = Obj;

        }

        private static  List<ShapeObject> _ClipBoardObject = new List<ShapeObject>();

        public static List<ShapeObject> ClipBoardObject
        {
            get { return _ClipBoardObject;}
            set { _ClipBoardObject = value; }            
        }

        public static int ClipBoardObjectBaseX { get; set; }

        public static int ClipBoardObjectBaseY { get; set; }

        public static bool LineButtonSelect
        {
            get
            {
                return MainForm.Instance.LineToolStripButton.Checked;
            }
        }

	    public static bool LinesButtonSelect
        {
            get
            {
                return MainForm.Instance.LinesToolStripButton.Checked;
            }
        }

        public static bool CircleButtonSelect
        {
            get
            {
                return MainForm.Instance.CircleToolStripButton.Checked;
            }
        }

        public static bool SquareButtonSelect
        {
            get
            {
                return MainForm.Instance.SquareToolStripButton.Checked;
            }
        }

	    public static bool PolygonButtonSelect
        {
            get
            {
                return MainForm.Instance.PolygonToolStripButton.Checked;
            }
        }

        public static bool TextButtonSelect
        {
            get
            {
                return MainForm.Instance.TextToolStripButton.Checked;
            }
        }

        public static bool GridMode
        {
            get { return MainForm.Instance.GridToolStripButton.Checked; }
        }

        public static bool SnapMode
        {
            get { return MainForm.Instance.SnapToolStripButton.Checked; }
        }

        public static bool ToolTipNotSelect
        {
            get
            {
                if (LineButtonSelect == false && LinesButtonSelect == false && CircleButtonSelect == false &&
                   SquareButtonSelect == false && PolygonButtonSelect == false && TextButtonSelect == false)
                {
                    return true;
                }
                return false;
            }
        }

        public static bool SymbolViewListSelect
        {
            get
            {
                if (MainForm.Instance.SymbolListView.FocusedItem == null)
                {
                    return false;
                }

                ListViewItem SelectedItem = MainForm.Instance.SymbolListView.Items[MainForm.Instance.SymbolListView.FocusedItem.Index];

                return SelectedItem.Selected;
            }
        }

        public static bool ShapeContorolSelect { get; set; }

        private static Size _GridSize = new Size(5, 5);

        public static Size GridSize
        {
            get { return _GridSize; }
            set { _GridSize = value; }
        }

        public static bool RangeSelectMode { get; set; }

        public static bool DragMode
        {
            get
            {
                BaseForm frm = (BaseForm)MainForm.Instance.ActiveMdiChild;
                
                foreach (ShapeObject Shape in frm.ShapeCollection)
                {
                    if (Shape.ModeMove || Shape.ModeModify || Shape.ModeCreate) { return true; }
                }
                return false;
            }

        }

        public static void RefreshControlBmp(Symbol_Draw obj, ViewObject Vobj)
        {
            if (obj.Width == 0 || obj.Height == 0) { return; }

            //コントロールの外観を描画するBitmapの作成
            Bitmap bmp = new Bitmap(obj.Width, obj.Height);

            //キャプチャする
            //Panel nonPanel = new Panel();
            //MainForm.Instance.Controls.Add(nonPanel);
            MainForm.Instance.nonPanel.Controls.Add(obj);
            obj.DrawToBitmap(bmp, new Rectangle(0, 0, obj.Width, obj.Height));
            Vobj.ControlBmp = bmp; //obj.ControlBmp();

            MainForm.Instance.nonPanel.Controls.Remove(obj);
            //nonPanel.Dispose();

            //Console.WriteLine(DateTime.Now.Millisecond + "Bmp更新");

        }  

        public static void DrawSelectFalse()
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

        public static bool IsShapeMaking { get; set; }
        public static bool IsShapeMoving { get; set; }
        public static bool IsShapeChanging { get; set; }

    }    

    #region 多点用コレクションディタ

    public class MyListEditor : CollectionEditor
    {
        private MyList<Point> beforeData;
        private MyList<Point> nowData;
        private PropertyDescriptor pd;
        private object selectedObject;

        public MyListEditor(Type type) : base(type) { }

        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm cf = base.CreateCollectionForm();
            cf.FormClosed += new FormClosedEventHandler(cf_FormClosed);
            return cf;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Console.WriteLine("EditValue");
            //PropertyGridに表示されているオブジェクトのインスタンスを取得する。
            selectedObject = MainForm.Instance.PropertyForm.PropertyView.SelectedObject;
            beforeData = new MyList<Point>((MyList<Point>)value);

            pd = context.PropertyDescriptor;
            return base.EditValue(context, provider, value);
        }

        void cf_FormClosed(object sender, FormClosedEventArgs e)
        {
            nowData = pd.GetValue(selectedObject) as MyList<Point>;

            //比較処理
            bool IsSame = true;
            if (nowData.Count != beforeData.Count)
            {
                Change();
                return;
            }

            for (int i = 0; i < nowData.Count; i++)
            {
                if (nowData[i] != beforeData[i])
                {
                    IsSame = false;
                    break;
                }
            }
            if (IsSame == false) { Change(); }



        }
        /// <summary>
        /// データに変化があった場合の処理
        /// </summary>
        private void Change()
        {

            ShapeManager shapeManager = ShapeManager.CreateInstance();
            ShapeObjectChangeCommand shapeObjectChangeCommand =
                new ShapeObjectChangeCommand(shapeManager, (ShapeObject)selectedObject, beforeData.Items, nowData.Items);

            shapeObjectChangeCommand.Execute();

            CommandManager.UndoCommandStack.Push(shapeObjectChangeCommand);



        }

    }

    

    #endregion
}
