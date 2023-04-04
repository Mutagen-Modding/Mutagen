using FluentAssertions;
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
        fk.ToString().Should().Be("123456:Skyrim.esm<Skyrim.Npc>");
    }
    
    [Fact]
    public void FromString()
    {
        var fk = FormLinkInformation.Factory("123456:Skyrim.esm<Skyrim.Npc>");
        fk.FormKey.Should().Be(FormKey.Factory("123456:Skyrim.esm"));
        fk.Type.Should().Be(typeof(Npc));
    }
}