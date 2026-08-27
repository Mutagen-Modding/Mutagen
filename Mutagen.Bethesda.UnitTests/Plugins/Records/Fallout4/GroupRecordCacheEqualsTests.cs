using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

/// <summary>
/// Same latent defect as PackageDataEqualsTests (Mutagen-Modding/Mutagen#686),
/// but for the other Dict codegen path: Group.RecordCache is DictMode.KeyedValue
/// (an ICache&lt;T, FormKey&gt;, not an IReadOnlyDictionary), which the mask
/// generator already handles with EqualsMaskHelper.CacheEqualsHelper instead of
/// DictEqualsHelper. A compile-clean regen of the KeyedValue branch isn't proof
/// it's behaviorally right - this exercises it the same way the Package.Data
/// tests exercise the KeyValue branch.
/// </summary>
public class GroupRecordCacheEqualsTests
{
    private static readonly ModKey TestModKey = ModKey.FromNameAndExtension("GroupRecordCacheEqualsTests.esp");

    private static Fallout4Group<Static> MakeGroup(Fallout4Mod mod, params (uint Id, string EditorId)[] entries)
    {
        var group = new Fallout4Group<Static>(mod);
        foreach (var (id, editorId) in entries)
        {
            group.Add(new Static(new FormKey(TestModKey, id), Fallout4Release.Fallout4)
            {
                EditorID = editorId,
            });
        }
        return group;
    }

    [Fact]
    public void ReorderedEntries_SameContent_AreEqual()
    {
        var mod = new Fallout4Mod(TestModKey, Fallout4Release.Fallout4);
        var lhs = MakeGroup(mod, (0x800, "First"), (0x801, "Second"));
        var rhs = MakeGroup(mod, (0x801, "Second"), (0x800, "First"));

        lhs.Equals(rhs).ShouldBeTrue();
        lhs.GetEqualsMask(rhs).All(b => b).ShouldBeTrue();
    }

    [Fact]
    public void SameKeys_DifferingValueUnderSameKey_AreNotEqual()
    {
        var mod = new Fallout4Mod(TestModKey, Fallout4Release.Fallout4);
        var lhs = MakeGroup(mod, (0x800, "First"), (0x801, "Second"));
        var rhs = MakeGroup(mod, (0x801, "SecondButDifferent"), (0x800, "First"));

        lhs.Equals(rhs).ShouldBeFalse();
        lhs.GetEqualsMask(rhs).All(b => b).ShouldBeFalse();
    }
}
