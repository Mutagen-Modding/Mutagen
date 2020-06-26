using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Skyrim;
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
            WarmupAll.Init();
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
            var package = new DirectModLinkCache<SkyrimMod>(new SkyrimMod(Utility.ModKey));

            // Test query fails
            Assert.False(package.TryLookup(UnusedFormKey, out var _));
            Assert.False(package.TryLookup(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<ISkyrimMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<ISkyrimMajorRecordGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<INpcGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<INpcGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<Npc>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<Npc>(FormKey.Null, out var _));
        }

        [Fact]
        public void Direct_NoMatch()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            mod.Npcs.AddNew();
            var package = new DirectModLinkCache<SkyrimMod>(mod);

            // Test query fails
            Assert.False(package.TryLookup(UnusedFormKey, out var _));
            Assert.False(package.TryLookup(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<ISkyrimMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<ISkyrimMajorRecordGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<INpcGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<INpcGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<Npc>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<Npc>(FormKey.Null, out var _));
        }

        [Fact]
        public void Direct_Typical()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var npc1 = mod.Npcs.AddNew();
            var npc2 = mod.Npcs.AddNew();
            var package = new DirectModLinkCache<SkyrimMod>(mod);

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
                Assert.True(package.TryLookup<ISkyrimMajorRecordGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<ISkyrimMajorRecordGetter>(npc2.FormKey, out var rec));
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
            var wrapper = Oblivion.OblivionMod.CreateFromBinaryOverlay(PathToTestFile);
            var package = wrapper.CreateLinkCache();
            {
                Assert.True(package.TryLookup<Oblivion.INpcGetter>(TestFileFormKey, out var rec));
            }
            {
                Assert.False(package.TryLookup<Oblivion.Npc>(TestFileFormKey, out var rec));
            }
        }
        #endregion

        #region LoadOrder
        [Fact]
        public void LoadOrder_Empty()
        {
            var package = new LoadOrderLinkCache<SkyrimMod>(new LoadOrder<SkyrimMod>());

            // Test query fails
            Assert.False(package.TryLookup(UnusedFormKey, out var _));
            Assert.False(package.TryLookup(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<ISkyrimMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<ISkyrimMajorRecordGetter>(FormKey.Null, out var _));
        }

        [Fact]
        public void LoadOrder_NoMatch()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            mod.Npcs.AddNew();
            var loadOrder = new LoadOrder<SkyrimMod>();
            loadOrder.Add(mod);
            var package = new LoadOrderLinkCache<SkyrimMod>(loadOrder);

            // Test query fails
            Assert.False(package.TryLookup(UnusedFormKey, out var _));
            Assert.False(package.TryLookup(FormKey.Null, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryLookup<ISkyrimMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryLookup<ISkyrimMajorRecordGetter>(FormKey.Null, out var _));
        }

        [Fact]
        public void LoadOrder_Single()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var npc1 = mod.Npcs.AddNew();
            var npc2 = mod.Npcs.AddNew();
            var loadOrder = new LoadOrder<SkyrimMod>();
            loadOrder.Add(mod);
            var package = new LoadOrderLinkCache<SkyrimMod>(loadOrder);

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
                Assert.True(package.TryLookup<ISkyrimMajorRecordGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<ISkyrimMajorRecordGetter>(npc2.FormKey, out var rec));
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
            var mod1 = new SkyrimMod(Utility.ModKey);
            var mod2 = new SkyrimMod(new ModKey("Dummy2", true));
            var npc1 = mod1.Npcs.AddNew();
            var npc2 = mod2.Npcs.AddNew();
            var loadOrder = new LoadOrder<SkyrimMod>();
            loadOrder.Add(mod1);
            loadOrder.Add(mod2);
            var package = new LoadOrderLinkCache<SkyrimMod>(loadOrder);

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
                Assert.True(package.TryLookup<ISkyrimMajorRecordGetter>(npc1.FormKey, out var rec));
                Assert.Same(rec, npc1);
            }
            {
                Assert.True(package.TryLookup<ISkyrimMajorRecordGetter>(npc2.FormKey, out var rec));
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
            var mod1 = new SkyrimMod(Utility.ModKey);
            var mod2 = new SkyrimMod(new ModKey("Dummy2", true));
            var unoverriddenNpc = mod1.Npcs.AddNew();
            var overriddenNpc = mod1.Npcs.AddNew();
            var topModNpc = mod2.Npcs.AddNew();
            var overrideNpc = (Npc)overriddenNpc.DeepCopy();
            mod2.Npcs.RecordCache.Set(overrideNpc);
            var loadOrder = new LoadOrder<SkyrimMod>();
            loadOrder.Add(mod1);
            loadOrder.Add(mod2);
            var package = new LoadOrderLinkCache<SkyrimMod>(loadOrder);

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
                Assert.True(package.TryLookup<ISkyrimMajorRecordGetter>(overriddenNpc.FormKey, out var rec));
                Assert.Same(rec, overrideNpc);
                Assert.NotSame(rec, overriddenNpc);
                Assert.True(package.TryLookup<ISkyrimMajorRecordGetter>(unoverriddenNpc.FormKey, out rec));
                Assert.Same(rec, unoverriddenNpc);
                Assert.True(package.TryLookup<ISkyrimMajorRecordGetter>(topModNpc.FormKey, out rec));
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
            var wrapper = Oblivion.OblivionMod.CreateFromBinaryOverlay(PathToTestFile);
            var overrideWrapper = Oblivion.OblivionMod.CreateFromBinaryOverlay(PathToOverrideFile);
            var loadOrder = new LoadOrder<Oblivion.IOblivionModGetter>();
            loadOrder.Add(wrapper);
            loadOrder.Add(overrideWrapper);
            var package = loadOrder.CreateLinkCache();
            {
                Assert.True(package.TryLookup<Oblivion.INpcGetter>(TestFileFormKey, out var rec));
                Assert.True(package.TryLookup<Oblivion.INpcGetter>(TestFileFormKey2, out rec));
                Assert.True(rec.Name.TryGet(out var name));
                Assert.Equal("A Name", name);
            }
            {
                Assert.False(package.TryLookup<Oblivion.Npc>(TestFileFormKey, out var rec));
                Assert.False(package.TryLookup<Oblivion.Npc>(TestFileFormKey2, out rec));
            }
        }
        #endregion

        #region FormLink Resolves
        [Fact]
        public void FormLink_TryResolve_NoLink()
        {
            FormLink<INpc> formLink = new FormLink<INpc>(UnusedFormKey);
            var package = new DirectModLinkCache<SkyrimMod>(new SkyrimMod(Utility.ModKey));
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_TryResolve_Linked()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var npc = mod.Npcs.AddNew();
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            Assert.True(formLink.TryResolve(package, out var linkedRec));
            Assert.Same(npc, linkedRec);
        }

        [Fact]
        public void FormLink_TryResolve_DeepRecord_NoLink()
        {
            FormLink<IPlacedNpc> formLink = new FormLink<IPlacedNpc>(UnusedFormKey);
            var package = new DirectModLinkCache<SkyrimMod>(new SkyrimMod(Utility.ModKey));
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_TryResolve_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey());
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey());
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            FormLink<IPlacedNpc> placedFormLink = new FormLink<IPlacedNpc>(placedNpc.FormKey);
            Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
            Assert.Same(placedNpc, linkedPlacedNpc);
            FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
            Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
            Assert.Same(cell, linkedCell);
            FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
            Assert.Same(worldspace, linkedWorldspace);
        }

        [Fact]
        public void FormLink_Resolve_NoLink()
        {
            FormLink<INpc> formLink = new FormLink<INpc>(UnusedFormKey);
            var package = new DirectModLinkCache<SkyrimMod>(new SkyrimMod(Utility.ModKey));
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Resolve_Linked()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var npc = mod.Npcs.AddNew();
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            Assert.Same(npc, formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Resolve_DeepRecord_NoLink()
        {
            FormLink<IPlacedNpc> formLink = new FormLink<IPlacedNpc>(UnusedFormKey);
            var package = new DirectModLinkCache<SkyrimMod>(new SkyrimMod(Utility.ModKey));
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Resolve_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey());
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey());
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            FormLink<IPlacedNpc> placedFormLink = new FormLink<IPlacedNpc>(placedNpc.FormKey);
            Assert.Same(placedNpc, placedFormLink.Resolve(package));
            FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
            Assert.Same(cell, cellFormLink.Resolve(package));
            FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            Assert.Same(worldspace, worldspaceFormLink.Resolve(package));
        }

        [Fact]
        public void FormLink_TryResolve_MarkerInterface()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var spell = mod.Spells.AddNew();
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(spell.FormKey);
            Assert.True(formLink.TryResolve(package, out var linkedRec));
            Assert.Same(spell, linkedRec);
        }

        [Fact]
        public void FormLink_TryResolve_MarkerInterface_NoLink()
        {
            FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(UnusedFormKey);
            var package = new DirectModLinkCache<SkyrimMod>(new SkyrimMod(Utility.ModKey));
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_TryResolve_MarkerInterface_DeepRecord_NoLink()
        {
            FormLink<IPlaced> formLink = new FormLink<IPlaced>(UnusedFormKey);
            var package = new DirectModLinkCache<SkyrimMod>(new SkyrimMod(Utility.ModKey));
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_TryResolve_MarkerInterface_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey());
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey());
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            FormLink<IPlaced> placedFormLink = new FormLink<IPlaced>(placedNpc.FormKey);
            Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
            Assert.Same(placedNpc, linkedPlacedNpc);
            FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
            Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
            Assert.Same(cell, linkedCell);
            FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
            Assert.Same(worldspace, linkedWorldspace);
        }

        [Fact]
        public void FormLink_Resolve_MarkerInterface()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var spell = mod.Spells.AddNew();
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(spell.FormKey);
            Assert.Same(spell, formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Resolve_MarkerInterface_NoLink()
        {
            FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(UnusedFormKey);
            var package = new DirectModLinkCache<SkyrimMod>(new SkyrimMod(Utility.ModKey));
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Resolve_MarkerInterface_DeepRecord_NoLink()
        {
            FormLink<IPlaced> formLink = new FormLink<IPlaced>(UnusedFormKey);
            var package = new DirectModLinkCache<SkyrimMod>(new SkyrimMod(Utility.ModKey));
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Resolve_MarkerInterface_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey());
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey());
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            FormLink<IPlaced> placedFormLink = new FormLink<IPlaced>(placedNpc.FormKey);
            Assert.Same(placedNpc, placedFormLink.Resolve(package));
            FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
            Assert.Same(cell, cellFormLink.Resolve(package));
            FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            Assert.Same(worldspace, worldspaceFormLink.Resolve(package));
        }
        #endregion

        #region EDIDLink Resolves
        [Fact]
        public void EDIDLink_TryResolve_NoLink()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            Assert.False(link.TryResolve(package, out var _));
        }

        [Fact]
        public void EDIDLink_TryResolve_Linked()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.True(link.TryResolve(package, out var linkedRec));
            Assert.Same(effect, linkedRec);
        }

        [Fact]
        public void EDIDLink_Resolve_NoLink()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            Assert.Null(link.Resolve(package));
        }

        [Fact]
        public void EDIDLink_Resolve_Linked()
        {
            var mod = new SkyrimMod(Utility.ModKey);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var package = new DirectModLinkCache<SkyrimMod>(mod);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.Same(effect, link.Resolve(package));
        }
        #endregion
    }
}
