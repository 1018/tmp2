using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using CommonClassLibrary;
using System.Reflection;


namespace LocalSimulator.ProjectMaker
{        
    [TypeConverter(typeof(PropertyConverter)),Serializable]
    public class ShapeObject : IShapeObject//ICloneable
    {
        protected bool ShapeObjectChangeMode = false;

        protected Point MoveStart_MousePoint;

        #region プロパティ

        public bool ModeCreate { get; set; }
        
        public bool ModeMove   { get; set; }
        
        public bool ModeModify { get; set; }
        
        public Point CopyPoint { get; set; }        
        
        [Visible(true), DisplayName("タイプ"), Category("00 種別")]
        public string ShapeType
        {
            get
            {
                return this.GetType().Name;
            }
        }       

        private bool _Selected;

        [Browsable(false)]
        public virtual bool Selected
        {
            get { return _Selected; }

            set
            {
                _Selected = value;
            }
        }

        private bool _Deleted;
        [Browsable(false)]
        public bool Deleted
        {
            get { return _Deleted; }

            set
            {
                _Deleted = value;
            }
        }


        [Browsable(false)]
        public BaseForm ParentForm { get; set; }

        #endregion

        //コンストラクタ
        public ShapeObject() { }
        
        #region 描画処理
       
        protected virtual void DrawFill(SolidBrush MyBrush,PaintEventArgs e){ }

        protected virtual void DrawBorder(Pen MyPen, PaintEventArgs e) { }

        public virtual void DrawSelect(PaintEventArgs e) { }

        public virtual void Redraw(PaintEventArgs e) { }

        public virtual void ReMake() { }
        

        #endregion

        #region 判定関連

        //移動選択範囲判定        
        public virtual bool JudgeMoveRange(Point NowPt) { return false; }
        //{
        //    if (Deleted) { return false; }

        //    switch (Type)
        //    {
        //        case ShapeTypeFormat.DrawType.Circle:

        //            //楕円内包判定
        //            //
        //            //  x^2     y^2
        //            // ----- + ----- <= 1 が真で、内包されている。
        //            //  a^2     b^2
        //            //
        //            // a = 横軸の長さ/2　b = 縦軸の長さ/2
        //            // x,y は楕円の中心からの点座標とする。

        //            double CenterX = Draw_Location.X + Draw_Size.Width / 2;
        //            double CenterY = Draw_Location.Y + Draw_Size.Height / 2;
        //            double PtX = NowPt.X - CenterX;
        //            double PtY = NowPt.Y - CenterY;

        //            double X2 = Math.Pow(PtX, 2);
        //            double Y2 = Math.Pow(PtY, 2);
        //            double a2 = Math.Pow(Draw_Size.Width / 2 + (3 + Draw_BorderWidth / 2), 2);
        //            double b2 = Math.Pow(Draw_Size.Height / 2 + (3 + Draw_BorderWidth / 2), 2);

        //            double Ans = X2 / a2 + Y2 / b2;

        //            if (Ans <= 1) { return true; }

        //            return false;

        //        case ShapeTypeFormat.DrawType.Line:

        //            Point SelectPoint1 = new Point();
        //            Point SelectPoint2 = new Point();
        //            Point SelectPoint3 = new Point();
        //            Point SelectPoint4 = new Point();

        //            double dx = Draw_EndPoint.X - Draw_StartPoint.X;
        //            double dy = Draw_EndPoint.Y - Draw_StartPoint.Y;

        //            //90度－二点角度
        //            double Rad = (Math.PI / 2) - Math.Atan2(dy, dx);

        //            double PosCos = Math.Cos(Rad) * (3 + Draw_BorderWidth / 2);
        //            double PosSin = Math.Sin(Rad) * (3 + Draw_BorderWidth / 2);


        //            SelectPoint1.X = Draw_StartPoint.X + (int)(PosCos - PosSin);
        //            SelectPoint1.Y = Draw_StartPoint.Y - (int)(PosCos + PosSin);
        //            SelectPoint2.X = Draw_StartPoint.X - (int)(PosCos + PosSin);
        //            SelectPoint2.Y = Draw_StartPoint.Y - (int)(PosCos - PosSin);

