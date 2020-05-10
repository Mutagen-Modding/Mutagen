using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for classes that contain Major Records and can enumerate them.
    /// </summary>
    public interface IMajorRecordEnumerable : IMajorRecordGetterEnumerable
    {
        /// <summary>
        /// Enumerates all contained Major Records
        /// </summary>
        /// <returns>Enumerable of all contained Major Records</returns>
        new IEnumerable<IMajorRecordCommon> EnumerateMajorRecords();
        /// <summary>
        /// Enumerates all contained Major Records of the specified generic type
        /// </summary>
        /// <returns>Enumerable of all contained Major Records</returns>
        new IEnumerable<T> EnumerateMajorRecords<T>()
            where T : class, IMajorRecordCommon;
    }

    /// <summary>
    /// An interface for classes that contain Major Record Getter interfaces and can enumerate them
    /// </summary>
    public interface IMajorRecordGetterEnumerable
    {
        /// <summary>
        /// Enumerates all contained Major Record Getters
        /// </summary>
        /// <returns>Enumerable of all contained Major Record Getters</returns>
        IEnumerable<IMajorRecordCommonGetter> EnumerateMajorRecords();
        /// <summary>
        /// Enumerates all contained Major Record Getters of the specified generic type
        /// </summary>
        /// <returns>Enumerable of all contained Major Record Getters</returns>
        IEnumerable<T> EnumerateMajorRecords<T>()
            where T : class, IMajorRecordCommonGetter;
    }
}
