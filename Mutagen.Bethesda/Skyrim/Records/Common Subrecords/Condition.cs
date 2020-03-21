using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Condition
    {
        // ToDo
        // Confirm correctness and completeness
        [Flags]
        public enum Flag
        {
            OR = 0x01,
            ParametersUseAliases = 0x02,
            UseGlobal = 0x04,
            UsePackData = 0x08,
            SwapSubjectAndTarget = 0x10
        }

        public enum RunOnType
        {
            Subject = 0,
            Target = 1,
            Reference = 2,
            CombatTarget = 3,
            LinkedReference = 4,
            QuestAlias = 5,
            PackageData = 6,
            EventData = 7,
        }

        public static Condition CreateFromBinary(
            MutagenFrame frame,
            RecordTypeConverter? recordTypeConverter)
        {
            var subRecMeta = frame.GetSubRecord();
            if (subRecMeta.RecordType != Condition_Registration.CTDA_HEADER)
            {
                throw new ArgumentException();
            }
            var flagByte = frame.GetUInt8(subRecMeta.HeaderLength);
            Condition.Flag flag = ConditionBinaryCreateTranslation.GetFlag(flagByte);
            if (flag.HasFlag(Condition.Flag.UseGlobal))
            {
                return ConditionGlobal.CreateFromBinary(frame.SpawnWithLength(subRecMeta.ContentLength, checkFraming: false));
            }
            else
            {
                return ConditionFloat.CreateFromBinary(frame.SpawnWithLength(subRecMeta.ContentLength, checkFraming: false));
            }
        }
    }

    namespace Internals
    {
        public partial class Condition_Registration
        {
            public static readonly RecordType CIS1_HEADER = new RecordType("CIS1");
            public static readonly RecordType CIS2_HEADER = new RecordType("CIS2");
        }

        public partial class ConditionBinaryCreateTranslation
        {
            public const byte CompareMask = 0xE0;
            public const byte FlagMask = 0x1F;
            public const int EventFunctionIndex = 4672;

            public static Condition.Flag GetFlag(byte b)
            {
                return (Condition.Flag)(FlagMask & b);
            }

            public static CompareOperator GetCompareOperator(byte b)
            {
                return (CompareOperator)((CompareMask & b) >> 5);
            }

            public static void FillConditionsList(IList<Condition> conditions, MutagenFrame frame)
            {
                var countMeta = frame.ReadSubRecordFrame();
                if (countMeta.Header.RecordType != Faction_Registration.CITC_HEADER
                    || countMeta.Content.Length != 4)
                {
                    throw new ArgumentException();
                }
                var count = BinaryPrimitives.ReadInt32LittleEndian(countMeta.Content);
                List<Condition> conds = new List<Condition>(count);
                for (int i = 0; i < count; i++)
                {
                    conds.Add(Condition.CreateFromBinary(frame, default(RecordTypeConverter)));
                }
                conditions.SetTo(conds);
            }

            static partial void FillBinaryFlagsCustom(MutagenFrame frame, ICondition item)
            {
                byte b = frame.ReadUInt8();
                item.Flags = GetFlag(b);
                item.CompareOperator = GetCompareOperator(b);
            }

            public static void CustomStringImports(MutagenFrame frame, IConditionData item)
            {
                if (!frame.MetaData.TryGetSubrecordFrame(frame.Reader, out var subMeta)) return;
                if (!(item is IFunctionConditionDataInternal funcData)) return;
                switch (subMeta.Header.RecordType.TypeInt)
                {
                    case 0x31534943: // CIS1
                        funcData.ParameterOneString = BinaryStringUtility.ProcessWholeToZString(subMeta.Content);
                        break;
                    case 0x32534943: // CIS2
                        funcData.ParameterTwoString = BinaryStringUtility.ProcessWholeToZString(subMeta.Content);
                        break;
                    default:
                        return;
                }
                frame.Position += subMeta.Header.TotalLength;
            }
        }

        public partial class ConditionBinaryWriteTranslation
        {
            public static byte GetFlagWriteByte(Condition.Flag flag, CompareOperator compare)
            {
                int b = ((int)flag) & 0x1F;
                int b2 = ((int)compare) << 5;
                return (byte)(b & b2);
            }

            public static void WriteConditionsList(IReadOnlyList<IConditionGetter>? condList, MutagenWriter writer)
            {
                if (condList == null) return;
                using (HeaderExport.ExportSubRecordHeader(writer, Faction_Registration.CITC_HEADER))
                {
                    writer.Write(condList.Count);
                }
                foreach (var cond in condList)
                {
                    cond.WriteToBinary(writer);
                }
            }

            static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IConditionGetter item)
            {
                writer.Write(GetFlagWriteByte(item.Flags, item.CompareOperator));
            }

            public static void CustomStringExports(MutagenWriter writer, IConditionDataGetter obj)
            {
                if (!(obj is IFunctionConditionDataGetter funcData)) return;
                if (funcData.ParameterOneString.TryGet(out var param1))
                {
                    using (HeaderExport.ExportSubRecordHeader(writer, Condition_Registration.CIS1_HEADER))
                    {
                        StringBinaryTranslation.WriteString(writer, param1, StringBinaryType.NullTerminate);
                    }
                }
                if (funcData.ParameterTwoString.TryGet(out var param2))
                {
                    using (HeaderExport.ExportSubRecordHeader(writer, Condition_Registration.CIS2_HEADER))
                    {
                        StringBinaryTranslation.WriteString(writer, param2, StringBinaryType.NullTerminate);
                    }
                }
            }
        }

        public partial class ConditionBinaryOverlay
        {
            private static RecordType[] IncludeTriggers = new RecordType[]
            {
                new RecordType("CIS1"),
                new RecordType("CIS2"),
            };

            private Condition.Flag GetFlagsCustom(int location) => ConditionBinaryCreateTranslation.GetFlag(_data.Span[location]);
            public CompareOperator CompareOperator => ConditionBinaryCreateTranslation.GetCompareOperator(_data.Span[0]);

            public static ConditionBinaryOverlay ConditionFactory(BinaryMemoryReadStream stream, BinaryOverlayFactoryPackage package)
            {
                var subRecMeta = package.Meta.GetSubRecordFrame(stream);
                if (subRecMeta.Header.RecordType != Condition_Registration.CTDA_HEADER)
                {
                    throw new ArgumentException();
                }
                Condition.Flag flag = ConditionBinaryCreateTranslation.GetFlag(subRecMeta.Content[0]);
                if (flag.HasFlag(Condition.Flag.UseGlobal))
                {
                    return ConditionGlobalBinaryOverlay.ConditionGlobalFactory(stream, package);
                }
                else
                {
                    return ConditionFloatBinaryOverlay.ConditionFloatFactory(stream, package);
                }
            }

            public static IReadOnlyList<ConditionBinaryOverlay> ConstructBinayOverlayList(BinaryMemoryReadStream stream, BinaryOverlayFactoryPackage package)
            {
                var counterMeta = package.Meta.ReadSubRecordFrame(stream);
                if (counterMeta.Header.RecordType != Faction_Registration.CITC_HEADER
                    || counterMeta.Content.Length != 4)
                {
                    throw new ArgumentException();
                }
                var count = BinaryPrimitives.ReadUInt32LittleEndian(counterMeta.Content);
                var span = stream.RemainingMemory;
                var pos = stream.Position;
                var recordLocs = ParseRecordLocations(
                    stream: stream,
                    finalPos: long.MaxValue,
                    constants: package.Meta.SubConstants,
                    trigger: Condition_Registration.CTDA_HEADER,
                    includeTriggers: IncludeTriggers,
                    skipHeader: false);
                span = span.Slice(0, stream.Position - pos);
                if (count != recordLocs.Length)
                {
                    throw new ArgumentException("Number of parsed conditions did not matched labeled count.");
                }
                return BinaryOverlaySetList<ConditionBinaryOverlay>.FactoryByArray(
                    mem: span,
                    package: package,
                    getter: (s, p) => ConditionBinaryOverlay.ConditionFactory(new BinaryMemoryReadStream(s), p),
                    locs: recordLocs);
            }
        }

        public partial class ConditionGlobalBinaryCreateTranslation
        {
            static partial void FillBinaryDataCustom(MutagenFrame frame, IConditionGlobal item)
            {
                var functionIndex = frame.GetUInt16();
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    item.Data = GetEventData.CreateFromBinary(frame);
                }
                else
                {
                    item.Data = FunctionConditionData.CreateFromBinary(frame);
                }
            }

            static partial void CustomBinaryEndImport(MutagenFrame frame, IConditionGlobal obj)
            {
                ConditionBinaryCreateTranslation.CustomStringImports(frame, obj.Data);
            }
        }

        public partial class ConditionGlobalBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IConditionGlobalGetter item)
            {
                item.Data.WriteToBinary(writer);
            }

            static partial void CustomBinaryEndExport(MutagenWriter writer, IConditionGlobalGetter obj)
            {
                ConditionBinaryWriteTranslation.CustomStringExports(writer, obj.Data);
            }
        }

        public partial class ConditionFloatBinaryCreateTranslation
        {
            static partial void FillBinaryDataCustom(MutagenFrame frame, IConditionFloat item)
            {
                var functionIndex = frame.GetUInt16();
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    item.Data = GetEventData.CreateFromBinary(frame);
                }
                else
                {
                    item.Data = FunctionConditionData.CreateFromBinary(frame);
                }
            }

            static partial void CustomBinaryEndImport(MutagenFrame frame, IConditionFloat obj)
            {
                ConditionBinaryCreateTranslation.CustomStringImports(frame, obj.Data);
            }
        }

        public partial class ConditionFloatBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IConditionFloatGetter item)
            {
                item.Data.WriteToBinary(writer);
            }

            static partial void CustomBinaryEndExport(MutagenWriter writer, IConditionFloatGetter obj)
            {
                ConditionBinaryWriteTranslation.CustomStringExports(writer, obj.Data);
            }
        }

        public partial class FunctionConditionDataBinaryCreateTranslation
        {
            static partial void FillBinaryParameterParsingCustom(MutagenFrame frame, IFunctionConditionDataInternal item)
            {
                item.ParameterOneNumber = frame.ReadInt32();
                item.ParameterTwoNumber = frame.ReadInt32();
                item.ParameterOneRecord.FormKey = FormKey.Factory(frame.MasterReferences!, (uint)item.ParameterOneNumber);
                item.ParameterTwoRecord.FormKey = FormKey.Factory(frame.MasterReferences!, (uint)item.ParameterTwoNumber);
                item.Unknown3 = frame.ReadInt32();
                item.Unknown4 = frame.ReadInt32();
                item.Unknown5 = frame.ReadInt32();
            }
        }

        public partial class FunctionConditionDataBinaryWriteTranslation
        {
            static partial void WriteBinaryParameterParsingCustom(MutagenWriter writer, IFunctionConditionDataGetter item)
            {
                writer.Write(item.ParameterOneNumber);
                writer.Write(item.ParameterTwoNumber);
                writer.Write(item.Unknown3);
                writer.Write(item.Unknown4);
                writer.Write(item.Unknown5);
            }
        }

        public partial class ConditionFloatBinaryOverlay
        {
            private IConditionDataGetter GetDataCustom(int location)
            {
                var functionIndex = BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(location));
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    return GetEventDataBinaryOverlay.GetEventDataFactory(new BinaryMemoryReadStream(_data.Slice(location)), _package);
                }
                else
                {
                    return FunctionConditionDataBinaryOverlay.FunctionConditionDataFactory(new BinaryMemoryReadStream(_data.Slice(location)), _package);
                }
            }

            partial void CustomCtor(IBinaryReadStream stream, int finalPos, int offset)
            {
                stream.Position = offset;
                _data = stream.RemainingMemory;
            }
        }

        public partial class ConditionGlobalBinaryOverlay
        {
            private IConditionDataGetter GetDataCustom(int location)
            {
                var functionIndex = BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(location));
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    return GetEventDataBinaryOverlay.GetEventDataFactory(new BinaryMemoryReadStream(_data.Slice(location)), _package);
                }
                else
                {
                    return FunctionConditionDataBinaryOverlay.FunctionConditionDataFactory(new BinaryMemoryReadStream(_data.Slice(location)), _package);
                }
            }

            partial void CustomCtor(IBinaryReadStream stream, int finalPos, int offset)
            {
                stream.Position = offset;
                _data = stream.RemainingMemory;
            }
        }

        public partial class FunctionConditionDataBinaryOverlay
        {
            private ReadOnlyMemorySlice<byte> _data2;

            public IFormLinkGetter<ISkyrimMajorRecordGetter> ParameterOneRecord => new FormLink<ISkyrimMajorRecordGetter>(FormKey.Factory(_package.MasterReferences, BinaryPrimitives.ReadUInt32LittleEndian(_data2)));

            public int ParameterOneNumber => BinaryPrimitives.ReadInt32LittleEndian(_data2);

            public IFormLinkGetter<ISkyrimMajorRecordGetter> ParameterTwoRecord => new FormLink<ISkyrimMajorRecordGetter>(FormKey.Factory(_package.MasterReferences, BinaryPrimitives.ReadUInt32LittleEndian(_data2.Slice(4))));

            public int ParameterTwoNumber => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(4));
            
            public int Unknown3 => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(8));

            public int Unknown4 => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(12));

            public int Unknown5 => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(16));

            private ReadOnlyMemorySlice<byte> _stringParamData1;
            public bool ParameterOneString_IsSet { get; private set; }
            public string? ParameterOneString => ParameterOneString_IsSet ? BinaryStringUtility.ProcessWholeToZString(_stringParamData1) : null;

            private ReadOnlyMemorySlice<byte> _stringParamData2;
            public bool ParameterTwoString_IsSet { get; private set; }
            public string? ParameterTwoString => ParameterTwoString_IsSet ? BinaryStringUtility.ProcessWholeToZString(_stringParamData2) : null;

            partial void CustomCtor(IBinaryReadStream stream, int finalPos, int offset)
            {
                _data2 = stream.RemainingMemory.Slice(4, 0x14);
                stream.Position += 0x18;
                if (stream.Complete || !_package.Meta.TryGetSubrecord(stream, out var subFrame)) return;
                switch (subFrame.RecordTypeInt)
                {
                    case 0x31534943: // CIS1
                        _stringParamData1 = stream.RemainingMemory.Slice(subFrame.HeaderLength, subFrame.ContentLength);
                        ParameterOneString_IsSet = true;
                        break;
                    case 0x32534943: // CIS2
                        _stringParamData2 = stream.RemainingMemory.Slice(subFrame.HeaderLength, subFrame.ContentLength);
                        ParameterTwoString_IsSet = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
