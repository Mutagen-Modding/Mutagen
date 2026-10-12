using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Internals;
using Noggog;
using Xunit;
using StarfieldConstants = Mutagen.Bethesda.Starfield.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

public class NavmeshVertexFormVersionTests
{
    private const ushort ThreeFloatVertexFormVersion = 476;
    private const ushort VertexWithUnknownFormVersion = 493;
    private const int VertexUnknown = 7;

    private static readonly MasterReferenceCollection Masters = new(StarfieldConstants.Starfield);
    private static readonly P3Float[] Points = [new(1, 2, 3), new(4, 5, 6), new(7, 8, 9)];

    public static TheoryData<ushort, bool> FormVersionsAndTheirVertexLayout => new()
    {
        { ThreeFloatVertexFormVersion, false },
        { VertexWithUnknownFormVersion, true },
    };

    private static byte[] Navmesh(ushort formVersion, bool vertexHasUnknown)
    {
        var content = Subrecord("NVNM", Geometry(vertexHasUnknown));
        var header = new byte[GameConstants.Starfield.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("NAVM").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)content.Length));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), formVersion);
        return header.Concat(content).ToArray();
    }

    private static byte[] Geometry(bool vertexHasUnknown)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        writer.Write(17u);
        writer.Write(0xA5E9A03Cu);
        writer.Write(0L);
        writer.Write(Points.Length);
        foreach (var point in Points)
        {
            writer.Write(point.X);
            writer.Write(point.Y);
            writer.Write(point.Z);
            if (vertexHasUnknown) writer.Write(VertexUnknown);
        }
        writer.Write(1);
        WriteTriangleOverTheThreeVertices(writer);
        WriteNoEdgeLinksDoorsCoverMappingsOrWaypoints(writer);
        WriteAnEmptyGrid(writer);
        return stream.ToArray();
    }

    private static void WriteTriangleOverTheThreeVertices(BinaryWriter writer)
    {
        foreach (short vertex in new short[] { 0, 1, 2 }) writer.Write(vertex);
        foreach (short edgeLink in new short[] { -1, -1, -1 }) writer.Write(edgeLink);
        writer.Write(0f);
        writer.Write((byte)0);
        writer.Write((ushort)0);
        writer.Write((ushort)0);
    }

    private static void WriteNoEdgeLinksDoorsCoverMappingsOrWaypoints(BinaryWriter writer)
    {
        foreach (var _ in Enumerable.Range(0, 5)) writer.Write(0);
    }

    private static void WriteAnEmptyGrid(BinaryWriter writer)
    {
        writer.Write(0u);
        writer.Write(new byte[sizeof(float) * (2 + 3 + 3)]);
    }

    private static byte[] Subrecord(string type, byte[] content)
    {
        var header = new byte[6];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(4), checked((ushort)content.Length));
        return header.Concat(content).ToArray();
    }

    private static ParsingMeta Meta() => new(
        GameConstants.Starfield,
        StarfieldConstants.Starfield,
        SeparatedMasterPackage.NotSeparate(Masters));

    private static INavigationMeshGetter ParseDeep(byte[] bytes)
    {
        using var reader = new MutagenBinaryReadStream(new MemoryStream(bytes), Meta());
        return NavigationMesh.CreateFromBinary(new MutagenFrame(reader));
    }

    private static INavigationMeshGetter ParseOverlay(byte[] bytes)
    {
        return NavigationMeshBinaryOverlay.NavigationMeshFactory(bytes, new BinaryOverlayFactoryPackage(Meta()));
    }

    private static byte[] Write(INavigationMeshGetter item)
    {
        var stream = new MemoryStream();
        using (var writer = new MutagenWriter(
                   stream,
                   new WritingBundle(GameConstants.Starfield)
                   {
                       MasterReferences = Masters,
                       SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(Masters),
                   }))
        {
            NavigationMeshBinaryWriteTranslation.Instance.Write(writer, item, default);
        }
        return stream.ToArray();
    }

    private static byte[] GeometryWritten(byte[] record) =>
        RecordSpanExtensions.FindAllOfSubrecord(
                new ReadOnlyMemorySlice<byte>(record).Slice(GameConstants.Starfield.MajorConstants.HeaderLength),
                GameConstants.Starfield,
                RecordTypes.NVNM)
            .Single()
            .Content
            .ToArray();

    private static void ShouldHoldTheGeometry(INavigationMeshGetter navmesh, bool vertexHasUnknown)
    {
        var geometry = navmesh.NavmeshGeometry.ShouldNotBeNull();
        geometry.Vertices.Select(x => x.Point).ShouldBe(Points);
        geometry.Vertices.Select(x => x.Unknown).ShouldAllBe(x => x == (vertexHasUnknown ? VertexUnknown : 0));
        geometry.Triangles.Single().Vertices.ShouldBe(new P3Int16(0, 1, 2));
    }

    [Theory]
    [MemberData(nameof(FormVersionsAndTheirVertexLayout))]
    public void DeepParse_ReadsTheVertexLayoutOfItsFormVersion(ushort formVersion, bool vertexHasUnknown)
    {
        ShouldHoldTheGeometry(ParseDeep(Navmesh(formVersion, vertexHasUnknown)), vertexHasUnknown);
    }

    [Theory]
    [MemberData(nameof(FormVersionsAndTheirVertexLayout))]
    public void Overlay_ReadsTheVertexLayoutOfItsFormVersion(ushort formVersion, bool vertexHasUnknown)
    {
        ShouldHoldTheGeometry(ParseOverlay(Navmesh(formVersion, vertexHasUnknown)), vertexHasUnknown);
    }

    [Theory]
    [MemberData(nameof(FormVersionsAndTheirVertexLayout))]
    public void DeepParse_WritesTheGeometryItRead(ushort formVersion, bool vertexHasUnknown)
    {
        GeometryWritten(Write(ParseDeep(Navmesh(formVersion, vertexHasUnknown)))).ShouldBe(Geometry(vertexHasUnknown));
    }

    [Theory]
    [MemberData(nameof(FormVersionsAndTheirVertexLayout))]
    public void Overlay_WritesTheGeometryItRead(ushort formVersion, bool vertexHasUnknown)
    {
        GeometryWritten(Write(ParseOverlay(Navmesh(formVersion, vertexHasUnknown)))).ShouldBe(Geometry(vertexHasUnknown));
    }
}
