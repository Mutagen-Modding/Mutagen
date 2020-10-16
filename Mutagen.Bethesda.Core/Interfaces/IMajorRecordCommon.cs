using Loqui;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface that Major Record objects implement to hook into the common systems
    /// </summary>
    public interface IMajorRecordCommon : IMajorRecordCommonGetter, ILinkedFormKeyContainer
    {
        /// <summary>
        /// Marker of whether the content is compressed
        /// </summary>
        new bool IsCompressed { get; set; }

        /// <summary>
        /// Marker of whether the content is deleted
        /// </summary>
        new bool IsDeleted { get; set; }

        /// <summary>
        /// Raw integer flag data
        /// </summary>
        new int MajorRecordFlagsRaw { get; set; }
    }

    /// <summary>
    /// An interface that Major Record objects implement to hook into the common getter systems
    /// </summary>
    public interface IMajorRecordCommonGetter : IDuplicatable, IFormVersionGetter, ILinkedFormKeyContainerGetter, ILoquiObject
    {
        /// <summary>
        /// The usually unique string identifier assigned to the Major Record
        /// </summary>
        string? EditorID { get; }

        /// <summary>
        /// Marker of whether the content is compressed
        /// </summary>
        bool IsCompressed { get; }

        /// <summary>
        /// Marker of whether the content is deleted
        /// </summary>
        bool IsDeleted { get; }

        /// <summary>
        /// Raw integer flag data
        /// </summary>
        int MajorRecordFlagsRaw { get; }

        /// <summary>
        /// The unique identifier assigned to the Major Record
        /// </summary>
        FormKey FormKey { get; }

        /// <summary>
        /// Form Version of the record
        /// </summary>
        new ushort? FormVersion { get; }
    }
}
