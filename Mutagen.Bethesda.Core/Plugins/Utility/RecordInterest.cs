using Noggog;

namespace Mutagen.Bethesda.Plugins.Utility;

public class RecordInterest
{
    public ICollection<RecordType> InterestingTypes { get; }
    public ICollection<RecordType> UninterestingTypes { get; }
    public bool EmptyMeansInterested = true;

    public RecordInterest(
        IEnumerable<RecordType>? interestingTypes = null,
        IEnumerable<RecordType>? uninterestingTypes = null)
    {
        this.InterestingTypes = new HashSet<RecordType>(
            interestingTypes ?? EnumerableExt<RecordType>.Empty);
        this.UninterestingTypes = new HashSet<RecordType>(
            uninterestingTypes ?? EnumerableExt<RecordType>.Empty);
    }

    public RecordInterest(
        params RecordType[] interestingTypes)
        : this (interestingTypes: interestingTypes, uninterestingTypes: null)
    {
    }

    public bool IsInterested(RecordType type)
    {
        if (InterestingTypes?.Count > 0)
        {
            if (!InterestingTypes.Contains(type)) return false;
        }
        else if (UninterestingTypes?.Count <= 0)
        {
            return this.EmptyMeansInterested;
        }
        return !UninterestingTypes?.Contains(type) ?? true;
    }
}