using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Condition
    {
        public abstract ConditionData Data { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IConditionDataGetter IConditionGetter.Data => this.Data;

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
            if (!frame.Reader.TryGetSubrecord(Mutagen.Bethesda.Skyrim.Internals.RecordTypes.CTDA, out var subRecMeta))
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

    public partial interface ICondition
    {
        new ConditionData Data { get; set; }
    }

    public partial interface IConditionGetter
    {
        IConditionDataGetter Data { get; }
    }

    namespace Internals
    {
        public partial class Condition_Registration
        {
            public static readonly RecordType CIS1 = new RecordType("CIS1");
            public static readonly RecordType CIS2 = new RecordType("CIS2");
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

            public static void FillConditionsList(IList<Condition> conditions, MutagenFrame frame, int count)
            {
                conditions.Clear();
                for (int i = 0; i < count; i++)
                {
                    conditions.Add(Condition.CreateFromBinary(frame, default(RecordTypeConverter)));
                }
            }

            public static void FillConditionsList(IList<Condition> conditions, MutagenFrame frame)
            {
                conditions.Clear();
                while (frame.Reader.TryGetSubrecord(RecordTypes.CTDA, out var subMeta))
                {
                    conditions.Add(Condition.CreateFromBinary(frame, default(RecordTypeConverter)));
                }
            }

            static partial void FillBinaryFlagsCustom(MutagenFrame frame, ICondition item)
            {
                byte b = frame.ReadUInt8();
                item.Flags = GetFlag(b);
                item.CompareOperator = GetCompareOperator(b);
            }

            public static void CustomStringImports(MutagenFrame frame, IConditionData item)
            {
                if (!frame.Reader.TryGetSubrecordFrame(out var subMeta)) return;
                if (!(item is IFunctionConditionData funcData)) return;
                switch (subMeta.RecordType.TypeInt)
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
                frame.Position += subMeta.TotalLength;
            }
        }

        public partial class ConditionBinaryWriteTranslation
        {
            public static byte GetFlagWriteByte(Condition.Flag flag, CompareOperator compare)
            {
                int b = ((int)flag) & 0x1F;
                int b2 = ((int)compare) << 5;
                return (byte)(b | b2);
            }

            public static void WriteConditionsList(IReadOnlyList<IConditionGetter> condList, MutagenWriter writer)
            {
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
                    using (HeaderExport.Subrecord(writer, Condition_Registration.CIS1))
                    {
                        StringBinaryTranslation.Instance.Write(writer, param1, StringBinaryType.NullTerminate);
                    }
                }
                if (funcData.ParameterTwoString.TryGet(out var param2))
                {
                    using (HeaderExport.Subrecord(writer, Condition_Registration.CIS2))
                    {
                        StringBinaryTranslation.Instance.Write(writer, param2, StringBinaryType.NullTerminate);
                    }
                }
            }
        }

        public abstract partial class ConditionBinaryOverlay
        {
            public abstract IConditionDataGetter Data { get; }
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IConditionDataGetter IConditionGetter.Data => this.Data;

            private static ICollectionGetter<RecordType> IncludeTriggers = new CollectionGetterWrapper<RecordType>(
                new RecordType[]
                {
                    new RecordType("CIS1"),
                    new RecordType("CIS2"),
                });

            private Condition.Flag GetFlagsCustom(int location) => ConditionBinaryCreateTranslation.GetFlag(_data.Span[location]);
            public CompareOperator CompareOperator => ConditionBinaryCreateTranslation.GetCompareOperator(_data.Span[0]);

            public static ConditionBinaryOverlay ConditionFactory(OverlayStream stream, BinaryOverlayFactoryPackage package)
            {
                var subRecMeta = stream.GetSubrecordFrame();
                if (subRecMeta.RecordType != RecordTypes.CTDA)
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

            public static IReadOnlyList<ConditionBinaryOverlay> ConstructBinayOverlayCountedList(OverlayStream stream, BinaryOverlayFactoryPackage package)
            {
                var counterMeta = stream.ReadSubrecordFrame();
                if (counterMeta.RecordType != RecordTypes.CITC
                    || counterMeta.Content.Length != 4)
                {
                    throw new ArgumentException();
                }
                var count = BinaryPrimitives.ReadUInt32LittleEndian(counterMeta.Content);
                var ret = ConstructBinayOverlayList(stream, package);
                if (count != ret.Count)
                {
                    throw new ArgumentException("Number of parsed conditions did not matched labeled count.");
                }
                return ret;
            }

            public static IReadOnlyList<ConditionBinaryOverlay> ConstructBinayOverlayList(OverlayStream stream, BinaryOverlayFactoryPackage package)
            {
                var span = stream.RemainingMemory;
                var pos = stream.Position;
                var recordLocs = ParseRecordLocations(
                    stream: stream,
                    finalPos: long.MaxValue,
                    constants: package.MetaData.Constants.SubConstants,
                    trigger: RecordTypes.CTDA,
                    includeTriggers: IncludeTriggers,
                    skipHeader: false);
                span = span.Slice(0, stream.Position - pos);
                return BinaryOverlayList.FactoryByArray<ConditionBinaryOverlay>(
                    mem: span,
                    package: package,
                    getter: (s, p) => ConditionBinaryOverlay.ConditionFactory(new OverlayStream(s, p), p),
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
            static partial void FillBinaryParameterParsingCustom(MutagenFrame frame, IFunctionConditionData item)
            {
                item.ParameterOneNumber = frame.ReadInt32();
                item.ParameterTwoNumber = frame.ReadInt32();
                item.ParameterOneRecord.FormKey = FormKey.Factory(frame.MetaData.MasterReferences!, (uint)item.ParameterOneNumber);
                item.ParameterTwoRecord.FormKey = FormKey.Factory(frame.MetaData.MasterReferences!, (uint)item.ParameterTwoNumber);
                GetEventDataBinaryCreateTranslation.FillEndingParams(frame, item);
            }
        }

        public partial class GetEventDataBinaryCreateTranslation
        {
            public static void FillEndingParams(MutagenFrame frame, IConditionData item)
            {
                item.RunOnType = EnumBinaryTranslation<Condition.RunOnType>.Instance.Parse(frame: frame.SpawnWithLength(4));
                item.Reference.SetTo(
                    Mutagen.Bethesda.Binary.FormLinkBinaryTranslation.Instance.Parse(
                        frame: frame,
                        defaultVal: FormKey.Null));
                item.Unknown3 = frame.ReadInt32();
            }

            static partial void FillBinaryParameterParsingCustom(MutagenFrame frame, IGetEventData item)
            {
                FillEndingParams(frame, item);
            }
        }

        public partial class FunctionConditionDataBinaryWriteTranslation
        {
            static partial void WriteBinaryParameterParsingCustom(MutagenWriter writer, IFunctionConditionDataGetter item)
            {
                var paramTypes = ConditionData.GetParameterTypes(item.Function);
                switch (paramTypes.First.GetCategory())
                {
                    case ConditionData.ParameterCategory.None:
                    case ConditionData.ParameterCategory.Number:
                        writer.Write(item.ParameterOneNumber);
                        break;
                    case ConditionData.ParameterCategory.Form:
                        FormKeyBinaryTranslation.Instance.Write(writer, item.ParameterOneRecord.FormKey);
                        break;
                    case ConditionData.ParameterCategory.String:
                    default:
                        throw new NotImplementedException();
                }
                switch (paramTypes.Second.GetCategory())
                {
                    case ConditionData.ParameterCategory.None:
                    case ConditionData.ParameterCategory.Number:
                        writer.Write(item.ParameterTwoNumber);
                        break;
                    case ConditionData.ParameterCategory.Form:
                        FormKeyBinaryTranslation.Instance.Write(writer, item.ParameterTwoRecord.FormKey);
                        break;
                    case ConditionData.ParameterCategory.String:
                    default:
                        throw new NotImplementedException();
                }
                GetEventDataBinaryWriteTranslation.WriteCommonParams(writer, item);
            }
        }

        public partial class GetEventDataBinaryWriteTranslation
        {
            public static void WriteCommonParams(MutagenWriter writer, IConditionDataGetter item)
            {
                Mutagen.Bethesda.Binary.EnumBinaryTranslation<Condition.RunOnType>.Instance.Write(
                    writer,
                    item.RunOnType,
                    length: 4);
                Mutagen.Bethesda.Binary.FormLinkBinaryTranslation.Instance.Write(
                    writer: writer,
                    item: item.Reference);
                writer.Write(item.Unknown3);
            }

            static partial void WriteBinaryParameterParsingCustom(MutagenWriter writer, IGetEventDataGetter item)
            {
                WriteCommonParams(writer, item);
            }
        }

        public partial class ConditionFloatBinaryOverlay
        {
            private IConditionDataGetter GetDataCustom(int location)
            {
                var functionIndex = BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(location));
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    return GetEventDataBinaryOverlay.GetEventDataFactory(new OverlayStream(_data.Slice(location), _package), _package);
                }
                else
                {
                    return FunctionConditionDataBinaryOverlay.FunctionConditionDataFactory(new OverlayStream(_data.Slice(location), _package), _package);
                }
            }

            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
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
                    return GetEventDataBinaryOverlay.GetEventDataFactory(new OverlayStream(_data.Slice(location), _package), _package);
                }
                else
                {
                    return FunctionConditionDataBinaryOverlay.FunctionConditionDataFactory(new OverlayStream(_data.Slice(location), _package), _package);
                }
            }

            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                stream.Position = offset;
                _data = stream.RemainingMemory;
            }
        }

        public partial class ConditionDataBinaryOverlay
        {
            public Condition.RunOnType RunOnType => (Condition.RunOnType)BinaryPrimitives.ReadInt32LittleEndian(_data.Span.Slice(0xC, 0x4));
            public IFormLinkGetter<ISkyrimMajorRecordGetter> Reference => new FormLink<ISkyrimMajorRecordGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data.Span.Slice(0x10, 0x4))));
            public Int32 Unknown3 => BinaryPrimitives.ReadInt32LittleEndian(_data.Slice(0x14, 0x4));
        }

        public partial class FunctionConditionDataBinaryOverlay
        {
            private ReadOnlyMemorySlice<byte> _data2;

            public IFormLinkGetter<ISkyrimMajorRecordGetter> ParameterOneRecord => new FormLink<ISkyrimMajorRecordGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data2)));

            public int ParameterOneNumber => BinaryPrimitives.ReadInt32LittleEndian(_data2);

            public IFormLinkGetter<ISkyrimMajorRecordGetter> ParameterTwoRecord => new FormLink<ISkyrimMajorRecordGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data2.Slice(4))));

            public int ParameterTwoNumber => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(4));

            private ReadOnlyMemorySlice<byte> _stringParamData1;
            public bool ParameterOneString_IsSet { get; private set; }
            public string? ParameterOneString => ParameterOneString_IsSet ? BinaryStringUtility.ProcessWholeToZString(_stringParamData1) : null;

            private ReadOnlyMemorySlice<byte> _stringParamData2;
            public bool ParameterTwoString_IsSet { get; private set; }
            public string? ParameterTwoString => ParameterTwoString_IsSet ? BinaryStringUtility.ProcessWholeToZString(_stringParamData2) : null;

            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                stream.Position -= 0x4;
                _data2 = stream.RemainingMemory.Slice(4, 0x14);
                stream.Position += 0x18;
                if (stream.Complete || !stream.TryGetSubrecord(out var subFrame)) return;
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
