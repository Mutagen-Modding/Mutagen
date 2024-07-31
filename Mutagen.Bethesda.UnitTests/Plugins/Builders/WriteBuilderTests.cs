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
}