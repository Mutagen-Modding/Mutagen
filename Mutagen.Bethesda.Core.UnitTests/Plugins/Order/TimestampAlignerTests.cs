using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing;
using Noggog.Testing.IO;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class TimestampAlignerTests
{
    static readonly string DataFolder = $"{PathingUtil.DrivePrefix}DataFolder";
        
    private IEnumerable<(ModKey ModKey, DateTime Write)> GetTypical()
    {
        return new List<(ModKey ModKey, DateTime Write)>
        {
            (TestConstants.PluginModKey4, new DateTime(2020, 8, 8, 10, 11, 0)),
            (TestConstants.PluginModKey, new DateTime(2020, 8, 8, 10, 8, 0)),
            (TestConstants.PluginModKey3, new DateTime(2020, 8, 8, 10, 10, 0)),
            (TestConstants.PluginModKey2, new DateTime(2020, 8, 8, 10, 9, 0)),
        };
    }
        
    private IEnumerable<(ModKey ModKey, DateTime Write)> GetSameTimestamps()
    {
        return new List<(ModKey ModKey, DateTime Write)>
        {
            (TestConstants.PluginModKey4, new DateTime(2020, 8, 8, 10, 11, 0)),
            (TestConstants.PluginModKey, new DateTime(2020, 8, 8, 10, 8, 0)),
            (TestConstants.PluginModKey3, new DateTime(2020, 8, 8, 10, 10, 0)),
            (TestConstants.PluginModKey2,  new DateTime(2020, 8, 8, 10, 11, 0)),
        };
    }
        
    private IFileSystem GetFileSystem(IEnumerable<(ModKey ModKey, DateTime Write)> items)
    {
        var dict = new Dictionary<string, MockFileData>();
        foreach (var item in items)
        {
            dict.Add(Path.Combine(DataFolder, item.ModKey.FileName), new MockFileData(string.Empty)
            {
                LastWriteTime = item.Write
            });
        }

        return new MockFileSystem(dict);
    }

    private IEnumerable<ILoadOrderListingGetter> GetListings(IEnumerable<(ModKey ModKey, DateTime Write)> items)
    {
        return items.Select(i => new LoadOrderListing(i.ModKey, true));
    }
        
    [Fact]
    public void Typical()
    {
        var results = new TimestampAligner(null!)
            .AlignToTimestamps(GetTypical())
            .ToList();
        Assert.Equal(TestConstants.PluginModKey, results[0]);
        Assert.Equal(TestConstants.PluginModKey2, results[1]);
        Assert.Equal(TestConstants.PluginModKey3, results[2]);
        Assert.Equal(TestConstants.PluginModKey4, results[3]);
    }
        
    [Fact]
    public void Typical_Filesystem()
    {
        var items = GetTypical();
        var fs = GetFileSystem(items);
        var results = new TimestampAligner(fs)
            .AlignToTimestamps(GetListings(items), DataFolder)
            .ToList();
        Assert.Equal(new LoadOrderListing(TestConstants.PluginModKey, true), results[0]);
        Assert.Equal(new LoadOrderListing(TestConstants.PluginModKey2, true), results[1]);
        Assert.Equal(new LoadOrderListing(TestConstants.PluginModKey3, true), results[2]);
        Assert.Equal(new LoadOrderListing(TestConstants.PluginModKey4, true), results[3]);
    }

    [Fact]
    public void SameTimestamps()
    {
        var results = LoadOrder.AlignToTimestamps(GetSameTimestamps())
            .ToList();
        Assert.Equal(TestConstants.PluginModKey, results[0]);
        Assert.Equal(TestConstants.PluginModKey3, results[1]);
        Assert.Equal(TestConstants.PluginModKey4, results[2]);
        Assert.Equal(TestConstants.PluginModKey2, results[3]);
    }

    [Fact]
    public void SameTimestamps_Filesystem()
    {
        var items = GetSameTimestamps();
        var fs = GetFileSystem(items);
        var results = new TimestampAligner(fs)
            .AlignToTimestamps(GetListings(items), DataFolder)
            .ToList();
        Assert.Equal(new LoadOrderListing(TestConstants.PluginModKey, true), results[0]);
        Assert.Equal(new LoadOrderListing(TestConstants.PluginModKey3, true), results[1]);
        Assert.Equal(new LoadOrderListing(TestConstants.PluginModKey4, true), results[2]);
        Assert.Equal(new LoadOrderListing(TestConstants.PluginModKey2, true), results[3]);
    }
}