using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class FormLink_Tests
    {
        [Fact]
        public void FormLinkEquals()
        {
            FormLink<INpcGetter> link1 = new FormLink<INpcGetter>(Utility.Form1);
            FormLink<INpcGetter> link2 = new FormLink<INpcGetter>(Utility.Form1);
            link1.Should().Be(link2);
            link2.Should().Be(link1);
        }

        [Fact]
        public void FormLinkEquals_DifferingGetterSetter()
        {
            FormLink<INpcGetter> getter = new FormLink<INpcGetter>(Utility.Form1);
            FormLink<INpc> setter = new FormLink<INpc>(Utility.Form1);
            FormLink<Npc> direct = new FormLink<Npc>(Utility.Form1);
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
            FormLink<INpcGetter> link1 = new FormLink<INpcGetter>(Utility.Form1);
            FormLink<IWeaponGetter> link2 = new FormLink<IWeaponGetter>(Utility.Form1);
            link1.Should().NotBe(link2);
            link2.Should().NotBe(link1);
        }

        [Fact]
        public void FormLinkEquals_DifferingTypes()
        {
            FormLink<IConstructibleGetter> link1 = new FormLink<IConstructibleGetter>(Utility.Form1);
            FormLink<IWeaponGetter> link2 = new FormLink<IWeaponGetter>(Utility.Form1);
            link1.Should().Be(link2);
            link2.Should().Be(link1);
        }

        [Fact]
        public void FormLinkSet()
        {
            FormLink<INpcGetter> link1 = new FormLink<INpcGetter>(Utility.Form1);
            FormLink<INpcGetter> link2 = new FormLink<INpcGetter>(Utility.Form1);
            HashSet<FormLink<INpcGetter>> set = new HashSet<FormLink<INpcGetter>>()
            {
                link1,
                link2
            };
            set.Should().HaveCount(1);
        }

        [Fact]
        public void FormLinkSet_DifferingGetterSetter()
        {
            FormLink<INpcGetter> getter = new FormLink<INpcGetter>(Utility.Form1);
            FormLink<INpc> setter = new FormLink<INpc>(Utility.Form1);
            FormLink<Npc> direct = new FormLink<Npc>(Utility.Form1);
            HashSet<IFormLinkGetter<INpcGetter>> set = new HashSet<IFormLinkGetter<INpcGetter>>()
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
            FormLink<IConstructibleGetter> link1 = new FormLink<IConstructibleGetter>(Utility.Form1);
            FormLink<IWeaponGetter> link2 = new FormLink<IWeaponGetter>(Utility.Form1);
            HashSet<IFormLinkGetter<IConstructibleGetter>> set = new HashSet<IFormLinkGetter<IConstructibleGetter>>()
            {
                link1,
                link2,
            };
            set.Should().HaveCount(1);
        }
        
        [Fact]
        public void SetToOnObjects()
        {
            var n = new Npc(Utility.Form1, SkyrimRelease.SkyrimSE);
            var r = new Race(Utility.Form2, SkyrimRelease.SkyrimSE);
            n.Race.IsNull.Should().BeTrue();
            n.Race.SetTo(r);
            n.Race.FormKey.Should().Be(Utility.Form2);
        }

        [Fact]
        public void NormalCollectionContains()
        {
            var set = new HashSet<IFormLinkGetter<ISkyrimMajorRecordGetter>>();
            set.Add(new FormLink<IFactionGetter>(Utility.Form1));
            set.Contains(new FormLink<IFactionGetter>(Utility.Form1)).Should().BeTrue();
            set.Contains(new FormLink<IWeaponGetter>(Utility.Form1)).Should().BeFalse();
        }

        [Fact]
        public void TypelessCollectionContains()
        {
            var set = new HashSet<IFormLinkGetter<ISkyrimMajorRecordGetter>>(FormLink<ISkyrimMajorRecordGetter>.TypelessComparer);
            set.Add(new FormLink<IFactionGetter>(Utility.Form1));
            set.Contains(new FormLink<IFactionGetter>(Utility.Form1)).Should().BeTrue();
            set.Contains(new FormLink<IWeaponGetter>(Utility.Form1)).Should().BeTrue();
        }

        [Fact]
        public void EqualityToActualRecord()
        {
            var npc = new Npc(Utility.Form1, SkyrimRelease.SkyrimSE);
            var link = npc.AsLink();
            npc.Should().Be(link);
            link.Should().Be(npc);
        }
    }
}
