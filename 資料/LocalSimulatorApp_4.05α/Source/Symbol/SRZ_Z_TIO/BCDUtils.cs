using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolLibrary
{
    public static class BCDUtils
    {
        public static int ToInt(IEnumerable<byte> bcd, bool sign)
        {
            int result = 0;

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

        public static int ToInt(IEnumerable<byte> bcd)
        {
            return ToInt(bcd, false);
        }

        public static int ToInt(IEnumerable<byte> bcd, int startIndex, bool sign)
        {
            if (startIndex < 0) throw new ArgumentException("0以上の値を設定して下さい。", "startIndex");
            if (bcd.Count() < startIndex) throw new InvalidOperationException("指定された値をInt型に変換することが出来ません。");

            return ToInt(bcd.Skip(startIndex), sign);
        }

        public static int ToInt(IEnumerable<byte> bcd, int startIndex)
        {
            return ToInt(bcd, startIndex, false);
        }

        public static int ToInt(IEnumerable<byte> bcd, int startIndex, int byteCount, bool sign)
        {
            if (startIndex < 0) throw new ArgumentException("0以上の値を設定して下さい。", "startIndex");
            if (byteCount < 1) throw new ArgumentException("1以上の値を設定して下さい。", "byteCount");
            if (bcd.Count() < startIndex + byteCount) throw new InvalidOperationException("指定された値をInt型に変換することが出来ません。");

            return BCDUtils.ToInt(bcd.Skip(startIndex).Take(byteCount), sign);
        }

        public static int ToInt(IEnumerable<byte> bcd, int startIndex, int byteCount)
        {
            return ToInt(bcd, startIndex, byteCount, false);
        }

        public static byte[] ToBCD(int num, int byteCount)
        {
            return ToBCD<int>(num, byteCount);
        }

        public static byte[] ToBCD(long num, int byteCount)
        {
            return ToBCD<long>(num, byteCount);
        }

        public static byte[] ToBCD(bool num, int byteCount)
        {
            if (num == true)
            {
                return ToBCD<int>(1, byteCount);
            }
            else
            {
                return ToBCD<int>(0, byteCount);
            }
        }

        private static byte[] ToBCD<T>(T num, int byteCount) where T : struct, IConvertible
        {
            long val = Convert.ToInt64(num);

            byte[] bcdNumber = new byte[byteCount];
            for (int i = 1; i <= byteCount; i++)
            {
                long mod = val % 100;

                long digit2 = mod % 10;
                long digit1 = (mod - digit2) / 10;

                bcdNumber[byteCount - i] = Convert.ToByte((digit1 * 16) + digit2);

                val = (val - mod) / 100;
            }

            return bcdNumber;
        }
    }
}
