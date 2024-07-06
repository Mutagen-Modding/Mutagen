using System.Diagnostics;
using System.IO.Abstractions;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Allocators;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.UnitTests.Placeholders;

public interface ITestMod : IMod, ITestModGetter
{
}

public interface ITestModGetter : IModGetter
{
}
    
public class TestMod : ITestMod, IDisposable
{
    #region Interface

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks() => throw new NotImplementedException();
    public ModKey ModKey { get; }
    public GameRelease GameRelease { get; }

    IList<MasterReference> IMod.MasterReferences => throw new NotImplementedException();
    public IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>>? OverriddenForms { get; }

    IReadOnlyList<IMasterReferenceGetter> IModGetter.MasterReferences => throw new NotImplementedException();

    public uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false) => throw new NotImplementedException();
    public IBinaryModdedWriteBuilderLoadOrderChoice BeginWrite => throw new NotImplementedException();

    public bool CanUseLocalization { get; }
    public bool UsingLocalization { get; set; }
    public bool CanBeLightMaster { get; }
    public bool IsLightMaster { get; set; }
    public bool CanBeMediumMaster { get; }
    public bool IsMediumMaster { get; set; }
    public bool ListsOverriddenForms { get; }

    IGroup? IMod.TryGetTopLevelGroup(Type type)
    {
        throw new NotImplementedException();
    }

    public uint NextFormID { get; set; }

    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords()
    {
        return EnumerateMajorRecords();
    }

    IEnumerable<TMajor> IMajorRecordEnumerable.EnumerateMajorRecords<TMajor>(bool throwIfUnknown)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IMajorRecord> EnumerateMajorRecords(Type? t, bool throwIfUnknown = true)
    {
        throw new NotImplementedException();
    }

    public void Remove(FormKey formKey)
    {
        throw new NotImplementedException();
    }

    public void Remove(IEnumerable<FormKey> formKeys)
    {
        throw new NotImplementedException();
    }

    public void Remove(HashSet<FormKey> formKeys)
    {
        throw new NotImplementedException();
    }

    public void Remove(IEnumerable<IFormLinkIdentifier> formLinks)
    {
        throw new NotImplementedException();
    }

    public void Remove(FormKey formKey, Type type, bool throwIfUnknown = true)
    {
        throw new NotImplementedException();
    }

    public void Remove(IEnumerable<FormKey> formKeys, Type type, bool throwIfUnknown = true)
    {
        throw new NotImplementedException();
    }

    public void Remove(HashSet<FormKey> formKeys, Type type, bool throwIfUnknown = true)
    {
        throw new NotImplementedException();
    }

    public void Remove<TMajor>(FormKey formKey, bool throwIfUnknown = true) where TMajor : IMajorRecordGetter
    {
        throw new NotImplementedException();
    }

    public void Remove<TMajor>(HashSet<FormKey> formKeys, bool throwIfUnknown = true) where TMajor : IMajorRecordGetter
    {
        throw new NotImplementedException();
    }

    public void Remove<TMajor>(IEnumerable<FormKey> formKeys, bool throwIfUnknown = true) where TMajor : IMajorRecordGetter
    {
        throw new NotImplementedException();
    }

    public void Remove<TMajor>(TMajor record, bool throwIfUnknown = true) where TMajor : IMajorRecordGetter
    {
        throw new NotImplementedException();
    }

    public void Remove<TMajor>(IEnumerable<TMajor> records, bool throwIfUnknown = true) where TMajor : IMajorRecordGetter
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IMajorRecord> EnumerateMajorRecords()
    {
        throw new NotImplementedException();
    }

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
    {
        throw new NotImplementedException();
    }

    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords(Type type, bool throwIfUnknown)
    {
        return EnumerateMajorRecords(type, throwIfUnknown);
    }

    public IEnumerable<IModContext<TMajor>> EnumerateMajorRecordSimpleContexts<TMajor>(bool throwIfUnknown = true) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> EnumerateMajorRecordSimpleContexts(Type t, bool throwIfUnknown = true)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> EnumerateMajorRecordSimpleContexts()
    {
        throw new NotImplementedException();
    }

    public void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping)
    {
        throw new NotImplementedException();
    }

    public IMask<bool> GetEqualsMask(object rhs, EqualsMaskHelper.Include include = EqualsMaskHelper.Include.OnlyFailures)
    {
        throw new NotImplementedException();
    }

    IGroupGetter<TMajor>? IModGetter.TryGetTopLevelGroup<TMajor>()
    {
        throw new NotImplementedException();
    }

    public IGroupGetter? TryGetTopLevelGroup(Type type)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinary(FilePath path, BinaryWriteParameters? param = null)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinaryParallel(FilePath path, BinaryWriteParameters? param = null, ParallelWriteParameters? parallelWriteParameters = null)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinary(Stream stream, BinaryWriteParameters? param = null)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinaryParallel(Stream stream, BinaryWriteParameters? param = null, ParallelWriteParameters? parallelWriteParameters = null)
    {
        throw new NotImplementedException();
    }

    public TAlloc SetAllocator<TAlloc>(TAlloc allocator) where TAlloc : IFormKeyAllocator
    {
        throw new NotImplementedException();
    }

    public void SetOverriddenForms(IEnumerable<FormKey>? formKeys)
    {
        throw new NotImplementedException();
    }

    public IGroup<TMajor>? TryGetTopLevelGroup<TMajor>() where TMajor : IMajorRecord
    {
        throw new NotImplementedException();
    }

    public FormKey GetNextFormKey()
    {
        throw new NotImplementedException();
    }

    public FormKey GetNextFormKey(string? editorID)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
    }

    #endregion

    public TestMod(ModKey modKey)
    {
        ModKey = modKey;
        NextFormID = 0x800;
    }
}