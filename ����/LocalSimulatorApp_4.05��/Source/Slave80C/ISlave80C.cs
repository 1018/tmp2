using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CommonClassLibrary;

namespace SymbolLibrary
{


    public class Position
    {
        public double mmX { get; set; }
        public double mmY { get; set; }
        public double mmZ { get; set; }
        public double mmT { get; set; }
    }

    public interface ISlave80C
    {
        Position BasePos{ get; }
        Position OffsetPos { get; }
        int SlaveNumber { get;}
        int UnitNumber { get; }
        int ModeNumber { get; }
        string TypeName { get;}
        bool GetWafer();
        bool PutWafer();
        bool InterfaceWafer { get; }        
    }
}
