using Loqui;
using Mutagen.Bethesda.Plugins.Internals;
using System;

namespace Mutagen.Bethesda.Plugins.Records
{
    /// <summary>
    /// An interface that Major Record objects implement to hook into the common systems
    /// </summary>
    public interface IMajorRecordCommon : IMajorRecordCommonGetter, IFormLinkContainer
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
        bool Disable();
    }

    /// <summary>
    /// An interface that Major Record objects implement to hook into the common getter systems
    /// </summary>
    public interface IMajorRecordCommonGetter : 
        IFormVersionGetter, 
        IFormLinkContainerGetter,
        IEquatable<IFormLinkGetter>,
        ILoquiObject,
        IMajorRecordIdentifier
    {
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
        /// Form Version of the record
        /// </summary>
        new ushort? FormVersion { get; }
    }

    public static class IMajorRecordCommonGetterExt
    {
        public static IFormLinkGetter ToFormLinkInformation(this IMajorRecordCommonGetter majorRec)
        {
            return FormLinkInformation.Factory(majorRec);
        }
    }
}
