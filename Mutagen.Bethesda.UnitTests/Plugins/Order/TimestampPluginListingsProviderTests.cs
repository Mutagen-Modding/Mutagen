using AutoFixture;
using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class TimestampPluginListingsProviderTests : TypicalTest
    {
        [Fact]
        public void Typical()
        {
            var pluginRawListingsReader = Fixture.Create<IPluginRawListingsReader>();
            var aligner = Fixture.Create<ITimestampAligner>();
            var pluginPathContext = Fixture.Create<IPluginListingsPathProvider>();
            var dataDirectoryContext = Fixture.Create<IDataDirectoryContext>();
            var timestampedPluginListingsPreferences = Fixture.Create<ITimestampedPluginListingsPreferences>();
            new TimestampedPluginListingsProvider(
                    aligner,
                timestampedPluginListingsPreferences,
                pluginRawListingsReader,
                dataDirectoryContext,
                pluginPathContext)
                .Get()
                .Should().Equal(
                    aligner.AlignToTimestamps(
                        pluginRawListingsReader.Read(pluginPathContext.Path),
                        dataDirectoryContext.Path,
                        timestampedPluginListingsPreferences.ThrowOnMissingMods));
        }
    }
}