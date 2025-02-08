using Shouldly;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class ListsModsMixInTests
{
    [Fact]
    public void ListsMod_Empty()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMod(TestConstants.LightModKey)
            .ShouldBeFalse();
    }

    [Fact]
    public void ListsMod_Typical()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMod(TestConstants.LightModKey)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void ListsMod_Enabled()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMod(TestConstants.LightModKey, enabled: true)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey, enabled: false)
            .ShouldBeFalse();
        listings
            .ListsMod(TestConstants.LightModKey2, enabled: false)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey2, enabled: true)
            .ShouldBeFalse();
        listings
            .ListsMod(TestConstants.LightModKey3, enabled: true)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey3, enabled: false)
            .ShouldBeFalse();
    }

    [Fact]
    public void EmptyListings()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeFalse();
    }

    [Fact]
    public void EmptyInput()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods()
            .ShouldBeTrue();
    }

    [Fact]
    public void Single()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMods(TestConstants.LightModKey)
            .ShouldBeTrue();
        listings
            .ListsMods(TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ListsMods(TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ListsMods(TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void Typical()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMods(TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ListsMods(TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ListsMods(TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void Enabled_EmptyListings()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(true, TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeFalse();
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(false, TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeFalse();
    }

    [Fact]
    public void Enabled_EmptyInput()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(true)
            .ShouldBeTrue();
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(false)
            .ShouldBeTrue();
    }

    [Fact]
    public void Enabled_Single()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMods(true, TestConstants.LightModKey)
            .ShouldBeTrue();
        listings
            .ListsMods(false, TestConstants.LightModKey)
            .ShouldBeFalse();
        listings
            .ListsMods(false, TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ListsMods(true, TestConstants.LightModKey2)
            .ShouldBeFalse();
        listings
            .ListsMods(true, TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ListsMods(false, TestConstants.LightModKey3)
            .ShouldBeFalse();
        listings
            .ListsMods(true, TestConstants.LightModKey4)
            .ShouldBeFalse();
        listings
            .ListsMods(false, TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void Enabled_Typical()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMods(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ListsMods(false, TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ListsMods(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey3)
            .ShouldBeFalse();
        listings
            .ListsMods(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey4)
            .ShouldBeFalse();
    }
}