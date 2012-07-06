using System;
using CommonClassLibrary;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;

namespace PC_PLC
{
    [Serializable]
    public class DeviceManager : IDeviceManager
    {
        delegate DeviceElement CreateElementDelegate(string target);

        static DeviceManager()
        {
            ElementCreaterDic = new Dictionary<string, CreateElementDelegate>();

            foreach (string prefix in BitDevice.PrefixsFix)
            {
                string tempPrefix = prefix;     // ラムダ式のための一時変数
                ElementCreaterDic[prefix] =
                    new CreateElementDelegate((target) => new BitDevice(target, tempPrefix, 16));
            }

            foreach (string prefix in BitDevice.PrefixsDec)
            {
                string tempPrefix = prefix;     // ラムダ式のための一時変数
                ElementCreaterDic[prefix] =
                    new CreateElementDelegate((target) => new BitDevice(target, tempPrefix, 10));
            }

            foreach (string prefix in WordDevice.PrefixsFix)
            {
                string tempPrefix = prefix;     // ラムダ式のための一時変数
                ElementCreaterDic[prefix] =
                    new CreateElementDelegate((target) => new WordDevice(target, tempPrefix, 16));
            }

            foreach (string prefix in WordDevice.PrefixsDec)
            {
                string tempPrefix = prefix;     // ラムダ式のための一時変数
                ElementCreaterDic[prefix] =
                    new CreateElementDelegate((target) => new WordDevice(target, tempPrefix, 10));
            }

            foreach (string prefix in VirtualDevice.Prefixs)
            {
                string tempPrefix = prefix;     // ラムダ式のための一時変数
                ElementCreaterDic[prefix] =
                    new CreateElementDelegate((target) => new VirtualDevice(target, tempPrefix, 10));
            }

            foreach (string prefix in BufDevice.Prefixs)
            {
                string tempPrefix = prefix;     // ラムダ式のための一時変数
                ElementCreaterDic[prefix] =
                    new CreateElementDelegate((target) => new BufDevice(target, tempPrefix, 10));
            }

            foreach (string prefix in CCLinkDevice.Prefixs)
            {
                string tempPrefix = prefix;     // ラムダ式のための一時変数
                ElementCreaterDic[prefix] =
                    new CreateElementDelegate((target) => new CCLinkDevice(target, tempPrefix, 10));
            }
        }

        static Dictionary<string, CreateElementDelegate> ElementCreaterDic;

        DeviceElement IDeviceManager.ToElement(string address)
        {
            // nullチェック
            if (string.IsNullOrEmpty(address)) { return null; }

            // 大文字変換
            string convertAddress = address.ToUpper();

            //if (false)
            //{
            //    // タイプ判断
            //    #region 16進ビットデバイス
            //    foreach (string prefix in BitDevice.PrefixsFix)
            //    {
            //        if (convertAddress.IndexOf(prefix) == 0)
            //        {
            //            try { return new BitDevice(convertAddress, prefix, 16); }
            //            catch { }
            //        }
            //    }
            //    #endregion
            //    #region 10進ビットデバイス
            //    foreach (string prefix in BitDevice.PrefixsDec)
            //    {
            //        if (convertAddress.IndexOf(prefix) == 0)
            //        {
            //            try { return new BitDevice(convertAddress, prefix, 10); }
            //            catch { }
            //        }
            //    }
            //    #endregion
            //    #region 16進ワードデバイス
            //    foreach (string prefix in WordDevice.PrefixsFix)
            //    {
            //        if (convertAddress.IndexOf(prefix) == 0)
            //        {
            //            try { return new WordDevice(convertAddress, prefix, 16); }
            //            catch { }
            //        }
            //    }
            //    #endregion
            //    #region 10進ワードデバイス
            //    foreach (string prefix in WordDevice.PrefixsDec)
            //    {
            //        if (convertAddress.IndexOf(prefix) == 0)
            //        {
            //            try { return new WordDevice(convertAddress, prefix, 10); }
            //            catch { }
            //        }
            //    }
            //    #endregion
            //    #region 仮想デバイス
            //    foreach (string prefix in VirtualDevice.Prefixs)
            //    {
            //        if (convertAddress.IndexOf(prefix) == 0)
            //        {
            //            try { return new VirtualDevice(convertAddress, prefix, 10); }
            //            catch { }
            //        }
            //    }
            //    #endregion
            //    #region バッファメモリ
            //    foreach (string prefix in BufDevice.Prefixs)
            //    {
            //        if (convertAddress.IndexOf(prefix) == 0)
            //        {
            //            try { return new BufDevice(convertAddress, prefix, 10); }
            //            catch { }
            //        }
            //    }
            //    #endregion
            //    #region CC-LINKデバイス
            //    foreach (string prefix in CCLinkDevice.Prefixs)
            //    {
            //        if (convertAddress.IndexOf(prefix) == 0)
            //        {
            //            try { return new CCLinkDevice(convertAddress, prefix, 10); }
            //            catch { }
            //        }
            //    }
            //    #endregion
            //}
            //else
            //{
                string prefix = GetPrefix(convertAddress);
                if (ElementCreaterDic.ContainsKey(prefix))
                {
                    return ElementCreaterDic[prefix].Invoke(convertAddress);
                }
            //}

            // 不正アドレス
            return null;
        }

