using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;
using Xunit;
using Fo4 = Mutagen.Bethesda.Fallout4;
using Ob = Mutagen.Bethesda.Oblivion;
using Sk = Mutagen.Bethesda.Skyrim;
using Sf = Mutagen.Bethesda.Starfield;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class RecordCountTests
{
    private static void WrittenCountMatchesFile(IModGetter mod, GameRelease release)
    {
        using var stream = new MemoryStream();
        mod.WriteToBinary(stream);
        var bytes = stream.ToArray();

        var meta = GameConstants.Get(release);
        var majorHeaderLength = meta.MajorConstants.HeaderLength;
        var headerCount = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(majorHeaderLength + 10));

        uint fileCount = 0;
        var pos = majorHeaderLength + checked((int)BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4)));
        while (pos < bytes.Length)
        {
            fileCount++;
            var length = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(pos + 4)));
            pos += bytes.AsSpan(pos, 4).SequenceEqual("GRUP"u8)
                ? meta.GroupConstants.HeaderLength
                : majorHeaderLength + length;
        }

        headerCount.ShouldBe(fileCount);
    }

    [Theory, MutagenAutoData]
    public void SkyrimWorldspaceAndDialogGroupsCounted(ModKey modKey)
    {
        var mod = new Sk.SkyrimMod(modKey, Sk.SkyrimRelease.SkyrimSE);
        var topCellOnly = mod.Worldspaces.AddNew();
        topCellOnly.TopCell = new Sk.Cell(mod);
        topCellOnly.TopCell.Persistent.Add(new Sk.PlacedObject(mod));
        var blocksOnly = mod.Worldspaces.AddNew();
        var subBlock = new Sk.WorldspaceSubBlock();
        var cell = new Sk.Cell(mod);
        cell.Temporary.Add(new Sk.PlacedObject(mod));
        subBlock.Items.Add(cell);
        var block = new Sk.WorldspaceBlock();
        block.Items.Add(subBlock);
        blocksOnly.SubCells.Add(block);
        mod.DialogTopics.AddNew();
        mod.DialogTopics.AddNew().Responses.Add(new Sk.DialogResponses(mod));

        WrittenCountMatchesFile(mod, GameRelease.SkyrimSE);
    }

    [Theory, MutagenAutoData]
    public void OblivionWorldspaceAndDialogGroupsCounted(ModKey modKey)
    {
        var mod = new Ob.OblivionMod(modKey, Ob.OblivionRelease.Oblivion);
        var worldspace = mod.Worldspaces.AddNew();
        worldspace.TopCell = new Ob.Cell(mod);
        worldspace.TopCell.Persistent.Add(new Ob.PlacedObject(mod));
        mod.DialogTopics.AddNew();
        mod.DialogTopics.AddNew().Items.Add(new Ob.DialogItem(mod));

        WrittenCountMatchesFile(mod, GameRelease.Oblivion);
    }

    [Theory, MutagenAutoData]
    public void Fallout4WorldspaceGroupsCounted(ModKey modKey)
    {
        var mod = new Fo4.Fallout4Mod(modKey, Fo4.Fallout4Release.Fallout4);
        var worldspace = mod.Worldspaces.AddNew();
        worldspace.TopCell = new Fo4.Cell(mod);
        worldspace.TopCell.Persistent.Add(new Fo4.PlacedObject(mod));

        WrittenCountMatchesFile(mod, GameRelease.Fallout4);
    }

    [Theory, MutagenAutoData]
    public void StarfieldWorldspaceGroupsCounted(ModKey modKey)
    {
        var mod = new Sf.StarfieldMod(modKey, Sf.StarfieldRelease.Starfield);
        var worldspace = mod.Worldspaces.AddNew();
        worldspace.TopCell = new Sf.Cell(mod);
        worldspace.TopCell.Persistent.Add(new Sf.PlacedObject(mod));

        WrittenCountMatchesFile(mod, GameRelease.Starfield);
    }
}
