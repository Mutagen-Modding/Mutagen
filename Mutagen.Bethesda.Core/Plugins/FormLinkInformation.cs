using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins;

public sealed record FormLinkInformation(FormKey FormKey, Type Type) : IFormLinkGetter
{
    public static readonly FormLinkInformation Null = new(FormKey.Null, typeof(IMajorRecordGetter));

    public FormKey? FormKeyNullable => FormKey;

    public bool IsNull => FormKey.IsNull;

    public override string ToString() => IFormLinkIdentifier.GetString(this);

    public static FormLinkInformation Factory<TMajorGetter>(IFormLinkGetter<TMajorGetter> link)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLinkInformation(link.FormKey, typeof(TMajorGetter));
    }

    public static bool TryFactory<TMajorGetter>(IFormLinkNullableGetter<TMajorGetter> link, [MaybeNullWhen(false)] out FormLinkInformation info)
        where TMajorGetter : class, IMajorRecordGetter
    {
        if (link.FormKeyNullable == null)
        {
            info = null;
            return false;
        }
        info = new FormLinkInformation(link.FormKey, typeof(TMajorGetter));
        return true;
    }

    public static FormLinkInformation Factory<TMajorGetter>(IFormLinkNullableGetter<TMajorGetter> link)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLinkInformation(link.FormKey, typeof(TMajorGetter));
    }

    public static FormLinkInformation Factory(IMajorRecordGetter majorRec)
    {
        return new FormLinkInformation(majorRec.FormKey, majorRec.Registration.GetterType);
    }

    public static FormLinkInformation Factory(IFormLinkIdentifier rhs) => new(rhs.FormKey, rhs.Type);

    public bool TryGetModKey([MaybeNullWhen(false)] out ModKey modKey)
    {
        modKey = FormKey.ModKey;
        return true;
    }

    public bool TryResolveFormKey(ILinkCache cache, out FormKey formKey)
    {
        formKey = FormKey;
        return true;
    }

    public static bool TryFactory(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out FormLinkInformation info)
    {
        var startIndex = str.IndexOf('<');
        if (startIndex == -1
            || !str.EndsWith(">")
            || !Plugins.FormKey.TryFactory(str.Slice(0, startIndex), out var fk))
        {
            info = default;
            return false;
        }

        str = str.Slice(startIndex + 1);
        str = str.Slice(0, str.Length - 1);

        if (str == "Bethesda.MajorRecord")
        {
            info = new FormLinkInformation(fk, typeof(IMajorRecordGetter));
            return true;
        }

        if (str.StartsWith("Mutagen."))
        {
            str = str.Slice(0, "Mutagen.".Length);
        }

        if (str.StartsWith("Bethesda."))
        {
            str = str.Slice(0, "Bethesda.".Length);
        }

        var splitIndex = str.IndexOf(".");

        if (splitIndex == -1)
        {
            info = default;
            return false;
        }

        var gameName = str.Slice(0, splitIndex);

        var typeString = $"Mutagen.Bethesda.{str}, Mutagen.Bethesda.{gameName}";

        var t = System.Type.GetType(typeString);
        if (t == null)
        {
            info = default;
            return false;
        }
        
        info = new FormLinkInformation(fk, t);
        return true;
    }

    public static FormLinkInformation Factory(ReadOnlySpan<char> str)
    {
        if (!TryFactory(str, out var fk))
        {
            throw new ArgumentException(nameof(str));
        }

        return fk;
    }

    public bool TryResolveCommon(ILinkCache cache, [MaybeNullWhen(false)] out IMajorRecordGetter majorRecord)
    {
        return cache.TryResolve<IMajorRecordGetter>(FormKey, out majorRecord);
    }
}