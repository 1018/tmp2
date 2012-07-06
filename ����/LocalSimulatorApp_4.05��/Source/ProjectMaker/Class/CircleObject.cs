using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using CommonClassLibrary;

namespace LocalSimulator.ProjectMaker
{
    public class CircleObject : ShapeObject
    {
        private Point   drawLocation;
        private Size    drawSize;
        private int     drawBorderWidth;

        private Point       _Location;
        private Size        _Size;
        private int         _BorderWidth = 1;
        private Color       _BorderColor = Color.Black;
        private DashStyle   _BorderStyle = DashStyle.Solid;
        private Color       _FillColor;

        private IDrawing    drawing;

        [Visible(true), DisplayName("位置"), Category("90 配置")]
        public virtual Point Location
        {
            get { return _Location; }
            set
            {
                _Location = value;
            }
        }

        [Visible(true), DisplayName("サイズ"), Category("90 配置")]
        public virtual Size Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
            }
        }

        [Visible(true), DisplayName("境界線サイズ"), Category("01 表示")]
        public int BorderWidth
        {
            get { return _BorderWidth; }
            set
            {
                _BorderWidth = value;
            }
        }

        [Visible(true), DisplayName("境界線色"), Category("01 表示")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set
            {
                _BorderColor = value;
            }
        }

        [Visible(true), DisplayName("境界線スタイル"), Category("01 表示")]
        public DashStyle BorderStyle
        {
            get { return _BorderStyle; }
            set
            {
                _BorderStyle = value;
            }
        }

        [Visible(true), DisplayName("塗りつぶし色"), Category("01 表示")]
        public Color FillColor
        {
            get { return _FillColor; }
            set
            {
                _FillColor = value;
            }
        }
        
        public override void DrawSelect(PaintEventArgs e)
        {
            
                DrawSelectCircle(e.Graphics, drawLocation);
                DrawSelectCircle(e.Graphics, new Point(drawLocation.X + drawSize.Width / 2, drawLocation.Y));
                DrawSelectCircle(e.Graphics, new Point(drawLocation.X + drawSize.Width, drawLocation.Y));
                DrawSelectCircle(e.Graphics, new Point(drawLocation.X + drawSize.Width, drawLocation.Y + drawSize.Height / 2));
                DrawSelectCircle(e.Graphics, new Point(drawLocation.X + drawSize.Width, drawLocation.Y + drawSize.Height));
                DrawSelectCircle(e.Graphics, new Point(drawLocation.X + drawSize.Width / 2, drawLocation.Y + drawSize.Height));
                DrawSelectCircle(e.Graphics, new Point(drawLocation.X, drawLocation.Y + drawSize.Height));
                DrawSelectCircle(e.Graphics, new Point(drawLocation.X, drawLocation.Y + drawSize.Height / 2));
            
        }

        public override void Redraw(PaintEventArgs e)
        {
            drawing.Draw(e.Graphics);
        }

        public override void ReMake()
        {
            drawLocation = ZoomManager.MagnifyPoint(Location, ParentForm.Zoom);
            drawSize = ZoomManager.MagnifySize(Size, ParentForm.Zoom);
            drawBorderWidth = (int)((double)BorderWidth * ParentForm.Zoom);

            List<Point> drawPoints = new List<Point>();
            drawPoints.Add(drawLocation);
            drawPoints.Add(new Point(drawLocation.X + drawSize.Width, drawLocation.Y + drawSize.Height));

            drawing = new CircleDrawing(this.BorderColor, this.drawBorderWidth, this.BorderStyle, this.FillColor, drawPoints, false);


        }
                         
        public override bool JudgeMoveRange(Point NowPt)
        {
            if (Deleted) { return false; }

            //楕円内包判定
            //
            //  x^2     y^2
            // ----- + ----- <= 1 が真で、内包されている。
            //  a^2     b^2
            //
            // a = 横軸の長さ/2　b = 縦軸の長さ/2
            // x,y は楕円の中心からの点座標とする。

            double CenterX = drawLocation.X + drawSize.Width / 2;
            double CenterY = drawLocation.Y + drawSize.Height / 2;
            double PtX = NowPt.X - CenterX;
            double PtY = NowPt.Y - CenterY;

            double X2 = Math.Pow(PtX, 2);
            double Y2 = Math.Pow(PtY, 2);
            double a2 = Math.Pow(drawSize.Width / 2 + (3 + drawBorderWidth / 2), 2);
            double b2 = Math.Pow(drawSize.Height / 2 + (3 + drawBorderWidth / 2), 2);

            double Ans = X2 / a2 + Y2 / b2;

            if (Ans <= 1) { return true; }

            return false; 


        }   
        
        public override void Initialize(List<Point> settingPoints)
        {
            int width = 0;
            int height = 0;
            int startX = 0;
            int startY = 0;

            if (settingPoints[0].X < settingPoints[1].X)
            {
                startX = settingPoints[0].X;
                width = settingPoints[1].X - settingPoints[0].X;
            }
            else
            {
                startX = settingPoints[1].X;
                width = settingPoints[0].X - settingPoints[1].X;
            }

            if (settingPoints[0].Y < settingPoints[1].Y)
            {
                startY = settingPoints[0].Y;
                height = settingPoints[1].Y - settingPoints[0].Y;
            }
            else
            {
                startY = settingPoints[1].Y;
                height = settingPoints[0].Y - settingPoints[1].Y;
            }

            Point newLocation = ZoomManager.ToBasePoint(new Point(startX, startY),ParentForm.Zoom);
            Size newSize = ZoomManager.ToBaseSize(new Size(width, height),ParentForm.Zoom);
            
            this.Location = newLocation;
            this.Size = newSize;            
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
                    return Cursors.SizeNS;
                }
                //左 & 右 
                else if (JudgeModify == "Left" || JudgeModify == "Right")
                {
                    return Cursors.SizeWE;
                }

            }

            return ParentForm.Cursor;
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

        //public override void Clipboard_Cut()
        //{
        //    //切り取り処理
        //    if (Selected)
        //    {
        //        CopyPoint = Location;
        //        if (Location.X < MainCtrl.ClipBoardObjectBaseX || MainCtrl.ClipBoardObjectBaseX == 0)
        //        {
        //            MainCtrl.ClipBoardObjectBaseX = Location.X;
        //        }

        //        if (Location.Y < MainCtrl.ClipBoardObjectBaseY || MainCtrl.ClipBoardObjectBaseY == 0)
        //        {
        //            MainCtrl.ClipBoardObjectBaseY = Location.Y;
        //        }

        //        MainCtrl.ClipBoardObject.Add(this);
        //        this.Deleted = true;
        //        this.Selected = false;
               
        //        BaseFormIoList.RemoveSymbol(ParentForm.Number, this);
        //    }
        
        //}
        //public override void Clipboard_Copy()
        //{
        //    if (Selected)
        //    {                
        //        CopyPoint = Location;

        //        if (Location.X < MainCtrl.ClipBoardObjectBaseX || MainCtrl.ClipBoardObjectBaseX == 0)
        //        {
        //            MainCtrl.ClipBoardObjectBaseX = Location.X;
        //        }

        //        if (Location.Y < MainCtrl.ClipBoardObjectBaseY || MainCtrl.ClipBoardObjectBaseY == 0)
        //        {
        //            MainCtrl.ClipBoardObjectBaseY = Location.Y;
        //        }

        //        MainCtrl.ClipBoardObject.Add(this);
        //    }
        //}
        //public override void ClipBoard_Paste()
        //{
        //    //コントロールコピー
        //    CircleObject obj = (CircleObject)this.Clone();
            
        //    //差分算出
        //    int PasteX = MainCtrl.ShowContextMenuPoint.X + CopyPoint.X - MainCtrl.ClipBoardObjectBaseX;
        //    int PasteY = MainCtrl.ShowContextMenuPoint.Y + CopyPoint.Y - MainCtrl.ClipBoardObjectBaseY;

        //    obj.Location = new Point(PasteX, PasteY);

        //    obj.Selected = true;
        //    ParentForm.ShapeCollection_Add(obj);
        //    MainCtrl.PropertyView_Set(obj);
        //}

        public override List<Point> ChangePoint(string changePointName, List<Point> originPoints, Point changeDistance)
        {
            ShapeManager shapeManager = ShapeManager.CreateInstance();

            List<Point> newPoints = new List<Point>();
            
            newPoints.Add(originPoints[0]);
            newPoints.Add(originPoints[1]);

            switch (changePointName)
            {
                case "Up":               

                    newPoints[0] = shapeManager.SnapPoint(new Point(newPoints[0].X, newPoints[0].Y + changeDistance.Y), 1);
                    break;

                case "Left":

                    newPoints[0] = shapeManager.SnapPoint(new Point(newPoints[0].X + changeDistance.X, newPoints[0].Y), 1);
                    break;

                case "Down":
                    newPoints[1] = shapeManager.SnapPoint(new Point(newPoints[1].X, newPoints[1].Y + changeDistance.Y), 1);
                    break;

                case "Right":

                    newPoints[1] = shapeManager.SnapPoint(new Point(newPoints[1].X + changeDistance.X, newPoints[1].Y), 1);
                    break;

                case "LeftUp":

                    newPoints[0] = shapeManager.SnapPoint(new Point(newPoints[0].X + changeDistance.X, newPoints[0].Y + changeDistance.Y), 1);
                    break;

                case "RightDown":

                    newPoints[1] = shapeManager.SnapPoint(new Point(newPoints[1].X + changeDistance.X, newPoints[1].Y + changeDistance.Y), 1);
                    break;

                case "LeftDown":

                    newPoints[0] = shapeManager.SnapPoint(new Point(newPoints[0].X + changeDistance.X, newPoints[0].Y), 1);
                    newPoints[1] = shapeManager.SnapPoint(new Point(newPoints[1].X, newPoints[1].Y + changeDistance.Y), 1);
                    break;

                case "RightUp":

                    newPoints[0] = shapeManager.SnapPoint(new Point(newPoints[0].X, newPoints[0].Y + changeDistance.Y), 1);
                    newPoints[1] = shapeManager.SnapPoint(new Point(newPoints[1].X + changeDistance.X, newPoints[1].Y), 1);
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
        }
    }


}
