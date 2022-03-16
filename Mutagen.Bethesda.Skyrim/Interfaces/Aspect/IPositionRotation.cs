using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public interface IPositionRotation : IPositionRotationGetter
    {
        new P3Float Position { get; set; }
        new P3Float Rotation { get; set; }
    }

    public interface IPositionRotationGetter
    {
        P3Float Position { get; }
        P3Float Rotation { get; }
    }
}
