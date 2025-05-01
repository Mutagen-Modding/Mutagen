using Shouldly;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim.Assets;

public class AssetTypeTests
{
    public IAssetType GetInstance<TAssetType>()
        where TAssetType : class, IAssetType
    {
        return TAssetType.Instance;
    }

    [Fact]
    public void TestAllImplementationsAreNotNull()
    {
        var methodInfo = typeof(AssetTypeTests).GetMethod("GetInstance");

        foreach (var implementation in typeof(IAssetType).GetInheritingFromInterface())
        {
            var method = methodInfo?.MakeGenericMethod(implementation);

            var instance = method?.Invoke(this, null);

            instance.ShouldNotBeNull();
        }

    }

    [Fact]
    public void TestAllImplementationsHaveNoBaseClass()
    {
        var objectType = typeof(object);
        
        foreach (var implementation in typeof(IAssetType).GetInheritingFromInterface())
        {
            implementation.BaseType.ShouldBe(objectType);
        }
    }

    [Fact]
    public void TestGetAssetType()
    {
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Music", "Special", "Dungeon", "DungeonBoss01.xwm")).ShouldBeOfType<SkyrimMusicAssetType>();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Music", "Special", "Dungeon", "DungeonBoss01.wav")).ShouldBeOfType<SkyrimMusicAssetType>();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("XMusic", "Special", "Dungeon", "DungeonBoss01.wav")).ShouldBeNull();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Music", "Special", "Dungeon", "DungeonBoss01.mp3")).ShouldBeNull();

        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Meshes", "Armor", "Iron", "IronGauntlets.nif")).ShouldBeOfType<SkyrimModelAssetType>();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("XMeshes", "Armor", "Iron", "IronGauntlets.nif")).ShouldBeNull();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Meshes", "Armor", "Iron", "IronGauntlets.abc")).ShouldBeNull();

        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Textures", "Armor", "Iron", "IronGauntlets.dds")).ShouldBeOfType<SkyrimTextureAssetType>();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("XTextures", "Armor", "Iron", "IronGauntlets.dds")).ShouldBeNull();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Textures", "Armor", "Iron", "IronGauntlets.abc")).ShouldBeNull();

        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Interface", "map.swf")).ShouldBeOfType<SkyrimInterfaceAssetType>();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("XInterface", "map.swf")).ShouldBeNull();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Interface", "map.swv")).ShouldBeNull();

        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Sound", "FX", "IronGauntlets.wav")).ShouldBeOfType<SkyrimSoundAssetType>();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("XSound", "FX", "IronGauntlets.wav")).ShouldBeNull();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Sound", "FX", "IronGauntlets.abc")).ShouldBeNull();

        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("SEQ", "Skyrim.seq")).ShouldBeOfType<SkyrimSeqAssetType>();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("seq", "Skyrim.seq")).ShouldBeOfType<SkyrimSeqAssetType>();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("s3q", "Skyrim.abc")).ShouldBeNull();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("SEQ", "Skyrim.abc")).ShouldBeNull();

        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Scripts", "Source", "HelloWorld.psc")).ShouldBeOfType<SkyrimScriptSourceAssetType>();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Script", "Source", "HelloWorld.psc")).ShouldBeNull();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Scripts", "Source", "HelloWorld.abc")).ShouldBeNull();

        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Scripts", "HelloWorld.pex")).ShouldBeOfType<SkyrimScriptCompiledAssetType>();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Script", "HelloWorld.pex")).ShouldBeNull();
        AssetTypeLocator.TryGetGetAssetType(GameCategory.Skyrim, Path.Combine("Scripts", "HelloWorld.abc")).ShouldBeNull();
    }
}
