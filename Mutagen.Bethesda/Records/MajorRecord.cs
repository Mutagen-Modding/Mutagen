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
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public partial interface IMajorRecord : IFormKey, IDuplicatable, IMajorRecordCommon
    {
        new FormKey FormKey { get; }
    }

    [DebuggerDisplay("{GetType().Name} {this.EditorID?.ToString()} {this.FormKey.ToString()}")]
    public partial class MajorRecord : ILinkSubContainer
    {
        public int MajorRecordFlagsRaw { get; set; }

        public MajorRecordFlag MajorRecordFlags
        {
            get => (MajorRecordFlag)this.MajorRecordFlagsRaw;
            set => this.MajorRecordFlagsRaw = (int)value;
        }

        [Flags]
        public enum MajorRecordFlag
        {
            Compressed = 0x00040000,
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string TitleString => $"{this.EditorID} - {this.FormKey.ToString()}";

        bool IMajorRecordCommon.IsCompressed
        {
            get => this.MajorRecordFlags.HasFlag(MajorRecordFlag.Compressed);
            set => this.MajorRecordFlags.SetFlag(MajorRecordFlag.Compressed, value);
        }

        public static void Fill_Binary(
            MutagenFrame frame,
            MajorRecord record,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            Fill_Binary_Structs(
                record,
                frame,
                masterReferences,
                errorMask);
            for (int i = 0; i < MajorRecord_Registration.NumTypedFields; i++)
            {
                Fill_Binary_RecordTypes(
                    record,
                    frame,
                    masterReferences,
                    errorMask: errorMask);
            }
        }

        object IDuplicatable.Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)> duplicatedRecordTracker = null)
        {
            return this.Duplicate(getNextFormKey, duplicatedRecordTracker);
        }
    }
}
