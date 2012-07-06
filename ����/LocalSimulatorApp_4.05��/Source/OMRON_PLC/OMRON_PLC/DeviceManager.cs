using System;
using CommonClassLibrary;

namespace OMRON_PLC
{
    [Serializable]
    public class DeviceManager : IDeviceManager
    {
        #region IDeviceManager メンバ

        public DeviceElement ToElement(string address)
        {
            // nullチェック
            if (string.IsNullOrEmpty(address))
            {
                return null;
            }

            // 大文字変換
            string Address = address.ToUpper();

            // タイプ判断
            try
            {
                #region 10進ワードデバイス
                foreach (string prefix in WordDevice.Prefixs)
                {
                    if (Address.IndexOf(prefix) == 0)
                    {
                        try { return new WordDevice(Address, prefix, 10); }
                        catch { }
                    }
                }
                #endregion
                #region 仮想デバイス
                foreach (string prefix in VirtualDevice.Prefixs)
                {
                    if (Address.IndexOf(prefix) == 0)
                    {
                        try { return new VirtualDevice(Address, prefix, 10); }
                        catch { }
                    }
                }
                #endregion

                #region CIO(数字のみ指定) 最後に行うこと
                try
                {
                    return new WordDevice(Address, "CIO", 10);
                }
                catch
                {
                }
                #endregion
            }
            catch
            {
            }

            // 不正アドレス
            return null;
        }

        #endregion

        #region WordDevice
        private class WordDevice : DeviceElement
        {
            #region 接頭文字定義
            static private string[] _Prefixs = new string[] { "CIO", "DM", "D", "E0" , "W" , "H" };

            static public string[] Prefixs
            {
                get { return (string[])_Prefixs.Clone(); }
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
                this.DeviceType = DeviceType.WordDevice;

                this.Prefix = prefix;
                this.BaseNumber = number;

                // "D" は "DM" として扱う
                if (this.Prefix == "D")
                {
                    this.Prefix = "DM";
                }


                int PrefixLen = (address.IndexOf(prefix) != -1 ? prefix.Length : 0);

                int commaIdx = address.IndexOf('.');
                if (commaIdx == -1)
                {
                    this.AddressOffset = Convert.ToInt32(address.Substring(PrefixLen), number);
                }
                else
                {
                    int AddrLen = commaIdx - PrefixLen;
                    this.AddressOffset = Convert.ToInt32(address.Substring(PrefixLen, AddrLen), number);
                    this.BitOffset = Convert.ToInt32(address.Substring(commaIdx + 1), 10);

                    if (this.BitOffset >= 0x10)
                    {
                        this.AddressOffset += (this.BitOffset >> 4);  // _BitNum ÷ 16
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

        #region VirtualDevice
        private class VirtualDevice : CommonClassLibrary.DeviceElement
        {
            #region 接頭文字定義
            static private string[] _Prefixs = new string[] { "KM" };

            static public string[] Prefixs
            {
                get { return (string[])_Prefixs.Clone(); }
            }
            #endregion

            int _bitOffset = -1;
            public override int BitOffset
            {
                get { return _bitOffset; }

                protected
                set { _bitOffset = value; }
            }

            public VirtualDevice(string address, string prefix, int number)
            {
                this.DeviceType = DeviceType.VirtualDevice;

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
                        this.BitOffset = (this.BitOffset & 0x0F);   // _BitNum Mob 16
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
