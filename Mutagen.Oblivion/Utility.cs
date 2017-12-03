using Loqui;
using Mutagen.Oblivion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Internals
{
    public class Utility
    {
        public RecordType GetRecordType<T>()
            where T : IMajorRecord
        {
            var register = LoquiRegistration.GetRegister(typeof(T));
            var regType = register.GetType();
            var trigRecordMember = regType.GetMember(Constants.TRIGGERING_RECORDTYPE_MEMBER).First();
            //trigRecordMember.
            throw new NotImplementedException();
        }
    }
}
