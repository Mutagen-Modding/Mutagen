using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Masters;

public class SeparatedMasterPackageTests
{
    [Theory, MutagenAutoData]
    public void TryLookupModKeyNormal(
        ModKey originating,
        ModKey modA,
        ModKey modB)
    {
        var masterColl = new MasterReferenceCollection(originating);
        masterColl.SetTo(new []
        {
            new MasterReference()
            {
                Master = modA,
            },
            new MasterReference()
            {
                Master = modB,
            }
        });

        var package = SeparatedMasterPackage.NotSeparate(masterColl);

        package.TryLookupModKey(originating, reference: true, out var style, out var index).ShouldBeTrue();
        index.ShouldEqual(2);
        style.ShouldBe(MasterStyle.Full);
        package.TryLookupModKey(modA, reference: true, out var aStyle, out var aIndex).ShouldBeTrue();
        aIndex.ShouldEqual(0);
        aStyle.ShouldBe(MasterStyle.Full);
        package.TryLookupModKey(modB, reference: true, out var bStyle, out var bIndex).ShouldBeTrue();
        bIndex.ShouldEqual(1);
        bStyle.ShouldBe(MasterStyle.Full);
    }
    
    [Theory, MutagenAutoData]
    public void GetLoadOrderNormal(
        ModKey originating,
        ModKey modA,
        ModKey modB)
    {
        var masterColl = new MasterReferenceCollection(originating);
        masterColl.SetTo(new []
        {
            new MasterReference()
            {
                Master = modA,
            },
            new MasterReference()
            {
                Master = modB,
            }
        });

        var package = SeparatedMasterPackage.NotSeparate(masterColl);

        package.GetFormKey(new FormID(0x00123456), reference: true).ShouldBe(new FormKey(modA, 0x123456));
        package.GetFormKey(new FormID(0x01123456), reference: true).ShouldBe(new FormKey(modB, 0x123456));
        package.GetFormKey(new FormID(0x02123456), reference: true).ShouldBe(new FormKey(originating, 0x123456));
    }
    
    [Theory, MutagenAutoData]
    public void TryLookupModKeySeparated(
        ModKey originating,
        ModKey modA,
        ModKey modB,
        ModKey lightA,
        ModKey lightB,
        ModKey mediumA,
        ModKey mediumB)
    {
        var masterColl = new MasterReferenceCollection(originating);
        masterColl.SetTo(new []
        {
            new MasterReference()
            {
                Master = modA,
            },
            new MasterReference()
            {
                Master = lightA,
            },
            new MasterReference()
            {
                Master = mediumA,
            },
            new MasterReference()
            {
                Master = modB,
            },
            new MasterReference()
            {
                Master = lightB,
            },
            new MasterReference()
            {
                Master = mediumB,
            }
        });

        var orig = MastersTestUtil.GetFlags(originating, MasterStyle.Full);
        var lo = new LoadOrder<IModFlagsGetter>();
        lo.Add(MastersTestUtil.GetFlags(modA, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Small));
        lo.Add(MastersTestUtil.GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(MastersTestUtil.GetFlags(modB, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightB, MasterStyle.Small));
        lo.Add(MastersTestUtil.GetFlags(mediumB, MasterStyle.Medium));
        lo.Add(orig);

        var package = SeparatedMasterPackage.Separate(orig.ModKey, MasterStyle.Full, masterColl, lo);

        package.TryLookupModKey(originating, reference: true, out var origStyle, out var origIndex).ShouldBeTrue();
        origIndex.ShouldEqual(2);
        origStyle.ShouldBe(MasterStyle.Full);
        package.TryLookupModKey(modA, reference: true, out var aStyle, out var aIndex).ShouldBeTrue();
        aIndex.ShouldEqual(0);
        aStyle.ShouldBe(MasterStyle.Full);
        package.TryLookupModKey(modB, reference: true, out var bStyle, out var bIndex).ShouldBeTrue();
        bIndex.ShouldEqual(1);
        bStyle.ShouldBe(MasterStyle.Full);
        package.TryLookupModKey(lightA, reference: true, out var lightAStyle, out var lightAIndex).ShouldBeTrue();
        lightAIndex.ShouldEqual(0);
        lightAStyle.ShouldBe(MasterStyle.Small);
        package.TryLookupModKey(lightB, reference: true, out var lightBStyle, out var lightBIndex).ShouldBeTrue();
        lightBIndex.ShouldEqual(1);
        lightBStyle.ShouldBe(MasterStyle.Small);
        package.TryLookupModKey(mediumA, reference: true, out var mediumAStyle, out var mediumAIndex).ShouldBeTrue();
        mediumAIndex.ShouldEqual(0);
        mediumAStyle.ShouldBe(MasterStyle.Medium);
        package.TryLookupModKey(mediumB, reference: true, out var mediumBStyle, out var mediumBIndex).ShouldBeTrue();
        mediumBIndex.ShouldEqual(1);
        mediumBStyle.ShouldBe(MasterStyle.Medium);
    }
    
    [Theory, MutagenAutoData]
    public void GetLoadOrderSeparated(
        ModKey originating,
        ModKey modA,
        ModKey modB,
        ModKey lightA,
        ModKey lightB,
        ModKey mediumA,
        ModKey mediumB)
    {
        var masterColl = new MasterReferenceCollection(originating);
        masterColl.SetTo(new []
        {
            new MasterReference()
            {
                Master = modA,
            },
            new MasterReference()
            {
                Master = lightA,
            },
            new MasterReference()
            {
                Master = mediumA,
            },
            new MasterReference()
            {
                Master = modB,
            },
            new MasterReference()
            {
                Master = lightB,
            },
            new MasterReference()
            {
                Master = mediumB,
            }
        });

        var orig = MastersTestUtil.GetFlags(originating, MasterStyle.Full);
        var lo = new LoadOrder<IModFlagsGetter>();
        lo.Add(MastersTestUtil.GetFlags(modA, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Small));
        lo.Add(MastersTestUtil.GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(MastersTestUtil.GetFlags(modB, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightB, MasterStyle.Small));
        lo.Add(MastersTestUtil.GetFlags(mediumB, MasterStyle.Medium));
        lo.Add(orig);

        var package = SeparatedMasterPackage.Separate(orig.ModKey, MasterStyle.Full, masterColl, lo);

        package.GetFormKey(new FormID(0x00123456), reference: true).ShouldBe(new FormKey(modA, 0x123456));
        package.GetFormKey(new FormID(0xFE000123), reference: true).ShouldBe(new FormKey(lightA, 0x123));
        package.GetFormKey(new FormID(0xFD001234), reference: true).ShouldBe(new FormKey(mediumA, 0x1234));
        package.GetFormKey(new FormID(0x01123456), reference: true).ShouldBe(new FormKey(modB, 0x123456));
        package.GetFormKey(new FormID(0xFE001123), reference: true).ShouldBe(new FormKey(lightB, 0x123));
        package.GetFormKey(new FormID(0xFD011234), reference: true).ShouldBe(new FormKey(mediumB, 0x1234));
        package.GetFormKey(new FormID(0x02123456), reference: true).ShouldBe(new FormKey(originating, 0x123456));
    }
}