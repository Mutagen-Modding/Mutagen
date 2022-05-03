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
            .ListsMod(TestConstants.LightMasterModKey)
            .Should().BeFalse();
    }

    [Fact]
    public void ListsMod_Typical()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightMasterModKey, true),
            new LoadOrderListing(TestConstants.LightMasterModKey2, false),
            new LoadOrderListing(TestConstants.LightMasterModKey3, true),
        };
        listings
            .ListsMod(TestConstants.LightMasterModKey)
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightMasterModKey2)
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightMasterModKey3)
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightMasterModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void ListsMod_Enabled()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightMasterModKey, true),
            new LoadOrderListing(TestConstants.LightMasterModKey2, false),
            new LoadOrderListing(TestConstants.LightMasterModKey3, true),
        };
        listings
            .ListsMod(TestConstants.LightMasterModKey, enabled: true)
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightMasterModKey, enabled: false)
            .Should().BeFalse();
        listings
            .ListsMod(TestConstants.LightMasterModKey2, enabled: false)
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightMasterModKey2, enabled: true)
            .Should().BeFalse();
        listings
            .ListsMod(TestConstants.LightMasterModKey3, enabled: true)
            .Should().BeTrue();
        listings
            .ListsMod(TestConstants.LightMasterModKey3, enabled: false)
            .Should().BeFalse();
    }

    [Fact]
    public void EmptyListings()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
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
            new LoadOrderListing(TestConstants.LightMasterModKey, true),
            new LoadOrderListing(TestConstants.LightMasterModKey2, false),
            new LoadOrderListing(TestConstants.LightMasterModKey3, true),
        };
        listings
            .ListsMods(TestConstants.LightMasterModKey)
            .Should().BeTrue();
        listings
            .ListsMods(TestConstants.LightMasterModKey2)
            .Should().BeTrue();
        listings
            .ListsMods(TestConstants.LightMasterModKey3)
            .Should().BeTrue();
        listings
            .ListsMods(TestConstants.LightMasterModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Typical()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightMasterModKey, true),
            new LoadOrderListing(TestConstants.LightMasterModKey2, false),
            new LoadOrderListing(TestConstants.LightMasterModKey3, true),
        };
        listings
            .ListsMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey3)
            .Should().BeTrue();
        listings
            .ListsMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
            .Should().BeTrue();
        listings
            .ListsMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Enabled_EmptyListings()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(true, TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
            .Should().BeFalse();
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(false, TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
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
            new LoadOrderListing(TestConstants.LightMasterModKey, true),
            new LoadOrderListing(TestConstants.LightMasterModKey2, false),
            new LoadOrderListing(TestConstants.LightMasterModKey3, true),
        };
        listings
            .ListsMods(true, TestConstants.LightMasterModKey)
            .Should().BeTrue();
        listings
            .ListsMods(false, TestConstants.LightMasterModKey)
            .Should().BeFalse();
        listings
            .ListsMods(false, TestConstants.LightMasterModKey2)
            .Should().BeTrue();
        listings
            .ListsMods(true, TestConstants.LightMasterModKey2)
            .Should().BeFalse();
        listings
            .ListsMods(true, TestConstants.LightMasterModKey3)
            .Should().BeTrue();
        listings
            .ListsMods(false, TestConstants.LightMasterModKey3)
            .Should().BeFalse();
        listings
            .ListsMods(true, TestConstants.LightMasterModKey4)
            .Should().BeFalse();
        listings
            .ListsMods(false, TestConstants.LightMasterModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Enabled_Typical()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightMasterModKey, true),
            new LoadOrderListing(TestConstants.LightMasterModKey2, false),
            new LoadOrderListing(TestConstants.LightMasterModKey3, true),
        };
        listings
            .ListsMods(
                true,
                TestConstants.LightMasterModKey, TestConstants.LightMasterModKey3)
            .Should().BeTrue();
        listings
            .ListsMods(false, TestConstants.LightMasterModKey2)
            .Should().BeTrue();
        listings
            .ListsMods(
                true,
                TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey3)
            .Should().BeFalse();
        listings
            .ListsMods(
                true,
                TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey4)
            .Should().BeFalse();
    }
}