        static string GetPrefix(string target)
        {
            if (target.StartsWith("KM") || target.StartsWith("ZR"))
            {
                return target.Substring(0, 2);
            }
            else
            {
                return target.Substring(0, 1);
            }
        }

        #region BitDevice
        private class BitDevice : DeviceElement
        {
            #region 接頭文字定義
            private static string[] prefixsFix = new string[] { "X", "Y", "B" };
            private static string[] prefixsDec = new string[] { "M", "L", "S" };

            public static IEnumerable<string> PrefixsFix
            {
                get { return prefixsFix; }
            }

            public static IEnumerable<string> PrefixsDec
            {
                get { return prefixsDec; }
            }
            #endregion

            public BitDevice(string address, string prefix, int number)
            {
                // タイプ
                this.DeviceType = CommonClassLibrary.DeviceType.BitDevice;

                // 接頭文字
                this.Prefix = prefix;

                // 基数
                this.BaseNumber = number;

                // アドレス
                this.AddressOffset = Convert.ToInt32(address.Substring(prefix.Length), number);
            }

            public override string ToString()
            {
                string resStr = "";

                // 接頭文字
                resStr += this.Prefix;

                // アドレス
                resStr += Convert.ToString(this.AddressOffset, this.BaseNumber).ToUpper();

                return resStr;
            }
        }
        #endregion
        #region WordDevice
        private class WordDevice : CommonClassLibrary.DeviceElement
        {
            #region 接頭文字定義
            private static string[] prefixsDec = new string[] { "D", "ZR" };
            private static string[] prefixsFix = new string[] { "W" };

            public static IEnumerable<string> PrefixsDec
            {
                get { return prefixsDec; }
            }

            public static IEnumerable<string> PrefixsFix
            {
                get { return prefixsFix; }
            }
            #endregion

            int bitOffset = -1;
            public override int BitOffset
            {
                get { return bitOffset; }

                protected
                set { bitOffset = value; }
            }

            public WordDevice(string address, string prefix, int number)
            {
                this.DeviceType = CommonClassLibrary.DeviceType.WordDevice;

                this.Prefix = prefix;
                this.BaseNumber = number;

                int commaIdx = address.IndexOf('.');
                if (commaIdx == -1)
                {
                    this.AddressOffset = Convert.ToInt32(address.Substring(prefix.Length), number);
                }
                else
                {
                    int AddrLen = commaIdx - prefix.Length;
                    this.AddressOffset = Convert.ToInt32(address.Substring(prefix.Length, AddrLen), number);
                    this.BitOffset = Convert.ToInt32(address.Substring(commaIdx + 1), 10);

                    if (this.BitOffset >= 0x10)
                    {
                        this.AddressOffset += (this.BitOffset >> 4);   // _BitNum ÷ 16
                        this.BitOffset = (this.BitOffset & 0x0F);     // _BitNum Mob 16
                    }
                }
            }

            public override string ToString()
            {
                string resStr = "";

                // 接頭文字
                resStr += this.Prefix;

                // アドレス
                resStr += Convert.ToString(this.AddressOffset, this.BaseNumber).ToUpper();

                // ビット指定
                if (this.BitOffset != -1)
                    resStr += "." + Convert.ToString(this.BitOffset, 10);

                return resStr;
            }
        }
        #endregion
        #region BufDevice
        private class BufDevice : CommonClassLibrary.DeviceElement
        {
            #region 接頭文字定義
            private static string[] prefixs = new string[] { "U" };

            public static IEnumerable<string> Prefixs
            {
                get { return prefixs; }
            }
            #endregion

            int bitOffset = -1;
            public override int BitOffset
            {
                get { return bitOffset; }

                protected
                set { bitOffset = value; }
            }

            int unitNumber = -1;
            public override int UnitNumber
            {
                get { return unitNumber; }

                protected
                set { unitNumber = value; }
            }

            public BufDevice(string address, string prefix, int number)
            {
                this.DeviceType = CommonClassLibrary.DeviceType.BufDevice;

                this.Prefix = prefix;
                this.BaseNumber = number;

                int G_Idx = address.IndexOf('G');
                int commaIdx = address.IndexOf('.');

                this.UnitNumber = Convert.ToInt32(address.Substring(prefix.Length, G_Idx - prefix.Length), 16);

                if (commaIdx == -1)
                {
                    this.AddressOffset = Convert.ToInt32(address.Substring(G_Idx + 1), 10);
                }
                else
                {
                    int AddrLen = commaIdx - (G_Idx + 1);
                    this.AddressOffset = Convert.ToInt32(address.Substring(G_Idx + 1, commaIdx - (G_Idx + 1)), 10);
                    this.BitOffset = Convert.ToInt32(address.Substring(commaIdx + 1), 10);

                    if (this.BitOffset >= 0x10)
                    {
                        this.AddressOffset += (this.BitOffset >> 4);
                        this.BitOffset = (this.BitOffset & 0x0F);
                    }
                }
            }

