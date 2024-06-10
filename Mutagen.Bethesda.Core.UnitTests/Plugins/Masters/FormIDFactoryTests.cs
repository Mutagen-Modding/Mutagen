using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Masters.DI;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Masters;

public class FormIDFactoryTests
{
    [Theory, MutagenAutoData]
    public void TypicalMasters(
        ModKey originating,
        ModKey modKeyA,
        ModKey modKeyB,
        Type recordType,
        FormIDFactory sut)
    {
        var coll = new MasterReferenceCollection(originating);
        coll.SetTo(new []
        {
            new MasterReference()
            {
                Master = modKeyA
            },
            new MasterReference()
            {
                Master = modKeyB
            },
        });

        sut.GetFormID(
                GameConstants.SkyrimSE, coll, new FormLinkInformation(
                    new FormKey(modKeyA, 123), recordType))
            .Should().Be(
                new FormID(
                    new ModIndex(0),
                    123));
        sut.GetFormID(
                GameConstants.SkyrimSE, coll, new FormLinkInformation(
                    new FormKey(modKeyB, 456), recordType))
            .Should().Be(
                new FormID(
                    new ModIndex(1),
                    456));
        sut.GetFormID(
                GameConstants.SkyrimSE, coll, new FormLinkInformation(
                    new FormKey(originating, 789), recordType))
            .Should().Be(
                new FormID(
                    new ModIndex(2),
                    789));
    }
    
    [Theory, MutagenAutoData]
    public void NoMasters(
        ModKey originating,
        Type recordType,
        FormIDFactory sut)
    {
        var coll = new MasterReferenceCollection(originating);

        sut.GetFormID(
                GameConstants.SkyrimSE, coll, new FormLinkInformation(
                    new FormKey(originating, 789), recordType))
            .Should().Be(
                new FormID(
                    new ModIndex(0),
                    789));
    }
}