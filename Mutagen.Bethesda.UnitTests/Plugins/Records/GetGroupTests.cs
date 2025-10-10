using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class GetGroupTests
{
    /// <summary>
    /// Tests that TryGetTopLevelGroup correctly maps variant types (like GlobalShort, GlobalFloat, etc.)
    /// to their parent group. This validates the fix for the GetGroup method which now includes
    /// inheriting types in the switch statement.
    /// </summary>
    [Theory, MutagenModAutoData]
    public void TryGetTopLevelGroup_WithGlobalShort_ReturnsGlobalsGroup(
        SkyrimMod mod)
    {
        // Act - Query using GlobalShort type parameter
        var group = mod.TryGetTopLevelGroup(typeof(GlobalShort));

        // Assert - Should return the Globals group (even though GlobalShort is a variant)
        group.ShouldNotBeNull();
        group.ShouldBe(mod.Globals);
    }

    [Theory, MutagenModAutoData]
    public void TryGetTopLevelGroup_WithGlobalFloat_ReturnsGlobalsGroup(
        SkyrimMod mod)
    {
        // Act - Query using GlobalFloat type parameter
        var group = mod.TryGetTopLevelGroup(typeof(GlobalFloat));

        // Assert - Should return the Globals group
        group.ShouldNotBeNull();
        group.ShouldBe(mod.Globals);
    }

    [Theory, MutagenModAutoData]
    public void TryGetTopLevelGroup_WithGlobalInt_ReturnsGlobalsGroup(
        SkyrimMod mod)
    {
        // Act - Query using GlobalInt type parameter
        var group = mod.TryGetTopLevelGroup(typeof(GlobalInt));

        // Assert - Should return the Globals group
        group.ShouldNotBeNull();
        group.ShouldBe(mod.Globals);
    }

    [Theory, MutagenModAutoData]
    public void TryGetTopLevelGroup_WithGlobalUnknown_ReturnsGlobalsGroup(
        SkyrimMod mod)
    {
        // Act - Query using GlobalUnknown type parameter
        var group = mod.TryGetTopLevelGroup(typeof(GlobalUnknown));

        // Assert - Should return the Globals group
        group.ShouldNotBeNull();
        group.ShouldBe(mod.Globals);
    }

    [Theory, MutagenModAutoData]
    public void TryGetTopLevelGroup_WithGameSettingInt_ReturnsGameSettingsGroup(
        SkyrimMod mod)
    {
        // Act - Query using GameSettingInt type parameter
        var group = mod.TryGetTopLevelGroup(typeof(GameSettingInt));

        // Assert - Should return the GameSettings group
        group.ShouldNotBeNull();
        group.ShouldBe(mod.GameSettings);
    }

    [Theory, MutagenModAutoData]
    public void TryGetTopLevelGroup_WithGameSettingFloat_ReturnsGameSettingsGroup(
        SkyrimMod mod)
    {
        // Act - Query using GameSettingFloat type parameter
        var group = mod.TryGetTopLevelGroup(typeof(GameSettingFloat));

        // Assert - Should return the GameSettings group
        group.ShouldNotBeNull();
        group.ShouldBe(mod.GameSettings);
    }

    [Theory, MutagenModAutoData]
    public void TryGetTopLevelGroup_WithGameSettingString_ReturnsGameSettingsGroup(
        SkyrimMod mod)
    {
        // Act - Query using GameSettingString type parameter
        var group = mod.TryGetTopLevelGroup(typeof(GameSettingString));

        // Assert - Should return the GameSettings group
        group.ShouldNotBeNull();
        group.ShouldBe(mod.GameSettings);
    }

    [Theory, MutagenModAutoData]
    public void TryGetTopLevelGroup_WithGameSettingBool_ReturnsGameSettingsGroup(
        SkyrimMod mod)
    {
        // Act - Query using GameSettingBool type parameter
        var group = mod.TryGetTopLevelGroup(typeof(GameSettingBool));

        // Assert - Should return the GameSettings group
        group.ShouldNotBeNull();
        group.ShouldBe(mod.GameSettings);
    }

    [Theory, MutagenModAutoData]
    public void TryGetTopLevelGroup_WithIGlobalShortGetter_ReturnsGlobalsGroup(
        SkyrimMod mod)
    {
        // Act - Query using IGlobalShortGetter interface type
        var group = mod.TryGetTopLevelGroup(typeof(IGlobalShortGetter));

        // Assert - Should return the Globals group
        group.ShouldNotBeNull();
        group.ShouldBe(mod.Globals);
    }

    [Theory, MutagenModAutoData]
    public void TryGetTopLevelGroup_WithIGameSettingIntGetter_ReturnsGameSettingsGroup(
        SkyrimMod mod)
    {
        // Act - Query using IGameSettingIntGetter interface type
        var group = mod.TryGetTopLevelGroup(typeof(IGameSettingIntGetter));

        // Assert - Should return the GameSettings group
        group.ShouldNotBeNull();
        group.ShouldBe(mod.GameSettings);
    }
}
