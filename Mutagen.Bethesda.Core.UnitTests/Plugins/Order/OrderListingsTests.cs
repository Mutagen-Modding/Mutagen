using FluentAssertions;
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
        ModKey ccEsl = new ModKey("CC", ModType.LightMaster);
        ModKey ccEsl2 = new ModKey("CC2", ModType.LightMaster);
        ModKey esm = new ModKey("Normal", ModType.Master);
        ModKey esm2 = new ModKey("Normal2", ModType.Master);
        ModKey esl = new ModKey("Normal", ModType.LightMaster);
        ModKey esl2 = new ModKey("Normal2", ModType.LightMaster);
        ModKey esp = new ModKey("Normal", ModType.Plugin);
        ModKey esp2 = new ModKey("Normal2", ModType.Plugin);

        var ordered = new OrderListings().Order(
                implicitListings: new ModKey[]
                {
                    baseEsm,
                    baseEsm2,
                },
                creationClubListings: new ModKey[]
                {
                    ccEsl,
                    ccEsl2,
                    ccEsm,
                    ccEsm2,
                },
                pluginsListings: new ModKey[]
                {
                    esm,
                    esm2,
                    esl,
                    esl2,
                    esp,
                    esp2,
                },
                selector: m => m)
            .ToList();
        ordered.Should().Equal(new ModKey[]
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
                creationClubListings: new ModKey[]
                {
                    ccEsm,
                    ccEsm2,
                    ccEsm3,
                },
                pluginsListings: new ModKey[]
                {
                    ccEsm2,
                    esm,
                    ccEsm,
                    esm2,
                },
                selector: m => m)
            .Should().Equal(new ModKey[]
            {
                // First, because wasn't listed on plugins
                ccEsm3,
                // 2nd because it was first on the plugins listings
                ccEsm2,
                // Was listed last on the plugins listing
                ccEsm,
                esm,
                esm2,
            });
    }
}