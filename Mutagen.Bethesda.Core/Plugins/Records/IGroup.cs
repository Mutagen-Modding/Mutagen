using Noggog;
using Loqui;
using Mutagen.Bethesda.Plugins.Assets;

namespace Mutagen.Bethesda.Plugins.Records;

public interface IGroupCommonGetter :
    IFormLinkContainerGetter, 
    IAssetLinkContainerGetter
{
    /// <summary>
    /// Number of contained records
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Enumerable containing all the FormKeys present in the group
    /// </summary>
    IEnumerable<ILoquiObject> Records { get; }
        
    ILoquiRegistration ContainedRecordRegistration { get; }
        
    Type ContainedRecordType { get; }
}

/// <summary>
/// An interface that Group Record objects implement to hook into the common systems
/// </summary>
public interface IGroupGetter : IGroupCommonGetter
{
    /// <summary>
    /// Mod object the Group belongs to
    /// </summary>
    IMod SourceMod { get; }

    /// <summary>
    /// Enumerable containing all the FormKeys present in the group
    /// </summary>
    IEnumerable<FormKey> FormKeys { get; }

    /// <summary>
    /// Enumerable containing all the FormKeys present in the group
    /// </summary>
    new IEnumerable<IMajorRecordGetter> Records { get; }
    
    /// <summary>
    /// Access to records in an IReadOnlyCache interface
    /// </summary>
    IReadOnlyCache<IMajorRecordGetter, FormKey> RecordCache { get; }

    /// <summary>
    /// Gets the record associated with the specified key
    /// </summary>
    /// <param name="key">FormKey to retrieve</param>
    /// <exception cref="KeyNotFoundException">A record with the given FormKey does not exist</exception>
    /// <returns>Record associated with the specified key</returns>
    IMajorRecordGetter this[FormKey key] { get; }

    /// <summary>
    /// Checks if a record with the specified key exists in the group
    /// </summary>
    /// <param name="key">Key to search for</param>
    /// <returns>True if record found with given key</returns>
    bool ContainsKey(FormKey key);
}

/// <summary>
/// An interface that Group Record objects implement to hook into the common systems
/// </summary>
public interface IListGroupGetter : IGroupCommonGetter
{
    /// <summary>
    /// Number of contained records
    /// </summary>
    new int Count { get; }
    
    /// <summary>
    /// Enumerable containing all the objects present in the group
    /// </summary>
    new IReadOnlyList<ILoquiObject> Records { get; }
}

public interface IGroupCommonGetter<out TObject> : IGroupCommonGetter, IReadOnlyCollection<TObject>
    where TObject : ILoquiObject
{
    /// <summary>
    /// Enumerable containing all the objects present in the group
    /// </summary>
    new IEnumerable<TObject> Records { get; }

    /// <summary>
    /// Number of contained records
    /// </summary>
    new int Count { get; }
}
    
/// <summary>
/// An interface that Group Record objects implement to hook into the common systems
/// </summary>
public interface IGroupGetter<out TMajor> : IGroupCommonGetter<TMajor>, IGroupGetter
    where TMajor : IMajorRecordGetter
{
    /// <summary>
    /// Enumerable containing all the objects present in the group
    /// </summary>
    new IEnumerable<TMajor> Records { get; }
    
    /// <summary>
    /// Access to records in an IReadOnlyCache interface
    /// </summary>
    new IReadOnlyCache<TMajor, FormKey> RecordCache { get; }

    /// <summary>
    /// Gets the record associated with the specified key
    /// </summary>
    /// <param name="key">FormKey to retrieve</param>
    /// <exception cref="KeyNotFoundException">A record with the given FormKey does not exist</exception>
    /// <returns>Record associated with the specified key</returns>
    new TMajor this[FormKey key] { get; }
}

/// <summary>
/// An interface that Group Record objects implement to hook into the common systems
/// </summary>
public interface IListGroupGetter<out TObject> : IGroupCommonGetter<TObject>, IListGroupGetter, IReadOnlyList<TObject>
    where TObject : ILoquiObject
{
    /// <summary>
    /// Number of contained records
    /// </summary>
    new int Count { get; }

    new IEnumerable<TObject> GetEnumerator();
    
    /// <summary>
    /// Enumerable containing all the objects present in the group
    /// </summary>
    new IReadOnlyList<TObject> Records { get; }
}

