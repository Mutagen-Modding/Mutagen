using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

public class NavmeshVertexFormVersionTests
{
    private const ushort VertexUnknownFormVersion = 493;
    private const ushort FormVersionBeforeVertexUnknown = VertexUnknownFormVersion - 1;

    private static byte[] Navmesh(ModKey modKey, ushort formVersion, IReadOnlyCollection<P3Float> points, int? unknown)
    {
        var masters = new MasterReferenceCollection(modKey);
        var formVersionLocation = GameConstants.Starfield.MajorConstants.FormVersionLocationOffset!.Value;
        var defaults = new NavmeshGeometry();
        var stream = new MemoryStream();
        using (var writer = new MutagenWriter(
                   stream,
                   new WritingBundle(GameConstants.Starfield)
                   {
                       MasterReferences = masters,
                       SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(masters),
                   }))
        {
            using (HeaderExport.Record(writer, RecordTypes.NAVM))
            {
                writer.WriteZeros((uint)(formVersionLocation - GameConstants.Starfield.MajorConstants.TypeAndLengthLength));
                writer.Write(formVersion);
                writer.WriteZeros((uint)(GameConstants.Starfield.MajorConstants.HeaderLength - formVersionLocation - sizeof(ushort)));
                using (HeaderExport.Subrecord(writer, RecordTypes.NVNM))
                {
                    writer.Write(defaults.NavmeshVersion);
                    writer.Write(defaults.CrcHash);
                    defaults.Parent.WriteToBinary(writer);
                    writer.Write(points.Count);
                    foreach (var point in points)
                    {
                        P3FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, point);
                        writer.Write(unknown);
                    }
                    writer.Write(defaults.Triangles.Count);
                    writer.Write(defaults.EdgeLinks.Count);
                    writer.Write(defaults.DoorTriangles.Count);
                    writer.Write(defaults.Cover.Count);
                    writer.Write(defaults.CoverTriangleMappings.Count);
                    writer.Write(defaults.Waypoints.Count);
                    writer.Write(defaults.GridSize);
                    P2FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, defaults.GridMaxDistance);
                    P3FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, defaults.GridMin);
                    P3FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, defaults.GridMax);
                }
            }
        }
        return stream.ToArray();
    }

    private static ParsingMeta Meta(ModKey modKey) => new(
        GameConstants.Starfield,
        modKey,
        SeparatedMasterPackage.NotSeparate(new MasterReferenceCollection(modKey)));

    private static void ShouldHoldTheVertices(INavigationMeshGetter navmesh, IEnumerable<P3Float> points, int unknown)
    {
        var vertices = navmesh.NavmeshGeometry.ShouldNotBeNull().Vertices;
        vertices.Select(x => x.Point).ShouldBe(points);
        vertices.ShouldAllBe(x => x.Unknown == unknown);
    }

    private static INavigationMeshGetter DeepParse(byte[] navmesh, ModKey modKey) =>
        NavigationMesh.CreateFromBinary(new MutagenFrame(new MutagenMemoryReadStream(navmesh, Meta(modKey))));

    private static INavigationMeshGetter Overlay(byte[] navmesh, ModKey modKey) =>
        NavigationMeshBinaryOverlay.NavigationMeshFactory(navmesh, new BinaryOverlayFactoryPackage(Meta(modKey)));

    private static INavigationMeshGetter WriteAndReread(
        IFileSystem fileSystem,
        ModPath modPath,
        StarfieldMod mod,
        ushort formVersion,
        IEnumerable<P3Float> points,
        int unknown)
    {
        var geometry = new NavmeshGeometry();
        geometry.Vertices.SetTo(points.Select(point => new NavmeshVertex
        {
            Point = point,
            Unknown = unknown,
        }));
        var cell = new Cell(mod);
        cell.NavigationMeshes.Add(new NavigationMesh(mod)
        {
            FormVersion = formVersion,
            NavmeshGeometry = geometry,
        });
        mod.Cells.Add(new CellBlock
        {
            SubBlocks =
            {
                new CellSubBlock
                {
                    Cells = { cell },
                },
            },
        });

        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .Write();
        var reread = StarfieldMod.Create(mod.StarfieldRelease)
            .FromPath(modPath)
            .WithNoLoadOrder()
            .Mutable()
            .WithFileSystem(fileSystem)
            .Construct();
        return reread.EnumerateMajorRecords<INavigationMesh>().ShouldHaveSingleItem();
    }

    [Theory, MutagenAutoData(GameRelease.Starfield)]
    public void DeepParse_BeforeTheVertexUnknownFormVersion_ReadsVerticesWithoutIt(
        ModKey modKey,
        P3Float[] points)
    {
        var navmesh = DeepParse(Navmesh(modKey, FormVersionBeforeVertexUnknown, points, unknown: null), modKey);

        ShouldHoldTheVertices(navmesh, points, unknown: 0);
    }

    [Theory, MutagenAutoData(GameRelease.Starfield)]
    public void DeepParse_AtTheVertexUnknownFormVersion_ReadsVerticesWithIt(
        ModKey modKey,
        P3Float[] points,
        int unknown)
    {
        var navmesh = DeepParse(Navmesh(modKey, VertexUnknownFormVersion, points, unknown), modKey);

        ShouldHoldTheVertices(navmesh, points, unknown);
    }

    [Theory, MutagenAutoData(GameRelease.Starfield)]
    public void Overlay_BeforeTheVertexUnknownFormVersion_ReadsVerticesWithoutIt(
        ModKey modKey,
        P3Float[] points)
    {
        var navmesh = Overlay(Navmesh(modKey, FormVersionBeforeVertexUnknown, points, unknown: null), modKey);

        ShouldHoldTheVertices(navmesh, points, unknown: 0);
    }

    [Theory, MutagenAutoData(GameRelease.Starfield)]
    public void Overlay_AtTheVertexUnknownFormVersion_ReadsVerticesWithIt(
        ModKey modKey,
        P3Float[] points,
        int unknown)
    {
        var navmesh = Overlay(Navmesh(modKey, VertexUnknownFormVersion, points, unknown), modKey);

        ShouldHoldTheVertices(navmesh, points, unknown);
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void Write_BeforeTheVertexUnknownFormVersion_LeavesItOut(
        IFileSystem fileSystem,
        ModPath modPath,
        StarfieldMod mod,
        P3Float[] points,
        int unknown)
    {
        var navmesh = WriteAndReread(fileSystem, modPath, mod, FormVersionBeforeVertexUnknown, points, unknown);

        ShouldHoldTheVertices(navmesh, points, unknown: 0);
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void Write_AtTheVertexUnknownFormVersion_KeepsIt(
        IFileSystem fileSystem,
        ModPath modPath,
        StarfieldMod mod,
        P3Float[] points,
        int unknown)
    {
        var navmesh = WriteAndReread(fileSystem, modPath, mod, VertexUnknownFormVersion, points, unknown);

        ShouldHoldTheVertices(navmesh, points, unknown);
    }
}
