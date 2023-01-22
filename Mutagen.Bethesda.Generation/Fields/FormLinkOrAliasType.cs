using Loqui.Generation;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Generation.Fields;

public class FormLinkOrAliasType : FormLinkType
{
    public override string ClassTypeString => "FormLinkOrAlias";

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
}