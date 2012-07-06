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
    public class PolygonObject : ShapeObject
    {        
        private List<Point>     drawPoints = new List<Point>();
        private int             drawBorderWidth = 1;
        private MyList<Point>   _Points = new MyList<Point>();
        private int             _BorderWidth = 1;
        private Color           _BorderColor = Color.Black;
        private DashStyle       _BorderStyle = DashStyle.Solid;       
        private Color           _FillColor;

        private IDrawing        drawing;
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

        
        [Visible(true), DisplayName("塗りつぶし色"), Category("01 表示")]
        public Color FillColor
        {
            get { return _FillColor; }
            set
            {
                _FillColor = value;
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
            drawPoints = new List<Point>();
            for (int i = 0; i < Points.Count; i++)
            {
                drawPoints.Add(ZoomManager.MagnifyPoint(Points[i], ParentForm.Zoom));
            }
            drawing = new PolygonDrawing(this.BorderColor, this.drawBorderWidth, this.BorderStyle, this.FillColor, drawPoints);
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
                    newPoints.Add(shapeManager.SnapPoint(new Point(originPoints[i].X + changeDistance.X, originPoints[i].Y + changeDistance.Y), 1));
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

            return IsPointInPolygon(NowPt, drawPoints.Count, drawPoints.ToArray());

        }
        /// <summary>
        /// 点の多角形に対する内外判定
        /// </summary>
        /// <param name="pointTarget"></param>
        /// <param name="uiCountPoint"></param>
        /// <param name="aPoint"></param>
        /// <returns></returns>
        private bool IsPointInPolygon(Point pointTarget, int uiCountPoint, Point[] aPoint)
        {
            int iCountCrossing = 0;

            Point point0 = aPoint[0];
            bool bFlag0x = (pointTarget.X <= point0.X);
            bool bFlag0y = (pointTarget.Y <= point0.Y);

            // レイの方向は、Ｘプラス方向
            for (int ui = 1; ui < uiCountPoint + 1; ui++)
            {
                Point point1 = aPoint[ui % uiCountPoint];	// 最後は始点が入る（多角形データの始点と終点が一致していないデータ対応）
                bool bFlag1x = (pointTarget.X <= point1.X);
                bool bFlag1y = (pointTarget.Y <= point1.Y);
                if (bFlag0y != bFlag1y)
                {	// 線分はレイを横切る可能性あり。
                    if (bFlag0x == bFlag1x)
                    {	// 線分の２端点は対象点に対して両方右か両方左にある
                        if (bFlag0x)
                        {	// 完全に右。⇒線分はレイを横切る
                            iCountCrossing += (bFlag0y ? -1 : 1);	// 上から下にレイを横切るときには、交差回数を１引く、下から上は１足す。
                        }
                    }
                    else
                    {	// レイと交差するかどうか、対象点と同じ高さで、対象点の右で交差するか、左で交差するかを求める。
                        if (pointTarget.X <= (point0.X + (point1.X - point0.X) * (pointTarget.Y - point0.Y) / (point1.Y - point0.Y)))
                        {	// 線分は、対象点と同じ高さで、対象点の右で交差する。⇒線分はレイを横切る
                            iCountCrossing += (bFlag0y ? -1 : 1);	// 上から下にレイを横切るときには、交差回数を１引く、下から上は１足す。
                        }
                    }
                }
                // 次の判定のために、
                point0 = point1;
                bFlag0x = bFlag1x;
                bFlag0y = bFlag1y;
            }

            // クロスカウントがゼロのとき外、ゼロ以外のとき内。
            return (0 != iCountCrossing);
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
