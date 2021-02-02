using FluentAssertions;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.Utility;
using System;
using System.Linq;
using System.Reactive.Disposables;
using Xunit;
#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

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

    public abstract class Linking_Abstract_Tests : IClassFixture<LinkingInit>
    {
        public static FormKey UnusedFormKey = new FormKey(Utility.PluginModKey, 123456);
        public static string UnusedEditorID = "Unused";
        public static FormKey TestFileFormKey = new FormKey(Utility.SkyrimTestMod.ModKey, 0x800);
        public static FormKey TestFileFormKey2 = new FormKey(Utility.SkyrimTestMod.ModKey, 0x801);
        public static string TestFileEditorID = "Record1";
        public static string TestFileEditorID2 = "Record2";

        public abstract IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter);
        public abstract bool ReadOnly { get; }

        protected abstract ILinkCache<ISkyrimMod> GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs = null);

        protected abstract ILinkCache<ISkyrimMod> GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs = null);

        #region Direct Mod
        [Fact]
        public void Direct_Empty()
        {
            using var disp = ConvertMod(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE), out var mod);
            var package = GetLinkCache(mod);

            // Test FormKey fails
            Assert.False(package.TryResolve(UnusedFormKey, out var _));
            Assert.False(package.TryResolve(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IObjectEffectGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IObjectEffectGetter>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IObjectEffect>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IObjectEffect>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<ObjectEffect>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<ObjectEffect>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IEffectRecord>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IEffectRecord>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IEffectRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IEffectRecordGetter>(FormKey.Null, out var _));

            // Test EditorID fails
            Assert.False(package.TryResolve(UnusedEditorID, out var _));
            Assert.False(package.TryResolve(string.Empty, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(string.Empty, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(string.Empty, out var _));
            Assert.False(package.TryResolve<IObjectEffectGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IObjectEffectGetter>(string.Empty, out var _));
            Assert.False(package.TryResolve<IObjectEffect>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IObjectEffect>(string.Empty, out var _));
            Assert.False(package.TryResolve<ObjectEffect>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<ObjectEffect>(string.Empty, out var _));
            Assert.False(package.TryResolve<IEffectRecord>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IEffectRecord>(string.Empty, out var _));
            Assert.False(package.TryResolve<IEffectRecordGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IEffectRecordGetter>(string.Empty, out var _));
        }

        [Fact]
        public void Direct_NoMatch()
        {
            var prototype = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            prototype.ObjectEffects.AddNew();
            using var disp = ConvertMod(prototype, out var mod);
            var package = GetLinkCache(mod);

            // Test FormKey fails
            Assert.False(package.TryResolve(UnusedFormKey, out var _));
            Assert.False(package.TryResolve(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IObjectEffectGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IObjectEffectGetter>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IObjectEffect>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IObjectEffect>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<ObjectEffect>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<ObjectEffect>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IEffectRecord>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IEffectRecord>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IEffectRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IEffectRecordGetter>(FormKey.Null, out var _));

            // Test EditorID fails
            Assert.False(package.TryResolve(UnusedEditorID, out var _));
            Assert.False(package.TryResolve(string.Empty, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(string.Empty, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(string.Empty, out var _));
            Assert.False(package.TryResolve<IObjectEffectGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IObjectEffectGetter>(string.Empty, out var _));
            Assert.False(package.TryResolve<IObjectEffect>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IObjectEffect>(string.Empty, out var _));
            Assert.False(package.TryResolve<ObjectEffect>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<ObjectEffect>(string.Empty, out var _));
            Assert.False(package.TryResolve<IEffectRecord>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IEffectRecord>(string.Empty, out var _));
            Assert.False(package.TryResolve<IEffectRecordGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IEffectRecordGetter>(string.Empty, out var _));
        }

        [Fact]
        public void Direct_Typical()
        {
            var prototype = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var objEffect1 = prototype.ObjectEffects.AddNew("EDID1");
            var objEffect2 = prototype.ObjectEffects.AddNew("EDID2");
            using var disp = ConvertMod(prototype, out var mod);
            var package = GetLinkCache(mod);

            // Do linked interfaces first, as this tests a specific edge case
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }

            {
                Assert.True(package.TryResolve(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            if (ReadOnly)
            {
                Assert.False(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var _));
            }
            else
            {
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
            }
        }

        [Fact]
        public void Direct_ReadOnlyMechanics()
        {
            var wrapper = SkyrimMod.CreateFromBinaryOverlay(Utility.SkyrimTestMod, SkyrimRelease.SkyrimSE);
            var package = GetLinkCache(wrapper);
            {
                Assert.True(package.TryResolve<INpcGetter>(TestFileFormKey, out var rec));
            }
            {
                Assert.True(package.TryResolve<INpcGetter>(TestFileEditorID, out var rec));
            }
            {
                Assert.False(package.TryResolve<INpc>(TestFileFormKey, out var rec));
            }
            {
                Assert.False(package.TryResolve<INpc>(TestFileEditorID, out var rec));
            }
            {
                Assert.False(package.TryResolve<Npc>(TestFileFormKey, out var rec));
            }
            {
                Assert.False(package.TryResolve<Npc>(TestFileEditorID, out var rec));
            }
        }
        #endregion

        #region LoadOrder
        [Fact]
        public void LoadOrder_Empty()
        {
            var package = new LoadOrder<ISkyrimModGetter>().ToImmutableLinkCache();

            // Test FormKey fails
            Assert.False(package.TryResolve(UnusedFormKey, out var _));
            Assert.False(package.TryResolve(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(FormKey.Null, out var _));

            // Test EditorID fails
            Assert.False(package.TryResolve(UnusedEditorID, out var _));
            Assert.False(package.TryResolve(string.Empty, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(string.Empty, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(string.Empty, out var _));
        }

        [Fact]
        public void LoadOrder_NoMatch()
        {
            var prototype = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            prototype.Npcs.AddNew();
            using var disp = ConvertMod(prototype, out var mod);
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            loadOrder.Add(mod);
            var package = GetLinkCache(loadOrder);

            // Test FormKey fails
            Assert.False(package.TryResolve(UnusedFormKey, out var _));
            Assert.False(package.TryResolve(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(FormKey.Null, out var _));

            // Test EditorID fails
            Assert.False(package.TryResolve(UnusedEditorID, out var _));
            Assert.False(package.TryResolve(string.Empty, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IMajorRecordCommonGetter>(string.Empty, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(string.Empty, out var _));
        }

        [Fact]
        public void LoadOrder_Single()
        {
            var prototype = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var objEffect1 = prototype.ObjectEffects.AddNew("EditorID1");
            var objEffect2 = prototype.ObjectEffects.AddNew("EditorID2");
            using var disp = ConvertMod(prototype, out var mod);
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            loadOrder.Add(mod);
            var package = GetLinkCache(loadOrder);

            // Test query successes

            // Do linked interfaces first, as this tests a specific edge case
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }

            {
                Assert.True(package.TryResolve(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            if (ReadOnly)
            {
                Assert.False(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var _));
            }
            else
            {
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
            }
        }

        [Fact]
        public void LoadOrder_OneInEach()
        {
            var prototype1 = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var prototype2 = new SkyrimMod(new ModKey("Dummy2", ModType.Master), SkyrimRelease.SkyrimLE);
            var objEffect1 = prototype1.ObjectEffects.AddNew("EditorID1");
            var objEffect2 = prototype2.ObjectEffects.AddNew("EditorID2");
            using var disp1 = ConvertMod(prototype1, out var mod1);
            using var disp2 = ConvertMod(prototype2, out var mod2);
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            loadOrder.Add(mod1);
            loadOrder.Add(mod2);
            var package = GetLinkCache(loadOrder);

            // Test query successes

            // Do linked interfaces first, as this tests a specific edge case
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }

            {
                Assert.True(package.TryResolve(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            }
            if (ReadOnly)
            {
                Assert.False(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var _));
            }
            else
            {
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                }
            }
        }

        [Fact]
        public void LoadOrder_Overridden()
        {
            var prototype1 = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var prototype2 = new SkyrimMod(new ModKey("Dummy2", ModType.Master), SkyrimRelease.SkyrimLE);
            var unoverriddenRec = prototype1.ObjectEffects.AddNew("EditorID1");
            var overriddenRec = prototype1.ObjectEffects.AddNew("EditorID2");
            var topModRec = prototype2.ObjectEffects.AddNew("EditorID3");
            var overrideRec = (ObjectEffect)overriddenRec.DeepCopy();
            prototype2.ObjectEffects.RecordCache.Set(overrideRec);
            using var disp1 = ConvertMod(prototype1, out var mod1);
            using var disp2 = ConvertMod(prototype2, out var mod2);
            var loadOrder = new LoadOrder<ISkyrimModGetter>
            {
                mod1,
                mod2
            };
            var package = GetLinkCache(loadOrder);

            // Test query successes

            // Do linked interfaces first, as this tests a specific edge case
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
                Assert.True(package.TryResolve<IEffectRecordGetter>(unoverriddenRec.FormKey, out rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                Assert.True(package.TryResolve<IEffectRecordGetter>(topModRec.FormKey, out rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
                Assert.True(package.TryResolve<IEffectRecordGetter>(overriddenRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
                Assert.True(package.TryResolve<IEffectRecordGetter>(unoverriddenRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                Assert.True(package.TryResolve<IEffectRecordGetter>(topModRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            }

            {
                Assert.True(package.TryResolve(overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
                Assert.True(package.TryResolve(unoverriddenRec.FormKey, out rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                Assert.True(package.TryResolve(topModRec.FormKey, out rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
                Assert.True(package.TryResolve(overriddenRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
                Assert.True(package.TryResolve(unoverriddenRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                Assert.True(package.TryResolve(topModRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            }
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(unoverriddenRec.FormKey, out rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(topModRec.FormKey, out rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(overriddenRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(unoverriddenRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(topModRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            }
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(unoverriddenRec.FormKey, out rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(topModRec.FormKey, out rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(overriddenRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(unoverriddenRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(topModRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            }
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
                Assert.True(package.TryResolve<IObjectEffectGetter>(unoverriddenRec.FormKey, out rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                Assert.True(package.TryResolve<IObjectEffectGetter>(topModRec.FormKey, out rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
                Assert.True(package.TryResolve<IObjectEffectGetter>(overriddenRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
                Assert.True(package.TryResolve<IObjectEffectGetter>(unoverriddenRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                Assert.True(package.TryResolve<IObjectEffectGetter>(topModRec.EditorID, out rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            }
            if (ReadOnly)
            {
                Assert.False(package.TryResolve<IObjectEffect>(overriddenRec.FormKey, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(unoverriddenRec.FormKey, out _));
                Assert.False(package.TryResolve<IObjectEffect>(topModRec.FormKey, out _));
                Assert.False(package.TryResolve<ObjectEffect>(overriddenRec.FormKey, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(unoverriddenRec.FormKey, out _));
                Assert.False(package.TryResolve<ObjectEffect>(topModRec.FormKey, out _));
                Assert.False(package.TryResolve<IEffectRecord>(overriddenRec.FormKey, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(unoverriddenRec.FormKey, out _));
                Assert.False(package.TryResolve<IEffectRecord>(topModRec.FormKey, out _));
                Assert.False(package.TryResolve<IObjectEffect>(overriddenRec.EditorID, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(unoverriddenRec.EditorID, out _));
                Assert.False(package.TryResolve<IObjectEffect>(topModRec.EditorID, out _));
                Assert.False(package.TryResolve<ObjectEffect>(overriddenRec.EditorID, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(unoverriddenRec.EditorID, out _));
                Assert.False(package.TryResolve<ObjectEffect>(topModRec.EditorID, out _));
                Assert.False(package.TryResolve<IEffectRecord>(overriddenRec.EditorID, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(unoverriddenRec.EditorID, out _));
                Assert.False(package.TryResolve<IEffectRecord>(topModRec.EditorID, out _));
            }
            else
            {
                {
                    Assert.True(package.TryResolve<IObjectEffect>(overriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                    Assert.True(package.TryResolve<IObjectEffect>(unoverriddenRec.FormKey, out rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                    Assert.True(package.TryResolve<IObjectEffect>(topModRec.FormKey, out rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                    Assert.True(package.TryResolve<IObjectEffect>(overriddenRec.EditorID, out rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                    Assert.True(package.TryResolve<IObjectEffect>(unoverriddenRec.EditorID, out rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                    Assert.True(package.TryResolve<IObjectEffect>(topModRec.EditorID, out rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                }
                {
                    Assert.True(package.TryResolve<ObjectEffect>(overriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                    Assert.True(package.TryResolve<ObjectEffect>(unoverriddenRec.FormKey, out rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                    Assert.True(package.TryResolve<ObjectEffect>(topModRec.FormKey, out rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                    Assert.True(package.TryResolve<ObjectEffect>(overriddenRec.EditorID, out rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                    Assert.True(package.TryResolve<ObjectEffect>(unoverriddenRec.EditorID, out rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                    Assert.True(package.TryResolve<ObjectEffect>(topModRec.EditorID, out rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                }
                {
                    Assert.True(package.TryResolve<IEffectRecord>(overriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                    Assert.True(package.TryResolve<IEffectRecord>(unoverriddenRec.FormKey, out rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                    Assert.True(package.TryResolve<IEffectRecord>(topModRec.FormKey, out rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                    Assert.True(package.TryResolve<IEffectRecord>(overriddenRec.EditorID, out rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                    Assert.True(package.TryResolve<IEffectRecord>(unoverriddenRec.EditorID, out rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                    Assert.True(package.TryResolve<IEffectRecord>(topModRec.EditorID, out rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                }
            }
        }

        [Fact]
        public void LoadOrder_ReadOnlyMechanics()
        {
            var wrapper = SkyrimMod.CreateFromBinaryOverlay(Utility.SkyrimTestMod, SkyrimRelease.SkyrimSE);
            var overrideWrapper = SkyrimMod.CreateFromBinaryOverlay(Utility.SkyrimOverrideMod, SkyrimRelease.SkyrimSE);
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            loadOrder.Add(wrapper);
            loadOrder.Add(overrideWrapper);
            var package = GetLinkCache(loadOrder);
            {
                Assert.True(package.TryResolve<INpcGetter>(TestFileFormKey, out var rec));
                Assert.True(package.TryResolve<INpcGetter>(TestFileFormKey2, out rec));
                Assert.True(rec.Name.TryGet(out var name));
                Assert.Equal("A Name", name.String);
            }
            {
                Assert.False(package.TryResolve<INpc>(TestFileFormKey, out var rec));
                Assert.False(package.TryResolve<INpc>(TestFileFormKey2, out rec));
            }
            {
                Assert.False(package.TryResolve<Npc>(TestFileFormKey, out var rec));
                Assert.False(package.TryResolve<Npc>(TestFileFormKey2, out rec));
            }
        }
        #endregion

        #region Direct FormLink Resolves
        [Fact]
        public void FormLink_Direct_TryResolve_NoLink()
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_Direct_TryResolve_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var package = GetLinkCache(mod);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            Assert.True(formLink.TryResolve(package, out var linkedRec));
            Assert.Same(npc, linkedRec);
        }

        [Fact]
        public void FormLink_Direct_TryResolve_DeepRecord_NoLink()
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_Direct_TryResolve_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = GetLinkCache(mod);
            var placedFormLink = new FormLink<IPlacedNpcGetter>(placedNpc.FormKey);
            Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
            Assert.Same(placedNpc, linkedPlacedNpc);
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
            Assert.Same(cell, linkedCell);
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
            Assert.Same(worldspace, linkedWorldspace);
        }

        [Fact]
        public void FormLink_Direct_Resolve_NoLink()
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Direct_Resolve_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var package = GetLinkCache(mod);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            Assert.Same(npc, formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Direct_Resolve_DeepRecord_NoLink()
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Direct_Resolve_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = GetLinkCache(mod);
            var placedFormLink = new FormLink<IPlacedNpcGetter>(placedNpc.FormKey);
            Assert.Same(placedNpc, placedFormLink.Resolve(package));
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            Assert.Same(cell, cellFormLink.Resolve(package));
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            Assert.Same(worldspace, worldspaceFormLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Direct_TryResolve_MarkerInterface()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var package = GetLinkCache(mod);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            Assert.True(formLink.TryResolve(package, out var linkedRec));
            Assert.Same(spell, linkedRec);
        }

        [Fact]
        public void FormLink_Direct_TryResolve_MarkerInterface_NoLink()
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_Direct_TryResolve_MarkerInterface_DeepRecord_NoLink()
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_Direct_TryResolve_MarkerInterface_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = GetLinkCache(mod);
            var placedFormLink = new FormLink<IPlacedGetter>(placedNpc.FormKey);
            Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
            Assert.Same(placedNpc, linkedPlacedNpc);
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
            Assert.Same(cell, linkedCell);
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
            Assert.Same(worldspace, linkedWorldspace);
        }

        [Fact]
        public void FormLink_Direct_Resolve_MarkerInterface()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var package = GetLinkCache(mod);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            Assert.Same(spell, formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Direct_Resolve_MarkerInterface_NoLink()
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Direct_Resolve_MarkerInterface_DeepRecord_NoLink()
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_Direct_Resolve_MarkerInterface_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = GetLinkCache(mod);
            var placedFormLink = new FormLink<IPlacedGetter>(placedNpc.FormKey);
            Assert.Same(placedNpc, placedFormLink.Resolve(package));
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            Assert.Same(cell, cellFormLink.Resolve(package));
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            Assert.Same(worldspace, worldspaceFormLink.Resolve(package));
        }
        #endregion

        #region Load Order FormLink Resolves
        LoadOrder<ISkyrimModGetter> TypicalLoadOrder()
        {
            return new LoadOrder<ISkyrimModGetter>()
            {
                new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE),
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
        }

        [Fact]
        public void FormLink_LoadOrder_TryResolve_NoLink()
        {
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            FormLink<INpc> formLink = new FormLink<INpc>(UnusedFormKey);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_LoadOrder_TryResolve_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var package = GetLinkCache(loadOrder);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            Assert.True(formLink.TryResolve(package, out var linkedRec));
            Assert.Same(npc, linkedRec);
        }

        [Fact]
        public void FormLink_LoadOrder_TryResolve_DeepRecord_NoLink()
        {
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            FormLink<IPlacedNpc> formLink = new FormLink<IPlacedNpc>(UnusedFormKey);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_LoadOrder_TryResolve_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var package = GetLinkCache(loadOrder);
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
        public void FormLink_LoadOrder_Resolve_NoLink()
        {
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            FormLink<INpc> formLink = new FormLink<INpc>(UnusedFormKey);
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_LoadOrder_Resolve_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var package = GetLinkCache(loadOrder);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            Assert.Same(npc, formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_LoadOrder_Resolve_DeepRecord_NoLink()
        {
            FormLink<IPlacedNpc> formLink = new FormLink<IPlacedNpc>(UnusedFormKey);
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_LoadOrder_Resolve_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var package = GetLinkCache(loadOrder);
            FormLink<IPlacedNpc> placedFormLink = new FormLink<IPlacedNpc>(placedNpc.FormKey);
            Assert.Same(placedNpc, placedFormLink.Resolve(package));
            FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
            Assert.Same(cell, cellFormLink.Resolve(package));
            FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            Assert.Same(worldspace, worldspaceFormLink.Resolve(package));
        }

        [Fact]
        public void FormLink_LoadOrder_TryResolve_MarkerInterface()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var package = GetLinkCache(loadOrder);
            FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(spell.FormKey);
            Assert.True(formLink.TryResolve(package, out var linkedRec));
            Assert.Same(spell, linkedRec);
        }

        [Fact]
        public void FormLink_LoadOrder_TryResolve_MarkerInterface_NoLink()
        {
            FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(UnusedFormKey);
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_LoadOrder_TryResolve_MarkerInterface_DeepRecord_NoLink()
        {
            FormLink<IPlaced> formLink = new FormLink<IPlaced>(UnusedFormKey);
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_LoadOrder_TryResolve_MarkerInterface_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var package = GetLinkCache(loadOrder);
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
        public void FormLink_LoadOrder_Resolve_MarkerInterface()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var package = GetLinkCache(loadOrder);
            FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(spell.FormKey);
            Assert.Same(spell, formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_LoadOrder_Resolve_MarkerInterface_NoLink()
        {
            FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(UnusedFormKey);
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_LoadOrder_Resolve_MarkerInterface_DeepRecord_NoLink()
        {
            FormLink<IPlaced> formLink = new FormLink<IPlaced>(UnusedFormKey);
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            Assert.Null(formLink.Resolve(package));
        }

        [Fact]
        public void FormLink_LoadOrder_Resolve_MarkerInterface_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var package = GetLinkCache(loadOrder);
            FormLink<IPlaced> placedFormLink = new FormLink<IPlaced>(placedNpc.FormKey);
            Assert.Same(placedNpc, placedFormLink.Resolve(package));
            FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
            Assert.Same(cell, cellFormLink.Resolve(package));
            FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            Assert.Same(worldspace, worldspaceFormLink.Resolve(package));
        }
        #endregion

        #region Direct FormLink Context Resolves
        [Fact]
        public void FormLink_Direct_TryResolveContext_NoLink()
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.False(formLink.TryResolveContext<ISkyrimMod, INpc>(package, out var _));
        }

        [Fact]
        public void FormLink_Direct_TryResolveContext_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var package = GetLinkCache(mod);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            Assert.True(formLink.TryResolveContext<ISkyrimMod, INpc>(package, out var linkedRec));
            linkedRec.Record.Should().BeSameAs(npc);
            linkedRec.ModKey.Should().Be(Utility.PluginModKey);
            linkedRec.Parent.Should().BeNull();
        }

        [Fact]
        public void FormLink_Direct_TryResolveContext_DeepRecord_NoLink()
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.False(formLink.TryResolveContext<ISkyrimMod, IPlacedNpc>(package, out var _));
        }

        [Fact]
        public void FormLink_Direct_TryResolveContext_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = GetLinkCache(mod);
            var placedFormLink = new FormLink<IPlacedNpcGetter>(placedNpc.FormKey);
            Assert.True(placedFormLink.TryResolveContext<ISkyrimMod, IPlacedNpc>(package, out var linkedPlacedNpc));
            linkedPlacedNpc.Record.Should().BeSameAs(placedNpc);
            linkedPlacedNpc.ModKey.Should().Be(Utility.PluginModKey);
            linkedPlacedNpc.Parent.Record.Should().Be(cell);
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            Assert.True(cellFormLink.TryResolveContext<ISkyrimMod, ICell>(package, out var linkedCell));
            linkedCell.Record.Should().BeSameAs(cell);
            linkedCell.ModKey.Should().Be(Utility.PluginModKey);
            linkedCell.Parent.Record.Should().Be(subBlock);
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            Assert.True(worldspaceFormLink.TryResolveContext<ISkyrimMod, IWorldspace>(package, out var linkedWorldspace));
            linkedWorldspace.Record.Should().BeSameAs(worldspace);
            linkedWorldspace.ModKey.Should().Be(Utility.PluginModKey);
            linkedWorldspace.Parent.Should().BeNull();
        }

        [Fact]
        public void FormLink_Direct_ResolveContext_NoLink()
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.Null(formLink.ResolveContext<ISkyrimMod, INpc>(package));
        }

        [Fact]
        public void FormLink_Direct_ResolveContext_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var package = GetLinkCache(mod);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolvedNpc = formLink.ResolveContext<ISkyrimMod, INpc>(package);
            resolvedNpc.Record.Should().BeSameAs(npc);
            resolvedNpc.ModKey.Should().Be(Utility.PluginModKey);
            resolvedNpc.Parent.Should().BeNull();
        }

        [Fact]
        public void FormLink_Direct_ResolveContext_DeepRecord_NoLink()
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.Null(formLink.ResolveContext<ISkyrimMod, IPlacedNpc>(package));
        }

        [Fact]
        public void FormLink_Direct_ResolveContext_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = GetLinkCache(mod);
            var placedFormLink = new FormLink<IPlacedNpc>(placedNpc.FormKey);
            var linkedPlacedNpc = placedFormLink.ResolveContext<ISkyrimMod, IPlacedNpc>(package);
            linkedPlacedNpc.Record.Should().BeSameAs(placedNpc);
            linkedPlacedNpc.ModKey.Should().Be(Utility.PluginModKey);
            linkedPlacedNpc.Parent.Record.Should().BeSameAs(cell);
            var cellFormLink = new FormLink<ICell>(cell.FormKey);
            var linkedCell = cellFormLink.ResolveContext<ISkyrimMod, ICell>(package);
            linkedCell.Record.Should().BeSameAs(cell);
            linkedCell.ModKey.Should().Be(Utility.PluginModKey);
            linkedCell.Parent.Record.Should().BeSameAs(subBlock);
            var worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            var linkedWorldspace = worldspaceFormLink.ResolveContext<ISkyrimMod, IWorldspace>(package);
            linkedWorldspace.Record.Should().BeSameAs(worldspace);
            linkedWorldspace.ModKey.Should().Be(Utility.PluginModKey);
            linkedWorldspace.Parent.Should().BeNull();
        }

        [Fact]
        public void FormLink_Direct_TryResolveContext_MarkerInterface()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var package = GetLinkCache(mod);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            Assert.True(formLink.TryResolveContext<ISkyrimMod, IEffectRecord>(package, out var linkedRec));
            linkedRec.Record.Should().BeSameAs(spell);
            linkedRec.ModKey.Should().Be(Utility.PluginModKey);
            linkedRec.Parent.Should().BeNull();
        }

        [Fact]
        public void FormLink_Direct_TryResolveContext_MarkerInterface_NoLink()
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_Direct_TryResolveContext_MarkerInterface_DeepRecord_NoLink()
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Fact]
        public void FormLink_Direct_TryResolveContext_MarkerInterface_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = GetLinkCache(mod);
            var placedFormLink = new FormLink<IPlacedGetter>(placedNpc.FormKey);
            Assert.True(placedFormLink.TryResolveContext<ISkyrimMod, IPlaced>(package, out var linkedPlacedNpc));
            linkedPlacedNpc.Record.Should().BeSameAs(placedNpc);
            linkedPlacedNpc.ModKey.Should().Be(Utility.PluginModKey);
            linkedPlacedNpc.Parent.Record.Should().BeSameAs(cell);
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            Assert.True(cellFormLink.TryResolveContext<ISkyrimMod, ICell>(package, out var linkedCell));
            linkedCell.Record.Should().BeSameAs(cell);
            linkedCell.ModKey.Should().Be(Utility.PluginModKey);
            linkedCell.Parent.Record.Should().BeSameAs(subBlock);
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            Assert.True(worldspaceFormLink.TryResolveContext<ISkyrimMod, IWorldspace>(package, out var linkedWorldspace));
            linkedWorldspace.Record.Should().BeSameAs(worldspace);
            linkedWorldspace.ModKey.Should().Be(Utility.PluginModKey);
            linkedWorldspace.Parent.Should().BeNull();
        }

        [Fact]
        public void FormLink_Direct_ResolveContext_MarkerInterface()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var package = GetLinkCache(mod);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            var resolved = formLink.ResolveContext<ISkyrimMod, IEffectRecord>(package);
            resolved.Record.Should().BeSameAs(spell);
            resolved.ModKey.Should().Be(Utility.PluginModKey);
            resolved.Parent.Should().BeNull();
        }

        [Fact]
        public void FormLink_Direct_ResolveContext_MarkerInterface_NoLink()
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            var resolved = formLink.ResolveContext<ISkyrimMod, IEffectRecord>(package);
            Assert.Null(resolved);
        }

        [Fact]
        public void FormLink_Direct_ResolveContext_MarkerInterface_DeepRecord_NoLink()
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            var resolved = formLink.ResolveContext<ISkyrimMod, IPlaced>(package);
            Assert.Null(resolved);
        }

        [Fact]
        public void FormLink_Direct_ResolveContext_MarkerInterface_DeepRecord_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var package = GetLinkCache(mod);
            var placedFormLink = new FormLink<IPlacedGetter>(placedNpc.FormKey);
            var resolvedPlaced = placedFormLink.ResolveContext<ISkyrimMod, IPlaced>(package);
            resolvedPlaced.Record.Should().BeSameAs(placedNpc);
            resolvedPlaced.ModKey.Should().Be(Utility.PluginModKey);
            resolvedPlaced.Parent.Record.Should().BeSameAs(cell);
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            var resolvedCell = cellFormLink.ResolveContext<ISkyrimMod, ICell>(package);
            resolvedCell.Record.Should().BeSameAs(cell);
            resolvedCell.ModKey.Should().Be(Utility.PluginModKey);
            resolvedCell.Parent.Record.Should().BeSameAs(subBlock);
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            var resolvedWorldspace = worldspaceFormLink.ResolveContext<ISkyrimMod, IWorldspace>(package);
            resolvedWorldspace.Record.Should().BeSameAs(worldspace);
            resolvedWorldspace.ModKey.Should().Be(Utility.PluginModKey);
            resolvedWorldspace.Parent.Should().BeNull();
        }
        #endregion

        #region FormLink Direct ResolveAll
        [Fact]
        public void FormLink_Direct_ResolveAll_Empty()
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            formLink.ResolveAll(package).Should().BeEmpty();
        }

        [Fact]
        public void FormLink_Direct_ResolveAll_Typed_Empty()
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            formLink.ResolveAll<IPlacedNpcGetter>(package).Should().BeEmpty();
        }

        [Fact]
        public void FormLink_Direct_ResolveAll_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var package = GetLinkCache(mod);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            var resolved = formLink.ResolveAll(package).ToArray();
            resolved.Should().HaveCount(1);
            resolved.First().Should().BeSameAs(npc);
        }
        #endregion

        #region FormLink LoadOrder ResolveAll
        [Fact]
        public void FormLink_LoadOrder_ResolveAll_Empty()
        {
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            FormLink<INpcGetter> formLink = new FormLink<INpcGetter>(UnusedFormKey);
            formLink.ResolveAll(package).Should().BeEmpty();
        }

        [Fact]
        public void FormLink_LoadOrder_ResolveAll_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var package = GetLinkCache(loadOrder);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAll(package).ToArray();
            resolved.Should().HaveCount(1);
            resolved.First().Should().BeSameAs(npc);
        }

        [Fact]
        public void FormLink_LoadOrder_ResolveAll_MultipleLinks()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(Utility.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var package = GetLinkCache(loadOrder);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAll(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Should().BeSameAs(npcOverride);
            resolved.Last().Should().BeSameAs(npc);
        }

        [Fact]
        public void FormLink_LoadOrder_ResolveAll_DoubleQuery()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(Utility.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var package = GetLinkCache(loadOrder);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAll(package).ToArray();
            resolved = formLink.ResolveAll(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Should().BeSameAs(npcOverride);
            resolved.Last().Should().BeSameAs(npc);
        }

        [Fact]
        public void FormLink_LoadOrder_ResolveAll_UnrelatedNotIncluded()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var unrelatedNpc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(Utility.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var package = GetLinkCache(loadOrder);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAll(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Should().BeSameAs(npcOverride);
            resolved.Last().Should().BeSameAs(npc);
        }

        [Fact]
        public void FormLink_LoadOrder_ResolveAll_SeparateQueries()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var unrelatedNpc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(Utility.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var package = GetLinkCache(loadOrder);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAll(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Should().BeSameAs(npcOverride);
            resolved.Last().Should().BeSameAs(npc);
            formLink = new FormLink<INpcGetter>(unrelatedNpc.FormKey);
            resolved = formLink.ResolveAll(package).ToArray();
            resolved.Should().HaveCount(1);
            resolved.First().Should().BeSameAs(unrelatedNpc);
        }
        #endregion

        #region FormLink Direct ResolveAllContexts
        [Fact]
        public void FormLink_Direct_ResolveAllContexts_Empty()
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            formLink.ResolveAllContexts<ISkyrimMod, IEffectRecord>(package).Should().BeEmpty();
        }

        [Fact]
        public void FormLink_Direct_ResolveAllContexts_Typed_Empty()
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var package = GetLinkCache(new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE));
            formLink.ResolveAllContexts<ISkyrimMod, IPlacedNpc, IPlacedNpcGetter>(package).Should().BeEmpty();
        }

        [Fact]
        public void FormLink_Direct_ResolveAllContexts_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var package = GetLinkCache(mod);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAllContexts<ISkyrimMod, INpc>(package).ToArray();
            resolved.Should().HaveCount(1);
            resolved.First().Record.Should().BeSameAs(npc);
            resolved.First().ModKey.Should().Be(Utility.PluginModKey);
            resolved.First().Parent.Should().BeNull();
        }
        #endregion

        #region FormLink LoadOrder ResolveAllContexts
        [Fact]
        public void FormLink_LoadOrder_ResolveAllContexts_Empty()
        {
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            formLink.ResolveAllContexts<ISkyrimMod, INpc>(package).Should().BeEmpty();
        }

        [Fact]
        public void FormLink_LoadOrder_ResolveAllContexts_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var package = GetLinkCache(loadOrder);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAllContexts<ISkyrimMod, INpc>(package).ToArray();
            resolved.Should().HaveCount(1);
            resolved.First().Record.Should().BeSameAs(npc);
            resolved.First().ModKey.Should().Be(Utility.PluginModKey);
            resolved.First().Parent.Should().BeNull();
        }

        [Fact]
        public void FormLink_LoadOrder_ResolveAllContexts_MultipleLinks()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(Utility.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var package = GetLinkCache(loadOrder);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAllContexts<ISkyrimMod, INpc>(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(Utility.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(Utility.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
        }

        [Fact]
        public void FormLink_LoadOrder_ResolveAllContexts_DoubleQuery()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(Utility.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var package = GetLinkCache(loadOrder);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAllContexts<ISkyrimMod, INpc>(package).ToArray();
            resolved = formLink.ResolveAllContexts<ISkyrimMod, INpc>(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(Utility.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(Utility.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
        }

        [Fact]
        public void FormLink_LoadOrder_ResolveAllContexts_UnrelatedNotIncluded()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var unrelatedNpc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(Utility.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var package = GetLinkCache(loadOrder);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAllContexts<ISkyrimMod, INpc>(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(Utility.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(Utility.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
        }

        [Fact]
        public void FormLink_LoadOrder_ResolveAllContexts_SeparateQueries()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var unrelatedNpc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(Utility.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var package = GetLinkCache(loadOrder);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAllContexts<ISkyrimMod, INpc>(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(Utility.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(Utility.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
            formLink = new FormLink<INpcGetter>(unrelatedNpc.FormKey);
            resolved = formLink.ResolveAllContexts<ISkyrimMod, INpc>(package).ToArray();
            resolved.Should().HaveCount(1);
            resolved.First().Record.Should().BeSameAs(unrelatedNpc);
            resolved.First().ModKey.Should().Be(Utility.PluginModKey);
            resolved.First().Parent.Should().BeNull();
        }
        #endregion

        #region EDIDLink Resolves
        [Fact]
        public void EDIDLink_TryResolve_NoLink()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var package = GetLinkCache(mod);
            Assert.False(link.TryResolve(package, out var _));
        }

        [Fact]
        public void EDIDLink_TryResolve_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var package = GetLinkCache(mod);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.True(link.TryResolve(package, out var linkedRec));
            Assert.Same(effect, linkedRec);
        }

        [Fact]
        public void EDIDLink_Resolve_NoLink()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var package = GetLinkCache(mod);
            Assert.Null(link.TryResolve(package).Value);
        }

        [Fact]
        public void EDIDLink_Resolve_Linked()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var package = GetLinkCache(mod);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.Same(effect, link.TryResolve(package).Value);
        }
        #endregion

        #region Subtype Linking
        [Fact]
        public void SubtypeLinking_Typical()
        {
            var prototype = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimLE);
            var armor = prototype.Armors.AddNew();
            var llist = prototype.LeveledItems.AddNew();
            llist.Entries = new ExtendedList<LeveledItemEntry>()
            {
                new LeveledItemEntry()
                {
                    Data = new LeveledItemEntryData()
                    {
                        Reference = armor.FormKey
                    }
                }
            };
            using var disp = ConvertMod(prototype, out var mod);
            var package = GetLinkCache(mod);
            Assert.True(mod.LeveledItems.First().Entries[0].Data.Reference.TryResolve(package, out IArmorGetter armorGetterLink));
        }
        #endregion
    }

    public class Linking_ImmutableDirect_Tests : Linking_Abstract_Tests
    {
        public override bool ReadOnly => false;

        public override IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter)
        {
            getter = mod;
            return Disposable.Empty;
        }

        protected override ILinkCache<ISkyrimMod> GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
        {
            return new ImmutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter, prefs);
        }

        protected override ILinkCache<ISkyrimMod> GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
        {
            return loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(prefs);
        }
    }

    public class Linking_ImmutableOverlay_Tests : Linking_Abstract_Tests
    {
        public override bool ReadOnly => true;

        public override IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter)
        {
            return ConvertModToOverlay(mod, out getter);
        }

        public static IDisposable ConvertModToOverlay(SkyrimMod mod, out ISkyrimModGetter getter)
        {
            var tempFile = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            var path = new ModPath(mod.ModKey, tempFile.File.Path);
            mod.WriteToBinaryParallel(
                path,
                new BinaryWriteParameters()
                {
                    ModKey = BinaryWriteParameters.ModKeyOption.NoCheck,
                });
            var overlay = SkyrimMod.CreateFromBinaryOverlay(path, SkyrimRelease.SkyrimLE);
            getter = overlay;
            return Disposable.Create(() =>
            {
                overlay.Dispose();
                tempFile.Dispose();
            });
        }

        protected override ILinkCache<ISkyrimMod> GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
        {
            return new ImmutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter, prefs);
        }

        protected override ILinkCache<ISkyrimMod> GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
        {
            return loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(prefs);
        }
    }

    public class Linking_MutableDirect_Tests : Linking_Abstract_Tests
    {
        public override bool ReadOnly => false;

        public override IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter)
        {
            getter = mod;
            return Disposable.Empty;
        }

        protected override ILinkCache<ISkyrimMod> GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
        {
            return new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter, prefs);
        }

        protected override ILinkCache<ISkyrimMod> GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
        {
            return new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(), prefs);
        }
    }

    public class Linking_MutableOverlay_Tests : Linking_Abstract_Tests
    {
        public override bool ReadOnly => true;

        public override IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter)
        {
            var tempFile = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            var path = new ModPath(mod.ModKey, tempFile.File.Path);
            mod.WriteToBinaryParallel(
                path,
                new BinaryWriteParameters()
                {
                    ModKey = BinaryWriteParameters.ModKeyOption.NoCheck,
                });
            var overlay = SkyrimMod.CreateFromBinaryOverlay(path, SkyrimRelease.SkyrimLE);
            getter = overlay;
            return Disposable.Create(() =>
            {
                overlay.Dispose();
                tempFile.Dispose();
            });
        }

        protected override ILinkCache<ISkyrimMod> GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
        {
            return new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter, prefs);
        }

        protected override ILinkCache<ISkyrimMod> GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
        {
            return new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(), prefs);
        }
    }
}
