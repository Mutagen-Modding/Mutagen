using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Placeholders;
using Noggog.Testing.Extensions;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class FormLinkTests
{
    [Fact]
    public void FormLinkEquals()
    {
        FormLink<ITestMajorRecordGetter> link1 = new(TestConstants.Form1);
        FormLink<ITestMajorRecordGetter> link2 = new(TestConstants.Form1);
        link1.ShouldBe(link2);
        link2.ShouldBe(link1);
    }

    [Fact]
    public void FormLinkEquals_DifferingGetterSetter()
    {
        FormLink<ITestMajorRecordGetter> getter = new(TestConstants.Form1);
        FormLink<ITestMajorRecord> setter = new(TestConstants.Form1);
        FormLink<TestMajorRecord> direct = new(TestConstants.Form1);
        getter.ShouldEqual(setter);
        getter.ShouldEqual(direct);
        setter.ShouldEqual(direct);
        setter.ShouldEqual(getter);
        direct.ShouldEqual(getter);
        direct.ShouldEqual(setter);
    }

    [Fact]
    public void FormLinkEquals_CompletelyDifferingTypes()
    {
        FormLink<ITestMajorRecordGetter> link1 = new(TestConstants.Form1);
        FormLink<IOtherTestMajorRecordGetter> link2 = new(TestConstants.Form1);
        link1.ShouldNotBe<object>(link2);
        link2.ShouldNotBe<object>(link1);
    }

    [Fact]
    public void FormLinkEquals_DifferingTypes()
    {
        FormLink<ITestMajorRecordGetter> link1 = new(TestConstants.Form1);
        FormLink<ILeafTestMajorRecordGetter> link2 = new(TestConstants.Form1);
        link1.ShouldEqual(link2);
        link2.ShouldEqual(link1);
    }

    [Fact]
    public void FormLinkSet()
    {
        FormLink<ITestMajorRecordGetter> link1 = new(TestConstants.Form1);
        FormLink<ITestMajorRecordGetter> link2 = new(TestConstants.Form1);
        HashSet<FormLink<ITestMajorRecordGetter>> set = new()
        {
            link1,
            link2
        };
        set.Count.ShouldBe(1);
    }

    [Fact]
    public void FormLinkSet_DifferingGetterSetter()
    {
        FormLink<ITestMajorRecordGetter> getter = new(TestConstants.Form1);
        FormLink<ITestMajorRecord> setter = new(TestConstants.Form1);
        FormLink<TestMajorRecord> direct = new(TestConstants.Form1);
        HashSet<IFormLinkGetter<ITestMajorRecordGetter>> set = new()
        {
            getter,
            setter,
            direct,
        };
        set.Count.ShouldBe(1);
    }

    [Fact]
    public void FormLinkSet_DifferingTypes()
    {
        FormLink<ITestMajorRecordGetter> link1 = new(TestConstants.Form1);
        FormLink<ILeafTestMajorRecordGetter> link2 = new(TestConstants.Form1);
        HashSet<IFormLinkGetter<ITestMajorRecordGetter>> set = new()
        {
            link1,
            link2,
        };
        set.Count.ShouldBe(1);
    }
        
    [Fact]
    public void SetToOnObjects()
    {
        var n = new TestMajorRecord(TestConstants.Form1);
        var r = Substitute.For<IOtherTestMajorRecordGetter>();
        r.FormKey.Returns(TestConstants.Form2);
        n.FormLink.IsNull.ShouldBeTrue();
        n.FormLink.SetTo(r);
        n.FormLink.FormKey.ShouldBe(TestConstants.Form2);
    }

    [Fact]
    public void NormalCollectionContains()
    {
        var set = new HashSet<IFormLinkGetter<IMajorRecordGetter>>();
        set.Add(new FormLink<ITestMajorRecordGetter>(TestConstants.Form1));
        set.Contains(new FormLink<ITestMajorRecordGetter>(TestConstants.Form1)).ShouldBeTrue();
        set.Contains(new FormLink<IOtherTestMajorRecordGetter>(TestConstants.Form1)).ShouldBeFalse();
    }

    [Fact]
    public void TypelessCollectionContains()
    {
        var set = new HashSet<IFormLinkGetter<IMajorRecordGetter>>(FormLink<IMajorRecordGetter>.TypelessComparer);
        set.Add(new FormLink<ITestMajorRecordGetter>(TestConstants.Form1));
        set.Contains(new FormLink<ITestMajorRecordGetter>(TestConstants.Form1)).ShouldBeTrue();
        set.Contains(new FormLink<IOtherTestMajorRecordGetter>(TestConstants.Form1)).ShouldBeTrue();
    }
}