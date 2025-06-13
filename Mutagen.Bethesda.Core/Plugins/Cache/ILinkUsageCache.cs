using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Cache;

public interface ILinkUsageCache
{
    /// <summary>
    /// Returns all records that have fields that point to a specific FormKey
    /// </summary>
    /// <param name="identifier">FormKey and Type to check for usage of</param>
    /// <typeparam name="TUserRecordScope">Type of record you want to check would be making use of the given FormKey.  This narrows the search to the scope provided.</typeparam>
    /// <returns>Collection of record links of the scoped type that are pointing to the given FormKey</returns>
    IReadOnlyCollection<IFormLinkGetter<TUserRecordScope>> GetUsagesOf<TUserRecordScope>(
        IFormLinkIdentifier identifier)
        where TUserRecordScope : class, IMajorRecordGetter;
    
    /// <summary>
    /// Returns all records that have fields that point to a specific record
    /// </summary>
    /// <param name="identifier">FormKey and Type to check for usage of</param>
    /// <returns>Collection of record FormKeys that are pointing to the given FormKey</returns>
    IReadOnlyCollection<FormKey> GetUsagesOf(
        IFormLinkIdentifier identifier);

    /// <summary>
    /// Returns all records that have fields that point to a specific FormKey
    /// </summary>
    /// <param name="majorRecord">Major Record to check for usage of</param>
    /// <typeparam name="TUserRecordScope">Type of record you want to check would be making use of the given FormKey.  This narrows the search to the scope provided.</typeparam>
    /// <returns>Collection of record links of the scoped type that have fields pointing to the given record</returns>
    IReadOnlyCollection<IFormLinkGetter<TUserRecordScope>> GetUsagesOf<TUserRecordScope>(
        IMajorRecordGetter majorRecord)
        where TUserRecordScope : class, IMajorRecordGetter;

    /// <summary>
    /// Returns all records that have fields that point to a specific record
    /// </summary>
    /// <param name="majorRecord">Major Record to check for usage of</param>
    /// <returns>Collection of record FormKeys that have fields pointing to the given record</returns>
    IReadOnlyCollection<FormKey> GetUsagesOf(
        IMajorRecordGetter majorRecord);

    /// <summary>
    /// Returns all records that have fields that point to a specific FormKey
    /// </summary>
    /// <param name="formKey">FormKey to check for usage of</param>
    /// <returns>Collection of record FormKeys that have fields pointing to the given FormKey</returns>
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    IReadOnlyCollection<FormKey> GetUsagesOf(FormKey formKey);
}