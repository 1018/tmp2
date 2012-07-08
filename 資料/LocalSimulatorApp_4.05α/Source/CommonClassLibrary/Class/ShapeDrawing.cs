using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CommonClassLibrary
{
    public interface IDrawing
    {
        void Draw(Graphics canvas);
        void Draw(Graphics canvas, double zoom);
    }

    public class CircleDrawing : IDrawing
    {
        private Pen pen = null;
        private SolidBrush brush = null;
        private List<Point> drawPoints = new List<Point>();        
        private bool IsAssist = false;

        public CircleDrawing(Color borderColor, int borderWidth, DashStyle borderStyle, Color fillColor, List<Point> drawPoints, bool IsAssist)
        {
            brush = new SolidBrush(fillColor);
            pen = new Pen(borderColor, borderWidth);
            pen.DashStyle = borderStyle;
            this.drawPoints = drawPoints;            
            this.IsAssist = IsAssist;
        }

        public void Draw(Graphics canvas)
        {
            Draw(canvas, 1);            
        }

        public void Draw(Graphics canvas, double zoom)
        {
            if (drawPoints.Count < 2) { return; }

            int width = 0;
            int height = 0;
            int startX = 0;
            int startY = 0;

            if (drawPoints[0].X < drawPoints[1].X)
            {
                startX = drawPoints[0].X;
                width = drawPoints[1].X - drawPoints[0].X;
            }
            else
            {
                startX = drawPoints[1].X;
                width = drawPoints[0].X - drawPoints[1].X;
            }

            if (drawPoints[0].Y < drawPoints[1].Y)
            {
                startY = drawPoints[0].Y;
                height = drawPoints[1].Y - drawPoints[0].Y;
            }
            else
            {
                startY = drawPoints[1].Y;
                height = drawPoints[0].Y - drawPoints[1].Y;
            }

            Rectangle rect = new Rectangle((int)(startX * zoom), (int)(startY * zoom), (int)(width * zoom), (int)(height * zoom));
            Pen zoomPen = new Pen(pen.Brush, (float)(pen.Width * zoom));

            canvas.FillEllipse(brush, rect);
            canvas.DrawEllipse(zoomPen, rect);

            //補助線描画
            if (IsAssist)
            {
                Pen assistPen = new Pen(Color.Black, 1);
                assistPen.DashStyle = DashStyle.Dot;
                canvas.DrawRectangle(assistPen, rect);
            }
        }


    }

    public class SquareDrawing : IDrawing
    {
        private Pen pen = null;
        private SolidBrush brush = null;
        private List<Point> drawPoints = new List<Point>();

        public SquareDrawing(Color borderColor, int borderWidth, DashStyle borderStyle, Color fillColor, List<Point> drawPoints)
        {
            brush = new SolidBrush(fillColor);
            pen = new Pen(borderColor, borderWidth);
            pen.DashStyle = borderStyle;
            this.drawPoints = drawPoints;
        }

        public void Draw(Graphics canvas)
        {
            Draw(canvas, 1);                    
        }

        public void Draw(Graphics canvas, double zoom)
        {
            if (drawPoints.Count < 2) { return; }

            int width = 0;
            int height = 0;
            int startX = 0;
            int startY = 0;

            if (drawPoints[0].X < drawPoints[1].X)
            {
                startX = drawPoints[0].X;
                width = drawPoints[1].X - drawPoints[0].X;
            }
            else
            {
                startX = drawPoints[1].X;
                width = drawPoints[0].X - drawPoints[1].X;
            }

            if (drawPoints[0].Y < drawPoints[1].Y)
            {
                startY = drawPoints[0].Y;
                height = drawPoints[1].Y - drawPoints[0].Y;
            }
            else
            {
                startY = drawPoints[1].Y;
                height = drawPoints[0].Y - drawPoints[1].Y;
            }

            Rectangle rect = new Rectangle((int)(startX * zoom), (int)(startY * zoom), (int)(width * zoom), (int)(height * zoom));
            Pen zoomPen = new Pen(pen.Brush, (float)(pen.Width * zoom));
            zoomPen.DashStyle = pen.DashStyle;
            canvas.FillRectangle(brush, rect);
            canvas.DrawRectangle(zoomPen, rect);
        }


    }

    public class TextDrawing : IDrawing
    {
        private Pen pen = null;
        private SolidBrush rectBrush = null;
        private StringFormat textFormat = new StringFormat();
        private SolidBrush textBrush = null;
        private Font textFont = null;
        private string textString = null;
        private List<Point> drawPoints = new List<Point>();

        public TextDrawing(Color borderColor, int borderWidth, DashStyle borderStyle, Color fillColor,
                           Color textColor, string textString, Font textFont, ContentAlignment alignment, List<Point> drawPoints)
        {
            rectBrush = new SolidBrush(fillColor);
            pen = new Pen(borderColor, borderWidth);
            pen.DashStyle = borderStyle;

            #region Alignment設定
            switch (alignment)
            {
                case ContentAlignment.BottomCenter:
                    textFormat.Alignment = StringAlignment.Center;
                    textFormat.LineAlignment = StringAlignment.Far;
                    break;

                case ContentAlignment.BottomLeft:
                    textFormat.Alignment = StringAlignment.Near;
                    textFormat.LineAlignment = StringAlignment.Far;
                    break;

                case ContentAlignment.BottomRight:
                    textFormat.Alignment = StringAlignment.Far;
                    textFormat.LineAlignment = StringAlignment.Far;
                    break;

                case ContentAlignment.MiddleCenter:
                    textFormat.Alignment = StringAlignment.Center;
                    textFormat.LineAlignment = StringAlignment.Center;
                    break;

                case ContentAlignment.MiddleLeft:

                    textFormat.Alignment = StringAlignment.Near;
                    textFormat.LineAlignment = StringAlignment.Center;
                    break;

                case ContentAlignment.MiddleRight:
                    textFormat.Alignment = StringAlignment.Far;
                    textFormat.LineAlignment = StringAlignment.Center;
                    break;

                case ContentAlignment.TopCenter:
                    textFormat.Alignment = StringAlignment.Center;
                    textFormat.LineAlignment = StringAlignment.Near;
                    break;

                case ContentAlignment.TopLeft:
                    textFormat.Alignment = StringAlignment.Near;
                    textFormat.LineAlignment = StringAlignment.Near;
                    break;

                case ContentAlignment.TopRight:
                    textFormat.Alignment = StringAlignment.Far;
                    textFormat.LineAlignment = StringAlignment.Near;
                    break;
            }
            #endregion

            this.textBrush = new SolidBrush(textColor);
            this.textFont = textFont;
            this.textString = textString;
            this.drawPoints = drawPoints;

        }

        public void Draw(Graphics canvas)
        {
            Draw(canvas, 1);
        }

        public void Draw(Graphics canvas, double zoom)
        {
            if (drawPoints.Count < 2) { return; }

            int width = 0;
            int height = 0;
            int startX = 0;
            int startY = 0;

            if (drawPoints[0].X < drawPoints[1].X)
            {
                startX = drawPoints[0].X;
                width = drawPoints[1].X - drawPoints[0].X;
            }
            else
            {
                startX = drawPoints[1].X;
                width = drawPoints[0].X - drawPoints[1].X;
            }

            if (drawPoints[0].Y < drawPoints[1].Y)
            {
                startY = drawPoints[0].Y;
                height = drawPoints[1].Y - drawPoints[0].Y;
            }
            else
            {
                startY = drawPoints[1].Y;
                height = drawPoints[0].Y - drawPoints[1].Y;
            }

            Rectangle rect = new Rectangle((int)(startX * zoom), (int)(startY * zoom), (int)(width * zoom), (int)(height * zoom));
            Pen zoomPen = new Pen(pen.Brush, (float)(pen.Width * zoom));
            zoomPen.DashStyle = pen.DashStyle;
            Font zoomFont = new Font(textFont.Name, (float)(textFont.Size * zoom));
            canvas.FillRectangle(rectBrush, rect);
            canvas.DrawRectangle(zoomPen, rect);
            canvas.DrawString(textString, zoomFont, textBrush, rect, textFormat);

        }


    }

    public class LinesDrawing : IDrawing
    {
        private Pen borderPen = null;
        private Pen linePen = null;
        private List<Point> drawPoints = new List<Point>();

        public LinesDrawing(Color borderColor, int borderWidth, DashStyle borderStyle, Color lineColor, int lineWidth, DashStyle lineStyle, List<Point> drawPoints)
        {
            int outLineWidth = lineWidth;
            int inLineWidth = lineWidth - borderWidth * 2;
            if (inLineWidth < 1) { inLineWidth = 1; }

            borderPen = new Pen(borderColor, outLineWidth);
            borderPen.DashStyle = borderStyle;

            linePen = new Pen(lineColor, inLineWidth);
            linePen.DashStyle = lineStyle;

            this.drawPoints = drawPoints;
        }

        public void Draw(Graphics canvas)
        {
            Draw(canvas, 1);        
        }

        public void Draw(Graphics canvas ,double zoom)
        {
            if (drawPoints.Count < 2) { return; }

            Pen zoomBorderPen = new Pen(borderPen.Brush, (float)(borderPen.Width * zoom));
            zoomBorderPen.DashStyle = borderPen.DashStyle;

            Pen zoomLinePen = new Pen(linePen.Brush, (float)(linePen.Width * zoom));
            zoomLinePen.DashStyle = linePen.DashStyle;

            List<Point> zoomDrawPoints = new List<Point>();

            foreach (Point drawPoint in drawPoints)
            {
                zoomDrawPoints.Add(new Point((int)(drawPoint.X * zoom), (int)(drawPoint.Y * zoom)));
            }

            canvas.DrawLines(zoomBorderPen, zoomDrawPoints.ToArray());

            canvas.DrawLines(zoomLinePen, zoomDrawPoints.ToArray());
        }


    }

    public class LineDrawing : IDrawing
    {
        private Pen borderPen = null;
        private Pen linePen = null;
        private List<Point> drawPoints = new List<Point>();

        public LineDrawing(Color borderColor, int borderWidth, DashStyle borderStyle, Color lineColor, int lineWidth, DashStyle lineStyle,  List<Point> drawPoints)
        {
            int outLineWidth = lineWidth;
            int inLineWidth = lineWidth - borderWidth * 2;
            if (inLineWidth < 1) { inLineWidth = 1; }

            borderPen = new Pen(borderColor, outLineWidth);
            borderPen.DashStyle = borderStyle;

            linePen = new Pen(lineColor, inLineWidth);
            linePen.DashStyle = lineStyle;

            this.drawPoints = drawPoints;
        }

        public void Draw(Graphics canvas)
        {
            Draw(canvas, 1);        
        }

        public void Draw(Graphics canvas ,double zoom)
        {
            if (drawPoints.Count < 2) { return; }

            Pen zoomBorderPen = new Pen(borderPen.Brush, (float)(borderPen.Width * zoom));
            zoomBorderPen.DashStyle = borderPen.DashStyle;

            Pen zoomLinePen = new Pen(linePen.Brush, (float)(linePen.Width * zoom));
            zoomLinePen.DashStyle = linePen.DashStyle;

            List<Point> zoomDrawPoints = new List<Point>();

            foreach (Point drawPoint in drawPoints)
            {
                zoomDrawPoints.Add(new Point((int)(drawPoint.X * zoom), (int)(drawPoint.Y * zoom)));
            }


            canvas.DrawLines(zoomBorderPen, zoomDrawPoints.ToArray());

            canvas.DrawLines(zoomLinePen, zoomDrawPoints.ToArray());

        }

    }

    public class PolygonDrawing : IDrawing
    {
        private Pen pen = null;
        private SolidBrush brush = null;
        private List<Point> drawPoints = new List<Point>();

        public PolygonDrawing(Color borderColor, int borderWidth, DashStyle borderStyle, Color fillColor, List<Point> drawPoints)
        {

            brush = new SolidBrush(fillColor);
            pen = new Pen(borderColor, borderWidth);
            pen.DashStyle = borderStyle;
            this.drawPoints = drawPoints;
            
        }

        public void Draw(Graphics canvas)
        {
            Draw(canvas, 1);        
        }

        public void Draw(Graphics canvas, double zoom)
        {
            if (drawPoints.Count < 2) { return; }

            Pen zoomPen = new Pen(pen.Brush, (float)(pen.Width * zoom));
            zoomPen.DashStyle = pen.DashStyle;

            List<Point> zoomDrawPoints = new List<Point>();

            foreach (Point drawPoint in drawPoints)
            {
                zoomDrawPoints.Add(new Point((int)(drawPoint.X * zoom), (int)(drawPoint.Y * zoom)));
            }


            canvas.FillPolygon(brush, zoomDrawPoints.ToArray());

            canvas.DrawPolygon(zoomPen, zoomDrawPoints.ToArray());


        }


    }

}
      
