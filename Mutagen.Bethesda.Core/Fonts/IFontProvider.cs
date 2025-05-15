using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Fonts;

public interface IFontProvider
{
    /// <summary>
    /// Gets the data relative file paths of the font libraries
    /// </summary>
    IReadOnlyList<DataRelativePath> FontLibraries { get; }

    /// <summary>
    /// Gets the font mappings
    /// <example>
    /// "$HandwrittenFont" => "SkyrimBooks_Handwritten_Bold"
    /// </example>
    /// </summary>
    IReadOnlyDictionary<string, FontMapping> FontMappings { get; }

    /// <summary>
    /// Gets the valid characters for names
    /// </summary>
    IReadOnlyList<char> ValidNameChars { get; }

    /// <summary>
    /// Gets the valid characters for books
    /// </summary>
    IReadOnlyList<char> ValidBookChars { get; }
}