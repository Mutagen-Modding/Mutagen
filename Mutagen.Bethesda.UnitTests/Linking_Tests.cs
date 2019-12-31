using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Linking_Tests
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

        #region FormLink Resolves
        [Fact]
        public void FormLink_TryResolve_NoLink()
        {
            FormIDLink<INPC> formLink = new FormIDLink<INPC>(SomeFormKey);
            var package = new DirectModLinkingPackage<OblivionMod>(new OblivionMod(ModKey.Dummy));
            Assert.False(formLink.TryResolve(package, out var link));
        }

        [Fact]
        public void FormLink_TryResolve_Linked()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var npc = mod.NPCs.AddNew();
            var package = new DirectModLinkingPackage<OblivionMod>(mod);
            FormIDLink<INPC> formLink = new FormIDLink<INPC>(npc.FormKey);
            Assert.True(formLink.TryResolve(package, out var linkedRec));
            Assert.Same(npc, linkedRec);
        }

        [Fact]
        public void FormLink_Resolve_NoLink()
        {
            FormIDLink<INPC> formLink = new FormIDLink<INPC>(SomeFormKey);
            var package = new DirectModLinkingPackage<OblivionMod>(new OblivionMod(ModKey.Dummy));
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Resolve_Linked()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var npc = mod.NPCs.AddNew();
            var package = new DirectModLinkingPackage<OblivionMod>(mod);
            FormIDLink<INPC> formLink = new FormIDLink<INPC>(npc.FormKey);
            Assert.Same(npc, formLink.Resolve(package));
        }
        #endregion

        #region EDIDLink Resolves
        [Fact]
        public void EDIDLink_TryResolve_NoLink()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var package = new DirectModLinkingPackage<OblivionMod>(mod);
            Assert.False(link.TryResolve(package, out var _));
        }

        [Fact]
        public void EDIDLink_TryResolve_Linked()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var package = new DirectModLinkingPackage<OblivionMod>(mod);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.True(link.TryResolve(package, out var linkedRec));
            Assert.Same(effect, linkedRec);
        }

        [Fact]
        public void EDIDLink_Resolve_NoLink()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var package = new DirectModLinkingPackage<OblivionMod>(mod);
            Assert.Null(link.Resolve(package));
        }

        [Fact]
        public void EDIDLink_Resolve_Linked()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var package = new DirectModLinkingPackage<OblivionMod>(mod);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.Same(effect, link.Resolve(package));
        }
        #endregion
    }
}
