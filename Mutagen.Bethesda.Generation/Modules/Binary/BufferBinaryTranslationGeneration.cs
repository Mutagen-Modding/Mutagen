using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class BufferBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public override bool NeedsNamespacePrefix => false;

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        return $"ByteArrayBinaryTranslation<{Module.ReaderClass}, {Module.WriterClass}>.Instance";
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
        BufferType zero = typeGen as BufferType;
        sb.AppendLine($"{readerAccessor}.Position += {zero.Length};");
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
        if (inline)
        {
            throw new NotImplementedException();
        }
        if (asyncMode == AsyncMode.Direct) throw new NotImplementedException();
        BufferType buf = typeGen as BufferType;
        sb.AppendLine($"{readerAccessor}.Position += {buf.Length};");
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
        BufferType zero = typeGen as BufferType;
        using (var args = sb.Call(
                   $"{this.NamespacePrefix}{GetTranslatorInstance(typeGen, getter: true)}.Write"))
        {
            args.Add($"writer: {writerAccessor}");
            if (zero.Static)
            {
                args.Add($"item: {objGen.CommonClassName(LoquiInterfaceType.IGetter, MaskType.Normal)}.{typeGen.Name}");
            }
            else
            {
                args.Add($"item: {itemAccessor}");
            }
        }
    }
        
    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        BufferType buf = typeGen as BufferType;
        return buf.Length;
    }
}