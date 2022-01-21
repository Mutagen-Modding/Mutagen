using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Assets;
using Noggog;

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
        return $"new AssetLink{(getter ? "Getter" : null)}<{AssetTypeString}>({AssetTypeString}.Instance)";
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

    public override void GenerateForCopy(FileGeneration fg, Accessor accessor, Accessor rhs, Accessor copyMaskAccessor, bool protectedMembers, bool deepCopy)
    {
        fg.AppendLine($"{accessor}.{nameof(IAssetLink.RawPath)} = {rhs}{this.NullChar}.{nameof(IAssetLink.RawPath)};");
    }
}