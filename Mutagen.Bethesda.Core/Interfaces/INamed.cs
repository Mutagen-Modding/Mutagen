using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface implemented by Major Records that have names
    /// </summary>
    public interface INamed
    {
        /// <summary>
        /// The display name of the record
        /// </summary>
        String? Name { get; set; }
    }
}
