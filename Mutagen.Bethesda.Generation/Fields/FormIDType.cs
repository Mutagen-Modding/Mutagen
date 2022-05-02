using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Generation.Fields;

public class FormIDType : PrimitiveType
{
    public override Type Type(bool getter) => typeof(FormID);

    public override void GenerateForEquals(StructuredStringBuilder sb, Accessor accessor, Accessor rhsAccessor, Accessor maskAccessor)
    {
        if (!this.IntegrateField) return;
        sb.AppendLine($"if ({this.GetTranslationIfAccessor(maskAccessor)})");
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"if (!{accessor}.Equals({rhsAccessor})) return false;");
        }
    }

    public override void GenerateForEqualsMask(StructuredStringBuilder sb, Accessor accessor, Accessor rhsAccessor, string retAccessor)
    {
        if (!this.IntegrateField) return;
        sb.AppendLine($"{retAccessor} = {accessor}.Equals({rhsAccessor});");
    }
}