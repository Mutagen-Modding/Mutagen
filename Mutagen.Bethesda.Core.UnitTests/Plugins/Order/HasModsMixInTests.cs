using Shouldly;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class HasModsMixInTests
{
    [Fact]
    public void HasMod_Empty()
    {
        Enumerable.Empty<ModListing>()
            .ModExists(TestConstants.LightModKey)
            .ShouldBeFalse();
    }

    [Fact]
    public void HasMod_Typical()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, modExists: true),
            new ModListing(TestConstants.LightModKey2, false, modExists: true),
            new ModListing(TestConstants.LightModKey3, true, modExists: true),
        };
        listings
            .ModExists(TestConstants.LightModKey)
            .ShouldBeTrue();
        listings
            .ModExists(TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ModExists(TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ModExists(TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void HasMod_Enabled()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, modExists: true),
            new ModListing(TestConstants.LightModKey2, false, modExists: true),
            new ModListing(TestConstants.LightModKey3, true, modExists: true),
        };
        listings
            .ModExists(TestConstants.LightModKey, enabled: true)
            .ShouldBeTrue();
        listings
            .ModExists(TestConstants.LightModKey, enabled: false)
            .ShouldBeFalse();
        listings
            .ModExists(TestConstants.LightModKey2, enabled: false)
            .ShouldBeTrue();
        listings
            .ModExists(TestConstants.LightModKey2, enabled: true)
            .ShouldBeFalse();
        listings
            .ModExists(TestConstants.LightModKey3, enabled: true)
            .ShouldBeTrue();
        listings
            .ModExists(TestConstants.LightModKey3, enabled: false)
            .ShouldBeFalse();
    }

    [Fact]
    public void EmptyListings()
    {
        Enumerable.Empty<ModListing>()
            .ModsExist(TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeFalse();
    }

    [Fact]
    public void EmptyInput()
    {
        Enumerable.Empty<ModListing>()
            .ModsExist()
            .ShouldBeTrue();
    }

    [Fact]
    public void Single()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, modExists: true),
            new ModListing(TestConstants.LightModKey2, false, modExists: true),
            new ModListing(TestConstants.LightModKey3, true, modExists: true),
        };
        listings
            .ModsExist(TestConstants.LightModKey)
            .ShouldBeTrue();
        listings
            .ModsExist(TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ModsExist(TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ModsExist(TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void Typical()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, modExists: true),
            new ModListing(TestConstants.LightModKey2, false, modExists: true),
            new ModListing(TestConstants.LightModKey3, true, modExists: true),
        };
        listings
            .ModsExist(TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ModsExist(TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ModsExist(TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void Enabled_EmptyListings()
    {
        Enumerable.Empty<ModListing>()
            .ModsExist(true, TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeFalse();
        Enumerable.Empty<ModListing>()
            .ModsExist(false, TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeFalse();
    }

    [Fact]
    public void Enabled_EmptyInput()
    {
        Enumerable.Empty<ModListing>()
            .ModsExist(true)
            .ShouldBeTrue();
        Enumerable.Empty<ModListing>()
            .ModsExist(false)
            .ShouldBeTrue();
    }

    [Fact]
    public void Enabled_Single()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, modExists: true),
            new ModListing(TestConstants.LightModKey2, false, modExists: true),
            new ModListing(TestConstants.LightModKey3, true, modExists: true),
        };
        listings
            .ModsExist(true, TestConstants.LightModKey)
            .ShouldBeTrue();
        listings
            .ModsExist(false, TestConstants.LightModKey)
            .ShouldBeFalse();
        listings
            .ModsExist(false, TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ModsExist(true, TestConstants.LightModKey2)
            .ShouldBeFalse();
        listings
            .ModsExist(true, TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ModsExist(false, TestConstants.LightModKey3)
            .ShouldBeFalse();
        listings
            .ModsExist(true, TestConstants.LightModKey4)
            .ShouldBeFalse();
        listings
            .ModsExist(false, TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void Exists_Single()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, modExists: true),
            new ModListing(TestConstants.LightModKey2, false, modExists: false),
            new ModListing(TestConstants.LightModKey3, true, modExists: true),
        };
        listings
            .ModsExist(TestConstants.LightModKey)
            .ShouldBeTrue();
        listings
            .ModsExist(TestConstants.LightModKey2)
            .ShouldBeFalse();
        listings
            .ModsExist(TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ModsExist(TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void Enabled_Typical()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, modExists: true),
            new ModListing(TestConstants.LightModKey2, false, modExists: true),
            new ModListing(TestConstants.LightModKey3, true, modExists: true),
        };
        listings
            .ModsExist(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ModsExist(false, TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ModsExist(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey3)
            .ShouldBeFalse();
        listings
            .ModsExist(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey4)
            .ShouldBeFalse();
    }
}