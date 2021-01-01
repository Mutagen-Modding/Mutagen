using FluentAssertions;
using Loqui;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Misc_Tests
    {
        [Fact]
        public void RegistrationTest()
        {
            Assert.True(LoquiRegistration.TryLocateRegistration(typeof(Mutagen.Bethesda.Oblivion.INpcGetter), out var regis));
            Assert.Same(Npc_Registration.Instance, regis);
        }

        // Just testing new C# records act as expected
        [Fact]
        public void LoadOrderListingTests()
        {
            var listing1 = new LoadOrderListing(Utility.PluginModKey, enabled: true);
            var listing1Eq = new LoadOrderListing
            {
                ModKey = Utility.PluginModKey,
                Enabled = true,
            };
            var listing1Disabled = new LoadOrderListing
            {
                ModKey = Utility.PluginModKey,
                Enabled = false,
            };
            var listing2 = new LoadOrderListing(Utility.PluginModKey2, enabled: true);
            var listing2Eq = new LoadOrderListing()
            {
                ModKey = Utility.PluginModKey2,
                Enabled = true
            };
            var listing2Disabled = new LoadOrderListing()
            {
                ModKey = Utility.PluginModKey2,
                Enabled = false
            };

            listing1.Should().BeEquivalentTo(listing1Eq);
            listing1.Should().NotBeEquivalentTo(listing1Disabled);
            listing1.Should().NotBeEquivalentTo(listing2);
            listing2.Should().BeEquivalentTo(listing2Eq);
            listing2.Should().NotBeEquivalentTo(listing2Disabled);
            listing2.Should().NotBeEquivalentTo(listing1);
        }
    }
}
