using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace SymbolLibrary
{
    static public class AsciiUtils
    {
        /// <summary>
        /// ASCIIコード列 → 数値 変換
        /// </summary>
        /// <param name="ascii"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        static public int ToInt(byte[] ascii, int startIndex, int length)
        {
            byte[] source = ascii.Skip(startIndex).Take(length).ToArray();

            return ToInt(source, false);
        }
        /// <summary>
        /// ASCIIコード列 → 数値 変換
        /// </summary>
        /// <param name="ascii"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <param name="isHex"></param>
        /// <returns></returns>
        static public int ToInt(byte[] ascii, int startIndex, int length, bool isHex)
        {
            byte[] source = ascii.Skip(startIndex).Take(length).ToArray();

            return ToInt(source, isHex);
        }
        /// <summary>
        /// ASCIIコード列 → 数値 変換
        /// </summary>
        /// <param name="ascii"></param>
        /// <returns></returns>
        static public int ToInt(byte[] ascii)
        {
            return ToInt(ascii, false);
        }
        /// <summary>
        /// ASCIIコード列 → 数値 変換
        /// </summary>
        /// <param name="ascii"></param>
        /// <param name="isHex"></param>
        /// <returns></returns>
        static public int ToInt(byte[] ascii, bool isHex)
        {
            string str = Encoding.ASCII.GetString(ascii);

            if (isHex)
            {
                return int.Parse(str, NumberStyles.HexNumber);
            }
            else
            {
                return int.Parse(str, NumberStyles.Integer);
            }
        }


        /// <summary>
        /// 数値 → ASCIIコード列
        /// </summary>
        /// <param name="num"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        static public byte[] ToAscii(int num, int length)
        {
            return ToAscii(num, length, false);
        }
        /// <summary>
        /// 数値 → ASCIIコード列
        /// </summary>
        /// <param name="num"></param>
        /// <param name="length"></param>
        /// <param name="isHex"></param>
        /// <returns></returns>
        static public byte[] ToAscii(int num, int length, bool isHex)
        {
            string str = num.ToString(isHex ? "X" : "D");

            // 桁合わせ
            if (str.Length < length)
            {
                str = str.PadLeft(length);
            }
            else if (str.Length > length)
            {
                str = str.Substring(str.Length - length);
            }

            return ToAscii(str);
        }
        /// <summary>
        /// 文字列 → ASCIIコード列
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public byte[] ToAscii(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }


        /// <summary>
        /// ASCIIコード列 → 文字列
        /// </summary>
        /// <param name="ascii"></param>
        /// <returns></returns>
        static public string ToString(byte[] ascii)
        {
            return Encoding.ASCII.GetString(ascii);
        }
        /// <summary>
        /// ASCIIコード列 → 文字列
        /// </summary>
        /// <param name="ascii"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        static public string ToString(byte[] ascii, int startIndex, int length)
        {
            return Encoding.ASCII.GetString(ascii, startIndex, length);
        }
    }
}
