using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking
{
    public partial class ALinkingTests
    {
        [Fact]
        public void LoadOrderEmpty()
        {
            var package = new LoadOrder<ISkyrimModGetter>().ToImmutableLinkCache();

            // Test FormKey fails
            Assert.False(package.TryResolve(UnusedFormKey, out var _));
            Assert.False(package.TryResolve(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IMajorRecordGetter>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(FormKey.Null, out var _));

            // Test EditorID fails
            Assert.False(package.TryResolve(UnusedEditorID, out var _));
            Assert.False(package.TryResolve(string.Empty, out var _));
            Assert.False(package.TryResolve<IMajorRecordGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IMajorRecordGetter>(string.Empty, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(string.Empty, out var _));
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
            var (style, package) = GetLinkCache(loadOrder, cacheType);

            // Test FormKey fails
            Assert.False(package.TryResolve(UnusedFormKey, out var _));
            Assert.False(package.TryResolve(FormKey.Null, out var _));
            Assert.False(package.TryResolve<IMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<IMajorRecordGetter>(FormKey.Null, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedFormKey, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(FormKey.Null, out var _));

            // Test EditorID fails
            Assert.False(package.TryResolve(UnusedEditorID, out var _));
            Assert.False(package.TryResolve(string.Empty, out var _));
            Assert.False(package.TryResolve<IMajorRecordGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<IMajorRecordGetter>(string.Empty, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(UnusedEditorID, out var _));
            Assert.False(package.TryResolve<ISkyrimMajorRecordGetter>(string.Empty, out var _));
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
                Assert.True(package.TryResolve<IObjectBoundedGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectBoundedGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectBoundedGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectBoundedGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect2.FormKey);
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
                Assert.True(package.TryResolve<IMajorRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(objEffect2.EditorID, out var rec));
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
                Assert.True(package.TryResolve<IObjectBoundedGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectBoundedGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectBoundedGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectBoundedGetter>(objEffect2.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, objEffect2.FormKey);
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
                Assert.True(package.TryResolve<IMajorRecordGetter>(objEffect1.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(objEffect1.EditorID, out var rec));
                Assert.Equal(rec.FormKey, objEffect1.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(objEffect2.FormKey, out var rec));
                Assert.Equal(rec.FormKey, objEffect2.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(objEffect2.EditorID, out var rec));
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
                Assert.True(package.TryResolve<IObjectBoundedGetter>(overriddenRec.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectBoundedGetter>(unoverriddenRec.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectBoundedGetter>(topModRec.FormKey, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectBoundedGetter>(overriddenRec.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectBoundedGetter>(unoverriddenRec.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IObjectBoundedGetter>(topModRec.EditorID, out var rec));
                Assert.Equal((rec as IMajorRecordGetter).FormKey, topModRec.FormKey);
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
                Assert.True(package.TryResolve<IMajorRecordGetter>(overriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(unoverriddenRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(topModRec.FormKey, out var rec));
                Assert.Equal(rec.FormKey, topModRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(overriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, overrideRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(unoverriddenRec.EditorID, out var rec));
                Assert.Equal(rec.FormKey, unoverriddenRec.FormKey);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(topModRec.EditorID, out var rec));
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
                Assert.True(package.TryResolve<IObjectBoundedGetter>(overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                (rec as IMajorRecordGetter).EditorID.Should().Be(overriddenRec.EditorID);
            });

            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve(overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
                rec.EditorID.Should().Be(overriddenRec.EditorID);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(package.TryResolve<IMajorRecordGetter>(overriddenRec.FormKey, out var rec, ResolveTarget.Origin));
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
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void LoadOrderReadOnlyMechanics(LinkCachePreferences.RetentionType cacheType)
        {
            var wrapper = SkyrimMod.CreateFromBinaryOverlay(TestDataPathing.SkyrimTestMod, SkyrimRelease.SkyrimSE);
            var overrideWrapper = SkyrimMod.CreateFromBinaryOverlay(TestDataPathing.SkyrimOverrideMod, SkyrimRelease.SkyrimSE);
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
                    Assert.NotNull(rec.Name);
                    Assert.Equal("A Name", rec.Name.String);
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
    }
}