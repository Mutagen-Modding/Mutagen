using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for an object that can duplicate itself while maintaining internal FormLink routing between itself
    /// and other accompanying duplicated records.
    ///
    /// Eg, a set of Major Records can be duplicated with new FormKeys, but still maintain their pointers to each other's new keys.
    /// </summary>
    public interface IDuplicatable
    {
        /// <summary>
        /// Creates a new copy of the record with a new FormKey, but all the same contents.
        /// Optionally, a duplicated records tracker can be provided, which will reroute all internal FormLinks in addition.
        /// </summary>
        /// <param name="getNextFormKey">Function to retrieve new FormKeys from when needed</param>
        /// <param name="duplicatedRecordTracker">Tracker object for rerouting FormLinks</param>
        /// <returns>Duplicated object</returns>
        object Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)>? duplicatedRecordTracker = null);
    }
}
