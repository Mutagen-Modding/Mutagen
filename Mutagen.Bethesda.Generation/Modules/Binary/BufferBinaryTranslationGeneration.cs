using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class BufferBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public override bool NeedsNamespacePrefix => false;

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        return $"ByteArrayBinaryTranslation<{Module.ReaderClass}, {Module.WriterClass}>.Instance";
    }

    public override async Task GenerateCopyIn(
        FileGeneration fg,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor readerAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
        BufferType zero = typeGen as BufferType;
        fg.AppendLine($"{readerAccessor}.Position += {zero.Length};");
    }

    public override void GenerateCopyInRet(
        FileGeneration fg,
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
        fg.AppendLine($"{readerAccessor}.Position += {buf.Length};");
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
        BufferType zero = typeGen as BufferType;
        using (var args = new ArgsWrapper(fg,
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