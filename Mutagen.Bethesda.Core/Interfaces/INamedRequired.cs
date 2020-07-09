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
    public interface INamedRequired : INamedRequiredGetter
    {
        /// <summary>
        /// The display name of the record
        /// </summary>
        new String Name { get; set; }
    }

    /// <summary>
    /// An interface implemented by Major Records that have names
    /// </summary>
    public interface INamedRequiredGetter
    {
        /// <summary>
        /// The display name of the record
        /// </summary>
        String Name { get; }
    }
}
