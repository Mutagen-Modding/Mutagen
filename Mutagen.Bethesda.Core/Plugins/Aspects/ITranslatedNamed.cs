using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Plugins.Aspects;

/// <summary>
/// An interface implemented by Major Records that have names
/// </summary>
public interface ITranslatedNamed : ITranslatedNamedRequired, ITranslatedNamedGetter, INamed
{
    /// <summary>
    /// The display name of the record
    /// </summary>
    new TranslatedString? Name { get; set; }
}

/// <summary>
/// An interface implemented by Major Records that have names
/// </summary>
public interface ITranslatedNamedGetter : ITranslatedNamedRequiredGetter, INamedGetter
{
    /// <summary>
    /// The display name of the record
    /// </summary>
    new ITranslatedStringGetter? Name { get; }
}