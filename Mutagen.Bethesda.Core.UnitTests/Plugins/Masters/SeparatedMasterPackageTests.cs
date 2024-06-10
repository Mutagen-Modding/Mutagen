using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing.AutoData;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Masters;

public class SeparatedMasterPackageTests
{
    [Theory, MutagenAutoData]
    public void Normal(
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

        var package = SeparatedMasterPackage.Factory(masterColl);
        package.Normal.Should().NotBeNull();
        package.Lookup.Should().NotBeNull();
        package.Light.Should().BeNull();
        package.Medium.Should().BeNull();

        package.Lookup.Count.Should().Be(3);
        package.Lookup.TryGetValue(originating, out var origResult).Should().BeTrue();
        origResult.Index.ID.Should().Be(2);
        origResult.Style.Should().Be(MasterStyle.Normal);
        package.Lookup.TryGetValue(modA, out var aResult).Should().BeTrue();
        aResult.Index.ID.Should().Be(0);
        aResult.Style.Should().Be(MasterStyle.Normal);
        package.Lookup.TryGetValue(modB, out var bResult).Should().BeTrue();
        bResult.Index.ID.Should().Be(1);
        bResult.Style.Should().Be(MasterStyle.Normal);
    }

    private IModFlagsGetter GetFlags(ModKey modKey, MasterStyle style)
    {
        var modGetter = Substitute.For<IModFlagsGetter>();
        modGetter.ModKey.Returns(modKey);
        modGetter.CanBeLightMaster.Returns(true);
        modGetter.CanBeHalfMaster.Returns(true);
        switch (style)
        {
            case MasterStyle.Normal:
                modGetter.IsLightMaster.Returns(false);
                modGetter.IsHalfMaster.Returns(false);
                break;
            case MasterStyle.Light:
                modGetter.IsLightMaster.Returns(true);
                modGetter.IsHalfMaster.Returns(false);
                break;
            case MasterStyle.Medium:
                modGetter.IsLightMaster.Returns(false);
                modGetter.IsHalfMaster.Returns(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }

        return modGetter;
    }
    
    [Theory, MutagenAutoData]
    public void Separated(
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

        var lo = new LoadOrder<IModFlagsGetter>();
        lo.Add(GetFlags(modA, MasterStyle.Normal));
        lo.Add(GetFlags(lightA, MasterStyle.Light));
        lo.Add(GetFlags(mediumA, MasterStyle.Medium));
        lo.Add(GetFlags(modB, MasterStyle.Normal));
        lo.Add(GetFlags(lightB, MasterStyle.Light));
        lo.Add(GetFlags(mediumB, MasterStyle.Medium));

        var package = SeparatedMasterPackage.Factory(masterColl, lo);
        package.Normal.Should().NotBeNull();
        package.Lookup.Should().NotBeNull();
        package.Light.Should().NotBeNull();
        package.Medium.Should().NotBeNull();

        package.Lookup.Count.Should().Be(7);
        package.Lookup.TryGetValue(originating, out var origResult).Should().BeTrue();
        origResult.Index.ID.Should().Be(2);
        origResult.Style.Should().Be(MasterStyle.Normal);
        package.Lookup.TryGetValue(modA, out var aResult).Should().BeTrue();
        aResult.Index.ID.Should().Be(0);
        aResult.Style.Should().Be(MasterStyle.Normal);
        package.Lookup.TryGetValue(modB, out var bResult).Should().BeTrue();
        bResult.Index.ID.Should().Be(1);
        bResult.Style.Should().Be(MasterStyle.Normal);
        package.Lookup.TryGetValue(lightA, out var lightAResult).Should().BeTrue();
        lightAResult.Index.ID.Should().Be(0);
        lightAResult.Style.Should().Be(MasterStyle.Light);
        package.Lookup.TryGetValue(lightB, out var lightBResult).Should().BeTrue();
        lightBResult.Index.ID.Should().Be(1);
        lightBResult.Style.Should().Be(MasterStyle.Light);
        package.Lookup.TryGetValue(mediumA, out var mediumAResult).Should().BeTrue();
        mediumAResult.Index.ID.Should().Be(0);
        mediumAResult.Style.Should().Be(MasterStyle.Medium);
        package.Lookup.TryGetValue(mediumB, out var mediumBResult).Should().BeTrue();
        mediumBResult.Index.ID.Should().Be(1);
        mediumBResult.Style.Should().Be(MasterStyle.Medium);
    }
}