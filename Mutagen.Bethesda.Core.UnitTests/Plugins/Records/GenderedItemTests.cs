using Mutagen.Bethesda.Plugins.Records;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Core.UnitTests.Plugins.Records;

public class GenderedItemTests
{
    private sealed record LoquiLike(int Value);

    [Fact]
    public void Equals_SameMembers_ReturnsTrue()
    {
        var g1 = new GenderedItem<byte>(1, 1);
        var g2 = new GenderedItem<byte>(1, 1);
        g1.Equals(g2).ShouldBeTrue();
        g1.Equals((object)g2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentMale_ReturnsFalse()
    {
        var g1 = new GenderedItem<byte>(1, 1);
        var g2 = new GenderedItem<byte>(2, 1);
        g1.Equals(g2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentFemale_ReturnsFalse()
    {
        var g1 = new GenderedItem<byte>(1, 1);
        var g2 = new GenderedItem<byte>(1, 2);
        g1.Equals(g2).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameMembers_ReturnsSameHash()
    {
        var g1 = new GenderedItem<byte>(1, 2);
        var g2 = new GenderedItem<byte>(1, 2);
        g1.GetHashCode().ShouldBe(g2.GetHashCode());
    }

    [Fact]
    public void Equals_LoquiLikeMembers_UsesValueEquality()
    {
        var g1 = new GenderedItem<LoquiLike?>(new LoquiLike(1), new LoquiLike(2));
        var g2 = new GenderedItem<LoquiLike?>(new LoquiLike(1), new LoquiLike(2));
        var g3 = new GenderedItem<LoquiLike?>(new LoquiLike(1), new LoquiLike(3));

        g1.Equals(g2).ShouldBeTrue();
        g1.Equals(g3).ShouldBeFalse();
    }

    [Fact]
    public void Equals_NullMembers_DoesNotThrow()
    {
        var g1 = new GenderedItem<LoquiLike?>(null, null);
        var g2 = new GenderedItem<LoquiLike?>(null, null);
        g1.Equals(g2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Object_NullOrWrongType_ReturnsFalse()
    {
        var g1 = new GenderedItem<byte>(1, 1);
        g1.Equals((object?)null).ShouldBeFalse();
        g1.Equals("not a gendered item").ShouldBeFalse();
    }
}
