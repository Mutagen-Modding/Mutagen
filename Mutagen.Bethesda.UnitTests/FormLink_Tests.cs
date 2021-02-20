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
    }
}
