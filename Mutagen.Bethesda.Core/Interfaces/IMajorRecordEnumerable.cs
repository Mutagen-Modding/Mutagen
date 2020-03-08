using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IMajorRecordEnumerable : IMajorRecordGetterEnumerable
    {
        new IEnumerable<IMajorRecordCommon> EnumerateMajorRecords();
        new IEnumerable<T> EnumerateMajorRecords<T>()
            where T : class, IMajorRecordCommon;
    }

    public interface IMajorRecordGetterEnumerable
    {
        IEnumerable<IMajorRecordCommonGetter> EnumerateMajorRecords();
        IEnumerable<T> EnumerateMajorRecords<T>()
            where T : class, IMajorRecordCommonGetter;
    }
}
