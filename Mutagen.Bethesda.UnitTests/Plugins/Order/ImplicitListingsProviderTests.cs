using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture.Xunit2;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class ImplicitListingsProviderTests
    {
        [Theory, MutagenAutoData]
        public void Typical(
            [Frozen]MockFileSystem fs,
            ImplicitListingsProvider sut)
        {
            sut.ListingModKeys.Listings.Returns(new List<ModKey>()
            {
                Utility.MasterModKey,
                Utility.MasterModKey2
            });
            fs.File.WriteAllText(Path.Combine(sut.DataFolder.Path, Utility.MasterModKey.FileName), string.Empty);

            sut.Get()
                .Should().Equal(new ModListing(Utility.MasterModKey, true));
        }
    }
}