using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for classes that contain links and can enumerate them.
    /// </summary>
    public interface ILinkContainer
    {
        /// <summary>
        /// Enumerable of all contained links
        /// </summary>
        /// <returns>Enumerable of all contained links</returns>
        IEnumerable<ILinkGetter> Links { get; }
    }
}
