using Noggog;
using Mutagen.Bethesda.Strings.DI;

namespace Mutagen.Bethesda.Strings;

/// <summary>
/// Class to specify string reading behaviors when importing a mod
/// </summary>
public sealed record StringsReadParameters
{
    /// <summary>
    /// If specified, normal string folder path locations will be ignored in favor of the path provided.
    /// </summary>
    public DirectoryPath? StringsFolderOverride { get; init; }
    
    /// <summary>
    /// If specified, normal bsa folder path locations will be ignored in favor of the path provided.
    /// </summary>
    public DirectoryPath? BsaFolderOverride { get; init; }

    /// <summary>
    /// The object to retrieve the encodings to be used
    /// </summary>
    public IMutagenEncodingProvider? EncodingProvider { get; init; }

    /// <summary>
    /// Controls a few things:<br/>
    /// 1)  What language TranslatedString members query when their `String` members are accessed
    /// 2)  What language a non-localized TranslatedString will be interpreted as when exported in a now localized context.
    /// </summary>
    public Language? TargetLanguage { get; init; }
        
    /// <summary>
    /// Overrides what encoding to be used for strings that have no translation concepts
    /// </summary>
    public IMutagenEncoding? NonTranslatedEncodingOverride { get; init; }
        
    /// <summary>
    /// Overrides what encoding to be used for TranslatedStrings that are not localized
    /// </summary>
    public IMutagenEncoding? NonLocalizedEncodingOverride { get; init; }
}