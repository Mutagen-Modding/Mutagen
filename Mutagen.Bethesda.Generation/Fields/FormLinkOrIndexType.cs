using Loqui.Generation;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Fields;

public class FormLinkOrIndexType : FormLinkType
{
    public override string ClassTypeString => "FormLinkOrIndex";

    public override async Task GenerateForCtor(StructuredStringBuilder sb)
    {
        sb.AppendLine($"_{Name} = new {DirectTypeName(getter: false, internalInterface: true)}(this);");
    }

    public override string GetNewForNonNullable()
    {
        return $"default!";
    }

    public override void GenerateToString(StructuredStringBuilder sb, string name, Accessor accessor, string sbAccessor)
    {
        if (!this.IntegrateField) return;
        sb.AppendLine($"sb.{nameof(StructuredStringBuilder.AppendItem)}({accessor}{(string.IsNullOrWhiteSpace(this.Name) ? null : $", \"{this.Name}\"")});");
    }

    public override void GenerateForCopy(StructuredStringBuilder sb, Accessor accessor, Accessor rhs, Accessor copyMaskAccessor, bool protectedMembers, bool deepCopy)
    {
        sb.AppendLine($"if ({(deepCopy ? this.GetTranslationIfAccessor(copyMaskAccessor) : this.SkipCheck(copyMaskAccessor, deepCopy))})");
        using (sb.CurlyBrace())
        {
            if (this.Nullable
                || deepCopy)
            {
                sb.AppendLine($"{accessor}.SetTo({rhs});");
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}