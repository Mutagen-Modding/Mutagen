using System.Diagnostics;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;

namespace Mutagen.Bethesda.Skyrim;

public partial class ScriptEntry
{
    public enum Flag : byte
    {
        Local = 0,
        Inherited = 1,
        Removed = 2,
        InheritedAndRemoved = 3,
    }
}

partial class ScriptEntrySetterCommon
{
    private static partial void RemapInferredAssetLinks(
        IScriptEntry obj,
        IReadOnlyDictionary<IAssetLinkGetter, string> mapping,
        AssetLinkQuery queryCategories)
    {
        if (!queryCategories.HasFlag(AssetLinkQuery.Inferred)) return;

        if (string.IsNullOrWhiteSpace(obj.Name)) return;

        var compiledAsset = new AssetLink<SkyrimScriptCompiledAssetType>(obj.Name + SkyrimScriptCompiledAssetType.PexExtension);
        var sourceAsset = new AssetLink<SkyrimScriptSourceAssetType>(obj.Name + SkyrimScriptSourceAssetType.PscExtension);

        if (mapping.TryGetValue(compiledAsset, out var newCompiledAsset))
        {
            var directoryParts = newCompiledAsset.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (directoryParts.Length > 1)
            {
                var newName = directoryParts[^1].TrimEnd(SkyrimScriptCompiledAssetType.PexExtension);
                obj.Name = newName;
            }
        }
        else if (mapping.TryGetValue(sourceAsset, out var newSourceAsset))
        {
            var directoryParts = newSourceAsset.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (directoryParts.Length > 1)
            {
                var newName = directoryParts[^1].TrimEnd(SkyrimScriptSourceAssetType.PscExtension);
                obj.Name = newName;
            }
        }
    }
}

partial class ScriptEntryCommon
{
    public static partial IEnumerable<IAssetLinkGetter> GetInferredAssetLinks(IScriptEntryGetter obj, Type? assetType)
    {
        if (string.IsNullOrWhiteSpace(obj.Name)) yield break;

        yield return new AssetLink<SkyrimScriptCompiledAssetType>(obj.Name + SkyrimScriptCompiledAssetType.PexExtension);
        yield return new AssetLink<SkyrimScriptSourceAssetType>(obj.Name + SkyrimScriptSourceAssetType.PscExtension);
    }
}

partial class ScriptEntryBinaryOverlay
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string INamedRequiredGetter.Name => this.Name ?? string.Empty;

    public IReadOnlyList<IScriptPropertyGetter> Properties => throw new NotImplementedException();

    public string Name => throw new NotImplementedException();

    public ScriptEntry.Flag Flags => throw new NotImplementedException();
}