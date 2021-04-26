using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Plugins.Aspects
{
    /// <summary>
    /// An interface implemented by Major Records that have names
    /// </summary>
    public interface ITranslatedNamedRequired : ITranslatedNamedRequiredGetter, INamedRequired
    {
        /// <summary>
        /// The display name of the record
        /// </summary>
        new TranslatedString Name { get; set; }
    }

    /// <summary>
    /// An interface implemented by Major Records that have names
    /// </summary>
    public interface ITranslatedNamedRequiredGetter : INamedRequiredGetter
    {
        /// <summary>
        /// The display name of the record
        /// </summary>
        new ITranslatedStringGetter Name { get; }
    }
}
