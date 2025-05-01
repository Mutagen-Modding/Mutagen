using System.Collections;
using System.Diagnostics;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Morrowind.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Morrowind.Records;

internal class MorrowindGroupWrapper<TMajor> : IMorrowindGroupGetter<TMajor>
    where TMajor : class, IMorrowindMajorRecordGetter, IBinaryItem
{
    private readonly GroupMergeGetter<IMorrowindGroupGetter<TMajor>, TMajor> _groupMerge;

    public MorrowindGroupWrapper(GroupMergeGetter<IMorrowindGroupGetter<TMajor>, TMajor> groupMerge)
    {
        _groupMerge = groupMerge;
    }

    #region IGroupGetter Forwarding

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks() => _groupMerge.EnumerateFormLinks();

    public IMod SourceMod => _groupMerge.SourceMod;
    
    IReadOnlyCache<TMajor, FormKey> IGroupGetter<TMajor>.RecordCache => _groupMerge.RecordCache;

    public TMajor this[FormKey key] => _groupMerge[key];

    IEnumerable<TMajor> IGroupGetter<TMajor>.Records => _groupMerge.Records;
    IEnumerable<ILoquiObject> IGroupCommonGetter.Records => _groupMerge.Records;
    IEnumerable<TMajor> IGroupCommonGetter<TMajor>.Records => _groupMerge.Records;

    IReadOnlyCache<IMajorRecordGetter, FormKey> IGroupGetter.RecordCache => ((IGroupGetter)_groupMerge).RecordCache;

    public int Count => _groupMerge.Count;

    IMajorRecordGetter IGroupGetter.this[FormKey key] => ((IGroupGetter)_groupMerge)[key];

    public IEnumerable<FormKey> FormKeys => _groupMerge.FormKeys;

    IEnumerable<IMajorRecordGetter> IGroupGetter.Records => ((IGroupGetter)_groupMerge).Records;
    
    public Type ContainedRecordType => typeof(TMajor);

    public bool ContainsKey(FormKey key)
    {
        return _groupMerge.ContainsKey(key);
    }

    public IEnumerator<TMajor> GetEnumerator()
    {
        return _groupMerge.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_groupMerge).GetEnumerator();
    }

    #endregion
    
    #region Common Routing

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ILoquiRegistration ILoquiObject.Registration => MorrowindGroup_Registration.Instance;

    public static MorrowindGroup_Registration StaticRegistration => MorrowindGroup_Registration.Instance;

    [DebuggerStepThrough]
    protected object CommonInstance(Type type0) =>
        GenericCommonInstanceGetter.Get(MorrowindGroupCommon<TMajor>.Instance, typeof(TMajor), type0);

    [DebuggerStepThrough]
    protected object CommonSetterTranslationInstance() => MorrowindGroupSetterTranslationCommon.Instance;

    [DebuggerStepThrough]
    object IMorrowindGroupGetter<TMajor>.CommonInstance(Type type0) => this.CommonInstance(type0);

    [DebuggerStepThrough]
    object? IMorrowindGroupGetter<TMajor>.CommonSetterInstance(Type type0) => null;

    [DebuggerStepThrough]
    object IMorrowindGroupGetter<TMajor>.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

    #endregion

    public IReadOnlyCache<TMajor, FormKey> RecordCache => _groupMerge.RecordCache;
    
    public GroupTypeEnum Type => _groupMerge.SubGroups[^1].Type;
    
    public int LastModified => _groupMerge.SubGroups[^1].LastModified;

    public int Unknown => _groupMerge.SubGroups[^1].Unknown;
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected object BinaryWriteTranslator => MorrowindGroupBinaryWriteTranslation.Instance;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object IBinaryItem.BinaryWriteTranslator => this.BinaryWriteTranslator;
    
    void IBinaryItem.WriteToBinary(
        MutagenWriter writer,
        TypedWriteParams translationParams = default)
    {
        ((MorrowindGroupBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
            item: this,
            writer: writer,
            translationParams: translationParams);
    }

    public void Print(StructuredStringBuilder fg, string? name = null)
    {
        MorrowindGroupMixIn.Print(
            item: this,
            name: name);
    }
    [DebuggerStepThrough]
    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords() => this.EnumerateMajorRecords();
    [DebuggerStepThrough]
    IEnumerable<TRhs> IMajorRecordGetterEnumerable.EnumerateMajorRecords<TRhs>(bool throwIfUnknown) => this.EnumerateMajorRecords<TMajor, TRhs>(throwIfUnknown: throwIfUnknown);
    [DebuggerStepThrough]
    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords(Type type, bool throwIfUnknown) => this.EnumerateMajorRecords(type: type, throwIfUnknown: throwIfUnknown);

    public ILoquiRegistration ContainedRecordRegistration => _groupMerge.ContainedRecordRegistration;
    
    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(
        AssetLinkQuery queryCategories = AssetLinkQuery.Listed, 
        IAssetLinkCache? linkCache = null, 
        Type? assetType = null)
    {
        return _groupMerge.EnumerateAssetLinks(queryCategories, linkCache, assetType);
    }
}