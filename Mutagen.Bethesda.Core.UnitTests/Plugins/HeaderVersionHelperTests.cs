using Shouldly;
using Mutagen.Bethesda.Plugins.Utility;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class HeaderVersionHelperTests
{
    [Theory, DefaultAutoData]
    public void ForceLower(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetInitialFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion + 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 0x800)
            .ShouldEqual(0x800);
        HeaderVersionHelper.GetInitialFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion + 1,
            forceUseLowerFormIDRanges: true,
            higherFormIdRange: 0x800)
            .ShouldEqual(0);
    }

    [Theory, DefaultAutoData]
    public void ForceHigher(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetInitialFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion - 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 0x800)
            .ShouldEqual(0);
        HeaderVersionHelper.GetInitialFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion - 1,
            forceUseLowerFormIDRanges: false,
            higherFormIdRange: 0x800)
            .ShouldEqual(0x800);
    }

    [Theory, DefaultAutoData]
    public void HeaderVersionToUseLowerRelease(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetInitialFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion - 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 0x800)
            .ShouldEqual(0);
    }

    [Theory, DefaultAutoData]
    public void HeaderVersionToUseHigherRelease(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetInitialFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion + 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 0x800)
            .ShouldEqual(0x800);
    }

    [Theory, DefaultAutoData]
    public void AllowedRelease(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetInitialFormId(
            release,
            allowedReleases: new HashSet<GameRelease>()
            {
                release
            },
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion - 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 0x800)
            .ShouldEqual(0);
    }

    [Theory, DefaultAutoData]
    public void DisallowedRelease(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetInitialFormId(
            release,
            allowedReleases: new HashSet<GameRelease>()
            {
            },
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion - 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 0x800)
            .ShouldEqual(0x800);
    }

    [Theory, DefaultAutoData]
    public void ForceUseLowerRangesWithNoListedLowerVersion(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetInitialFormId(
            release,
            allowedReleases: new HashSet<GameRelease>()
            {
            },
            headerVersion: headerVersion,
            useLowerRangesVersion: null,
            forceUseLowerFormIDRanges: true,
            higherFormIdRange: 0x800)
            .ShouldEqual(0);
    }
}
