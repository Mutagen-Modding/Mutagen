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
    public class LinkingInit : IDisposable
    {
        public LinkingInit()
        {
            Warmup.Init();
        }

        public void Dispose()
        {
        }
    }

    public class Linking_Tests : IClassFixture<LinkingInit>
    {
        public static FormKey UnusedFormKey = new FormKey(Utility.ModKey, 123456);
        public static string PathToTestFile = "../../../test.esp";
        public static string PathToOverrideFile = "../../../override.esp";
        public static FormKey TestFileFormKey = new FormKey(ModKey.Factory("test.esp"), 0xD62);
        public static FormKey TestFileFormKey2 = new FormKey(ModKey.Factory("test.esp"), 0xD63);

        #region Direct Mod
        [Fact]
        public void Direct_Empty()
        {
            var package = new DirectModLinkCache<OblivionMod>(new OblivionMod(Utility.ModKey));

            // Test query fails
            Assert.False(package.TryLookup(UnusedFormKey, out var _));
            Assert.False(package.TryLookup(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<INpcGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<INpcGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<Npc>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<Npc>(FormKey.Null, out var _));
        }

        [Fact]
        public void Direct_NoMatch()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Npcs.AddNew();
            var package = new DirectModLinkCache<OblivionMod>(mod);

            // Test query fails
            Assert.False(package.TryLookup(UnusedFormKey, out var _));
            Assert.False(package.TryLookup(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<INpcGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<INpcGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<Npc>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<Npc>(FormKey.Null, out var _));
        }

        [Fact]
        public void Direct_Typical()
        {
            var mod = new OblivionMod(Utility.ModKey);
            var npc1 = mod.Npcs.AddNew();
            var npc2 = mod.Npcs.AddNew();
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
                Assert.True(package.TryLookup<INpcGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<INpcGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<Npc>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<Npc>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
        }

        [Fact]
        public void Direct_ReadOnlyMechanics()
        {
            var wrapper = OblivionMod.CreateFromBinaryOverlay(PathToTestFile);
            var package = wrapper.CreateLinkCache();
            {
                Assert.True(package.TryLookup<INpcGetter>(TestFileFormKey, out var rec));
            }
            {
                Assert.False(package.TryLookup<Npc>(TestFileFormKey, out var rec));
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
            Assert.False(package.TryLookup(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(FormKey.Null, out var _));
        }

        [Fact]
        public void LoadOrder_NoMatch()
        {
            var mod = new OblivionMod(Utility.ModKey);
            mod.Npcs.AddNew();
            var loadOrder = new LoadOrder<OblivionMod>();
            loadOrder.Add(mod);
            var package = new LoadOrderLinkCache<OblivionMod>(loadOrder);

            // Test query fails
            Assert.False(package.TryLookup(UnusedFormKey, out var _));
            Assert.False(package.TryLookup(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IOblivionMajorRecordGetter>(FormKey.Null, out var _));
        }

        [Fact]
        public void LoadOrder_Single()
        {
            var mod = new OblivionMod(Utility.ModKey);
            var npc1 = mod.Npcs.AddNew();
            var npc2 = mod.Npcs.AddNew();
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
                Assert.True(package.TryLookup<INpcGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<INpcGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<Npc>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<Npc>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
        }

        [Fact]
        public void LoadOrder_OneInEach()
        {
            var mod1 = new OblivionMod(Utility.ModKey);
            var mod2 = new OblivionMod(new ModKey("Dummy2", true));
            var npc1 = mod1.Npcs.AddNew();
            var npc2 = mod2.Npcs.AddNew();
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
                Assert.True(package.TryLookup<INpcGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<INpcGetter>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
            {
                Assert.True(package.TryLookup<Npc>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<Npc>(npc2.FormKey, out var rec));
                Assert.Same(rec, npc2);
            }
        }

        [Fact]
        public void LoadOrder_Overridden()
        {
            var mod1 = new OblivionMod(Utility.ModKey);
            var mod2 = new OblivionMod(new ModKey("Dummy2", true));
            var unoverriddenNpc = mod1.Npcs.AddNew();
            var overriddenNpc = mod1.Npcs.AddNew();
            var topModNpc = mod2.Npcs.AddNew();
            var overrideNpc = (Npc)overriddenNpc.DeepCopy();
            mod2.Npcs.RecordCache.Set(overrideNpc);
            var loadOrder = new LoadOrder<OblivionMod>();
            loadOrder.Add(mod1);
            loadOrder.Add(mod2);
            var package = new LoadOrderLinkCache<OblivionMod>(loadOrder);

            // Test query successes
            {
                Assert.True(package.TryLookup(overriddenNpc.FormKey, out var rec));
                Assert.Same(rec, overrideNpc);
                Assert.NotSame(rec, overriddenNpc);
                Assert.True(package.TryLookup(unoverriddenNpc.FormKey, out rec));
                Assert.Same(rec, unoverriddenNpc);
                Assert.True(package.TryLookup(topModNpc.FormKey, out rec));
                Assert.Same(rec, topModNpc);
            }
            {
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(overriddenNpc.FormKey, out var rec));
                Assert.Same(rec, overrideNpc);
                Assert.NotSame(rec, overriddenNpc);
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(unoverriddenNpc.FormKey, out rec));
                Assert.Same(rec, unoverriddenNpc);
                Assert.True(package.TryLookup<IMajorRecordCommonGetter>(topModNpc.FormKey, out rec));
                Assert.Same(rec, topModNpc);
            }
            {
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(overriddenNpc.FormKey, out var rec));
                Assert.Same(rec, overrideNpc);
                Assert.NotSame(rec, overriddenNpc);
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(unoverriddenNpc.FormKey, out rec));
                Assert.Same(rec, unoverriddenNpc);
                Assert.True(package.TryLookup<IOblivionMajorRecordGetter>(topModNpc.FormKey, out rec));
                Assert.Same(rec, topModNpc);
            }
            {
                Assert.True(package.TryLookup<INpcGetter>(overriddenNpc.FormKey, out var rec));
                Assert.Same(rec, overrideNpc);
                Assert.NotSame(rec, overriddenNpc);
                Assert.True(package.TryLookup<INpcGetter>(unoverriddenNpc.FormKey, out rec));
                Assert.Same(rec, unoverriddenNpc);
                Assert.True(package.TryLookup<INpcGetter>(topModNpc.FormKey, out rec));
                Assert.Same(rec, topModNpc);
            }
            {
                Assert.True(package.TryLookup<Npc>(overriddenNpc.FormKey, out var rec));
                Assert.Same(rec, overrideNpc);
                Assert.NotSame(rec, overriddenNpc);
                Assert.True(package.TryLookup<Npc>(unoverriddenNpc.FormKey, out rec));
                Assert.Same(rec, unoverriddenNpc);
                Assert.True(package.TryLookup<Npc>(topModNpc.FormKey, out rec));
                Assert.Same(rec, topModNpc);
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
                Assert.True(package.TryLookup<INpcGetter>(TestFileFormKey, out var rec));
                Assert.True(package.TryLookup<INpcGetter>(TestFileFormKey2, out rec));
                Assert.True(rec.Name.TryGet(out var name));
                Assert.Equal("A Name", name);
            }
            {
                Assert.False(package.TryLookup<Npc>(TestFileFormKey, out var rec));
                Assert.False(package.TryLookup<Npc>(TestFileFormKey2, out rec));
            }
        }
        #endregion

        #region FormLink Resolves
        [Fact]
        public void FormLink_TryResolve_NoLink()
        {
            FormLink<INpc> formLink = new FormLink<INpc>(UnusedFormKey);
            var package = new DirectModLinkCache<OblivionMod>(new OblivionMod(Utility.ModKey));
            Assert.False(formLink.TryResolve(package, out var link));
        }

        [Fact]
        public void FormLink_TryResolve_Linked()
        {
            var mod = new OblivionMod(Utility.ModKey);
            var npc = mod.Npcs.AddNew();
            var package = new DirectModLinkCache<OblivionMod>(mod);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            Assert.True(formLink.TryResolve(package, out var linkedRec));
            Assert.Same(npc, linkedRec);
        }

        [Fact]
        public void FormLink_Resolve_NoLink()
        {
            FormLink<INpc> formLink = new FormLink<INpc>(UnusedFormKey);
            var package = new DirectModLinkCache<OblivionMod>(new OblivionMod(Utility.ModKey));
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Resolve_Linked()
        {
            var mod = new OblivionMod(Utility.ModKey);
            var npc = mod.Npcs.AddNew();
            var package = new DirectModLinkCache<OblivionMod>(mod);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            Assert.Same(npc, formLink.Resolve(package));
        }
        #endregion

        #region EDIDLink Resolves
        [Fact]
        public void EDIDLink_TryResolve_NoLink()
        {
            var mod = new OblivionMod(Utility.ModKey);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var package = new DirectModLinkCache<OblivionMod>(mod);
            Assert.False(link.TryResolve(package, out var _));
        }

        [Fact]
        public void EDIDLink_TryResolve_Linked()
        {
            var mod = new OblivionMod(Utility.ModKey);
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
            var mod = new OblivionMod(Utility.ModKey);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var package = new DirectModLinkCache<OblivionMod>(mod);
            Assert.Null(link.Resolve(package));
        }

        [Fact]
        public void EDIDLink_Resolve_Linked()
        {
            var mod = new OblivionMod(Utility.ModKey);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var package = new DirectModLinkCache<OblivionMod>(mod);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.Same(effect, link.Resolve(package));
        }
        #endregion
    }
}
