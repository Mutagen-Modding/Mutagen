using FluentAssertions;
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
            .Should().BeFalse();
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
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightModKey2)
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightModKey3)
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightModKey4)
            .Should().BeFalse();
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
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightModKey, enabled: false)
            .Should().BeFalse();
        listings
            .ListsMod(TestConstants.LightModKey2, enabled: false)
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightModKey2, enabled: true)
            .Should().BeFalse();
        listings
            .ListsMod(TestConstants.LightModKey3, enabled: true)
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightModKey3, enabled: false)
            .Should().BeFalse();
    }

    [Fact]
    public void EmptyListings()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(TestConstants.LightModKey, TestConstants.LightModKey2)
            .Should().BeFalse();
    }

    [Fact]
    public void EmptyInput()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods()
            .Should().BeTrue();
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
            .Should().BeTrue();
        listings
            .ListsMods(TestConstants.LightModKey2)
            .Should().BeTrue();
        listings
            .ListsMods(TestConstants.LightModKey3)
            .Should().BeTrue();
        listings
            .ListsMods(TestConstants.LightModKey4)
            .Should().BeFalse();
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
            .Should().BeTrue();
        listings
            .ListsMods(TestConstants.LightModKey, TestConstants.LightModKey2)
            .Should().BeTrue();
        listings
            .ListsMods(TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Enabled_EmptyListings()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(true, TestConstants.LightModKey, TestConstants.LightModKey2)
            .Should().BeFalse();
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(false, TestConstants.LightModKey, TestConstants.LightModKey2)
            .Should().BeFalse();
    }

    [Fact]
    public void Enabled_EmptyInput()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(true)
            .Should().BeTrue();
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(false)
            .Should().BeTrue();
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
            .Should().BeTrue();
        listings
            .ListsMods(false, TestConstants.LightModKey)
            .Should().BeFalse();
        listings
            .ListsMods(false, TestConstants.LightModKey2)
            .Should().BeTrue();
        listings
            .ListsMods(true, TestConstants.LightModKey2)
            .Should().BeFalse();
        listings
            .ListsMods(true, TestConstants.LightModKey3)
            .Should().BeTrue();
        listings
            .ListsMods(false, TestConstants.LightModKey3)
            .Should().BeFalse();
        listings
            .ListsMods(true, TestConstants.LightModKey4)
            .Should().BeFalse();
        listings
            .ListsMods(false, TestConstants.LightModKey4)
            .Should().BeFalse();
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
            .Should().BeTrue();
        listings
            .ListsMods(false, TestConstants.LightModKey2)
            .Should().BeTrue();
        listings
            .ListsMods(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey3)
            .Should().BeFalse();
        listings
            .ListsMods(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey4)
            .Should().BeFalse();
    }
}