using System.Xml.Linq;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Strings;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Fields;

public class StringType : Loqui.Generation.StringType
{
    public StringBinaryType BinaryType;
    public StringsSource? Translated;

    public override IEnumerable<string> GetRequiredNamespaces()
    {
        foreach (var ns in base.GetRequiredNamespaces())
        {
            yield return ns;
        }
        if (Translated != null)
        {
            yield return "Mutagen.Bethesda.Strings";
        }
    }

    public override string TypeName(bool getter, bool needsCovariance = false)
    {
        if (this.Translated.HasValue)
        {
            return getter ? nameof(ITranslatedStringGetter) : nameof(TranslatedString);
        }
        else
        {
            return base.TypeName(getter, needsCovariance);
        }
    }

    public override string GetDefault(bool getter)
    {
        if (this.Translated.HasValue)
        {
            if (this.Nullable)
            {
                return $"default({nameof(TranslatedString)}?)";
            }
            else
            {
                return getter ? "TranslatedString.Empty" : "string.Empty";
            }
        }
        else
        {
            return base.GetDefault(getter);
        }
    }

    public override async Task Load(XElement node, bool requireName = true)
    {
        this.BinaryType = node.GetAttribute<StringBinaryType>("binaryType", defaultVal: StringBinaryType.NullTerminate);
        this.Translated = node.GetAttribute<StringsSource?>("translated", defaultVal: null);
        await base.Load(node, requireName);
    }

    public override void GenerateClear(StructuredStringBuilder sb, Accessor identifier)
    {
        if (this.Translated.HasValue
            && !this.Nullable)
        {
            sb.AppendLine($"{identifier}.Clear();");
        }
        else
        {
            base.GenerateClear(sb, identifier);
        }
    }

    public override string GenerateEqualsSnippet(Accessor accessor, Accessor rhsAccessor, bool negate)
    {
        if (this.Translated.HasValue)
        {
            return $"{(negate ? "!" : null)}object.Equals({accessor.Access}, {rhsAccessor.Access})";
        }
        else
        {
            return base.GenerateEqualsSnippet(accessor, rhsAccessor, negate);
        }
    }

    public override void GenerateForCopy(StructuredStringBuilder sb, Accessor accessor, Accessor rhs, Accessor copyMaskAccessor, bool protectedMembers, bool deepCopy)
    {
        if (this.Translated.HasValue)
        {
            sb.AppendLine($"if ({(deepCopy ? this.GetTranslationIfAccessor(copyMaskAccessor) : this.SkipCheck(copyMaskAccessor, deepCopy))})");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"{accessor.Access} = {rhs}{this.NullChar}.DeepCopy();");
            }
        }
        else
        {
            base.GenerateForCopy(sb, accessor, rhs, copyMaskAccessor, protectedMembers, deepCopy);
        }
    }

    public override string GetDuplicate(Accessor accessor)
    {
        if (this.Translated.HasValue)
        {
            return $"{accessor}{this.NullChar}.DeepCopy()";
        }
        return base.GetDuplicate(accessor);
    }
}