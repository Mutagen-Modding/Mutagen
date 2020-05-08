using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda
{
    public partial interface IMajorRecord : IMajorRecordCommon
    {
        new FormKey FormKey { get; }
    }

    public partial interface IMajorRecordGetter : IMajorRecordCommonGetter, IDuplicatable
    {
    }

    [DebuggerDisplay("{GetType().Name} {this.EditorID?.ToString()} {this.FormKey.ToString()}")]
    public partial class MajorRecord : ILinkedFormKeyContainer
    {
        #region EditorID
        public virtual String? EditorID { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String? IMajorRecordGetter.EditorID => this.EditorID;
        #endregion

        /// <summary>
        /// A convenience property to print "EditorID - FormKey"
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string TitleString => $"{this.EditorID} - {this.FormKey}";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsCompressed
        {
            get => EnumExt.HasFlag(this.MajorRecordFlagsRaw, Mutagen.Bethesda.Internals.Constants.CompressedFlag);
            set => this.MajorRecordFlagsRaw = EnumExt.SetFlag(this.MajorRecordFlagsRaw, Mutagen.Bethesda.Internals.Constants.CompressedFlag, value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool IMajorRecordCommonGetter.IsCompressed => this.IsCompressed;

        object IDuplicatable.Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)>? duplicatedRecordTracker)
        {
            return this.Duplicate(getNextFormKey, duplicatedRecordTracker);
        }
    }
}

namespace Mutagen.Bethesda.Internals
{
    public partial class MajorRecordBinaryOverlay : IMajorRecordCommonGetter
    {
        public bool IsCompressed => EnumExt.HasFlag(this.MajorRecordFlagsRaw, Mutagen.Bethesda.Internals.Constants.CompressedFlag);

        object IDuplicatable.Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)>? duplicatedRecordTracker)
        {
            return ((MajorRecordCommon)this.CommonInstance()).Duplicate(this, getNextFormKey, duplicatedRecordTracker);
        }
    }
}
