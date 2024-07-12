using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Records;
using NSubstitute;

namespace Mutagen.Bethesda.UnitTests.Plugins.Masters;

internal static class MastersTestUtil
{
    internal static IModFlagsGetter GetFlags(ModKey modKey, MasterStyle style)
    {
        var modGetter = Substitute.For<IModFlagsGetter>();
        modGetter.ModKey.Returns(modKey);
        modGetter.CanBeSmallMaster.Returns(true);
        modGetter.CanBeMediumMaster.Returns(true);
        switch (style)
        {
            case MasterStyle.Full:
                modGetter.IsSmallMaster.Returns(false);
                modGetter.IsMediumMaster.Returns(false);
                break;
            case MasterStyle.Light:
                modGetter.IsSmallMaster.Returns(true);
                modGetter.IsMediumMaster.Returns(false);
                break;
            case MasterStyle.Medium:
                modGetter.IsSmallMaster.Returns(false);
                modGetter.IsMediumMaster.Returns(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }

        return modGetter;
    }
}