using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class FormLinkOrAliasTranslationGeneration : FormLinkBinaryTranslationGeneration
{
    public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor,
        Accessor packageAccessor)
    {
        FormLinkType linkType = typeGen as FormLinkType;
        return $"new {linkType.DirectTypeName(getter: true, internalInterface: true)}(this, FormKey.Factory({packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.MasterReferences)}!, BinaryPrimitives.ReadUInt32LittleEndian({dataAccessor})))";
    }
}