using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

/// <summary>
/// Repro for Mutagen-Modding/Mutagen#686: the generated Equals for Dict-typed
/// fields (Package.Data here) compared entries by sequence/insertion order
/// instead of by key, so two packages with identical Data content inserted in
/// a different order were Equals-unequal despite GetEqualsMask reporting them
/// equal. The writer emits keys sorted while the parser inserts in on-disk
/// UNAM order, so this is a real round-trip scenario, not a contrived one.
/// </summary>
public class PackageDataEqualsTests
{
    private static readonly ModKey TestModKey = ModKey.FromNameAndExtension("PackageDataEqualsTests.esp");

    private static Package MakePackage(params (sbyte Key, uint Value)[] entries)
    {
        var package = new Package(new FormKey(TestModKey, 0x800), Fallout4Release.Fallout4);
        foreach (var (key, value) in entries)
        {
            package.Data.Add(key, new PackageDataInt { Data = value });
        }
        return package;
    }

    [Fact]
    public void ReorderedEntries_SameContent_AreEqual()
    {
        var lhs = MakePackage((5, 11), (2, 22));
        var rhs = MakePackage((2, 22), (5, 11));

        lhs.Equals(rhs).ShouldBeTrue();
        lhs.GetEqualsMask(rhs).All(b => b).ShouldBeTrue();
    }

    [Fact]
    public void SameKeys_DifferingValueUnderSameKey_AreNotEqual()
    {
        // Differs by Name (an APackageData base-class field) rather than Data
        // (a PackageDataInt-only field): GetEqualsMask's dispatch for dict
        // values reached through the abstract IAPackageDataGetter base
        // interface only compares base-class fields correctly - a separate,
        // pre-existing bug unrelated to #686, tracked independently. Name
        // keeps this test inside #686's scope: by-key vs. by-sequence Equals.
        var lhs = MakePackage((5, 11), (2, 22));
        var rhs = MakePackage((5, 11), (2, 22));
        rhs.Data[2].Name = "Different";

        lhs.Equals(rhs).ShouldBeFalse();
        lhs.GetEqualsMask(rhs).All(b => b).ShouldBeFalse();
    }
}
