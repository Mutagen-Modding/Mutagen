using System.Diagnostics;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim.Assets;

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

partial class ScriptEntryCommon
{
    public static partial IEnumerable<IAssetLink> GetInferredAssetLinks(IScriptEntryGetter obj, Type? assetType)
    {
        if (string.IsNullOrWhiteSpace(obj.Name)) yield break;

        yield return new AssetLink<SkyrimScriptCompiledAssetType>($"{obj.Name}.{SkyrimScriptCompiledAssetType.PexExtension}");
        yield return new AssetLink<SkyrimScriptSourceAssetType>($"{obj.Name}.{SkyrimScriptSourceAssetType.PscExtension}");
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