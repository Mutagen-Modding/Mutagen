using System.Collections;
using System.Diagnostics;
using Loqui;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records;

public abstract class AListGroup<TObject> : IListGroup<TObject>
    where TObject : class, ILoquiObject, IFormLinkContainer
{
    private static readonly ILoquiRegistration _registration = LoquiRegistration.GetRegister(typeof(TObject));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected abstract IExtendedList<TObject> ProtectedList { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal IExtendedList<TObject> InternalCache => ProtectedList;
    
    IEnumerable<ILoquiObject> IGroupCommonGetter.Records => ProtectedList;
    IExtendedList<TObject> IListGroup<TObject>.Records => ProtectedList;
    IEnumerable<TObject> IGroupCommonGetter<TObject>.Records => ProtectedList;
    IEnumerable<TObject> IGroupCommon<TObject>.Records => ProtectedList;
    IReadOnlyList<ILoquiObject> IListGroupGetter.Records => ProtectedList;

    IReadOnlyList<TObject> IListGroupGetter<TObject>.Records => ProtectedList;

    public int Count => ProtectedList.Count;

    public bool Remove(TObject item) => ProtectedList.Remove(item);
    
    public bool IsReadOnly => false;

    IEnumerable<TObject> IListGroupGetter<TObject>.GetEnumerator() => ProtectedList;

    public TObject this[int index]
    {
        get => ProtectedList[index];
        set => ProtectedList[index] = value;
    }

    /// <inheritdoc />
    public ILoquiRegistration ContainedRecordRegistration => _registration;
    
    /// <inheritdoc />
    public Type ContainedRecordType => typeof(TObject);

    /// <inheritdoc />
    public void AddUntyped(IMajorRecord record) => Add(ConfirmCorrectType(record, nameof(record)));

    private TObject ConfirmCorrectType(IMajorRecord record, string paramName)
    {
        if (record == null) throw new ArgumentNullException(paramName);
        if (record is not TObject cast)
        {
            throw new ArgumentException(
                $"A record was added of the wrong type.  Expected {typeof(TObject)}, but was given {record.GetType()}",
                paramName);
        }

        return cast;
    }

    /// <inheritdoc />
    public int IndexOf(TObject item) => ProtectedList.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, TObject item) => ProtectedList.Insert(index, item);

    /// <inheritdoc />
    public void RemoveAt(int index) => ProtectedList.RemoveAt(index);

    /// <inheritdoc />
    public void AddRange(IEnumerable<TObject> collection) => ProtectedList.AddRange(collection);

    /// <inheritdoc />
    public void InsertRange(IEnumerable<TObject> collection, int index) => ProtectedList.InsertRange(collection, index);

    /// <inheritdoc />
    public void RemoveRange(int index, int count) => ProtectedList.RemoveRange(index, count);

    /// <inheritdoc />
    public void Move(int original, int destination) => ProtectedList.Move(original, destination);
    
    /// <inheritdoc />
    public abstract void RemapListedAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping);
    
    /// <inheritdoc />
    public abstract IEnumerable<IAssetLink> EnumerateListedAssetLinks();

    /// <inheritdoc />
    public void Add(TObject item) => ProtectedList.Add(item);

    /// <inheritdoc />
    public bool Contains(TObject item) => ProtectedList.Contains(item);
    
    /// <inheritdoc />
    public void CopyTo(TObject[] array, int arrayIndex) => ProtectedList.CopyTo(array, arrayIndex);
    
    /// <inheritdoc />
    public void Clear() => InternalCache.Clear();

    IEnumerator<TObject> IEnumerable<TObject>.GetEnumerator() => ProtectedList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ProtectedList.GetEnumerator();
    
    /// <inheritdoc />
    public abstract IEnumerable<IFormLinkGetter> EnumerateFormLinks();

    /// <inheritdoc />
    public abstract IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories,
        IAssetLinkCache? linkCache = null,
        Type? assetType = null);
}