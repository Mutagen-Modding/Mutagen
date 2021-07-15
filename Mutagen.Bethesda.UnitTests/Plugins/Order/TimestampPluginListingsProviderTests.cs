using AutoFixture;
using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class TimestampPluginListingsProviderTests
    {
        [Theory, MutagenAutoData]
        public void Typical(TimestampedPluginListingsProvider sut)
        {
            sut.Get()
                .Should().Equal(
                    sut.Aligner.AlignToTimestamps(
                        sut.RawListingsReader.Read(sut.ListingsPathProvider.Path),
                        sut.DirectoryProvider.Path,
                        sut.Prefs.ThrowOnMissingMods));
        }
    }
}