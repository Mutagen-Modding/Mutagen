using System;
using System.IO.Abstractions.TestingHelpers;
using Mutagen.Bethesda.Plugins.Order;
using Xunit;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing;
using Noggog;
using Noggog.Testing.IO;
using NSubstitute;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class LoadOrderWriterTests
{
    private static readonly string BaseFolder = $"{PathingUtil.DrivePrefix}BaseFolder";
        
    [Fact]
    public void WriteExclude()
    {
        var fs = new MockFileSystem();
        fs.Directory.CreateDirectory(BaseFolder);
        var path = Path.Combine(BaseFolder, "Plugins.txt");
        var markers = Substitute.For<IHasEnabledMarkersProvider>();
        markers.HasEnabledMarkers.Returns(false);
        var implicitMods = Substitute.For<IImplicitListingModKeyProvider>();
        implicitMods.Listings.Returns(Array.Empty<ModKey>());
        new LoadOrderWriter(
                fs,
                markers,
                implicitMods)
            .Write(
                path,
                new LoadOrderListing[]
                {
                    new LoadOrderListing(TestConstants.PluginModKey, false),
                    new LoadOrderListing(TestConstants.PluginModKey2, true),
                    new LoadOrderListing(TestConstants.PluginModKey3, false),
                });
        var lines = fs.File.ReadAllLines(path).ToList();
        Assert.Single(lines);
        Assert.Equal(TestConstants.PluginModKey2.FileName, lines[0]);
    }

    [Fact]
    public void WriteMarkers()
    {
        var fs = new MockFileSystem();
        fs.Directory.CreateDirectory(BaseFolder);
        var path = Path.Combine(BaseFolder, "Plugins.txt");
        var markers = Substitute.For<IHasEnabledMarkersProvider>();
        markers.HasEnabledMarkers.Returns(true);
        var implicitMods = Substitute.For<IImplicitListingModKeyProvider>();
        implicitMods.Listings.Returns(Array.Empty<ModKey>());
        new LoadOrderWriter(
                fs,
                markers,
                implicitMods)
            .Write(
                path,
                new LoadOrderListing[]
                {
                    new LoadOrderListing(TestConstants.PluginModKey, false),
                    new LoadOrderListing(TestConstants.PluginModKey2, true),
                    new LoadOrderListing(TestConstants.PluginModKey3, false),
                });
        var lines = fs.File.ReadAllLines(path).ToList();
        Assert.Equal(3, lines.Count);
        Assert.Equal($"{TestConstants.PluginModKey.FileName}", lines[0]);
        Assert.Equal($"*{TestConstants.PluginModKey2.FileName}", lines[1]);
        Assert.Equal($"{TestConstants.PluginModKey3.FileName}", lines[2]);
    }

    [Fact]
    public void WriteImplicitFilteredOut()
    {
        var fs = new MockFileSystem();
        fs.Directory.CreateDirectory(BaseFolder);
        var path = Path.Combine(BaseFolder, "Plugins.txt");
        var markers = Substitute.For<IHasEnabledMarkersProvider>();
        markers.HasEnabledMarkers.Returns(true);
        var implicitMods = Substitute.For<IImplicitListingModKeyProvider>();
        implicitMods.Listings.Returns(TestConstants.Skyrim.AsEnumerable().ToList());
        new LoadOrderWriter(
                fs,
                markers,
                implicitMods)
            .Write(
                path,
                new LoadOrderListing[]
                {
                    new LoadOrderListing(TestConstants.Skyrim, true),
                    new LoadOrderListing(TestConstants.PluginModKey, true),
                    new LoadOrderListing(TestConstants.PluginModKey2, false),
                },
                removeImplicitMods: true);
        var lines = fs.File.ReadAllLines(path).ToList();
        Assert.Equal(2, lines.Count);
        Assert.Equal($"*{TestConstants.PluginModKey.FileName}", lines[0]);
        Assert.Equal($"{TestConstants.PluginModKey2.FileName}", lines[1]);
    }

    [Fact]
    public void WriteImplicit()
    {
        var fs = new MockFileSystem();
        fs.Directory.CreateDirectory(BaseFolder);
        var path = Path.Combine(BaseFolder, "Plugins.txt");
        var markers = Substitute.For<IHasEnabledMarkersProvider>();
        markers.HasEnabledMarkers.Returns(true);
        var implicitMods = Substitute.For<IImplicitListingModKeyProvider>();
        implicitMods.Listings.Returns(TestConstants.Skyrim.AsEnumerable().ToList());
        new LoadOrderWriter(
                fs,
                markers,
                implicitMods)
            .Write(
                path,
                new LoadOrderListing[]
                {
                    new LoadOrderListing(TestConstants.Skyrim, true),
                    new LoadOrderListing(TestConstants.PluginModKey, true),
                    new LoadOrderListing(TestConstants.PluginModKey2, false),
                },
                removeImplicitMods: false);
        var lines = fs.File.ReadAllLines(path).ToList();
        Assert.Equal(3, lines.Count);
        Assert.Equal($"*{TestConstants.Skyrim.FileName}", lines[0]);
        Assert.Equal($"*{TestConstants.PluginModKey.FileName}", lines[1]);
        Assert.Equal($"{TestConstants.PluginModKey2.FileName}", lines[2]);
    }
}