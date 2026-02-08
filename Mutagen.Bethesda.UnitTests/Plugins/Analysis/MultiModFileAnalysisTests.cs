using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class MultiModFileAnalysisTests
{
    [Fact]
    public void IsSplitFileName_NumericSuffix_ReturnsTrue()
    {
        MultiModFileAnalysis.IsSplitFileName("MyMod_2", "MyMod").ShouldBeTrue();
    }

    [Fact]
    public void IsSplitFileName_NonNumericSuffix_ReturnsFalse()
    {
        MultiModFileAnalysis.IsSplitFileName("MyMod_Patch", "MyMod").ShouldBeFalse();
    }

    [Fact]
    public void IsSplitFileName_NoUnderscore_ReturnsFalse()
    {
        MultiModFileAnalysis.IsSplitFileName("MyMod2", "MyMod").ShouldBeFalse();
    }

    [Fact]
    public void IsSplitFileName_DifferentBase_ReturnsFalse()
    {
        MultiModFileAnalysis.IsSplitFileName("OtherMod_2", "MyMod").ShouldBeFalse();
    }

    [Fact]
    public void IsSplitFileName_WithIndex_ReturnsCorrectIndex()
    {
        MultiModFileAnalysis.IsSplitFileName("MyMod_3", "MyMod", out var index).ShouldBeTrue();
        index.ShouldBe(3);
    }

    [Fact]
    public void IsSplitFileName_LargeIndex_ReturnsCorrectIndex()
    {
        MultiModFileAnalysis.IsSplitFileName("MyMod_15", "MyMod", out var index).ShouldBeTrue();
        index.ShouldBe(15);
    }

    [Fact]
    public void IsSplitModSibling_MatchingType_ReturnsTrue()
    {
        var baseKey = new ModKey("MyMod", ModType.Plugin);
        var candidate = new ModKey("MyMod_2", ModType.Plugin);
        MultiModFileAnalysis.IsSplitModSibling(candidate, baseKey).ShouldBeTrue();
    }

    [Fact]
    public void IsSplitModSibling_DifferentType_ReturnsFalse()
    {
        var baseKey = new ModKey("MyMod", ModType.Plugin);
        var candidate = new ModKey("MyMod_2", ModType.Master);
        MultiModFileAnalysis.IsSplitModSibling(candidate, baseKey).ShouldBeFalse();
    }

    [Fact]
    public void IsSplitModSibling_NotASplitName_ReturnsFalse()
    {
        var baseKey = new ModKey("MyMod", ModType.Plugin);
        var candidate = new ModKey("OtherMod", ModType.Plugin);
        MultiModFileAnalysis.IsSplitModSibling(candidate, baseKey).ShouldBeFalse();
    }

    [Fact]
    public void IsSplitModSibling_SameModKey_ReturnsFalse()
    {
        var baseKey = new ModKey("MyMod", ModType.Plugin);
        MultiModFileAnalysis.IsSplitModSibling(baseKey, baseKey).ShouldBeFalse();
    }
}
