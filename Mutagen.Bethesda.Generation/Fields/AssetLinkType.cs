using System.Xml.Linq;
using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Generation.Fields;

public class AssetLinkType : StringType
{
    public string RawAssetTypeString;

    public string AssetTypeString => $"{RawAssetTypeString}AssetType";

    public override IEnumerable<string> GetRequiredNamespaces()
    {
        foreach (var ns in base.GetRequiredNamespaces())
        {
            yield return ns;
        }
        yield return "Mutagen.Bethesda.Assets";
        yield return $"Mutagen.Bethesda.{this.ObjectGen.ProtoGen.Protocol.Namespace}.Assets";
    }

    public override string GetDefault(bool getter)
    {
        return this.Nullable ? "null" : $"new AssetLink{(getter ? "Getter" : null)}<{AssetTypeString}>({AssetTypeString}.Instance)";
    }

    public override async Task Load(XElement node, bool requireName = true)
    {
        await base.Load(node, requireName);
        if (Translated.HasValue)
        {
            throw new ArgumentException($"AssetLink cannot be translated: {this.ObjectGen.Name}:{this.Name}");
        }

        if (!node.TryGetAttribute<string>("assetType", out var assetStr))
        {
            throw new ArgumentException($"AssetLink must list assetType: {this.ObjectGen.Name}:{this.Name}");
        }

        RawAssetTypeString = assetStr;
    }

    public override string TypeName(bool getter, bool needsCovariance = false)
    {
        if (getter)
        {
            return $"{nameof(IAssetLinkGetter)}<{AssetTypeString}>";
        }
        else
        {
            return $"{nameof(IAssetLink)}<{AssetTypeString}>";
        }
    }

    public override void GenerateForCopy(StructuredStringBuilder sb, Accessor accessor, Accessor rhs, Accessor copyMaskAccessor, bool protectedMembers, bool deepCopy)
    {
        if (this.Nullable)
        {
            sb.AppendLine($"{accessor} = {nameof(PluginUtilityTranslation)}.{nameof(PluginUtilityTranslation.AssetNullableDeepCopyIn)}({accessor}, {rhs});");
        }
        else
        {
            sb.AppendLine($"{accessor}.{nameof(IAssetLink.RawPath)} = {rhs}{this.NullChar}.{nameof(IAssetLink.RawPath)};");
        }
    }

    public override void GenerateClear(StructuredStringBuilder sb, Accessor identifier)
    {
        if (this.Nullable)
        {
            sb.AppendLine($"{identifier.Access} = default;");
        }
        else
        {
            sb.AppendLine($"{identifier.Access}.SetToNull();");
        }
    }

    public override void GenerateCopySetToConverter(StructuredStringBuilder sb)
    {
        sb.AppendLine($".Select(r => r{NullChar}.AsSetter())");
    }

    public override string GetDuplicate(Accessor accessor)
    {
        return $"{accessor}{NullChar}.AsSetter()";
    }
}