        //            SelectPoint3.X = Draw_EndPoint.X - (int)(PosCos - PosSin);
        //            SelectPoint3.Y = Draw_EndPoint.Y + (int)(PosCos + PosSin);
        //            SelectPoint4.X = Draw_EndPoint.X + (int)(PosCos + PosSin);
        //            SelectPoint4.Y = Draw_EndPoint.Y + (int)(PosCos - PosSin);

        //            //中央点算出
        //            Point LCenterPoint = GetCenterPoint(SelectPoint1, SelectPoint2, SelectPoint3, SelectPoint4);

        //            //中央点－現在マウスポインタを線分とし、選択枠の四辺との交差を調べる
        //            bool LCrossJudge1 = GetCrossJudge(LCenterPoint, NowPt, SelectPoint1, SelectPoint2);
        //            bool LCrossJudge2 = GetCrossJudge(LCenterPoint, NowPt, SelectPoint2, SelectPoint3);
        //            bool LCrossJudge3 = GetCrossJudge(LCenterPoint, NowPt, SelectPoint3, SelectPoint4);
        //            bool LCrossJudge4 = GetCrossJudge(LCenterPoint, NowPt, SelectPoint4, SelectPoint1);

        //            if ( !LCrossJudge1  && !LCrossJudge2 && !LCrossJudge3 && !LCrossJudge4 ){ return true; }

        //            return false;
        //case ShapeTypeFormat.DrawType.Lines:

        //            for (int i = 0; i < Draw_Points.Count - 1; i++)
        //            {
        //                Point B_SelectPoint1 = new Point();
        //                Point B_SelectPoint2 = new Point();
        //                Point B_SelectPoint3 = new Point();
        //                Point B_SelectPoint4 = new Point();

        //                double B_dx = Draw_Points[i + 1].X - Draw_Points[i].X;
        //                double B_dy = Draw_Points[i + 1].Y - Draw_Points[i].Y;

        //                //90度－二点角度
        //                double B_Rad = (Math.PI / 2) - Math.Atan2(B_dy, B_dx);

        //                double B_PosCos = Math.Cos(B_Rad) * (3 + Draw_BorderWidth / 2);
        //                double B_PosSin = Math.Sin(B_Rad) * (3 + Draw_BorderWidth / 2);

        //                B_SelectPoint1.X = Draw_Points[i].X + (int)(B_PosCos - B_PosSin);
        //                B_SelectPoint1.Y = Draw_Points[i].Y - (int)(B_PosCos + B_PosSin);
        //                B_SelectPoint2.X = Draw_Points[i].X - (int)(B_PosCos + B_PosSin);
        //                B_SelectPoint2.Y = Draw_Points[i].Y - (int)(B_PosCos - B_PosSin);

        //                B_SelectPoint3.X = Draw_Points[i + 1].X - (int)(B_PosCos - B_PosSin);
        //                B_SelectPoint3.Y = Draw_Points[i + 1].Y + (int)(B_PosCos + B_PosSin);
        //                B_SelectPoint4.X = Draw_Points[i + 1].X + (int)(B_PosCos + B_PosSin);
        //                B_SelectPoint4.Y = Draw_Points[i + 1].Y + (int)(B_PosCos - B_PosSin);

        //                //中央点算出
        //                Point B_LCenterPoint = GetCenterPoint(B_SelectPoint1, B_SelectPoint2, B_SelectPoint3, B_SelectPoint4);

        //                //中央点－現在マウスポインタを線分とし、選択枠の四辺との交差を調べる
        //                bool B_LCrossJudge1 = GetCrossJudge(B_LCenterPoint, NowPt, B_SelectPoint1, B_SelectPoint2);
        //                bool B_LCrossJudge2 = GetCrossJudge(B_LCenterPoint, NowPt, B_SelectPoint2, B_SelectPoint3);
        //                bool B_LCrossJudge3 = GetCrossJudge(B_LCenterPoint, NowPt, B_SelectPoint3, B_SelectPoint4);
        //                bool B_LCrossJudge4 = GetCrossJudge(B_LCenterPoint, NowPt, B_SelectPoint4, B_SelectPoint1);

        //                if (!B_LCrossJudge1 && !B_LCrossJudge2 && !B_LCrossJudge3 && !B_LCrossJudge4) { return true; }
        //            }

        //            return false;

        //        case ShapeTypeFormat.DrawType.Polygon:

