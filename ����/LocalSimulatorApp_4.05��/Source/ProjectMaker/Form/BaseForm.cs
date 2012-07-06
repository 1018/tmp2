using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CommonClassLibrary;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace LocalSimulator.ProjectMaker
{
    [TypeConverter(typeof(PropertyConverter))]
    public partial class BaseForm : Form, IBaseForm
    {
        #region API宣言

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool MoveWindow(
            IntPtr hWnd,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            bool bRepaint);

        #endregion

        ShapeManager shapeManager;


        //コンストラクタ
        public BaseForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            //最大化不可
            this.MaximizeBox = false;

            this.shapeManager = ShapeManager.CreateInstance();

            this.ShapeCollection.ObjectAdded += new EventHandler(ShapeCollection_ObjectAdded);
            this.ShapeCollection.ObjectRemoved += new EventHandler(ShapeCollection_ObjectRemoved);
            this.ShapeCollection.ObjectRenew += new EventHandler(ShapeCollection_ObjectRenew);
        }

        void ShapeCollection_ObjectRenew(object sender, EventArgs e)
        {
            //IoListForm.Instance.ClearSymbol();

            foreach (ShapeObject obj in ShapeCollection)
            {
                ViewObject Vobj = obj as ViewObject;

                if (Vobj != null)
                {
                    IoListForm.Instance.AddSymbol(this, Vobj.ControlInstance);
                }
            }
        }

        void ShapeCollection_ObjectRemoved(object sender, EventArgs e)
        {
            ViewObject Vobj = sender as ViewObject;

            if (Vobj != null)
            {
                IoListForm.Instance.RemoveSymbol(this, Vobj.ControlInstance);
            }
        }

        private void ShapeCollection_ObjectAdded(object sender, EventArgs e)
        {
            ViewObject Vobj = sender as ViewObject;

            if (Vobj != null)
            {
                IoListForm.Instance.AddSymbol(this, Vobj.ControlInstance);
            }
        }

        private int _Number = 0;

        private string _TitleName = "BaseForm";
        public const int BaseFormNameMaxLength = 30;

        private double _Zoom = 1;
        public double Zoom
        {
            get
            {
                return _Zoom;
            }
            set
            {
                _Zoom = value;

                //クライアント外領域算出
                int MargeWidth = this.Size.Width - this.ClientSize.Width;
                int MargeHeight = this.Size.Height - this.ClientSize.Height;

                //クライアント領域算出
                int ClientSizeWidth = BaseFormSize.Width - MargeWidth;
                int ClientSizeHeight = BaseFormSize.Height - MargeHeight;

                //クライアント領域にZoom補正をかけ、クライアント外サイズを加算する
                Size NewSize = new Size((int)(ClientSizeWidth * _Zoom) + MargeWidth, (int)(ClientSizeHeight * _Zoom) + MargeHeight);

                //サイズ変更
                SetWindowBounds(NewSize);

                //再作画
                foreach (ShapeObject shapeObject in ShapeCollection)
                {
                    shapeObject.ReMake();
                }

                this.Refresh();

            }
        }

        #region 表示プロパティ

        [Category("種別")]
        [DisplayName("番号")]
        [Visible(true)]
        public int Number
        {
            set
            {
                if (value <= 9999 && value >= 0)
                {
                    _Number = value;
                    this.Text = _Number.ToString().PadLeft(4, '0') + " - " + _TitleName;
                    MainForm.Instance.ProjectForm.Redraw();
                }
            }
            get
            {
                return _Number;
            }
        }

        [Category("種別")]
        [DisplayName("タイトル名")]
        [Visible(true)]
        public string TitleName
        {
            set
            {
                if (_TitleName.Length <= BaseFormNameMaxLength)
                {
                    _TitleName = value;
                    this.Text = _Number.ToString().PadLeft(4, '0') + " - " + _TitleName;
                    MainForm.Instance.ProjectForm.Redraw();
                }
            }
            get
            {
                return _TitleName;
            }
        }

        [DisplayName("タイトルフォント")]
        [Visible(true)]
        public new Font Font
        {
            set { base.Font = value; }
            get { return base.Font; }
        }

        [DisplayName("背景色")]
        [Visible(true)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        private Size _BaseFormSize = new Size();

        [Category("配置")]
        [DisplayName("サイズ")]
        [Visible(true)]
        public Size BaseFormSize
        {
            get
            {
                return _BaseFormSize;
            }
            set
            {
                //クライアント外領域算出
                int MargeWidth = this.Size.Width - this.ClientSize.Width;
                int MargeHeight = this.Size.Height - this.ClientSize.Height;

                //クライアント領域算出
                int ClientSizeWidth = value.Width - MargeWidth;
                int ClientSizeHeight = value.Height - MargeHeight;

                //クライアント領域にZoom補正をかけ、クライアント外サイズを加算する
                Size NewSize = new Size((int)(ClientSizeWidth * this.Zoom) + MargeWidth, (int)(ClientSizeHeight * this.Zoom) + MargeHeight);

                //サイズ変更
                SetWindowBounds(NewSize);

                _BaseFormSize = value;

            }
        }

        //public enum EnumViewDirection
        //{
        //    上, 下, 左, 右, 前, 後
        //}        

        //public class DirectionSettingFormat
        //{
        //    public DirectionSettingFormat()            
        //    {
        //        Direction = EnumViewDirection.上;
        //        IsUse = false;
        //        Offset = new Point(0, 0); ;
        //    }

        //    public DirectionSettingFormat( EnumViewDirection direction, bool isUse, Point offset)
        //    {
        //        Direction = direction;
        //        IsUse = isUse;
        //        Offset = offset;
        //    }

        //    public override string ToString()
        //    {
        //        return IsUse.ToString() + "," + "{" + Offset.X.ToString() + "," + Offset.Y.ToString() + "}";
        //    }

        //    private bool _IsUse;

        //    private Point _Offset;
        //    [Browsable(false)]
        //    public EnumViewDirection Direction;

        //    [DisplayName("描画フラグ")]
        //    public bool IsUse
        //    {
        //        get { return _IsUse; }
        //        set
        //        {
        //            BaseForm frm = (BaseForm)MainForm.Instance.PropertyView.SelectedObject;
        //            if (frm == null || frm.Direction != Direction)
        //            {
        //                _IsUse = value;
        //            }
        //            else
        //            {
        //                _IsUse = true;
        //            }
        //        }
        //    }

        //    [DisplayName("オフセット座標")]
        //    public Point Offset
        //    {
        //        get { return _Offset; }
        //        set { _Offset = value; }
        //    }

        //    #region イベント
        //    public event EventHandler ValueChanged;

        //    public void OnValueChanged(EventArgs e)
        //    {
        //        if (ValueChanged != null)
        //        {
        //            ValueChanged(this, e);
        //        }
        //    }
        //    #endregion

        //}
        //[TypeConverter(typeof(NestConverter))]
        //public class ViewSettingFormat
        //{
        //    [Browsable(false)]
        //    public List<DirectionSettingFormat> DirectionSettings { get; private set; }

        //    public DirectionSettingFormat this[EnumViewDirection direction]
        //    {
        //        get
        //        {
        //            PropertyDescriptorCollection pdCollection = TypeDescriptor.GetProperties(this);

        //            foreach (PropertyDescriptor pi in pdCollection)
        //            {
        //                if (pi.PropertyType == typeof(DirectionSettingFormat))
        //                {                            

        //                    DirectionSettingFormat obj = (DirectionSettingFormat)pi.GetValue(this);
        //                    if (obj.Direction == direction)
        //                    {
        //                        return obj;
        //                    }

        //                }


        //            }
        //            return null;
        //        }
        //    }

        //    public ViewSettingFormat()
        //    {  
        //        UpDirection = new DirectionSettingFormat(EnumViewDirection.上,  true, new Point(0, 0));
        //        DownDirection = new DirectionSettingFormat(EnumViewDirection.下, false, new Point(0, 0));
        //        FrontDirection = new DirectionSettingFormat(EnumViewDirection.前, false, new Point(0, 0));
        //        BackDirection = new DirectionSettingFormat(EnumViewDirection.後, false, new Point(0, 0));
        //        LeftDirection = new DirectionSettingFormat(EnumViewDirection.左, false, new Point(0, 0));
        //        RightDirection = new DirectionSettingFormat(EnumViewDirection.右, false, new Point(0, 0));

        //        DirectionSettings = new List<DirectionSettingFormat>();
        //        DirectionSettings.Add(UpDirection);
        //        DirectionSettings.Add(DownDirection);
        //        DirectionSettings.Add(FrontDirection);
        //        DirectionSettings.Add(BackDirection);
        //        DirectionSettings.Add(LeftDirection);
        //        DirectionSettings.Add(RightDirection);
        //    }

        //    [DisplayName("上面")]
        //    [TypeConverter(typeof(NestConverter))]
        //    public DirectionSettingFormat UpDirection
        //    {
        //        get;
        //        set;
        //    }

        //    [DisplayName("下面")]
        //    [TypeConverter(typeof(NestConverter))]
        //    public DirectionSettingFormat DownDirection 
        //    { 
        //        get; 
        //        set; 
        //    }

        //    [DisplayName("前面")]
        //    [TypeConverter(typeof(NestConverter))]
        //    public DirectionSettingFormat FrontDirection { get; set; }

        //    [DisplayName("背面")]
        //    [TypeConverter(typeof(NestConverter))]
        //    public DirectionSettingFormat BackDirection { get; set; }

        //    [DisplayName("左面")]
        //    [TypeConverter(typeof(NestConverter))]
        //    public DirectionSettingFormat LeftDirection { get; set; }

        //    [DisplayName("右面")]
        //    [TypeConverter(typeof(NestConverter))]
        //    public DirectionSettingFormat RightDirection { get; set; }



        //}


        //private ViewSettingFormat _ViewSetting = new ViewSettingFormat();


        //[TypeConverter(typeof(NestConverter))]
        //[Category("描画設定")]
        //[DisplayName("描画面設定")]
        //[Visible(true)]
        //public ViewSettingFormat ViewSetting
        //{
        //    get { return _ViewSetting; }
        //    set { _ViewSetting = value; }
        //}

        //private EnumViewDirection _Direction;



        //[Category("描画設定")]
        //[DisplayName("基準面")]
        //[Visible(true)]
        //public EnumViewDirection Direction
        //{
        //    get { return _Direction; }
        //    set
        //    {
        //        DirectionSettingFormat obj = ViewSetting[value];
        //        obj.IsUse = true;
        //        _Direction = value;
        //    }
        //}

        #endregion


        private Point SelectStartPoint;

        public OriginalCollection<ShapeObject> ShapeCollection = new OriginalCollection<ShapeObject>();

        private Cursor MoveCursor = Cursors.Default;

        public int SelectedCount { get; set; }

        public bool PasteMode = false;

        private void BaseForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (MainCtrl.IsShapeMaking) { return; }
            switch (e.KeyCode)
            {
                #region Escキー押下時
                case Keys.Escape:

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
                    //強制的にマウス移動イベントを上げて、カーソルを元に戻す。
                    Point NowPoint = this.PointToClient(Cursor.Position);
                    MouseEventArgs Mea = new MouseEventArgs(MouseButtons.None, 0, NowPoint.X, NowPoint.Y, 0);

                    this.BaseForm_MouseMove(null, Mea);

                    break;

                #endregion;

                #region Deleteキー押下時
                case Keys.Delete:

                    List<ShapeObject> selectObjects = new List<ShapeObject>();

                    selectObjects = ShapeCollection.FindAll(delegate(ShapeObject selectObject)
                    {
                        return selectObject.Selected;
                    });
                    ShapeObjectDeleteCommand command = new ShapeObjectDeleteCommand(this, selectObjects);
                    command.Execute();
                    CommandManager.UndoCommandStack.Push(command);

                    MainForm.Instance.PropertyView.SelectedObject = null;

                    break;

                #endregion;

                #region 上下左右キー押下時
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:

                    foreach (ShapeObject obj in ShapeCollection)
                    {
                        if (obj.Selected)
                        {
                            ShapeObjectLittleMoving shapeObjectLittleMoving = new ShapeObjectLittleMoving(this, e);
                            return;
                        }
                    }
                    break;
                #endregion

                #region コピー・切り取り・貼り付け
                case Keys.C:
                    if (e.Modifiers == Keys.Control)
                    {
                        //MainCtrl.ShowContextMenuPoint = ZoomManager.ToBasePoint(PointToClient(Cursor.Position), this.Zoom);
                        //this.shapeManager.PastePoint = ZoomManager.ToBasePoint(PointToClient(Cursor.Position), this.Zoom);
                        CopyContextMenu_Click(null, null);
                    }
                    break;

                case Keys.V:
                    if (e.Modifiers == Keys.Control)
                    {
                        //MainCtrl.ShowContextMenuPoint = ZoomManager.ToBasePoint(PointToClient(Cursor.Position), this.Zoom);
                        this.shapeManager.PastePoint = ZoomManager.ToBasePoint(PointToClient(Cursor.Position), this.Zoom);
                        PasteContextMenu_Click(null, null);
                    }
                    break;

                case Keys.X:
                    if (e.Modifiers == Keys.Control)
                    {
                        //MainCtrl.ShowContextMenuPoint = ZoomManager.ToBasePoint(PointToClient(Cursor.Position), this.Zoom);
                        //this.shapeManager.PastePoint = ZoomManager.ToBasePoint(PointToClient(Cursor.Position), this.Zoom);
                        CutContextMenu_Click(null, null);
                    }
                    break;

                case Keys.Z:
                    if (e.Modifiers == Keys.Control)
                    {
                        if (CommandManager.UndoCommandStack.Count != 0)
                        {
                            ICommand UndoCommand = CommandManager.UndoCommandStack.Pop();
                            UndoCommand.Undo();
                            CommandManager.RedoCommandStack.Push(UndoCommand);
                        }
                    }
                    break;

                case Keys.Y:
                    if (e.Modifiers == Keys.Control)
                    {
                        if (CommandManager.RedoCommandStack.Count != 0)
                        {
                            ICommand RedoCommand = CommandManager.RedoCommandStack.Pop();
                            RedoCommand.Redo();
                            CommandManager.UndoCommandStack.Push(RedoCommand, false);
                        }
                    }
                    break;

                #endregion
            }

            MainForm.Instance.PropertyView.Refresh();
        }

        #region 通常イベント
        public void BaseForm_Load()
        {
        }


        #endregion

        #region マウスイベント＆ペイントイベント
        private void BaseForm_MouseDown(object sender, MouseEventArgs e)
        {
            MainForm.Instance.ActiveControl = this;

            if (MainCtrl.IsShapeMoving == true) { return; }
            if (MainCtrl.IsShapeMaking == true) { return; }
            if (MainCtrl.IsShapeChanging == true) { return; }

            // 描画モードOFF時
            if (MainCtrl.ToolTipNotSelect == true && MainCtrl.SymbolViewListSelect == false)
            {
                #region 変形処理
                for (int i = ShapeCollection.Count - 1; i >= 0; i--)
                {
                    ShapeObject obj = ShapeCollection[i];

                    //現在選択中 AND 変形範囲検索
                    if (obj.Selected == true && obj.JudgeModifyRange(e.Location) != "None")
                    {
                        SelectChange(obj);
                        ShapeObjectChanging shapeObjectChanging = new ShapeObjectChanging(this, obj, e);
                        return;
                    }
                }
                #endregion

                #region 移動処理
                for (int i = ShapeCollection.Count - 1; i >= 0; i--)
                {
                    ShapeObject obj = ShapeCollection[i];

                    //現在非選択中 AND 選択範囲検索 AND (Ctrlｷｰ OR Shiftｷｰ)
                    if (obj.Selected == false && obj.JudgeMoveRange(e.Location) && (((Control.ModifierKeys & Keys.Control) == Keys.Control) || ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            //選択に追加する
                            obj.Selected = true;
                            SelectedCount = SelectedCount + 1;
                            //ShapeObject SelectedObj = obj;
                            if (SelectedCount > 1)
                            {
                                MainCtrl.PropertyView_Set(null);
                            }
                            else if (SelectedCount == 1)
                            {
                                MainCtrl.PropertyView_Set(obj);
                            }

                            this.Refresh();

                            ShapeObjectMoving shapeObjectMoving = new ShapeObjectMoving(this, e);
                            return;
                        }
                        else
                        {
                            SelectChange(obj);
                            ShowContextMenu(obj, e.Location);
                            return;
                        }
                    }
                    //現在選択中 AND 選択範囲検索 AND (Ctrlｷｰ OR Shiftｷｰ)
                    else if (obj.Selected == true && obj.JudgeMoveRange(e.Location) && (((Control.ModifierKeys & Keys.Control) == Keys.Control) || ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            //選択から外す
                            obj.Selected = false;
                            SelectedCount = SelectedCount - 1;
                            //ShapeObject SelectedObj = obj;
                            if (SelectedCount > 1)
                            {
                                MainCtrl.PropertyView_Set(null);
                            }
                            else if (SelectedCount == 1)
                            {
                                //残った1つを選択
                                for (int j = ShapeCollection.Count - 1; j >= 0; j--)
                                {
                                    if (ShapeCollection[j].Selected == true)
                                    {
                                        obj = ShapeCollection[j];
                                    }
                                }
                                MainCtrl.PropertyView_Set(obj);
                            }

                            this.Refresh();

                            if (SelectedCount > 0)
                            {
                                ShapeObjectMoving shapeObjectMoving = new ShapeObjectMoving(this, e);
                            }
                            return;
                        }
                        else
                        {
                            SelectChange(obj);
                            ShowContextMenu(obj, e.Location);
                            return;
                        }
                    }
                    //現在選択中 AND 選択範囲検索
                    else if (obj.Selected == true && obj.JudgeMoveRange(e.Location))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            SelectedCount = 1;
                            ShapeObjectMoving shapeObjectMoving = new ShapeObjectMoving(this, e);
                            return;
                        }
                        else
                        {
                            ShowContextMenu(obj, e.Location);
                            return;
                        }
                    }

                    //現在非選択中 AND 選択範囲検索
                    else if (obj.Selected == false && obj.JudgeMoveRange(e.Location))
                    {
                        SelectChange(obj);
                        if (e.Button == MouseButtons.Left)
                        {
                            SelectedCount = 1;
                            ShapeObjectMoving shapeObjectMoving = new ShapeObjectMoving(this, e);
                            return;
                        }
                        else
                        {
                            ShowContextMenu(obj, e.Location);
                            return;
                        }
                    }

                }
                #endregion

                //選択中Object無し OR !(Ctrlｷｰ OR Shiftｷｰ)
                if (!((Control.ModifierKeys & Keys.Control) == Keys.Control || ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)))
                {
                    SelectedCount = 0;
                    foreach (ShapeObject obj in ShapeCollection)
                    {
                        obj.Selected = false;
                    }
                }

                //コンテキストメニュー表示
                if (e.Button == MouseButtons.Right)
                {
                    this.Refresh();
                    ShowContextMenu(this, e.Location);
                    return;
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (SelectedCount == 0)
                    {
                        //BaseFormプロパティ表示
                        MainCtrl.PropertyView_Set(this);
                    }

                    //範囲選択開始
                    MainCtrl.RangeSelectMode = true;
                    SelectStartPoint = e.Location;
                    this.Refresh();
                    return;
                }

            }
            else if (e.Button == MouseButtons.Left && MainCtrl.IsShapeMaking == false)
            {
                if (MainCtrl.ToolTipNotSelect == false || MainCtrl.SymbolViewListSelect == true)
                {
                    ShapeObjectMaking shapeObjectmaking = new ShapeObjectMaking(this, e);
                    SelectedCount = 1;
                }
            }


            MainForm.Instance.ActiveControl = this;
        }

        public void BaseForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainCtrl.IsShapeChanging) { return; }
            if (MainCtrl.IsShapeMaking) { return; }
            if (MainCtrl.IsShapeMoving) { return; }

            if (MainCtrl.RangeSelectMode)
            {
                this.Refresh();
            }

            #region カーソル変形

            //カーソル変形（描画）
            if (MainCtrl.SymbolViewListSelect || !MainCtrl.ToolTipNotSelect)
            {
                this.Cursor = Cursors.Cross;
                MoveCursor = Cursors.Cross;
                return;
            }

            //カーソル変形（通常）
            this.Cursor = Cursors.Default;

            //カーソル変形（移動）
            for (int i = ShapeCollection.Count - 1; i >= 0; i--)
            {
                ShapeObject obj = ShapeCollection[i];
                this.Cursor = obj.GetMoveCursor(e);
            }

            //カーソル変形（変形）
            for (int i = ShapeCollection.Count - 1; i >= 0; i--)
            {
                ShapeObject obj = ShapeCollection[i];
                this.Cursor = obj.GetModifyCursor(e);

            }

            #endregion

        }

        private void BaseForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (MainCtrl.IsShapeChanging) { return; }
            if (MainCtrl.IsShapeMaking) { return; }
            if (MainCtrl.IsShapeMoving) { return; }


            MainForm.Instance.ActiveControl = this;

            #region 範囲選択終了
            if (MainCtrl.RangeSelectMode)
            {
                Point NowPoint = this.PointToClient(Cursor.Position);
                //SelectedCount = 0;
                ShapeObject SelectedObj = null;

                //範囲内に入っているオブジェクトをすべて選択する。
                if (!(((Control.ModifierKeys & Keys.Control) == Keys.Control) || ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)))
                {
                    foreach (ShapeObject obj in ShapeCollection)
                    {
                        if (obj.JudgeSelectRange(SelectStartPoint, NowPoint))
                        {
                            obj.Selected = true;
                            SelectedCount = SelectedCount + 1;
                            SelectedObj = obj;
                        }
                    }

                    if (SelectedCount > 1)
                    {
                        MainCtrl.PropertyView_Set(null);
                    }
                    else if (SelectedCount == 1)
                    {
                        MainCtrl.PropertyView_Set(SelectedObj);
                    }
                }

                //範囲内に入っているオブジェクトの選択状態をすべて反転する。
                else
                {
                    foreach (ShapeObject obj in ShapeCollection)
                    {
                        if (obj.JudgeSelectRange(SelectStartPoint, NowPoint))
                        {
                            obj.Selected = !obj.Selected;
                            if (obj.Selected)
                            {
                                SelectedCount = SelectedCount + 1;
                            }
                            else
                            {
                                SelectedCount = SelectedCount - 1;
                            }
                        }
                    }

                    if (SelectedCount > 1)
                    {
                        MainCtrl.PropertyView_Set(null);
                    }
                    else if (SelectedCount == 1)
                    {
                        //残った1つを選択
                        for (int j = ShapeCollection.Count - 1; j >= 0; j--)
                        {
                            if (ShapeCollection[j].Selected == true)
                            {
                                SelectedObj = ShapeCollection[j];
                            }
                        }
                        MainCtrl.PropertyView_Set(SelectedObj);
                    }
                }

                MainCtrl.RangeSelectMode = false;
                this.Refresh();
            }
            #endregion


        }

        private void BaseForm_Paint(object sender, PaintEventArgs e)
        {
            //Console.WriteLine(DateTime.Now.Millisecond + " PaintEvent");
            if (DesignMode) { return; }

            #region 範囲選択描画
            if (MainCtrl.RangeSelectMode && ParentForm.ActiveMdiChild == this)
            {
                Point Pt1 = SelectStartPoint;
                Point Pt2 = this.PointToClient(Cursor.Position);

                e.Graphics.DrawLine(Pens.Gray, Pt1.X, Pt1.Y, Pt2.X, Pt1.Y);
                e.Graphics.DrawLine(Pens.Gray, Pt2.X, Pt1.Y, Pt2.X, Pt2.Y);
                e.Graphics.DrawLine(Pens.Gray, Pt2.X, Pt2.Y, Pt1.X, Pt2.Y);
                e.Graphics.DrawLine(Pens.Gray, Pt1.X, Pt2.Y, Pt1.X, Pt1.Y);
            }
            #endregion

            #region ShapeObject再描画

            foreach (ShapeObject obj in ShapeCollection) { obj.Redraw(e); }

            //選択枠描画
            Form MainFrm = this.MdiParent;

            if (MainForm.Instance.ActiveMdiChild == this)
            {
                foreach (ShapeObject obj in ShapeCollection)
                {
                    if (obj.Selected) { obj.DrawSelect(e); }
                }
            }

            #endregion

            #region Grid表示
            if (MainCtrl.GridMode)
            {
                Size GridSize = ZoomManager.MagnifySize(MainCtrl.GridSize, this.Zoom);
                ControlPaint.DrawGrid(e.Graphics, this.ClientRectangle, GridSize, Color.White);
            }
            #endregion
        }
        #endregion

        public void SelectChange(ShapeObject shapeObject)
        {

            foreach (ShapeObject obj in ShapeCollection)
            {
                obj.Selected = false;
            }
            shapeObject.Selected = true;

            this.Refresh();
            MainCtrl.PropertyView_Set(shapeObject);

        }


        #region コンテキストメニュー

        public void ShowContextMenu(object sender, Point Pt)
        {
            ContextMenuStrip Cms = new ContextMenuStrip();

            string tp = sender.GetType().BaseType.Name;

            if (tp == "ShapeObject")
            {
                ToolStripMenuItem CutContextMenu = new ToolStripMenuItem();
                CutContextMenu.Text = "切り取り";
                CutContextMenu.Click += new EventHandler(CutContextMenu_Click);
                Cms.Items.Add(CutContextMenu);

                ToolStripMenuItem CopyContextMenu = new ToolStripMenuItem();
                CopyContextMenu.Text = "コピー";
                CopyContextMenu.Click += new EventHandler(CopyContextMenu_Click);
                Cms.Items.Add(CopyContextMenu);

                ToolStripMenuItem FrontContextMenu = new ToolStripMenuItem();
                FrontContextMenu.Text = "最前面へ移動";
                FrontContextMenu.Click += new EventHandler(FrontContextMenu_Click);
                Cms.Items.Add(FrontContextMenu);

                ToolStripMenuItem BackContextMenu = new ToolStripMenuItem();
                BackContextMenu.Text = "最背面へ移動";
                BackContextMenu.Click += new EventHandler(BackContextMenu_Click);
                Cms.Items.Add(BackContextMenu);

            }
            else
            {
                ToolStripMenuItem PasteContextMenu = new ToolStripMenuItem();
                PasteContextMenu.Text = "貼り付け";
                PasteContextMenu.Click += new EventHandler(PasteContextMenu_Click);
                Cms.Items.Add(PasteContextMenu);

                this.shapeManager.PastePoint = ZoomManager.ToBasePoint(PointToClient(Cursor.Position), this.Zoom);
                //MainCtrl.ShowContextMenuPoint = ZoomManager.ToBasePoint(PointToClient(Cursor.Position),this.Zoom);
            }
            Cms.Show(this, Pt);


        }

        private void CutContextMenu_Click(object sender, EventArgs e)
        {
            this.shapeManager.Cut(this);

            //bool SelectedObject = false;

            ////選択されているオブジェクトがあるか確認
            //foreach (ShapeObject obj in ShapeCollection)
            //{                
            //    if (obj.Selected)
            //    {
            //        SelectedObject = true;
            //        break;
            //    }
            //}

            //if (!SelectedObject) { return; }

            //MainCtrl.ClipBoardObject = new List<ShapeObject>();
            //MainCtrl.ClipBoardObjectBaseX = 0;
            //MainCtrl.ClipBoardObjectBaseY = 0;

            ////切り取り処理
            //foreach (ShapeObject obj in ShapeCollection)
            //{
            //    obj.Clipboard_Cut();
            //}
            ////現在選択数をゼロにする。
            //SelectedCount = 0;
            //this.Refresh();
            //MainCtrl.PropertyView_Set(null);

        }

        private void CopyContextMenu_Click(object sender, EventArgs e)
        {

            this.shapeManager.Copy(this);

        }

        private void PasteContextMenu_Click(object sender, EventArgs e)
        {
            //全ての選択を解除する。
            foreach (ShapeObject obj in ShapeCollection)
            {
                obj.Selected = false;
            }

            if (this.shapeManager.ClipBoardObjects.Count == 0) { return; }

            int pasteX = this.shapeManager.PastePoint.X - this.shapeManager.CopyPoint.X;
            int pasteY = this.shapeManager.PastePoint.Y - this.shapeManager.CopyPoint.Y;
            Point pasteDistance = new Point(pasteX, pasteY);

            ShapeObjectPasteCommand pasteCommand =
                new ShapeObjectPasteCommand(this.shapeManager, this, pasteDistance, this.shapeManager.ClipBoardObjects);

            pasteCommand.Execute();
            CommandManager.UndoCommandStack.Push(pasteCommand);

        }

        private void FrontContextMenu_Click(object sender, EventArgs e)
        {
            //最前面へ移動処理
            int MaxCount = ShapeCollection.Count;
            int SortNumber = 0;
            for (int i = 0; i < MaxCount; i++)
            {
                ShapeObject obj = ShapeCollection[SortNumber];
                if (obj.Selected)
                {
                    ShapeCollection.Remove(obj);
                    //SelectedCount = SelectedCount - 1;
                    ShapeCollection.Add(obj);
                }
                else { SortNumber++; }

            }

            this.Refresh();
        }

        private void BackContextMenu_Click(object sender, EventArgs e)
        {
            //最背面へ移動処理
            int MaxCount = ShapeCollection.Count;
            int SortNumber = 0;
            for (int i = 0; i < MaxCount; i++)
            {
                ShapeObject obj = ShapeCollection[SortNumber];
                if (obj.Selected)
                {
                    ShapeCollection.Remove(obj);

                    if (obj.ShapeType != "ViewObject")
                    {
                        ShapeCollection.Insert(0, obj);
                    }
                    else
                    {
                        int Index = ShapeCollection.Count;
                        for (int j = ShapeCollection.Count - 1; j >= 0; j--)
                        {
                            ShapeObject InObj = ShapeCollection[j];
                            if (InObj.ShapeType == "ViewObject")
                            {
                                Index = j;
                            }
                        }
                        ShapeCollection.Insert(Index, obj);
                    }
                }
                SortNumber++;
            }

            this.Refresh();
        }


        #endregion

        #region 保存処理
        public BaseFormDataSerializeFormat Serialize()
        {

            //保存するクラスのインスタンスを作成
            BaseFormDataSerializeFormat baseFormData = new BaseFormDataSerializeFormat();

            baseFormData = (BaseFormDataSerializeFormat)SerializeSupport.Serialize(this);

            baseFormData.SymbolData = new List<SymbolDataSerializeFormat>();
            baseFormData.ShapeData = new List<ShapeDataSerializeFormat>();

            foreach (ShapeObject obj in ShapeCollection)
            {
                if (obj.ShapeType == "ViewObject")
                {
                    SymbolDataSerializeFormat objSerialize = (SymbolDataSerializeFormat)SerializeSupport.Serialize(((ViewObject)obj).ControlInstance);

                    if (objSerialize != null)
                    {
                        baseFormData.SymbolData.Add(objSerialize);
                    }
                }
                else
                {
                    ShapeDataSerializeFormat objSerialize = (ShapeDataSerializeFormat)SerializeSupport.Serialize(obj);
                    if (objSerialize != null)
                    {
                        baseFormData.ShapeData.Add(objSerialize);
                    }
                }
            }

            return baseFormData;
        }


        #endregion

        #region 復元処理

        public void Deserialize(BaseFormDataSerializeFormat baseFormData)
        {
            #region ShapeObjectロード

            foreach (ShapeDataSerializeFormat ShapeData in baseFormData.ShapeData)
            {

                ShapeObject MyShape = (ShapeObject)SerializeSupport.Deserialize(ShapeData, Type.GetType("LocalSimulator.ProjectMaker." + ShapeData.Type));

                MyShape.ParentForm = this;

                ShapeCollection.Add(MyShape);

                MyShape.ReMake();
            }

            #endregion

            #region SymbolObjectロード

            foreach (SymbolDataSerializeFormat SymbolData in baseFormData.SymbolData)
            {
                Symbol_Draw Symbol = (Symbol_Draw)SerializeSupport.Deserialize(SymbolData);

                ViewObject MyShape = new ViewObject();
                MyShape.ParentForm = this;
                MyShape.ControlInstance = Symbol;
                MyShape.Location = Symbol.Location;
                Symbol.Tag = MyShape;
                MyShape.Size = Symbol.Size;

                ShapeCollection.Add(MyShape);

                MainCtrl.RefreshControlBmp(Symbol, MyShape);

                Global.LogManager.Write("シンボル読み出し完了 " + Symbol.SymbolName);

            }
            #endregion

            this.Refresh();

        }
        #endregion

        //シンボルプロパティ変更
        public void PropertyChanged(object SourceObject, PropertyValueValidatingEventArgs e)
        {
            #region SymbolNameプロパティ変更
            if (e.ChangedItem.PropertyDescriptor != null)
            {
                if (e.ChangedItem.PropertyDescriptor.Name == "SymbolName")
                {
                    foreach (ShapeObject obj in ShapeCollection)
                    {
                        if (obj.ShapeType == "ViewObject")
                        {
                            ViewObject vobj = (ViewObject)obj;
                            if (vobj.ControlInstance.SymbolName == (string)e.ChangedItem.Value)
                            {
                                if (vobj.ControlInstance != SourceObject)
                                {
                                    //同じIDが存在する場合、設定値を元に戻す。
                                    MessageBox.Show("名称が重複しています。変更してください。", "警告",
                                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    e.Cancel = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region デバイスプロパティ変更
            do
            {
                DeviceFormat changedDevice = null;

                // DeviceFormat 非展開
                if (e.ChangedItem.Value is DeviceFormat)
                {
                    changedDevice = (DeviceFormat)e.ChangedItem.Value;
                }
                // DeviceFormat 展開(Value以外)
                //else if (typeof(DeviceFormat).IsAssignableFrom(e.ChangedItem.PropertyDescriptor.ComponentType))
                //{
                //    if (e.ChangedItem.Parent == null) { throw new ApplicationException(); }

                //    // DeviceFormatを指すGridItem
                //    GridItem DeviceItem = e.ChangedItem.Parent;

                //    ChangedDevice = (DeviceFormat)DeviceItem.Value;
                //}
                // DeviceFormat 展開(Value)
                //else if (e.ChangedItem.PropertyDescriptor.ComponentType == typeof(UInt16[]))
                //{
                //    if (e.ChangedItem.Parent == null) { throw new ApplicationException(); }

                //    // DeviceFormat.Valueを指すGridItem
                //    GridItem ValueItem = e.ChangedItem.Parent;

                //    // DeviceFormatと関係のないプロパティ
                //    if (!typeof(DeviceFormat).IsAssignableFrom(ValueItem.PropertyDescriptor.ComponentType))
                //    {
                //        break;
                //    }

                //    if (ValueItem.Parent == null) { throw new ApplicationException(); }

                //    // DeviceFormatを指すGridItem
                //    GridItem DeviceItem = ValueItem.Parent;


                //    // ...特に処理をすることはない
                //}

                // DeviceFormat 有効性チェック
                if (changedDevice != null)
                {
                    // 空白を認める
                    if (string.IsNullOrEmpty(changedDevice.Address))
                    {
                        break;
                    }

                    // アドレスチェック
                    if (!DeviceManager.IsValidate(changedDevice.Address))
                    {
                        MessageBox.Show("入力されたアドレスは不正です。", "警告",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                        return;
                    }

                    // ワード／ビット
                    DeviceElement inputDevice = DeviceManager.ToElement(changedDevice.Address);
                    bool inputIsBit = false;

                    // 入力 = ビットデバイス
                    if (inputDevice.DeviceType == DeviceType.BitDevice
                        || inputDevice.BitOffset != -1)
                    {
                        inputIsBit = true;
                    }

                    // 入力 = ビットデバイス
                    // かつ、DeviceFormat型がビットデバイスを許可していない
                    if (inputIsBit && (changedDevice.InputType & SetType.Bit) == 0)
                    {
                        MessageBox.Show("このプロパティにはワードデバイスのみ有効です。", "警告",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                        return;
                    }
                    // 入力 = ワードデバイス
                    // かつ、DeviceFormat型がワードデバイスを許可していない
                    if (!inputIsBit && (changedDevice.InputType & SetType.Word) == 0)
                    {
                        MessageBox.Show("このプロパティにはビットデバイスのみ有効です。", "警告",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                        return;
                    }

                    // アドレスを正規化
                    changedDevice.Address = DeviceManager.ToRegex(changedDevice.Address);
                }

            } while (false);
            #endregion

            #region Locationプロパティ変更

            if (e.ChangedItem.PropertyDescriptor != null)
            {
                if (e.ChangedItem.PropertyDescriptor.Name == "Location" && e.ChangedItem.Value is Point)
                {
                    Point chagePoint = (Point)e.ChangedItem.Value;
                }
            }

            #endregion

            if (SourceObject.GetType().BaseType == typeof(ShapeObject))
            {
                ShapeObject obj = (ShapeObject)SourceObject;
                obj.ReMake();
            }

            if (typeof(Symbol_Draw).IsAssignableFrom(SourceObject.GetType()))
            {
                ViewObject obj = (ViewObject)((Symbol_Draw)SourceObject).Tag;
                MainCtrl.RefreshControlBmp(obj.ControlInstance, obj);
            }

            this.Refresh();
        }

        private void BaseForm_Load(object sender, EventArgs e)
        {
            this.Text = _Number.ToString().PadLeft(4, '0') + " - " + _TitleName;
            this.Name = "BaseFormFormat";
        }

        private void BaseForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            IoListForm.Instance.RemoveSymbol(this);
        }


        /// <summary>
        /// SymbolNameを取得する。（重複チェックを行う）        
        /// </summary>
        public string GetSymbolName(Symbol_Draw Symbol)
        {
            if (String.IsNullOrEmpty(Symbol.SymbolName) == false)
            {
                //現在の名前と重複がないか調査する。
                bool IsSame = false;
                foreach (ShapeObject obj in ShapeCollection)
                {
                    if (obj.GetType() == typeof(ViewObject))
                    {
                        Symbol_Draw CompareSymbol = ((ViewObject)obj).ControlInstance;
                        if (CompareSymbol.SymbolName == Symbol.SymbolName || Symbol.SymbolName == "")
                        {
                            IsSame = true;
                            break;
                        }
                    }
                }
                //同じ名前がない場合はそのまま返す
                if (IsSame == false) { return Symbol.SymbolName; }

            }

            string SymbolType = Symbol.GetType().Assembly.GetName().Name;

            //ID重複を検索

            int i = 0;
            int Number = 1;

            while (i != ShapeCollection.Count)
            {
                if (ShapeCollection[i].ShapeType == "ViewObject")
                {
                    ViewObject obj = (ViewObject)ShapeCollection[i];
                    Symbol_Draw CompareSymbol = (Symbol_Draw)obj.ControlInstance;
                    if (SymbolType + Number == CompareSymbol.SymbolName)
                    {
                        Number = Number + 1;

                        i = -1;
                    }
                }

                i = i + 1;

            }

            return SymbolType + Number;

        }

        /// <summary>
        /// ウィンドウの位置とサイズを変更する
        /// </summary>
        /// <param name="ModifySize">変更後のウィンドウのサイズ</param>
        public void SetWindowBounds(Size ModifySize)
        {
            //MaximumSizeを大きくしておく

            this.MaximumSize = ModifySize;

            MoveWindow(this.Handle, this.Location.X, this.Location.Y, ModifySize.Width, ModifySize.Height, true);

            this.MinimumSize = ModifySize;

            //this.UpdateBounds();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return true;
            //return base.ProcessCmdKey(ref msg, keyData);
        }

        private void BaseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
    }

    public class BaseFormSurrogate : ISerializationSurrogate
    {

        #region ISerializationSurrogate メンバ

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            BaseForm frm = (BaseForm)obj;

            PropertyInfo[] PiCollection = frm.GetType().GetProperties();

            foreach (PropertyInfo pi in PiCollection)
            {
                if (Global.GetVisibleAttribute(frm, pi))
                {
                    info.AddValue(pi.Name, pi.GetValue(frm, null));
                }
            }


        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
