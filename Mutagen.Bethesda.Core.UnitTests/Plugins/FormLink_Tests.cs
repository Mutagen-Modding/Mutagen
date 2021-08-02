using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Core.UnitTests.Placeholders;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.Core.UnitTests.Plugins
{
    public class FormLink_Tests
    {
        [Fact]
        public void FormLinkEquals()
        {
            FormLink<ITestMajorRecordGetter> link1 = new(TestConstants.Form1);
            FormLink<ITestMajorRecordGetter> link2 = new(TestConstants.Form1);
            link1.Should().Be(link2);
            link2.Should().Be(link1);
        }

        [Fact]
        public void FormLinkEquals_DifferingGetterSetter()
        {
            FormLink<ITestMajorRecordGetter> getter = new(TestConstants.Form1);
            FormLink<ITestMajorRecord> setter = new(TestConstants.Form1);
            FormLink<TestMajorRecord> direct = new(TestConstants.Form1);
            getter.Should().Be(setter);
            getter.Should().Be(direct);
            setter.Should().Be(direct);
            setter.Should().Be(getter);
            direct.Should().Be(getter);
            direct.Should().Be(setter);
        }

        [Fact]
        public void FormLinkEquals_CompletelyDifferingTypes()
        {
            FormLink<ITestMajorRecordGetter> link1 = new(TestConstants.Form1);
            FormLink<IOtherTestMajorRecordGetter> link2 = new(TestConstants.Form1);
            link1.Should().NotBe(link2);
            link2.Should().NotBe(link1);
        }

        [Fact]
        public void FormLinkEquals_DifferingTypes()
        {
            FormLink<ITestMajorRecordGetter> link1 = new(TestConstants.Form1);
            FormLink<ILeafTestMajorRecordGetter> link2 = new(TestConstants.Form1);
            link1.Should().Be(link2);
            link2.Should().Be(link1);
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
            set.Should().HaveCount(1);
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
            set.Should().HaveCount(1);
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
            set.Should().HaveCount(1);
        }
        
        [Fact]
        public void SetToOnObjects()
        {
            var n = new TestMajorRecord(TestConstants.Form1);
            var r = Substitute.For<IOtherTestMajorRecordGetter>();
            r.FormKey.Returns(TestConstants.Form2);
            n.FormLink.IsNull.Should().BeTrue();
            n.FormLink.SetTo(r);
            n.FormLink.FormKey.Should().Be(TestConstants.Form2);
        }

        [Fact]
        public void NormalCollectionContains()
        {
            var set = new HashSet<IFormLinkGetter<IMajorRecordCommonGetter>>();
            set.Add(new FormLink<ITestMajorRecordGetter>(TestConstants.Form1));
            set.Contains(new FormLink<ITestMajorRecordGetter>(TestConstants.Form1)).Should().BeTrue();
            set.Contains(new FormLink<IOtherTestMajorRecordGetter>(TestConstants.Form1)).Should().BeFalse();
        }

        [Fact]
        public void TypelessCollectionContains()
        {
            var set = new HashSet<IFormLinkGetter<IMajorRecordCommonGetter>>(FormLink<IMajorRecordCommonGetter>.TypelessComparer);
            set.Add(new FormLink<ITestMajorRecordGetter>(TestConstants.Form1));
            set.Contains(new FormLink<ITestMajorRecordGetter>(TestConstants.Form1)).Should().BeTrue();
            set.Contains(new FormLink<IOtherTestMajorRecordGetter>(TestConstants.Form1)).Should().BeTrue();
        }
    }
}
