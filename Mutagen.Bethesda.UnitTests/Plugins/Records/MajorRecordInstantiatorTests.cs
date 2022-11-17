using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Utility;
using Xunit;
using Mutagen.Bethesda.Oblivion;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class MajorRecordInstantiatorTests
{
    [Fact]
    public void DirectGeneric()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator<Ammunition>.Activator(form, GameRelease.Oblivion);
        Assert.IsType<Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void SetterGeneric()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator<IAmmunition>.Activator(form, GameRelease.Oblivion);
        Assert.IsType<Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void GetterGeneric()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator<IAmmunitionGetter>.Activator(form, GameRelease.Oblivion);
        Assert.IsType<Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }
        
    [Fact]
    public void Direct()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator.Activator(form, GameRelease.Oblivion, typeof(Ammunition));
        Assert.IsType<Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void Setter()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator.Activator(form, GameRelease.Oblivion, typeof(IAmmunition));
        Assert.IsType<Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void Getter()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator.Activator(form, GameRelease.Oblivion, typeof(IAmmunitionGetter));
        Assert.IsType<Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }
}