using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking
{
    public partial class ALinkingTests
    {
        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void LoadOrderEmpty(LinkCachePreferences.RetentionType cacheType)
        {
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            var (style, linkCache) = GetLinkCache(loadOrder, cacheType);

            // Test FormKey fails
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest(linkCache, UnusedFormKey, out var _));
            });
            Assert.False(TryTest(linkCache, FormKey.Null, out var _));
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest<IMajorRecord, IMajorRecordGetter>(linkCache, UnusedFormKey, out var _));
            });
            Assert.False(TryTest<IMajorRecord, IMajorRecordGetter>(linkCache, FormKey.Null, out var _));
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(linkCache, UnusedFormKey, out var _));
            });
            Assert.False(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(linkCache, FormKey.Null, out var _));
            // Test EditorID fails
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest(linkCache, UnusedEditorID, out var _));
            });
            Assert.False(TryTest(linkCache, string.Empty, out var _));
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest<IMajorRecord, IMajorRecordGetter>(linkCache, UnusedEditorID, out var _));
            });
            Assert.False(TryTest<IMajorRecord, IMajorRecordGetter>(linkCache, string.Empty, out var _));
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(linkCache, UnusedEditorID, out var _));
            });
            Assert.False(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(linkCache, string.Empty, out var _));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void LoadOrderNoMatch(LinkCachePreferences.RetentionType cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            prototype.Npcs.AddNew();
            using var disp = ConvertMod(prototype, out var mod);
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            loadOrder.Add(mod);
            var (style, cache) = GetLinkCache(loadOrder, cacheType);

            // Test FormKey fails 
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest(cache, UnusedFormKey, out var _));
            });
            Assert.False(TryTest(cache, FormKey.Null, out var _));
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest<IMajorRecord, IMajorRecordGetter>(cache, UnusedFormKey, out var _));
            });
            Assert.False(TryTest<IMajorRecord, IMajorRecordGetter>(cache, FormKey.Null, out var _));
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, UnusedFormKey, out var _));
            });
            Assert.False(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, FormKey.Null, out var _));

            // Test EditorID fails
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest(cache, UnusedEditorID, out var _));
            });
            Assert.False(TryTest(cache, string.Empty, out var _));
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest<IMajorRecord, IMajorRecordGetter>(cache, UnusedEditorID, out var _));
            });
            Assert.False(TryTest<IMajorRecord, IMajorRecordGetter>(cache, string.Empty, out var _));
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, UnusedEditorID, out var _));
            });
            Assert.False(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, string.Empty, out var _));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void LoadOrderSingle(LinkCachePreferences.RetentionType cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var objEffect1 = prototype.ObjectEffects.AddNew("EditorID1");
            var objEffect2 = prototype.ObjectEffects.AddNew("EditorID2");
            using var disp = ConvertMod(prototype, out var mod);
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            loadOrder.Add(mod);
            var (style, cache) = GetLinkCache(loadOrder, cacheType);

            // Test query successes

            // Do linked interfaces first, as this tests a specific edge case
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, objEffect1.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, objEffect1.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, objEffect2.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, objEffect2.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect2.FormKey);
            });

            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });

            if (ReadOnly)
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect2.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect2.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect2.EditorID, out var _));
                });
            }
            else
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
            }
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void LoadOrderOneInEach(LinkCachePreferences.RetentionType cacheType)
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
            var (style, cache) = GetLinkCache(loadOrder, cacheType);

            // Test query successes

            // Do linked interfaces first, as this tests a specific edge case
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, objEffect1.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, objEffect1.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, objEffect2.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, objEffect2.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, objEffect2.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            if (ReadOnly)
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect1.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect2.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect2.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect2.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect1.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect2.EditorID, out var _));
                });
            }
            else
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect1.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect1.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect1.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect2.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, objEffect2.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, objEffect2.FormKey);
                });
            }
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void LoadOrderOverridden(LinkCachePreferences.RetentionType cacheType)
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
            var (style, cache) = GetLinkCache(loadOrder, cacheType);

            // Test query successes

            // Do linked interfaces first, as this tests a specific edge case
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, unoverriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, topModRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, overriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, unoverriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, topModRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, overriddenRec.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, unoverriddenRec.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, topModRec.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, overriddenRec.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, unoverriddenRec.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, topModRec.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, topModRec.FormKey);
            });

            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, unoverriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, topModRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, overriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, unoverriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, topModRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, unoverriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, topModRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, overriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, unoverriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, topModRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, unoverriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, topModRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, overriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, unoverriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, topModRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });

            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, unoverriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, topModRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, overriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, unoverriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, topModRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });

            if (ReadOnly)
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, overriddenRec.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, unoverriddenRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, topModRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, overriddenRec.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, unoverriddenRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, topModRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, overriddenRec.FormKey, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, unoverriddenRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, topModRec.FormKey, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, overriddenRec.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, unoverriddenRec.EditorID, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, topModRec.EditorID, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, overriddenRec.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, unoverriddenRec.EditorID, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, topModRec.EditorID, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, overriddenRec.EditorID, out var _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, unoverriddenRec.EditorID, out _));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, topModRec.EditorID, out _));
                });
            }
            else
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, overriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, unoverriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, topModRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, overriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, unoverriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, topModRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, overriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, unoverriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, topModRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, overriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, unoverriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, topModRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });

                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, overriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, unoverriddenRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, topModRec.FormKey, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, overriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, overrideRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, unoverriddenRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, topModRec.EditorID, out var rec));
                    Assert.Equal(rec.FormKey, topModRec.FormKey);
                });
            }
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void LoadOrderOriginatingTarget(LinkCachePreferences.RetentionType cacheType)
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
            var (style, cache) = GetLinkCache(loadOrder, cacheType);

            // Test query successes

            // Do linked interfaces first, as this tests a specific edge case
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IEffectRecord, IEffectRecordGetter>(cache, overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                rec.EditorID.Should().Be(overriddenRec.EditorID);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<INamed, INamedGetter>(cache, overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                (rec as IMajorRecordGetter).EditorID.Should().Be(overriddenRec.EditorID);
            });

            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest(cache, overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                rec.EditorID.Should().Be(overriddenRec.EditorID);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IMajorRecord, IMajorRecordGetter>(cache, overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                rec.EditorID.Should().Be(overriddenRec.EditorID);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                rec.EditorID.Should().Be(overriddenRec.EditorID);
            });

            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(TryTest<IObjectEffect, IObjectEffectGetter>(cache, overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                rec.EditorID.Should().Be(overriddenRec.EditorID);
            });

            if (ReadOnly)
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IObjectEffect, IObjectEffect>(cache, overriddenRec.FormKey, out var _, ResolveTarget.Origin));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<ObjectEffect, ObjectEffect>(cache, overriddenRec.FormKey, out var _, ResolveTarget.Origin));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.False(TryTest<IEffectRecord, IEffectRecord>(cache, overriddenRec.FormKey, out var _, ResolveTarget.Origin));
                });
            }
            else
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IObjectEffect, IObjectEffect>(cache, overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                    rec.EditorID.Should().Be(overriddenRec.EditorID);
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<ObjectEffect, ObjectEffect>(cache, overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                    rec.EditorID.Should().Be(overriddenRec.EditorID);
                });

                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<IEffectRecord, IEffectRecord>(cache, overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                    rec.EditorID.Should().Be(overriddenRec.EditorID);
                });
            }
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void LoadOrderReadOnlyMechanics(LinkCachePreferences.RetentionType cacheType)
        {
            var wrapper = SkyrimMod.CreateFromBinaryOverlay(TestDataPathing.SkyrimTestMod, SkyrimRelease.SkyrimSE);
            var overrideWrapper = SkyrimMod.CreateFromBinaryOverlay(TestDataPathing.SkyrimOverrideMod, SkyrimRelease.SkyrimSE);
            var loadOrder = new LoadOrder<ISkyrimModGetter>();
            loadOrder.Add(wrapper);
            loadOrder.Add(overrideWrapper);
            var (style, cache) = GetLinkCache(loadOrder, cacheType);
            {
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<INpc, INpcGetter>(cache, TestFileFormKey, out var rec));
                });
                WrapPotentialThrow(cacheType, style, () =>
                {
                    Assert.True(TryTest<INpc, INpcGetter>(cache, TestFileFormKey2, out var rec));
                    Assert.NotNull(rec.Name);
                    Assert.Equal("A Name", rec.Name.String);
                });
            }
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest<INpc, INpc>(cache, TestFileFormKey, out var rec));
                Assert.False(TryTest<INpc, INpc>(cache, TestFileFormKey2, out rec));
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(TryTest<Npc, Npc>(cache, TestFileFormKey, out var rec));
                Assert.False(TryTest<Npc, Npc>(cache, TestFileFormKey2, out rec));
            });
        }
    }
}