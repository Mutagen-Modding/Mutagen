using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Utility;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class MajorRecordInstantiatorTests
{
    [Fact]
    public void DirectGeneric()
    {
        WarmupOblivion.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator<Oblivion.Ammunition>.Activator(form, GameRelease.Oblivion);
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void SetterGeneric()
    {
        WarmupOblivion.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator<Oblivion.IAmmunition>.Activator(form, GameRelease.Oblivion);
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void GetterGeneric()
    {
        WarmupOblivion.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator<Oblivion.IAmmunitionGetter>.Activator(form, GameRelease.Oblivion);
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }
        
    [Fact]
    public void Direct()
    {
        WarmupOblivion.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator.Activator(form, GameRelease.Oblivion, typeof(Oblivion.Ammunition));
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void Setter()
    {
        WarmupOblivion.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator.Activator(form, GameRelease.Oblivion, typeof(Oblivion.IAmmunition));
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void Getter()
    {
        WarmupOblivion.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator.Activator(form, GameRelease.Oblivion, typeof(Oblivion.IAmmunitionGetter));
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }
}