public interface IGroupCommon : IGroupCommonGetter, IAssetLinkContainer
{
    /// <summary>
    /// Adds
    /// </summary>
    /// <param name="record"></param>
    /// <exception cref="ArgumentException">
    /// A record with the same FormKey already exists in the group, or is of the wrong type.
    /// </exception>
    void AddUntyped(IMajorRecord record);
}

public interface IGroup : IGroupCommon, IGroupGetter
{
    /// <summary>
    /// Enumerable containing all the FormKeys present in the group
    /// </summary>
    new IEnumerable<IMajorRecord> Records { get; }

    /// <summary>
    /// Adds or replaces the major record
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Record was the wrong type
    /// </exception>
    /// <param name="record">The record</param>
    void SetUntyped(IMajorRecord record);

    /// <summary>
    /// Adds or updates the major records given
    /// </summary>
    /// <exception cref="ArgumentException">
    /// A record was the wrong type.  The contents of the group will be undefined.  Some records may have been added.
    /// </exception>
    /// <param name="records">The records</param>
    void SetUntyped(IEnumerable<IMajorRecord> records);
}

public interface IListGroup : IGroupCommon
{
}


public interface IGroupCommon<TObject> : IGroupCommonGetter<TObject>, IGroupCommon, IClearable
    where TObject : ILoquiObject
{
    /// <summary>
    /// Enumerable containing all the objects present in the group
    /// </summary>
    new IEnumerable<TObject> Records { get; }
}

public interface IGroup<TMajor> : IGroupGetter<TMajor>, IGroup, IGroupCommon<TMajor>
    where TMajor : IMajorRecord
{
    /// <summary>
    /// Enumerable containing all the FormKeys present in the group
    /// </summary>
    new IEnumerable<IMajorRecord> Records { get; }
    
    /// <summary>
    /// Access to records in an ICache interface
    /// </summary>
    new ICache<TMajor, FormKey> RecordCache { get; }
        
    /// <summary>
    /// Adds a major record to the group
    /// </summary>
    /// <param name="record">The record</param>
    /// <exception cref="ArgumentException">
    /// A record with the same FormKey already exists in the group
    /// </exception>
    void Add(TMajor record);
        
    /// <summary>
    /// Adds a major record to the group and then returns it
    /// </summary>
    /// <param name="record">The record</param>
    /// <exception cref="ArgumentException">
    /// A record with the same FormKey already exists in the group
    /// </exception>
    /// <returns>The record</returns>
    TMajor AddReturn(TMajor record);

    /// <summary>
    /// Adds or replaces the major record
    /// </summary>
    /// <param name="record">The record</param>
    void Set(TMajor record);

    /// <summary>
    /// Adds or updates the major records given
    /// </summary>
    /// <param name="records">The records</param>
    void Set(IEnumerable<TMajor> records);

    /// <summary>
    /// Removes the item matching the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    bool Remove(FormKey key);

    /// <summary>
    /// Removes all items matching the specified keys
    /// </summary>
    /// <param name="keys">The keys.</param>
    void Remove(IEnumerable<FormKey> keys);

    /// <summary>
    /// Gets the record associated with the specified key
    /// </summary>
    /// <param name="key">FormKey to retrieve</param>
    /// <exception cref="KeyNotFoundException">A record with the given FormKey does not exist</exception>
    /// <returns>Record associated with the specified key</returns>
    new TMajor this[FormKey key] { get; }
}

public interface IListGroup<TObject> : IListGroupGetter<TObject>, IListGroup, IGroupCommon<TObject>, IExtendedList<TObject>
    where TObject : ILoquiObject
{
    /// <summary>
    /// Access to records in an IList interface
    /// </summary>
    new IExtendedList<TObject> Records { get; }

    /// <summary>
    /// Gets the record associated with the specified key
    /// </summary>
    /// <param name="index">Index to get or set</param>
    /// <exception cref="KeyNotFoundException">A record with the given FormKey does not exist</exception>
    /// <returns>Record associated with the specified key</returns>
    new TObject this[int index] { get; set; }

    /// <summary>
    /// Number of contained records
    /// </summary>
    new int Count { get; }
    
    new void Clear();
}