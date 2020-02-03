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
        public static string PathToTestFile = "../../test.esp";
        public static string PathToOverrideFile = "../../override.esp";
        public static FormKey TestFileFormKey = new FormKey(ModKey.Factory("test.esp"), 0xD62);
        public static FormKey TestFileFormKey2 = new FormKey(ModKey.Factory("test.esp"), 0xD63);

        #region Direct Mod
        [Fact]
        public void Direct_Empty()
        {
            var package = new DirectModLinkingPackage<OblivionMod>(new OblivionMod(ModKey.Dummy));

            // Test query fails
            Assert.False(package.TryGetMajorRecord(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<INPCGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<INPCGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<NPC>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<NPC>(FormKey.NULL, out var _));
        }

        [Fact]
        public void Direct_NoMatch()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            mod.NPCs.AddNew();
            var package = new DirectModLinkingPackage<OblivionMod>(mod);

            // Test query fails
            Assert.False(package.TryGetMajorRecord(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<INPCGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<INPCGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<NPC>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<NPC>(FormKey.NULL, out var _));
        }

        [Fact]
        public void Direct_Typical()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var npc1 = mod.NPCs.AddNew();
            var npc2 = mod.NPCs.AddNew();
            var package = new DirectModLinkingPackage<OblivionMod>(mod);

            {
                Assert.True(package.TryGetMajorRecord(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<IMajorRecordCommonGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<IMajorRecordCommonGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<INPCGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<INPCGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<NPC>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<NPC>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
        }

        [Fact]
        public void Direct_ReadOnlyMechanics()
        {
            var wrapper = OblivionMod.CreateFromBinaryOverlay(PathToTestFile);
            var package = wrapper.CreateLinkingPackage();
            {
                Assert.True(package.TryGetMajorRecord<INPCGetter>(TestFileFormKey, out var rec));
            }
            {
                Assert.False(package.TryGetMajorRecord<NPC>(TestFileFormKey, out var rec));
            }
        }
        #endregion

        #region Modlist
        [Fact]
        public void ModList_Empty()
        {
            var package = new ModListLinkingPackage<OblivionMod>(new ModList<OblivionMod>());

            // Test query fails
            Assert.False(package.TryGetMajorRecord(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(FormKey.NULL, out var _));
        }

        [Fact]
        public void ModList_NoMatch()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            mod.NPCs.AddNew();
            var modList = new ModList<OblivionMod>();
            modList.Add(mod);
            var package = new ModListLinkingPackage<OblivionMod>(modList);

            // Test query fails
            Assert.False(package.TryGetMajorRecord(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IMajorRecordCommonGetter>(FormKey.NULL, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(FormKey.NULL, out var _));
        }

        [Fact]
        public void ModList_Single()
        {
            var mod = new OblivionMod(ModKey.Dummy);
            var npc1 = mod.NPCs.AddNew();
            var npc2 = mod.NPCs.AddNew();
            var modList = new ModList<OblivionMod>();
            modList.Add(mod);
            var package = new ModListLinkingPackage<OblivionMod>(modList);

            // Test query successes
            {
                Assert.True(package.TryGetMajorRecord(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<IMajorRecordCommonGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<IMajorRecordCommonGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<INPCGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<INPCGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<NPC>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<NPC>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
        }

        [Fact]
        public void ModList_OneInEach()
        {
            var mod1 = new OblivionMod(ModKey.Dummy);
            var mod2 = new OblivionMod(new ModKey("Dummy2", true));
            var npc1 = mod1.NPCs.AddNew();
            var npc2 = mod2.NPCs.AddNew();
            var modList = new ModList<OblivionMod>();
            modList.Add(mod1);
            modList.Add(mod2);
            var package = new ModListLinkingPackage<OblivionMod>(modList);

            // Test query successes
            {
                Assert.True(package.TryGetMajorRecord(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<IMajorRecordCommonGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<IMajorRecordCommonGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<INPCGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<INPCGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryGetMajorRecord<NPC>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryGetMajorRecord<NPC>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
        }

        [Fact]
        public void ModList_Overridden()
        {
            var mod1 = new OblivionMod(ModKey.Dummy);
            var mod2 = new OblivionMod(new ModKey("Dummy2", true));
            var unoverriddenNPC = mod1.NPCs.AddNew();
            var overriddenNPC = mod1.NPCs.AddNew();
            var topModNPC = mod2.NPCs.AddNew();
            var overrideNPC = (NPC)overriddenNPC.DeepCopy();
            mod2.NPCs.RecordCache.Set(overrideNPC);
            var modList = new ModList<OblivionMod>();
            modList.Add(mod1);
            modList.Add(mod2);
            var package = new ModListLinkingPackage<OblivionMod>(modList);

            // Test query successes
            {
                Assert.True(package.TryGetMajorRecord(overriddenNPC.FormKey, out var rec));
                Assert.Same(rec, overrideNPC);
                Assert.NotSame(rec, overriddenNPC);
                Assert.True(package.TryGetMajorRecord(unoverriddenNPC.FormKey, out rec));
                Assert.Same(rec, unoverriddenNPC);
                Assert.True(package.TryGetMajorRecord(topModNPC.FormKey, out rec));
                Assert.Same(rec, topModNPC);
            }
            {
                Assert.True(package.TryGetMajorRecord<IMajorRecordCommonGetter>(overriddenNPC.FormKey, out var rec));
                Assert.Same(rec, overrideNPC);
                Assert.NotSame(rec, overriddenNPC);
                Assert.True(package.TryGetMajorRecord<IMajorRecordCommonGetter>(unoverriddenNPC.FormKey, out rec));
                Assert.Same(rec, unoverriddenNPC);
                Assert.True(package.TryGetMajorRecord<IMajorRecordCommonGetter>(topModNPC.FormKey, out rec));
                Assert.Same(rec, topModNPC);
            }
            {
                Assert.True(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(overriddenNPC.FormKey, out var rec));
                Assert.Same(rec, overrideNPC);
                Assert.NotSame(rec, overriddenNPC);
                Assert.True(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(unoverriddenNPC.FormKey, out rec));
                Assert.Same(rec, unoverriddenNPC);
                Assert.True(package.TryGetMajorRecord<IOblivionMajorRecordGetter>(topModNPC.FormKey, out rec));
                Assert.Same(rec, topModNPC);
            }
            {
                Assert.True(package.TryGetMajorRecord<INPCGetter>(overriddenNPC.FormKey, out var rec));
                Assert.Same(rec, overrideNPC);
                Assert.NotSame(rec, overriddenNPC);
                Assert.True(package.TryGetMajorRecord<INPCGetter>(unoverriddenNPC.FormKey, out rec));
                Assert.Same(rec, unoverriddenNPC);
                Assert.True(package.TryGetMajorRecord<INPCGetter>(topModNPC.FormKey, out rec));
                Assert.Same(rec, topModNPC);
            }
            {
                Assert.True(package.TryGetMajorRecord<NPC>(overriddenNPC.FormKey, out var rec));
                Assert.Same(rec, overrideNPC);
                Assert.NotSame(rec, overriddenNPC);
                Assert.True(package.TryGetMajorRecord<NPC>(unoverriddenNPC.FormKey, out rec));
                Assert.Same(rec, unoverriddenNPC);
                Assert.True(package.TryGetMajorRecord<NPC>(topModNPC.FormKey, out rec));
                Assert.Same(rec, topModNPC);
            }
        }

        [Fact]
        public void ModList_ReadOnlyMechanics()
        {
            var wrapper = OblivionMod.CreateFromBinaryOverlay(PathToTestFile);
            var overrideWrapper = OblivionMod.CreateFromBinaryOverlay(PathToOverrideFile);
            var modlist = new ModList<IOblivionModGetter>();
            modlist.Add(wrapper);
            modlist.Add(overrideWrapper);
            var package = modlist.CreateLinkingPackage();
            {
                Assert.True(package.TryGetMajorRecord<INPCGetter>(TestFileFormKey, out var rec));
                Assert.True(package.TryGetMajorRecord<INPCGetter>(TestFileFormKey2, out rec));
                Assert.True(rec.Name.TryGet(out var name));
                Assert.Equal("A Name", name);
            }
            {
                Assert.False(package.TryGetMajorRecord<NPC>(TestFileFormKey, out var rec));
                Assert.False(package.TryGetMajorRecord<NPC>(TestFileFormKey2, out rec));
            }
        }
        #endregion

        #region FormLink Resolves
        [Fact]
        public void FormLink_TryResolve_NoLink()
        {
            FormIDLink<INPC> formLink = new FormIDLink<INPC>(UnusedFormKey);
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
            FormIDLink<INPC> formLink = new FormIDLink<INPC>(UnusedFormKey);
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
