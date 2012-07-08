using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using CommonClassLibrary;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace LocalSimulator.ProjectMaker
{   
    [Serializable]
    public class ViewObject : ShapeObject
    {
        
        private Point       drawLocation;
        private Size        drawSize;

        private Point       _Location;
        private Size        _Size;
        private Symbol_Draw _ControlInstance;  

        #region プロパティ
        
        public Point Location
        {
            get { return _Location; }
            set
            {
                _Location = value;
                ControlInstance.Location = value;
            }
        }
        
        public  Size Size
        {
            get { return _Size; }
            set
            {
                Size OldSize = _Size;
                _Size = value;
                ControlInstance.Size = value;
                //Debug.WriteLine(value.ToString());
                // サイズ変更チェック
                if (!Size.Equals(OldSize))
                {
                    MainCtrl.RefreshControlBmp(ControlInstance, this);
                }                
            }
        }

        public Symbol_Draw ControlInstance
        {
            get { return _ControlInstance; }
            set
            {
                _ControlInstance = value;
                value.LocationChanged += new EventHandler(Control_LocationChanged);
                value.SizeChanged += new EventHandler(Control_SizeChanged);
                value.RedrawOrder += new EventHandler(value_RedrawOrder);
            }
        }

        void value_RedrawOrder(object sender, EventArgs e)
        {
            MainCtrl.RefreshControlBmp(ControlInstance, this);
            this.ParentForm.Refresh();
        }
              


        public Bitmap ControlBmp{ get; set; }

        #endregion

        public override void Redraw(PaintEventArgs e)
        {
            if (ControlInstance.Zoom != ParentForm.Zoom)
            {
                ControlInstance.Zoom = ParentForm.Zoom;
                MainCtrl.RefreshControlBmp(ControlInstance, this);
            }

            drawLocation = ZoomManager.MagnifyPoint(Location, ParentForm.Zoom);
            drawSize = ZoomManager.MagnifySize(Size, ParentForm.Zoom);

            //画像を表示させる範囲
            Rectangle BmpRect = new Rectangle();

            BmpRect.Location = this.drawLocation;
            BmpRect.Size = this.drawSize;

            e.Graphics.DrawImage(ControlBmp, BmpRect);

        }

        public override void ReMake()
        {
            //Console.WriteLine("Remake");
            //MainCtrl.RefreshControlBmp(ControlInstance, this);
        }

        public override void DrawSelect(PaintEventArgs e)
        {
            //System.Diagnostics.Stopwatch sw = new Stopwatch();
            //sw.Start();

            DrawSelectCircle(e.Graphics, drawLocation);
            DrawSelectCircle(e.Graphics, new Point(drawLocation.X + drawSize.Width / 2, drawLocation.Y));
            DrawSelectCircle(e.Graphics, new Point(drawLocation.X + drawSize.Width, drawLocation.Y));
            DrawSelectCircle(e.Graphics, new Point(drawLocation.X + drawSize.Width, drawLocation.Y + drawSize.Height / 2));
            DrawSelectCircle(e.Graphics, new Point(drawLocation.X + drawSize.Width, drawLocation.Y + drawSize.Height));
            DrawSelectCircle(e.Graphics, new Point(drawLocation.X + drawSize.Width / 2, drawLocation.Y + drawSize.Height));
            DrawSelectCircle(e.Graphics, new Point(drawLocation.X, drawLocation.Y + drawSize.Height));
            DrawSelectCircle(e.Graphics, new Point(drawLocation.X, drawLocation.Y + drawSize.Height / 2));

            //Console.WriteLine(sw.Elapsed + " drawCircle ");
            
        }                       
         
        private void Control_SizeChanged(object sender, EventArgs e)
        {
            if (!ShapeObjectChangeMode)  //プロパティグリッド以外からの変更ではViewObjectサイズを同期させない。
            {
                if ((this.Size.Height != ControlInstance.Size.Height && ControlInstance.HeightStretch == false) ||
                   (this.Size.Width != ControlInstance.Size.Width && ControlInstance.WidthStretch == false))
                {
                    ControlInstance.Size = this.Size;
                }
                else
                {
                    this.Size = ControlInstance.Size;
                }
            }
        }

        private void Control_LocationChanged(object sender, EventArgs e)
        {
            if (!ShapeObjectChangeMode) { this.Location = ControlInstance.Location; }     //キャプチャ中は無視する。
        }

        public void CreateSymbol(string symbolName)
        {
            //コントロールのインスタンス生成            
            Assembly Asm = Assembly.LoadFrom(AppSetting.SymbolPath + "\\" + symbolName + ".dll");
            Type SymbolType = Asm.GetType(AppSetting.SymbolNameSpace + "." + symbolName + "_Draw");
            Symbol_Draw Symbol = (Symbol_Draw)Activator.CreateInstance(SymbolType);
            this.ControlInstance = Symbol;
            this.ControlInstance.Zoom = ParentForm.Zoom;

            //MainForm.Instance.nonPanel.Controls.Add(Symbol);

        }

        public override void Initialize(List<Point> settingPoints)
        {            

            //コントロールのインスタンス生成
            //string SymbolName = MainForm.Instance.SymbolView_List.SelectedItems[0].Text;
            //Assembly Asm = Assembly.LoadFrom(AppSetting.SymbolPath + "\\" + SymbolName + ".dll");
            //Type SymbolType = Asm.GetType(AppSetting.SymbolNameSpace + "." + SymbolName + "_Draw");
            //Symbol_Draw Symbol = (Symbol_Draw)Activator.CreateInstance(SymbolType);
                        
            //this.ControlInstance = Symbol;

            this.ControlInstance.Tag = this;

            this.Size = new Size(this.ControlInstance.Size.Width, this.ControlInstance.Size.Height);
            
            this.Location = ZoomManager.ToBasePoint(settingPoints[0], ParentForm.Zoom);

            //MainCtrl.RefreshControlBmp(this.ControlInstance, this);

            //SymbolName重複チェック
            this.ControlInstance.SymbolName = ParentForm.GetSymbolName(this.ControlInstance);

            //MainForm.Instance.NoVisiblePanel.Controls.Add(this.ControlInstance);
            
            ////ShapeCollection_Add(MyShape);
            
            //Frm.Refresh();
            MainCtrl.RefreshControlBmp(this.ControlInstance, this);
            
        }

        public override Cursor GetModifyCursor(MouseEventArgs e)
        {   
            string JudgeModify = this.JudgeModifyRange(e.Location);

            if (this.Selected)
            {                       
                //左上 & 右下    
                if (JudgeModify == "LeftUp" || JudgeModify == "RightDown")
                {
                    return Cursors.SizeNWSE;
                }
                //右上 & 左下    
                else if (JudgeModify == "RightUp" || JudgeModify == "LeftDown")
                {
                    return Cursors.SizeNESW;
                }
                //上 & 下    
                else if (JudgeModify == "Up" || JudgeModify == "Down")
                {
                    return  Cursors.SizeNS;
                }
                //左 & 右 
                else if (JudgeModify == "Left" || JudgeModify == "Right")
                {
                    return Cursors.SizeWE;
                }
            }
            return ParentForm.Cursor;
        }

        public override bool JudgeMoveRange(Point NowPt)
        {
            if (Deleted) { return false; }

            //中央点算出
            Point CenterPoint = new Point(drawLocation.X + drawSize.Width / 2, drawLocation.Y + drawSize.Height / 2);

            Point Pt1 = new Point(drawLocation.X - (3 + 1 / 2), drawLocation.Y - (3 + 1 / 2));                            //左上
            Point Pt2 = new Point(drawLocation.X - (3 + 1 / 2), drawLocation.Y + drawSize.Height + (3 + 1 / 2));              //左下
            Point Pt3 = new Point(drawLocation.X + drawSize.Width + (3 + 1 / 2), drawLocation.Y + drawSize.Height + (3 + 1 / 2)); //右下
            Point Pt4 = new Point(drawLocation.X + drawSize.Width + (3 + 1 / 2), drawLocation.Y - (3 + 1 / 2));               //右上

            //中央点－現在マウスポインタを線分とし、選択枠の四辺との交差を調べる
            bool CrossJudge1 = GetCrossJudge(CenterPoint, NowPt, Pt1, Pt2);
            bool CrossJudge2 = GetCrossJudge(CenterPoint, NowPt, Pt2, Pt3);
            bool CrossJudge3 = GetCrossJudge(CenterPoint, NowPt, Pt3, Pt4);
            bool CrossJudge4 = GetCrossJudge(CenterPoint, NowPt, Pt4, Pt1);

            if (!CrossJudge1 && !CrossJudge2 && !CrossJudge3 && !CrossJudge4) { return true; }

            return false;

        }   

        public override string JudgeModifyRange(Point NowPt)
        {
            if (Deleted) { return "None"; }

            bool LeftUp = JudgeModifyPoint(NowPt, drawLocation, 0);
            bool Up = JudgeModifyPoint(NowPt, new Point(drawLocation.X + drawSize.Width / 2, drawLocation.Y), 0);
            bool RightUp = JudgeModifyPoint(NowPt, new Point(drawLocation.X + drawSize.Width, drawLocation.Y), 0);
            bool Right = JudgeModifyPoint(NowPt, new Point(drawLocation.X + drawSize.Width, drawLocation.Y + drawSize.Height / 2), 0);
            bool RightDown = JudgeModifyPoint(NowPt, new Point(drawLocation.X + drawSize.Width, drawLocation.Y + drawSize.Height), 0);
            bool Down = JudgeModifyPoint(NowPt, new Point(drawLocation.X + drawSize.Width / 2, drawLocation.Y + drawSize.Height), 0);
            bool LeftDown = JudgeModifyPoint(NowPt, new Point(drawLocation.X, drawLocation.Y + drawSize.Height), 0);
            bool Left = JudgeModifyPoint(NowPt, new Point(drawLocation.X, drawLocation.Y + drawSize.Height / 2), 0);

            if (LeftUp) { return "LeftUp"; }
            if (Up) { return "Up"; }
            if (RightUp) { return "RightUp"; }
            if (Right) { return "Right"; }
            if (RightDown) { return "RightDown"; }
            if (Down) { return "Down"; }
            if (LeftDown) { return "LeftDown"; }
            if (Left) { return "Left"; }

            return "None";
        }

        //範囲選択時内包判定
        public override bool JudgeSelectRange(Point Pt1, Point Pt2)
        {
            int Point_Left;
            int Point_Up;
            int Point_Right;
            int Point_Down;

            //指定された2ポイントを、左上と右下に強制的に変える。
            if (Pt1.X < Pt2.X)
            {
                Point_Left = Pt1.X;
                Point_Right = Pt2.X;
            }
            else
            {
                Point_Left = Pt2.X;
                Point_Right = Pt1.X;
            }

            if (Pt1.Y < Pt2.Y)
            {
                Point_Up = Pt1.Y;
                Point_Down = Pt2.Y;
            }
            else
            {
                Point_Up = Pt2.Y;
                Point_Down = Pt1.Y;
            }
            Point Point_LeftUp = new Point(Point_Left, Point_Up);
            Point Point_RightDown = new Point(Point_Right, Point_Down);


            Point Object_Point_LeftUp = new Point();
            Point Object_Point_RightDown = new Point();
            
            Object_Point_LeftUp = drawLocation;
            Object_Point_RightDown = new Point(drawLocation.X + drawSize.Width, drawLocation.Y + drawSize.Height);
            

            if (Point_LeftUp.X < Object_Point_LeftUp.X && Point_LeftUp.Y < Object_Point_LeftUp.Y &&
                Point_RightDown.X > Object_Point_RightDown.X && Point_RightDown.Y > Object_Point_RightDown.Y)
            {
                return true;
            }

            return false;
        }       

        public override List<Point> ChangePoint(string changePointName, List<Point> originPoints, Point changeDistance)
        {
            ShapeManager shapeManager = ShapeManager.CreateInstance();

            List<Point> newPoints = new List<Point>();

            newPoints.Add(originPoints[0]);
            newPoints.Add(originPoints[1]);

            int changeDistanceX = changeDistance.X;
            int changeDistanceY = changeDistance.Y;

            if (this.ControlInstance.WidthStretch == false) { changeDistanceX = 0; }
            if (this.ControlInstance.HeightStretch == false) { changeDistanceY = 0; }

            switch (changePointName)
            {
                case "Up":
                    newPoints[0] = shapeManager.SnapPoint(new Point(newPoints[0].X, newPoints[0].Y + changeDistanceY), 1);
                    break;

                case "Left":
                    newPoints[0] = shapeManager.SnapPoint(new Point(newPoints[0].X + changeDistanceX, newPoints[0].Y), 1);
                    break;

                case "Down":
                    newPoints[1] = shapeManager.SnapPoint(new Point(newPoints[1].X, newPoints[1].Y + changeDistanceY), 1);
                    break;

                case "Right":
                    newPoints[1] = shapeManager.SnapPoint(new Point(newPoints[1].X + changeDistanceX, newPoints[1].Y), 1);
                    break;

                case "LeftUp":
                    newPoints[0] = shapeManager.SnapPoint(new Point(newPoints[0].X + changeDistanceX, newPoints[0].Y + changeDistanceY), 1);
                    break;

                case "RightDown":
                    newPoints[1] = shapeManager.SnapPoint(new Point(newPoints[1].X + changeDistanceX, newPoints[1].Y + changeDistanceY), 1);
                    break;

                case "LeftDown":

                    newPoints[0] = shapeManager.SnapPoint(new Point(newPoints[0].X + changeDistanceX, newPoints[0].Y), 1);
                    newPoints[1] = shapeManager.SnapPoint(new Point(newPoints[1].X, newPoints[1].Y + changeDistanceY), 1);
                    break;

                case "RightUp":

                    newPoints[0] = shapeManager.SnapPoint(new Point(newPoints[0].X, newPoints[0].Y + changeDistanceY), 1);
                    newPoints[1] = shapeManager.SnapPoint(new Point(newPoints[1].X + changeDistanceX, newPoints[1].Y), 1);
                    break;

            }

            if (((newPoints[1].X - newPoints[0].X) < 1) || ((newPoints[1].Y - newPoints[0].Y) < 1))
            {
                return NowPoints();
            }
            else
            {
                return newPoints;
            }

        }

        public override void Change(List<Point> changePoints)
        {
            Location = changePoints[0];
            int width = changePoints[1].X - changePoints[0].X;
            int height = changePoints[1].Y - changePoints[0].Y;
            Size = new Size(width, height);

        }

        public override object Serialize()
        {
            if (this.Deleted == false)
            {
                //ビューオブジェクト内ユーザーコントロールのインスタンスを取得
                Symbol_Draw Symbol = this.ControlInstance;

                SymbolDataSerializeFormat Sdf = new SymbolDataSerializeFormat();
                Sdf.Type = Path.GetFileNameWithoutExtension(Symbol.GetType().Assembly.ManifestModule.ToString());
                Sdf.SymbolName = Symbol.SymbolName;
                Sdf.PropertyData = MakeSymbolProperty(Symbol);

                return Sdf;
            }
            return null;
        }

        //public override object Clone()
        //{
        //    //System.Diagnostics.Stopwatch sw = new Stopwatch();
        //    //sw.Start();

        //    #region 値型コピー

        //    ViewObject obj = (ViewObject)this.MemberwiseClone();

        //    //Console.WriteLine(sw.Elapsed + " ValueClone");

        //    //obj.Deleted = false;

        //    #endregion

        //    #region イベント登録

        //    //ParentForm.MouseDown += new MouseEventHandler(obj.Form_MouseDown);
        //    //ParentForm.MouseMove += new MouseEventHandler(obj.Form_MouseMove);
        //    //ParentForm.MouseUp   += new MouseEventHandler(obj.Form_MouseUp);

        //    #endregion

        //    #region 参照型コピー
        //    if (this.ControlInstance != null)
        //    {
        //        DeepCopy copyMethod = new DeepCopy(this.ControlInstance);
        //        Symbol_Draw Symbol = (Symbol_Draw)copyMethod.Execute();
        //        //Symbol.SymbolName = ParentForm.GetSymbolName(Symbol);
        //        obj.ControlInstance = Symbol;
        //        //Symbol_Draw baseSymbol = this.ControlInstance;

        //        ////Symbol_Draw Symbol = (Symbol_Draw)baseSymbol.Clone();


        //        ////コントロールのインスタンス生成
        //        //Type SymbolType = baseSymbol.GetType();
        //        //Symbol_Draw Symbol = (Symbol_Draw)Activator.CreateInstance(SymbolType);

        //        ////プロパティコピー
        //        //PropertyDescriptorCollection Properties = TypeDescriptor.GetProperties(Symbol, null, true);

        //        //obj.ControlInstance = Symbol;

        //        //Console.WriteLine(sw.Elapsed + " Nextbefore");

        //        //foreach (PropertyDescriptor Property in Properties)
        //        //{
        //        //    if (Global.GetVisibleAttribute(Symbol, Property))
        //        //    {
        //        //        if (!Property.PropertyType.IsValueType)
        //        //        {
        //        //            DeepCopy copyMethod = new DeepCopy(baseSymbol);
        //        //            object copySymbol = copyMethod.Execute(); 
        //        //        }

        //        //        if (Property.PropertyType.Name == "DeviceFormat")
        //        //        {
        //        //            DeviceFormat baseValue = (DeviceFormat)Property.GetValue(baseSymbol);
        //        //            Property.SetValue(Symbol, baseValue.Clone());
        //        //        }
        //        //        else if (Property.PropertyType.Name == "SpanFormat")
        //        //        {
        //        //            SpanFormat baseValue = (SpanFormat)Property.GetValue(baseSymbol);
        //        //            Property.SetValue(Symbol, baseValue.Clone());
        //        //        }
        //        //        else if (Property.Name == "SymbolName")
        //        //        {
        //        //            Symbol.SymbolName = ParentForm.GetSymbolName(Symbol);
        //        //        }
        //        //        else
        //        //        {
        //        //            object baseValue = Property.GetValue(baseSymbol);
        //        //            Property.SetValue(Symbol, baseValue);
        //        //        }
        //        //    }
        //        //}

        //        Symbol.Tag = obj;

        //        obj.ParentForm = ParentForm;
        //    }
        //    #endregion

        //    //Console.WriteLine(sw.Elapsed + " AllClone");

        //    return obj;
        //}


        public override List<Point> NowPoints()
        {
            List<Point> points = new List<Point>();

            points.Add(Location);
            points.Add(new Point(Location.X + Size.Width, Location.Y + Size.Height));
            return points;
        }

        public override void Move(List<Point> movePoints)
        {
            Location = new Point(movePoints[0].X, movePoints[0].Y);
            Size = new Size(movePoints[1].X - movePoints[0].X, movePoints[1].Y - movePoints[0].Y);

        }

    }

    public class DeepCopy
    {
        private object copyObject = null;
        private object baseObject = null;

        public DeepCopy(object baseObject)
        {
            this.baseObject = baseObject;
            this.copyObject = Activator.CreateInstance(baseObject.GetType());
        }

        public object Execute()
        {
            //PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(baseObject, null, true);
            PropertyInfo[] properties = baseObject.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (Global.GetVisibleAttribute(baseObject, property))
                {
                    ShallowCopy(property, baseObject, copyObject);
                }
            }

            return copyObject;
        }

        public void Execute(object baseObject,object copyObject)
        {            

            PropertyInfo[] properties = baseObject.GetType().GetProperties();
           
            foreach (PropertyInfo property in properties)
            {
                ShallowCopy(property, baseObject, copyObject);
                
            }            

        }

        public void Execute(Array baseObjects, Array copyObjects)
        {            
            copyObjects = Array.CreateInstance(baseObjects.GetType().GetElementType(), baseObjects.Length);

            for (int i = 0; i < baseObjects.Length; i ++ )
            {
                object baseObject = baseObjects.GetValue(i);
                object copyObject = copyObjects.GetValue(i);

                if (baseObject.GetType().IsValueType)
                {
                    copyObjects.SetValue(baseObject, i);
                }
                else
                {
                    PropertyInfo[] properties = baseObject.GetType().GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        ShallowCopy(property, baseObject, copyObject);
                    }
                }
            }

        }

        private void ShallowCopy(PropertyInfo property, object baseObject, object copyObject)
        {
            if (property.CanRead && property.CanWrite)
            {
                if (property.PropertyType.IsArray)
                {
                    Array baseData = (Array)property.GetValue(baseObject, null);
                    Array copyData = (Array)property.GetValue(copyObject, null);
                    Execute(baseData, copyData);
                }
                else if (!property.PropertyType.IsValueType && property.PropertyType != typeof(String) && property.PropertyType != typeof(Font))
                {                    
                    object baseData = property.GetValue(baseObject, null);
                    object copyData = property.GetValue(copyObject, null);
                    Execute(baseData, copyData);                    
                }
                else
                {
                    if (property.GetIndexParameters().Length == 0)
                    {
                        object baseData = property.GetValue(baseObject, null);
                        property.SetValue(copyObject, baseData, null);
                    }
                }

               
            }
        }

    }


}
