using System.Collections;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Skyrim;

internal sealed class SkyrimListGroupMergedOverlay : ISkyrimListGroupGetter<ICellBlockGetter>
{
    private readonly ISkyrimListGroupGetter<ICellBlockGetter>[] _sub;
    private IReadOnlyList<ICellBlockGetter>? _records;

    public SkyrimListGroupMergedOverlay(ISkyrimListGroupGetter<ICellBlockGetter>[] sub)
    {
        _sub = sub;
    }

    public IReadOnlyList<ICellBlockGetter> Records
    {
        get
        {
            if (_records != null) return _records;
            _records = CellBlockConsolidator.ConsolidateBlocks(_sub);
            return _records;
        }
    }

    public GroupTypeEnum Type => _sub[^1].Type;
    public int LastModified => _sub[^1].LastModified;
    public int Unknown => _sub[^1].Unknown;
    public int Count => Records.Count;

    public IEnumerator<ICellBlockGetter> GetEnumerator() => Records.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    ILoquiRegistration ILoquiObject.Registration => SkyrimListGroup_Registration.Instance;

    public object CommonInstance(Type type) => GenericCommonInstanceGetter.Get(SkyrimListGroupCommon<ICellBlockGetter>.Instance, typeof(ICellBlockGetter), type);
    public object? CommonSetterInstance(Type type) => null;
    public object CommonSetterTranslationInstance() => SkyrimListGroupSetterTranslationCommon.Instance;

    object IBinaryItem.BinaryWriteTranslator => SkyrimListGroupBinaryWriteTranslation.Instance;

    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        SkyrimListGroupBinaryWriteTranslation.Instance.Write(writer, this, translationParams);
    }

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"SkyrimListGroupMergedOverlay ({_sub.Length} sources, {Records.Count} records)");
    }

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true) => SkyrimListGroupCommon<ICellBlockGetter>.Instance.EnumerateFormLinks(this, iterateNestedRecords);
    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories, IAssetLinkCache? linkCache, Type? assetType) => SkyrimListGroupCommon<ICellBlockGetter>.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords() => this.EnumerateMajorRecords();
    IEnumerable<TMajor> IMajorRecordGetterEnumerable.EnumerateMajorRecords<TMajor>(bool throwIfUnknown) => this.EnumerateMajorRecords<ICellBlockGetter, TMajor>(throwIfUnknown: throwIfUnknown);
    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords(Type type, bool throwIfUnknown) => this.EnumerateMajorRecords(type: type, throwIfUnknown: throwIfUnknown);
}
