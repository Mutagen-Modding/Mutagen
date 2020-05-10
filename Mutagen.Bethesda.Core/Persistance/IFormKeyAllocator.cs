using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for something that can allocate new FormKeys when requested
    /// </summary>
    public interface IFormKeyAllocator
    {
        /// <summary>
        /// Requests a new unused FormKey, with no other requirements
        /// </summary>
        /// <returns>An unused FormKey</returns>
        FormKey GetNextFormKey();
        
        /// <summary>
        /// Requests a new unused FormKey, given an EditorID to be used for syncronization purposes.
        /// The EditorID can be used to provide persistance syncronization by the implementation.
        /// </summary>
        /// <returns>An unused FormKey</returns>
        FormKey GetNextFormKey(string editorID);
    }
}
