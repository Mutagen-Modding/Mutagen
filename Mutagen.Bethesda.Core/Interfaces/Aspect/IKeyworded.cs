using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface implemented by Major Records that have keywords
    /// </summary>
    public interface IKeyworded<TKeyword> : IKeywordedGetter<TKeyword>, IMajorRecordCommon
        where TKeyword : IKeywordCommonGetter
    {
        new ExtendedList<IFormLink<TKeyword>>? Keywords { get; }
    }

    /// <summary>
    /// An interface implemented by Major Records that have keywords
    /// </summary>
    public interface IKeywordedGetter : IMajorRecordCommonGetter
    {
        IReadOnlyList<IFormLink<IKeywordCommonGetter>>? Keywords { get; }
    }

    /// <summary>
    /// An interface implemented by Major Records that have keywords
    /// </summary>
    public interface IKeywordedGetter<TKeyword> : IKeywordedGetter
        where TKeyword : IKeywordCommonGetter
    {
        new IReadOnlyList<IFormLink<TKeyword>>? Keywords { get; }
    }

    public static class IKeywordedExt
    {
        public static bool HasKeyword(this IKeywordedGetter keyworded, IKeywordCommonGetter keyword)
        {
            var fk = keyword.FormKey;
            return keyworded.Keywords.Any(k => k.FormKey == fk);
        }
    }
}
