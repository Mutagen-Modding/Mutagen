using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class PluginCommentTrimmerTests
{
    [Theory]
    [MutagenAutoData]
    public void TypicalOff(
        PluginListingCommentTrimmer sut)
    {
        sut.Trim("ModB.esp")
            .ToString()
            .Should().Be("ModB.esp");
    }
    
    [Theory]
    [MutagenAutoData]
    public void TypicalOn(
        PluginListingCommentTrimmer sut)
    {
        sut.Trim("*ModB.esp")
            .ToString()
            .Should().Be("*ModB.esp");
    }
    
    [Theory]
    [MutagenAutoData]
    public void CommentsWithNoEntry(
        PluginListingCommentTrimmer sut)
    {
        sut.Trim("#ModB.esp")
            .ToString()
            .Should().Be("");
    }

    
    [Theory]
    [MutagenAutoData]
    public void HalfwayComment(
        PluginListingCommentTrimmer sut)
    {
        sut.Trim("Mod#B.esp")
            .ToString()
            .Should().Be("Mod");
    }

    [Theory]
    [MutagenAutoData]
    public void CommentTrimming(
        PluginListingCommentTrimmer sut)
    {
        sut.Trim("ModB.esp#Hello")
            .ToString()
            .Should().Be("ModB.esp");
    }
}