using System.IO.Abstractions;
using IniParser;
using IniParser.Model.Configuration;
using IniParser.Parser;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Inis.DI;
using Noggog;
namespace Mutagen.Bethesda.Fonts.DI;

public interface IGetFontConfigListing
{
	/// <summary>
	/// Queries the related ini file and looks for the font configuration asset
	/// </summary>
	/// <returns>The file path of the font configuration asset</returns>
	AssetPath? Get();

	/// <summary>
	/// Queries the related ini file and looks for the font configuration asset
	/// </summary>
	/// <param name="path">Path to the file containing INI data</param>
	/// <returns>The file path of the font configuration asset</returns>
	AssetPath? Get(FilePath path);

	/// <summary>
	/// Queries the related ini file and looks for the font configuration asset
	/// </summary>
	/// <param name="iniStream">Stream containing INI data</param>
	/// <returns>The file path of the font configuration asset</returns>
	AssetPath? Get(Stream iniStream);
}

public sealed class GetFontConfigListing : IGetFontConfigListing
{
	private static readonly IniParserConfiguration Config = new()
	{
		AllowDuplicateKeys = true,
		AllowDuplicateSections = true,
		AllowKeysWithoutSection = true,
		AllowCreateSectionsOnFly = true,
		CaseInsensitive = true,
		SkipInvalidLines = true,
	};

	private readonly IFileSystem _fileSystem;
	private readonly IIniPathProvider _iniPathProvider;

	public GetFontConfigListing(
		IFileSystem fileSystem,
		IIniPathProvider iniPathProvider)
	{
		_fileSystem = fileSystem;
		_iniPathProvider = iniPathProvider;
	}

	/// <inheritdoc />
	public AssetPath? Get()
	{
		return Get(_iniPathProvider.Path);
	}

	/// <inheritdoc />
	public AssetPath? Get(FilePath path)
	{
		if (!_fileSystem.File.Exists(path)) return null;

		var fileStream = _fileSystem.File.OpenRead(path.Path);
		return Get(fileStream);
	}

	/// <inheritdoc />
	public AssetPath? Get(Stream iniStream)
	{
		// Release exists as parameter, in case future games need different handling
		Console.WriteLine("GetFontConfigListing.Get");
		var iniDataParser = new IniDataParser(Config);
		Console.WriteLine("GetFontConfigListing.GetXX");
		var parser = new FileIniDataParser(iniDataParser);
		Console.WriteLine("GetFontConfigListing.Get2");
		var data = parser.ReadData(new StreamReader(iniStream));
		Console.WriteLine("GetFontConfigListing.Get3");
		var basePath = data["Fonts"];
		var configFile = basePath["sFontConfigFile"];
		Console.WriteLine("GetFontConfigListing.Get4");
		if (configFile is null) return null;
		Console.WriteLine("GetFontConfigListing.Get5");
		Console.WriteLine("GetFontConfigListing.Get6" + configFile);

		return configFile;
	}
}
