using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IMajorRecordIdentifier
    {
        /// <summary>
        /// The unique identifier assigned to the Major Record
        /// </summary>
        FormKey FormKey { get; }

        /// <summary>
        /// The usually unique string identifier assigned to the Major Record
        /// </summary>
        string? EditorID { get; }
    }
}
