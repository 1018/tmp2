using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MITSUBISHI_PLC
{
    public enum CommunicationType
    {
        [Name("ｼﾐｭﾚｰﾀ")]
        Simulator,
        [Name("Ethernet")]
        Ethernet,
        [Name("RS-232C")]
        RS_232C,
        [Name("USB")]
        USB,
    }
}
