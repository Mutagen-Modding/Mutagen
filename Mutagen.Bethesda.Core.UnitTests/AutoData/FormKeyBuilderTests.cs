using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;
using Xunit;

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
        formKey2.ModKey.ShouldBe(formKey.ModKey);
        formKey2.ID.ShouldBe(0x801u);
    }
    
    public class Wrapper
    {
        public FormKey FormKey { get; set; }
    }
    
    [Theory, MutagenAutoData]
    public void FormKeyBuilder(FormKey formKey, FormKey formKey2, Wrapper formKey3)
    {
        formKey.ModKey.ShouldNotBe(ModKey.Null);
        formKey.ID.ShouldBe(0x800u);
        formKey2.ModKey.ShouldNotBe(ModKey.Null);
        formKey2.ModKey.ShouldBe(formKey.ModKey);
        formKey2.ID.ShouldBe(0x801u);
        formKey3.FormKey.ModKey.ShouldNotBe(ModKey.Null);
        formKey3.FormKey.ModKey.ShouldBe(formKey.ModKey);
        formKey3.FormKey.ID.ShouldBe(0x802u);
    }
    
    [Theory, MutagenAutoData]
    public void FormKeyReset1(Wrapper mk)
    {
        mk.FormKey.ToString().ShouldBe("000800:Mod0.esp");
    }
        
    [Theory, MutagenAutoData]
    public void FormKeyReset2(Wrapper mk)
    {
        mk.FormKey.ToString().ShouldBe("000800:Mod0.esp");
    }
}