using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class FormKeyBuilderTests
{
    [Theory, MutagenAutoData]
    public void FormKeyFuncBuilder(Func<FormKey> formKeyFunc)
    {
        var formKey = formKeyFunc();
        formKey.ModKey.ShouldNotBe(ModKey.Null);
        formKey.ID.ShouldBe(0x800u);
        var formKey2 = formKeyFunc();
        formKey2.ModKey.ShouldNotBe(ModKey.Null);
        formKey2.ModKey.ShouldNotBe(formKey.ModKey);
        formKey2.ID.ShouldBe(0x800u);
    }
    
    [Theory, MutagenAutoData]
    public void FormKeyBuilder(FormKey formKey, FormKey formKey2)
    {
        formKey.ModKey.ShouldNotBe(ModKey.Null);
        formKey.ID.ShouldBe(0x800u);
        formKey2.ModKey.ShouldNotBe(ModKey.Null);
        formKey2.ModKey.ShouldNotBe(formKey.ModKey);
        formKey2.ID.ShouldBe(0x800u);
    }
}