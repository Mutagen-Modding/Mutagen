using Loqui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
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
