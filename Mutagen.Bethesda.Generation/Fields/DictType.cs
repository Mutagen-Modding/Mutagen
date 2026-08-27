using System.Xml.Linq;
using Loqui.Generation;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Fields;

public class DictType : Loqui.Generation.DictType
{
    /// <summary>
    /// This parameter is necessessary if the enums are not accessible at generation time by the generation program.
    /// If they are, this parameter can be removed in favor of reflection
    /// </summary>
    public byte? NumEnumKeys;

    public override async Task Load(XElement node, bool requireName = true)
    {
        await base.Load(node, requireName);
        if (node.TryGetAttribute("numEnumKeys", out byte num))
        {
            NumEnumKeys = num;
        }
    }

    // Loqui.Generation.DictType_Typical/DictType_KeyedValue emit Equals via
    // SequenceEqualNullable, an order-sensitive comparison. That disagrees
    // with GetEqualsMask, which already compares by key via
    // EqualsMaskHelper.DictEqualsHelper/CacheEqualsHelper (Mutagen-Modding/Mutagen#686).
    // Reuse those same runtime helpers here so Equals and
    // GetEqualsMask(...).All(...) stay provably consistent.

    public override string GenerateEqualsSnippet(Accessor accessor, Accessor rhsAccessor, bool negate = false)
    {
        var expression = EqualsBoolExpression(accessor, rhsAccessor);
        return negate ? $"!({expression})" : expression;
    }

    public override void GenerateForEquals(StructuredStringBuilder sb, Accessor accessor, Accessor rhsAccessor, Accessor maskAccessor)
    {
        sb.AppendLine($"if ({this.GetTranslationIfAccessor(maskAccessor)})");
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"if (!({EqualsBoolExpression(accessor, rhsAccessor)})) return false;");
        }
    }

    private string EqualsBoolExpression(Accessor accessor, Accessor rhsAccessor)
    {
        var helperName = this.Mode == DictMode.KeyedValue ? "CacheEqualsHelper" : "DictEqualsHelper";
        var maskGetterArg = this.ValueTypeGen is LoquiType
            ? "maskGetter: (k, l, r) => l.GetEqualsMask(r, EqualsMaskHelper.Include.All), "
            : string.Empty;
        // Both operands are null-forgiven here: when this.Nullable, the call
        // below is only ever reached after the null-guard has already proven
        // both sides non-null; the compiler's flow analysis can't follow that
        // reasoning across the guard's && / || though, hence the "!".
        var lhsExpr = this.Nullable ? $"{accessor.Access}!" : accessor.Access;
        var rhsExpr = this.Nullable ? $"{rhsAccessor.Access}!" : rhsAccessor.Access;
        var helperCall = $"EqualsMaskHelper.{helperName}(lhs: {lhsExpr}, rhs: {rhsExpr}, {maskGetterArg}include: EqualsMaskHelper.Include.All)?.Overall ?? true";
        if (!this.Nullable)
        {
            return helperCall;
        }
        // Preserve SequenceEqualNullable's null-tolerance: both null is equal,
        // exactly one null is unequal, neither null falls through to the helper.
        return $"(({accessor.Access} == null) == ({rhsAccessor.Access} == null)) && ({accessor.Access} == null || ({helperCall}))";
    }
}
