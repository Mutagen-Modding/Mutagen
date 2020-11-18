using Loqui;
using System;
using System.Collections.Generic;
using System.Text;
using Noggog;

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
        
        /// <summary>
        /// Disables the record by setting the RecordFlag to Initially Disabled.
        /// <returns>Returns true if the disable was successful.</returns>
        /// </summary>
        public bool Disable()
        {
            if (this.IsDeleted) return false;
            EnumExt.SetFlag(MajorRecordFlagsRaw, (int) Internals.Constants.InitiallyDisabled, true);
            return true;
        }
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
