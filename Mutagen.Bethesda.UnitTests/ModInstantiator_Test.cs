using Loqui;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class NoReleaseModInstantiator_Test : AModInstantiator_Test<OblivionMod, IOblivionMod, IOblivionModGetter, OblivionModBinaryOverlay>
    {
        public override ModPath ModPath => Utility.OblivionTestMod;
        public override GameRelease Release => GameRelease.Oblivion;
        public override ILoquiRegistration Registration => OblivionMod_Registration.Instance;
    }

    public class ReleaseModInstantiator_Test : AModInstantiator_Test<SkyrimMod, ISkyrimMod, ISkyrimModGetter, SkyrimModBinaryOverlay>
    {
        public override ModPath ModPath => Utility.SkyrimTestMod;
        public override GameRelease Release => GameRelease.SkyrimSE;
        public override ILoquiRegistration Registration => SkyrimMod_Registration.Instance;
    }

    public abstract class AModInstantiator_Test<TDirect, TSetter, TGetter, TOverlay>
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
            var ret = ModInstantiator.GetActivator<TDirect>(Registration)(ModPath, Release);
            Assert.IsType<TDirect>(ret);
            Assert.Equal(ModPath.ModKey, ret.ModKey);
        }

        [Fact]
        public void Setter()
        {
            var ret = ModInstantiator.GetActivator<TSetter>(Registration)(ModPath, Release);
            Assert.IsType<TDirect>(ret);
            Assert.Equal(ModPath.ModKey, ret.ModKey);
        }

        [Fact]
        public void Getter()
        {
            var ret = ModInstantiator.GetActivator<TGetter>(Registration)(ModPath, Release);
            Assert.IsType<TDirect>(ret);
            Assert.Equal(ModPath.ModKey, ret.ModKey);
        }
        [Fact]
        public void Import_Direct()
        {
            var ret = ModInstantiator.GetImporter<TDirect>(Registration)(
                ModPath,
                Release);
            Assert.IsType<TDirect>(ret);
            Assert.Equal(ModPath.ModKey, ret.ModKey);
        }

        [Fact]
        public void Import_Setter()
        {
            var ret = ModInstantiator.GetImporter<TSetter>(Registration)(
                ModPath,
                Release);
            Assert.IsType<TDirect>(ret);
            Assert.Equal(ModPath.ModKey, ret.ModKey);
        }

        [Fact]
        public void Import_Getter()
        {
            var ret = ModInstantiator.GetImporter<TGetter>(Registration)(
                ModPath,
                Release);
            Assert.IsType<TOverlay>(ret);
            Assert.Equal(ModPath.ModKey, ret.ModKey);
        }
    }
}
