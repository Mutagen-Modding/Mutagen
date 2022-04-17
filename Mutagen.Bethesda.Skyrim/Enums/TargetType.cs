using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim;

public enum TargetType
{
    Self = 0,
    Touch = 1,
    Aimed = 2,
    TargetActor = 3,
    TargetLocation = 4,
}