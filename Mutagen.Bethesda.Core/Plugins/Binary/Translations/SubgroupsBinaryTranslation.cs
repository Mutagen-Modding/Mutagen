using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

internal class SubgroupsBinaryTranslation<T>
    where T : class, IMajorRecord, IBinaryItem
{
    public static readonly SubgroupsBinaryTranslation<T> Instance = new();
    public delegate void FillFunc(
        MutagenFrame reader,
        T item);

    public static readonly FillFunc? Fill;
    public static readonly bool IsPartialFormable;
    public static readonly IReadOnlyCollection<int> Subgroups = Array.Empty<int>();

    static SubgroupsBinaryTranslation()
    {
        IsPartialFormable = (bool?)LoquiRegistration.GetRegister(typeof(T))!.GetType()
            .GetField(Constants.PartialFormMember)?.GetValue(null) ?? false;
        if (!IsPartialFormable) return;
        Fill = GetCreateFunc();
        Subgroups = (IReadOnlyCollection<int>?)LoquiRegistration.GetRegister(typeof(T))!.GetType()
            .GetField(Constants.SubgroupsMember)?.GetValue(null) ?? Array.Empty<int>();
    }
    
    private static FillFunc GetCreateFunc()
    {
        // var tType = typeof(T);
        // LoquiRegistration.GetRegister(typeof(T)).
        // var method = tType.GetMethods()
        //     .Where((methodInfo) => methodInfo.Name.Equals("CreateFromBinary"))
        //     .Where((methodInfo) => methodInfo.IsStatic
        //                            && methodInfo.IsPublic)
        //     .Where((methodInfo) => methodInfo.ReturnType.Equals(tType))
        //     .Where((methodInfo) => methodInfo.GetParameters().Length == 2)
        //     .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(MutagenFrame)))
        //     .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(TypedParseParams)))
        //     .FirstOrDefault();
        // if (method != null)
        // {
        //     return DelegateBuilder.BuildDelegate<FillFunc>(method);
        // }
        // else
        {
            throw new ArgumentException();
        }
    }

    public static bool TryReadOrphanedSubgroups(MutagenFrame frame,[MaybeNullWhen(false)] out T record)
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
        Fill(frame, record);
        return true;
    }
}