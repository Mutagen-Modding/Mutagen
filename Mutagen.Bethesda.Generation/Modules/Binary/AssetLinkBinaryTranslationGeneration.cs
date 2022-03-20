using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;

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
            $"new AssetLinkGetter<{at.AssetTypeString}>({at.AssetTypeString}.Instance, {base.GenerateForTypicalWrapper(objGen, typeGen, dataAccessor, packageAccessor)})";
    }

    public override async Task GenerateCopyIn(
        FileGeneration fg,
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

        extraArgs.Add($"stringBinaryType: {nameof(StringBinaryType)}.{asset.BinaryType}");
        switch (asset.BinaryType)
        {
            case StringBinaryType.NullTerminate:
                if (!data.HasTrigger)
                {
                    extraArgs.Add("parseWhole: false");
                }

                break;
            default:
                break;
        }
        
        extraArgs.Add($"assetType: {asset.AssetTypeString}.Instance");

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
                SkipErrorMask = !this.DoErrorMasks
            });
    }
 
    public override async Task GenerateWrapperFields( 
        FileGeneration fg, 
        ObjectGeneration objGen,  
        TypeGeneration typeGen,  
        Accessor dataAccessor,  
        int? currentPosition,  
        string passedLengthAccessor,  
        DataType dataType = null) 
    { 
        AssetLinkType asset = typeGen as AssetLinkType; 
        var data = asset.GetFieldData(); 
        if (data.HasTrigger) 
        { 
            await base.GenerateWrapperFields(fg, objGen, typeGen, dataAccessor, currentPosition, passedLengthAccessor, dataType); 
            return; 
        } 
        switch (asset.BinaryType) 
        { 
            case StringBinaryType.NullTerminate: 
                fg.AppendLine($"public {typeGen.TypeName(getter: true)}{asset.NullChar} {typeGen.Name} {{ get; private set; }} = null!;"); 
                break; 
            default: 
                await base.GenerateWrapperFields(fg, objGen, typeGen, dataAccessor, currentPosition, passedLengthAccessor, dataType); 
                return; 
        } 
 
    } 
 
    public override void GenerateCopyInRet( 
        FileGeneration fg, 
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
        using (var args = new ArgsWrapper(fg, 
                   $"{retAccessor}{this.NamespacePrefix}AssetLinkBinaryTranslation.Instance.Parse")) 
        { 
            args.Add(nodeAccessor.Access); 
            if (this.DoErrorMasks) 
            { 
                args.Add($"errorMask: {errorMaskAccessor}"); 
            } 
            args.Add($"item: out {outItemAccessor}"); 
            args.Add($"assetType: {assetType.AssetTypeString}.Instance");
            args.Add($"parseWhole: {(data.HasTrigger ? "true" : "false")}"); 
            if (data.Length.HasValue) 
            { 
                args.Add($"length: {data.Length.Value}"); 
            } 
            args.Add($"binaryType: {nameof(StringBinaryType)}.{assetType.BinaryType}"); 
            if (assetType.Translated.HasValue) 
            { 
                args.Add($"source: {nameof(StringsSource)}.{assetType.Translated.Value}"); 
            } 
        } 
    }

    public override async Task GenerateWrapperUnknownLengthParse(
        FileGeneration fg,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        int? passedLength,
        string passedLengthAccessor)
    {
        AssetLinkType asset = typeGen as AssetLinkType;
        switch (asset.BinaryType)
        {
            case StringBinaryType.NullTerminate:
                fg.AppendLine(
                    $"ret.{typeGen.Name} = new AssetLink<{asset.AssetTypeString}>({asset.AssetTypeString}.Instance, {nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ParseUnknownLengthString)}(ret._data.Slice({passedLengthAccessor}), package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Encodings)}.{nameof(EncodingBundle.NonTranslated)}));");
                fg.AppendLine(
                    $"ret.{typeGen.Name}EndingPos = {(passedLengthAccessor == null ? null : $"{passedLengthAccessor} + ")}{(asset.Translated == null ? $"ret.{AccessorTransform(typeGen, typeGen.Name)}.Length + 1" : "5")};");
                break;
            default:
                await base.GenerateWrapperUnknownLengthParse(fg, objGen, typeGen, passedLength, passedLengthAccessor);
                break;
        }
    } 
}