using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Text;
using Mutagen.Bethesda.Core.UnitTests;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public class Equals_Tests
    {
        [Fact]
        public void FreshEquals()
        {
            var npc1 = new Npc(TestConstants.Form1);
            var npc2 = new Npc(TestConstants.Form1);
            Assert.Equal(npc1, npc2);
        }

        [Fact]
        public void SimpleEquals()
        {
            var npc1 = new Npc(TestConstants.Form1)
            {
                Name = "TEST"
            };
            var npc2 = new Npc(TestConstants.Form1)
            {
                Name = "TEST"
            };
            Assert.Equal(npc1, npc2);
        }
    }
}
