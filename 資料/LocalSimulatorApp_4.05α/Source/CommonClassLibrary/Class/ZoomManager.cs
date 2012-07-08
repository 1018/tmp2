using System;
using System.Drawing;

namespace CommonClassLibrary
{
    public class ZoomManager
    {
        /// <summary>
        /// 倍率を反映したPointを返す。
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static Point MagnifyPoint(Point Pt, double Zoom)
        {
            int x = Convert.ToInt32((double)Pt.X * Zoom);
            int y = Convert.ToInt32((double)Pt.Y * Zoom);
            return new Point(x, y);
        }

        /// <summary>
        /// 倍率を反映したPointFを返す。
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static PointF MagnifyPoint(PointF Pt, double Zoom)
        {
            float x = (float)((double)Pt.X * Zoom);
            float y = (float)((double)Pt.Y * Zoom);
            return new PointF(x, y);
        }

        /// <summary>
        /// 倍率を反映したSizeを返す。
        /// </summary>
        /// <param name="sz"></param>
        /// <returns></returns>
        public static Size MagnifySize(Size Sz, double Zoom)
        {
            int w = Convert.ToInt32((double)Sz.Width * Zoom);
            int h = Convert.ToInt32((double)Sz.Height * Zoom);
            return new Size(w, h);
        }

        /// <summary>
        /// 倍率を反映したSizeFを返す。
        /// </summary>
        /// <param name="sz"></param>
        /// <returns></returns>
        public static SizeF MagnifySize(SizeF sz, double Zoom)
        {
            float x = (float)((double)sz.Width * Zoom);
            float y = (float)((double)sz.Height * Zoom);
            return new SizeF(x, y);
        }

        /// <summary>
        /// 倍率を反映したRectangleを返す。
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rectangle MagnifyRectangle(Rectangle Rect, double Zoom)
        {
            int x = Convert.ToInt32((double)Rect.X * Zoom);
            int y = Convert.ToInt32((double)Rect.Y * Zoom);
            int w = Convert.ToInt32((double)Rect.Width * Zoom);
            int h = Convert.ToInt32((double)Rect.Height * Zoom);
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// 倍率を反映したRectangleFを返す。
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static RectangleF MagnifyRectangle(RectangleF Rect, double Zoom)
        {
            float x = (float)((double)Rect.X * Zoom);
            float y = (float)((double)Rect.Y * Zoom);
            float w = (float)((double)Rect.Width * Zoom);
            float h = (float)((double)Rect.Height * Zoom);
            return new RectangleF(x, y, w, h);
        }

        /// <summary>
        /// 倍率が反映されたPointを元に戻す。
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static Point ToBasePoint(Point Pt, double Zoom)
        {
            int x = 0;
            int y = 0;

            //if (_ZoomZero == false)
            //{
                x = Convert.ToInt32((double)Pt.X / Zoom);
                y = Convert.ToInt32((double)Pt.Y / Zoom);
            //}
            return new Point(x, y);
        }

        /// <summary>
        /// 倍率が反映されたPointFを元に戻す。
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static PointF ToBasePoint(PointF Pt, double Zoom)
        {
            float x = 0;
            float y = 0;

            //if (_ZoomZero == false)
            //{
                x = (float)((double)Pt.X / Zoom);
                y = (float)((double)Pt.Y / Zoom);
            //}
            return new PointF(x, y);
        }

        /// <summary>
        /// 倍率が反映されたSizeを元に戻す。
        /// </summary>
        /// <param name="sz"></param>
        /// <returns></returns>
        public static Size ToBaseSize(Size Sz, double Zoom)
        {
            int w = 0;
            int h = 0;

            //if (_ZoomZero == false)
            //{
                w = Convert.ToInt32((double)Sz.Width / Zoom);
                h = Convert.ToInt32((double)Sz.Height / Zoom);
            //}
            return new Size(w, h);
        }

        /// <summary>
        /// 倍率が反映されたSizeFを元に戻す。
        /// </summary>
        /// <param name="sz"></param>
        /// <returns></returns>
        public static SizeF ToBaseSize(SizeF Sz, double Zoom)
        {
            float w = 0;
            float h = 0;

            //if (_ZoomZero == false)
            //{
                w = (float)((double)Sz.Width / Zoom);
                h = (float)((double)Sz.Height / Zoom);
            //}
            return new SizeF(w, h);
        }

        /// <summary>
        /// 倍率が反映されたRectangleを元に戻す。
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rectangle ToBaseRectangle(Rectangle Rect, double Zoom)
        {
            int x = 0;
            int y = 0;
            int w = 0;
            int h = 0;

            //if (_ZoomZero == false)
            //{
                x = Convert.ToInt32((double)Rect.X / Zoom);
                y = Convert.ToInt32((double)Rect.Y / Zoom);
                w = Convert.ToInt32((double)Rect.Width / Zoom);
                h = Convert.ToInt32((double)Rect.Height / Zoom);
            //}
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// 倍率が反映されたRectangleFを元に戻す。
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static RectangleF ToBaseRectangle(RectangleF Rect, double Zoom)
        {
            float x = 0;
            float y = 0;
            float w = 0;
            float h = 0;

            //if (_ZoomZero == false)
            //{
                x = (float)((double)Rect.X / Zoom);
                y = (float)((double)Rect.Y / Zoom);
                w = (float)((double)Rect.Width / Zoom);
                h = (float)((double)Rect.Height / Zoom);
            //}
            return new RectangleF(x, y, w, h);
        }
    }

   
}
