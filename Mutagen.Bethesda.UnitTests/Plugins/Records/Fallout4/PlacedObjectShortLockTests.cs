using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Xunit;
using Fallout4Constants = Mutagen.Bethesda.Fallout4.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class PlacedObjectShortLockTests
{
    private const int ShortLockLength = 12;
    private const int FullLockLength = 16;
    private const LockLevel Level = (LockLevel)0xFE;
    private const short TeleportMarkerTriangle = 0x01C0;
    private const float Scale = 1.04f;
    private const ushort FormVersion = 131;

    private static readonly MasterReferenceCollection Masters = new(Fallout4Constants.Fallout4);

    private static byte[] BuildRefrBytesWithLock(int lockLength)
    {
        var lockData = new byte[lockLength];
        lockData[0] = (byte)Level;
        var navmeshDoorLink = new byte[8];
        BinaryPrimitives.WriteInt16LittleEndian(navmeshDoorLink.AsSpan(4), TeleportMarkerTriangle);
        var content = new List<byte>();
        content.AddRange(Subrecord("NAME", BitConverter.GetBytes(0x000314F9u)));
        content.AddRange(Subrecord("XLOC", lockData));
        content.AddRange(Subrecord("XNDP", navmeshDoorLink));
        content.AddRange(Subrecord("XSCL", BitConverter.GetBytes(Scale)));
        content.AddRange(Subrecord("DATA", new byte[24]));
        var contentBytes = content.ToArray();

        var header = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("REFR").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)contentBytes.Length));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), FormVersion);

        return header.Concat(contentBytes).ToArray();
    }

    private static byte[] Subrecord(string type, byte[] content)
    {
        var header = new byte[6];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(4), checked((ushort)content.Length));
        return header.Concat(content).ToArray();
    }

    private static ParsingMeta Meta() => new(
        GameConstants.Fallout4,
        Fallout4Constants.Fallout4,
        SeparatedMasterPackage.NotSeparate(Masters));

    private static IPlacedObjectGetter ParseDeep(byte[] bytes)
    {
        using var reader = new MutagenBinaryReadStream(new MemoryStream(bytes), Meta());
        return PlacedObject.CreateFromBinary(new MutagenFrame(reader));
    }

    private static IPlacedObjectGetter ParseOverlay(byte[] bytes)
    {
        return PlacedObjectBinaryOverlay.PlacedObjectFactory(bytes, new BinaryOverlayFactoryPackage(Meta()));
    }

    private static byte[] Write(IPlacedObjectGetter item)
    {
        var stream = new MemoryStream();
        using (var writer = new MutagenWriter(
                   stream,
                   new WritingBundle(GameConstants.Fallout4)
                   {
                       MasterReferences = Masters,
                       SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(Masters),
                   }))
        {
            PlacedObjectBinaryWriteTranslation.Instance.Write(writer, item, default);
        }
        return stream.ToArray();
    }

    private static int LockLength(byte[] record) =>
        RecordSpanExtensions.FindAllOfSubrecord(
                new ReadOnlyMemorySlice<byte>(record).Slice(GameConstants.Fallout4.MajorConstants.HeaderLength),
                GameConstants.Fallout4,
                RecordTypes.XLOC)
            .Single().ContentLength;

    private static void ShouldReadTheLockAndWhatFollows(IPlacedObjectGetter item)
    {
        item.Lock.ShouldNotBeNull().Level.ShouldBe(Level);
        item.NavigationDoorLink.ShouldNotBeNull().TeleportMarkerTriangle.ShouldBe(TeleportMarkerTriangle);
        item.Scale.ShouldBe(Scale);
    }

    [Fact]
    public void DeepParse_TwelveByteLock_ReadsTheLockAndWhatFollows()
    {
        ShouldReadTheLockAndWhatFollows(ParseDeep(BuildRefrBytesWithLock(ShortLockLength)));
    }

    [Fact]
    public void Overlay_TwelveByteLock_ReadsTheLockAndWhatFollows()
    {
        ShouldReadTheLockAndWhatFollows(ParseOverlay(BuildRefrBytesWithLock(ShortLockLength)));
    }

    [Theory]
    [InlineData(ShortLockLength)]
    [InlineData(FullLockLength)]
    public void DeepParse_WritesTheLockBackAtItsLength_AndTheOverlayRereadsIt(int lockLength)
    {
        var written = Write(ParseDeep(BuildRefrBytesWithLock(lockLength)));
        LockLength(written).ShouldBe(lockLength);
        ShouldReadTheLockAndWhatFollows(ParseOverlay(written));
    }

    [Theory]
    [InlineData(ShortLockLength)]
    [InlineData(FullLockLength)]
    public void Overlay_WritesTheLockBackAtItsLength_AndTheDeepParseRereadsIt(int lockLength)
    {
        var written = Write(ParseOverlay(BuildRefrBytesWithLock(lockLength)));
        LockLength(written).ShouldBe(lockLength);
        ShouldReadTheLockAndWhatFollows(ParseDeep(written));
    }
}
