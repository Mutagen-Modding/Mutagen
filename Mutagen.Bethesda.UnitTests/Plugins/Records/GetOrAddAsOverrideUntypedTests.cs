using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class GetOrAddAsOverrideUntypedTests
{
    [Fact]
    public void TryGetOrAddAsOverrideUntyped_Success_NewRecord()
    {
        // Arrange
        var masterMod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var npc = masterMod.Npcs.AddNew();
        npc.Name = "TestNpc";

        var pluginMod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        // Act
        var result = pluginMod.Npcs.TryGetOrAddAsOverrideUntyped(npc, out var overrideRecord);

        // Assert
        Assert.True(result);
        Assert.NotNull(overrideRecord);
        Assert.Equal(npc.FormKey, overrideRecord.FormKey);
        Assert.Equal(1, pluginMod.Npcs.Count);
        Assert.Same(overrideRecord, pluginMod.Npcs.Records.First());
        Assert.Equal("TestNpc", ((INpcGetter)overrideRecord).Name?.String);
    }

    [Fact]
    public void TryGetOrAddAsOverrideUntyped_TypeMismatch_ReturnsFalse()
    {
        // Arrange
        var masterMod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var npc = masterMod.Npcs.AddNew();

        var pluginMod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        // Act - try to add an NPC to the Weapons group
        var result = pluginMod.Weapons.TryGetOrAddAsOverrideUntyped(npc, out var overrideRecord);

        // Assert
        Assert.False(result);
        Assert.Null(overrideRecord);
        Assert.Equal(0, pluginMod.Weapons.Count);
    }

    [Fact]
    public void TryGetOrAddAsOverrideUntyped_ExistingOverride_ReturnsExisting()
    {
        // Arrange
        var masterMod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var npc = masterMod.Npcs.AddNew();
        npc.Name = "OriginalName";

        var pluginMod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        // Act - first call creates the override
        var firstResult = pluginMod.Npcs.TryGetOrAddAsOverrideUntyped(npc, out var firstOverride);
        Assert.NotNull(firstOverride);
        ((Npc)firstOverride).Name = "ModifiedName";

        // Act - second call should return the existing override
        var secondResult = pluginMod.Npcs.TryGetOrAddAsOverrideUntyped(npc, out var secondOverride);

        // Assert
        Assert.True(firstResult);
        Assert.True(secondResult);
        Assert.Same(firstOverride, secondOverride);
        Assert.Equal(1, pluginMod.Npcs.Count);
        Assert.Equal("ModifiedName", ((INpcGetter)secondOverride).Name?.String);
    }

    [Fact]
    public void TryGetOrAddAsOverrideUntyped_FormKeyPreservation()
    {
        // Arrange
        var masterMod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var npc = masterMod.Npcs.AddNew(TestConstants.Form1);

        var pluginMod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        // Act
        var result = pluginMod.Npcs.TryGetOrAddAsOverrideUntyped(npc, out var overrideRecord);

        // Assert
        Assert.True(result);
        Assert.NotNull(overrideRecord);
        Assert.Equal(TestConstants.Form1, overrideRecord.FormKey);
        Assert.Equal(npc.FormKey, overrideRecord.FormKey);
    }

    [Fact]
    public void TryGetOrAddAsOverrideUntyped_MultipleRecords_Success()
    {
        // Arrange
        var masterMod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var npc1 = masterMod.Npcs.AddNew();
        var npc2 = masterMod.Npcs.AddNew();
        var weapon = masterMod.Weapons.AddNew();

        var pluginMod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        // Act
        var npc1Result = pluginMod.Npcs.TryGetOrAddAsOverrideUntyped(npc1, out var npc1Override);
        var npc2Result = pluginMod.Npcs.TryGetOrAddAsOverrideUntyped(npc2, out var npc2Override);
        var weaponResult = pluginMod.Weapons.TryGetOrAddAsOverrideUntyped(weapon, out var weaponOverride);

        // Assert
        Assert.True(npc1Result);
        Assert.True(npc2Result);
        Assert.True(weaponResult);
        Assert.Equal(2, pluginMod.Npcs.Count);
        Assert.Equal(1, pluginMod.Weapons.Count);
        Assert.NotSame(npc1Override, npc2Override);
        Assert.Equal(npc1.FormKey, npc1Override.FormKey);
        Assert.Equal(npc2.FormKey, npc2Override.FormKey);
        Assert.Equal(weapon.FormKey, weaponOverride.FormKey);
    }

    [Fact]
    public void TryGetOrAddAsOverrideUntyped_WithEditorId()
    {
        // Arrange
        var masterMod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var npc = masterMod.Npcs.AddNew();
        npc.EditorID = TestConstants.Edid1;
        npc.Name = "TestNpc";

        var pluginMod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        // Act
        var result = pluginMod.Npcs.TryGetOrAddAsOverrideUntyped(npc, out var overrideRecord);

        // Assert
        Assert.True(result);
        Assert.NotNull(overrideRecord);
        Assert.Equal(TestConstants.Edid1, overrideRecord.EditorID);
        Assert.Equal("TestNpc", ((INpcGetter)overrideRecord).Name?.String);
    }

    [Fact]
    public void TryGetOrAddAsOverrideUntyped_DifferentRecordTypes()
    {
        // Arrange
        var masterMod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var armor = masterMod.Armors.AddNew();
        var weapon = masterMod.Weapons.AddNew();
        var spell = masterMod.Spells.AddNew();

        var pluginMod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        // Act
        var armorResult = pluginMod.Armors.TryGetOrAddAsOverrideUntyped(armor, out var armorOverride);
        var weaponResult = pluginMod.Weapons.TryGetOrAddAsOverrideUntyped(weapon, out var weaponOverride);
        var spellResult = pluginMod.Spells.TryGetOrAddAsOverrideUntyped(spell, out var spellOverride);

        // Assert
        Assert.True(armorResult);
        Assert.True(weaponResult);
        Assert.True(spellResult);
        Assert.NotNull(armorOverride);
        Assert.NotNull(weaponOverride);
        Assert.NotNull(spellOverride);
        Assert.Equal(1, pluginMod.Armors.Count);
        Assert.Equal(1, pluginMod.Weapons.Count);
        Assert.Equal(1, pluginMod.Spells.Count);
    }

    [Fact]
    public void TryGetOrAddAsOverrideUntyped_WrongGroupType_Weapon_To_Armor()
    {
        // Arrange
        var masterMod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var weapon = masterMod.Weapons.AddNew();

        var pluginMod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        // Act - try to add a Weapon to the Armors group
        var result = pluginMod.Armors.TryGetOrAddAsOverrideUntyped(weapon, out var overrideRecord);

        // Assert
        Assert.False(result);
        Assert.Null(overrideRecord);
        Assert.Equal(0, pluginMod.Armors.Count);
    }

    [Fact]
    public void TryGetOrAddAsOverrideUntyped_EmptyGroup()
    {
        // Arrange
        var masterMod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var npc = masterMod.Npcs.AddNew();

        var pluginMod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        // Assert the group is initially empty
        Assert.Equal(0, pluginMod.Npcs.Count);

        // Act
        var result = pluginMod.Npcs.TryGetOrAddAsOverrideUntyped(npc, out var overrideRecord);

        // Assert
        Assert.True(result);
        Assert.NotNull(overrideRecord);
        Assert.Equal(1, pluginMod.Npcs.Count);
    }

    [Fact]
    public void TryGetOrAddAsOverrideUntyped_Idempotent()
    {
        // Arrange
        var masterMod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var npc = masterMod.Npcs.AddNew();
        npc.EditorID = "TestNPC";

        var pluginMod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        // Act - call multiple times
        var result1 = pluginMod.Npcs.TryGetOrAddAsOverrideUntyped(npc, out var override1);
        var result2 = pluginMod.Npcs.TryGetOrAddAsOverrideUntyped(npc, out var override2);
        var result3 = pluginMod.Npcs.TryGetOrAddAsOverrideUntyped(npc, out var override3);

        // Assert - all should return the same instance
        Assert.True(result1);
        Assert.True(result2);
        Assert.True(result3);
        Assert.Same(override1, override2);
        Assert.Same(override2, override3);
        Assert.Equal(1, pluginMod.Npcs.Count);
    }

    [Fact]
    public void TryGetOrAddAsOverrideUntyped_Quest_Success()
    {
        // Arrange
        var masterMod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var quest = masterMod.Quests.AddNew();
        quest.EditorID = "TestQuest";

        var pluginMod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        // Act
        var result = pluginMod.Quests.TryGetOrAddAsOverrideUntyped(quest, out var overrideRecord);

        // Assert
        Assert.True(result);
        Assert.NotNull(overrideRecord);
        Assert.Equal(quest.FormKey, overrideRecord.FormKey);
        Assert.Equal("TestQuest", overrideRecord.EditorID);
    }
}
