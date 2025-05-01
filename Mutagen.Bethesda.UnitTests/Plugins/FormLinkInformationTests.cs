using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class FormLinkInformationTests
{
    [Fact]
    public void String()
    {
        var fk = new FormLinkInformation(FormKey.Factory("123456:Skyrim.esm"), typeof(Npc));
        fk.ToString().ShouldBe("123456:Skyrim.esm<Skyrim.Npc>");
    }
    
    [Fact]
    public void FromString()
    {
        var fk = FormLinkInformation.Factory("123456:Skyrim.esm<Skyrim.Npc>");
        fk.FormKey.ShouldBe(FormKey.Factory("123456:Skyrim.esm"));
        fk.Type.ShouldBe(typeof(Npc));
    }

    [Fact]
    public void TryToStandardizedIdentifier()
    {
        var fk = FormLinkInformation.Factory("123456:Skyrim.esm<Skyrim.Npc>");
        fk.TryToStandardizedIdentifier(out var standardized)
            .ShouldBeTrue();
        standardized.FormKey.ShouldBe(FormKey.Factory("123456:Skyrim.esm"));
        standardized.Type.ShouldBe(typeof(INpcGetter));
    }
}