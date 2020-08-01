using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class InstantiatorTests
    {
        [Fact]
        public void MajorRecord_Direct()
        {
            var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
            var ret = MajorRecordInstantiator<Ammunition>.Activator(form);
            Assert.IsType<Ammunition>(ret);
            Assert.Equal(form, ret.FormKey);
        }

        [Fact]
        public void MajorRecord_Setter()
        {
            var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
            var ret = MajorRecordInstantiator<IAmmunition>.Activator(form);
            Assert.IsType<Ammunition>(ret);
            Assert.Equal(form, ret.FormKey);
        }

        [Fact]
        public void MajorRecord_Getter()
        {
            var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
            var ret = MajorRecordInstantiator<IAmmunitionGetter>.Activator(form);
            Assert.IsType<Ammunition>(ret);
            Assert.Equal(form, ret.FormKey);
        }

        [Fact]
        public void Mod_Direct()
        {
            var modKey = new ModKey("Test", ModType.Plugin);
            var ret = ModInstantiator<OblivionMod>.Activator(modKey);
            Assert.IsType<OblivionMod>(ret);
            Assert.Equal(modKey, ret.ModKey);
        }

        [Fact]
        public void Mod_Setter()
        {
            var modKey = new ModKey("Test", ModType.Plugin);
            var ret = ModInstantiator<IOblivionMod>.Activator(modKey);
            Assert.IsType<OblivionMod>(ret);
            Assert.Equal(modKey, ret.ModKey);
        }

        [Fact]
        public void Mod_Getter()
        {
            var modKey = new ModKey("Test", ModType.Plugin);
            var ret = ModInstantiator<IOblivionModGetter>.Activator(modKey);
            Assert.IsType<OblivionMod>(ret);
            Assert.Equal(modKey, ret.ModKey);
        }

        [Fact]
        public void Mod_Import_Direct()
        {
            var modKey = new ModKey("Test", ModType.Plugin);
            var ret = ModInstantiator<OblivionMod>.Importer(new ModPath(modKey, Utility.PathToTestFile));
            Assert.IsType<OblivionMod>(ret);
            Assert.Equal(modKey, ret.ModKey);
        }

        [Fact]
        public void Mod_Import_Setter()
        {
            var modKey = new ModKey("Test", ModType.Plugin);
            var ret = ModInstantiator<IOblivionMod>.Importer(new ModPath(modKey, Utility.PathToTestFile));
            Assert.IsType<OblivionMod>(ret);
            Assert.Equal(modKey, ret.ModKey);
        }

        [Fact]
        public void Mod_Import_Getter()
        {
            var modKey = new ModKey("Test", ModType.Plugin);
            var ret = ModInstantiator<IOblivionModGetter>.Importer(new ModPath(modKey, Utility.PathToTestFile));
            Assert.IsType<OblivionModBinaryOverlay>(ret);
            Assert.Equal(modKey, ret.ModKey);
        }
    }
}
