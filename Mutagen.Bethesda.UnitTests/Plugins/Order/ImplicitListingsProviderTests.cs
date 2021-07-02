using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Implicit;
using Mutagen.Bethesda.Plugins.Order;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class ImplicitListingsProviderTests
    {
        [Fact]
        public void Typical()
        {
            var dataDir = new DataDirectoryInjection("C:/SomeDir");
            var listings = Substitute.For<IImplicitListingModKeyProvider>();
            listings.Listings.Returns(new List<ModKey>()
            {
                Utility.MasterModKey,
                Utility.MasterModKey2
            });
            var fs = Substitute.For<IFileSystem>();
            fs.File.Exists(Path.Combine(dataDir.Path, Utility.MasterModKey.FileName)).Returns(true);
            fs.File.Exists(Path.Combine(dataDir.Path, Utility.MasterModKey2.FileName)).Returns(false);

            new ImplicitListingsProvider(
                    fs,
                    dataDir,
                    listings)
                .Get()
                .Should().BeEquivalentTo(new ModListing(Utility.MasterModKey, true));
        }
    }
}