        //            //for (int i = 0; i < Draw_Points.Count; i++)
        //            //{
        //            //    int nextP = i + 1;
        //            //    //始点と終点の線
        //            //    if (nextP == Draw_Points.Count)
        //            //    {
        //            //        nextP = 0;
        //            //    }
        //            //    Point P_SelectPoint1 = new Point();
        //            //    Point P_SelectPoint2 = new Point();
        //            //    Point P_SelectPoint3 = new Point();
        //            //    Point P_SelectPoint4 = new Point();

        //            //    double P_dx = Draw_Points[nextP].X - Draw_Points[i].X;
        //            //    double P_dy = Draw_Points[nextP].Y - Draw_Points[i].Y;

        //            //    //90度－二点角度
        //            //    double P_Rad = (Math.PI / 2) - Math.Atan2(P_dy, P_dx);

        //            //    double P_PosCos = Math.Cos(P_Rad) * (3 + Draw_BorderWidth / 2);
        //            //    double P_PosSin = Math.Sin(P_Rad) * (3 + Draw_BorderWidth / 2);

        //            //    P_SelectPoint1.X = Draw_Points[i].X + (int)(P_PosCos - P_PosSin);
        //            //    P_SelectPoint1.Y = Draw_Points[i].Y - (int)(P_PosCos + P_PosSin);
        //            //    P_SelectPoint2.X = Draw_Points[i].X - (int)(P_PosCos + P_PosSin);
        //            //    P_SelectPoint2.Y = Draw_Points[i].Y - (int)(P_PosCos - P_PosSin);

        //            //    P_SelectPoint3.X = Draw_Points[nextP].X - (int)(P_PosCos - P_PosSin);
        //            //    P_SelectPoint3.Y = Draw_Points[nextP].Y + (int)(P_PosCos + P_PosSin);
        //            //    P_SelectPoint4.X = Draw_Points[nextP].X + (int)(P_PosCos + P_PosSin);
        //            //    P_SelectPoint4.Y = Draw_Points[nextP].Y + (int)(P_PosCos - P_PosSin);

        //            //    //中央点算出
        //            //    Point P_LCenterPoint = GetCenterPoint(P_SelectPoint1, P_SelectPoint2, P_SelectPoint3, P_SelectPoint4);

        //            //    //中央点－現在マウスポインタを線分とし、選択枠の四辺との交差を調べる
        //            //    bool P_LCrossJudge1 = GetCrossJudge(P_LCenterPoint, NowPt, P_SelectPoint1, P_SelectPoint2);
        //            //    bool P_LCrossJudge2 = GetCrossJudge(P_LCenterPoint, NowPt, P_SelectPoint2, P_SelectPoint3);
        //            //    bool P_LCrossJudge3 = GetCrossJudge(P_LCenterPoint, NowPt, P_SelectPoint3, P_SelectPoint4);
        //            //    bool P_LCrossJudge4 = GetCrossJudge(P_LCenterPoint, NowPt, P_SelectPoint4, P_SelectPoint1);

        //            //    if (!P_LCrossJudge1 && !P_LCrossJudge2 && !P_LCrossJudge3 && !P_LCrossJudge4) { return true; }
        //            //}

        //            //return false;

        //            return IsPointInPolygon(NowPt, Draw_Points.Count, Draw_Points.ToArray());

                
        //        default:

        //            //中央点算出
        //            Point CenterPoint = new Point(Draw_Location.X + Draw_Size.Width / 2, Draw_Location.Y + Draw_Size.Height / 2);

        //            Point Pt1 = new Point(Draw_Location.X - (3 + Draw_BorderWidth / 2), Draw_Location.Y - (3 + Draw_BorderWidth / 2));                            //左上
        //            Point Pt2 = new Point(Draw_Location.X - (3 + Draw_BorderWidth / 2), Draw_Location.Y + Draw_Size.Height + (3 + Draw_BorderWidth / 2));              //左下
        //            Point Pt3 = new Point(Draw_Location.X + Draw_Size.Width + (3 + Draw_BorderWidth / 2), Draw_Location.Y + Draw_Size.Height + (3 + Draw_BorderWidth / 2)); //右下
        //            Point Pt4 = new Point(Draw_Location.X + Draw_Size.Width + (3 + Draw_BorderWidth / 2), Draw_Location.Y - (3 + Draw_BorderWidth / 2));               //右上

