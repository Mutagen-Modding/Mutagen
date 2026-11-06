using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;
using static Mutagen.Bethesda.UnitTests.Plugins.Records.RawRecordBytes;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

public class ConstructibleObjectNameTests
{
    private const uint WorkbenchKeywordId = 0x801;

    private static readonly GameConstants Constants = GameConstants.Starfield;

    private static byte[] CobjBytes(string editorId, string? name, string description)
    {
        var subrecords = new List<(string Type, byte[] Content)>();
        subrecords.Add(("EDID", Str(editorId)));
        if (name != null)
        {
            subrecords.Add(("FULL", Str(name)));
        }
        subrecords.Add(("DESC", Str(description)));
        subrecords.Add(("BNAM", U32(WorkbenchKeywordId)));
        subrecords.Add(("NNAM", new byte[sizeof(ushort)]));
        subrecords.Add(("SNAM", new byte[sizeof(float)]));
        subrecords.Add(("TNAM", new byte[sizeof(byte)]));
        subrecords.Add(("DATA", U32(0)));
        return Record(Constants, "COBJ", subrecords.ToArray());
    }

    private static IEnumerable<IConstructibleObjectGetter> ReadBothStrict(byte[] bytes)
    {
        var meta = Meta(Constants);
        meta.ThrowOnUnknown = true;
        yield return ConstructibleObject.CreateFromBinary(new MutagenFrame(new MutagenMemoryReadStream(bytes, meta)));
        yield return ConstructibleObjectBinaryOverlay.ConstructibleObjectFactory(new OverlayStream(bytes, meta), new BinaryOverlayFactoryPackage(meta));
    }

    private static byte[] Write(IConstructibleObjectGetter record)
    {
        var masters = new MasterReferenceCollection(TestModKey);
        var bundle = new WritingBundle(Constants)
        {
            MasterReferences = masters,
            SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(masters),
        };
        var stream = new MemoryStream();
        using (var writer = new MutagenWriter(stream, bundle, dispose: false))
        {
            record.WriteToBinary(writer);
        }
        return stream.ToArray();
    }

    private static void WriteAndCheckName(
        IFileSystem fileSystem,
        StarfieldMod mod,
        ConstructibleObject cobj,
        string name,
        DirectoryPath existingDirectory,
        bool localized)
    {
        mod.UsingLocalization = localized;
        cobj.Name = name;
        var modPath = new ModPath(mod.ModKey, Path.Combine(existingDirectory, mod.ModKey.FileName));
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();

        var readParams = new BinaryReadParameters()
        {
            FileSystem = fileSystem,
            ThrowOnUnknownSubrecord = true
        };
        using var overlay = StarfieldMod.CreateFromBinaryOverlay(modPath, StarfieldRelease.Starfield, readParams);
        overlay.UsingLocalization.ShouldBe(localized);
        overlay.ConstructibleObjects.ShouldHaveSingleItem().Name!.String.ShouldBe(name);

        if (!localized)
        {
            // Deep reads do not resolve strings files from a fake file system (same for Weapon names),
            // so the localized case is only checked through the overlay
            var deep = StarfieldMod.CreateFromBinary(modPath, StarfieldRelease.Starfield, readParams);
            deep.ConstructibleObjects.ShouldHaveSingleItem().Name!.String.ShouldBe(name);
        }
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void Full_ReadsName(string editorId, string name, string description)
    {
        foreach (var cobj in ReadBothStrict(CobjBytes(editorId, name, description)))
        {
            cobj.EditorID.ShouldBe(editorId);
            cobj.Name.ShouldNotBeNull();
            cobj.Name.String.ShouldBe(name);
            ((INamedGetter)cobj).Name.ShouldBe(name);
            cobj.Description.ShouldNotBeNull();
            cobj.Description.String.ShouldBe(description);
            cobj.WorkbenchKeyword.FormKey.ShouldBe(new FormKey(TestModKey, WorkbenchKeywordId));
        }
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void NoFull_NameIsNull(string editorId, string description)
    {
        foreach (var cobj in ReadBothStrict(CobjBytes(editorId, null, description)))
        {
            cobj.Name.ShouldBeNull();
            cobj.Description.ShouldNotBeNull();
            cobj.Description.String.ShouldBe(description);
        }
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void Full_WritesSameBytes(string editorId, string name, string description)
    {
        var bytes = CobjBytes(editorId, name, description);
        foreach (var cobj in ReadBothStrict(bytes))
        {
            Write(cobj).ShouldBe(bytes);
        }
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void NoFull_WritesSameBytes(string editorId, string description)
    {
        var bytes = CobjBytes(editorId, null, description);
        foreach (var cobj in ReadBothStrict(bytes))
        {
            Write(cobj).ShouldBe(bytes);
        }
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void ModRoundTrip_KeepsName(
        IFileSystem fileSystem,
        StarfieldMod mod,
        ConstructibleObject cobj,
        string name,
        DirectoryPath existingDirectory)
    {
        WriteAndCheckName(fileSystem, mod, cobj, name, existingDirectory, localized: false);
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void LocalizedModRoundTrip_KeepsName(
        IFileSystem fileSystem,
        StarfieldMod mod,
        ConstructibleObject cobj,
        string name,
        DirectoryPath existingDirectory)
    {
        WriteAndCheckName(fileSystem, mod, cobj, name, existingDirectory, localized: true);
        fileSystem.File.Exists(Path.Combine(existingDirectory, "Strings", $"{mod.ModKey.Name}_en.STRINGS"))
            .ShouldBeTrue();
    }
}