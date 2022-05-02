using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class CustomLogicTranslationGeneration : BinaryTranslationGeneration
{
    public override bool DoErrorMasks => true;

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        throw new NotImplementedException();
    }

    public override bool ShouldGenerateWrite(TypeGeneration typeGen)
    {
        return true;
    }

    public override bool ShouldGenerateCopyIn(TypeGeneration typeGen)
    {
        return true;
    }

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor readerAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
        GenerateFill(
            sb: sb,
            objGen: objGen,
            field: typeGen,
            frameAccessor: readerAccessor,
            isAsync: false,
            useReturnValue: true);
    }

    public override void GenerateCopyInRet(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration targetGen,
        TypeGeneration typeGen,
        Accessor readerAccessor,
        AsyncMode asyncMode,
        Accessor retAccessor,
        Accessor outItemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationAccessor,
        Accessor converterAccessor,
        bool inline)
    {
        throw new NotImplementedException();
    }

    public override async Task GenerateWrite(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor writerAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor,
        Accessor converterAccessor)
    {
        this.GenerateWrite(
            sb: sb,
            obj: objGen,
            field: typeGen,
            writerAccessor: writerAccessor);
    }

    public static void GenerateCreatePartialMethods(
        StructuredStringBuilder sb,
        ObjectGeneration obj,
        TypeGeneration field,
        bool isAsync,
        bool useReturnValue)
    {
        var fieldData = field.GetFieldData();
        var returningParseResult = useReturnValue && fieldData.HasTrigger;
        if (!isAsync)
        {
            using (var args = sb.Function(
                       $"public static partial {(returningParseResult ? nameof(ParseResult) : "void")} FillBinary{field.Name}Custom",
                       semiColon: true))
            {
                args.Add($"{nameof(MutagenFrame)} frame");
                args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                if (returningParseResult && obj.GetObjectType() == ObjectType.Subrecord)
                {
                    args.Add($"{nameof(PreviousParse)} lastParsed");
                }
            }
            sb.AppendLine();
        }
    }

    public static void GenerateWritePartialMethods(
        StructuredStringBuilder sb,
        ObjectGeneration obj,
        TypeGeneration field,
        bool isAsync)
    {
        using (var args = sb.Function(
                   $"public static partial void WriteBinary{field.Name}Custom{obj.GetGenericTypes(MaskType.Normal)}"))
        {
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
            args.SemiColon = true;
            args.Add($"{nameof(MutagenWriter)} writer");
            args.Add($"{obj.Interface(getter: true, internalInterface: true)} item");
        }
        sb.AppendLine();
        using (var args = sb.Function(
                   $"public static void WriteBinary{field.Name}{obj.GetGenericTypes(MaskType.Normal)}"))
        {
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
            args.Add($"{nameof(MutagenWriter)} writer");
            args.Add($"{obj.Interface(getter: true, internalInterface: true)} item");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"WriteBinary{field.Name}Custom"))
            {
                args.Add("writer: writer");
                args.Add("item: item");
            }
        }
        sb.AppendLine();
    }

    public void GenerateWrite(
        StructuredStringBuilder sb,
        ObjectGeneration obj,
        TypeGeneration field,
        Accessor writerAccessor)
    {
        using (var args = sb.Call(
                   $"{this.Module.TranslationWriteClass(obj)}.WriteBinary{field.Name}"))
        {
            args.Add($"writer: {writerAccessor}");
            args.Add("item: item");
        }
    }

    public void GenerateFill(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration field,
        Accessor frameAccessor,
        bool isAsync,
        bool useReturnValue)
    {
        var data = field.GetFieldData();
        var returningParseValue = useReturnValue && data.HasTrigger;
        using (var args = sb.Call(
                   $"{(returningParseValue ? "return " : null)}{Loqui.Generation.Utility.Await(isAsync)}{this.Module.TranslationCreateClass(field.ObjectGen)}.FillBinary{field.Name}Custom"))
        {
            args.Add($"frame: {(data.HasTrigger ? $"{frameAccessor}.SpawnWithLength(frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)} + contentLength)" : frameAccessor)}");
            args.AddPassArg("item");
            if (returningParseValue && objGen.GetObjectType() == ObjectType.Subrecord)
            {
                args.AddPassArg("lastParsed");
            }
        }
    }

    public async Task GenerateForCustomFlagWrapperFields(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor,
        int? currentPosition,
        string passedLenAccessor,
        DataType? dataType = null)
    {
        var fieldData = typeGen.GetFieldData();
        var gen = this.Module.GetTypeGeneration(typeGen.GetType());
        string loc;
        if (fieldData.HasTrigger)
        {
            using (var args = sb.Call(
                       $"partial void {typeGen.Name}CustomParse"))
            {
                args.Add($"{nameof(OverlayStream)} stream");
                args.Add($"long finalPos");
                args.Add($"int offset");
            }
            if (typeGen.Nullable && !typeGen.CanBeNullable(getter: true))
            {
                sb.AppendLine($"public bool {typeGen.Name}_IsSet => Get{typeGen.Name}IsSetCustom();");
            }
            loc = $"_{typeGen.Name}Location.Value";
        }
        else if (dataType != null)
        {
            loc = $"_{typeGen.Name}Location";
            DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(sb, dataType, objGen, typeGen, passedLenAccessor);
        }
        else
        {
            loc = passedLenAccessor;
        }
        using (var args = sb.Call(
                   $"public partial {typeGen.TypeName(getter: true)}{typeGen.NullChar} Get{typeGen.Name}Custom"))
        {
            if (!fieldData.HasTrigger && dataType == null)
            {
                args.Add($"int location");
            }
        }
        using (var args = sb.Call(
                   $"public {typeGen.OverrideStr}{typeGen.TypeName(getter: true)}{typeGen.NullChar} {typeGen.Name} => Get{typeGen.Name}Custom"))
        {
            if (!fieldData.HasTrigger && dataType == null)
            {
                args.Add($"location: {loc ?? "0x0"}");
            }
        }
        if (!fieldData.HasTrigger)
        {
            currentPosition += fieldData.Length ?? await gen.ExpectedLength(objGen, typeGen);
        }
    }

    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen, 
        TypeGeneration typeGen, 
        Accessor dataAccessor, 
        int? passedLength,
        string passedLengthAccessor,
        DataType? data = null)
    {
        var fieldData = typeGen.GetFieldData();
        var returningParseValue = fieldData.HasTrigger;
        if (data != null)
        {
            DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(sb, data, objGen, typeGen, passedLengthAccessor);
        }
        using (var args = sb.Call(
                   $"{(returningParseValue ? "public " : null)}partial {(returningParseValue ? nameof(ParseResult) : "void")} {(typeGen.Name == null ? typeGen.GetFieldData().RecordType?.ToString() : typeGen.Name)}CustomParse"))
        {
            args.Add($"{nameof(OverlayStream)} stream");
            args.Add($"int offset");
            if (returningParseValue && objGen.GetObjectType() == ObjectType.Subrecord)
            {
                args.Add($"{nameof(PreviousParse)} lastParsed");
            }
        }
    }

    public override async Task GenerateWrapperRecordTypeParse(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen,  
        TypeGeneration typeGen, 
        Accessor locationAccessor, 
        Accessor packageAccessor, 
        Accessor converterAccessor)
    {
        var fieldData = typeGen.GetFieldData();
        var returningParseValue = fieldData.HasTrigger;
        using (var args = sb.Call(
                   $"{(returningParseValue ? "return " : null)}{(typeGen.Name == null ? typeGen.GetFieldData().RecordType?.ToString() : typeGen.Name)}CustomParse"))
        {
            args.Add("stream");
            args.Add("offset");
            if (returningParseValue && objGen.GetObjectType() == ObjectType.Subrecord)
            {
                args.AddPassArg("lastParsed");
            }
        }
    }

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        CustomLogic custom = typeGen as CustomLogic;
        var data = typeGen.GetFieldData();
        return data.Length;
    }
}