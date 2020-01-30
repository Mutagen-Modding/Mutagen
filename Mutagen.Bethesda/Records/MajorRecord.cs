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
    public partial interface IMajorRecord : IFormKey, IMajorRecordCommon
    {
        new FormKey FormKey { get; }
    }

    public partial interface IMajorRecordGetter : IMajorRecordCommonGetter, IDuplicatable
    {
    }

    [DebuggerDisplay("{GetType().Name} {this.EditorID?.ToString()} {this.FormKey.ToString()}")]
    public partial class MajorRecord : ILinkContainer
    {
        public MajorRecordFlag MajorRecordFlags
        {
            get => (MajorRecordFlag)this.MajorRecordFlagsRaw;
            set => this.MajorRecordFlagsRaw = (int)value;
        }

        #region EditorID
        public bool EditorID_IsSet
        {
            get => _hasBeenSetTracker[(int)MajorRecord_FieldIndex.EditorID];
            set => _hasBeenSetTracker[(int)MajorRecord_FieldIndex.EditorID] = value;
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool IMajorRecordGetter.EditorID_IsSet => EditorID_IsSet;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private String _EditorID;
        public String EditorID
        {
            get => this._EditorID;
            set => EditorID_Set(value);
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String IMajorRecordGetter.EditorID => this.EditorID;
        public virtual void EditorID_Set(
            String value,
            bool markSet = true)
        {
            _EditorID = value;
            _hasBeenSetTracker[(int)MajorRecord_FieldIndex.EditorID] = markSet;
        }
        public void EditorID_Unset()
        {
            this.EditorID_Set(default(String), false);
        }
        #endregion

        [Flags]
        public enum MajorRecordFlag
        {
            Compressed = 0x00040000,
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string TitleString => $"{this.EditorID} - {this.FormKey.ToString()}";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool IMajorRecordCommon.IsCompressed
        {
            get => this.MajorRecordFlags.HasFlag(MajorRecordFlag.Compressed);
            set => this.MajorRecordFlags = this.MajorRecordFlags.SetFlag(MajorRecordFlag.Compressed, value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool IMajorRecordCommonGetter.IsCompressed
        {
            get => this.MajorRecordFlags.HasFlag(MajorRecordFlag.Compressed);
        }

        object IDuplicatable.Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)> duplicatedRecordTracker)
        {
            return this.Duplicate(getNextFormKey, duplicatedRecordTracker);
        }
    }
}

namespace Mutagen.Bethesda.Internals
{
    public delegate T MajorRecordActivator<T>(FormKey formKey) where T : IMajorRecordInternal;

    public partial class MajorRecordBinaryOverlay : IMajorRecordCommonGetter
    {
        public bool IsCompressed => ((MajorRecord.MajorRecordFlag)this.MajorRecordFlagsRaw).HasFlag(MajorRecord.MajorRecordFlag.Compressed);

        public Task WriteToXmlFolder(DirectoryPath? dir, string name, XElement node, int counter, ErrorMaskBuilder errorMask)
        {
            throw new NotImplementedException();
        }

        object IDuplicatable.Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)> duplicatedRecordTracker)
        {
            return ((MajorRecordCommon)this.CommonInstance()).Duplicate(this, getNextFormKey, duplicatedRecordTracker);
        }
    }
}
