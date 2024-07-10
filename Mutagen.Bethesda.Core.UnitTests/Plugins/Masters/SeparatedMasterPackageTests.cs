using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing.AutoData;
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

        package.TryLookupModKey(originating, out var style, out var index).Should().BeTrue();
        index.Should().Be(2);
        style.Should().Be(MasterStyle.Full);
        package.TryLookupModKey(modA, out var aStyle, out var aIndex).Should().BeTrue();
        aIndex.Should().Be(0);
        aStyle.Should().Be(MasterStyle.Full);
        package.TryLookupModKey(modB, out var bStyle, out var bIndex).Should().BeTrue();
        bIndex.Should().Be(1);
        bStyle.Should().Be(MasterStyle.Full);
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

        package.GetFormKey(new FormID(0x00123456)).Should().Be(new FormKey(modA, 0x123456));
        package.GetFormKey(new FormID(0x01123456)).Should().Be(new FormKey(modB, 0x123456));
        package.GetFormKey(new FormID(0x02123456)).Should().Be(new FormKey(originating, 0x123456));
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
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(MastersTestUtil.GetFlags(modB, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightB, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumB, MasterStyle.Medium));
        lo.Add(orig);

        var package = SeparatedMasterPackage.Separate(orig.ModKey, MasterStyle.Full, masterColl, lo);

        package.TryLookupModKey(originating, out var origStyle, out var origIndex).Should().BeTrue();
        origIndex.Should().Be(2);
        origStyle.Should().Be(MasterStyle.Full);
        package.TryLookupModKey(modA, out var aStyle, out var aIndex).Should().BeTrue();
        aIndex.Should().Be(0);
        aStyle.Should().Be(MasterStyle.Full);
        package.TryLookupModKey(modB, out var bStyle, out var bIndex).Should().BeTrue();
        bIndex.Should().Be(1);
        bStyle.Should().Be(MasterStyle.Full);
        package.TryLookupModKey(lightA, out var lightAStyle, out var lightAIndex).Should().BeTrue();
        lightAIndex.Should().Be(0);
        lightAStyle.Should().Be(MasterStyle.Light);
        package.TryLookupModKey(lightB, out var lightBStyle, out var lightBIndex).Should().BeTrue();
        lightBIndex.Should().Be(1);
        lightBStyle.Should().Be(MasterStyle.Light);
        package.TryLookupModKey(mediumA, out var mediumAStyle, out var mediumAIndex).Should().BeTrue();
        mediumAIndex.Should().Be(0);
        mediumAStyle.Should().Be(MasterStyle.Medium);
        package.TryLookupModKey(mediumB, out var mediumBStyle, out var mediumBIndex).Should().BeTrue();
        mediumBIndex.Should().Be(1);
        mediumBStyle.Should().Be(MasterStyle.Medium);
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
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(MastersTestUtil.GetFlags(modB, MasterStyle.Full));
        lo.Add(MastersTestUtil.GetFlags(lightB, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumB, MasterStyle.Medium));
        lo.Add(orig);

        var package = SeparatedMasterPackage.Separate(orig.ModKey, MasterStyle.Full, masterColl, lo);

        package.GetFormKey(new FormID(0x00123456)).Should().Be(new FormKey(modA, 0x123456));
        package.GetFormKey(new FormID(0xFE000123)).Should().Be(new FormKey(lightA, 0x123));
        package.GetFormKey(new FormID(0xFD001234)).Should().Be(new FormKey(mediumA, 0x1234));
        package.GetFormKey(new FormID(0x01123456)).Should().Be(new FormKey(modB, 0x123456));
        package.GetFormKey(new FormID(0xFE001123)).Should().Be(new FormKey(lightB, 0x123));
        package.GetFormKey(new FormID(0xFD011234)).Should().Be(new FormKey(mediumB, 0x1234));
        package.GetFormKey(new FormID(0x02123456)).Should().Be(new FormKey(originating, 0x123456));
    }
}