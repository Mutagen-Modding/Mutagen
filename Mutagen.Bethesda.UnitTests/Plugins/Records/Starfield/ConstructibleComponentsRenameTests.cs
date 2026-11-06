using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

#pragma warning disable CS0618 // Type or member is obsolete
public class ConstructibleComponentsRenameTests
{
    private static ConstructibleObjectComponent Component(uint count, ModKey? owner = null) => new()
    {
        Component = new FormLink<IItemGetter>(new FormKey(owner ?? ModKey.FromFileName("Test.esp"), 0x123456)),
        RequiredCount = count,
    };

    [Fact]
    public void ObsoleteClassMemberForwardsToNewMember()
    {
        var cobj = new ConstructibleObject(FormKey.Factory("000800:Test.esp"), StarfieldRelease.Starfield);
        cobj.ConstructableComponents.ShouldBeNull();

        cobj.ConstructableComponents = new ExtendedList<ConstructibleObjectComponent> { Component(1) };
        cobj.ConstructibleComponents.ShouldBeSameAs(cobj.ConstructableComponents);

        cobj.ConstructibleComponents = new ExtendedList<ConstructibleObjectComponent> { Component(2) };
        cobj.ConstructableComponents.ShouldBeSameAs(cobj.ConstructibleComponents);

        IConstructibleObject setter = cobj;
        setter.ConstructableComponents.ShouldBeSameAs(cobj.ConstructibleComponents);
        setter.ConstructableComponents = null;
        cobj.ConstructibleComponents.ShouldBeNull();

        IConstructibleObjectGetter getter = cobj;
        getter.ConstructableComponents.ShouldBeNull();
        cobj.ConstructibleComponents = new ExtendedList<ConstructibleObjectComponent> { Component(3) };
        getter.ConstructableComponents.ShouldBeSameAs(getter.ConstructibleComponents);
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void ObsoleteGetterMemberForwardsOnOverlay(StarfieldMod mod)
    {
        var cobj = mod.ConstructibleObjects.AddNew();
        cobj.ConstructibleComponents = new ExtendedList<ConstructibleObjectComponent> { Component(4, mod.ModKey), Component(5, mod.ModKey) };

        var stream = new MemoryTributary();
        mod.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = StarfieldMod.CreateFromBinaryOverlay(stream, StarfieldRelease.Starfield, mod.ModKey);

        var read = overlay.ConstructibleObjects.ShouldHaveSingleItem();
        read.ConstructibleComponents.ShouldNotBeNull();
        read.ConstructibleComponents.Select(c => c.RequiredCount).ShouldBe(new uint[] { 4, 5 });
        read.ConstructableComponents.ShouldBeSameAs(read.ConstructibleComponents);
    }
}
#pragma warning restore CS0618
