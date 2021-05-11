using FluentAssertions;
using Loqui;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Oblivion.Internals;
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
            var listing1 = new ModListing(Utility.PluginModKey, enabled: true);
            var listing1Eq = new ModListing
            {
                ModKey = Utility.PluginModKey,
                Enabled = true,
            };
            var listing1Disabled = new ModListing
            {
                ModKey = Utility.PluginModKey,
                Enabled = false,
            };
            var listing2 = new ModListing(Utility.PluginModKey2, enabled: true);
            var listing2Eq = new ModListing()
            {
                ModKey = Utility.PluginModKey2,
                Enabled = true
            };
            var listing2Disabled = new ModListing()
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
