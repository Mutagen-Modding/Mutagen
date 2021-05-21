using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Aspects
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
