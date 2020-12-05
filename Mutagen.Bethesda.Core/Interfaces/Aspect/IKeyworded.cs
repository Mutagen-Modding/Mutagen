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
    public interface IKeyworded<TKeyword> : IKeywordedGetter<TKeyword>
        where TKeyword : IKeywordCommonGetter
    {
        new ExtendedList<IFormLink<TKeyword>>? Keywords { get; set; }
    }

    /// <summary>
    /// An interface implemented by Major Records that have keywords
    /// </summary>
    public interface IKeywordedGetter
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
}
