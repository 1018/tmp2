using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using CommonClassLibrary;
using System.Drawing.Design;

namespace LocalSimulator.ProjectMaker
{
    public class LinesObject : ShapeObject
    {   
        private MyList<Point> _Points = new MyList<Point>();
        private int           _BorderWidth = 1;
        private Color         _BorderColor = Color.Black;
        private DashStyle     _BorderStyle = DashStyle.Solid;
        private int           _LineWidth = 1;
        private Color         _LineColor = Color.Black;
        private DashStyle     _LineStyle = DashStyle.Solid;
        
        private List<Point>   drawPoints = new List<Point>();
        private int           drawBorderWidth = 1;
        private int           drawLineWidth = 1;

        private IDrawing      drawing = null;

        [Editor(typeof(MyListEditor), typeof(UITypeEditor))]
        [Visible(true), DisplayName("多点データ"), Category("90 配置")]
        public MyList<Point> Points
        {
            get { return _Points; }
            set
            {
                _Points = value;                
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

        
        [Visible(true), DisplayName("線サイズ"), Category("01 表示")]
        public int LineWidth
        {
            get { return _LineWidth; }
            set
            {
                _LineWidth = value;
            }
        }
                
        [Visible(true), DisplayName("線色"), Category("01 表示")]
        public Color LineColor
        {
            get { return _LineColor; }
            set
            {
                _LineColor = value;
            }
        }

        [Visible(true), DisplayName("線スタイル"), Category("01 表示")]
        public DashStyle LineStyle
        {
            get { return _LineStyle; }
            set
            {
                _LineStyle = value;
            }
        }

        public override void Redraw(PaintEventArgs e)
        {
            if (Deleted) { return; }

            drawing.Draw(e.Graphics);

        }

        public override void ReMake()
        {
            drawBorderWidth = (int)((double)BorderWidth * ParentForm.Zoom);
            drawLineWidth   = (int)((double)LineWidth * ParentForm.Zoom);
            drawPoints = new List<Point>();
            for (int i = 0; i < Points.Count; i++)
            {
                drawPoints.Add(ZoomManager.MagnifyPoint(Points[i], ParentForm.Zoom));
            }
            drawing = new LinesDrawing(this.BorderColor, drawBorderWidth, this.BorderStyle,
                                      this.LineColor, drawLineWidth, this.LineStyle, drawPoints);
        }            

        public override void DrawSelect(PaintEventArgs e)
        {
            if (Points.Count != drawPoints.Count) { return; }
            for (int i = 0; i < Points.Count; i++)
            {
                DrawSelectCircle(e.Graphics, drawPoints[i]);
            }            
        }

        public override List<Point> ChangePoint(string changePointName, List<Point> originPoints, Point changeDistance)
        {
            ShapeManager shapeManager = ShapeManager.CreateInstance();
            int index = Convert.ToInt32(changePointName);
            List<Point> newPoints = new List<Point>();

            for (int i = 0; i < originPoints.Count; i++)
            {
                if (i == index)
                {
                    newPoints.Add(shapeManager.SnapPoint(new Point(originPoints[i].X + changeDistance.X, originPoints[i].Y + changeDistance.Y),1));
                }
                else
                {
                    newPoints.Add(originPoints[i]);
                }
            }

            return newPoints;

        }

        public override void Change(List<Point> changePoints)
        {
            Points = new MyList<Point>();

            for (int i = 0; i < changePoints.Count; i++)
            {
                Points.Add(changePoints[i]);
            }

        }
                
        public override void Initialize(List<Point> settingPoints)
        {
            Points = new MyList<Point>();

            for (int i = 0; i < settingPoints.Count; i++)
            {
                Points.Add(ZoomManager.ToBasePoint(settingPoints[i], ParentForm.Zoom));
            }

            ReMake();  
        }

        public override Cursor GetModifyCursor(MouseEventArgs e)
        {
            string JudgeModify = JudgeModifyRange(e.Location);

            if (Selected && JudgeModify != "None")
            {
                int ChangePoint = int.Parse(JudgeModify);
                Point JudgePoint = new Point(0, 0);
                //始点
                if (ChangePoint == 0)
                {
                    //基準点 = 2番目のポイント
                    JudgePoint.X = Points[1].X;
                    JudgePoint.Y = Points[1].Y;
                }
                //終点
                else if (ChangePoint == Points.Count - 1)
                {
                    //基準点 = 最後から2番目のポイント
                    JudgePoint.X = Points[Points.Count - 2].X;
                    JudgePoint.Y = Points[Points.Count - 2].Y;
                }
                //その他
                else
                {
                    //基準点 = 前後のポイントの中間点
                    JudgePoint.X = (Points[ChangePoint - 1].X + Points[ChangePoint + 1].X) / 2;
                    JudgePoint.Y = (Points[ChangePoint - 1].Y + Points[ChangePoint + 1].Y) / 2;
                }

                //基準点より左上か右下
                if ((Points[ChangePoint].X < JudgePoint.X && Points[ChangePoint].Y < JudgePoint.Y) ||
                    (Points[ChangePoint].X > JudgePoint.X && Points[ChangePoint].Y > JudgePoint.Y))
                {
                    return Cursors.SizeNWSE;
                }
                //基準点より左下か右上
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
                 
            for (int i = 0; i < drawPoints.Count - 1; i++)
            {
                Point B_SelectPoint1 = new Point();
                Point B_SelectPoint2 = new Point();
                Point B_SelectPoint3 = new Point();
                Point B_SelectPoint4 = new Point();

                double B_dx = drawPoints[i + 1].X - drawPoints[i].X;
                double B_dy = drawPoints[i + 1].Y - drawPoints[i].Y;

                //90度－二点角度
                double B_Rad = (Math.PI / 2) - Math.Atan2(B_dy, B_dx);

                double B_PosCos = Math.Cos(B_Rad) * (3 + drawBorderWidth / 2);
                double B_PosSin = Math.Sin(B_Rad) * (3 + drawBorderWidth / 2);

                B_SelectPoint1.X = drawPoints[i].X + (int)(B_PosCos - B_PosSin);
                B_SelectPoint1.Y = drawPoints[i].Y - (int)(B_PosCos + B_PosSin);
                B_SelectPoint2.X = drawPoints[i].X - (int)(B_PosCos + B_PosSin);
                B_SelectPoint2.Y = drawPoints[i].Y - (int)(B_PosCos - B_PosSin);

                B_SelectPoint3.X = drawPoints[i + 1].X - (int)(B_PosCos - B_PosSin);
                B_SelectPoint3.Y = drawPoints[i + 1].Y + (int)(B_PosCos + B_PosSin);
                B_SelectPoint4.X = drawPoints[i + 1].X + (int)(B_PosCos + B_PosSin);
                B_SelectPoint4.Y = drawPoints[i + 1].Y + (int)(B_PosCos - B_PosSin);

                //中央点算出
                Point B_LCenterPoint = GetCenterPoint(B_SelectPoint1, B_SelectPoint2, B_SelectPoint3, B_SelectPoint4);

                //中央点－現在マウスポインタを線分とし、選択枠の四辺との交差を調べる
                bool B_LCrossJudge1 = GetCrossJudge(B_LCenterPoint, NowPt, B_SelectPoint1, B_SelectPoint2);
                bool B_LCrossJudge2 = GetCrossJudge(B_LCenterPoint, NowPt, B_SelectPoint2, B_SelectPoint3);
                bool B_LCrossJudge3 = GetCrossJudge(B_LCenterPoint, NowPt, B_SelectPoint3, B_SelectPoint4);
                bool B_LCrossJudge4 = GetCrossJudge(B_LCenterPoint, NowPt, B_SelectPoint4, B_SelectPoint1);

                if (!B_LCrossJudge1 && !B_LCrossJudge2 && !B_LCrossJudge3 && !B_LCrossJudge4) { return true; }
            }

            return false;


        }   

        public override string JudgeModifyRange(Point NowPt)
        {
            if (Deleted) { return "None"; }

            for (int i = 0; i < drawPoints.Count - 1; i++)
            {
                double dx = drawPoints[i + 1].X - drawPoints[i].X;
                double dy = drawPoints[i + 1].Y - drawPoints[i].Y;

                //90度－二点角度
                double Rad = (Math.PI / 2) - Math.Atan2(dy, dx);

                bool StartPt = JudgeModifyPoint(NowPt, drawPoints[i], Rad);
                bool EndPt = JudgeModifyPoint(NowPt, drawPoints[i + 1], Rad);

                if (StartPt) { return i.ToString(); }
                if (EndPt) { return (i + 1).ToString(); }
            }

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
            
            //全ポイントから最小、最大を割り出す
            Object_Point_LeftUp = drawPoints[0];
            Object_Point_RightDown = drawPoints[0];
            for (int i = 0; i < drawPoints.Count; i++)
            {
                if (Object_Point_LeftUp.X > drawPoints[i].X)
                {
                    Object_Point_LeftUp.X = drawPoints[i].X;
                }
                if (Object_Point_LeftUp.Y > drawPoints[i].Y)
                {
                    Object_Point_LeftUp.Y = drawPoints[i].Y;
                }
                if (Object_Point_RightDown.X < drawPoints[i].X)
                {
                    Object_Point_RightDown.X = drawPoints[i].X;
                }
                if (Object_Point_RightDown.Y < drawPoints[i].Y)
                {
                    Object_Point_RightDown.Y = drawPoints[i].Y;
                }
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
            return Points.Items;
        }

        public override void Move(List<Point> movePoints)
        {
            Points = new MyList<Point>();
            foreach (Point movePoint in movePoints)
            {
                Points.Add(movePoint);
            }
        }


    }
    
}
