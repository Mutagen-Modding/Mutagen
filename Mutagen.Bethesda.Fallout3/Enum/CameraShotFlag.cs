namespace Mutagen.Bethesda.Fallout3;

[Flags]
public enum CameraShotFlag : uint
{
    PositionFollowsLocation = 0x01,
    RotationFollowsTarget = 0x02,
    DontFollowBone = 0x04,
    FirstPersonCamera = 0x08,
    NoTracer = 0x10,
    StartAtTimeZero = 0x20,
}
