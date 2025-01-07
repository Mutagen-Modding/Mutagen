using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
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
            new ParsingMeta(
                GameConstants.Get(GameRelease.SkyrimSE),
                ModKey.FromNameAndExtension("Skyrim.esm"),
                SeparatedMasterPackage.EmptyNull));
        stream.Disposed.Should().BeFalse();
    }
}