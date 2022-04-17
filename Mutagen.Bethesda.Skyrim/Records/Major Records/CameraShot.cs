using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim;

public partial class CameraShot
{
    public enum ActionType
    { 
        Shoot,
        Fly,
        Hit,
        Zoom,
    }

    public enum LocationType
    {
        Attacker,
        Projectile,
        Target,
        LeadActor,
    }

    public enum TargetType
    { 
        Attacker,
        Projectile,
        Target,
        LeadActor,
    }

    [Flags]
    public enum Flag
    {
        PositionFollowsLocation = 0x01,
        RotationFollowsTarget = 0x02,
        DoNotFollowBone = 0x04,
        FirstPersonCamera = 0x08,
        NoTracer = 0x10,
        StartAtTimeZero = 0x20
    }
}