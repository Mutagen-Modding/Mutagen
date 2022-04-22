using System;

namespace Mutagen.Bethesda.Fallout4;

partial class VisualEffect
{
    [Flags]
    public enum Flag
    {
        RotateToFaceTarget = 0x01,
        AttachToCamera = 0x02,
        InheritRotation = 0x04,
    }
}