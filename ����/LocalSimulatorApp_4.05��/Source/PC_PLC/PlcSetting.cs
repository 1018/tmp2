using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonClassLibrary;

namespace PC_PLC
{
    public class PlcSetting : IPlcSetting
    {

        #region IPlcSetting メンバ

        public ConnectionMode FormShow(ref object[] setting)
        {
            //設定不要の為処理なし
            return ConnectionMode.Normal;
        }

        #endregion
    }
}
