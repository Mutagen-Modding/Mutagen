using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class ModKeyBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<ModKey>
{
    public override bool NeedsGenerics => false;

    public ModKeyBinaryTranslationGeneration()
        : base(expectedLen: null, typeName: null, nullable: null)
    {
        PreferDirectTranslation = false;
    }

    public override string GenerateForTypicalWrapper(
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        Accessor dataAccessor,
        Accessor packageAccessor)
    {
        return $"{nameof(ModKey)}.{nameof(ModKey.FromNameAndExtension)}({nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ProcessWholeToZString)}({dataAccessor}, {packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Encodings)}.{nameof(EncodingBundle.NonTranslated)}))";
    }
}