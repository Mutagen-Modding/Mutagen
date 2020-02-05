using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Linking_Tests
    {
        public static FormKey UnusedFormKey = new FormKey(ModKey.Dummy, 123456);
        public static string PathToTestFile = "../../../test.esp";
        public static string PathToOverrideFile = "../../../override.esp";
        public static FormKey TestFileFormKey = new FormKey(ModKey.Factory("test.esp"), 0xD62);
        public static FormKey TestFileFormKey2 = new FormKey(ModKey.Factory("test.esp"), 0xD63);

        #region Direct Mod
        [Fact]
        public void Direct_Empty()
        {
            var package = new DirectModLinkCache<OblivionMod>(new OblivionMod(ModKey.Dummy));

            // Test query fails
            Assert.False(package.TryLookup(UnusedFormKey, out var _));
            Assert.False(package.TryLookup(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<INPCGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<INPCGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<NPC>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<NPC>(FormKey.NULL, out var _));
        }

        [Fact]
        public void Direct_NoMatch()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            mod.NPCs.AddNew();
            var package = new DirectModLinkCache<OblivionMod>(mod);

            // Test query fails
            Assert.False(package.TryLookup(UnusedFormKey, out var _));
            Assert.False(package.TryLookup(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<INPCGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<INPCGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<NPC>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<NPC>(FormKey.NULL, out var _));
        }

        [Fact]
        public void Direct_Typical()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var npc1 = mod.NPCs.AddNew();
            var npc2 = mod.NPCs.AddNew();
            var package = new DirectModLinkCache<OblivionMod>(mod);

            {
                Assert.True(package.TryLookup(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<INPCGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<INPCGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<NPC>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<NPC>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
        }

        [Fact]
        public void Direct_ReadOnlyMechanics()
        {
            var wrapper = OblivionMod.CreateFromBinaryOverlay(PathToTestFile);
            var package = wrapper.CreateLinkCache();
            {
                Assert.True(package.TryLookup<INPCGetter>(TestFileFormKey, out var rec));
            }
            {
                Assert.False(package.TryLookup<NPC>(TestFileFormKey, out var rec));
            }
        }
        #endregion

        #region LoadOrder
        [Fact]
        public void LoadOrder_Empty()
        {
            var package = new LoadOrderLinkCache<OblivionMod>(new LoadOrder<OblivionMod>());

            // Test query fails
            Assert.False(package.TryLookup(UnusedFormKey, out var _));
            Assert.False(package.TryLookup(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(FormKey.NULL, out var _));
        }

        [Fact]
        public void LoadOrder_NoMatch()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            mod.NPCs.AddNew();
            var loadOrder = new LoadOrder<OblivionMod>();
            loadOrder.Add(mod);
            var package = new LoadOrderLinkCache<OblivionMod>(loadOrder);

            // Test query fails
            Assert.False(package.TryLookup(UnusedFormKey, out var _));
            Assert.False(package.TryLookup(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(FormKey.NULL, out var _));
        }

        [Fact]
        public void LoadOrder_Single()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var npc1 = mod.NPCs.AddNew();
            var npc2 = mod.NPCs.AddNew();
            var loadOrder = new LoadOrder<OblivionMod>();
            loadOrder.Add(mod);
            var package = new LoadOrderLinkCache<OblivionMod>(loadOrder);

            // Test query successes
            {
                Assert.True(package.TryLookup(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<INPCGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<INPCGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<NPC>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<NPC>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
        }

        [Fact]
        public void LoadOrder_OneInEach()
        {
            var mod1 = new OblivionMod(ModKey.Dummy);
            var mod2 = new OblivionMod(new ModKey("Dummy2", true));
            var npc1 = mod1.NPCs.AddNew();
            var npc2 = mod2.NPCs.AddNew();
            var loadOrder = new LoadOrder<OblivionMod>();
            loadOrder.Add(mod1);
            loadOrder.Add(mod2);
            var package = new LoadOrderLinkCache<OblivionMod>(loadOrder);

            // Test query successes
            {
                Assert.True(package.TryLookup(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<INPCGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<INPCGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<NPC>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<NPC>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
        }

        [Fact]
        public void LoadOrder_Overridden()
        {
            var mod1 = new OblivionMod(ModKey.Dummy);
            var mod2 = new OblivionMod(new ModKey("Dummy2", true));
            var unoverriddenNPC = mod1.NPCs.AddNew();
            var overriddenNPC = mod1.NPCs.AddNew();
            var topModNPC = mod2.NPCs.AddNew();
            var overrideNPC = (NPC)overriddenNPC.DeepCopy();
            mod2.NPCs.RecordCache.Set(overrideNPC);
            var loadOrder = new LoadOrder<OblivionMod>();
            loadOrder.Add(mod1);
            loadOrder.Add(mod2);
            var package = new LoadOrderLinkCache<OblivionMod>(loadOrder);

            // Test query successes
            {
                Assert.True(package.TryLookup(overriddenNPC.FormKey, out var rec));
                Assert.Same(rec, overrideNPC);
                Assert.NotSame(rec, overriddenNPC);
                Assert.True(package.TryLookup(unoverriddenNPC.FormKey, out rec));
                Assert.Same(rec, unoverriddenNPC);
                Assert.True(package.TryLookup(topModNPC.FormKey, out rec));
                Assert.Same(rec, topModNPC);
            }
            {
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(overriddenNPC.FormKey, out var rec));
                Assert.Same(rec, overrideNPC);
                Assert.NotSame(rec, overriddenNPC);
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(unoverriddenNPC.FormKey, out rec));
                Assert.Same(rec, unoverriddenNPC);
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(topModNPC.FormKey, out rec));
                Assert.Same(rec, topModNPC);
            }
            {
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(overriddenNPC.FormKey, out var rec));
                Assert.Same(rec, overrideNPC);
                Assert.NotSame(rec, overriddenNPC);
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(unoverriddenNPC.FormKey, out rec));
                Assert.Same(rec, unoverriddenNPC);
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(topModNPC.FormKey, out rec));
                Assert.Same(rec, topModNPC);
            }
            {
                Assert.True(package.TryLookup<INPCGetter>(overriddenNPC.FormKey, out var rec));
                Assert.Same(rec, overrideNPC);
                Assert.NotSame(rec, overriddenNPC);
                Assert.True(package.TryLookup<INPCGetter>(unoverriddenNPC.FormKey, out rec));
                Assert.Same(rec, unoverriddenNPC);
                Assert.True(package.TryLookup<INPCGetter>(topModNPC.FormKey, out rec));
                Assert.Same(rec, topModNPC);
            }
            {
                Assert.True(package.TryLookup<NPC>(overriddenNPC.FormKey, out var rec));
                Assert.Same(rec, overrideNPC);
                Assert.NotSame(rec, overriddenNPC);
                Assert.True(package.TryLookup<NPC>(unoverriddenNPC.FormKey, out rec));
                Assert.Same(rec, unoverriddenNPC);
                Assert.True(package.TryLookup<NPC>(topModNPC.FormKey, out rec));
                Assert.Same(rec, topModNPC);
            }
        }

        [Fact]
        public void LoadOrder_ReadOnlyMechanics()
        {
            var wrapper = OblivionMod.CreateFromBinaryOverlay(PathToTestFile);
            var overrideWrapper = OblivionMod.CreateFromBinaryOverlay(PathToOverrideFile);
            var loadOrder = new LoadOrder<IOblivionModGetter>();
            loadOrder.Add(wrapper);
            loadOrder.Add(overrideWrapper);
            var package = loadOrder.CreateLinkCache();
            {
                Assert.True(package.TryLookup<INPCGetter>(TestFileFormKey, out var rec));
                Assert.True(package.TryLookup<INPCGetter>(TestFileFormKey2, out rec));
                Assert.True(rec.Name.TryGet(out var name));
                Assert.Equal("A Name", name);
            }
            {
                Assert.False(package.TryLookup<NPC>(TestFileFormKey, out var rec));
                Assert.False(package.TryLookup<NPC>(TestFileFormKey2, out rec));
            }
        }
        #endregion

        #region FormLink Resolves
        [Fact]
        public void FormLink_TryResolve_NoLink()
        {
            FormIDLink<INPC> formLink = new FormIDLink<INPC>(UnusedFormKey);
            var package = new DirectModLinkCache<OblivionMod>(new OblivionMod(ModKey.Dummy));
            Assert.False(formLink.TryResolve(package, out var link));
        }

        [Fact]
        public void FormLink_TryResolve_Linked()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var npc = mod.NPCs.AddNew();
            var package = new DirectModLinkCache<OblivionMod>(mod);
            FormIDLink<INPC> formLink = new FormIDLink<INPC>(npc.FormKey);
            Assert.True(formLink.TryResolve(package, out var linkedRec));
            Assert.Same(npc, linkedRec);
        }

        [Fact]
        public void FormLink_Resolve_NoLink()
        {
            FormIDLink<INPC> formLink = new FormIDLink<INPC>(UnusedFormKey);
            var package = new DirectModLinkCache<OblivionMod>(new OblivionMod(ModKey.Dummy));
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Resolve_Linked()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var npc = mod.NPCs.AddNew();
            var package = new DirectModLinkCache<OblivionMod>(mod);
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
            var package = new DirectModLinkCache<OblivionMod>(mod);
            Assert.False(link.TryResolve(package, out var _));
        }

        [Fact]
        public void EDIDLink_TryResolve_Linked()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var package = new DirectModLinkCache<OblivionMod>(mod);
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
            var package = new DirectModLinkCache<OblivionMod>(mod);
            Assert.Null(link.Resolve(package));
        }

        [Fact]
        public void EDIDLink_Resolve_Linked()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var package = new DirectModLinkCache<OblivionMod>(mod);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.Same(effect, link.Resolve(package));
        }
        #endregion
    }
}
