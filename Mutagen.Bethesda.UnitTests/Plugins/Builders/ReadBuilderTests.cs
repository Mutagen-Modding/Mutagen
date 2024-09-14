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

public class ReadBuilderTests
{
    [Theory, MutagenAutoData]
    public void SeparatedNoMasterNoDataFolderWithHeaderMasters(
        IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingDir)
    {
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        var modPath = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.WriteToBinary(modPath, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });

        var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrderFromHeaderMasters()
            .WithNoDataFolder()
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal();
    }
    
    [Theory, MutagenAutoData]
    public void SeparatedNoDataFolderWithHeaderMasters(
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
            .WithLoadOrder(master)
            .ToPath(modPath)
            .WithFileSystem(fileSystem)
            .Write();

        try
        {
            var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
                .FromPath(modPath)
                .WithLoadOrderFromHeaderMasters()
                .WithNoDataFolder()
                .WithFileSystem(fileSystem)
                .Mutable()
                .Construct();
        }
        catch (Exception e)
        {
            if ((e as RecordException)?.InnerException is not MissingModMappingException)
            {
                throw new Exception();
            }
        }
    }
    
    [Theory, MutagenAutoData]
    public void SeparatedNoMasterNoLoadOrder(
        IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingDir)
    {
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        var modPath = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.WriteToBinary(modPath, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });

        var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal();
    }
    
    [Theory, MutagenAutoData]
    public void SeparatedNoLoadOrder(
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
            .WithLoadOrder(master)
            .ToPath(modPath)
            .WithFileSystem(fileSystem)
            .Write();

        try
        {
            var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
                .FromPath(modPath)
                .WithNoLoadOrder()
                .WithFileSystem(fileSystem)
                .Mutable()
                .Construct();
        }
        catch (Exception e)
        {
            if ((e as RecordException)?.InnerException is not MissingModMappingException)
            {
                throw new Exception();
            }
        }
    }
    
    [Theory, MutagenAutoData]
    public void SeparatedDataFolder(
        IFileSystem fileSystem,
        ModKey masterMod,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        var master = new StarfieldMod(masterMod, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var masterPath = Path.Combine(existingDataDir, masterMod.FileName);
        master.WriteToBinary(masterPath, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        mod.BeginWrite
            .WithLoadOrder(master)
            .ToPath(modPath)
            .WithFileSystem(fileSystem)
            .Write();

        var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal(masterMod);
    }
    
    [Theory, MutagenAutoData]
    public void SeparatedDataFolderMissingMaster(
        IFileSystem fileSystem,
        ModKey masterMod,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        var master = new StarfieldMod(masterMod, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        mod.BeginWrite
            .WithLoadOrder(master)
            .ToPath(modPath)
            .WithFileSystem(fileSystem)
            .Write();
        
        try
        {
            var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
                .FromPath(modPath)
                .WithLoadOrderFromHeaderMasters()
                .WithDataFolder(existingDataDir)
                .WithFileSystem(fileSystem)
                .Mutable()
                .Construct();
        }
        catch (Exception e)
        {
            if ((e as RecordException)?.InnerException is not MissingModMappingException)
            {
                throw new Exception();
            }
        }
    }
    
    [Theory, MutagenAutoData]
    public void SeparatedDataFolderMissingUnrelated(
        IFileSystem fileSystem,
        ModKey masterMod,
        ModKey unrelatedModKey,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        var unrelated = new StarfieldMod(unrelatedModKey, StarfieldRelease.Starfield);
        var master = new StarfieldMod(masterMod, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var masterPath = Path.Combine(existingDataDir, masterMod.FileName);
        master.WriteToBinary(masterPath, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        mod.BeginWrite
            .WithLoadOrder(master)
            .ToPath(modPath)
            .WithFileSystem(fileSystem)
            .Write();

        var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrder(masterMod, unrelatedModKey, modKey)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal(masterMod);
    }

    [Theory, MutagenAutoData]
    public void KnownMasterTests(
        IFileSystem fileSystem,
        ModKey masterMod,
        ModKey masterMod2,
        ModKey modKey,
        DirectoryPath existingDataDir)
    {
        var master = new StarfieldMod(masterMod, StarfieldRelease.Starfield);
        var masterWeapon = master.Weapons.AddNew();
        var masterPath = Path.Combine(existingDataDir, masterMod.FileName);
        master.WriteToBinary(masterPath, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });
        
        var master2 = new StarfieldMod(masterMod2, StarfieldRelease.Starfield);
        var master2Weapon = master2.Weapons.AddNew();
        
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.Weapons.GetOrAddAsOverride(masterWeapon);
        mod.Weapons.GetOrAddAsOverride(master2Weapon);
        var modPath = Path.Combine(existingDataDir, mod.ModKey.FileName);
        mod.BeginWrite
            .WithLoadOrder(masterMod, masterMod2)
            .WithDataFolder(existingDataDir)
            .ToPath(modPath)
            .WithKnownMasters(master2)
            .WithFileSystem(fileSystem)
            .Write();

        var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrder(masterMod, masterMod2)
            .WithDataFolder(existingDataDir)
            .WithFileSystem(fileSystem)
            .WithKnownMasters(master2)
            .Mutable()
            .Construct();
        reimport.MasterReferences.Select(x => x.Master).Should()
            .Equal(masterMod, masterMod2);
    }
}