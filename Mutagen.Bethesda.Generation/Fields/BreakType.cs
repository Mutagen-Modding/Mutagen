using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Plugin;

namespace Mutagen.Bethesda.Generation.Fields;

public class BreakType : TypeGeneration
{
    public override bool IsEnumerable => throw new NotImplementedException();

    public override bool IsClass => throw new NotImplementedException();

    public override bool HasDefault => throw new NotImplementedException();

    public override bool CopyNeedsTryCatch => throw new NotImplementedException();

    public override CopyLevel CopyLevel => CopyLevel.All;

    public int Index;

    public BreakType()
    {
        IntegrateField = false;
    }

    public override string GenerateACopy(string rhsAccessor)
    {
        throw new NotImplementedException();
    }

    public override void GenerateClear(StructuredStringBuilder sb, Accessor accessorPrefix)
    {
        throw new NotImplementedException();
    }

    public override async Task GenerateForClass(StructuredStringBuilder sb)
    {
        throw new NotImplementedException();
    }

    public override void GenerateForCopy(StructuredStringBuilder sb, Accessor accessor, Accessor rhs, Accessor copyMaskAccessor, bool protectedMembers, bool deepCopy)
    {
        sb.AppendLine($"if ({rhs}.{VersioningModule.VersioningFieldName}.HasFlag({this.ObjectGen.ObjectName}.{VersioningModule.VersioningEnumName}.Break{Index})) return;");
    }

    public override void GenerateForEquals(StructuredStringBuilder sb, Accessor accessor, Accessor rhsAccessor, Accessor maskAccessor)
    {
        throw new NotImplementedException();
    }

    public override void GenerateForEqualsMask(StructuredStringBuilder sb, Accessor accessor, Accessor rhsAccessor, string retAccessor)
    {
        throw new NotImplementedException();
    }

    public override void GenerateForNullableCheck(StructuredStringBuilder sb, Accessor accessor, string checkMaskAccessor)
    {
        throw new NotImplementedException();
    }

    public override void GenerateForHash(StructuredStringBuilder sb, Accessor accessor, string hashResultAccessor)
    {
        throw new NotImplementedException();
    }

    public override void GenerateForInterface(StructuredStringBuilder sb, bool getter, bool internalInterface)
    {
        throw new NotImplementedException();
    }

    public override void GenerateGetNth(StructuredStringBuilder sb, Accessor identifier)
    {
        throw new NotImplementedException();
    }

    public override void GenerateSetNth(StructuredStringBuilder sb, Accessor accessor, Accessor rhs, bool internalUse)
    {
        throw new NotImplementedException();
    }

    public override void GenerateToString(StructuredStringBuilder sb, string name, Accessor accessor, string sbAccessor)
    {
        throw new NotImplementedException();
    }

    public override void GenerateUnsetNth(StructuredStringBuilder sb, Accessor identifier)
    {
        throw new NotImplementedException();
    }

    public override string GetDuplicate(Accessor accessor)
    {
        throw new NotImplementedException();
    }

    public override string SkipCheck(Accessor copyMaskAccessor, bool deepCopy)
    {
        throw new NotImplementedException();
    }

    public override string TypeName(bool getter, bool needsCovariance = false)
    {
        throw new NotImplementedException();
    }
}