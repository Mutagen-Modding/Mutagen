using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Condition
    {
        [Flags]
        public enum Flag
        {
            OR = 0x01,
            RunOnTarget = 0x02,
            UseGlobal = 0x04
        }
        
        static Condition CustomRecordTypeTrigger(
            MutagenFrame frame,
            RecordType recordType, 
            RecordTypeConverter recordTypeConverter,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            var pos = frame.PositionWithOffset;
            var span = frame.ReadSpan(0x1A);
            byte[] newBytes = new byte[span.Length + 4];
            span.CopyTo(newBytes.AsSpan());
            newBytes[4] = 0x18;
            newBytes[3] = (byte)'A';
            LoquiBinaryTranslation<Condition>.Instance.Parse(
                frame: new MutagenFrame(new MutagenMemoryReadStream(newBytes, frame.MetaData, offsetReference: pos)),
                item: out var item,
                errorMask: errorMask,
                masterReferences: masterReferences,
                recordTypeConverter: recordTypeConverter);
            return item;
        }
    }

    namespace Internals
    {
        public partial class ConditionBinaryCreateTranslation
        {
            public const byte Mask = 0xF0;

            public static Condition.Flag GetFlag(byte b)
            {
                return (Condition.Flag)(0xF & b);
            }

            public static CompareOperator GetCompareOperator(byte b)
            {
                return (CompareOperator)((Mask & b) >> 4);
            }

            static partial void FillBinaryInitialParserCustom(MutagenFrame frame, Condition item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                byte b = frame.ReadUInt8();
                item.Flags = GetFlag(b);
                item.CompareOperator = GetCompareOperator(b);
            }
        }

        public partial class ConditionBinaryWriteTranslation
        {
            static partial void WriteBinaryInitialParserCustom(MutagenWriter writer, IConditionInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                byte b = (byte)item.Flags;
                b |= (byte)(((int)(item.CompareOperator) * 16) & ConditionBinaryCreateTranslation.Mask);
                writer.Write(b);
            }
        }

        public partial class ConditionBinaryWrapper
        {
            public Condition.Flag Flags => ConditionBinaryCreateTranslation.GetFlag(_data.Span[0]);
            public CompareOperator CompareOperator => ConditionBinaryCreateTranslation.GetCompareOperator(_data.Span[0]);

            static ConditionBinaryWrapper CustomRecordTypeTrigger(
                BinaryMemoryReadStream stream,
                RecordType recordType,
                BinaryWrapperFactoryPackage package,
                RecordTypeConverter recordTypeConverter)
            {
                var rawBytes = stream.ReadSpan(0x1A);
                byte[] newBytes = new byte[rawBytes.Length + 4];
                rawBytes.CopyTo(newBytes.AsSpan());
                newBytes[4] = 0x18;
                newBytes[3] = (byte)'A';
                return ConditionBinaryWrapper.ConditionFactory(
                    stream: new BinaryMemoryReadStream(newBytes),
                    package: package,
                    recordTypeConverter: recordTypeConverter);
            }
        }
    }
}
