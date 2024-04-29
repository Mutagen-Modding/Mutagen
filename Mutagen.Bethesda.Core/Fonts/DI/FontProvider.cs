using Mutagen.Bethesda.Strings;
using Noggog;
namespace Mutagen.Bethesda.Fonts.DI;

public interface IFontProvider {
	/// <summary>
	/// Gets the data relative file paths of the font libraries
	/// </summary>
	IReadOnlyList<FilePath> FontLibraries { get; }

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

public class FontProvider : IFontProvider {
	private readonly List<FilePath> _fontLibraries = new();
	private readonly Dictionary<string, FontMapping> _fontMappings = new();
	private char[] _validNameChars = Array.Empty<char>();
	private char[] _validBookChars = Array.Empty<char>();

	public IReadOnlyList<FilePath> FontLibraries => _fontLibraries;
	public IReadOnlyDictionary<string, FontMapping> FontMappings => _fontMappings;
	public IReadOnlyList<char> ValidNameChars => _validNameChars;
	public IReadOnlyList<char> ValidBookChars => _validBookChars;

	public FontProvider(
		Language language,
		IGetFontConfig fontConfig)
	{
		var configFileStream = fontConfig.GetStream(language);
		Init(configFileStream);
	}

	private void Init(Stream stream)
	{
		var reader = new StreamReader(stream);
		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null) break;

			if (line.StartsWith("map", StringComparison.OrdinalIgnoreCase))
			{
				// map "$HandwrittenFont" = "Handwritten_Institute" Normal
				// map "$CClub_Font_Bold" = "Eurostile LT Cyr Std" Demi
				// map "ControllerButtonsInverted" = "Controller  Buttons inverted" Normal
				var span = line.AsSpan();

				// Skip the map and the first quote
				span = span[5..];

				// Get the font alias
				var aliasEnd = span.IndexOf('"');
				var alias = span[..aliasEnd].ToString();

				// Skip the alias, the equals sign and the third quote
				span = span[(aliasEnd + 5)..];

				// Get the font id
				var fonIdEnd = span.IndexOf('"');
				var fontId = span[..fonIdEnd].ToString();

				// Skip the font id and the space
				span = span[(fonIdEnd + 2)..];

				// Get the font weight
				var fontWeight = span.ToString();

				_fontMappings.Add(alias, new FontMapping(fontId, fontWeight));
			}
			else if (line.StartsWith("fontlib", StringComparison.OrdinalIgnoreCase))
			{
				// fontlib "Interface\fonts_console.swf"
				// fontlib "fonts_en"
				var lib = line[9..^1];

				// Add starting interface folder if missing
				if (!lib.StartsWith("Interface", StringComparison.OrdinalIgnoreCase))
				{
					lib = @"Interface\" + lib;
				}

				_fontLibraries.Add(lib);
			}
			else if (line.StartsWith("validBookChars", StringComparison.OrdinalIgnoreCase))
			{
				// validBookChars "`1234567890-=~!@#%^&*"
				_validBookChars = line[16..^1].ToCharArray();
			}
			else if (line.StartsWith("validNameChars", StringComparison.OrdinalIgnoreCase))
			{
				// validNameChars "`1234567890-=~!@#%^&*"
				_validNameChars = line[16..^1].ToCharArray();
			}
		}
	}
}
