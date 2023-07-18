using System.Reflection;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
namespace Mutagen.Bethesda.Assets; 

#if NET7_0_OR_GREATER
public class AssetTypeLocator
{
	private static readonly Dictionary<GameCategory, Dictionary<string,Dictionary<string,IAssetType>>> Types;

	static AssetTypeLocator()
	{
		Types = new Dictionary<GameCategory, Dictionary<string,Dictionary<string,IAssetType>>>();

		foreach (var type in typeof(IAssetType).GetInheritingFromInterface())
		{
			if (type.Namespace == null) continue;

			GameCategory gameCategory;
			switch (type.Namespace.TrimStart("Mutagen.Bethesda."))
			{
				case "Oblivion.Assets":
					gameCategory = GameCategory.Oblivion;
					break;
				case "Skyrim.Assets":
					gameCategory = GameCategory.Skyrim;
					break;
				case "Fallout4.Assets":
					gameCategory = GameCategory.Fallout4;
					break;
				default:
					continue;
			}

			var instanceProperty = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
			if (instanceProperty?.GetValue(null) is not IAssetType assetType) continue;

			var gameTypes = Types.GetOrAdd(gameCategory);
			foreach (var extension in assetType.FileExtensions)
			{
				var baseFolders = gameTypes.GetOrAdd(extension, () => new Dictionary<string, IAssetType>());
				baseFolders.TryAdd(assetType.BaseFolder, assetType);
			}
		}
	}

	/// <summary>
	/// Parse asset type by game release and path
	/// </summary>
	/// <param name="gameCategory">Release of the game this asset comes from</param>
	/// <param name="path">Path of the asset</param>
	/// <returns>Instance of the parsed asset type</returns>
	/// <exception cref="ArgumentException">When the asset type couldn't be determined</exception>
	public static IAssetType GetAssetType(GameCategory gameCategory, string path) {
		var assetType = TryGetGetAssetType(gameCategory, path);

		if (assetType is null) {
			throw new ArgumentException($"Could not determine asset type for {path} in {gameCategory}");
		}

		return assetType;
	}

	/// <summary>
	/// Parse asset type by game release and path
	/// </summary>
	/// <param name="gameCategory">Release of the game this asset comes from</param>
	/// <param name="path">Path of the asset</param>
	/// <returns>Instance of the parsed asset type or null if no asset type could be determined</returns>
	public static IAssetType? TryGetGetAssetType(GameCategory gameCategory, string path)
	{
		// Get dictionary for game category
		if (!Types.TryGetValue(gameCategory, out var gameTypes)) return null;

		// Get dictionary for file extension
		if (!gameTypes.TryGetValue(Path.GetExtension(path), out var folders)) return null;

		// Get asset type from base folder
		var dataRelativePath = AssetLink.ConvertToDataRelativePath(path);
		foreach (var (baseFolder, assetType) in folders)
		{
			if (dataRelativePath.StartsWith(baseFolder, AssetLink.PathComparison))
			{
				return assetType;
			}
		}

		return null;
	}
}
#endif