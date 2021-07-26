using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Allocators;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Core.UnitTests.Placeholders
{
    public interface ITestMod : IMod, ITestModGetter
    {
    }

    public interface ITestModGetter : IModGetter
    {
    }
    
    public class TestMod : ITestMod, IDisposable
    {
        #region Interface

        public IEnumerable<IFormLinkGetter> ContainedFormLinks => throw new NotImplementedException();
        public ModKey ModKey { get; }
        public GameRelease GameRelease { get; }

        IList<MasterReference> IMod.MasterReferences => throw new NotImplementedException();

        IReadOnlyList<IMasterReferenceGetter> IModGetter.MasterReferences => throw new NotImplementedException();

        public bool CanUseLocalization { get; }
        public bool UsingLocalization { get; set; }

        public uint NextFormID { get; set; }

        IEnumerable<IMajorRecordCommonGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords()
        {
            return EnumerateMajorRecords();
        }

        IEnumerable<TMajor> IMajorRecordEnumerable.EnumerateMajorRecords<TMajor>(bool throwIfUnknown)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMajorRecordCommon> EnumerateMajorRecords(Type? t, bool throwIfUnknown = true)
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

        public void Remove<TMajor>(FormKey formKey, bool throwIfUnknown = true) where TMajor : IMajorRecordCommonGetter
        {
            throw new NotImplementedException();
        }

        public void Remove<TMajor>(HashSet<FormKey> formKeys, bool throwIfUnknown = true) where TMajor : IMajorRecordCommonGetter
        {
            throw new NotImplementedException();
        }

        public void Remove<TMajor>(IEnumerable<FormKey> formKeys, bool throwIfUnknown = true) where TMajor : IMajorRecordCommonGetter
        {
            throw new NotImplementedException();
        }

        public void Remove<TMajor>(TMajor record, bool throwIfUnknown = true) where TMajor : IMajorRecordCommonGetter
        {
            throw new NotImplementedException();
        }

        public void Remove<TMajor>(IEnumerable<TMajor> records, bool throwIfUnknown = true) where TMajor : IMajorRecordCommonGetter
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMajorRecordCommon> EnumerateMajorRecords()
        {
            throw new NotImplementedException();
        }

        IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IMajorRecordCommonGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords(Type type, bool throwIfUnknown)
        {
            return EnumerateMajorRecords(type, throwIfUnknown);
        }

        public void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping)
        {
            throw new NotImplementedException();
        }

        public IMask<bool> GetEqualsMask(object rhs, EqualsMaskHelper.Include include = EqualsMaskHelper.Include.OnlyFailures)
        {
            throw new NotImplementedException();
        }

        IGroupCommonGetter<TMajor> IModGetter.GetTopLevelGroup<TMajor>()
        {
            throw new NotImplementedException();
        }

        public IGroupCommonGetter<IMajorRecordCommonGetter> GetTopLevelGroup(Type type)
        {
            throw new NotImplementedException();
        }

        public void WriteToBinary(FilePath path, BinaryWriteParameters? param = null, IFileSystem? fileSystem = null)
        {
            throw new NotImplementedException();
        }

        public void WriteToBinaryParallel(FilePath path, BinaryWriteParameters? param = null, IFileSystem? fileSystem = null)
        {
            throw new NotImplementedException();
        }

        public TAlloc SetAllocator<TAlloc>(TAlloc allocator) where TAlloc : IFormKeyAllocator
        {
            throw new NotImplementedException();
        }

        public IGroupCommon<TMajor> GetTopLevelGroup<TMajor>() where TMajor : IMajorRecordCommon
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
}