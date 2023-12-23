using Loqui.Generation;
using Noggog;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class FilePathBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<FilePathType>
{
    public FilePathBinaryTranslationGeneration()
        : base(nullable: true, expectedLen: null)
    {
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
        var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
        using (var args = sb.Call(
                   $"{this.NamespacePrefix}FilePathBinaryTranslation.Instance.Write{(typeGen.Nullable ? "Nullable" : null)}"))
        {
            args.Add($"writer: {writerAccessor}");
            args.Add($"item: {itemAccessor}");
            if (data.RecordType.HasValue)
            {
                args.Add($"header: translationParams.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
            }
            else
            {
                args.Add($"length: {data.Length.Value}");
            }
        }
    }

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor frameAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
        var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
        if (data.HasTrigger)
        {
            sb.AppendLine($"{frameAccessor}.Position += Constants.SUBRECORD_LENGTH;");
        }

        TranslationGeneration.WrapParseCall(
            new TranslationWrapParseArgs()
            {
                FG = sb,
                TypeGen = typeGen,
                TranslatorLine = $"{this.NamespacePrefix}FilePathBinaryTranslation.Instance",
                MaskAccessor = errorMaskAccessor,
                ItemAccessor = itemAccessor,
                TranslationMaskAccessor = translationMaskAccessor,
                IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                ExtraArgs = $"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : "")}".AsEnumerable(),
                SkipErrorMask = true
            });
    }

    public override async Task GenerateCopyInRet(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration targetGen,
        TypeGeneration typeGen,
        Accessor nodeAccessor,
        AsyncMode asyncMode,
        Accessor retAccessor,
        Accessor outItemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationAccessor,
        Accessor converterAccessor,
        bool inline)
    {
        if (inline) throw new NotImplementedException();
        var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
        using (var args = sb.Call(
                   $"{retAccessor}{Loqui.Generation.Utility.Await(asyncMode)}{this.NamespacePrefix}FilePathBinaryTranslation.Instance.Parse",
                   suffixLine: Loqui.Generation.Utility.ConfigAwait(asyncMode)))
        {
            args.Add(nodeAccessor.Access);
            if (asyncMode == AsyncMode.Off)
            {
                args.Add($"item: out {outItemAccessor}");
            }
            if (data.RecordType.HasValue)
            {
                args.Add($"header: translationParams.Convert({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
            }
            else
            {
                args.Add($"length: {data.Length.Value}");
            }
        }
    }
}