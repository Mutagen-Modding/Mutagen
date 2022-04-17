using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

public partial class Activator
{
    [Flags]
    public enum MajorFlag
    {
        NeverFades = 0x0000_0002,
        NonOccluder = 0x0000_0004,
        HeadingMarker = 0x0000_0080,
        MustUpdateAnims = 0x0000_0100,
        HiddenFromLocalMap = 0x0000_0200,
        HeadtrackMarker = 0x0000_0400,
        UsedAsPlatform = 0x0000_0800,
        PackInUseOnly = 0x0000_1000,
        HasDistantLod = 0x0000_8000,
        RandomAnimStart = 0x0001_0000,
        Dangerous = 0x0002_0000,
        IgnoreObjectInteraction = 0x0010_0000,
        IsMarker = 0x0080_0000,
        Obstacle = 0x0200_0000,
        NavMeshGenerationFilter = 0x0400_0000,
        NavMeshGenerationBoundingBox = 0x0800_0000,
        ChildCanUse = 0x2000_0000,
        NavMeshGenerationGround = 0x4000_0000,
    }

    [Flags]
    public enum Flag
    {
        NoDisplacement = 0x01,
        IgnoredBySandbox = 0x02, 
        IsARadio = 0x10,
    }
}

internal partial class ActivatorBinaryOverlay
{
    public IReadOnlyList<IConditionGetter>? Conditions { get; private set; }

    partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        Conditions = ConditionBinaryOverlay.ConstructBinayOverlayCountedList(stream, _package);
    }
}