using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transport3DClassLibrary
{
    public interface IChip
    {
        bool HasChip { get; set; }

        Transport3D_Draw.Point3DFormat ChipPoint { get; set; }

        Transport3D_Draw.Size3DFormat ChipSize { get; set; }
    }
}
