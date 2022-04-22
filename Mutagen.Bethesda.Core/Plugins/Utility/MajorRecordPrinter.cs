using Loqui;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Utility;

public static class MajorRecordPrinter<TMajor>
    where TMajor : class, IMajorRecordGetter
{
    private static readonly string _TypeString;

    static MajorRecordPrinter()
    {
        var t = typeof(TMajor);
        if (LoquiRegistration.TryGetRegister(t, out var regis))
        {
            _TypeString = $"{regis.ProtocolKey.Namespace}.{t.Name}";
        }
        else
        {
            _TypeString = t.Name;
        }
    }

    public static string TypeString => _TypeString;

    public static string ToString(IMajorRecordGetter majorRec)
    {
        if (majorRec.EditorID is {} edid)
        {
            return $"{TypeString} {edid} {majorRec.FormKey}";
        }
        return $"{TypeString} {majorRec.FormKey}";
    }
}