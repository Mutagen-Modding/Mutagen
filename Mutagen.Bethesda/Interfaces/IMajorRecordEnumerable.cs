using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IMajorRecordEnumerable : IMajorRecordGetterEnumerable
    {
        new IEnumerable<IMajorRecordCommon> EnumerateMajorRecords();
    }

    public interface IMajorRecordGetterEnumerable
    {
        IEnumerable<IMajorRecordCommonGetter> EnumerateMajorRecords();
    }
}
