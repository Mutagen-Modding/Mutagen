using FluentAssertions;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Cache.Implementations;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.Utility;
using System;
using System.Linq;
using System.Reactive.Disposables;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Xunit;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache
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

    public abstract class Linking_Abstract_Tests : IClassFixture<LinkingInit>, IClassFixture<LoquiUse>
    {
        public enum LinkCacheTestTypes
        {
            Identifiers,
            WholeRecord
        }

        public enum LinkCacheStyle
        {
            HasCaching,
            OnlyDirect
        }

        public static FormKey UnusedFormKey = new FormKey(TestConstants.PluginModKey, 123456);
        public static string UnusedEditorID = "Unused";
        public static FormKey TestFileFormKey = new FormKey(TestPathing.SkyrimTestMod.ModKey, 0x800);
        public static FormKey TestFileFormKey2 = new FormKey(TestPathing.SkyrimTestMod.ModKey, 0x801);
        public static string TestFileEditorID = "Record1";
        public static string TestFileEditorID2 = "Record2";

        public abstract IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter);
        public abstract bool ReadOnly { get; }

        protected abstract (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs);

        protected abstract (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs);

        protected LinkCachePreferences GetPrefs(LinkCacheTestTypes type) => type switch
        {
            LinkCacheTestTypes.Identifiers => LinkCachePreferences.OnlyIdentifiers(),
            LinkCacheTestTypes.WholeRecord => LinkCachePreferences.WholeRecord(),
            _ => throw new NotImplementedException()
        };

        protected (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCacheTestTypes type) => GetLinkCache(modGetter, GetPrefs(type));

        protected (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCacheTestTypes type) => GetLinkCache(loadOrder, GetPrefs(type));

        protected void WrapPotentialThrow(LinkCacheTestTypes cacheType, LinkCacheStyle style, Action a)
        {
            switch (cacheType)
            {
                case LinkCacheTestTypes.Identifiers when style != LinkCacheStyle.OnlyDirect:
                    Assert.Throws<ArgumentException>(a);
                    break;
                default:
                    a();
                    break;
            }
        }

        #region Direct Mod
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void Direct_Empty(LinkCacheTestTypes cacheType)
        {
            using var disp = ConvertMod(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);

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

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void Direct_NoMatch(LinkCacheTestTypes cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            prototype.ObjectEffects.AddNew();
            using var disp = ConvertMod(prototype, out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);

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

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void Direct_Typical(LinkCacheTestTypes cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var objEffect1 = prototype.ObjectEffects.AddNew("EDID1");
            var objEffect2 = prototype.ObjectEffects.AddNew("EDID2");
            using var disp = ConvertMod(prototype, out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);

            // Do linked interfaces first, as this tests a specific edge case
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            if (ReadOnly)
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var _));
                });

                Assert.False(package.TryResolve<ObjectEffect>(TestConstants.UnusedForm, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(TestConstants.UnusedForm, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(TestConstants.UnusedForm, out var _));
                Assert.False(package.TryResolve<ObjectEffect>(TestConstants.UnusedEdid, out var _));
                Assert.False(package.TryResolve<IObjectEffect>(TestConstants.UnusedEdid, out var _));
                Assert.False(package.TryResolve<IEffectRecord>(TestConstants.UnusedEdid, out var _));
            }
            else
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
            }
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void Direct_ReadOnlyMechanics(LinkCacheTestTypes cacheType)
        {
            var wrapper = SkyrimMod.CreateFromBinaryOverlay(TestPathing.SkyrimTestMod, SkyrimRelease.SkyrimSE);
            var (style, package) = GetLinkCache(wrapper, cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<INpcGetter>(TestFileFormKey, out var rec));
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<INpcGetter>(TestFileEditorID, out var rec));
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(package.TryResolve<INpc>(TestFileFormKey, out var rec));
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(package.TryResolve<INpc>(TestFileEditorID, out var rec));
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(package.TryResolve<Npc>(TestFileFormKey, out var rec));
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(package.TryResolve<Npc>(TestFileEditorID, out var rec));
            });
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

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void LoadOrder_NoMatch(LinkCacheTestTypes cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            prototype.Npcs.AddNew();
            using var disp = ConvertMod(prototype, out var mod);
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            loadOrder.Add(mod);
            var (style, package) = GetLinkCache(loadOrder, cacheType);

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

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void LoadOrder_Single(LinkCacheTestTypes cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var objEffect1 = prototype.ObjectEffects.AddNew("EditorID1");
            var objEffect2 = prototype.ObjectEffects.AddNew("EditorID2");
            using var disp = ConvertMod(prototype, out var mod);
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            loadOrder.Add(mod);
            var (style, package) = GetLinkCache(loadOrder, cacheType);

            // Test query successes

            // Do linked interfaces first, as this tests a specific edge case
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });

            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });

            if (ReadOnly)
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var _));
                });
            }
            else
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
            }
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void LoadOrder_OneInEach(LinkCacheTestTypes cacheType)
        {
            var prototype1 = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var prototype2 = new SkyrimMod(new ModKey("Dummy2", ModType.Master), SkyrimRelease.SkyrimLE);
            var objEffect1 = prototype1.ObjectEffects.AddNew("EditorID1");
            var objEffect2 = prototype2.ObjectEffects.AddNew("EditorID2");
            using var disp1 = ConvertMod(prototype1, out var mod1);
            using var disp2 = ConvertMod(prototype2, out var mod2);
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            loadOrder.Add(mod1);
            loadOrder.Add(mod2);
            var (style, package) = GetLinkCache(loadOrder, cacheType);

            // Test query successes

            // Do linked interfaces first, as this tests a specific edge case
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            if (ReadOnly)
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var _));
                });
            }
            else
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
            }
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void LoadOrder_Overridden(LinkCacheTestTypes cacheType)
        {
            var prototype1 = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
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
            var (style, package) = GetLinkCache(loadOrder, cacheType);

            // Test query successes

            // Do linked interfaces first, as this tests a specific edge case
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(unoverriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(topModRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(overriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(unoverriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(topModRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });

            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(unoverriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(topModRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(overriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(unoverriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(topModRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(unoverriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(topModRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(overriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(unoverriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(topModRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(unoverriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(topModRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(overriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(unoverriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(topModRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });

            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(unoverriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(topModRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(overriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(unoverriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(topModRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });

            if (ReadOnly)
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(overriddenRec.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(unoverriddenRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(topModRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(overriddenRec.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(unoverriddenRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(topModRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(overriddenRec.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(unoverriddenRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(topModRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(overriddenRec.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(unoverriddenRec.EditorID, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(topModRec.EditorID, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(overriddenRec.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(unoverriddenRec.EditorID, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(topModRec.EditorID, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(overriddenRec.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(unoverriddenRec.EditorID, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(topModRec.EditorID, out _));
                });
            }
            else
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(overriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(unoverriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(topModRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(overriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(unoverriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(topModRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(overriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(unoverriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(topModRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(overriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(unoverriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(topModRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });

                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(overriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(unoverriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(topModRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(overriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(unoverriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(topModRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });
            }
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void LoadOrder_OriginatingTarget(LinkCacheTestTypes cacheType)
        {
            var prototype1 = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var prototype2 = new SkyrimMod(new ModKey("Dummy2", ModType.Master), SkyrimRelease.SkyrimLE);
            var overriddenRec = prototype1.ObjectEffects.AddNew("EditorID1");
            var overrideRec = overriddenRec.DeepCopy();
            overrideRec.EditorID = "EditorID1";
            prototype2.ObjectEffects.RecordCache.Set(overrideRec);
            using var disp1 = ConvertMod(prototype1, out var mod1);
            using var disp2 = ConvertMod(prototype2, out var mod2);
            var loadOrder = new LoadOrder<ISkyrimModGetter>
            {
                mod1,
                mod2
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);

            // Test query successes

            // Do linked interfaces first, as this tests a specific edge case
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IEffectRecordGetter>(overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                rec.EditorID.Should().Be(overriddenRec.EditorID);
            });

            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                rec.EditorID.Should().Be(overriddenRec.EditorID);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordCommonGetter>(overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                rec.EditorID.Should().Be(overriddenRec.EditorID);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<ISkyrimMajorRecordGetter>(overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                rec.EditorID.Should().Be(overriddenRec.EditorID);
            });

            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectEffectGetter>(overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                rec.EditorID.Should().Be(overriddenRec.EditorID);
            });

            if (ReadOnly)
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IObjectEffect>(overriddenRec.FormKey, out var _, ResolveTarget.Origin));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<ObjectEffect>(overriddenRec.FormKey, out var _, ResolveTarget.Origin));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(package.TryResolve<IEffectRecord>(overriddenRec.FormKey, out var _, ResolveTarget.Origin));
                });
            }
            else
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IObjectEffect>(overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                    rec.EditorID.Should().Be(overriddenRec.EditorID);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<ObjectEffect>(overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                    rec.EditorID.Should().Be(overriddenRec.EditorID);
                });

                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<IEffectRecord>(overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                    rec.EditorID.Should().Be(overriddenRec.EditorID);
                });
            }
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void LoadOrder_ReadOnlyMechanics(LinkCacheTestTypes cacheType)
        {
            var wrapper = SkyrimMod.CreateFromBinaryOverlay(TestPathing.SkyrimTestMod, SkyrimRelease.SkyrimSE);
            var overrideWrapper = SkyrimMod.CreateFromBinaryOverlay(TestPathing.SkyrimOverrideMod, SkyrimRelease.SkyrimSE);
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            loadOrder.Add(wrapper);
            loadOrder.Add(overrideWrapper);
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<INpcGetter>(TestFileFormKey, out var rec));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(package.TryResolve<INpcGetter>(TestFileFormKey2, out var rec));
                    Assert.True(rec.Name.TryGet(out var name));
                    Assert.Equal("A Name", name.String);
                });
            }
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(package.TryResolve<INpc>(TestFileFormKey, out var rec));
                Assert.False(package.TryResolve<INpc>(TestFileFormKey2, out rec));
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(package.TryResolve<Npc>(TestFileFormKey, out var rec));
                Assert.False(package.TryResolve<Npc>(TestFileFormKey2, out rec));
            });
        }
        #endregion

        #region Direct FormLink Resolves
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolve_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolve_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(formLink.TryResolve(package, out var linkedRec));
                Assert.Same(npc, linkedRec);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolve_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolve_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedNpcGetter>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
                Assert.Same(placedNpc, linkedPlacedNpc);
            });
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
                Assert.Same(cell, linkedCell);
            });
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
                Assert.Same(worldspace, linkedWorldspace);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_Resolve_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.Null(formLink.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_Resolve_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(npc, formLink.TryResolve(package));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_Resolve_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.Null(formLink.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_Resolve_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedNpcGetter>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(placedNpc, placedFormLink.TryResolve(package));
            });
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(cell, cellFormLink.TryResolve(package));
            });
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(worldspace, worldspaceFormLink.TryResolve(package));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolve_MarkerInterface(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(formLink.TryResolve(package, out var linkedRec));
                Assert.Same(spell, linkedRec);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolve_MarkerInterface_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolve_MarkerInterface_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolve_MarkerInterface_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedGetter>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
                Assert.Same(placedNpc, linkedPlacedNpc);
            });
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
                Assert.Same(cell, linkedCell);
            });
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
                Assert.Same(worldspace, linkedWorldspace);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_Resolve_MarkerInterface(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(spell, formLink.TryResolve(package));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_Resolve_MarkerInterface_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.Null(formLink.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_Resolve_MarkerInterface_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.Null(formLink.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_Resolve_MarkerInterface_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedGetter>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(placedNpc, placedFormLink.TryResolve(package));
            });
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(cell, cellFormLink.TryResolve(package));
            });
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(worldspace, worldspaceFormLink.TryResolve(package));
            });
        }
        #endregion

        #region Load Order FormLink Resolves
        LoadOrder<ISkyrimModGetter> TypicalLoadOrder()
        {
            return new LoadOrder<ISkyrimModGetter>()
            {
                new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE),
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
        }

        [Fact]
        public void FormLink_LoadOrder_TryResolve_NoLink()
        {
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            FormLink<INpc> formLink = new FormLink<INpc>(UnusedFormKey);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_TryResolve_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(formLink.TryResolve(package, out var linkedRec));
                Assert.Same(npc, linkedRec);
            });
        }

        [Fact]
        public void FormLink_LoadOrder_TryResolve_DeepRecord_NoLink()
        {
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            FormLink<IPlacedNpc> formLink = new FormLink<IPlacedNpc>(UnusedFormKey);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_TryResolve_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
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
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            FormLink<IPlacedNpc> placedFormLink = new FormLink<IPlacedNpc>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
                Assert.Same(placedNpc, linkedPlacedNpc);
            });
            FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
                Assert.Same(cell, linkedCell);
            });
            FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
                Assert.Same(worldspace, linkedWorldspace);
            });
        }

        [Fact]
        public void FormLink_LoadOrder_Resolve_NoLink()
        {
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            FormLink<INpc> formLink = new FormLink<INpc>(UnusedFormKey);
            Assert.Null(formLink.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_Resolve_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(npc, formLink.TryResolve(package));
            });
        }

        [Fact]
        public void FormLink_LoadOrder_Resolve_DeepRecord_NoLink()
        {
            FormLink<IPlacedNpc> formLink = new FormLink<IPlacedNpc>(UnusedFormKey);
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            Assert.Null(formLink.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_Resolve_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
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
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            FormLink<IPlacedNpc> placedFormLink = new FormLink<IPlacedNpc>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(placedNpc, placedFormLink.TryResolve(package));
            });
            FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(cell, cellFormLink.TryResolve(package));
            });
            FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(worldspace, worldspaceFormLink.TryResolve(package));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_TryResolve_MarkerInterface(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(spell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(formLink.TryResolve(package, out var linkedRec));
                Assert.Same(spell, linkedRec);
            });
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

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_TryResolve_MarkerInterface_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
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
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            FormLink<IPlaced> placedFormLink = new FormLink<IPlaced>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
                Assert.Same(placedNpc, linkedPlacedNpc);
            });
            FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
                Assert.Same(cell, linkedCell);
            });
            FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
                Assert.Same(worldspace, linkedWorldspace);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_Resolve_MarkerInterface(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(spell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(spell, formLink.TryResolve(package));
            });
        }

        [Fact]
        public void FormLink_LoadOrder_Resolve_MarkerInterface_NoLink()
        {
            FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(UnusedFormKey);
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            Assert.Null(formLink.TryResolve(package));
        }

        [Fact]
        public void FormLink_LoadOrder_Resolve_MarkerInterface_DeepRecord_NoLink()
        {
            FormLink<IPlaced> formLink = new FormLink<IPlaced>(UnusedFormKey);
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            Assert.Null(formLink.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_Resolve_MarkerInterface_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
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
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            FormLink<IPlaced> placedFormLink = new FormLink<IPlaced>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(placedNpc, placedFormLink.Resolve(package));
            });
            FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(cell, cellFormLink.Resolve(package));
            });
            FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(worldspace, worldspaceFormLink.Resolve(package));
            });
        }
        #endregion

        #region Direct FormLink Context Resolves
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(formLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package, out var _));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(formLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package, out var linkedRec));
                linkedRec.Record.Should().BeSameAs(npc);
                linkedRec.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedRec.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(formLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(package, out var _));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedNpcGetter>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(placedFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(package, out var linkedPlacedNpc));
                linkedPlacedNpc.Record.Should().BeSameAs(placedNpc);
                linkedPlacedNpc.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedPlacedNpc.Parent.Record.Should().Be(cell);
                var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
                Assert.True(cellFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(package, out var linkedCell));
                linkedCell.Record.Should().BeSameAs(cell);
                linkedCell.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedCell.Parent.Record.Should().Be(subBlock);
                var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
                Assert.True(worldspaceFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(package, out var linkedWorldspace));
                linkedWorldspace.Record.Should().BeSameAs(worldspace);
                linkedWorldspace.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedWorldspace.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Null(formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolvedNpc = formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package);
                resolvedNpc.Record.Should().BeSameAs(npc);
                resolvedNpc.ModKey.Should().Be(TestConstants.PluginModKey);
                resolvedNpc.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Null(formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(package));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedNpc>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var linkedPlacedNpc = placedFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(package);
                linkedPlacedNpc.Record.Should().BeSameAs(placedNpc);
                linkedPlacedNpc.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedPlacedNpc.Parent.Record.Should().BeSameAs(cell);
                var cellFormLink = new FormLink<ICell>(cell.FormKey);
                var linkedCell = cellFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(package);
                linkedCell.Record.Should().BeSameAs(cell);
                linkedCell.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedCell.Parent.Record.Should().BeSameAs(subBlock);
                var worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
                var linkedWorldspace = worldspaceFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(package);
                linkedWorldspace.Record.Should().BeSameAs(worldspace);
                linkedWorldspace.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedWorldspace.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_MarkerInterface(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(formLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(package, out var linkedRec));
                linkedRec.Record.Should().BeSameAs(spell);
                linkedRec.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedRec.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_MarkerInterface_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_MarkerInterface_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_MarkerInterface_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedGetter>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(placedFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(package, out var linkedPlacedNpc));
                linkedPlacedNpc.Record.Should().BeSameAs(placedNpc);
                linkedPlacedNpc.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedPlacedNpc.Parent.Record.Should().BeSameAs(cell);
                var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
                Assert.True(cellFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(package, out var linkedCell));
                linkedCell.Record.Should().BeSameAs(cell);
                linkedCell.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedCell.Parent.Record.Should().BeSameAs(subBlock);
                var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
                Assert.True(worldspaceFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(package, out var linkedWorldspace));
                linkedWorldspace.Record.Should().BeSameAs(worldspace);
                linkedWorldspace.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedWorldspace.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_MarkerInterface(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(package);
                resolved.Record.Should().BeSameAs(spell);
                resolved.ModKey.Should().Be(TestConstants.PluginModKey);
                resolved.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_MarkerInterface_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(package);
                Assert.Null(resolved);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_MarkerInterface_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(package);
                Assert.Null(resolved);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_MarkerInterface_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedGetter>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolvedPlaced = placedFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(package);
                resolvedPlaced.Record.Should().BeSameAs(placedNpc);
                resolvedPlaced.ModKey.Should().Be(TestConstants.PluginModKey);
                resolvedPlaced.Parent.Record.Should().BeSameAs(cell);
                var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
                var resolvedCell = cellFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(package);
                resolvedCell.Record.Should().BeSameAs(cell);
                resolvedCell.ModKey.Should().Be(TestConstants.PluginModKey);
                resolvedCell.Parent.Record.Should().BeSameAs(subBlock);
                var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
                var resolvedWorldspace = worldspaceFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(package);
                resolvedWorldspace.Record.Should().BeSameAs(worldspace);
                resolvedWorldspace.ModKey.Should().Be(TestConstants.PluginModKey);
                resolvedWorldspace.Parent.Should().BeNull();
            });
        }
        #endregion

        #region FormLink Direct ResolveAll
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveAll_Empty(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            formLink.ResolveAll(package).Should().BeEmpty();
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveAll_Typed_Empty(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            formLink.ResolveAll<IPlacedGetter, IPlacedNpcGetter>(package).Should().BeEmpty();
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveAll_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveAll(package).ToArray();
                resolved.Should().HaveCount(1);
                resolved.First().Should().BeSameAs(npc);
            });
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

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_ResolveAll_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveAll(package).ToArray();
                resolved.Should().HaveCount(1);
                resolved.First().Should().BeSameAs(npc);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_ResolveAll_MultipleLinks(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveAll(package).ToArray();
                resolved.Should().HaveCount(2);
                resolved.First().Should().BeSameAs(npcOverride);
                resolved.Last().Should().BeSameAs(npc);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_ResolveAll_DoubleQuery(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveAll(package).ToArray();
                resolved.Should().HaveCount(2);
                resolved.First().Should().BeSameAs(npcOverride);
                resolved.Last().Should().BeSameAs(npc);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_ResolveAll_UnrelatedNotIncluded(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var unrelatedNpc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveAll(package).ToArray();
                resolved.Should().HaveCount(2);
                resolved.First().Should().BeSameAs(npcOverride);
                resolved.Last().Should().BeSameAs(npc);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_ResolveAll_SeparateQueries(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var unrelatedNpc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveAll(package).ToArray();
                resolved.Should().HaveCount(2);
                resolved.First().Should().BeSameAs(npcOverride);
                resolved.Last().Should().BeSameAs(npc);
            });
            formLink = new FormLink<INpcGetter>(unrelatedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveAll(package).ToArray();
                resolved.Should().HaveCount(1);
                resolved.First().Should().BeSameAs(unrelatedNpc);
            });
        }
        #endregion

        #region FormLink Direct ResolveAllContexts
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveAllContexts_Empty(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(package).Should().BeEmpty();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveAllContexts_Typed_Empty(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IPlacedGetter, IPlacedNpc, IPlacedNpcGetter>(package).Should().BeEmpty();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveAllContexts_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package).ToArray();
                resolved.Should().HaveCount(1);
                resolved.First().Record.Should().BeSameAs(npc);
                resolved.First().ModKey.Should().Be(TestConstants.PluginModKey);
                resolved.First().Parent.Should().BeNull();
            });
        }
        #endregion

        #region FormLink LoadOrder ResolveAllContexts
        [Fact]
        public void FormLink_LoadOrder_ResolveAllContexts_Empty()
        {
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package).Should().BeEmpty();
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_ResolveAllContexts_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package).ToArray();
            resolved.Should().HaveCount(1);
            resolved.First().Record.Should().BeSameAs(npc);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.First().Parent.Should().BeNull();
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_ResolveAllContexts_MultipleLinks(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_ResolveAllContexts_DoubleQuery(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package).ToArray();
            resolved = formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_ResolveAllContexts_UnrelatedNotIncluded(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var unrelatedNpc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_LoadOrder_ResolveAllContexts_SeparateQueries(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var unrelatedNpc = mod.Npcs.AddNew();
            var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
            var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
            npcOverride.FaceParts = new NpcFaceParts();
            var loadOrder = new LoadOrder<ISkyrimModGetter>()
            {
                mod,
                new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
                mod2
            };
            var (style, package) = GetLinkCache(loadOrder, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            var resolved = formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package).ToArray();
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
            formLink = new FormLink<INpcGetter>(unrelatedNpc.FormKey);
            resolved = formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package).ToArray();
            resolved.Should().HaveCount(1);
            resolved.First().Record.Should().BeSameAs(unrelatedNpc);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.First().Parent.Should().BeNull();
        }
        #endregion

        #region EDIDLink Resolves
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void EDIDLink_TryResolve_NoLink(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var (style, package) = GetLinkCache(mod, cacheType);
            Assert.False(link.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void EDIDLink_TryResolve_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var (style, package) = GetLinkCache(mod, cacheType);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.True(link.TryResolve(package, out var linkedRec));
            Assert.Same(effect, linkedRec);
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void EDIDLink_Resolve_NoLink(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var (style, package) = GetLinkCache(mod, cacheType);
            Assert.Null(link.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void EDIDLink_Resolve_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var (style, package) = GetLinkCache(mod, cacheType);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.Same(effect, link.TryResolve(package));
        }
        #endregion

        #region Subtype Linking
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void SubtypeLinking_Typical(LinkCacheTestTypes cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var armor = prototype.Armors.AddNew();
            var llist = prototype.LeveledItems.AddNew();
            llist.Entries = new ExtendedList<LeveledItemEntry>()
            {
                new LeveledItemEntry()
                {
                    Data = new LeveledItemEntryData()
                    {
                        Reference = armor.AsLink()
                    }
                }
            };
            using var disp = ConvertMod(prototype, out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(mod.LeveledItems.First().Entries[0].Data.Reference.TryResolve(package, out IArmorGetter armorGetterLink));
            });
        }
        #endregion

        #region Resolve Identifiers
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void ResolveLinkIdentifiers(LinkCacheTestTypes cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var armor = prototype.Armors.AddNew();
            var llist = prototype.LeveledItems.AddNew();
            llist.Entries = new ExtendedList<LeveledItemEntry>()
            {
                new LeveledItemEntry()
                {
                    Data = new LeveledItemEntryData()
                    {
                        Reference = armor.AsLink()
                    }
                }
            };
            using var disp = ConvertMod(prototype, out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);
            Assert.True(package.TryResolveIdentifier(mod.LeveledItems.First().Entries[0].Data.Reference.FormKey, out _));
        }
        #endregion

        #region Warmup Caching
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void Warmup_Caches(LinkCacheTestTypes cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var armor = prototype.Armors.AddNew();
            var llist = prototype.LeveledItems.AddNew();
            llist.Entries = new ExtendedList<LeveledItemEntry>()
            {
                new LeveledItemEntry()
                {
                    Data = new LeveledItemEntryData()
                    {
                        Reference = armor.AsLink()
                    }
                }
            };
            using var disp = ConvertMod(prototype, out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);
            package.Warmup<IArmorGetter>();
            prototype.Armors.Clear();
            prototype.LeveledItems.Clear();
            package.TryResolveIdentifier<IArmorGetter>(armor.FormKey, out _)
                .Should().Be(style == LinkCacheStyle.HasCaching || ReadOnly);
            package.TryResolveIdentifier<ILeveledItemGetter>(llist.FormKey, out _)
                .Should().Be(ReadOnly);
        }
        #endregion

        #region Specific Issue Tests
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void PlacedInCellQuerySucceedsIfMajorRecordType(LinkCacheTestTypes cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            var placed = new PlacedObject(prototype);
            prototype.Cells.Records.Add(new CellBlock()
            {
                SubBlocks = new ExtendedList<CellSubBlock>()
                {
                    new CellSubBlock()
                    {
                        Cells = new ExtendedList<Cell>()
                        {
                            new Cell(prototype)
                            {
                                Temporary = new ExtendedList<IPlaced>()
                                {
                                    placed
                                }
                            }
                        }
                    }
                }
            });
            using var disp = ConvertMod(prototype, out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                package.TryResolveContext<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(placed.FormKey, out var rec)
                .Should().BeTrue();
                rec.Record.Should().Be(placed);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                package.TryResolve<ISkyrimMajorRecordGetter>(placed.FormKey, out var rec2)
                .Should().BeTrue();
                rec2.Should().Be(placed);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void PlacedInWorldspaceQuerySucceedsIfMajorRecordType(LinkCacheTestTypes cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            var placed = new PlacedObject(prototype);
            prototype.Worldspaces.Add(new Worldspace(prototype)
            {
                SubCells = new ExtendedList<WorldspaceBlock>()
                {
                    new WorldspaceBlock()
                    {
                        Items = new ExtendedList<WorldspaceSubBlock>()
                        {
                            new WorldspaceSubBlock()
                            {
                                Items = new ExtendedList<Cell>()
                                {
                                    new Cell(prototype)
                                    {
                                        Temporary = new ExtendedList<IPlaced>()
                                        {
                                            placed
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
            using var disp = ConvertMod(prototype, out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                package.TryResolve<ISkyrimMajorRecordGetter>(placed.FormKey, out var rec2)
                .Should().BeTrue();
                rec2.Should().Be(placed);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                package.TryResolveContext<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(placed.FormKey, out var rec)
                .Should().BeTrue();
                rec.Record.Should().Be(placed);
            });
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

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.HasCaching, new ImmutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter, prefs));
        }

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.HasCaching, loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(prefs));
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
            var tempFile = new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath);
            var path = new ModPath(mod.ModKey, tempFile.File.Path);
            mod.WriteToBinaryParallel(
                path,
                new BinaryWriteParameters()
                {
                    ModKey = ModKeyOption.NoCheck,
                });
            var overlay = SkyrimMod.CreateFromBinaryOverlay(path, SkyrimRelease.SkyrimLE);
            getter = overlay;
            return Disposable.Create(() =>
            {
                overlay.Dispose();
                tempFile.Dispose();
            });
        }

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.HasCaching, new ImmutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter, prefs));
        }

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.HasCaching, loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(prefs));
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

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.OnlyDirect, new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter));
        }

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.HasCaching, new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(prefs)));
        }
    }

    public class Linking_MutableOverlay_Tests : Linking_Abstract_Tests
    {
        public override bool ReadOnly => true;

        public override IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter)
        {
            var tempFile = new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath);
            var path = new ModPath(mod.ModKey, tempFile.File.Path);
            mod.WriteToBinaryParallel(
                path,
                new BinaryWriteParameters()
                {
                    ModKey = ModKeyOption.NoCheck,
                });
            var overlay = SkyrimMod.CreateFromBinaryOverlay(path, SkyrimRelease.SkyrimLE);
            getter = overlay;
            return Disposable.Create(() =>
            {
                overlay.Dispose();
                tempFile.Dispose();
            });
        }

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.OnlyDirect, new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter));
        }

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.OnlyDirect, new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>()));
        }
    }
}
