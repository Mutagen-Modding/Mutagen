using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class AssetLinkBinaryTranslationGeneration : StringBinaryTranslationGeneration
{
    public override Accessor AccessorTransform(TypeGeneration typeGen, Accessor a)
    {
        return $"{a}{typeGen.NullChar}.{nameof(IAssetLink.RawPath)}";
    }
 
    public override bool AllowDirectParse( 
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        bool squashedRepeatedList)
    {
        return false;
    }

    public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        return false;
    }

    public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor,
        Accessor packageAccessor)
    {
        AssetLinkType at = typeGen as AssetLinkType;
        return
            $"new AssetLinkGetter<{at.AssetTypeString}>({base.GenerateForTypicalWrapper(objGen, typeGen, dataAccessor, packageAccessor)})";
    }

    public override async Task GenerateCopyIn(
        StructuredStringBuilder fg,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor frameAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
        if (!typeGen.Nullable)
        {
            await base.GenerateCopyIn(fg, objGen, typeGen, frameAccessor, itemAccessor, errorMaskAccessor, translationMaskAccessor);
            return;
        }
        
        AssetLinkType asset = typeGen as AssetLinkType;
        var data = typeGen.GetFieldData();
        if (data.HasTrigger)
        {
            fg.AppendLine(
                $"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
        }

        List<string> extraArgs = new List<string>();
        extraArgs.Add($"reader: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : null)}");
        if (asset.Translated.HasValue)
        {
            extraArgs.Add($"source: {nameof(StringsSource)}.{asset.Translated.Value}");
        }

        TranslationGeneration.WrapParseCall(
            new TranslationWrapParseArgs()
            {
                FG = fg,
                TypeGen = typeGen,
                TranslatorLine = $"{this.NamespacePrefix}AssetLinkBinaryTranslation.Instance",
                MaskAccessor = errorMaskAccessor,
                ItemAccessor = itemAccessor,
                TranslationMaskAccessor = null,
                IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                ExtraArgs = extraArgs.ToArray(),
                SkipErrorMask = !this.DoErrorMasks,
                Generic = asset.AssetTypeString
            });
    }

    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor structDataAccessor,
        Accessor recordDataAccessor,
        int? currentPosition,
        string passedLengthAccessor,
        DataType? dataType = null)
    {
        AssetLinkType asset = typeGen as AssetLinkType; 
        var data = asset.GetFieldData(); 
        if (data.HasTrigger) 
        { 
            await base.GenerateWrapperFields(sb, objGen, typeGen, structDataAccessor, recordDataAccessor, currentPosition, passedLengthAccessor, dataType); 
            return; 
        } 
        switch (asset.BinaryType) 
        { 
            case StringBinaryType.NullTerminate: 
                sb.AppendLine($"public {typeGen.TypeName(getter: true)}{asset.NullChar} {typeGen.Name} {{ get; private set; }} = null!;"); 
                break; 
            default: 
                await base.GenerateWrapperFields(sb, objGen, typeGen, structDataAccessor, recordDataAccessor, currentPosition, passedLengthAccessor, dataType); 
                return; 
        } 
    } 
 
    public override void GenerateCopyInRet( 
        StructuredStringBuilder fg, 
        ObjectGeneration objGen, 
        TypeGeneration targetGen, 
        TypeGeneration typeGen, 
        Accessor nodeAccessor, 
        AsyncMode asyncMode, 
        Accessor retAccessor, 
        Accessor outItemAccessor, 
        Accessor errorMaskAccessor, 
        Accessor translationMaskAccessor, 
        Accessor converterAccessor, 
        bool inline) 
    {
        if (inline) 
        { 
            fg.AppendLine($"transl: {this.GetTranslatorInstance(typeGen, getter: false)}.Parse"); 
            return; 
        } 
        if (asyncMode != AsyncMode.Off) throw new NotImplementedException(); 
        var assetType = typeGen as AssetLinkType; 
        var data = typeGen.GetFieldData(); 
        using (var args = fg.Call( 
                   $"{retAccessor}{this.NamespacePrefix}AssetLinkBinaryTranslation.Instance.Parse<{assetType.AssetTypeString}>")) 
        { 
            args.Add(nodeAccessor.Access); 
            if (this.DoErrorMasks) 
            { 
                args.Add($"errorMask: {errorMaskAccessor}"); 
            } 
            args.Add($"item: out {outItemAccessor}"); 
            if (data.Length.HasValue) 
            { 
                args.Add($"length: {data.Length.Value}"); 
            } 
            if (assetType.Translated.HasValue) 
            { 
                args.Add($"source: {nameof(StringsSource)}.{assetType.Translated.Value}"); 
            } 
        } 
    }

    public override async Task GenerateWrapperUnknownLengthParse(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
        int? passedLength, 
        string? passedLengthAccessor,
        DataType? data = null)
    {
        AssetLinkType asset = typeGen as AssetLinkType;
        switch (asset.BinaryType)
        {
            case StringBinaryType.NullTerminate:
                sb.AppendLine(
                    $"ret.{typeGen.Name} = new AssetLink<{asset.AssetTypeString}>({nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ParseUnknownLengthString)}(ret.{dataAccessor}.Slice({passedLengthAccessor}), package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Encodings)}.{nameof(EncodingBundle.NonTranslated)}));");
                sb.AppendLine(
                    $"ret.{typeGen.Name}EndingPos = {(passedLengthAccessor == null ? null : $"{passedLengthAccessor} + ")}{(asset.Translated == null ? $"ret.{AccessorTransform(typeGen, typeGen.Name)}.Length + 1" : "5")};");
                break;
            default:
                await base.GenerateWrapperUnknownLengthParse(sb, objGen, typeGen, dataAccessor, passedLength, passedLengthAccessor);
                break;
        }
    } 
}