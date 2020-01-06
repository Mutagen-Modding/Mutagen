using Loqui.Internal;
using Mutagen.Bethesda.Binary;
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
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask)
        {
            var subRecMeta = frame.MetaData.GetSubRecord(frame);
            if (subRecMeta.RecordType != Condition_Registration.CTDA_HEADER)
            {
                throw new ArgumentException();
            }
            var flagByte = frame.GetUInt8(subRecMeta.HeaderLength);
            Condition.Flag flag = ConditionBinaryCreateTranslation.GetFlag(flagByte);
            if (flag.HasFlag(Condition.Flag.UseGlobal))
            {
                return ConditionGlobal.CreateFromBinary(frame.SpawnWithLength(subRecMeta.RecordLength, checkFraming: false), masterReferences);
            }
            else
            {
                return ConditionFloat.CreateFromBinary(frame.SpawnWithLength(subRecMeta.RecordLength, checkFraming: false), masterReferences);
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

            public static void FillConditionsList(IList<Condition> conditions, MutagenFrame frame, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var countMeta = frame.MetaData.ReadSubRecordFrame(frame);
                if (countMeta.Header.RecordType != Faction_Registration.CITC_HEADER
                    || countMeta.ContentSpan.Length != 4)
                {
                    throw new ArgumentException();
                }
                var count = BinaryPrimitives.ReadInt32LittleEndian(countMeta.ContentSpan);
                List<Condition> conds = new List<Condition>(count);
                for (int i = 0; i < count; i++)
                {
                    conds.Add(Condition.CreateFromBinary(frame, masterReferences, default(RecordTypeConverter), errorMask));
                }
                conditions.SetTo(conds);
            }

            static partial void FillBinaryFlagsCustom(MutagenFrame frame, ICondition item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                byte b = frame.ReadUInt8();
                item.Flags = GetFlag(b);
                item.CompareOperator = GetCompareOperator(b);
            }

            public static void CustomStringImports(MutagenFrame frame, IConditionData item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (!frame.MetaData.TryGetSubrecordFrame(frame.Reader, out var subMeta)) return;
                if (!(item is IFunctionConditionDataInternal funcData)) return;
                switch (subMeta.Header.RecordType.TypeInt)
                {
                    case 0x31534943: // CIS1
                        funcData.ParameterOneString = BinaryStringUtility.ProcessWholeToZString(subMeta.ContentSpan);
                        break;
                    case 0x32534943: // CIS2
                        funcData.ParameterTwoString = BinaryStringUtility.ProcessWholeToZString(subMeta.ContentSpan);
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

            public static void WriteConditionsList(IReadOnlySetList<IConditionGetter> condList, MutagenWriter writer, MasterReferences masterReferences)
            {
                if (!condList.HasBeenSet) return;
                using (HeaderExport.ExportSubRecordHeader(writer, Faction_Registration.CITC_HEADER))
                {
                    writer.Write(condList.Count);
                }
                foreach (var cond in condList)
                {
                    cond.WriteToBinary(writer, masterReferences);
                }
            }

            static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IConditionGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                writer.Write(GetFlagWriteByte(item.Flags, item.CompareOperator));
            }

            public static void CustomStringExports(MutagenWriter writer, IConditionDataGetter obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (!(obj is IFunctionConditionDataGetter funcData)) return;
                if (funcData.ParameterOneString_IsSet)
                {
                    using (HeaderExport.ExportSubRecordHeader(writer, Condition_Registration.CIS1_HEADER))
                    {
                        StringBinaryTranslation.WriteString(writer, funcData.ParameterOneString, true);
                    }
                }
                if (funcData.ParameterTwoString_IsSet)
                {
                    using (HeaderExport.ExportSubRecordHeader(writer, Condition_Registration.CIS2_HEADER))
                    {
                        StringBinaryTranslation.WriteString(writer, funcData.ParameterTwoString, true);
                    }
                }
            }
        }

        public partial class ConditionBinaryWrapper
        {
            private static RecordType[] IncludeTriggers = new RecordType[]
            {
                new RecordType("CIS1"),
                new RecordType("CIS2"),
            };

            private Condition.Flag GetFlagsCustom(int location) => ConditionBinaryCreateTranslation.GetFlag(_data.Span[location]);
            public CompareOperator CompareOperator => ConditionBinaryCreateTranslation.GetCompareOperator(_data.Span[0]);

            public static ConditionBinaryWrapper ConditionFactory(BinaryMemoryReadStream stream, BinaryWrapperFactoryPackage package)
            {
                var subRecMeta = package.Meta.GetSubRecordFrame(stream);
                if (subRecMeta.Header.RecordType != Condition_Registration.CTDA_HEADER)
                {
                    throw new ArgumentException();
                }
                Condition.Flag flag = ConditionBinaryCreateTranslation.GetFlag(subRecMeta.ContentSpan[0]);
                if (flag.HasFlag(Condition.Flag.UseGlobal))
                {
                    return ConditionGlobalBinaryWrapper.ConditionGlobalFactory(stream, package);
                }
                else
                {
                    return ConditionFloatBinaryWrapper.ConditionFloatFactory(stream, package);
                }
            }

            public static IReadOnlySetList<ConditionBinaryWrapper> ConstructBinayWrapperList(BinaryMemoryReadStream stream, BinaryWrapperFactoryPackage package)
            {
                var counterMeta = package.Meta.ReadSubRecordFrame(stream);
                if (counterMeta.Header.RecordType != Faction_Registration.CITC_HEADER
                    || counterMeta.ContentSpan.Length != 4)
                {
                    throw new ArgumentException();
                }
                var count = BinaryPrimitives.ReadUInt32LittleEndian(counterMeta.ContentSpan);
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
                return BinaryWrapperSetList<ConditionBinaryWrapper>.FactoryByArray(
                    mem: span,
                    package: package,
                    getter: (s, p) => ConditionBinaryWrapper.ConditionFactory(new BinaryMemoryReadStream(s), p),
                    locs: recordLocs);
            }
        }

        public partial class ConditionGlobalBinaryCreateTranslation
        {
            static partial void FillBinaryDataCustom(MutagenFrame frame, IConditionGlobal item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var functionIndex = frame.GetUInt16();
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    item.Data = GetEventData.CreateFromBinary(frame, masterReferences);
                }
                else
                {
                    item.Data = FunctionConditionData.CreateFromBinary(frame, masterReferences);
                }
            }

            static partial void CustomBinaryEndImport(MutagenFrame frame, IConditionGlobal obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                ConditionBinaryCreateTranslation.CustomStringImports(frame, obj.Data, masterReferences, errorMask);
            }
        }

        public partial class ConditionGlobalBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IConditionGlobalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                item.Data.WriteToBinary(writer, masterReferences);
            }

            static partial void CustomBinaryEndExport(MutagenWriter writer, IConditionGlobalGetter obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                ConditionBinaryWriteTranslation.CustomStringExports(writer, obj.Data, masterReferences, errorMask);
            }
        }

        public partial class ConditionFloatBinaryCreateTranslation
        {
            static partial void FillBinaryDataCustom(MutagenFrame frame, IConditionFloat item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var functionIndex = frame.GetUInt16();
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    item.Data = GetEventData.CreateFromBinary(frame, masterReferences);
                }
                else
                {
                    item.Data = FunctionConditionData.CreateFromBinary(frame, masterReferences);
                }
            }

            static partial void CustomBinaryEndImport(MutagenFrame frame, IConditionFloat obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                ConditionBinaryCreateTranslation.CustomStringImports(frame, obj.Data, masterReferences, errorMask);
            }
        }

        public partial class ConditionFloatBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IConditionFloatGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                item.Data.WriteToBinary(writer, masterReferences);
            }

            static partial void CustomBinaryEndExport(MutagenWriter writer, IConditionFloatGetter obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                ConditionBinaryWriteTranslation.CustomStringExports(writer, obj.Data, masterReferences, errorMask);
            }
        }

        public partial class FunctionConditionDataBinaryCreateTranslation
        {
            static partial void FillBinaryParameterParsingCustom(MutagenFrame frame, IFunctionConditionDataInternal item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                item.ParameterOneNumber = frame.ReadInt32();
                item.ParameterTwoNumber = frame.ReadInt32();
                item.ParameterOneRecord = new FormIDLink<SkyrimMajorRecord>(FormKey.Factory(masterReferences, (uint)item.ParameterOneNumber));
                item.ParameterTwoRecord = new FormIDLink<SkyrimMajorRecord>(FormKey.Factory(masterReferences, (uint)item.ParameterTwoNumber));
                item.Unknown3 = frame.ReadInt32();
                item.Unknown4 = frame.ReadInt32();
                item.Unknown5 = frame.ReadInt32();
            }
        }

        public partial class FunctionConditionDataBinaryWriteTranslation
        {
            static partial void WriteBinaryParameterParsingCustom(MutagenWriter writer, IFunctionConditionDataGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                writer.Write(item.ParameterOneNumber);
                writer.Write(item.ParameterTwoNumber);
                writer.Write(item.Unknown3);
                writer.Write(item.Unknown4);
                writer.Write(item.Unknown5);
            }
        }

        public partial class ConditionFloatBinaryWrapper
        {
            private IConditionDataGetter GetDataCustom(int location)
            {
                var functionIndex = BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(location));
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    return GetEventDataBinaryWrapper.GetEventDataFactory(new BinaryMemoryReadStream(_data.Slice(location)), _package);
                }
                else
                {
                    return FunctionConditionDataBinaryWrapper.FunctionConditionDataFactory(new BinaryMemoryReadStream(_data.Slice(location)), _package);
                }
            }

            partial void CustomCtor(IBinaryReadStream stream, int finalPos, int offset)
            {
                stream.Position = offset;
                _data = stream.RemainingMemory;
            }
        }

        public partial class ConditionGlobalBinaryWrapper
        {
            private IConditionDataGetter GetDataCustom(int location)
            {
                var functionIndex = BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(location));
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    return GetEventDataBinaryWrapper.GetEventDataFactory(new BinaryMemoryReadStream(_data.Slice(location)), _package);
                }
                else
                {
                    return FunctionConditionDataBinaryWrapper.FunctionConditionDataFactory(new BinaryMemoryReadStream(_data.Slice(location)), _package);
                }
            }

            partial void CustomCtor(IBinaryReadStream stream, int finalPos, int offset)
            {
                stream.Position = offset;
                _data = stream.RemainingMemory;
            }
        }

        public partial class FunctionConditionDataBinaryWrapper
        {
            private ReadOnlyMemorySlice<byte> _data2;

            public IFormIDLinkGetter<ISkyrimMajorRecordGetter> ParameterOneRecord => new FormIDLink<ISkyrimMajorRecordGetter>(FormKey.Factory(_package.MasterReferences, BinaryPrimitives.ReadUInt32LittleEndian(_data2)));

            public int ParameterOneNumber => BinaryPrimitives.ReadInt32LittleEndian(_data2);

            public IFormIDLinkGetter<ISkyrimMajorRecordGetter> ParameterTwoRecord => new FormIDLink<ISkyrimMajorRecordGetter>(FormKey.Factory(_package.MasterReferences, BinaryPrimitives.ReadUInt32LittleEndian(_data2.Slice(4))));

            public int ParameterTwoNumber => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(4));

            public int Unknown3 => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(8));

            public int Unknown4 => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(12));

            public int Unknown5 => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(16));

            private ReadOnlyMemorySlice<byte> _stringParamData1;
            public bool ParameterOneString_IsSet { get; private set; }
            public string ParameterOneString => BinaryStringUtility.ProcessWholeToZString(_stringParamData1);

            private ReadOnlyMemorySlice<byte> _stringParamData2;
            public bool ParameterTwoString_IsSet { get; private set; }
            public string ParameterTwoString => BinaryStringUtility.ProcessWholeToZString(_stringParamData2);

            partial void CustomCtor(IBinaryReadStream stream, int finalPos, int offset)
            {
                _data2 = stream.RemainingMemory.Slice(4, 0x14);
                stream.Position += 0x18;
                if (stream.Complete || !_package.Meta.TryGetSubrecord(stream, out var subFrame)) return;
                switch (subFrame.RecordTypeInt)
                {
                    case 0x31534943: // CIS1
                        _stringParamData1 = stream.RemainingMemory.Slice(subFrame.HeaderLength, subFrame.RecordLength);
                        ParameterOneString_IsSet = true;
                        break;
                    case 0x32534943: // CIS2
                        _stringParamData2 = stream.RemainingMemory.Slice(subFrame.HeaderLength, subFrame.RecordLength);
                        ParameterTwoString_IsSet = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
