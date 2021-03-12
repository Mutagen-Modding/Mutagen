using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for classes that contain FormKeys and can enumerate them.
    /// </summary>
    public interface IFormLinkContainer : IFormLinkContainerGetter
    {
        /// <summary>
        /// Swaps out all links to point to new FormKeys
        /// </summary>
        void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping);
    }

    /// <summary>
    /// An interface for classes that contain FormKeys and can enumerate them.
    /// </summary>
    public interface IFormLinkContainerGetter
    {
        /// <summary>
        /// Enumerable of all contained FormKeys
        /// </summary>
        IEnumerable<FormLinkInformation> ContainedFormLinks { get; }
    }
}
