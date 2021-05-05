using System;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Utility
{
    public static class MajorRecordPrinter<TMajor>
        where TMajor : class, IMajorRecordCommonGetter
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

        public static string ToString(IMajorRecordCommonGetter majorRec)
        {
            if (majorRec.EditorID.TryGet(out var edid))
            {
                return $"{TypeString} {edid} {majorRec.FormKey}";
            }
            return $"{TypeString} {majorRec.FormKey}";
        }
    }
}
