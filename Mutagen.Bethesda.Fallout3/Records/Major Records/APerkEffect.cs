using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout3;

// Non-generated property to store embedded script subrecords (SCHR, SCDA, SCTX, SLSD, SCVR, SCRO, SCRV)
// These belong inside each perk effect per xEdit, before the PRKF end marker
public partial class APerkEffect
{
    public List<(RecordType Type, byte[] Data)>? EmbeddedScriptSubrecords { get; set; }
}

partial class APerkEffectBinaryOverlay
{
    public byte Rank => throw new NotImplementedException();

    public byte Priority => throw new NotImplementedException();

    public IReadOnlyList<IPerkConditionGetter> Conditions => throw new NotImplementedException();
    public string? ButtonLabel => throw new NotImplementedException();
    public IPerkScriptFlagGetter Flags => throw new NotImplementedException();
}

partial class APerkEffectBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryFunctionParametersCustom(MutagenFrame frame, IAPerkEffect item, PreviousParse lastParsed)
    {
        return lastParsed;
    }
}

partial class APerkEffectBinaryWriteTranslation
{
    public static partial void WriteBinaryFunctionParametersCustom(MutagenWriter writer, IAPerkEffectGetter item)
    {
    }
}
