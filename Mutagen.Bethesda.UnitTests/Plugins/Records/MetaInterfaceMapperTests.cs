using FluentAssertions;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class MetaInterfaceMapperTests
{
    [Fact]
    internal void AspectInterface()
    {
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(typeof(IEnchantable), out var setterRegis)
            .Should().BeTrue();
        setterRegis.Setter.Should().BeTrue();
        setterRegis.Registrations.Select(x => x.GetterType).Should().Equal(
            typeof(IArmorGetter),
            typeof(IWeaponGetter));
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(typeof(IEnchantableGetter), out var getterRegis)
            .Should().BeTrue();
        getterRegis.Setter.Should().BeFalse();
        getterRegis.Registrations.Select(x => x.GetterType).Should().Equal(
            typeof(IArmorGetter),
            typeof(IWeaponGetter));
    }
    
    [Fact]
    internal void LinkInterface()
    {
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(typeof(IIdleRelation), out var setterRegis)
            .Should().BeTrue();
        setterRegis.Setter.Should().BeTrue();
        setterRegis.Registrations.Select(x => x.GetterType).Should().Equal(
            typeof(IActionRecordGetter),
            typeof(IIdleAnimationGetter));
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(typeof(IIdleRelationGetter), out var getterRegis)
            .Should().BeTrue();
        getterRegis.Setter.Should().BeFalse();
        getterRegis.Registrations.Select(x => x.GetterType).Should().Equal(
            typeof(IActionRecordGetter),
            typeof(IIdleAnimationGetter));
    }
    
    [Fact]
    internal void IsolatedAbstractInterface()
    {
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(typeof(IAStoryManagerNode), out var setterRegis)
            .Should().BeTrue();
        setterRegis.Setter.Should().BeTrue();
        setterRegis.Registrations.Select(x => x.GetterType).Should().Equal(
            typeof(IStoryManagerBranchNodeGetter),
            typeof(IStoryManagerQuestNodeGetter),
            typeof(IStoryManagerEventNodeGetter));
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(typeof(IAStoryManagerNodeGetter), out var getterRegis)
            .Should().BeTrue();
        getterRegis.Setter.Should().BeFalse();
        getterRegis.Registrations.Select(x => x.GetterType).Should().Equal(
            typeof(IStoryManagerBranchNodeGetter),
            typeof(IStoryManagerQuestNodeGetter),
            typeof(IStoryManagerEventNodeGetter));
    }
    
    [Fact]
    internal void AspectInterfaceByCategory()
    {
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(GameCategory.Skyrim, typeof(IEnchantable), out var regis)
            .Should().BeTrue();
        regis.Setter.Should().BeTrue();
        regis.Registrations.Select(x => x.GetterType).Should().Equal(
            typeof(IArmorGetter),
            typeof(IWeaponGetter));
    }
    
    [Fact]
    internal void AspectInterfaceByCategoryMiss()
    {
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(GameCategory.Oblivion, typeof(IEnchantable), out var regis)
            .Should().BeFalse();
    }
    
    [Fact]
    internal void AspectInterfaceName()
    {
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface("Mutagen.Bethesda.Skyrim.IEnchantable", out var setterRegis)
            .Should().BeTrue();
        setterRegis.Setter.Should().BeTrue();
        setterRegis.Registrations.Select(x => x.GetterType).Should().Equal(
            typeof(IArmorGetter),
            typeof(IWeaponGetter));
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface("Mutagen.Bethesda.Skyrim.IEnchantableGetter", out var getterRegis)
            .Should().BeTrue();
        getterRegis.Setter.Should().BeFalse();
        getterRegis.Registrations.Select(x => x.GetterType).Should().Equal(
            typeof(IArmorGetter),
            typeof(IWeaponGetter));
    }
    
    [Fact]
    internal void AspectInterfaceNameByCategory()
    {
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(GameCategory.Skyrim, "Mutagen.Bethesda.Skyrim.IEnchantable", out var setterRegis)
            .Should().BeTrue();
        setterRegis.Setter.Should().BeTrue();
        setterRegis.Registrations.Select(x => x.GetterType).Should().Equal(
            typeof(IArmorGetter),
            typeof(IWeaponGetter));
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(GameCategory.Skyrim, "Mutagen.Bethesda.Skyrim.IEnchantableGetter", out var getterRegis)
            .Should().BeTrue();
        getterRegis.Setter.Should().BeFalse();
        getterRegis.Registrations.Select(x => x.GetterType).Should().Equal(
            typeof(IArmorGetter),
            typeof(IWeaponGetter));
    }
    
    [Fact]
    internal void AspectInterfaceNameByCategoryMiss()
    {
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(GameCategory.Oblivion, "Mutagen.Bethesda.Skyrim.IEnchantable", out _)
            .Should().BeFalse();
        MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(GameCategory.Oblivion, "Mutagen.Bethesda.Skyrim.IEnchantableGetter", out _)
            .Should().BeFalse();
    }
}