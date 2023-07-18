using FluentAssertions;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Skyrim.Assets;

public class AssetTypeTests
{
#if NET7_0_OR_GREATER
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

            instance.Should().NotBeNull();
        }

    }

    [Fact]
    public void TestAllImplementationsHaveNoBaseClass()
    {
        var objectType = typeof(object);
        
        foreach (var implementation in typeof(IAssetType).GetInheritingFromInterface())
        {
            implementation.BaseType.Should().Be(objectType);
        }
    }
#endif

    [Fact]
    public void TestGetAssetType()
    {
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Music", "Special", "Dungeon", "DungeonBoss01.xwm")).Should().BeOfType<SkyrimMusicAssetType>();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Music", "Special", "Dungeon", "DungeonBoss01.wav")).Should().BeOfType<SkyrimMusicAssetType>();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("XMusic", "Special", "Dungeon", "DungeonBoss01.wav")).Should().BeNull();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Music", "Special", "Dungeon", "DungeonBoss01.mp3")).Should().BeNull();

        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Meshes", "Armor", "Iron", "IronGauntlets.nif")).Should().BeOfType<SkyrimModelAssetType>();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("XMeshes", "Armor", "Iron", "IronGauntlets.nif")).Should().BeNull();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Meshes", "Armor", "Iron", "IronGauntlets.abc")).Should().BeNull();

        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Textures", "Armor", "Iron", "IronGauntlets.dds")).Should().BeOfType<SkyrimTextureAssetType>();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("XTextures", "Armor", "Iron", "IronGauntlets.dds")).Should().BeNull();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Textures", "Armor", "Iron", "IronGauntlets.abc")).Should().BeNull();

        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Sound", "FX", "IronGauntlets.wav")).Should().BeOfType<SkyrimSoundAssetType>();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("XSound", "FX", "IronGauntlets.wav")).Should().BeNull();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Sound", "FX", "IronGauntlets.abc")).Should().BeNull();

        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("SEQ", "Skyrim.seq")).Should().BeOfType<SkyrimSeqAssetType>();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("seq", "Skyrim.seq")).Should().BeOfType<SkyrimSeqAssetType>();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("s3q", "Skyrim.abc")).Should().BeNull();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("SEQ", "Skyrim.abc")).Should().BeNull();

        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Scripts", "Source", "HelloWorld.psc")).Should().BeOfType<SkyrimScriptSourceAssetType>();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Script", "Source", "HelloWorld.psc")).Should().BeNull();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Scripts", "Source", "HelloWorld.abc")).Should().BeNull();

        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Scripts", "HelloWorld.pex")).Should().BeOfType<SkyrimScriptCompiledAssetType>();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Script", "HelloWorld.pex")).Should().BeNull();
        IAssetType.GetAssetType(GameCategory.Skyrim, Path.Combine("Scripts", "HelloWorld.abc")).Should().BeNull();
    }
}
