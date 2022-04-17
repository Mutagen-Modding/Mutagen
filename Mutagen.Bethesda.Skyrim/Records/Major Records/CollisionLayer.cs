using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim;

public partial class CollisionLayer
{
    [Flags]
    public enum Flag
    {
        TriggerVolume = 0x01,
        Sensor = 0x02,
        NavmeshObstacle = 0x04,
    }
}