namespace Mutagen.Bethesda.Plugins.Assets;

[Flags]
public enum AssetLinkQuery
{
    /// <summary>
    /// An AssetLink that is explicitly listed in a field
    /// </summary>
    Listed = 0x01,
    
    /// <summary>
    /// An AssetLink that can be inferred by fields that exist on the record
    /// </summary>
    Inferred = 0x02,
    
    /// <summary>
    /// An AssetLink that can be inferred by comparing several Major Records that need to be resolved and compared
    /// </summary>
    Resolved = 0x04,
}