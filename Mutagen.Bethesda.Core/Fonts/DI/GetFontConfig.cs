using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Strings;
using Noggog;
namespace Mutagen.Bethesda.Fonts.DI;

public interface IGetFontConfig
{
	/// <summary>
	/// Gets the file path of the font configuration file
	/// </summary>
	/// <param name="language">The language to get the font configuration file for</param>
	/// <returns>The file path of the font configuration file</returns>
	FilePath GetFilePath(Language language);

	/// <summary>
	/// Gets the font configuration file as a stream
	/// </summary>
	/// <param name="language">The language to get the font configuration file for</param>
	/// <returns>The font configuration file as a stream</returns>
	Stream GetStream(Language language);
}

public class GetFontConfig : IGetFontConfig
{
	private readonly IFileSystem _fileSystem;
	private readonly IGetFontConfigListing _iniListings;
	private readonly IDataDirectoryProvider _dataDirectoryProvider;

	public GetFontConfig(
		IFileSystem fileSystem,
		IGetFontConfigListing iniListings,
		IDataDirectoryProvider dataDirectoryProvider)
	{
		_fileSystem = fileSystem;
		_iniListings = iniListings;
		_dataDirectoryProvider = dataDirectoryProvider;
	}

	public FilePath GetFilePath(Language language)
	{
		var dataDirectory = _dataDirectoryProvider.Path;
		var iniFontConfig = _iniListings.Get();

		// Try to use the font config file specified in the ini
		if (iniFontConfig is {} config)
		{
			var fullPath = _fileSystem.Path.Combine(dataDirectory, config.Path);
			if (_fileSystem.File.Exists(fullPath))
			{
				return fullPath;
			}
		}

		// If the ini file doesn't specify a font config, try a language-specific one
		var isoLanguageString = StringsUtility.GetIsoLanguageString(language);
		var languageFilePath = _fileSystem.Path.Combine(dataDirectory, $"Interface/FontConfig_{isoLanguageString}.txt");
		if (_fileSystem.File.Exists(languageFilePath))
		{
			return languageFilePath;
		}

		// If the language-specific font config doesn't exist, use the default
		var defaultFilePath = _fileSystem.Path.Combine(dataDirectory, "Interface/FontConfig.txt");
		if (_fileSystem.File.Exists(defaultFilePath))
		{
			return defaultFilePath;
		}

		throw new FileNotFoundException();
	}

	public Stream GetStream(Language language)
	{
		return _fileSystem.File.OpenRead(GetFilePath(language));
	}
}
