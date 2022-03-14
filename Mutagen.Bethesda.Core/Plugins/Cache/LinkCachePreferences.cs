using System;
using Mutagen.Bethesda.Plugins.Records.Internals;

namespace Mutagen.Bethesda.Plugins.Cache;

public class LinkCachePreferences
{
    public enum RetentionType
    {
        WholeRecord,
        OnlyIdentifiers,
    }
    
    public static LinkCachePreferences Default => _wholeRecord;

    private static readonly LinkCachePreferences _wholeRecord = new()
    {
        Retention = RetentionType.WholeRecord
    };
    
    public static LinkCachePreferences WholeRecord() => _wholeRecord;

    private static readonly LinkCachePreferences _onlyIdentifiers = new()
    {
        Retention = RetentionType.OnlyIdentifiers
    };
    
    public static LinkCachePreferences OnlyIdentifiers() => _onlyIdentifiers;

    public RetentionType Retention { get; init; } = RetentionType.WholeRecord;
    
    public IMetaInterfaceMapGetter? MetaInterfaceMapGetterOverride { get; set; }
}