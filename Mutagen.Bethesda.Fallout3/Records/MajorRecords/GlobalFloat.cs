namespace Mutagen.Bethesda.Fallout3;

public partial class GlobalFloat
{
    public const char TRIGGER_CHAR = 'f';
    char IGlobalGetter.TypeChar => TRIGGER_CHAR;

    public override float? RawFloat
    {
        get => this.Data;
        set => this.Data = value;
    }
}

internal partial class GlobalFloatBinaryOverlay
{
    char IGlobalGetter.TypeChar => GlobalFloat.TRIGGER_CHAR;
    public override float? RawFloat => this.Data;
}