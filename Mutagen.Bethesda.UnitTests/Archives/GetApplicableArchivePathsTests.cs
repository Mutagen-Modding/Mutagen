using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Inis;
using Mutagen.Bethesda.Plugins;
using Noggog;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Archives
{
    public class GetApplicableArchivePathsTests
    {
        private const string SomeExplicitListingBsa = "SomeExplicitListing.bsa";
        private const string UnusedExplicitListingBsa = "SomeExplicitListing2.bsa";
        private const string SkyrimBsa = "Skyrim.bsa";
        private const string MyModBsa = "MyMod.bsa";
        private const string BaseFolder = "C:/BaseFolder";

        private IFileSystem GetFileSystem()
        {
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { Ini.GetTypicalPath(GameRelease.SkyrimSE).Path, new MockFileData($@"[Archive]
sResourceArchiveList={SomeExplicitListingBsa}, {UnusedExplicitListingBsa}") }
            });
            fs.Directory.CreateDirectory(BaseFolder);
            return fs;
        }

        private GetApplicableArchivePaths GetClass(IFileSystem fs)
        {
            return new GetApplicableArchivePaths(
                fs, 
                new GetArchiveIniListings(fs),
                new CheckArchiveApplicability());
        }

        #region No ModKey
        [Fact]
        public void NoModKey_Unordered()
        {
            var fs = GetFileSystem();
            fs.File.WriteAllText(Path.Combine(BaseFolder, MyModBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SkyrimBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SomeExplicitListingBsa), string.Empty);
            var get = GetClass(fs);
            var applicable = get.Get(GameRelease.SkyrimSE, BaseFolder, Enumerable.Empty<FileName>())
                .ToArray();
            applicable.Should().BeEquivalentTo(new FilePath[]
            {
                Path.Combine(BaseFolder, MyModBsa),
                Path.Combine(BaseFolder, SkyrimBsa),
                Path.Combine(BaseFolder, SomeExplicitListingBsa),
            });
        }

        [Fact]
        public void NoModKey_Ordered()
        {
            var fs = GetFileSystem();
            fs.File.WriteAllText(Path.Combine(BaseFolder, MyModBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SkyrimBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SomeExplicitListingBsa), string.Empty);
            var get = GetClass(fs);
            var applicable = get.Get(GameRelease.SkyrimSE, BaseFolder)
                .ToArray();
            applicable.Should().BeEquivalentTo(new FilePath[]
            {
                Path.Combine(BaseFolder, SkyrimBsa),
                Path.Combine(BaseFolder, SomeExplicitListingBsa),
                Path.Combine(BaseFolder, MyModBsa),
            });
        }

        #endregion

        #region GetApplicableArchivePaths
        [Fact]
        public void Empty()
        {
            var fs = GetFileSystem();
            var get = GetClass(fs);
            get.Get(GameRelease.SkyrimSE, BaseFolder, Utility.Skyrim, Enumerable.Empty<FileName>())
                .Should().BeEmpty();
        }

        [Fact]
        public void NullModKey()
        {
            var fs = GetFileSystem();
            var get = GetClass(fs);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SkyrimBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SomeExplicitListingBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, MyModBsa), string.Empty);
            var applicable = get.Get(GameRelease.SkyrimSE, BaseFolder, ModKey.Null)
                .Should().BeEmpty();
        }

        [Fact]
        public void BaseMod_Unordered()
        {
            var fs = GetFileSystem();
            var get = GetClass(fs);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SkyrimBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SomeExplicitListingBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, MyModBsa), string.Empty);
            var applicable = get.Get(GameRelease.SkyrimSE, BaseFolder, Utility.Skyrim, Enumerable.Empty<FileName>())
                .ToArray();
            applicable.Should().BeEquivalentTo(new FilePath[]
            {
                Path.Combine(BaseFolder, SkyrimBsa),
                Path.Combine(BaseFolder, SomeExplicitListingBsa)
            });
        }

        [Fact]
        public void Typical_Unordered()
        {
            var fs = GetFileSystem();
            var get = GetClass(fs);
            fs.File.WriteAllText(Path.Combine(BaseFolder, $"{Utility.MasterModKey2.Name}.bsa"), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SomeExplicitListingBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, MyModBsa), string.Empty);
            var applicable = get.Get(GameRelease.SkyrimSE, BaseFolder, Utility.MasterModKey2, Enumerable.Empty<FileName>())
                .ToArray();
            applicable.Should().BeEquivalentTo(new FilePath[]
            {
                Path.Combine(BaseFolder, $"{Utility.MasterModKey2.Name}.bsa"),
                Path.Combine(BaseFolder, SomeExplicitListingBsa)
            });
        }

        [Fact]
        public void BaseMod_Ordered()
        {
            var fs = GetFileSystem();
            var get = GetClass(fs);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SkyrimBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SomeExplicitListingBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, MyModBsa), string.Empty);
            var applicable = get.Get(GameRelease.SkyrimSE, BaseFolder, Utility.Skyrim)
                .ToArray();
            applicable.Should().BeEquivalentTo(new FilePath[]
            {
                Path.Combine(BaseFolder, SomeExplicitListingBsa),
                Path.Combine(BaseFolder, SkyrimBsa),
            });
        }

        [Fact]
        public void Typical_Ordered()
        {
            var fs = GetFileSystem();
            var get = GetClass(fs);
            fs.File.WriteAllText(Path.Combine(BaseFolder, $"{Utility.MasterModKey2.Name}.bsa"), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, SomeExplicitListingBsa), string.Empty);
            fs.File.WriteAllText(Path.Combine(BaseFolder, MyModBsa), string.Empty);
            var applicable = get.Get(GameRelease.SkyrimSE, BaseFolder, Utility.MasterModKey2)
                .ToArray();
            applicable.Should().BeEquivalentTo(new FilePath[]
            {
                Path.Combine(BaseFolder, SomeExplicitListingBsa),
                Path.Combine(BaseFolder, $"{Utility.MasterModKey2.Name}.bsa"),
            });
        }
        #endregion
    }
}