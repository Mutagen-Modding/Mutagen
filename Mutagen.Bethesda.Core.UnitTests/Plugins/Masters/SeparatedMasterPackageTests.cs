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

        package.TryLookupModKey(originating, out var origResult).Should().BeTrue();
        origResult!.Index.ID.Should().Be(2);
        origResult.Style.Should().Be(MasterStyle.Normal);
        package.TryLookupModKey(modA, out var aResult).Should().BeTrue();
        aResult!.Index.ID.Should().Be(0);
        aResult.Style.Should().Be(MasterStyle.Normal);
        package.TryLookupModKey(modB, out var bResult).Should().BeTrue();
        bResult!.Index.ID.Should().Be(1);
        bResult.Style.Should().Be(MasterStyle.Normal);
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

        package.GetFormKey(0x00123456).Should().Be(new FormKey(modA, 0x123456));
        package.GetFormKey(0x01123456).Should().Be(new FormKey(modB, 0x123456));
        package.GetFormKey(0x02123456).Should().Be(new FormKey(originating, 0x123456));
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

        var orig = MastersTestUtil.GetFlags(originating, MasterStyle.Normal);
        var lo = new LoadOrder<IModFlagsGetter>();
        lo.Add(MastersTestUtil.GetFlags(modA, MasterStyle.Normal));
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(MastersTestUtil.GetFlags(modB, MasterStyle.Normal));
        lo.Add(MastersTestUtil.GetFlags(lightB, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumB, MasterStyle.Medium));
        lo.Add(orig);

        var package = SeparatedMasterPackage.Separate(orig.ModKey, masterColl, lo);

        package.TryLookupModKey(originating, out var origResult).Should().BeTrue();
        origResult!.Index.ID.Should().Be(2);
        origResult.Style.Should().Be(MasterStyle.Normal);
        package.TryLookupModKey(modA, out var aResult).Should().BeTrue();
        aResult!.Index.ID.Should().Be(0);
        aResult.Style.Should().Be(MasterStyle.Normal);
        package.TryLookupModKey(modB, out var bResult).Should().BeTrue();
        bResult!.Index.ID.Should().Be(1);
        bResult.Style.Should().Be(MasterStyle.Normal);
        package.TryLookupModKey(lightA, out var lightAResult).Should().BeTrue();
        lightAResult!.Index.ID.Should().Be(0);
        lightAResult.Style.Should().Be(MasterStyle.Light);
        package.TryLookupModKey(lightB, out var lightBResult).Should().BeTrue();
        lightBResult!.Index.ID.Should().Be(1);
        lightBResult.Style.Should().Be(MasterStyle.Light);
        package.TryLookupModKey(mediumA, out var mediumAResult).Should().BeTrue();
        mediumAResult!.Index.ID.Should().Be(0);
        mediumAResult.Style.Should().Be(MasterStyle.Medium);
        package.TryLookupModKey(mediumB, out var mediumBResult).Should().BeTrue();
        mediumBResult!.Index.ID.Should().Be(1);
        mediumBResult.Style.Should().Be(MasterStyle.Medium);
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

        var orig = MastersTestUtil.GetFlags(originating, MasterStyle.Normal);
        var lo = new LoadOrder<IModFlagsGetter>();
        lo.Add(MastersTestUtil.GetFlags(modA, MasterStyle.Normal));
        lo.Add(MastersTestUtil.GetFlags(lightA, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(MastersTestUtil.GetFlags(modB, MasterStyle.Normal));
        lo.Add(MastersTestUtil.GetFlags(lightB, MasterStyle.Light));
        lo.Add(MastersTestUtil.GetFlags(mediumB, MasterStyle.Medium));
        lo.Add(orig);

        var package = SeparatedMasterPackage.Separate(orig.ModKey, masterColl, lo);

        package.GetFormKey(0x00123456).Should().Be(new FormKey(modA, 0x123456));
        package.GetFormKey(0xFE000123).Should().Be(new FormKey(lightA, 0x123));
        package.GetFormKey(0xFD001234).Should().Be(new FormKey(mediumA, 0x1234));
        package.GetFormKey(0x01123456).Should().Be(new FormKey(modB, 0x123456));
        package.GetFormKey(0xFE001123).Should().Be(new FormKey(lightB, 0x123));
        package.GetFormKey(0xFD011234).Should().Be(new FormKey(mediumB, 0x1234));
        package.GetFormKey(0x02123456).Should().Be(new FormKey(originating, 0x123456));
    }
}