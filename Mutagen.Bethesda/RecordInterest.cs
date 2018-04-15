using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class RecordInterest
    {
        public ICollection<RecordType> InterestingTypes { get; }
        public ICollection<RecordType> UninterestingTypes { get; }

        public RecordInterest(
            IEnumerable<RecordType> interestingTypes = null,
            IEnumerable<RecordType> uninterestingTypes = null)
        {
            this.InterestingTypes = new HashSet<RecordType>(
                interestingTypes ?? EnumerableExt<RecordType>.EMPTY);
            this.UninterestingTypes = new HashSet<RecordType>(
                uninterestingTypes ?? EnumerableExt<RecordType>.EMPTY);
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
            return !UninterestingTypes?.Contains(type) ?? true;
        }
    }
}
