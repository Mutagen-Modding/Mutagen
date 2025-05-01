using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Skyrim;

partial class APerkEffectBinaryOverlay
{
    public byte Rank => throw new NotImplementedException();

    public byte Priority => throw new NotImplementedException();

    public IReadOnlyList<IPerkConditionGetter> Conditions => throw new NotImplementedException();
    public ITranslatedStringGetter? ButtonLabel => throw new NotImplementedException();
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

partial class APerkEffectBinaryOverlay
{
}