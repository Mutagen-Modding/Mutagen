using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public partial class ALinkingTests
{
    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void DirectEmpty(LinkCachePreferences.RetentionType cacheType)
    {
        using var disp = ConvertMod(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), out var mod);
        var (style, package) = GetLinkCache(mod, cacheType);

        // Test FormKey fails
        Assert.False(package.TryResolve(UnusedFormKey, out var _));
        Assert.False(package.TryResolve(FormKey.Null, out var _));
        Assert.False(package.TryResolve<IMajorRecordGetter>(UnusedFormKey, out var _));
        Assert.False(package.TryResolve<IMajorRecordGetter>(FormKey.Null, out var _));
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
        Assert.False(package.TryResolve<IMajorRecordGetter>(UnusedEditorID, out var _));
        Assert.False(package.TryResolve<IMajorRecordGetter>(string.Empty, out var _));
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
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void DirectNoMatch(LinkCachePreferences.RetentionType cacheType)
    {
        var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        prototype.ObjectEffects.AddNew();
        using var disp = ConvertMod(prototype, out var mod);
        var (style, package) = GetLinkCache(mod, cacheType);

        // Test FormKey fails
        Assert.False(package.TryResolve(UnusedFormKey, out var _));
        Assert.False(package.TryResolve(FormKey.Null, out var _));
        Assert.False(package.TryResolve<IMajorRecordGetter>(UnusedFormKey, out var _));
        Assert.False(package.TryResolve<IMajorRecordGetter>(FormKey.Null, out var _));
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
        Assert.False(package.TryResolve<IMajorRecordGetter>(UnusedEditorID, out var _));
        Assert.False(package.TryResolve<IMajorRecordGetter>(string.Empty, out var _));
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
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void DirectTypical(LinkCachePreferences.RetentionType cacheType)
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
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void DirectReadOnlyMechanics(LinkCachePreferences.RetentionType cacheType)
    {
        var wrapper = SkyrimMod.CreateFromBinaryOverlay(TestDataPathing.SkyrimTestMod, SkyrimRelease.SkyrimSE);
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
}