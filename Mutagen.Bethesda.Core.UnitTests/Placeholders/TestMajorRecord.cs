using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.UnitTests.Placeholders;

public interface ITestMajorRecordGetter : IMajorRecordGetter
{
}

public interface ITestMajorRecord : ITestMajorRecordGetter, IMajorRecord
{
}
    
public class TestMajorRecord : ITestMajorRecord
{
    public bool IsCompressed { get; set; }
    public bool IsDeleted { get; set; }
    public object CommonInstance()
    {
        throw new NotImplementedException();
    }

    public object? CommonSetterInstance()
    {
        throw new NotImplementedException();
    }

    public object CommonSetterTranslationInstance()
    {
        throw new NotImplementedException();
    }

    public int MajorRecordFlagsRaw { get; set; }
    public bool Disable()
    {
        throw new System.NotImplementedException();
    }

    public ushort? FormVersion { get; }
    public IEnumerable<IFormLinkGetter> ContainedFormLinks => throw new NotImplementedException();
    public void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping)
    {
        throw new System.NotImplementedException();
    }

    public bool Equals(IFormLinkGetter? other)
    {
        throw new System.NotImplementedException();
    }

    public ILoquiRegistration Registration => throw new NotImplementedException();
    public FormKey FormKey { get; }
    public uint VersionControl { get; set; }
    public string? EditorID { get; set; }

    public TestMajorRecord(FormKey formKey)
    {
        FormKey = formKey;
    }

    public IFormLink<IOtherTestMajorRecordGetter> FormLink { get; } = new FormLink<IOtherTestMajorRecordGetter>();
    public object BinaryWriteTranslator { get; } = null!;
    public void WriteToBinary(MutagenWriter writer, TypedWriteParams? translationParams = null)
    {
        throw new NotImplementedException();
    }

    public void ToString(StructuredStringBuilder sb, string? name = null)
    {
        throw new NotImplementedException();
    }

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

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public Type Type => throw new NotImplementedException();
}