            public override string ToString()
            {
                string resStr = "";

                // 接頭文字
                resStr += this.Prefix;

                // ユニット番号
                resStr += Convert.ToString(this.UnitNumber, 16).ToUpper();

                // アドレス
                resStr += "G" + Convert.ToString(this.AddressOffset, this.BaseNumber).ToUpper();

                // ビット指定
                if (this.BitOffset != -1)
                    resStr += "." + Convert.ToString(this.BitOffset, 10);

                return resStr;
            }
        }
        #endregion
        #region VirtualDevice
        private class VirtualDevice : CommonClassLibrary.DeviceElement
        {
            #region 接頭文字定義
            private static string[] prefixs = new string[] { "KM" };

            public static IEnumerable<string> Prefixs
            {
                get { return prefixs; }
            }
            #endregion

            int bitOffset = -1;
            public override int BitOffset
            {
                get { return bitOffset; }

                protected
                set { bitOffset = value; }
            }

            public VirtualDevice(string address, string prefix, int number)
            {
                this.DeviceType = CommonClassLibrary.DeviceType.VirtualDevice;

                this.Prefix = prefix;
                this.BaseNumber = number;

                int commaIdx = address.IndexOf('.');
                if (commaIdx == -1)
                {
                    this.AddressOffset = Convert.ToInt32(address.Substring(prefix.Length), number);
                }
                else
                {
                    int AddrLen = commaIdx - prefix.Length;
                    this.AddressOffset = Convert.ToInt32(address.Substring(prefix.Length, AddrLen), number);
                    this.BitOffset = Convert.ToInt32(address.Substring(commaIdx + 1), 10);

                    if (this.BitOffset >= 0x10)
                    {
                        this.AddressOffset += (this.BitOffset >> 4);     // _BitNum ÷ 16
                        this.BitOffset = (this.BitOffset & 0x0F);     // _BitNum Mob 16
                    }
                }
            }

            public override string ToString()
            {
                string resStr = "";

                // 接頭文字
                resStr += this.Prefix;

                // アドレス
                resStr += Convert.ToString(this.AddressOffset, this.BaseNumber).ToUpper();

                // ビット指定
                if (this.BitOffset != -1)
                    resStr += "." + Convert.ToString(this.BitOffset, 10);

                return resStr;
            }

        }
        #endregion
        #region CC-LINKデバイス
        private class CCLinkDevice : CommonClassLibrary.DeviceElement
        {
            #region 接頭文字定義
            private static string[] prefixs = new string[] { "C" };

            public static IEnumerable<string> Prefixs
            {
                get { return prefixs; }
            }
            #endregion

            int _BitOffset = -1;
            public override int BitOffset
            {
                get { return _BitOffset; }

                protected
                set { _BitOffset = value; }
            }

            int _UnitNumber = -1;
            public override int UnitNumber
            {
                get { return _UnitNumber; }

                protected
                set { _UnitNumber = value; }
            }

            int _SlaveNumber;
            bool _IsInput;

            public CCLinkDevice(string address, string prefix, int number)
            {
                this.DeviceType = CommonClassLibrary.DeviceType.BufDevice;

                this.Prefix = prefix;
                this.BaseNumber = number;

                Regex reg = new Regex("C(.+)S(.+)([IO])(.+)");
                Match match = reg.Match(address.ToUpper());

                this.UnitNumber = int.Parse(match.Groups[1].Value, NumberStyles.HexNumber);

                int slaveNumber = int.Parse(match.Groups[2].Value, NumberStyles.HexNumber);
                this._SlaveNumber = slaveNumber;

                bool isInput = (match.Groups[3].Value == "I");
                this._IsInput = isInput;

                if (isInput)
                {
                    int ioNum = int.Parse(match.Groups[4].Value, NumberStyles.HexNumber);
                    int offset = 0xE0 + (2 * (slaveNumber - 1));

                    this.AddressOffset = offset + (ioNum >= 0x10 ? 1 : 0);
                    this.BitOffset = ioNum & 0x0F;
                }
                else
                {
                    int ioNum = int.Parse(match.Groups[4].Value, NumberStyles.HexNumber);
                    int offset = 0x160 + (2 * (slaveNumber - 1));

                    this.AddressOffset = offset + (ioNum >= 0x10 ? 1 : 0);
                    this.BitOffset = ioNum & 0x0F;
                }
            }

            public override string ToString()
            {
                string resStr;

                int ioNum;

                ioNum = this.BitOffset;

                // I10(O10)以上が指定された
                if (this.AddressOffset % 2 == 1)
                {
                    ioNum += 0x10;
                }

                resStr = string.Format("C{0:X2}S{1:X2}{2}{3:X2}",
                    this.UnitNumber,
                    this._SlaveNumber,
                    (this._IsInput) ? 'I' : 'O',
                    ioNum);

                return resStr;
            }
        }
        #endregion
    }
}
