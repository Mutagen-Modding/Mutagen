using System.IO.Abstractions.TestingHelpers;
using AutoFixture.Xunit2;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class ImplicitListingsProviderTests
{
    [Theory, MutagenAutoData]
    public void Typical(
        [Frozen]MockFileSystem fs,
        ImplicitListingsProvider sut)
    {
        sut.ListingModKeys.Listings.Returns(new List<ModKey>()
        {
            TestConstants.MasterModKey,
            TestConstants.MasterModKey2
        });
        fs.File.WriteAllText(Path.Combine(sut.DataFolder.Path, TestConstants.MasterModKey.FileName), string.Empty);

        sut.Get()
            .Should().Equal(new ModListing(TestConstants.MasterModKey, true, true));
    }
}