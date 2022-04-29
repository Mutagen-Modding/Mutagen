using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public enum ListBinaryType
{
    SubTrigger,
    Trigger,
    CounterRecord,
    PrependCount,
    Frame
}

public abstract class ListBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public virtual string TranslatorName => $"ListBinaryTranslation";

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        var list = typeGen as ListType;
        if (!Module.TryGetTypeGeneration(list.SubTypeGeneration.GetType(), out var subTransl))
        {
            throw new ArgumentException("Unsupported type generator: " + list.SubTypeGeneration);
        }

        var subMaskStr = subTransl.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration);
        return $"{TranslatorName}<{list.SubTypeGeneration.TypeName(getter, needsCovariance: true)}, {subMaskStr}>.Instance";
    }

    public override bool IsAsync(TypeGeneration gen, bool read)
    {
        var listType = gen as ListType;
        if (this.Module.TryGetTypeGeneration(listType.SubTypeGeneration.GetType(), out var keyGen)
            && keyGen.IsAsync(listType.SubTypeGeneration, read)) return true;
        return false;
    }

    protected virtual string GetWriteAccessor(Accessor itemAccessor)
    {
        return itemAccessor.Access;
    }

    public override void GenerateCopyInRet(
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
        throw new NotImplementedException();
    }

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        return null;
    }
}