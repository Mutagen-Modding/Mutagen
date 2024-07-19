using System.IO.Abstractions;
using Loqui;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

internal class NoReleaseModInstantiatorTest : AModInstantiatorTest<OblivionMod, IOblivionMod, IOblivionModGetter, OblivionModBinaryOverlay>
{
    public override ModPath ModPath => TestDataPathing.OblivionTestMod;
    public override GameRelease Release => GameRelease.Oblivion;
    public override ILoquiRegistration Registration => OblivionMod_Registration.Instance;
}

internal class ReleaseModInstantiatorTest : AModInstantiatorTest<SkyrimMod, ISkyrimMod, ISkyrimModGetter, SkyrimModBinaryOverlay>
{
    public override ModPath ModPath => TestDataPathing.SkyrimTestMod;
    public override GameRelease Release => GameRelease.SkyrimSE;
    public override ILoquiRegistration Registration => SkyrimMod_Registration.Instance;
}

public abstract class AModInstantiatorTest<TDirect, TSetter, TGetter, TOverlay>
    where TDirect : IMod
    where TSetter : IMod
    where TGetter : IModGetter
    where TOverlay : IModGetter
{
    public abstract ModPath ModPath { get; }
    public abstract GameRelease Release { get; }
    public abstract ILoquiRegistration Registration { get; }

    [Fact]
    public void Direct()
    {
        var ret = ModInstantiatorReflection.GetActivator<TDirect>(Registration)(ModPath, Release);
        Assert.IsType<TDirect>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }

    [Fact]
    public void Setter()
    {
        var ret = ModInstantiatorReflection.GetActivator<TSetter>(Registration)(ModPath, Release);
        Assert.IsType<TDirect>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }

    [Fact]
    public void Getter()
    {
        var ret = ModInstantiatorReflection.GetActivator<TGetter>(Registration)(ModPath, Release);
        Assert.IsType<TDirect>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }
    [Fact]
    public void Import_Direct()
    {
        var ret = ModInstantiatorReflection.GetImporter<TDirect>(Registration)(
            ModPath,
            Release);
        Assert.IsType<TDirect>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }

    [Fact]
    public void Import_Setter()
    {
        var ret = ModInstantiatorReflection.GetImporter<TSetter>(Registration)(
            ModPath,
            Release);
        Assert.IsType<TDirect>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }

    [Fact]
    public void Import_Getter()
    {
        var ret = ModInstantiatorReflection.GetImporter<TGetter>(Registration)(
            ModPath,
            Release);
        Assert.IsType<TOverlay>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }
}