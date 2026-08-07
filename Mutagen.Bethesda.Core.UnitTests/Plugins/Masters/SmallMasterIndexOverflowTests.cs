using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Masters;

/// <summary>
/// The small master index is a 12 bit field, so the counter that assigns those indices must not be
/// built on anything narrower.  This says nothing about how many masters a file may actually list.
/// </summary>
public class SmallMasterIndexOverflowTests
{
    private const int SmallMasterCount = 300;

    private static ModKey LightKey(int i) => ModKey.FromFileName($"Light{i}.esm");

    private static readonly ModKey Current = ModKey.FromFileName("Patch.esm");

    /// <summary>
    /// CreateUnsafe is used only because <see cref="MasterReferenceCollection"/> caps its list well
    /// below this count.  The subject here is index assignment, not what a real file may contain.
    /// </summary>
    private static IReadOnlySeparatedMasterPackage Package()
    {
        var loadOrder = new LoadOrder<IModFlagsGetter>();
        for (int i = 0; i < SmallMasterCount; i++)
        {
            loadOrder.Add(MastersTestUtil.GetFlags(LightKey(i), MasterStyle.Small));
        }
        loadOrder.Add(MastersTestUtil.GetFlags(Current, MasterStyle.Full));

        var masters = MasterReferenceCollection.CreateUnsafe(
            Current,
            Enumerable.Range(0, SmallMasterCount)
                .Select(i => new MasterReference { Master = LightKey(i) }));

        return SeparatedMasterPackage.Separate(Current, MasterStyle.Full, masters, loadOrder);
    }

    [Fact]
    public void IndicesDoNotWrapAtByteBoundary()
    {
        var package = Package();

        for (int i = 0; i < SmallMasterCount; i++)
        {
            package.TryLookupModKey(LightKey(i), reference: true, out var style, out var index)
                .ShouldBeTrue($"{LightKey(i)} was not found");
            style.ShouldBe(MasterStyle.Small, $"{LightKey(i)} style");
            index.ShouldBe((uint)i, $"{LightKey(i)} index");
        }
    }

    /// <summary>
    /// A wrapped index does not just mislabel one master, it silently collides with an earlier one
    /// </summary>
    [Fact]
    public void WrappedIndexWouldCollideWithAnEarlierMaster()
    {
        var package = Package();

        package.GetFormID(new FormKey(LightKey(43), 0x123))
            .ShouldBe(new FormID(0xFE02B123));
        package.GetFormID(new FormKey(LightKey(299), 0x123))
            .ShouldNotBe(package.GetFormID(new FormKey(LightKey(43), 0x123)));
    }
}
