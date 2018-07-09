using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Notifying;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class DialogItem
    {
        public enum Flag
        {
            Goodbye = 0x001,
            Random = 0x002,
            SayOnce = 0x004,
            RunImmediately = 0x008,
            InfoRefusal = 0x010,
            RandomEnd = 0x020,
            RunForRumors = 0x040
        }

        partial void CustomCtor()
        {
            this.CompiledScript_Property.Subscribe(
                (change) =>
                {
                    this.MetadataSummary.CompiledSizeInternal = change.New.Length;
                },
                cmds: NotifyingSubscribeParameters.NoFire);
        }

        private readonly static RecordTypeConverter conditionConverter = new RecordTypeConverter(
            new KeyValuePair<RecordType, RecordType>(
                DialogItem_Registration.CTDA_HEADER,
                new RecordType("CTDT")));

        private readonly static RecordTypeConverter metaConverter = new RecordTypeConverter(
            new KeyValuePair<RecordType, RecordType>(
                new RecordType("SCHR"),
                new RecordType("SCHD")));
        
        static partial void FillBinary_ConditionsOld_Custom(MutagenFrame frame, DialogItem item, ErrorMaskBuilder errorMask)
        {
            Mutagen.Bethesda.Binary.ListBinaryTranslation<Condition>.Instance.ParseRepeatedItem(
               frame: frame,
               triggeringRecord: new RecordType("CTDT"),
               item: item.Conditions,
               fieldIndex: (int)DialogItem_FieldIndex.Conditions,
               lengthLength: Mutagen.Bethesda.Constants.SUBRECORD_LENGTHLENGTH,
               errorMask: errorMask,
               transl: (MutagenFrame r, out Condition listItem, ErrorMaskBuilder listSubMask) =>
               {
                   byte[] bytes = r.ReadBytes(0x1A);
                   bytes[4] = 0x18;
                   byte[] newBytes = new byte[bytes.Length + 4];
                   Array.Copy(bytes, newBytes, bytes.Length);
                   return LoquiBinaryTranslation<Condition>.Instance.Parse(
                       frame: new MutagenFrame(new BinaryMemoryReadStream(newBytes)),
                       item: out listItem,
                       errorMask: listSubMask,
                       recordTypeConverter: conditionConverter);
               },
               parseIndefinitely: true);
        }

        static partial void WriteBinary_ConditionsOld_Custom(MutagenWriter writer, DialogItem item, ErrorMaskBuilder errorMask)
        {
        }

        static partial void FillBinary_MetadataSummaryOld_Custom(MutagenFrame frame, DialogItem item, ErrorMaskBuilder errorMask)
        {
            var tmpMetadataSummary = ScriptMetaSummary.Create_Binary(
                frame: frame,
                errorMask: errorMask,
                recordTypeConverter: metaConverter);
            item.MetadataSummary.CopyFieldsFrom(
                rhs: tmpMetadataSummary,
                def: null,
                cmds: null,
                copyMask: null,
                errorMask: errorMask);
        }

        static partial void WriteBinary_MetadataSummaryOld_Custom(MutagenWriter writer, DialogItem item, ErrorMaskBuilder errorMask)
        {
        }
    }
}
