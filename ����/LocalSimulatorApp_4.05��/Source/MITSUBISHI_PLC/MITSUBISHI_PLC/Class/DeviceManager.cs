using System;
using CommonClassLibrary;
using System.Collections.Generic;

namespace MelsecDeviceManager
{
    public class DeviceManager : CommonClassLibrary.IDeviceManager
    {
        DeviceElement IDeviceManager.ToElement(string address)
        {
            // nullチェック
            if (string.IsNullOrEmpty(address)) { return null; }

            // 大文字変換
            string convertAddress = address.ToUpper();

            // タイプ判断
            #region 16進ビットデバイス
            foreach (string prefix in BitDevice.PrefixsFix)
            {
                if (convertAddress.IndexOf(prefix) == 0)
                {
                    try { return new BitDevice(convertAddress, prefix, 16); }
                    catch { }
                }
            }
            #endregion
            #region 10進ビットデバイス
            foreach (string prefix in BitDevice.PrefixsDec)
            {
                if (convertAddress.IndexOf(prefix) == 0)
                {
                    try { return new BitDevice(convertAddress, prefix, 10); }
                    catch { }
                }
            }
            #endregion
            #region 16進ワードデバイス
            foreach (string prefix in WordDevice.PrefixsFix)
            {
                if (convertAddress.IndexOf(prefix) == 0)
                {
                    try { return new WordDevice(convertAddress, prefix, 16); }
                    catch { }
                }
            }
            #endregion
            #region 10進ワードデバイス
            foreach (string prefix in WordDevice.PrefixsDec)
            {
                if (convertAddress.IndexOf(prefix) == 0)
                {
                    try { return new WordDevice(convertAddress, prefix, 10); }
                    catch { }
                }
            }
            #endregion
            #region 仮想デバイス
            foreach (string prefix in VirtualDevice.Prefixs)
            {
                if (convertAddress.IndexOf(prefix) == 0)
                {
                    try { return new VirtualDevice(convertAddress, prefix, 10); }
                    catch { }
                }
            }
            #endregion
            #region バッファメモリ
            foreach (string prefix in BufDevice.Prefixs)
            {
                if (convertAddress.IndexOf(prefix) == 0)
                {
                    try { return new BufDevice(convertAddress, prefix, 10); }
                    catch { }
                }
            }
            #endregion

            // 不正アドレス
            return null;
        }

        #region BitDevice
        private class BitDevice : DeviceElement
        {
            #region 接頭文字定義
            private static string[] prefixsFix = new string[] { "X", "Y" ,"B" };
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
                get { return prefixsFix;}
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
    }
}
