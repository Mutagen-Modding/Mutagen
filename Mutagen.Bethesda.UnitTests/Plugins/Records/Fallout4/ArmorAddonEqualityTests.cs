using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class ArmorAddonEqualityTests
{
    private static ArmorAddon GetSomeArmorAddon()
    {
        return new ArmorAddon(FormKey.Null, Fallout4Release.Fallout4)
        {
            Priority = new Mutagen.Bethesda.Plugins.Records.GenderedItem<byte>(1, 2),
            WeightSliderEnabled = new Mutagen.Bethesda.Plugins.Records.GenderedItem<bool>(true, false),
            WorldModel = new Mutagen.Bethesda.Plugins.Records.GenderedItem<Model?>(
                new Model { File = "male.nif" },
                new Model { File = "female.nif" }),
        };
    }

    [Fact]
    public void DeepCopyEquals()
    {
        var armorAddon = GetSomeArmorAddon();
        var other = armorAddon.DeepCopy();

        armorAddon.Equals(other).ShouldBeTrue();
    }

    [Fact]
    public void DeepCopyEqualsMaskAgrees()
    {
        var armorAddon = GetSomeArmorAddon();
        var other = armorAddon.DeepCopy();

        var mask = armorAddon.GetEqualsMask(other);

        mask.Priority.Male.ShouldBeTrue();
        mask.Priority.Female.ShouldBeTrue();
        mask.WeightSliderEnabled.Male.ShouldBeTrue();
        mask.WeightSliderEnabled.Female.ShouldBeTrue();
        mask.WorldModel!.Overall.ShouldBeTrue();

        armorAddon.Equals(other).ShouldBeTrue();
    }
}
