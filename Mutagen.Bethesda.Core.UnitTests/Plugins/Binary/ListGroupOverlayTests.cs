using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary;

public abstract class AListGroupOverlayTests
{
    protected abstract GameRelease Release { get; }

    private GameConstants Constants => GameConstants.Get(Release);

    private byte[] ListGroupHolding(Action<MutagenWriter> writeEntry)
    {
        var stream = new MemoryStream();
        using (var writer = new MutagenWriter(stream, Constants))
        {
            using (HeaderExport.Group(writer, RecordTypes.GRUP))
            {
                writer.WriteZeros(Constants.GroupConstants.LengthAfterLength);
                writeEntry(writer);
            }
        }
        return stream.ToArray();
    }

    private IReadOnlyList<IMajorRecordGetter> ParseLocations(byte[] listGroup, ModKey modKey)
    {
        var meta = new ParsingMeta(
            Constants,
            modKey,
            SeparatedMasterPackage.NotSeparate(new MasterReferenceCollection(modKey)));
        var stream = new OverlayStream(listGroup, meta);
        stream.Position = Constants.GroupConstants.HeaderLength;
        return GroupListOverlay<IMajorRecordGetter>.Factory(
            stream,
            listGroup,
            new BinaryOverlayFactoryPackage(meta),
            offset: 0);
    }

    [Theory, MutagenAutoData]
    public void RecordWhereAGroupBelongs_ThrowsNamingTheRecordType(ModKey modKey)
    {
        var listGroup = ListGroupHolding(writer =>
        {
            using (HeaderExport.Record(writer, RecordTypes.MISC))
            {
                writer.WriteZeros(Constants.MajorConstants.LengthAfterLength);
            }
        });

        Should.Throw<RecordException>(() => ParseLocations(listGroup, modKey))
            .Message.ShouldContain(RecordTypes.MISC.Type);
    }

    [Theory, MutagenAutoData]
    public void GroupShorterThanItsHeader_ThrowsAsMalformed(ModKey modKey)
    {
        var listGroup = ListGroupHolding(writer =>
        {
            writer.Write(RecordTypes.GRUP.TypeInt);
            writer.WriteZeros(Constants.GroupConstants.LengthAfterType);
        });

        Should.Throw<MalformedDataException>(() => ParseLocations(listGroup, modKey));
    }

    [Theory, MutagenAutoData]
    public void EmptyGroup_ReadsAsOneEntry(ModKey modKey)
    {
        var listGroup = ListGroupHolding(writer =>
        {
            using (HeaderExport.Group(writer, RecordTypes.GRUP))
            {
                writer.WriteZeros(Constants.GroupConstants.LengthAfterLength);
            }
        });

        ParseLocations(listGroup, modKey).Count.ShouldBe(1);
    }
}

public class OblivionListGroupOverlayTests : AListGroupOverlayTests
{
    protected override GameRelease Release => GameRelease.Oblivion;
}

public class Fallout3ListGroupOverlayTests : AListGroupOverlayTests
{
    protected override GameRelease Release => GameRelease.Fallout3;
}

public class SkyrimListGroupOverlayTests : AListGroupOverlayTests
{
    protected override GameRelease Release => GameRelease.SkyrimSE;
}

public class Fallout4ListGroupOverlayTests : AListGroupOverlayTests
{
    protected override GameRelease Release => GameRelease.Fallout4;
}

public class StarfieldListGroupOverlayTests : AListGroupOverlayTests
{
    protected override GameRelease Release => GameRelease.Starfield;
}
