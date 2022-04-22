using Loqui; 
using Loqui.Generation; 
using Mutagen.Bethesda.Generation.Modules.Plugin; 
using Mutagen.Bethesda.Plugins.Binary.Overlay; 
using Mutagen.Bethesda.Plugins.Binary.Streams; 
using Mutagen.Bethesda.Plugins.Binary.Translations; 
using Mutagen.Bethesda.Plugins.Meta; 
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Generation.Fields; 
using StringType = Mutagen.Bethesda.Generation.Fields.StringType; 
 
namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class StringBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<string> 
{ 
    public override bool NeedsGenerics => false; 
 
    public override bool AllowDirectParse( 
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        bool squashedRepeatedList)
    {
        var str = typeGen as StringType;
        if (str.BinaryType != StringBinaryType.NullTerminate) return false;
        if (str.Translated.HasValue) return false;
        return !squashedRepeatedList;
    }

    public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        var str = typeGen as StringType;
        if (str.BinaryType != StringBinaryType.NullTerminate) return false;
        if (str.Translated.HasValue) return false;
        return true;
    }

    public StringBinaryTranslationGeneration()
        : base(nullable: true, expectedLen: null) 
    { 
    } 
 
    public override async Task GenerateWrite( 
        FileGeneration fg, 
        ObjectGeneration objGen, 
        TypeGeneration typeGen, 
        Accessor writerAccessor, 
        Accessor itemAccessor, 
        Accessor errorMaskAccessor, 
        Accessor translationMaskAccessor, 
        Accessor converterAccessor) 
    { 
        var stringType = typeGen as StringType; 
        var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData; 
        using (var args = new ArgsWrapper(fg, 
                   $"{this.NamespacePrefix}StringBinaryTranslation.Instance.Write{(typeGen.Nullable ? "Nullable" : null)}")) 
        { 
            args.Add($"writer: {writerAccessor}"); 
            args.Add($"item: {itemAccessor}"); 
            if (this.DoErrorMasks) 
            { 
                if (typeGen.HasIndex) 
                { 
                    args.Add($"fieldIndex: (int){typeGen.IndexEnumName}"); 
                } 
                args.Add($"errorMask: {errorMaskAccessor}"); 
            } 
            if (data.RecordType.HasValue) 
            { 
                args.Add($"header: translationParams.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})"); 
            } 
            else if (data.Length.HasValue) 
            { 
                args.Add($"length: {data.Length.Value}");  
            } 
            args.Add($"binaryType: {nameof(StringBinaryType)}.{stringType.BinaryType}"); 
            if (stringType.Translated.HasValue)
            {
                args.Add($"source: {nameof(StringsSource)}.{stringType.Translated.Value}");
            }
        }
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
        var str = typeGen as StringType; 
        var data = typeGen.GetFieldData(); 
        if (data.HasTrigger) 
        { 
            fg.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};"); 
        } 
 
        List<string> extraArgs = new List<string>(); 
        extraArgs.Add($"reader: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : null)}"); 
        if (str.Translated.HasValue)
        {
            extraArgs.Add($"source: {nameof(StringsSource)}.{str.Translated.Value}");
        }
        extraArgs.Add($"stringBinaryType: {nameof(StringBinaryType)}.{str.BinaryType}");
        switch (str.BinaryType)
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
 
        TranslationGeneration.WrapParseCall( 
            new TranslationWrapParseArgs() 
            { 
                FG = fg, 
                TypeGen = typeGen, 
                TranslatorLine = $"{this.NamespacePrefix}StringBinaryTranslation.Instance", 
                MaskAccessor = errorMaskAccessor, 
                ItemAccessor = itemAccessor, 
                TranslationMaskAccessor = null, 
                IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null, 
                ExtraArgs = extraArgs.ToArray(), 
                SkipErrorMask = !this.DoErrorMasks 
            }); 
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
        var stringType = typeGen as StringType; 
        var data = typeGen.GetFieldData(); 
        using (var args = new ArgsWrapper(fg, 
                   $"{retAccessor}{this.NamespacePrefix}StringBinaryTranslation.Instance.Parse")) 
        { 
            args.Add(nodeAccessor.Access); 
            if (this.DoErrorMasks) 
            { 
                args.Add($"errorMask: {errorMaskAccessor}"); 
            } 
            args.Add($"item: out {outItemAccessor}"); 
            args.Add($"parseWhole: {(data.HasTrigger ? "true" : "false")}"); 
            if (data.Length.HasValue) 
            { 
                args.Add($"length: {data.Length.Value}"); 
            } 
            args.Add($"binaryType: {nameof(StringBinaryType)}.{stringType.BinaryType}"); 
            if (stringType.Translated.HasValue) 
            { 
                args.Add($"source: {nameof(StringsSource)}.{stringType.Translated.Value}"); 
            } 
        } 
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
        StringType str = typeGen as StringType; 
        var data = str.GetFieldData(); 
        if (data.HasTrigger) 
        { 
            await base.GenerateWrapperFields(fg, objGen, typeGen, dataAccessor, currentPosition, passedLengthAccessor, dataType); 
            return; 
        } 
        switch (str.BinaryType) 
        { 
            case StringBinaryType.NullTerminate: 
                fg.AppendLine($"public {typeGen.TypeName(getter: true)}{str.NullChar} {typeGen.Name} {{ get; private set; }} = {(str.Translated.HasValue ? $"{nameof(TranslatedString)}.{nameof(TranslatedString.Empty)}" : "string.Empty")};"); 
                break; 
            default: 
                await base.GenerateWrapperFields(fg, objGen, typeGen, dataAccessor, currentPosition, passedLengthAccessor, dataType); 
                return; 
        } 
 
    } 
 
    public override string GenerateForTypicalWrapper( 
        ObjectGeneration objGen,  
        TypeGeneration typeGen,  
        Accessor dataAccessor, 
        Accessor packageAccessor) 
    { 
        StringType str = typeGen as StringType; 
        if (str.Translated.HasValue) 
        { 
            return $"StringBinaryTranslation.Instance.Parse({dataAccessor}, {nameof(StringsSource)}.{str.Translated.Value}, parsingBundle: {packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)})"; 
        } 
        else 
        { 
            switch (str.BinaryType) 
            { 
                case StringBinaryType.Plain: 
                case StringBinaryType.NullTerminate: 
                    var data = typeGen.GetFieldData(); 
                    var gendered = data.Parent as GenderedType; 
                    if (data.HasTrigger 
                        || (gendered?.MaleMarker.HasValue ?? false)) 
                    { 
                        return $"{nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ProcessWholeToZString)}({dataAccessor}, encoding: {packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Encodings)}.{nameof(EncodingBundle.NonTranslated)})"; 
                    } 
                    else 
                    { 
                        return $"{nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ParseUnknownLengthString)}({dataAccessor}, encoding: {packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Encodings)}.{nameof(EncodingBundle.NonTranslated)})"; 
                    } 
                case StringBinaryType.PrependLength: 
                    return $"{nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ParsePrependedString)}({dataAccessor}, lengthLength: 4, encoding: {packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Encodings)}.{nameof(EncodingBundle.NonTranslated)})"; 
                case StringBinaryType.PrependLengthUShort: 
                    return $"{nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ParsePrependedString)}({dataAccessor}, lengthLength: 2, encoding: {packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Encodings)}.{nameof(EncodingBundle.NonTranslated)})"; 
                default: 
                    throw new NotImplementedException(); 
            } 
        } 
    } 
 
    public override async Task GenerateWrapperUnknownLengthParse( 
        FileGeneration fg,  
        ObjectGeneration objGen, 
        TypeGeneration typeGen, 
        int? passedLength, 
        string? passedLengthAccessor) 
    { 
        StringType str = typeGen as StringType; 
        switch (str.BinaryType) 
        { 
            case StringBinaryType.PrependLength: 
                fg.AppendLine($"ret.{typeGen.Name}EndingPos = {(passedLengthAccessor == null ? null : $"{passedLengthAccessor} + ")}BinaryPrimitives.ReadInt32LittleEndian(ret._data{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}) + 4;"); 
                break; 
            case StringBinaryType.PrependLengthUShort: 
                fg.AppendLine($"ret.{typeGen.Name}EndingPos = {(passedLengthAccessor == null ? null : $"{passedLengthAccessor} + ")}BinaryPrimitives.ReadUInt16LittleEndian(ret._data{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}) + 2;"); 
                break; 
            case StringBinaryType.NullTerminate: 
                fg.AppendLine($"ret.{typeGen.Name} = {(str.Translated.HasValue ? $"({nameof(TranslatedString)})" : string.Empty)}{nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ParseUnknownLengthString)}(ret._data{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Encodings)}.{nameof(EncodingBundle.NonTranslated)});"); 
                fg.AppendLine($"ret.{typeGen.Name}EndingPos = {(passedLengthAccessor == null ? null : $"{passedLengthAccessor} + ")}{(str.Translated == null ? $"ret.{typeGen.Name}.Length + 1" : "5")};"); 
                break; 
            default: 
                if (typeGen.GetFieldData().Binary == BinaryGenerationType.Custom) return; 
                throw new NotImplementedException(); 
        } 
    } 
}