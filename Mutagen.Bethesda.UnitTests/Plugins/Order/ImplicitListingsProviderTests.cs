using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Implicit;
using Mutagen.Bethesda.Plugins.Implicit.DI;
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
            [Frozen]IDataDirectoryProvider dataDir,
            [Frozen]IImplicitListingModKeyProvider listings,
            ImplicitListingsProvider sut)
        {
            A.CallTo(() => listings.Listings).Returns(new List<ModKey>()
            {
                Utility.MasterModKey,
                Utility.MasterModKey2
            });
            fs.File.WriteAllText(Path.Combine(dataDir.Path, Utility.MasterModKey.FileName), string.Empty);

            sut.Get()
                .Should().Equal(new ModListing(Utility.MasterModKey, true));
        }
    }
}