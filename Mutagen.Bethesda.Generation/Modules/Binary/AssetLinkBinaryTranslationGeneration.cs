using Loqui.Generation;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Generation.Fields;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class AssetLinkBinaryTranslationGeneration : StringBinaryTranslationGeneration
{
    public override Accessor AccessorTransform(Accessor a)
    {
        return $"{a}.{nameof(IAssetLink.RawPath)}";
    }

    public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor,
        Accessor packageAccessor)
    {
        AssetLinkType at = typeGen as AssetLinkType;
        return
            $"new AssetLinkGetter<{at.AssetTypeString}>({at.AssetTypeString}.Instance, {base.GenerateForTypicalWrapper(objGen, typeGen, dataAccessor, packageAccessor)})";
    }
}