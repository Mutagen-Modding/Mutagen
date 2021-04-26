using System;

namespace Mutagen.Bethesda.Plugins.Records
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
