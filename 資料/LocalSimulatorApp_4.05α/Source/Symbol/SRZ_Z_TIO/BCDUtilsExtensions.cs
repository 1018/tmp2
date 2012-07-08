using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolLibrary
{
    static public class BCDUtilsExtensions
    {
        static public int ToInt(byte[] source, int startIndex, int byteCount)
        {
            return BCDUtils.ToInt(source.Skip(startIndex).Take(byteCount).ToArray());
        }

        static public int ToInt(byte[] source, int startIndex, int byteCount, bool sign)
        {
            int result = BCDUtils.ToInt(source.Skip(startIndex).Take(byteCount).ToArray());

            if (sign)
            {
                string strResult = result.ToString().PadLeft(byteCount * 2, '0');
                if (strResult.StartsWith("1"))
                {
                    result = -int.Parse(strResult.Substring(1));
                }
            }

            return result;
        }

        static public byte[] ToBCD(int source, int byteCount)
        {
            return BCDUtils.ToBCD(source, byteCount);
        }

        static public byte[] ToBCD(int source, int byteCount, bool sign)
        {
            List<byte> result = new List<byte>();

            if (sign)
            {
                if (source < 0)
                {
                    //result.Add(0x01);
                    result.AddRange(BCDUtils.ToBCD(Math.Abs(source) / 10000 + 10, 1));
                }
                else
                {
                    //result.Add(0x00);
                    result.AddRange(BCDUtils.ToBCD(Math.Abs(source) / 10000, 1));
                }
            }

            result.AddRange(BCDUtils.ToBCD(Math.Abs(source), byteCount - result.Count));

            return result.ToArray();
        }
    }
}
