using Loqui;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Utility
{
    public static class MajorRecordTypePrinter<TMajor>
        where TMajor : class, IMajorRecordCommonGetter
    {
        private static readonly string _TypeString;

        static MajorRecordTypePrinter()
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
    }
}
