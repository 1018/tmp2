using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LocalSimulator.MainProgram
{
    // Point型の拡張メソッド
    public static class PointExtend
    {
        /// <summary>
        /// Point型をオフセットした値を返します。
        /// </summary>
        /// <param name="basePoint">ベースとなるPoint</param>
        /// <param name="offset">オフセット値</param>
        /// <returns>オフセットされたPoint</returns>
        public static Point Add(this Point basePoint, Point offset)
        {
            return new Point(basePoint.X + offset.X, basePoint.Y + offset.Y);
        }

        /// <summary>
        /// Point型をオフセットした値を返します。
        /// </summary>
        /// <param name="basePoint">ベースとなるPoint</param>
        /// <param name="offset">オフセット値</param>
        /// <returns>オフセットされたPoint</returns>
        public static Point Add(this Point basePoint, Size offset)
        {
            return Add(basePoint, (Point)offset);
        }

    }
}
