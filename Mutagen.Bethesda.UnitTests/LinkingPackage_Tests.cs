using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class LinkingPackage_Tests
    {
        public static FormKey SomeFormKey = new FormKey(ModKey.Dummy, 123456);

        #region Direct Mod
        [Fact]
        public void Direct_Empty()
        {
            var package = new DirectModLinkingPackage<OblivionMod>(new OblivionMod(ModKey.Dummy));

            // Test query fails
            Assert.False(package.TryGetMajorRecord(SomeFormKey, out var _));
            Assert.False(package.TryGetMajorRecord(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(SomeFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(SomeFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(FormKey.NULL, out var _));
        }

        [Fact]
        public void Direct_Typical()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var npc1 = mod.NPCs.AddNew();
            var npc2 = mod.NPCs.AddNew();
            var package = new DirectModLinkingPackage<OblivionMod>(mod);
            
            // Test query fails
            Assert.False(package.TryGetMajorRecord(SomeFormKey, out var _));
            Assert.False(package.TryGetMajorRecord(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(SomeFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(SomeFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(FormKey.NULL, out var _));

            // Test query successes
            {
                Assert.True(package.TryGetMajorRecord(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            Assert.False(package.TryGetMajorRecord(FormKey.NULL, out var _));
            {
                Assert.True(package.TryGetMajorRecord<IMajorRecordCommonGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<IMajorRecordCommonGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(FormKey.NULL, out var _));
            {
                Assert.True(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(FormKey.NULL, out var _));
        }
        #endregion
    }
}
