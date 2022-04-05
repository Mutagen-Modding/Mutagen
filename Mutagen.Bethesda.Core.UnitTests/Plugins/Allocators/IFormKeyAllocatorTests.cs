using System;
using System.Data;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Mutagen.Bethesda.Plugins.Allocators;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Allocators;

public abstract class IFormKeyAllocatorTests<TFormKeyAllocator>
    where TFormKeyAllocator : IFormKeyAllocator
{
    protected virtual IFileSystem GetFileSystem() => new MockFileSystem();
        
    protected abstract TFormKeyAllocator CreateAllocator(IFileSystem fileSystem, IMod mod);

    protected void DisposeFormKeyAllocator(IFormKeyAllocator allocator)
    {
        if (allocator is IDisposable disposable)
            disposable.Dispose();
    }

    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void CanAllocateNewFormKey(IMod mod)
    {
        var fileSystem = GetFileSystem();
        var nextID = mod.NextFormID;

        var allocator = CreateAllocator(fileSystem, mod);

        var formKey = allocator.GetNextFormKey();
        Assert.Equal(mod.ModKey, formKey.ModKey);
        Assert.Equal(nextID, formKey.ID);

        DisposeFormKeyAllocator(allocator);
    }

    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void CanAllocateNewFormKeyFromEditorId(IMod mod)
    {
        var fileSystem = GetFileSystem();
        var nextID = mod.NextFormID;

        var allocator = CreateAllocator(fileSystem, mod);

        var formKey = allocator.GetNextFormKey(TestConstants.Edid1);

        Assert.Equal(mod.ModKey, formKey.ModKey);
        Assert.Equal(nextID, formKey.ID);

        DisposeFormKeyAllocator(allocator);
    }

    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void TwoAllocatedFormKeysAreDifferent(IMod mod)
    {
        var fileSystem = GetFileSystem();

        var allocator = CreateAllocator(fileSystem, mod);

        var formKey1 = allocator.GetNextFormKey();
        var formKey2 = allocator.GetNextFormKey();

        Assert.NotEqual(formKey1, formKey2);

        DisposeFormKeyAllocator(allocator);
    }

    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void TwoAllocatedFormKeysFromEditorIDsAreDifferent(IMod mod)
    {
        var fileSystem = GetFileSystem();

        var allocator = CreateAllocator(fileSystem, mod);

        var formKey1 = allocator.GetNextFormKey(TestConstants.Edid1);
        var formKey2 = allocator.GetNextFormKey(TestConstants.Edid2);

        Assert.NotEqual(formKey1, formKey2);

        DisposeFormKeyAllocator(allocator);
    }


    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void DuplicateAllocationThrows(IMod mod)
    {
        var fileSystem = GetFileSystem();
        var allocator = CreateAllocator(fileSystem, mod);

        var formKey1 = allocator.GetNextFormKey(TestConstants.Edid1);

        var e = Assert.Throws<ConstraintException>(() => allocator.GetNextFormKey(TestConstants.Edid1));

        DisposeFormKeyAllocator(allocator);
    }
}