using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Masters.DI;
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
        ModKey modKeyB,
        Type recordType)
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

        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(modKeyA, 0x123), recordType),
                reference: true)
            .Should().Be(
                new FormID(0x00000123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(modKeyA, 0x123), recordType),
                reference: false)
            .Should().Be(
                new FormID(0x00000123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(modKeyB, 0x456), recordType),
                reference: true)
            .Should().Be(
                new FormID(0x01000456));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(modKeyB, 0x456), recordType),
                reference: false)
            .Should().Be(
                new FormID(0x01000456));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(originating, 0x789), recordType),
                reference: true)
            .Should().Be(
                new FormID(0x02000789));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(originating, 0x789), recordType),
                reference: false)
            .Should().Be(
                new FormID(0x02000789));
    }
    
    [Theory, MutagenAutoData]
    internal void GetFormKeyTypicalMasters(
        ModKey originating,
        ModKey modKeyA,
        ModKey modKeyB,
        Type recordType)
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
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x00123456), reference: true)
            .Should().Be(new FormKey(modKeyA, 0x123456));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x00123456), reference: false)
            .Should().Be(new FormKey(modKeyA, 0x123456));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x01123456), reference: true)
            .Should().Be(new FormKey(modKeyB, 0x123456));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x01123456), reference: false)
            .Should().Be(new FormKey(modKeyB, 0x123456));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x02123456), reference: true)
            .Should().Be(new FormKey(originating, 0x123456));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x02123456), reference: false)
            .Should().Be(new FormKey(originating, 0x123456));
    }
    
    [Theory, MutagenAutoData]
    internal void GetFormIDNoMasters(
        ModKey originating,
        Type recordType)
    {
        var coll = new MasterReferenceCollection(originating);
        var masterPackage = SeparatedMasterPackage.NotSeparate(coll);

        FormIDTranslator.GetFormID(
                masterPackage,
                new FormLinkInformation(
                    new FormKey(originating, 0x789), recordType),
                reference: true)
            .Should().Be(
                new FormID(0x00000789));
        FormIDTranslator.GetFormID(
                masterPackage,
                new FormLinkInformation(
                    new FormKey(originating, 0x789), recordType),
                reference: false)
            .Should().Be(
                new FormID(0x00000789));
    }
    
    [Theory, MutagenAutoData]
    internal void GetFormKeyNoMasters(
        ModKey originating,
        Type recordType)
    {
        var coll = new MasterReferenceCollection(originating);
        var masterPackage = SeparatedMasterPackage.NotSeparate(coll);

        FormIDTranslator.GetFormKey(
                masterPackage,
                new FormID(0x00123456), 
                reference: true)
            .Should().Be(new FormKey(originating, 0x123456));
        FormIDTranslator.GetFormKey(
                masterPackage,
                new FormID(0x00123456), 
                reference: false)
            .Should().Be(new FormKey(originating, 0x123456));
    }
    
    [Theory, MutagenAutoData]
    internal void GetFormIDTypicalSeparateMasters(
        ModKey originating,
        ModKey modA,
        ModKey modB,
        ModKey lightA,
        ModKey lightB,
        ModKey mediumA,
        ModKey mediumB,
        Type recordType)
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
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(MastersTestUtil.GetFlags(modB, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightB, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumB, MasterStyle.Medium));
        lo.Add(orig);
        var masterPackage = SeparatedMasterPackage.Separate(orig.ModKey, MasterStyle.Full, coll, lo);

        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(modA, 0x123), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0x00000123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(modB, 0x456), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0x01000456));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(originating, 0x789), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0x02000789));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(lightA, 0x123), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0xFE000123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(lightB, 0x123), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0xFE001123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(mediumA, 0x1234), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0xFD001234));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(mediumB, 0x1234), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0xFD011234));
    }
    
    [Theory, MutagenAutoData]
    internal void GetFormKeyTypicalSeparateMasters(
        ModKey originating,
        ModKey modA,
        ModKey modB,
        ModKey lightA,
        ModKey lightB,
        ModKey mediumA,
        ModKey mediumB,
        Type recordType)
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
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(MastersTestUtil.GetFlags(modB, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightB, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumB, MasterStyle.Medium));
        lo.Add(orig);
        var masterPackage = SeparatedMasterPackage.Separate(orig.ModKey, MasterStyle.Full, coll, lo);

        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x00123456), 
                reference: true)
            .Should().Be(new FormKey(modA, 0x123456));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x00123456), 
                reference: false)
            .Should().Be(new FormKey(modA, 0x123456));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0xFE000123), 
                reference: true)
            .Should().Be(new FormKey(lightA, 0x123));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0xFE000123), 
                reference: false)
            .Should().Be(new FormKey(lightA, 0x123));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0xFD001234), 
                reference: true)
            .Should().Be(new FormKey(mediumA, 0x1234));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0xFD001234), 
                reference: false)
            .Should().Be(new FormKey(mediumA, 0x1234));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x01123456), 
                reference: true)
            .Should().Be(new FormKey(modB, 0x123456));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x01123456), 
                reference: false)
            .Should().Be(new FormKey(modB, 0x123456));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0xFE001123), 
                reference: true)
            .Should().Be(new FormKey(lightB, 0x123));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0xFE001123), 
                reference: false)
            .Should().Be(new FormKey(lightB, 0x123));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0xFD011234), 
                reference: true)
            .Should().Be(new FormKey(mediumB, 0x1234));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0xFD011234), 
                reference: false)
            .Should().Be(new FormKey(mediumB, 0x1234));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x02123456), 
                reference: true)
            .Should().Be(new FormKey(originating, 0x123456));
        FormIDTranslator.GetFormKey(masterPackage, new FormID(0x02123456), 
                reference: false)
            .Should().Be(new FormKey(originating, 0x123456));
    }
    
    [Theory, MutagenAutoData]
    internal void SeparateMastersOriginatingLight(
        ModKey originating,
        ModKey modA,
        ModKey lightA,
        Type recordType)
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
        var orig = MastersTestUtil.GetFlags(originating, MasterStyle.Light);
        lo.Add(MastersTestUtil.GetFlags(modA, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Light));
        lo.Add(orig);
        var masterPackage = SeparatedMasterPackage.Separate(orig.ModKey, MasterStyle.Light, coll, lo);

        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(modA, 0x123), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0x00000123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(modA, 0x123), recordType), 
                reference: false)
            .Should().Be(
                new FormID(0x00000123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(lightA, 0x123), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0xFE000123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(lightA, 0x123), recordType), 
                reference: false)
            .Should().Be(
                new FormID(0xFE000123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(originating, 0x123), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0x01000123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(originating, 0x123), recordType), 
                reference: false)
            .Should().Be(
                new FormID(0x01000123));
    }
    
    [Theory, MutagenAutoData]
    internal void SeparateMastersOriginatingMedium(
        ModKey originating,
        ModKey modA,
        ModKey mediumA,
        Type recordType)
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

        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(modA, 0x123), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0x00000123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(modA, 0x123), recordType), 
                reference: false)
            .Should().Be(
                new FormID(0x00000123));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(mediumA, 0x1234), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0xFD001234));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(mediumA, 0x1234), recordType), 
                reference: false)
            .Should().Be(
                new FormID(0xFD001234));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(originating, 0x1234), recordType), 
                reference: true)
            .Should().Be(
                new FormID(0x01001234));
        FormIDTranslator.GetFormID(masterPackage,
                new FormLinkInformation(
                    new FormKey(originating, 0x1234), recordType), 
                reference: false)
            .Should().Be(
                new FormID(0x01001234));
    }
}