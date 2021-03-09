using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Persistance;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class SimpleFormKeyAllocator_Tests : IFormKeyAllocator_Tests<SimpleFormKeyAllocator>
    {
        protected override SimpleFormKeyAllocator CreateFormKeyAllocator(IMod mod) => new(mod);

        [Fact]
        public void TestRegister()
        {
            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = new SimpleFormKeyAllocator(mod);

            allocator.Register(Utility.Edid1, Utility.Form1);

            var key = allocator.GetNextFormKey(Utility.Edid1);

            Assert.Equal(Utility.Form1, key);
        }

        [Fact]
        public void TestTryRegister()
        {
            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = new SimpleFormKeyAllocator(mod);

            allocator.Register(Utility.Edid1, Utility.Form1);

            Assert.False(allocator.TryRegister(Utility.Edid1, Utility.Form2));

            var key = allocator.GetNextFormKey(Utility.Edid1);

            Assert.Equal(Utility.Form1, key);
        }
    }
}
