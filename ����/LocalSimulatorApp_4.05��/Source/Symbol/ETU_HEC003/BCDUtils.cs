using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolLibrary
{
    public static class BCDUtils
    {
        /// <summary>
        /// BCD → Int32変換
        /// </summary>
        /// <param name="bcd">BCDデータ</param>
        /// <param name="sign">上位4bitを符号とする(trueで有効)</param>
        /// <returns>Int32値</returns>
        public static Int32 ToInt(byte[] bcd, bool sign)
        {
            Int32 result = 0;

            bool minus = false;
            bool first = true;

            foreach (byte b in bcd)
            {
                int digit1 = b >> 4;
                int digit2 = b & 0x0f;

                // 符号付きなら、先頭4bitは符号桁
                if (sign && first)
                {
                    first = false;

                    // 先頭4bitが1なら負数
                    if (digit1 == 1)
                    {
                        minus = true;
                    }
                    digit1 = 0;
                }

                result = (result * 100) + (digit1 * 10) + digit2;
            }

            return (!minus) ? result : -result;
        }
        /// <summary>
        /// BCD → Int32変換
        /// </summary>
        /// <param name="bcd">BCDデータ</param>
        /// <returns>Int32値</returns>
        public static Int32 ToInt(byte[] bcd)
        {
            return ToInt(bcd, false);
        }
        /// <summary>
        /// BCD → Int32変換
        /// </summary>
        /// <param name="bcd">BCDデータ</param>
        /// <param name="startIndex">BCDデータの開始位置</param>
        /// <param name="sign">上位4bitを符号とする(trueで有効)</param>
        /// <returns>Int32値</returns>
        public static Int32 ToInt(byte[] bcd, int startIndex, bool sign)
        {
            if (startIndex < 0) throw new ArgumentException("0以上の値を設定して下さい。", "startIndex");
            if (bcd.Count() < startIndex) throw new InvalidOperationException("指定された値をInt型に変換することが出来ません。");

            return ToInt(bcd.Skip(startIndex).ToArray(), sign);
        }
        /// <summary>
        /// BCD → Int32変換
        /// </summary>
        /// <param name="bcd">BCDデータ</param>
        /// <param name="startIndex">BCDデータの開始位置</param>
        /// <returns>Int32値</returns>
        public static Int32 ToInt(byte[] bcd, int startIndex)
        {
            return ToInt(bcd, startIndex, false);
        }
        /// <summary>
        /// BCD → Int32変換
        /// </summary>
        /// <param name="bcd">BCDデータ</param>
        /// <param name="startIndex">BCDデータの開始位置</param>
        /// <param name="byteCount">BCDデータのバイト数</param>
        /// <param name="sign">上位4bitを符号とする(trueで有効)</param>
        /// <returns>Int32値</returns>
        public static Int32 ToInt(byte[] bcd, int startIndex, int byteCount, bool sign)
        {
            if (startIndex < 0) throw new ArgumentException("0以上の値を設定して下さい。", "startIndex");
            if (byteCount < 1) throw new ArgumentException("1以上の値を設定して下さい。", "byteCount");
            if (bcd.Count() < startIndex + byteCount) throw new InvalidOperationException("指定された値をInt型に変換することが出来ません。");

            return BCDUtils.ToInt(bcd.Skip(startIndex).Take(byteCount).ToArray(), sign);
        }
        /// <summary>
        /// BCD → Int32変換
        /// </summary>
        /// <param name="bcd">BCDデータ</param>
        /// <param name="startIndex">BCDデータの開始位置</param>
        /// <param name="byteCount">BCDデータのバイト数</param>
        /// <returns>Int32値</returns>
        public static Int32 ToInt(byte[] bcd, int startIndex, int byteCount)
        {
            return ToInt(bcd, startIndex, byteCount, false);
        }

        /// <summary>
        /// Int32 → BCD変換
        /// </summary>
        /// <param name="num">Int32データ</param>
        /// <param name="byteCount">変換後のバイト数</param>
        /// <param name="sign">上位4bitを符号とする(trueで有効)</param>
        /// <returns>BCD値</returns>
        public static byte[] ToBCD(Int32 num, int byteCount, bool sign)
        {
            int val = Math.Abs(num);

            byte[] bcdNumber = new byte[byteCount];
            for (int i = 1; i <= byteCount; i++)
            {
                int mod = val % 100;

                int digit2 = mod % 10;
                int digit1 = (mod - digit2) / 10;

                bcdNumber[byteCount - i] = Convert.ToByte((digit1 * 16) + digit2);

                val = (val - mod) / 100;
            }

            // 上位4bitを符号とする
            if (sign)
            {
                bcdNumber[0] &= 0x0F;

                if (num < 0)
                {
                    bcdNumber[0] |= 0x10;
                }
            }

            return bcdNumber;
        }
        /// <summary>
        /// Int32 → BCD変換
        /// </summary>
        /// <param name="num">Int32データ</param>
        /// <param name="byteCount">変換後のバイト数</param>
        /// <returns>BCD値</returns>
        public static byte[] ToBCD(Int32 num, int byteCount)
        {
            return ToBCD(num, byteCount, false);
        }


        /*
        [STAThread]
        static void Main(string[] args)
        {
            byte[] source = { 0x12, 0x13, 0x45 };

            // Display the number of "121345"
            Console.WriteLine("ToInt() = {0}", ToInt(source, false));

            // Display the number of "-21345"
            Console.WriteLine("ToInt() = {0}", ToInt(source, true));

            // Display the number of "-345"
            Console.WriteLine("ToInt() = {0}", ToInt(source, 1, true));

            // Display the number of "13"
            Console.WriteLine("ToInt() = {0}", ToInt(source, 1, 1, false));


            byte[] destination;
            string resultStr;

            // Display the number of "00 00 01 86"
            destination = ToBCD(186, 4);
            resultStr = string.Join(" ", destination.Select((v) => v.ToString("X2")).ToArray());
            Console.WriteLine("ToBCD() = {0}", resultStr);

            // Display the number of "10 01 86"
            destination = ToBCD(-186, 3, true);
            resultStr = string.Join(" ", destination.Select((v) => v.ToString("X2")).ToArray());
            Console.WriteLine("ToBCD() = {0}", resultStr);


            Console.ReadKey();      // Wait...
        }
        */
    }
}
