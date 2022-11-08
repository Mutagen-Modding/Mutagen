using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class TimestampPluginListingsProviderTests
{
    [Theory, MutagenAutoData]
    public void Typical(TimestampedPluginListingsProvider sut)
    {
        sut.Get()
            .Should().Equal(
                sut.Aligner.AlignToTimestamps(
                    sut.RawListingsReader.Read(sut.ListingsPathContext.Path),
                    sut.DirectoryProvider.Path,
                    sut.Prefs.ThrowOnMissingMods));
    }
}