namespace Mutagen.Bethesda.Fallout76;

public partial class MaterialType
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag
    {
        StairMaterial = 0x01,
        ArrowsStick = 0x02,
        CanTunnel = 0x04,
    }
}

partial class MaterialTypeBinaryOverlay
{
    public Single Blue => 0f;
    public Single Green => 0f;
    public Single Red => 0f;
}
