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
    public class LineObject : ShapeObject
    {  
        
        private Point       drawStartPoint;
        private Point       drawEndPoint;
        private int         drawLineWidth = 1;
        private int         drawBorderWidth = 1; 

        private Point       _StartPoint;
        private Point       _EndPoint;
        private int         _LineWidth = 1;
        private Color       _LineColor = Color.Black;
        private DashStyle   _LineStyle = DashStyle.Solid;
        private int         _BorderWidth = 1;
        private Color       _BorderColor = Color.Black;
        private DashStyle   _BorderStyle = DashStyle.Solid;

        private IDrawing    drawing = null;

        #region プロパティ

        
        [Visible(true), DisplayName("開始点"), Category("90 配置")]
        public Point StartPoint
        {
            get { return _StartPoint; }
            set
            {
                _StartPoint = value;
                //ReMake();
                //if (base.ModeCreate || base.ModeModify || base.ModeMove) { return; }
                //ParentForm.Refresh();
            }
        }
        
        [Visible(true), DisplayName("終了点"), Category("90 配置")]
        public Point EndPoint
        {
            get { return _EndPoint; }
            set
            {
                _EndPoint = value;
                //ReMake();
                //if (base.ModeCreate || base.ModeModify || base.ModeMove) { return; }
                //ParentForm.Refresh();
            }
        }

        
        [Visible(true), DisplayName("線サイズ"), Category("01 表示")]
        public int LineWidth
        {
            get { return _LineWidth; }
            set
            {
                _LineWidth = value;
                //ReMake();
                //ParentForm.Refresh();
            }
        }

        
        [Visible(true), DisplayName("線色"), Category("01 表示")]
        public Color LineColor
        {
            get { return _LineColor; }
            set
            {
                _LineColor = value;
                //ReMake();
                //ParentForm.Refresh();
            }
        }

        
        [Visible(true), DisplayName("線スタイル"), Category("01 表示")]
        public DashStyle LineStyle
        {
            get { return _LineStyle; }
            set
            {
                _LineStyle = value;
                //ReMake();
                //ParentForm.Refresh();
            }
        }

        [Visible(true), DisplayName("境界線サイズ"), Category("01 表示")]
        public int BorderWidth
        {
            get { return _BorderWidth; }
            set
            {
                _BorderWidth = value;
                //ReMake();
                //ParentForm.Refresh();
            }
        }

        [Visible(true), DisplayName("境界線色"), Category("01 表示")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set
            {
                _BorderColor = value;
                //ReMake();
                //ParentForm.Refresh();
            }
        }

        [Visible(true), DisplayName("境界線スタイル"), Category("01 表示")]
        public DashStyle BorderStyle
        {
            get { return _BorderStyle; }
            set
            {
                _BorderStyle = value;
                //ReMake();
                //ParentForm.Refresh();
            }
        }

        #endregion

        public override void Redraw(PaintEventArgs e)
        {
            if (Deleted) { return; }

            drawing.Draw(e.Graphics);
           
        }

        public override void ReMake()
        {
            List<Point> drawPoints = new List<Point>();
            drawBorderWidth = (int)(this.BorderWidth * ParentForm.Zoom);
            drawLineWidth = (int)(this.LineWidth * ParentForm.Zoom);
            drawStartPoint = ZoomManager.MagnifyPoint(StartPoint, ParentForm.Zoom);
            drawEndPoint = ZoomManager.MagnifyPoint(EndPoint, ParentForm.Zoom);

            drawPoints.Add(drawStartPoint);
            drawPoints.Add(drawEndPoint);

            drawing = new LineDrawing(this.BorderColor, drawBorderWidth, this.BorderStyle,
                                      this.LineColor, drawLineWidth, this.LineStyle, drawPoints);
        }

        public override void DrawSelect(PaintEventArgs e)
        {            
            DrawSelectCircle(e.Graphics, ZoomManager.MagnifyPoint(StartPoint, ParentForm.Zoom));
            DrawSelectCircle(e.Graphics, ZoomManager.MagnifyPoint(EndPoint, ParentForm.Zoom));           
        }

        public override List<Point> ChangePoint(string changePointName, List<Point> originPoints, Point changeDistance)
        {
            ShapeManager shapeManager = ShapeManager.CreateInstance();

            Point newStartPoint = originPoints[0];
            Point newEndPoint = originPoints[1];

            if (changePointName == "StartPoint")
            {
                newStartPoint = shapeManager.SnapPoint(new Point(originPoints[0].X + changeDistance.X, originPoints[0].Y + changeDistance.Y),1);
            }
            else
            {
                newEndPoint = shapeManager.SnapPoint(new Point(originPoints[1].X + changeDistance.X, originPoints[1].Y + changeDistance.Y),1);

            }            

            List<Point> newPoints = new List<Point>();
            newPoints.Add(newStartPoint);
            newPoints.Add(newEndPoint);

            return newPoints;
            
        }

        public override void Change(List<Point> changePoints)
        {
            StartPoint = changePoints[0];
            EndPoint = changePoints[1];


        }

        public override void Initialize(List<Point> settingPoints)
        {           
            this.StartPoint = ZoomManager.ToBasePoint(settingPoints[0], ParentForm.Zoom);
            this.EndPoint = ZoomManager.ToBasePoint(settingPoints[1], ParentForm.Zoom);
        }

        public override Cursor GetModifyCursor(MouseEventArgs e)
        {
            
            string JudgeModify = this.JudgeModifyRange(e.Location);

            if (this.Selected && JudgeModify != "None")
            {
                if ((StartPoint.X < EndPoint.X && StartPoint.Y < EndPoint.Y) ||
                    (StartPoint.X > EndPoint.X && StartPoint.Y > EndPoint.Y))
                {
                    return Cursors.SizeNWSE;
                }
                else
                {
                    return Cursors.SizeNESW;
                }
            }

            return ParentForm.Cursor;
        }

        public override bool JudgeMoveRange(Point NowPt)
        {
            if (Deleted) { return false; }           
                            
            Point SelectPoint1 = new Point();
            Point SelectPoint2 = new Point();
            Point SelectPoint3 = new Point();
            Point SelectPoint4 = new Point();

            double dx = drawEndPoint.X - drawStartPoint.X;
            double dy = drawEndPoint.Y - drawStartPoint.Y;

            //90度－二点角度
            double Rad = (Math.PI / 2) - Math.Atan2(dy, dx);

            double PosCos = Math.Cos(Rad) * (3 + drawLineWidth / 2);
            double PosSin = Math.Sin(Rad) * (3 + drawLineWidth / 2);


            SelectPoint1.X = drawStartPoint.X + (int)(PosCos - PosSin);
            SelectPoint1.Y = drawStartPoint.Y - (int)(PosCos + PosSin);
            SelectPoint2.X = drawStartPoint.X - (int)(PosCos + PosSin);
            SelectPoint2.Y = drawStartPoint.Y - (int)(PosCos - PosSin);

            SelectPoint3.X = drawEndPoint.X - (int)(PosCos - PosSin);
            SelectPoint3.Y = drawEndPoint.Y + (int)(PosCos + PosSin);
            SelectPoint4.X = drawEndPoint.X + (int)(PosCos + PosSin);
            SelectPoint4.Y = drawEndPoint.Y + (int)(PosCos - PosSin);

            //中央点算出
            Point LCenterPoint = GetCenterPoint(SelectPoint1, SelectPoint2, SelectPoint3, SelectPoint4);

            //中央点－現在マウスポインタを線分とし、選択枠の四辺との交差を調べる
            bool LCrossJudge1 = GetCrossJudge(LCenterPoint, NowPt, SelectPoint1, SelectPoint2);
            bool LCrossJudge2 = GetCrossJudge(LCenterPoint, NowPt, SelectPoint2, SelectPoint3);
            bool LCrossJudge3 = GetCrossJudge(LCenterPoint, NowPt, SelectPoint3, SelectPoint4);
            bool LCrossJudge4 = GetCrossJudge(LCenterPoint, NowPt, SelectPoint4, SelectPoint1);

            if ( !LCrossJudge1  && !LCrossJudge2 && !LCrossJudge3 && !LCrossJudge4 ){ return true; }

            return false;             
                    
            }

        public override string JudgeModifyRange(Point NowPt)
        {
            if (Deleted) { return "None"; }

            double dx = drawEndPoint.X - drawStartPoint.X;
            double dy = drawEndPoint.Y - drawStartPoint.Y;

            //90度－二点角度
            double Rad = (Math.PI / 2) - Math.Atan2(dy, dx);

            bool StartPt = JudgeModifyPoint(NowPt, drawStartPoint, Rad);
            bool EndPt = JudgeModifyPoint(NowPt, drawEndPoint, Rad);

            if (StartPt) { return "StartPoint"; }
            if (EndPt) { return "EndPoint"; }

            return "None";
        }
        
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
            

            if (drawStartPoint.X < drawEndPoint.X)
            {
                Object_Point_LeftUp.X = drawStartPoint.X;
                Object_Point_RightDown.X = drawEndPoint.X;
            }
            else
            {
                Object_Point_LeftUp.X = drawEndPoint.X;
                Object_Point_RightDown.X = drawStartPoint.X;
            }

            if (StartPoint.Y < EndPoint.Y)
            {
                Object_Point_LeftUp.Y = drawStartPoint.Y;
                Object_Point_RightDown.Y = drawEndPoint.Y;
            }
            else
            {
                Object_Point_LeftUp.Y = drawEndPoint.Y;
                Object_Point_RightDown.Y = drawStartPoint.Y;
            }        

            if (Point_LeftUp.X < Object_Point_LeftUp.X && Point_LeftUp.Y < Object_Point_LeftUp.Y &&
                Point_RightDown.X > Object_Point_RightDown.X && Point_RightDown.Y > Object_Point_RightDown.Y)
            {
                return true;
            }

            return false;
        }

        public override List<Point> NowPoints()
        {
            List<Point> points = new List<Point>();
            points.Add(StartPoint);
            points.Add(EndPoint);
            return points;            
        }

        public override void Move(List<Point> movePoints)
        {
            StartPoint = new Point(movePoints[0].X, movePoints[0].Y);
            EndPoint = new Point(movePoints[1].X, movePoints[1].Y);

        }
    }
    
}
