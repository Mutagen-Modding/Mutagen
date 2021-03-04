using Mutagen.Bethesda.Oblivion;
using System;
using System.Data;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public abstract class IFormKeyAllocator_Tests<TFormKeyAllocator>
        where TFormKeyAllocator : IFormKeyAllocator
    {
        protected abstract TFormKeyAllocator CreateFormKeyAllocator(IMod mod);

        protected void DisposeFormKeyAllocator(IFormKeyAllocator allocator)
        {
            if (allocator is IDisposable disposable)
                disposable.Dispose();
        }

        [Fact]
        public void CanAllocateNewFormKey()
        {
            var mod = new OblivionMod(Utility.PluginModKey);
            var nextID = ((IMod)mod).NextFormID;

            var allocator = CreateFormKeyAllocator(mod);

            var formKey = allocator.GetNextFormKey();
            Assert.Equal(mod.ModKey, formKey.ModKey);
            Assert.Equal(nextID, formKey.ID);

            DisposeFormKeyAllocator(allocator);
        }

        [Fact]
        public void CanAllocateNewFormKeyFromEditorID()
        {
            var mod = new OblivionMod(Utility.PluginModKey);
            var nextID = ((IMod)mod).NextFormID;

            var allocator = CreateFormKeyAllocator(mod);

            var formKey = allocator.GetNextFormKey(Utility.Edid1);

            Assert.Equal(mod.ModKey, formKey.ModKey);
            Assert.Equal(nextID, formKey.ID);

            DisposeFormKeyAllocator(allocator);
        }

        [Fact]
        public void TwoAllocatedFormKeysAreDifferent()
        {
            var mod = new OblivionMod(Utility.PluginModKey);

            var allocator = CreateFormKeyAllocator(mod);

            var formKey1 = allocator.GetNextFormKey();
            var formKey2 = allocator.GetNextFormKey();

            Assert.NotEqual(formKey1, formKey2);

            DisposeFormKeyAllocator(allocator);
        }

        [Fact]
        public void TwoAllocatedFormKeysFromEditorIDsAreDifferent()
        {
            var mod = new OblivionMod(Utility.PluginModKey);

            var allocator = CreateFormKeyAllocator(mod);

            var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
            var formKey2 = allocator.GetNextFormKey(Utility.Edid2);

            Assert.NotEqual(formKey1, formKey2);

            DisposeFormKeyAllocator(allocator);
        }


        [Fact]
        public void DuplicateAllocationThrows()
        {
            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = CreateFormKeyAllocator(mod);

            var formKey1 = allocator.GetNextFormKey(Utility.Edid1);

            var e = Assert.Throws<ConstraintException>(() => allocator.GetNextFormKey(Utility.Edid1));

            DisposeFormKeyAllocator(allocator);
        }
    }
}
