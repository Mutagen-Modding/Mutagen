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
        Assert.Throws<MissingModMappingException>(() =>
        {
            mod.BeginWrite
                .ToPath(modPath)
                .WithLoadOrderFromHeaderMasters()
                .WithNoDataFolder()
                .WithFileSystem(fileSystem)
                .Write();
        });
    }
    
    [Theory, MutagenAutoData]
    public void ExtraIncludedMasters(
        IFileSystem fileSystem,
        ModKey masterModKey,
        ModKey modKey,
        ModKey extraMasterKey,
        DirectoryPath existingDir)
    {
        var extraMaster = new StarfieldMod(extraMasterKey, StarfieldRelease.Starfield);
        var master = new StarfieldMod(masterModKey, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        var modPath = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithLoadOrder(master, extraMaster)
            .WithFileSystem(fileSystem)
            .WithExtraIncludedMasters(extraMasterKey)
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrder(master, extraMaster)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal(masterModKey, extraMasterKey);
    }
    
    [Theory, MutagenAutoData]
    public void OverrideMasters(
        IFileSystem fileSystem,
        ModKey masterModKey,
        ModKey modKey,
        ModKey overrideMasterKey,
        DirectoryPath existingDir)
    {
        var overrideMod = new StarfieldMod(overrideMasterKey, StarfieldRelease.Starfield);
        var master = new StarfieldMod(masterModKey, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        var modPath = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithLoadOrder(master, overrideMod)
            .WithFileSystem(fileSystem)
            .WithExplicitOverridingMasterList(overrideMasterKey, masterModKey)
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrder(master, overrideMod)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal(masterModKey, overrideMasterKey);
    }
    
    [Theory, MutagenAutoData]
    public void WithAllParentMasters(
        IFileSystem fileSystem,
        ModKey transientMasterModKey,
        ModKey masterModKey,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        var transientMasterMod = new StarfieldMod(transientMasterModKey, StarfieldRelease.Starfield);
        var transientMasterWeapon = transientMasterMod.Weapons.AddNew();
        var transientMasterModPath = Path.Combine(existingDataDir, transientMasterMod.ModKey.FileName);
        transientMasterMod.BeginWrite
            .ToPath(transientMasterModPath)
            .WithLoadOrder(transientMasterModKey, masterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Write();
        
        var master = new StarfieldMod(masterModKey, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        master.Weapons.GetOrAddAsOverride(transientMasterWeapon);
        var masterModPath = Path.Combine(existingDataDir, master.ModKey.FileName);
        master.BeginWrite
            .ToPath(masterModPath)
            .WithLoadOrder(transientMasterModKey, masterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Write();
        
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        
        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithLoadOrder(transientMasterModKey, masterModKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .WithAllParentMasters()
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithKnownMasters(transientMasterMod, master)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal(transientMasterModKey, masterModKey);
    }
}