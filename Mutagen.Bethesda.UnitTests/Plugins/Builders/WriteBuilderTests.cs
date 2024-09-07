using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Builders;

public class WriteBuilderTests
{
    [Theory, MutagenAutoData]
    public void NoDataFolderWithHeaderMasters(
        IFileSystem fileSystem,
        ModKey masterMod,
        ModKey modKey,
        DirectoryPath existingDir)
    {
        var master = new StarfieldMod(masterMod, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        var modPath = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.BeginWrite
            .WithLoadOrderFromHeaderMasters()
            .WithNoDataFolder()
            .ToPath(modPath)
            .WithFileSystem(fileSystem)
            .Write();

        using var reimport = StarfieldMod.CreateFromBinaryOverlay(modPath, StarfieldRelease.Starfield,
            new BinaryReadParameters()
            {
                FileSystem = fileSystem
            });
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal(masterMod);
    }
    
    [Theory, MutagenAutoData]
    public void ExtraIncludedMasters(
        IFileSystem fileSystem,
        ModKey masterMod,
        ModKey modKey,
        ModKey extraMaster,
        DirectoryPath existingDir)
    {
        var master = new StarfieldMod(masterMod, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        var modPath = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.BeginWrite
            .WithLoadOrder(masterMod, extraMaster)
            .WithNoDataFolder()
            .ToPath(modPath)
            .WithFileSystem(fileSystem)
            .WithExtraIncludedMasters(extraMaster)
            .Write();

        using var reimport = StarfieldMod.CreateFromBinaryOverlay(modPath, StarfieldRelease.Starfield,
            new BinaryReadParameters()
            {
                FileSystem = fileSystem
            });
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal(masterMod, extraMaster);
    }
    
    [Theory, MutagenAutoData]
    public void OverrideMasters(
        IFileSystem fileSystem,
        ModKey masterMod,
        ModKey modKey,
        ModKey overrideMaster,
        DirectoryPath existingDir)
    {
        var master = new StarfieldMod(masterMod, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        var modPath = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.BeginWrite
            .WithLoadOrder(masterMod, overrideMaster)
            .WithNoDataFolder()
            .ToPath(modPath)
            .WithFileSystem(fileSystem)
            .WithExplicitOverridingMasterList(overrideMaster, masterMod)
            .Write();

        using var reimport = StarfieldMod.CreateFromBinaryOverlay(modPath, StarfieldRelease.Starfield,
            new BinaryReadParameters()
            {
                FileSystem = fileSystem
            });
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal(masterMod, overrideMaster);
    }
    
    [Theory, MutagenAutoData]
    public void WithLoadOrderTrimsSelfMod(
        IFileSystem fileSystem,
        ModKey otherModKey,
        ModKey modKey,
        ModKey afterModKey,
        DirectoryPath existingDataDir)
    {
        var master = new StarfieldMod(otherModKey, StarfieldRelease.Starfield);
        var weapon = master.Weapons.AddNew();
        var masterModPath = Path.Combine(existingDataDir, master.ModKey.FileName);
        master.BeginWrite
            .WithLoadOrder(otherModKey, modKey, afterModKey)
            .WithDataFolder(existingDataDir)
            .ToPath(masterModPath)
            .WithFileSystem(fileSystem)
            .Write();
        
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(weapon);
        
        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        mod.BeginWrite
            .WithLoadOrder(otherModKey, modKey, afterModKey)
            .WithDataFolder(existingDataDir)
            .ToPath(modPath)
            .WithFileSystem(fileSystem)
            .Write();

        using var reimport = StarfieldMod.CreateFromBinaryOverlay(modPath, StarfieldRelease.Starfield,
            new BinaryReadParameters()
            {
                FileSystem = fileSystem
            });
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal(otherModKey);
    }
    
    [Theory, MutagenAutoData]
    public void WithLoadOrderNoTrim(
        IFileSystem fileSystem,
        ModKey otherModKey,
        ModKey modKey,
        ModKey afterModKey,
        DirectoryPath existingDataDir)
    {
        var master = new StarfieldMod(otherModKey, StarfieldRelease.Starfield);
        var masterModPath = Path.Combine(existingDataDir, master.ModKey.FileName);
        Assert.Throws<MissingModException>(() =>
        {
            master.BeginWrite
                .WithLoadOrder(otherModKey, modKey, afterModKey)
                .WithDataFolder(existingDataDir)
                .ToPath(masterModPath)
                .WithFileSystem(fileSystem)
                .DoNotTrimLoadOrderAtSelf()
                .Write();
        });
    }
}