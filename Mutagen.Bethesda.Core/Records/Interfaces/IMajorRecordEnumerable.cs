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

        /// <summary>  
        /// Enumerates all contained Major Record of the specified type  
        /// </summary>  
        /// <param name="t">Type of major records to enumerate</param> 
        /// <param name="throwIfUnknown">Whether to throw an exception if type is unknown</param> 
        /// <exception cref="ArgumentException">If a non applicable type is provided, and throw parameter is on</exception>  
        /// <exception cref="ArgumentException">If a non applicable type is provided</exception>  
        /// <returns>Enumerable of all applicable major records</returns>  
        new IEnumerable<IMajorRecordCommon> EnumerateMajorRecords(Type t, bool throwIfUnknown = true);
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
        /// <returns>Enumerable of all applicable major records</returns>  
        IEnumerable<T> EnumerateMajorRecords<T>()
            where T : class, IMajorRecordCommonGetter;

        /// <summary>  
        /// Enumerates all contained Major Record Getters of the specified type  
        /// </summary>  
        /// <param name="t">Type to query and iterate</param> 
        /// <param name="throwIfUnknown">Whether to throw an exception if type is unknown</param> 
        /// <exception cref="ArgumentException">If a non applicable type is provided, and throw parameter is on</exception>  
        /// <returns>Enumerable of all applicable major records</returns>  
        IEnumerable<IMajorRecordCommonGetter> EnumerateMajorRecords(Type t, bool throwIfUnknown = true);
    }
}
