using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog.StructuredStrings;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class FormLinkOrIndexTests
{
    private record Flags(bool UseAliases = false, bool UsePackageData = false) : IFormLinkOrIndexFlagGetter;

    [Fact]
    public void StringOfLink()
    {
        var item = new FormLinkOrIndex<IKeywordGetter>(new Flags(), FormKey.Factory("123456:Skyrim.esm"));
        item.ToString().ShouldBe("123456:Skyrim.esm");
    }

    [Fact]
    public void StringOfAlias()
    {
        var item = new FormLinkOrIndex<IKeywordGetter>(new Flags(UseAliases: true), 3);
        item.ToString().ShouldBe("Alias 3");
    }

    [Fact]
    public void StringOfPackageData()
    {
        var item = new FormLinkOrIndexGetter<IKeywordGetter>(new Flags(UsePackageData: true), 3);
        item.ToString().ShouldBe("Package Data 3");
    }

    [Fact]
    public void Print()
    {
        var sb = new StructuredStringBuilder();
        sb.AppendItem(new FormLinkOrIndex<IKeywordGetter>(new Flags(UseAliases: true), 3), "Parameter");
        sb.ShouldContain("Parameter => Alias 3");
    }
}
