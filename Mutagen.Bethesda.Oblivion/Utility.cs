using Loqui;
using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public class Utility
    {
        public RecordType GetRecordType<T>()
            where T : IMajorRecord
        {
            var register = LoquiRegistration.GetRegister(typeof(T));
            var regType = register.GetType();
            var trigRecordMember = regType.GetMember(Mutagen.Bethesda.Constants.TRIGGERING_RECORDTYPE_MEMBER).First();
            //trigRecordMember.
            throw new NotImplementedException();
        }
    }
}
