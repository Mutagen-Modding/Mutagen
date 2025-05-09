using AutoFixture.Kernel;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class ModKeyBuilderTests
{
    [Theory, BasicAutoData]
    public void TypeReturnsModKey(
        ISpecimenContext context,
        ModKeyBuilder sut)
    {
        sut.Create(typeof(ModKey), context)
            .ShouldBeOfType<ModKey>();
    }
        
    [Theory, BasicAutoData]
    public void TypeReturnsPlugin(
        ISpecimenContext context,
        ModKeyBuilder sut)
    {
        ((ModKey) sut.Create(typeof(ModKey), context))
            .Type.ShouldBe(ModType.Plugin);
    }
        
    [Theory, BasicAutoData]
    public void TypeReturnsDifferent(
        ISpecimenContext context,
        ModKeyBuilder sut)
    {
        sut.Create(typeof(ModKey), context)
            .ShouldNotBe(
                sut.Create(typeof(ModKey), context));
    }

    public class Wrapper
    {
        public ModKey ModKey { get; set; }
    }
        
    [Theory, MutagenAutoData]
    public void ModKeyReset1(Wrapper mk)
    {
        mk.ModKey.ToString().ShouldBe("Mod0.esp");
    }
        
    [Theory, MutagenAutoData]
    public void ModKeyReset2(Wrapper mk)
    {
        mk.ModKey.ToString().ShouldBe("Mod0.esp");
    }
    
    [Theory, MutagenAutoData]
    public void ModKeyFuncBuilder(Func<ModKey> modKeyFunc)
    {
        var modKey = modKeyFunc();
        modKey.ShouldNotBe(ModKey.Null);
        var modKey2 = modKeyFunc();
        modKey2.ShouldNotBe(ModKey.Null);
        modKey2.ShouldNotBe(modKey);
    }

}