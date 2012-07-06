using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transport3DClassLibrary
{

    public enum DirectionEnum { X, Y, Z};

    public interface IConveyor
    {
        double CvSpeed { get; set; }

        DirectionEnum CvDirection { get; set; }

        int CvRun { get; set; }

    }
}