        //            //中央点－現在マウスポインタを線分とし、選択枠の四辺との交差を調べる
        //            bool CrossJudge1 = GetCrossJudge(CenterPoint, NowPt, Pt1, Pt2);
        //            bool CrossJudge2 = GetCrossJudge(CenterPoint, NowPt, Pt2, Pt3);
        //            bool CrossJudge3 = GetCrossJudge(CenterPoint, NowPt, Pt3, Pt4);
        //            bool CrossJudge4 = GetCrossJudge(CenterPoint, NowPt, Pt4, Pt1);

        //            if ( !CrossJudge1 && !CrossJudge2  && !CrossJudge3 && !CrossJudge4 ) { return true; }

        //            return false;
                    
        //    }
           

        //}   
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
	        for( int ui = 1; ui < uiCountPoint + 1; ui++ )
	        {
                Point point1 = aPoint[ui % uiCountPoint];	// 最後は始点が入る（多角形データの始点と終点が一致していないデータ対応）
	            bool bFlag1x = (pointTarget.X <= point1.X);
	            bool bFlag1y = (pointTarget.Y <= point1.Y);
	            if( bFlag0y != bFlag1y )
	            {	// 線分はレイを横切る可能性あり。
		            if( bFlag0x == bFlag1x )
		            {	// 線分の２端点は対象点に対して両方右か両方左にある
			            if( bFlag0x )
			            {	// 完全に右。⇒線分はレイを横切る
				            iCountCrossing += (bFlag0y ? -1 : 1);	// 上から下にレイを横切るときには、交差回数を１引く、下から上は１足す。
			            }
		            }
		            else
		            {	// レイと交差するかどうか、対象点と同じ高さで、対象点の右で交差するか、左で交差するかを求める。
			            if( pointTarget.X <= ( point0.X + (point1.X - point0.X) * (pointTarget.Y - point0.Y ) / (point1.Y - point0.Y) ) )
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
        //変形ポイント判定
        protected bool JudgeModifyPoint(Point NowPt, Point TerminalPt, double Radian)
        {
            if (Deleted) { return false; }

            int RectangleSize = 3;

            double PosCos = Math.Cos(Radian) * RectangleSize;
            double PosSin = Math.Sin(Radian) * RectangleSize;

            int Point1X = TerminalPt.X + (int)(PosCos - PosSin);
            int Point1Y = TerminalPt.Y - (int)(PosCos + PosSin);
            int Point2X = TerminalPt.X - (int)(PosCos + PosSin);
            int Point2Y = TerminalPt.Y - (int)(PosCos - PosSin);

            int Point3X = TerminalPt.X - (int)(PosCos - PosSin);
            int Point3Y = TerminalPt.Y + (int)(PosCos + PosSin);
            int Point4X = TerminalPt.X + (int)(PosCos + PosSin);
            int Point4Y = TerminalPt.Y + (int)(PosCos - PosSin);            

            //端調査
            bool CrossJudge1 = GetCrossJudge(TerminalPt, NowPt,
                               new Point(Point1X, Point1Y),
                               new Point(Point2X, Point2Y));

            bool CrossJudge2 = GetCrossJudge(TerminalPt, NowPt,
                               new Point(Point2X, Point2Y),
                               new Point(Point3X, Point3Y));

            bool CrossJudge3 = GetCrossJudge(TerminalPt, NowPt,
                               new Point(Point3X, Point3Y),
                               new Point(Point4X, Point4Y));

            bool CrossJudge4 = GetCrossJudge(TerminalPt, NowPt,
                               new Point(Point4X, Point4Y),
                               new Point(Point1X, Point1Y));



            if (CrossJudge1 == false && CrossJudge2 == false && CrossJudge3 == false && CrossJudge4 == false)
            {
                return true;
            }

            return false;



        }

        //変形範囲判定
        public virtual string JudgeModifyRange(Point NowPt)
        {
            return "None";
        }

        //範囲選択時内包判定
        public virtual bool JudgeSelectRange(Point Pt1, Point Pt2)
        {
            return false;
        }

        #endregion

        protected bool GetCrossJudge(Point Pt1, Point Pt2, Point Pt3, Point Pt4)
        {
            // A(x1, y1), B(x2, y2), C(x3, y3), D(x4, y4)のとき、線分ABとCDが交差しているかを判定する。
            //
            // 線分ABの直線方程式
            //
            //          ( y2 - y1 )  
            // y - y1 = ----------- * ( x - x1 ) を変形させ、
            //          ( x2 - x1 )
            //
            // ( y2 - y1 )  
            // ----------- * ( x - x1 ) - ( y - y1 ) = 0　とする。
            // ( x2 - x1 )
            //
            // x1 = x2の時に除算エラーが発生するので、( x2 - x1 )をかける。
            //
            // ( y2 - y1 ) * ( x - x1 ) - ( y - y1 ) * ( x2 - x1 )= 0
            //
            // 線分CDの直線方程式も同様に
            //
            // ( y4 - y3 ) * ( x - x3 ) - ( y - y3 ) * ( x4 - x3 )= 0
            //            
            //
            // 交差するにはC,Dが直線ABを境に別々のところに居ればいいので、
            //
            // 先の式にCの座標,Dの座標を代入した式の積が正ではない⇒それぞれの式の符号が異なるかまたは0である
            //
            // すなわち
            //
            // { ( y2 - y1 )( x3 - x1 ) - ( y3 - y1 )( x2 - x1 ) } { ( y2 - y1 )( x4 - x1 ) - ( y4 - y1 )( x2 - x1 ) } ≦ 0
            //
            // 同様に
            //
            // { ( y4 - y3 )( x1 - x3 ) - ( y1 - y3 )( x4 - x3 ) } { ( y4 - y3 )( x2 - x3 ) - ( y2 - y3 )( x4 - x3 ) } ≦ 0
            //
            //この両方が成り立つとき、線分ABと線分BCは交わっている（もしくは接している）
            //

            long JudgePt3 = (Pt2.Y - Pt1.Y) * (Pt3.X - Pt1.X) - (Pt3.Y - Pt1.Y) * (Pt2.X - Pt1.X);
            long JudgePt4 = (Pt2.Y - Pt1.Y) * (Pt4.X - Pt1.X) - (Pt4.Y - Pt1.Y) * (Pt2.X - Pt1.X);
            long JudgePt1 = (Pt4.Y - Pt3.Y) * (Pt1.X - Pt3.X) - (Pt1.Y - Pt3.Y) * (Pt4.X - Pt3.X);
            long JudgePt2 = (Pt4.Y - Pt3.Y) * (Pt2.X - Pt3.X) - (Pt2.Y - Pt3.Y) * (Pt4.X - Pt3.X);

            if (JudgePt3 * JudgePt4 <= 0 && JudgePt1 * JudgePt2 <= 0) { return true; }

            return false;

        }

        protected Point GetCenterPoint(Point Pt1, Point Pt2, Point Pt3, Point Pt4)
        {
            //4点の中央点算出
            int MinX = GetMinData(Pt1.X, Pt2.X, Pt3.X, Pt4.X);
            int MaxX = GetMaxData(Pt1.X, Pt2.X, Pt3.X, Pt4.X);
            int MinY = GetMinData(Pt1.Y, Pt2.Y, Pt3.Y, Pt4.Y);
            int MaxY = GetMaxData(Pt1.Y, Pt2.Y, Pt3.Y, Pt4.Y);

            return new Point((MaxX - MinX) / 2 + MinX, (MaxY - MinY) / 2 + MinY);

        }

        private int GetMinData(int Value1, int Value2, int Value3, int Value4)
        {
            int[] numbers = { Value1, Value2, Value3, Value4 };
            System.Collections.ArrayList lst = new System.Collections.ArrayList(numbers);
            lst.Sort();
            return (int)lst[0];

        }

        private int GetMaxData(int Value1, int Value2, int Value3, int Value4)
        {
            int[] numbers = { Value1, Value2, Value3, Value4 };
            System.Collections.ArrayList lst = new System.Collections.ArrayList(numbers);
            lst.Sort();
            return (int)lst[3];

        }

        protected void DrawSelectCircle(Graphics g, Point Pt)
        {
            g.FillEllipse(Brushes.White, (float)(Pt.X - 3), (float)(Pt.Y - 3), 6, 6);
            g.DrawEllipse(Pens.Black, (float)(Pt.X - 3), (float)(Pt.Y - 3), 6, 6);
        }

        protected Size SnapSize(Size NowSize)
        {
            if (MainCtrl.SnapMode)
            {
                int WaruX = 0;
                int WaruY = 0;

                if (NowSize.Width >= 0)
                { WaruX = (int)((double)NowSize.Width / (double)MainCtrl.GridSize.Width + 0.5); }
                else
                { WaruX = (int)((double)NowSize.Width / (double)MainCtrl.GridSize.Width - 0.5); }

                if (NowSize.Height >= 0)
                { WaruY = (int)((double)NowSize.Height / (double)MainCtrl.GridSize.Height + 0.5); }
                else
                { WaruY = (int)((double)NowSize.Height / (double)MainCtrl.GridSize.Height - 0.5); }

                return new Size(WaruX * MainCtrl.GridSize.Width, WaruY * MainCtrl.GridSize.Height);
            }
            else
            {
                return NowSize;
            }

        }

        protected Point SnapPoint(Point Pt)
        {
            if (MainCtrl.SnapMode)
            {
                int WaruX = (int)((double)Pt.X / (double)(MainCtrl.GridSize.Width * ParentForm.Zoom)+ 0.5);
                int WaruY = (int)((double)Pt.Y / (double)(MainCtrl.GridSize.Height * ParentForm.Zoom) + 0.5);

                return new Point((int)(WaruX * MainCtrl.GridSize.Width * ParentForm.Zoom),(int)( WaruY * MainCtrl.GridSize.Height * ParentForm.Zoom));
            }
            else
            {
                return Pt;
            }
        }

        public virtual List<Point> NowPoints()
        {
            return null;
        }

        public virtual List<Point> ChangePoint(string changePointName, List<Point> changeObjectStartPoints, Point changePoint) { return null; }

        public virtual void Change(List<Point> changePoints) { }

        public virtual void Move(List<Point> movePoints){}

        //public virtual object Clone()
        //{
        //    #region 値型コピー
        //    ShapeObject obj = (ShapeObject)MemberwiseClone();

        //    obj.Deleted = false;
        //    #endregion          

        //    return obj;
        //}

        public virtual void Initialize(List<Point> settingPoints)
        {           
           
        }
       
        public virtual Cursor GetModifyCursor(MouseEventArgs e)
        {
            return ParentForm.Cursor;
            
        }

        public Cursor GetMoveCursor(MouseEventArgs e)
        {            
            if (this.JudgeMoveRange(e.Location))
            {
                return Cursors.SizeAll;
            }

            return ParentForm.Cursor;
        }
       
        public virtual object Serialize()
        {
            if (this.Deleted == false)
            {
                ShapeDataSerializeFormat Sdf = new ShapeDataSerializeFormat();
                Sdf.PropertyData = MakeShapeProperty(this);
                return Sdf;

            }

            return null;

        }

        protected List<PropertyDataSerializeFormat> MakeShapeProperty(object Shape)
        {
            List<PropertyDataSerializeFormat> PropertyDatas = new List<PropertyDataSerializeFormat>();

            PropertyDescriptorCollection Properties = TypeDescriptor.GetProperties(Shape, null, true);

            foreach (PropertyDescriptor Property in Properties)
            {
                if (Property.IsBrowsable && Global.GetVisibleAttribute(Shape, Property))
                {
                    PropertyDataSerializeFormat PropertyData = new PropertyDataSerializeFormat();

                    //プロパティ名
                    PropertyData.Name = Property.Name;

                    //プロパティ値
                    PropertyData.Value = Property.Converter.ConvertTo(Property.GetValue(Shape), typeof(string));

                    PropertyDatas.Add(PropertyData);
                }
            }

            return PropertyDatas;            

        }

        protected List<PropertyDataSerializeFormat> MakeSymbolProperty(Control Symbol)
        {
            List<PropertyDataSerializeFormat> PropertyDatas = new List<PropertyDataSerializeFormat>();

            PropertyDescriptorCollection Properties = TypeDescriptor.GetProperties(Symbol, null, true);

            foreach (PropertyDescriptor Property in Properties)
            {

                if (Global.GetVisibleAttribute(Symbol, Property))
                {
                    PropertyDataSerializeFormat PropertyData = new PropertyDataSerializeFormat();

                    //プロパティ名
                    PropertyData.Name = Property.Name;

                    //プロパティ値 
                    object PropertyValue = Property.GetValue(Symbol);
                    
                    TypeConverter tc = Property.Converter;
                    if (tc.GetType() == typeof(TypeConverter) || tc.GetType() == typeof(ArrayConverter)) { tc = new NestConverter(); }
                    PropertyData.Value = tc.ConvertTo(PropertyValue, typeof(string));
                    //}

                    PropertyDatas.Add(PropertyData);
                }
            }

            return PropertyDatas;            

        }

    }
}
