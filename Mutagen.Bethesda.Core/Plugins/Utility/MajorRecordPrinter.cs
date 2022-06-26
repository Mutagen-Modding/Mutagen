using Loqui;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Utility;

public static class MajorRecordPrinter<TMajor>
    where TMajor : class, IMajorRecordGetter
{
    public static string TypeString { get; }
    
    static MajorRecordPrinter()
    {
        var t = typeof(TMajor);
        if (LoquiRegistration.TryGetRegister(t, out var regis))
        {
            TypeString = $"{regis.ProtocolKey.Namespace}.{t.Name}";
        }
        else
        {
            TypeString = t.Name;
        }
    }

    public static string ToString(IMajorRecordGetter majorRec)
    {
        if (majorRec.EditorID is {} edid)
        {
            return $"{TypeString} {edid} {majorRec.FormKey}";
        }
        return $"{TypeString} {majorRec.FormKey}";
    }
}
