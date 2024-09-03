using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
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
    public void WithAllParentMasters(
        IFileSystem fileSystem,
        ModKey transientMasterModKey,
        ModKey masterModKey,
        ModKey modKey,
        DirectoryPath existingDir)
    {
        var transientMasterMod = new StarfieldMod(transientMasterModKey, StarfieldRelease.Starfield);
        var transientMasterWeapon = transientMasterMod.Weapons.AddNew();
        var transientMasterModPath = Path.Combine(existingDir, transientMasterMod.ModKey.FileName);
        transientMasterMod.BeginWrite
            .WithLoadOrderFromHeaderMasters()
            .WithNoDataFolder()
            .ToPath(transientMasterModPath)
            .WithFileSystem(fileSystem)
            .Write();
        
        var master = new StarfieldMod(masterModKey, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        master.Weapons.GetOrAddAsOverride(transientMasterWeapon);
        var masterModPath = Path.Combine(existingDir, master.ModKey.FileName);
        master.BeginWrite
            .WithLoadOrderFromHeaderMasters()
            .WithNoDataFolder()
            .ToPath(masterModPath)
            .WithFileSystem(fileSystem)
            .Write();
        
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        
        var modPath = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.BeginWrite
            .WithLoadOrder(transientMasterModKey, masterModKey)
            .WithNoDataFolder()
            .ToPath(modPath)
            .WithFileSystem(fileSystem)
            .WithAllParentMasters()
            .Write();

        using var reimport = StarfieldMod.CreateFromBinaryOverlay(modPath, StarfieldRelease.Starfield,
            new BinaryReadParameters()
            {
                FileSystem = fileSystem
            });
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal(transientMasterModKey, masterModKey);
    }
}