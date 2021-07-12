using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using FluentAssertions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubListingsProviderIntegrationTests : TypicalTest
    {
        private const string DataDir = "C:/DataDirectory";

        [Fact]
        public void CccNotUsed()
        {
            var dataDirectoryInjection = new DataDirectoryInjection(DataDir);
            new CreationClubListingsProvider(
                    Fixture.Create<IFileSystem>(),
                    dataDirectoryInjection,
                    new CreationClubListingsPathInjection(default(FilePath?)),
                    new CreationClubRawListingsReader(
                        Fixture.Create<IFileSystem>(),
                        dataDirectoryInjection))
                .Get().Should().BeEmpty();
        }

        [Fact]
        public void CccMissing()
        {
            var fs = new MockFileSystem();
            fs.Directory.CreateDirectory(DataDir);
            Assert.Throws<FileNotFoundException>(() =>
            {
                var dataDirectoryInjection = new DataDirectoryInjection(DataDir);
                new CreationClubListingsProvider(
                        fs,
                        dataDirectoryInjection,
                        new CreationClubListingsPathInjection("C:/SomeMissingFile"),
                        new CreationClubRawListingsReader(
                            fs,
                            dataDirectoryInjection))
                    .Get();
            });
        }

        [Fact]
        public void GetListings()
        {
            var cccPath = Path.Combine(DataDir, "cccPlugin.txt");
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { cccPath, @"ModA.esp
ModB.esp"},
                { Path.Combine(DataDir, "ModA.esp"), string.Empty },
                { Path.Combine(DataDir, "ModB.esp"), string.Empty },
            });

            var dataDirectoryInjection = new DataDirectoryInjection(DataDir);
            new CreationClubListingsProvider(
                    fs,
                    dataDirectoryInjection,
                    new CreationClubListingsPathInjection(cccPath),
                    new CreationClubRawListingsReader(fs, dataDirectoryInjection))
                .Get().Should().Equal(
                    new ModListing("ModA.esp", true),
                    new ModListing("ModB.esp", true));
        }

        [Fact]
        public void SkipMissingListings()
        {
            var cccPath = Path.Combine(DataDir, "cccPlugin.txt");
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { cccPath, @"ModA.esp
ModB.esp"},
                { Path.Combine(DataDir, "ModA.esp"), string.Empty }
            });

            var dataDirectoryInjection = new DataDirectoryInjection(DataDir);
            new CreationClubListingsProvider(
                    fs,
                    dataDirectoryInjection,
                    new CreationClubListingsPathInjection(cccPath),
                    new CreationClubRawListingsReader(fs, dataDirectoryInjection))
                .Get().Should().Equal(
                    new ModListing("ModA.esp", true));   
        }
    }
}