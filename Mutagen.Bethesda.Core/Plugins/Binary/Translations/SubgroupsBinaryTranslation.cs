using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Loqui;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Noggog;
using Noggog.Utility;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

internal sealed class SubgroupsBinaryTranslation<T>
    where T : IMajorRecordGetter
{
    public static readonly SubgroupsBinaryTranslation<T> Instance = new();
    public delegate void FillFunc(
        MutagenFrame reader,
        T item);

    private static readonly Lazy<FillFunc>? Fill;
    public static readonly bool IsPartialFormable;
    public static readonly IReadOnlyCollection<int> Subgroups = Array.Empty<int>();

    static SubgroupsBinaryTranslation()
    {
        var t = LoquiRegistration.GetRegister(typeof(T)).GetType();
        var prop = t.GetProperty(Constants.PartialFormMember, BindingFlags.Static | BindingFlags.Public);
        IsPartialFormable = (bool?)prop?.GetValue(null) ?? false;
        if (!IsPartialFormable) return;
        Fill = new Lazy<FillFunc>(GetCreateFunc);
        Subgroups = (IReadOnlyCollection<int>?)LoquiRegistration.GetRegister(typeof(T))!.GetType()
            .GetProperty(Constants.SubgroupsMember, BindingFlags.Static | BindingFlags.Public)?.GetValue(null) ?? Array.Empty<int>();
    }
    
    private static FillFunc GetCreateFunc()
    {
        var tType = typeof(T);
        var register = LoquiRegistration.GetRegister(tType);
        var createTranslName = $"{tType.Namespace}.{tType.Name}BinaryCreateTranslation";
        var createTranslType = tType.Assembly.GetType(createTranslName)!;
        var setterType = tType.Assembly.GetType($"{tType.Namespace}.{register.SetterType.Name}Internal") ?? register.SetterType;
        var method = createTranslType.GetMethods()
            .Where((methodInfo) => methodInfo.Name.Equals("ParseSubgroupsLogic"))
            .Where((methodInfo) => methodInfo.IsStatic
                                   && methodInfo.IsPublic)
            .Where((methodInfo) => methodInfo.GetParameters().Length == 2)
            .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(MutagenFrame)))
            .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(setterType))
            .FirstOrDefault();
        if (method != null)
        {
            return DelegateBuilder.BuildDelegate<FillFunc>(method);
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public static bool TryReadOrphanedSubgroups(MutagenFrame frame, [MaybeNullWhen(false)] out T record)
    {
        if (Fill == null
            || !frame.TryGetGroupHeader(out var group)
            || !Subgroups.Contains(group.GroupType))
        {
            record = default;
            return false;
        }
        var fk = FormKeyBinaryTranslation.Instance.Parse(group.ContainedRecordTypeData, frame.MetaData.MasterReferences);
        record = MajorRecordInstantiator<T>.Activator(fk, frame.MetaData.Constants.Release);
        Fill.Value(frame, record);
        return true;
    }

    public static bool TryReadOrphanedSubgroupWrappers(ReadOnlyMemorySlice<byte> sourceBytes, BinaryOverlayFactoryPackage package, [MaybeNullWhen(false)] out T record)
    {
        if (Fill == null
            || !package.MetaData.Constants.TryGroup(sourceBytes, out var group)
            || !Subgroups.Contains(group.GroupType))
        {
            record = default;
            return false;
        }

        // ToDo
        // Avoid array copy by using MemoryPair
        var formId = new FormID(BinaryPrimitives.ReadUInt32LittleEndian(group.ContainedRecordTypeData));
        byte[] bytes = new byte[package.MetaData.Constants.MajorConstants.HeaderLength + group.TotalLength];
        MajorRecordHeaderWritable majorWritable = new MajorRecordHeaderWritable(package.MetaData, bytes);
        majorWritable.FormID = formId;
        majorWritable.FormVersion = package.MetaData.Constants.DefaultFormVersion!.Value;
        group.HeaderAndContentData.Span.CopyTo(bytes.AsSpan().Slice(majorWritable.HeaderLength));
        record = LoquiBinaryOverlayTranslation<T>.Create(new OverlayStream(bytes, package.MetaData), package, recordTypeConverter: null);
        return true;
    }
}