using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Skyrim;

public partial class Armor
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x0000_0004,
        Shield = 0x0000_0040
    }
    
    IFormLinkNullableGetter<IEffectRecordGetter> IEnchantableGetter.ObjectEffect => this.ObjectEffect;
}

partial class ArmorBinaryCreateTranslation
{
    public static partial void FillBinaryBodyTemplateCustom(MutagenFrame frame, IArmorInternal item, PreviousParse lastParsed)
    {
        item.BodyTemplate = BodyTemplateBinaryCreateTranslation.Parse(frame);
    }
}

partial class ArmorBinaryWriteTranslation
{
    public static partial void WriteBinaryBodyTemplateCustom(MutagenWriter writer, IArmorGetter item)
    {
        if (item.BodyTemplate is { } templ)
        {
            BodyTemplateBinaryWriteTranslation.Write(writer, templ);
        }
    }
}

partial class ArmorBinaryOverlay
{
    private int? _BodyTemplateLocation;
    public partial IBodyTemplateGetter? GetBodyTemplateCustom() => _BodyTemplateLocation.HasValue ? BodyTemplateBinaryOverlay.CustomFactory(new OverlayStream(_recordData.Slice(_BodyTemplateLocation!.Value), _package), _package) : default;
    public bool BodyTemplate_IsSet => _BodyTemplateLocation.HasValue;

    partial void BodyTemplateCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        _BodyTemplateLocation = (stream.Position - offset);
    }
}