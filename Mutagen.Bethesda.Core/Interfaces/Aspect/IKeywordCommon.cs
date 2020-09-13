using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface implemented by Major Records that are keywords
    /// </summary>
    public interface IKeywordCommon : IKeywordCommonGetter, IMajorRecordCommon
    {
    }

    /// <summary>
    /// An interface implemented by Major Records that are keywords
    /// </summary>
    public interface IKeywordCommonGetter : IMajorRecordCommonGetter
    {
    }
}
