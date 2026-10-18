using Mutagen.Bethesda.Plugins.Cache.Internals;
using Mutagen.Bethesda.Starfield;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache;

public class OverrideMaskRegistrationsTests
{
    [Fact]
    public void StarfieldCellUsesCopyMask()
    {
        OverrideMaskRegistrations.Get<ICellGetter>().ShouldBeSameAs(Starfield.ModContextExt.CellCopyMask);
    }
}
