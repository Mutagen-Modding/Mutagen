using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class FormKeyBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<FormKey>
{
    public override bool NeedsGenerics => false;

    public FormKeyBinaryTranslationGeneration()
        : base(expectedLen: 4)
    {
        this.PreferDirectTranslation = false;
        AdditionalCopyInParams.Add((o, t) => TryGet<string>.Succeed("reference: false"));
    }

    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb,
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        Accessor structDataAccessor,  
        Accessor recordDataAccessor, 
        int? currentPosition,
        string passedLengthAccessor,
        DataType dataType)
    {
        var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
        if (data.RecordType.HasValue
            || await this.ExpectedLength(objGen, typeGen) == null)
        {
            return;
            throw new NotImplementedException();
        }
        var posStr = dataType == null ? $"{passedLengthAccessor}" : $"_{dataType.GetFieldData().RecordType}Location + {passedLengthAccessor}";
        sb.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Name} => FormKeyBinaryTranslation.Instance.Parse({structDataAccessor}.Span.Slice({posStr}, {(await this.ExpectedLength(objGen, typeGen)).Value}), this._package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.MasterReferences)}, reference: false);");
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
        Accessor translationMaskAccessor,
        Accessor converterAccessor,
        bool inline)
    {
        if (inline) throw new NotImplementedException();
        if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
        var data = typeGen.GetFieldData();
        if (data.RecordType.HasValue)
        {
            sb.AppendLine("r.Position += Constants.SUBRECORD_LENGTH;");
        }
        using (var args = sb.Call(
                   $"{retAccessor}{this.NamespacePrefix}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
        {
            args.Add(nodeAccessor.Access);
            if (this.DoErrorMasks)
            {
                args.Add($"errorMask: {errorMaskAccessor}");
            }
            args.Add($"translationMask: {translationMaskAccessor}");
            args.Add($"reference: false");
        }
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
        if (!(await objGen.IsMajorRecord()))
        {
            throw new NotImplementedException();
        }
        using (var args = sb.Call(
                   $"{this.NamespacePrefix}{this.GetTranslatorInstance(typeGen, getter: true)}.Write{(typeGen.Nullable ? "Nullable" : null)}"))
        {
            args.Add($"writer: {writerAccessor}");
            args.Add($"item: item");
            args.Add($"reference: false");
        }
    }
}