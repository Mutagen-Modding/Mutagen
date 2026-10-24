using System.IO.Abstractions;
using Mutagen.Bethesda.Fallout3;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;
using Fallout3Light = Mutagen.Bethesda.Fallout3.Light;
using Fallout4Light = Mutagen.Bethesda.Fallout4.Light;
using OblivionLight = Mutagen.Bethesda.Oblivion.Light;
using SkyrimLight = Mutagen.Bethesda.Skyrim.Light;
using StarfieldFaction = Mutagen.Bethesda.Starfield.Faction;
using StarfieldLight = Mutagen.Bethesda.Starfield.Light;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class ShortSubrecordTests
{
    private const int ShorterThanAFloatOrFormID = 2;
    private static readonly RecordType FNAM = new("FNAM");
    private static readonly RecordType SNAM = new("SNAM");
    private static readonly RecordType LNAM = new("LNAM");
    private static readonly RecordType FVLD = new("FVLD");
    private static readonly RecordType GRPH = new("GRPH");

    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void Oblivion_ReadsAFieldOnlyFromASubrecordLongEnoughToHoldIt(
        IFileSystem fileSystem,
        OblivionMod mod,
        OblivionLight shortened,
        OblivionLight intact,
        float fade,
        DirectoryPath existingDirectory)
    {
        var sound = mod.GetNextFormKey();
        foreach (var light in new[] { shortened, intact })
        {
            light.Fade = fade;
            light.Sound.SetTo(sound);
        }
        var modPath = PathFor(mod, existingDirectory);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        ShortenSubrecords(fileSystem, modPath, mod.GameRelease, shortened.FormKey, FNAM, SNAM);

        var mutable = OblivionMod.Create(OblivionRelease.Oblivion)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        using var overlay = OblivionMod.Create(OblivionRelease.Oblivion)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        foreach (var reimport in new IOblivionModGetter[] { mutable, overlay })
        {
            reimport.Lights[shortened.FormKey].Fade.ShouldBeNull();
            reimport.Lights[shortened.FormKey].Sound.IsNull.ShouldBeTrue();
            reimport.Lights[intact.FormKey].Fade.ShouldBe(fade);
            reimport.Lights[intact.FormKey].Sound.FormKeyNullable.ShouldBe(sound);
        }
    }

    [Theory, MutagenModAutoData(GameRelease.Fallout3)]
    public void Fallout3_ReadsAFieldOnlyFromASubrecordLongEnoughToHoldIt(
        IFileSystem fileSystem,
        Fallout3Mod mod,
        Fallout3Light shortened,
        Fallout3Light intact,
        float fadeValue,
        DirectoryPath existingDirectory)
    {
        var sound = mod.GetNextFormKey();
        foreach (var light in new[] { shortened, intact })
        {
            light.FadeValue = fadeValue;
            light.Sound.SetTo(sound);
        }
        var modPath = PathFor(mod, existingDirectory);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        ShortenSubrecords(fileSystem, modPath, mod.GameRelease, shortened.FormKey, FNAM, SNAM);

        var mutable = Fallout3Mod.Create(Fallout3Release.Fallout3)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        using var overlay = Fallout3Mod.Create(Fallout3Release.Fallout3)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        foreach (var reimport in new IFallout3ModGetter[] { mutable, overlay })
        {
            reimport.Lights[shortened.FormKey].FadeValue.ShouldBe(default);
            reimport.Lights[shortened.FormKey].Sound.IsNull.ShouldBeTrue();
            reimport.Lights[intact.FormKey].FadeValue.ShouldBe(fadeValue);
            reimport.Lights[intact.FormKey].Sound.FormKeyNullable.ShouldBe(sound);
        }
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void Skyrim_ReadsAFieldOnlyFromASubrecordLongEnoughToHoldIt(
        IFileSystem fileSystem,
        SkyrimMod mod,
        SkyrimLight shortened,
        SkyrimLight intact,
        float fadeValue,
        DirectoryPath existingDirectory)
    {
        var sound = mod.GetNextFormKey();
        foreach (var light in new[] { shortened, intact })
        {
            light.FadeValue = fadeValue;
            light.Sound.SetTo(sound);
        }
        var modPath = PathFor(mod, existingDirectory);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        ShortenSubrecords(fileSystem, modPath, mod.GameRelease, shortened.FormKey, FNAM, SNAM);

        var mutable = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        using var overlay = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        foreach (var reimport in new ISkyrimModGetter[] { mutable, overlay })
        {
            reimport.Lights[shortened.FormKey].FadeValue.ShouldBe(default);
            reimport.Lights[shortened.FormKey].Sound.IsNull.ShouldBeTrue();
            reimport.Lights[intact.FormKey].FadeValue.ShouldBe(fadeValue);
            reimport.Lights[intact.FormKey].Sound.FormKeyNullable.ShouldBe(sound);
        }
    }

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void Fallout4_ReadsAFieldOnlyFromASubrecordLongEnoughToHoldIt(
        IFileSystem fileSystem,
        Fallout4Mod mod,
        Fallout4Light shortened,
        Fallout4Light intact,
        float fadeValue,
        DirectoryPath existingDirectory)
    {
        var sound = mod.GetNextFormKey();
        foreach (var light in new[] { shortened, intact })
        {
            light.FadeValue = fadeValue;
            light.Sound.SetTo(sound);
        }
        var modPath = PathFor(mod, existingDirectory);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        ShortenSubrecords(fileSystem, modPath, mod.GameRelease, shortened.FormKey, FNAM, SNAM);

        var mutable = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        using var overlay = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        foreach (var reimport in new IFallout4ModGetter[] { mutable, overlay })
        {
            reimport.Lights[shortened.FormKey].FadeValue.ShouldBe(default);
            reimport.Lights[shortened.FormKey].Sound.IsNull.ShouldBeTrue();
            reimport.Lights[intact.FormKey].FadeValue.ShouldBe(fadeValue);
            reimport.Lights[intact.FormKey].Sound.FormKeyNullable.ShouldBe(sound);
        }
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void Starfield_ReadsAFieldOnlyFromASubrecordLongEnoughToHoldIt(
        IFileSystem fileSystem,
        StarfieldMod mod,
        StarfieldLight shortened,
        StarfieldLight intact,
        float volumetricLightIntensityScale,
        DirectoryPath existingDirectory)
    {
        var lens = mod.GetNextFormKey();
        foreach (var light in new[] { shortened, intact })
        {
            light.VolumetricLightIntensityScale = volumetricLightIntensityScale;
            light.Lens.SetTo(lens);
        }
        var modPath = PathFor(mod, existingDirectory);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        ShortenSubrecords(fileSystem, modPath, mod.GameRelease, shortened.FormKey, FVLD, LNAM);

        var mutable = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        using var overlay = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Construct();
        foreach (var reimport in new IStarfieldModGetter[] { mutable, overlay })
        {
            reimport.Lights[shortened.FormKey].VolumetricLightIntensityScale.ShouldBeNull();
            reimport.Lights[shortened.FormKey].Lens.IsNull.ShouldBeTrue();
            reimport.Lights[intact.FormKey].VolumetricLightIntensityScale.ShouldBe(volumetricLightIntensityScale);
            reimport.Lights[intact.FormKey].Lens.FormKeyNullable.ShouldBe(lens);
        }
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void Starfield_ReadsAFieldAfterItsMarkerOnlyFromASubrecordLongEnoughToHoldIt(
        IFileSystem fileSystem,
        StarfieldMod mod,
        StarfieldFaction shortened,
        StarfieldFaction intact,
        float formationRadius,
        DirectoryPath existingDirectory)
    {
        shortened.FormationRadius = formationRadius;
        intact.FormationRadius = formationRadius;
        var modPath = PathFor(mod, existingDirectory);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        ShortenSubrecords(fileSystem, modPath, mod.GameRelease, shortened.FormKey, GRPH);

        var mutable = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        using var overlay = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Construct();
        foreach (var reimport in new IStarfieldModGetter[] { mutable, overlay })
        {
            reimport.Factions[shortened.FormKey].FormationRadius.ShouldBeNull();
            reimport.Factions[intact.FormKey].FormationRadius.ShouldBe(formationRadius);
        }
    }

    private static ModPath PathFor(IModGetter mod, DirectoryPath directory) =>
        new(mod.ModKey, Path.Combine(directory, mod.ModKey.FileName));

    private static void ShortenSubrecords(IFileSystem fileSystem, ModPath modPath, GameRelease release, FormKey shortened, params RecordType[] types)
    {
        var constants = GameConstants.Get(release);
        var file = new ReadOnlyMemorySlice<byte>(fileSystem.File.ReadAllBytes(modPath));
        var modHeaderLength = checked((int)constants.ModHeader(file).TotalLength);
        var group = constants.Group(file.Slice(modHeaderLength));

        using var writer = new MutagenWriter(modPath.Path, constants, fileSystem);
        writer.Write(file.Slice(0, modHeaderLength));
        using (HeaderExport.Group(writer, group.RecordType))
        {
            writer.Write(group.HeaderData.Slice(constants.GroupConstants.TypeAndLengthLength));
            foreach (var record in group.EnumerateMajorRecords().Select(x => x.Frame))
            {
                if (record.FormID.FullId != shortened.ID)
                {
                    writer.Write(record.HeaderAndContentData);
                    continue;
                }
                using (HeaderExport.Record(writer, record.RecordType))
                {
                    writer.Write(record.HeaderData.Slice(constants.MajorConstants.TypeAndLengthLength));
                    foreach (var subrecord in record.EnumerateSubrecords())
                    {
                        if (!types.Contains(subrecord.RecordType))
                        {
                            writer.Write(subrecord.HeaderAndContentData);
                            continue;
                        }
                        using (HeaderExport.Subrecord(writer, subrecord.RecordType))
                        {
                            writer.Write(subrecord.Content.Slice(0, ShorterThanAFloatOrFormID));
                        }
                    }
                }
            }
        }
    }
}
