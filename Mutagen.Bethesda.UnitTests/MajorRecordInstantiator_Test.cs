using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class MajorRecordInstantiator_Test
    {
        [Fact]
        public void Direct()
        {
            WarmupOblivion.Init();
            var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
            var ret = MajorRecordInstantiator<Oblivion.Ammunition>.Activator(form, GameRelease.Oblivion);
            Assert.IsType<Oblivion.Ammunition>(ret);
            Assert.Equal(form, ret.FormKey);
        }

        [Fact]
        public void Setter()
        {
            WarmupOblivion.Init();
            var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
            var ret = MajorRecordInstantiator<Oblivion.IAmmunition>.Activator(form, GameRelease.Oblivion);
            Assert.IsType<Oblivion.Ammunition>(ret);
            Assert.Equal(form, ret.FormKey);
        }

        [Fact]
        public void Getter()
        {
            WarmupOblivion.Init();
            var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
            var ret = MajorRecordInstantiator<Oblivion.IAmmunitionGetter>.Activator(form, GameRelease.Oblivion);
            Assert.IsType<Oblivion.Ammunition>(ret);
            Assert.Equal(form, ret.FormKey);
        }
    }
}
