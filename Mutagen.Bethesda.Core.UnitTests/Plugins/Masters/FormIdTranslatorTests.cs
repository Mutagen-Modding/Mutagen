using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Masters;

public class FormIdTranslatorTests
{
    [Theory, MutagenAutoData]
    internal void GetFormIDTypicalMasters(
        ModKey originating,
        ModKey modKeyA,
        ModKey modKeyB)
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
        var masterPackage = SeparatedMasterPackage.NotSeparate(coll);

        masterPackage.GetFormID(new FormKey(modKeyA, 0x123))
            .ShouldBe(new FormID(0x00000123));
        masterPackage.GetFormID(new FormKey(modKeyB, 0x456))
            .ShouldBe(new FormID(0x01000456));
        masterPackage.GetFormID(new FormKey(originating, 0x789))
            .ShouldBe(new FormID(0x02000789));
    }

    [Theory, MutagenAutoData]
    internal void GetFormKeyTypicalMasters(
        ModKey originating,
        ModKey modKeyA,
        ModKey modKeyB)
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
        var masterPackage = SeparatedMasterPackage.NotSeparate(coll);
        masterPackage.GetFormKey(new FormID(0x00123456), reference: true)
            .ShouldBe(new FormKey(modKeyA, 0x123456));
        masterPackage.GetFormKey(new FormID(0x00123456), reference: false)
            .ShouldBe(new FormKey(modKeyA, 0x123456));
        masterPackage.GetFormKey(new FormID(0x01123456), reference: true)
            .ShouldBe(new FormKey(modKeyB, 0x123456));
        masterPackage.GetFormKey(new FormID(0x01123456), reference: false)
            .ShouldBe(new FormKey(modKeyB, 0x123456));
        masterPackage.GetFormKey(new FormID(0x02123456), reference: true)
            .ShouldBe(new FormKey(originating, 0x123456));
        masterPackage.GetFormKey(new FormID(0x02123456), reference: false)
            .ShouldBe(new FormKey(originating, 0x123456));
    }

    [Theory, MutagenAutoData]
    internal void GetFormIDNoMasters(
        ModKey originating)
    {
        var coll = new MasterReferenceCollection(originating);
        var masterPackage = SeparatedMasterPackage.NotSeparate(coll);

        masterPackage.GetFormID(new FormKey(originating, 0x789))
            .ShouldBe(new FormID(0x00000789));
    }

    [Theory, MutagenAutoData]
    internal void GetFormKeyNoMasters(
        ModKey originating)
    {
        var coll = new MasterReferenceCollection(originating);
        var masterPackage = SeparatedMasterPackage.NotSeparate(coll);

        masterPackage.GetFormKey(new FormID(0x00123456), reference: true)
            .ShouldBe(new FormKey(originating, 0x123456));
        masterPackage.GetFormKey(new FormID(0x00123456), reference: false)
            .ShouldBe(new FormKey(originating, 0x123456));
    }

    [Theory, MutagenAutoData]
    internal void GetFormIDTypicalSeparateMasters(
        ModKey originating,
        ModKey modA,
        ModKey modB,
        ModKey lightA,
        ModKey lightB,
        ModKey mediumA,
        ModKey mediumB)
    {
        var coll = new MasterReferenceCollection(originating);
        coll.SetTo(new []
        {
            new MasterReference()
            {
                Master = modA
            },
            new MasterReference()
            {
                Master = lightA
            },
            new MasterReference()
            {
                Master = mediumA
            },
            new MasterReference()
            {
                Master = modB
            },
            new MasterReference()
            {
                Master = lightB
            },
            new MasterReference()
            {
                Master = mediumB
            },
        });
        var lo = new LoadOrder<IModFlagsGetter>();
        var orig = MastersTestUtil.GetFlags(originating, MasterStyle.Full);
        lo.Add(MastersTestUtil.GetFlags(modA, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Small));
        lo.Add(MastersTestUtil.GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(MastersTestUtil.GetFlags(modB, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightB, MasterStyle.Small));
        lo.Add(MastersTestUtil.GetFlags(mediumB, MasterStyle.Medium));
        lo.Add(orig);
        var masterPackage = SeparatedMasterPackage.Separate(orig.ModKey, MasterStyle.Full, coll, lo);

        masterPackage.GetFormID(new FormKey(modA, 0x123))
            .ShouldBe(new FormID(0x00000123));
        masterPackage.GetFormID(new FormKey(modB, 0x456))
            .ShouldBe(new FormID(0x01000456));
        masterPackage.GetFormID(new FormKey(originating, 0x789))
            .ShouldBe(new FormID(0x02000789));
        masterPackage.GetFormID(new FormKey(lightA, 0x123))
            .ShouldBe(new FormID(0xFE000123));
        masterPackage.GetFormID(new FormKey(lightB, 0x123))
            .ShouldBe(new FormID(0xFE001123));
        masterPackage.GetFormID(new FormKey(mediumA, 0x1234))
            .ShouldBe(new FormID(0xFD001234));
        masterPackage.GetFormID(new FormKey(mediumB, 0x1234))
            .ShouldBe(new FormID(0xFD011234));
    }

    [Theory, MutagenAutoData]
    internal void GetFormKeyTypicalSeparateMasters(
        ModKey originating,
        ModKey modA,
        ModKey modB,
        ModKey lightA,
        ModKey lightB,
        ModKey mediumA,
        ModKey mediumB)
    {
        var coll = new MasterReferenceCollection(originating);
        coll.SetTo(new []
        {
            new MasterReference()
            {
                Master = modA
            },
            new MasterReference()
            {
                Master = lightA
            },
            new MasterReference()
            {
                Master = mediumA
            },
            new MasterReference()
            {
                Master = modB
            },
            new MasterReference()
            {
                Master = lightB
            },
            new MasterReference()
            {
                Master = mediumB
            },
        });
        var lo = new LoadOrder<IModFlagsGetter>();
        var orig = MastersTestUtil.GetFlags(originating, MasterStyle.Full);
        lo.Add(MastersTestUtil.GetFlags(modA, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Small));
        lo.Add(MastersTestUtil.GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(MastersTestUtil.GetFlags(modB, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightB, MasterStyle.Small));
        lo.Add(MastersTestUtil.GetFlags(mediumB, MasterStyle.Medium));
        lo.Add(orig);
        var masterPackage = SeparatedMasterPackage.Separate(orig.ModKey, MasterStyle.Full, coll, lo);

        masterPackage.GetFormKey(new FormID(0x00123456), reference: true)
            .ShouldBe(new FormKey(modA, 0x123456));
        masterPackage.GetFormKey(new FormID(0x00123456), reference: false)
            .ShouldBe(new FormKey(modA, 0x123456));
        masterPackage.GetFormKey(new FormID(0xFE000123), reference: true)
            .ShouldBe(new FormKey(lightA, 0x123));
        masterPackage.GetFormKey(new FormID(0xFE000123), reference: false)
            .ShouldBe(new FormKey(lightA, 0x123));
        masterPackage.GetFormKey(new FormID(0xFD001234), reference: true)
            .ShouldBe(new FormKey(mediumA, 0x1234));
        masterPackage.GetFormKey(new FormID(0xFD001234), reference: false)
            .ShouldBe(new FormKey(mediumA, 0x1234));
        masterPackage.GetFormKey(new FormID(0x01123456), reference: true)
            .ShouldBe(new FormKey(modB, 0x123456));
        masterPackage.GetFormKey(new FormID(0x01123456), reference: false)
            .ShouldBe(new FormKey(modB, 0x123456));
        masterPackage.GetFormKey(new FormID(0xFE001123), reference: true)
            .ShouldBe(new FormKey(lightB, 0x123));
        masterPackage.GetFormKey(new FormID(0xFE001123), reference: false)
            .ShouldBe(new FormKey(lightB, 0x123));
        masterPackage.GetFormKey(new FormID(0xFD011234), reference: true)
            .ShouldBe(new FormKey(mediumB, 0x1234));
        masterPackage.GetFormKey(new FormID(0xFD011234), reference: false)
            .ShouldBe(new FormKey(mediumB, 0x1234));
        masterPackage.GetFormKey(new FormID(0x02123456), reference: true)
            .ShouldBe(new FormKey(originating, 0x123456));
        masterPackage.GetFormKey(new FormID(0x02123456), reference: false)
            .ShouldBe(new FormKey(originating, 0x123456));
    }

    [Theory, MutagenAutoData]
    internal void SeparateMastersOriginatingLight(
        ModKey originating,
        ModKey modA,
        ModKey lightA)
    {
        var coll = new MasterReferenceCollection(originating);
        coll.SetTo(new []
        {
            new MasterReference()
            {
                Master = modA
            },
            new MasterReference()
            {
                Master = lightA
            },
        });
        var lo = new LoadOrder<IModFlagsGetter>();
        var orig = MastersTestUtil.GetFlags(originating, MasterStyle.Small);
        lo.Add(MastersTestUtil.GetFlags(modA, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Small));
        lo.Add(orig);
        var masterPackage = SeparatedMasterPackage.Separate(orig.ModKey, MasterStyle.Small, coll, lo);

        masterPackage.GetFormID(new FormKey(modA, 0x123))
            .ShouldBe(new FormID(0x00000123));
        masterPackage.GetFormID(new FormKey(lightA, 0x123))
            .ShouldBe(new FormID(0xFE000123));
        masterPackage.GetFormID(new FormKey(originating, 0x123))
            .ShouldBe(new FormID(0x01000123));
    }

    [Theory, MutagenAutoData]
    internal void SeparateMastersOriginatingMedium(
        ModKey originating,
        ModKey modA,
        ModKey mediumA)
    {
        var coll = new MasterReferenceCollection(originating);
        coll.SetTo(new []
        {
            new MasterReference()
            {
                Master = modA
            },
            new MasterReference()
            {
                Master = mediumA
            },
        });
        var orig = MastersTestUtil.GetFlags(originating, MasterStyle.Medium);
        var lo = new LoadOrder<IModFlagsGetter>();
        lo.Add(MastersTestUtil.GetFlags(modA, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(orig);
        var masterPackage = SeparatedMasterPackage.Separate(orig.ModKey, MasterStyle.Medium, coll, lo);

        masterPackage.GetFormID(new FormKey(modA, 0x123))
            .ShouldBe(new FormID(0x00000123));
        masterPackage.GetFormID(new FormKey(mediumA, 0x1234))
            .ShouldBe(new FormID(0xFD001234));
        masterPackage.GetFormID(new FormKey(originating, 0x1234))
            .ShouldBe(new FormID(0x01001234));
    }
}
