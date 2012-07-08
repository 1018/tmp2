using System;
using System.Windows.Forms;

namespace CommonClassLibrary
{
    public interface IDeviceManager
    {
        DeviceElement ToElement(string address);
    }

    public enum DeviceType
    {
        None,           // 不明
        BitDevice,      // ビットデバイス
        WordDevice,     // ワードデバイス
        BufDevice,      // バッファデバイス
        VirtualDevice,  // 仮想デバイス
    }

    /// <summary>
    /// 文字列アドレスへの操作を行うクラス
    /// </summary>
    public static class DeviceManager
    {
        /// <summary>
        /// アドレス分解
        /// 
        /// 文字列として引き渡されたアドレスを要素に分解します。
        /// </summary>
        /// <param name="address">アドレス</param>
        /// <returns>要素</returns>
        public static DeviceElement ToElement(string address)
        {
            IDeviceManager Manager = Global.DeviceManager;
            if (Manager == null)
            {
                MessageBox.Show("メーカーを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return Manager.ToElement(address);
        }

        /// <summary>
        /// アドレスを正規化します。
        /// </summary>
        /// <param name="address">アドレス</param>
        /// <returns>正規化されたアドレス</returns>
        public static string ToRegex(string address)
        {
            DeviceElement Element = ToElement(address);
            if (Element == null)
            {
                throw new ArgumentException("アドレスが不正です。", "address");
            }
            return Element.ToString();
        }

        /// <summary>
        /// アドレスをオフセットします。
        /// </summary>
        /// <param name="address">アドレス</param>
        /// <param name="offset">オフセット</param>
        /// <returns>オフセットしたアドレス</returns>
        public static string Offset(string address, int offset)
        {
            if (String.IsNullOrEmpty(address)) { return null; }
            DeviceElement Element = ToElement(address);
            if (Element == null)
            {
                throw new ArgumentException("アドレスが不正です。", "address");
            }
            return Element.Offset(offset).ToString();
        }

        /// <summary>
        /// アドレスの正当性を判断します。
        /// </summary>
        /// <param name="address">アドレス</param>
        /// <returns>アドレスが正当ならtrue</returns>
        public static bool IsValidate(string address)
        {
            DeviceElement Element = ToElement(address);

            return Element != null;
        }
    }

    /// <summary>
    /// 要素に分解されたアドレスの基本クラス
    /// </summary>
    [Serializable]
    public abstract class DeviceElement
    {
        int baseNumber = 10;

        #region プロパティ

        // デバイスタイプ
        public virtual DeviceType DeviceType
        {
            get;
            protected set;
        }

        // 接頭文字
        public virtual string Prefix
        {
            get;
            protected set;
        }

        // アドレス
        public virtual int AddressOffset
        {
            get;
            protected set;
        }

        // アドレス表記(10進数 or 16進数)
        public virtual int BaseNumber
        {
            get
            {
                return baseNumber;
            }            
            protected set
            {
                if (value == 10 || value == 16)
                {
                    baseNumber = value;
                }
                else
                {
                    throw new ArgumentException("10 又は 16 を指定してください。", "value");
                }
            }
        }

        // ビット指定
        public virtual int BitOffset
        {
            get
            {
                return -1;
            }
            protected set
            {
            }
        }

        // ユニット番号
        public virtual int UnitNumber
        {
            get
            {
                return -1;
            }
            protected set
            {
            }
        }

        #endregion

        /// <summary>
        /// アドレスを文字列として取得します
        /// </summary>
        /// <returns>文字列化されたアドレス</returns>
        public override string ToString()
        {
            return Prefix + AddressOffset;
        }

        /// <summary>
        /// オフセットしたDeviceElementを取得します
        /// </summary>
        /// <param name="offset">オフセット量</param>
        /// <returns>オフセット後アドレス</returns>
        public virtual DeviceElement Offset(int offset)
        {
            DeviceElement baseAddress = (DeviceElement)this.MemberwiseClone();

            // ビットが使用されていなければ、
            // アドレス単位でオフセットする
            if (baseAddress.BitOffset == -1)
            {
                baseAddress.AddressOffset += offset;
            }
            // ビットが使用されていれば、
            // ビット単位でオフセットする
            else
            {
                baseAddress.BitOffset += offset;

                if (baseAddress.BitOffset >= 0)
                {
                    baseAddress.AddressOffset += (baseAddress.BitOffset >> 4);
                    baseAddress.BitOffset = (baseAddress.BitOffset & 0x0F);
                }
                else
                {
                    // －方向オフセット

                    while (baseAddress.BitOffset < 0)
                    {
                        baseAddress.AddressOffset -= 1;
                        baseAddress.BitOffset += 0x0F;
                    }
                }
            }

            if (baseAddress.AddressOffset < 0)
            {
                throw new InvalidOperationException("オフセットの結果、負のアドレスになりました。");
            }

            return baseAddress;
        }

        public static bool operator ==(DeviceElement a, DeviceElement b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a) == null || ((object)b) == null)
            {
                return false;
            }

            if ((a.Prefix == b.Prefix) &&
                (a.AddressOffset == b.AddressOffset) &&
                (a.BitOffset == b.BitOffset) &&
                (a.UnitNumber == b.UnitNumber) &&
                (a.DeviceType == b.DeviceType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(DeviceElement a, DeviceElement b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return (this == (DeviceElement)obj);
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}
