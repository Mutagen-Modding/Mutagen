using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Assets.DI;
using Mutagen.Bethesda.Strings;
namespace Mutagen.Bethesda.Fonts.DI;

public interface IGetFontConfig
{
	/// <summary>
	/// Gets the file path of the font configuration file
	/// </summary>
	/// <param name="language">The language to get the font configuration file for</param>
	/// <returns>The file path of the font configuration file</returns>
	DataRelativePath GetAssetPath(Language language);

	/// <summary>
	/// Gets the font configuration file as a stream
	/// </summary>
	/// <param name="language">The language to get the font configuration file for</param>
	/// <returns>The font configuration file as a stream</returns>
	Stream GetStream(Language language);
}

public class GetFontConfig : IGetFontConfig
{
	private readonly IAssetProvider _assetProvider;
	private readonly IGetFontConfigListing _iniListings;

	public GetFontConfig(
		IAssetProvider assetProvider,
		IGetFontConfigListing iniListings)
	{
		_assetProvider = assetProvider;
		_iniListings = iniListings;
	}

	public DataRelativePath GetAssetPath(Language language)
	{
		var iniFontConfig = _iniListings.Get();

		// Try to use the font config file specified in the ini
		if (iniFontConfig is {} configAssetPath && _assetProvider.Exists(configAssetPath))
		{
			return configAssetPath;
		}

		// If the ini file doesn't specify a font config, try a language-specific one
		var isoLanguageString = StringsUtility.GetIsoLanguageString(language);
		var languageAssetPath = new DataRelativePath($"Interface/FontConfig_{isoLanguageString}.txt");
		if (_assetProvider.Exists(languageAssetPath))
		{
			return languageAssetPath;
		}

		// If the language-specific font config doesn't exist, use the default
		var defaultAssetPath = new DataRelativePath("Interface/FontConfig.txt");
		if (_assetProvider.Exists(defaultAssetPath))
		{
			return defaultAssetPath;
		}

		throw new FileNotFoundException();
	}

	public Stream GetStream(Language language)
	{
		return _assetProvider.GetStream(GetAssetPath(language));
	}
}
