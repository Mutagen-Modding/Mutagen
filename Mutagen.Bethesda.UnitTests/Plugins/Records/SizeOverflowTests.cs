using System.IO;
using FluentAssertions;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public abstract class SizeOverflowTests
    {
        protected abstract IWorldspaceGetter Get(ModPath path);
        
        [Fact]
        public void WorldspaceOffsetOverflowTest()
        {
            var worldspace = Get(TestPathing.SizeOverflow);
            worldspace.OffsetData.HasValue.Should().BeTrue();
            worldspace.OffsetData!.Value.Length.Should().Be(0x3B);
        }
        
        [Fact]
        public void WorldspaceMaxHeightOverflowTest()
        {
            var worldspace = Get(TestPathing.SubObjectSizeOverflow);
            worldspace.MaxHeight.Should().NotBeNull();
            worldspace.MaxHeight!.Min.Should().Be(new P2Int16(-96, -96));
            worldspace.MaxHeight!.Max.Should().Be(new P2Int16(96, 97));
            worldspace.MaxHeight!.CellData.Length.Should().Be(0x33);
        }
    }
    
    public class SizeOverflowTestsDirect : SizeOverflowTests
    {
        protected override IWorldspaceGetter Get(ModPath path)
        {
            using var reader = new MutagenBinaryReadStream(path, GameRelease.SkyrimSE);
            return Worldspace.CreateFromBinary(new MutagenFrame(reader));
        }
    }
    
    public class SizeOverflowTestsOverlay : SizeOverflowTests
    {
        protected override IWorldspaceGetter Get(ModPath path)
        {
            var bytes = File.ReadAllBytes(path);
            return WorldspaceBinaryOverlay.WorldspaceFactory(
                bytes,
                new BinaryOverlayFactoryPackage(
                    new ParsingBundle(
                        GameConstants.SkyrimSE,
                        null!)));
        }
    }
}