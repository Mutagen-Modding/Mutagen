using System.IO;
using FluentAssertions;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public class SizeOverflowTests
    {
        [Fact]
        public void WorldspaceOffsetOverflowTest()
        {
            using var reader = new MutagenBinaryReadStream(TestPathing.SizeOverflow, GameRelease.SkyrimSE);
            var worldspace = Worldspace.CreateFromBinary(new MutagenFrame(reader));
            worldspace.OffsetData.HasValue.Should().BeTrue();
            worldspace.OffsetData!.Value.Length.Should().Be(0x3B);
        }
    }
}