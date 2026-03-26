namespace Mutagen.Bethesda.Fallout76;

public partial class CollisionLayer
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag
    {
        TriggerVolume = 0x01,
        Sensor = 0x02,
        NavmeshObstacle = 0x04,
    }
}

partial class CollisionLayerBinaryOverlay
{
    public UInt32 Colors => 0;
}
