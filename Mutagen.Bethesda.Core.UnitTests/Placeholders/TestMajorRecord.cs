using Loqui;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
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
    public IEnumerable<IFormLinkGetter> EnumerateFormLinks() => throw new NotImplementedException();
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
    public void WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams = default)
    {
        throw new NotImplementedException();
    }

    public void Print(StructuredStringBuilder sb, string? name = null)
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

    public Type Type => throw new NotImplementedException();
    
    public void RemapAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping, AssetLinkQuery query, IAssetLinkCache? linkCache)
    {
        throw new NotImplementedException();
    }

    public void RemapListedAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IAssetLink> EnumerateListedAssetLinks()
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null,
        Type? assetType = null)
    {
        throw new NotImplementedException();
    }
}
internal class TestMajorRecord_Registration : ILoquiRegistration
{
    public static TestMajorRecord_Registration Instance { get; } = new();

    public string GetNthName(ushort index) => throw new NotImplementedException();
    public bool GetNthIsLoqui(ushort index) => throw new NotImplementedException();
    public bool GetNthIsEnumerable(ushort index) => throw new NotImplementedException();
    public bool GetNthIsSingleton(ushort index) => throw new NotImplementedException();
    public bool IsNthDerivative(ushort index) => throw new NotImplementedException();
    public Type GetNthType(ushort index) => throw new NotImplementedException();
    public ushort? GetNameIndex(StringCaseAgnostic name) => throw new NotImplementedException();
    public bool IsProtected(ushort index) => throw new NotImplementedException();
    public ProtocolKey ProtocolKey { get; } = new("TestGame");
    public ushort AdditionalFieldCount { get; }
    public ushort FieldCount { get; }
    public Type MaskType => null!;
    public Type ErrorMaskType => null!;
    public Type ClassType { get; } = typeof(TestMajorRecord);
    public Type GetterType { get; } = typeof(ITestMajorRecordGetter);
    public Type SetterType { get; } = typeof(ITestMajorRecord);
    public Type? InternalGetterType { get; }
    public Type? InternalSetterType { get; }
    public string FullName => "Mutagen.Bethesda.TestGame.TestMajorRecord";
    public string Name => "TestMajorRecord";
    public string Namespace => "Mutagen.Bethesda";
    public byte GenericCount { get; }
    public Type? GenericRegistrationType { get; }
}