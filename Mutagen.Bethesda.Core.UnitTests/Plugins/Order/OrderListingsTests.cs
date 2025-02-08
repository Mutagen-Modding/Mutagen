using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order.DI;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class OrderListingsTests
{
    [Fact]
    public void OrderListings()
    {
        ModKey baseEsm = new ModKey("Base", ModType.Master);
        ModKey baseEsm2 = new ModKey("Base2", ModType.Master);
        ModKey ccEsm = new ModKey("CC", ModType.Master);
        ModKey ccEsm2 = new ModKey("CC2", ModType.Master);
        ModKey ccEsl = new ModKey("CC", ModType.Light);
        ModKey ccEsl2 = new ModKey("CC2", ModType.Light);
        ModKey esm = new ModKey("Normal", ModType.Master);
        ModKey esm2 = new ModKey("Normal2", ModType.Master);
        ModKey esl = new ModKey("Normal", ModType.Light);
        ModKey esl2 = new ModKey("Normal2", ModType.Light);
        ModKey esp = new ModKey("Normal", ModType.Plugin);
        ModKey esp2 = new ModKey("Normal2", ModType.Plugin);

        var ordered = new OrderListings().Order(
                implicitListings:
                [
                    baseEsm,
                    baseEsm2
                ],
                creationClubListings:
                [
                    ccEsl,
                    ccEsl2,
                    ccEsm,
                    ccEsm2
                ],
                pluginsListings:
                [
                    esm,
                    esm2,
                    esl,
                    esl2,
                    esp,
                    esp2
                ],
                selector: m => m)
            .ToList();
        ordered.ShouldBe(new[]
        {
            baseEsm,
            baseEsm2,
            ccEsm,
            ccEsm2,
            ccEsl,
            ccEsl2,
            esm,
            esm2,
            esl,
            esl2,
            esp,
            esp2,
        });
    }

    [Fact]
    public void OrderListings_EnsurePluginListedCCsDriveOrder()
    {
        ModKey ccEsm = new ModKey("CC", ModType.Master);
        ModKey ccEsm2 = new ModKey("CC2", ModType.Master);
        ModKey ccEsm3 = new ModKey("CC3", ModType.Master);
        ModKey esm = new ModKey("Normal", ModType.Master);
        ModKey esm2 = new ModKey("Normal2", ModType.Master);

        new OrderListings().Order(
                implicitListings: Array.Empty<ModKey>(),
                creationClubListings:
                [
                    ccEsm,
                    ccEsm2,
                    ccEsm3
                ],
                pluginsListings:
                [
                    ccEsm2,
                    esm,
                    ccEsm,
                    esm2
                ],
                selector: m => m)
            .ShouldBe([
                // First, because wasn't listed on plugins
                ccEsm3,
                // 2nd because it was first on the plugins listings
                ccEsm2,
                // Was listed last on the plugins listing
                ccEsm,
                esm,
                esm2
            ]);
    }
}