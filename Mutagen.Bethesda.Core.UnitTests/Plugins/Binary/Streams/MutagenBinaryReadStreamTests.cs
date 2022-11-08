using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;
using Noggog.Testing.IO;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Streams;

public class MutagenBinaryReadStreamTests
{
    [Theory, MutagenAutoData]
    public void StreamModKeyCtorDoesNotDisposeStream()
    {
        var stream = new DisposeTesterWrapStream(File.OpenRead(TestDataPathing.SkyrimOverrideMod));
        var mutaStream = new MutagenBinaryReadStream(
            stream,
            ModKey.FromNameAndExtension("Skyrim.esm"),
            GameRelease.SkyrimSE);
        stream.Disposed.Should().BeFalse();
    }
}