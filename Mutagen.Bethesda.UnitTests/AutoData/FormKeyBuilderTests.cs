using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class FormKeyBuilderTests
{
    [Theory, MutagenAutoData]
    public void FormKeyFuncBuilder(Func<FormKey> formKeyFunc)
    {
        var formKey = formKeyFunc();
        formKey.ModKey.Should().NotBeNull();
        formKey.ID.Should().Be(0x800);
        var formKey2 = formKeyFunc();
        formKey2.ModKey.Should().NotBeNull();
        formKey2.ModKey.Should().NotBe(formKey.ModKey);
        formKey2.ID.Should().Be(0x800);
    }
    
    [Theory, MutagenAutoData]
    public void FormKeyBuilder(FormKey formKey, FormKey formKey2)
    {
        formKey.ModKey.Should().NotBeNull();
        formKey.ID.Should().Be(0x800);
        formKey2.ModKey.Should().NotBeNull();
        formKey2.ModKey.Should().NotBe(formKey.ModKey);
        formKey2.ID.Should().Be(0x800);
    }
}