namespace Mutagen.Bethesda.Skyrim;

public partial class GlobalFloat
{
    public const char TRIGGER_CHAR = 'f';
    public override char TypeChar => TRIGGER_CHAR;

    public override float? RawFloat
    {
        get => this.Data;
        set => this.Data = value;
    }
}

partial class GlobalFloatBinaryOverlay
{
    public override char TypeChar => GlobalFloat.TRIGGER_CHAR;
    public override float? RawFloat => this.Data;
}