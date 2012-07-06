using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolLibrary
{
    static public class AsciiUtils
    {
        static public int ToInt(byte[] ascii)
        {
            string str = Encoding.ASCII.GetString(ascii);

            return int.Parse(str);
        }

        static public int ToInt(byte[] ascii, int startIndex, int length)
        {
            byte[] source = ascii.Skip(startIndex).Take(length).ToArray();

            return ToInt(source);
        }

        static public byte[] ToAscii(int num, int length)
        {
            string str = num.ToString().PadLeft(length);

            if (str.Length > length)
            {
                str = str.Substring(str.Length - length);
            }

            return Encoding.ASCII.GetBytes(str);
        }

        static public byte[] ToAscii(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        static public string ToString(byte[] ascii)
        {
            return Encoding.ASCII.GetString(ascii);
        }

        static public string ToString(byte[] ascii, int startIndex, int length)
        {
            return Encoding.ASCII.GetString(ascii, startIndex, length);
        }
    }
}
