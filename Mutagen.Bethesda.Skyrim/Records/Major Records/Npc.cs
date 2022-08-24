using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class Npc
{
    [Flags]
    public enum MajorFlag
    {
        BleedoutOverride = 0x2000_0000
    }
}
    
partial class NpcCommon
{
    public static partial IEnumerable<IAssetLink> GetResolvedAssetLinks(INpcGetter obj, IAssetLinkCache linkCache, Type? assetType)
    {
        if (assetType != null 
            && assetType != typeof(SkyrimTextureAssetType)
            && assetType != typeof(SkyrimModelAssetType))
        {
            yield break;
        }
        
        var npcRace = obj.Race.TryResolve(linkCache.FormLinkCache);
                
        //Valid race, npc has face gen head flag set and npc doesn't use template face data
        if (npcRace != null
            && (npcRace.Flags & Race.Flag.FaceGenHead) != 0
            && (obj.Configuration.Flags & NpcConfiguration.Flag.UseTemplate) == 0
            && (obj.Configuration.TemplateFlags & NpcConfiguration.TemplateFlag.Traits) == 0)
        {
            var formID = obj.FormKey.ID.ToString("X8");
            yield return new AssetLink<SkyrimTextureAssetType>(Path.Combine("actors", "character", "facegendata", "facetint", obj.FormKey.ModKey.FileName.String, $"{formID}.dds"));
            yield return new AssetLink<SkyrimModelAssetType>(Path.Combine("actors", "character", "facegendata", "facegeom", obj.FormKey.ModKey.FileName.String, $"{formID}.nif"));
        }
    }
}

partial class NpcBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryDataMarkerCustom(MutagenFrame frame, INpcInternal item)
    {
        // Skip marker
        frame.ReadSubrecord();
        return null;
    }
}

partial class NpcBinaryWriteTranslation
{
    public static partial void WriteBinaryDataMarkerCustom(MutagenWriter writer, INpcGetter item)
    {
        using var header = HeaderExport.Subrecord(writer, RecordTypes.DATA);
    }
}

partial class NpcBinaryOverlay
{
    public partial ParseResult DataMarkerCustomParse(OverlayStream stream, int offset)
    {
        // Skip marker
        stream.ReadSubrecord();
        return null;
    }
}