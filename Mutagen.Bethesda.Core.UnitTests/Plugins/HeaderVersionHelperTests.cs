using FluentAssertions;
using Mutagen.Bethesda.Plugins.Utility;
using Noggog.Testing.AutoFixture;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class HeaderVersionHelperTests
{
    [Theory, DefaultAutoData]
    public void ForceLower(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetNextFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion + 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 800)
            .Should().Be(800);
        HeaderVersionHelper.GetNextFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion + 1,
            forceUseLowerFormIDRanges: true,
            higherFormIdRange: 800)
            .Should().Be(1);
    }

    [Theory, DefaultAutoData]
    public void ForceHigher(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetNextFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion - 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 800)
            .Should().Be(1);
        HeaderVersionHelper.GetNextFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion - 1,
            forceUseLowerFormIDRanges: false,
            higherFormIdRange: 800)
            .Should().Be(800);
    }

    [Theory, DefaultAutoData]
    public void HeaderVersionToUseLowerRelease(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetNextFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion - 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 800)
            .Should().Be(1);
    }

    [Theory, DefaultAutoData]
    public void HeaderVersionToUseHigherRelease(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetNextFormId(
            release,
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion + 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 800)
            .Should().Be(800);
    }

    [Theory, DefaultAutoData]
    public void AllowedRelease(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetNextFormId(
            release,
            allowedReleases: new HashSet<GameRelease>()
            {
                release
            },
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion - 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 800)
            .Should().Be(1);
    }

    [Theory, DefaultAutoData]
    public void DisallowedRelease(
        GameRelease release,
        float headerVersion)
    {
        HeaderVersionHelper.GetNextFormId(
            release,
            allowedReleases: new HashSet<GameRelease>()
            {
            },
            headerVersion: headerVersion,
            useLowerRangesVersion: headerVersion - 1,
            forceUseLowerFormIDRanges: null,
            higherFormIdRange: 800)
            .Should().Be(800);
    }
}
