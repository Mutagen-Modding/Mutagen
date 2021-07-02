using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubListingsProviderTests : IClassFixture<Fixture>
    {
        private readonly Fixture _Fixture;
        private const string DataDir = "C:/DataDirectory";

        public CreationClubListingsProviderTests(Fixture fixture)
        {
            _Fixture = fixture;
        }
        
        [Fact]
        public void CccNotUsed()
        {
            var dataDirectoryInjection = new DataDirectoryInjection(DataDir);
            new CreationClubListingsProvider(
                    _Fixture.Inject.Create<IFileSystem>(),
                    dataDirectoryInjection,
                    new CreationClubPathInjection(default(FilePath?)),
                    new CreationClubRawListingsReader(
                        _Fixture.Inject.Create<IFileSystem>(),
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
                        new CreationClubPathInjection("C:/SomeMissingFile"),
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
                    new CreationClubPathInjection(cccPath),
                    new CreationClubRawListingsReader(fs, dataDirectoryInjection))
                .Get().Should().BeEquivalentTo(
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
                    new CreationClubPathInjection(cccPath),
                    new CreationClubRawListingsReader(fs, dataDirectoryInjection))
                .Get().Should().BeEquivalentTo(
                    new ModListing("ModA.esp", true));   
        }
    }
}