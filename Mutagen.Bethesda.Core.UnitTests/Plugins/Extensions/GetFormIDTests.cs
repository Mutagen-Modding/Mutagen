using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Extensions;

public class GetFormIDTests
{
    private static IModGetter CreateModGetter(
        ModKey modKey,
        GameRelease release,
        MasterStyle style,
        params ModKey[] masters)
    {
        var mod = Substitute.For<IModGetter>();
        mod.ModKey.Returns(modKey);
        mod.GameRelease.Returns(release);
        mod.MasterReferences.Returns(
            masters.Select(m => (IMasterReferenceGetter)new MasterReference { Master = m }).ToList());
        mod.CanBeSmallMaster.Returns(style == MasterStyle.Small);
        mod.IsSmallMaster.Returns(style == MasterStyle.Small);
        mod.CanBeMediumMaster.Returns(style == MasterStyle.Medium);
        mod.IsMediumMaster.Returns(style == MasterStyle.Medium);
        mod.IsMaster.Returns(true);
        return mod;
    }

    [Theory, MutagenAutoData]
    public void NullFormKeyReturnsNullFormID(
        ModKey originating)
    {
        var mod = CreateModGetter(originating, GameRelease.SkyrimSE, MasterStyle.Full);

        mod.GetFormID(FormKey.Null).ShouldBe(FormID.Null);
    }

    [Theory, MutagenAutoData]
    public void FirstMasterGetsIndexZero(
        ModKey originating,
        ModKey masterA)
    {
        var mod = CreateModGetter(originating, GameRelease.SkyrimSE, MasterStyle.Full, masterA);

        mod.GetFormID(new FormKey(masterA, 0x123))
            .ShouldBe(new FormID(0x00000123));
    }

    [Theory, MutagenAutoData]
    public void SecondMasterGetsIndexOne(
        ModKey originating,
        ModKey masterA,
        ModKey masterB)
    {
        var mod = CreateModGetter(originating, GameRelease.SkyrimSE, MasterStyle.Full, masterA, masterB);

        mod.GetFormID(new FormKey(masterB, 0x456))
            .ShouldBe(new FormID(0x01000456));
    }

    [Theory, MutagenAutoData]
    public void OriginatingModGetsLastIndex(
        ModKey originating,
        ModKey masterA,
        ModKey masterB)
    {
        var mod = CreateModGetter(originating, GameRelease.SkyrimSE, MasterStyle.Full, masterA, masterB);

        mod.GetFormID(new FormKey(originating, 0x789))
            .ShouldBe(new FormID(0x02000789));
    }

    [Theory, MutagenAutoData]
    public void NoMastersOriginatingGetsIndexZero(
        ModKey originating)
    {
        var mod = CreateModGetter(originating, GameRelease.SkyrimSE, MasterStyle.Full);

        mod.GetFormID(new FormKey(originating, 0x789))
            .ShouldBe(new FormID(0x00000789));
    }

    [Theory, MutagenAutoData]
    public void UnknownModKeyThrows(
        ModKey originating,
        ModKey masterA,
        ModKey unknown)
    {
        var mod = CreateModGetter(originating, GameRelease.SkyrimSE, MasterStyle.Full, masterA);

        Should.Throw<UnmappableFormIDException>(() =>
            mod.GetFormID(new FormKey(unknown, 0x123)));
    }

    [Theory, MutagenAutoData]
    public void SeparatedMastersWithLookup(
        ModKey originating,
        ModKey fullMaster,
        ModKey lightMaster,
        ModKey mediumMaster)
    {
        var mod = CreateModGetter(
            originating, GameRelease.Starfield, MasterStyle.Full,
            fullMaster, lightMaster, mediumMaster);

        var masterFlagLookup = new Cache<IModMasterStyledGetter, ModKey>(x => x.ModKey);
        masterFlagLookup.Add(new KeyedMasterStyle(fullMaster, MasterStyle.Full));
        masterFlagLookup.Add(new KeyedMasterStyle(lightMaster, MasterStyle.Small));
        masterFlagLookup.Add(new KeyedMasterStyle(mediumMaster, MasterStyle.Medium));

        // Full master at index 0
        mod.GetFormID(new FormKey(fullMaster, 0x123), masterFlagLookup)
            .ShouldBe(new FormID(0x00000123));

        // Light master at index 0 in the small list
        mod.GetFormID(new FormKey(lightMaster, 0x123), masterFlagLookup)
            .ShouldBe(new FormID(0xFE000123));

        // Medium master at index 0 in the medium list
        mod.GetFormID(new FormKey(mediumMaster, 0x1234), masterFlagLookup)
            .ShouldBe(new FormID(0xFD001234));

        // Originating mod goes into the full list after fullMaster
        mod.GetFormID(new FormKey(originating, 0x789), masterFlagLookup)
            .ShouldBe(new FormID(0x01000789));
    }
}
