using Loqui;
using Noggog;
namespace Mutagen.Bethesda.UnitTests.Placeholders;

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