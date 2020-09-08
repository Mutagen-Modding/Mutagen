using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IMajorRecordContextEnumerable<TMod>
        where TMod : IMod
    {
        /// <summary>  
        /// Enumerates all contained Major Record Getters of the specified generic type  
        /// </summary>
        /// <param name="throwIfUnknown">Whether to throw an exception if type is unknown</param> 
        /// <exception cref="ArgumentException">If a non applicable type is provided, and throw parameter is on</exception>
        /// <returns>Enumerable of all applicable major records</returns>  
        IEnumerable<ModContext<TMod, TSetter, TGetter>> EnumerateMajorRecordContexts<TSetter, TGetter>(bool throwIfUnknown = true)
            where TSetter : class, IMajorRecordCommon, TGetter
            where TGetter : class, IMajorRecordCommonGetter;

        /// <summary>  
        /// Enumerates all contained Major Record Getters of the specified type  
        /// </summary>  
        /// <param name="t">Type to query and iterate</param> 
        /// <param name="throwIfUnknown">Whether to throw an exception if type is unknown</param> 
        /// <exception cref="ArgumentException">If a non applicable type is provided, and throw parameter is on</exception>
        /// <returns>Enumerable of all applicable major records</returns>  
        IEnumerable<ModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> EnumerateMajorRecordContexts(Type t, bool throwIfUnknown = true);
    }
}
