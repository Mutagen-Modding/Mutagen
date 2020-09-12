using Loqui;
using Loqui.Generation;
using Noggog;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mutagen.Bethesda.Internals;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Generation
{
    public enum ListBinaryType
    {
        SubTrigger,
        Trigger,
        CounterRecord,
        PrependCount,
        Frame
    }

    public class ListBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public virtual string TranslatorName => $"ListBinaryTranslation";

        const string AsyncItemKey = "ListAsyncItem";
        const string ThreadKey = "ListThread";
        public const string CounterRecordType = "ListCounterRecordType";
        public const string CounterByteLength = "CounterByteLength";
        public const string NullIfCounterZero = "NullIfCounterZero";

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            var list = typeGen as ListType;
            if (!Module.TryGetTypeGeneration(list.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + list.SubTypeGeneration);
            }

            var subMaskStr = subTransl.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration);
            return $"{TranslatorName}<{list.SubTypeGeneration.TypeName(getter, needsCovariance: true)}, {subMaskStr}>.Instance";
        }

        public override bool IsAsync(TypeGeneration gen, bool read)
        {
            var listType = gen as ListType;
            if (this.Module.TryGetTypeGeneration(listType.SubTypeGeneration.GetType(), out var keyGen)
                && keyGen.IsAsync(listType.SubTypeGeneration, read)) return true;
            return false;
        }

        public override void Load(ObjectGeneration obj, TypeGeneration field, XElement node)
        {
            var listType = field as ListType;
            listType.CustomData[ThreadKey] = node.GetAttribute<bool>("thread", false);
            listType.CustomData[CounterRecordType] = node.GetAttribute("counterRecType", null);
            listType.CustomData[CounterByteLength] = node.GetAttribute("counterLength", default(byte));
            listType.CustomData[NullIfCounterZero] = node.GetAttribute("nullIfCounterZero", false);
            var asyncItem = node.GetAttribute<bool>("asyncItems", false);
            if (asyncItem && listType.SubTypeGeneration is LoquiType loqui)
            {
                loqui.CustomData[LoquiBinaryTranslationGeneration.AsyncOverrideKey] = asyncItem;
            }
            var data = listType.GetFieldData();
            var subData = listType.SubTypeGeneration.GetFieldData();
            ListBinaryType listBinaryType = GetListType(listType, data, subData);
            switch (listBinaryType)
            {
                case ListBinaryType.CounterRecord:
                    subData.HandleTrigger = listType.SubTypeGeneration is LoquiType;
                    break;
                default:
                    break;
            }

            if (listType.CustomData[CounterRecordType] != null
                && ((byte)listType.CustomData[CounterByteLength]) == 0)
            {
                listType.CustomData[CounterByteLength] = (byte)4;
            }
        }

        private ListBinaryType GetListType(
            ListType list,
            MutagenFieldData data,
            MutagenFieldData subData)
        {
            if (list.CustomData.TryGetValue(CounterRecordType, out var counterRecTypeObj)
                && counterRecTypeObj is string counterRecType
                && !string.IsNullOrWhiteSpace(counterRecType))
            {
                return ListBinaryType.CounterRecord;
            }
            if (list.CustomData.TryGetValue(CounterByteLength, out var prependCountObj)
                && prependCountObj is byte prependCount
                && prependCount > 0)
            {
                return ListBinaryType.PrependCount;
            }
            if (subData.HasTrigger)
            {
                return ListBinaryType.SubTrigger;
            }
            if (data.HasTrigger)
            {
                return ListBinaryType.Trigger;
            }
            return ListBinaryType.Frame;
        }

        protected virtual string GetWriteAccessor(Accessor itemAccessor)
        {
            return itemAccessor.Access;
        }

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor writerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor,
            Accessor converterAccessor)
        {
            var list = typeGen as ListType;
            if (!this.Module.TryGetTypeGeneration(list.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + list.SubTypeGeneration);
            }

            var data = typeGen.GetFieldData();
            if (data.MarkerType.HasValue)
            {
                fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}(writer, {objGen.RecordTypeHeaderName(data.MarkerType.Value)})) {{ }}");
            }

            var subData = list.SubTypeGeneration.GetFieldData();

            ListBinaryType listBinaryType = GetListType(list, data, subData);

            var allowDirectWrite = subTransl.AllowDirectWrite(objGen, list.SubTypeGeneration);
            var isLoqui = list.SubTypeGeneration is LoquiType;
            var listOfRecords = !isLoqui
                && listBinaryType == ListBinaryType.SubTrigger
                && allowDirectWrite;
            bool needsMasters = list.SubTypeGeneration is FormLinkType || list.SubTypeGeneration is LoquiType;

            var typeName = list.SubTypeGeneration.TypeName(getter: true, needsCovariance: true);
            if (list.SubTypeGeneration is LoquiType loqui)
            {
                typeName = loqui.TypeNameInternal(getter: true, internalInterface: true);
            }

            string suffix = string.Empty;
            switch (listBinaryType)
            {
                case ListBinaryType.CounterRecord:
                    suffix = "WithCounter";
                    break;
                default:
                    break;
            }

            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ListBinaryTranslation<{typeName}>.Instance.Write{suffix}{(listOfRecords ? "PerItem" : null)}"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"items: {GetWriteAccessor(itemAccessor)}");
                switch (listBinaryType)
                {
                    case ListBinaryType.SubTrigger:
                        break;
                    case ListBinaryType.Trigger:
                        args.Add($"recordType: recordTypeConverter.ConvertToCustom({data.TriggeringRecordSetAccessor})");
                        break;
                    case ListBinaryType.CounterRecord:
                        var counterType = new RecordType(list.CustomData[CounterRecordType] as string);
                        args.Add($"counterType: {objGen.RecordTypeHeaderName(counterType)}");
                        var counterLength = (byte)list.CustomData[CounterByteLength];
                        args.Add($"counterLength: {counterLength}");
                        if (subData.HasTrigger
                            && !subData.HandleTrigger)
                        {
                            args.Add($"recordType: recordTypeConverter.ConvertToCustom({subData.TriggeringRecordSetAccessor})");
                        }
                        else if (data.RecordType != null)
                        {
                            args.Add($"recordType: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                        }
                        if (subData.HasTrigger && !subData.HandleTrigger)
                        {
                            args.Add($"subRecordPerItem: true");
                        }
                        break;
                    case ListBinaryType.PrependCount:
                        if (data.HasTrigger && !subData.HasTrigger)
                        {
                            args.Add($"recordType: recordTypeConverter.ConvertToCustom({data.TriggeringRecordSetAccessor})");
                        }
                        byte countLen = (byte)list.CustomData[CounterByteLength];
                        switch (countLen)
                        {
                            case 2:
                                args.Add("countLengthLength: 2");
                                break;
                            case 4:
                                args.Add("countLengthLength: 4");
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                    case ListBinaryType.Frame:
                        break;
                    default:
                        break;
                }
                if (listOfRecords)
                {
                    args.Add($"recordType: recordTypeConverter.ConvertToCustom({subData.TriggeringRecordSetAccessor})");
                }
                if (this.Module.TranslationMaskParameter)
                {
                    args.Add($"translationMask: {translationMaskAccessor}");
                }
                if (list.CustomData.TryGetValue(NullIfCounterZero, out var nullIf)
                    && (bool)nullIf)
                {
                    args.Add("writeCounterIfNull: true");
                }
                if (allowDirectWrite)
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(list.SubTypeGeneration, getter: true)}.Write");
                }
                else
                {
                    args.Add((gen) =>
                    {
                        var listTranslMask = this.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetTranslationMaskTypeStr(list.SubTypeGeneration);
                        gen.AppendLine($"transl: (MutagenWriter subWriter, {typeName} subItem{(needsMasters ? $", {nameof(RecordTypeConverter)}? conv" : null)}) =>");
                        using (new BraceWrapper(gen))
                        {
                            subTransl.GenerateWrite(
                                fg: gen,
                                objGen: objGen,
                                typeGen: list.SubTypeGeneration,
                                writerAccessor: "subWriter",
                                translationAccessor: "listTranslMask",
                                itemAccessor: new Accessor($"subItem"),
                                errorMaskAccessor: null,
                                converterAccessor: needsMasters ? "conv" : null);
                        }
                    });
                }
            }
        }

        public override async Task GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            var list = typeGen as ListType;
            var data = list.GetFieldData();
            var subData = list.SubTypeGeneration.GetFieldData();
            if (!this.Module.TryGetTypeGeneration(list.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + list.SubTypeGeneration);
            }

            var isAsync = subTransl.IsAsync(list.SubTypeGeneration, read: true);
            ListBinaryType listBinaryType = GetListType(list, data, subData);

            if (typeGen.CustomData.TryGetValue(CounterRecordType, out var counterRecObj)
                && counterRecObj is string counterRecType)
            {
                // Nothing
            }
            else if (data.MarkerType.HasValue)
            {
                fg.AppendLine($"frame.Position += frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)} + contentLength; // Skip marker");
            }
            else if (listBinaryType == ListBinaryType.Trigger || (data.HasTrigger && !subData.HasTrigger))
            {
                fg.AppendLine($"frame.Position += frame.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
            }

            bool threading = list.CustomData.TryGetValue(ThreadKey, out var t)
                && (bool)t;

            bool needsRecordConv = list.SubTypeGeneration.NeedsRecordConverter();

            bool recordPerItem = false;
            if (listBinaryType == ListBinaryType.CounterRecord
                && subData.HasTrigger)
            {
                recordPerItem = true;
            }

            WrapSet(fg, itemAccessor, list, (wrapFg) =>
            {
                using (var args = new ArgsWrapper(wrapFg,
                    $"{(isAsync ? "(" : null)}{Loqui.Generation.Utility.Await(isAsync)}{this.Namespace}List{(isAsync ? "Async" : null)}BinaryTranslation<{list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)}>.Instance.Parse{(recordPerItem ? "PerItem" : null)}",
                    suffixLine: $"{Loqui.Generation.Utility.ConfigAwait(isAsync)}{(isAsync ? ")" : null)}")
                {
                    SemiColon = false,
                })
                {
                    if (list is ArrayType arr
                        && arr.FixedSize.HasValue)
                    {
                        args.AddPassArg($"frame");
                        args.Add($"amount: {arr.FixedSize.Value}");
                    }
                    else
                    {
                        switch (listBinaryType)
                        {
                            case ListBinaryType.SubTrigger:
                                args.AddPassArg($"frame");
                                if (needsRecordConv)
                                {
                                    args.Add($"triggeringRecord: {subData.TriggeringRecordSetAccessor}");
                                }
                                else
                                {
                                    args.Add($"triggeringRecord: recordTypeConverter.ConvertToCustom({subData.TriggeringRecordSetAccessor})");
                                }
                                if (list.SubTypeGeneration is LoquiType loqui
                                    && !loqui.TargetObjectGeneration.Abstract
                                    && loqui.TargetObjectGeneration.GetObjectData().TriggeringSource == null)
                                {
                                    args.Add("skipHeader: true");
                                }
                                break;
                            case ListBinaryType.Trigger:
                                args.Add($"frame: frame.SpawnWithLength(contentLength)");
                                break;
                            case ListBinaryType.CounterRecord:
                                args.AddPassArg($"frame");
                                if (typeGen.CustomData.TryGetValue(CounterRecordType, out var counterRecObj)
                                    && counterRecObj is string counterRecType)
                                {
                                    var len = (byte)typeGen.CustomData[CounterByteLength];
                                    args.Add($"countLengthLength: {len}");
                                    if (needsRecordConv)
                                    {
                                        args.Add($"countRecord: {objGen.RecordTypeHeaderName(counterRecType)}");
                                    }
                                    else
                                    {
                                        args.Add($"countRecord: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(counterRecType)})");
                                    }
                                }
                                if (data.RecordType != null)
                                {
                                    if (needsRecordConv)
                                    {
                                        args.Add($"triggeringRecord: {objGen.RecordTypeHeaderName(data.RecordType.Value)}");
                                    }
                                    else
                                    {
                                        args.Add($"triggeringRecord: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                                    }
                                }
                                else if (subData.HasTrigger)
                                {
                                    if (needsRecordConv)
                                    {
                                        args.Add($"triggeringRecord: {subData.TriggeringRecordSetAccessor}");
                                    }
                                    else
                                    {
                                        args.Add($"triggeringRecord: recordTypeConverter.ConvertToCustom({subData.TriggeringRecordSetAccessor})");
                                    }
                                }
                                break;
                            case ListBinaryType.PrependCount:
                                byte countLen = (byte)list.CustomData[CounterByteLength];
                                switch (countLen)
                                {
                                    case 2:
                                        args.Add("amount: frame.ReadUInt16()");
                                        break;
                                    case 4:
                                        args.Add("amount: frame.ReadInt32()");
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                                args.AddPassArg($"frame");
                                break;
                            case ListBinaryType.Frame:
                                args.AddPassArg($"frame");
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    if (threading)
                    {
                        args.Add($"thread: frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Parallel)}");
                    }
                    if (needsRecordConv)
                    {
                        args.AddPassArg($"recordTypeConverter");
                    }
                    var subGenTypes = subData.GenerationTypes.ToList();
                    var subGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
                    if (subGenTypes.Count <= 1
                        && subTransl.AllowDirectParse(
                            objGen,
                            typeGen: list.SubTypeGeneration,
                            squashedRepeatedList: listBinaryType == ListBinaryType.Trigger))
                    {
                        args.Add(subFg =>
                        {
                            subGen.GenerateCopyInRet(
                                fg: subFg,
                                objGen: objGen,
                                targetGen: list.SubTypeGeneration,
                                typeGen: list.SubTypeGeneration,
                                readerAccessor: "r",
                                translationAccessor: "listTranslMask",
                                retAccessor: "transl: ",
                                outItemAccessor: new Accessor("listSubItem"),
                                asyncMode: isAsync ? AsyncMode.Async : AsyncMode.Off,
                                errorMaskAccessor: "listErrMask",
                                converterAccessor: "conv",
                                inline: true);
                        });
                    }
                    else
                    {
                        args.Add((gen) =>
                        {
                            if (subGenTypes.Count <= 1)
                            {
                                if (subGen.AllowDirectParse(
                                    objGen: objGen,
                                    typeGen: list.SubTypeGeneration,
                                    squashedRepeatedList: listBinaryType == ListBinaryType.Trigger))
                                {
                                    subGen.GenerateCopyInRet(
                                        fg: gen,
                                        objGen: objGen,
                                        targetGen: list.SubTypeGeneration,
                                        typeGen: list.SubTypeGeneration,
                                        readerAccessor: "r",
                                        translationAccessor: "listTranslMask",
                                        retAccessor: "transl: ",
                                        outItemAccessor: new Accessor("listSubItem"),
                                        asyncMode: isAsync ? AsyncMode.Async : AsyncMode.Off,
                                        errorMaskAccessor: "listErrMask",
                                        converterAccessor: "conv",
                                        inline: true);
                                }
                                else
                                {
                                    gen.AppendLine($"transl: {Loqui.Generation.Utility.Async(isAsync)}(MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}{(isAsync ? null : $", out {list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)} listSubItem")}{(needsRecordConv ? $", {nameof(RecordTypeConverter)}? conv" : null)}) =>");
                                    using (new BraceWrapper(gen))
                                    {
                                        subGen.GenerateCopyInRet(
                                            fg: gen,
                                            objGen: objGen,
                                            targetGen: list.SubTypeGeneration,
                                            typeGen: list.SubTypeGeneration,
                                            readerAccessor: "r",
                                            translationAccessor: "listTranslMask",
                                            retAccessor: "return ",
                                            outItemAccessor: new Accessor("listSubItem"),
                                            asyncMode: isAsync ? AsyncMode.Async : AsyncMode.Off,
                                            errorMaskAccessor: "listErrMask",
                                            converterAccessor: "conv",
                                            inline: false);
                                    }
                                }
                            }
                            else
                            {
                                gen.AppendLine($"transl: {Loqui.Generation.Utility.Async(isAsync)}(MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}{(isAsync ? null : $", out {list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)} listSubItem")}{(needsRecordConv ? $", {nameof(RecordTypeConverter)}? conv" : null)}) =>");
                                using (new BraceWrapper(gen))
                                {
                                    gen.AppendLine("switch (header.TypeInt)");
                                    using (new BraceWrapper(gen))
                                    {
                                        foreach (var item in subGenTypes)
                                        {
                                            foreach (var trigger in item.Key)
                                            {
                                                gen.AppendLine($"case 0x{trigger.TypeInt:X}: // {trigger.Type}");
                                            }
                                            LoquiType targetLoqui = list.SubTypeGeneration as LoquiType;
                                            LoquiType specificLoqui = item.Value as LoquiType;
                                            using (new BraceWrapper(gen))
                                            {
                                                subGen.GenerateCopyInRet(
                                                    fg: gen,
                                                    objGen: objGen,
                                                    targetGen: list.SubTypeGeneration,
                                                    typeGen: item.Value,
                                                    readerAccessor: "r",
                                                    translationAccessor: "listTranslMask",
                                                    retAccessor: "return ",
                                                    outItemAccessor: new Accessor("listSubItem"),
                                                    asyncMode: AsyncMode.Async,
                                                    errorMaskAccessor: $"listErrMask",
                                                    converterAccessor: "conv",
                                                    inline: false);
                                            }
                                        }
                                        gen.AppendLine("default:");
                                        using (new DepthWrapper(gen))
                                        {
                                            gen.AppendLine("throw new NotImplementedException();");
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            });
        }

        public void WrapSet(FileGeneration fg, Accessor accessor, ListType list, Action<FileGeneration> a)
        {
            if (list.Nullable)
            {
                fg.AppendLine($"{accessor} = ");
                using (new DepthWrapper(fg))
                {
                    a(fg);
                    if (list.CustomData.TryGetValue(NullIfCounterZero, out var val)
                        && (bool)val)
                    {

                        fg.AppendLine($".CastExtendedListIfAny<{list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)}>();");
                    }
                    else
                    {
                        fg.AppendLine($".CastExtendedList<{list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)}>();");
                    }
                }
            }
            else
            {
                using (var args = new ArgsWrapper(fg,
                    $"{accessor}.SetTo"))
                {
                    args.Add(subFg => a(subFg));
                }
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor,
            Accessor converterAccessor,
            bool inline)
        {
            throw new NotImplementedException();
        }

        public override async Task GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor,
            int? currentPosition,
            string passedLengthAccessor,
            DataType dataType = null)
        {
            ListType list = typeGen as ListType;
            var data = list.GetFieldData();
            switch (data.BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    break;
                case BinaryGenerationType.NoGeneration:
                    return;
                case BinaryGenerationType.Custom:
                    if (typeGen.GetFieldData().HasTrigger)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"partial void {typeGen.Name}CustomParse"))
                        {
                            args.Add($"{nameof(OverlayStream)} stream");
                            args.Add($"long finalPos");
                            args.Add($"int offset");
                            args.Add($"{nameof(RecordType)} type");
                            args.Add($"int? lastParsed");
                        }
                    }
                    return;
                default:
                    throw new NotImplementedException();
            }
            var subGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
            var subData = list.SubTypeGeneration.GetFieldData();
            ListBinaryType listBinaryType = GetListType(list, data, subData);
            var expLen = await subGen.ExpectedLength(objGen, list.SubTypeGeneration);
            if (list.SubTypeGeneration is LoquiType loqui)
            {
                var typeName = this.Module.BinaryOverlayClassName(loqui);
                switch (listBinaryType)
                {
                    case ListBinaryType.PrependCount
                    when !data.HasTrigger:
                        if (expLen.HasValue)
                        {
                            fg.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => BinaryOverlayList.FactoryByCountLength<{typeName}>({dataAccessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, _package, {expLen}, countLength: {(byte)list.CustomData[CounterByteLength]}, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                        }
                        else if (objGen.Fields.Last() == typeGen)
                        {
                            fg.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => BinaryOverlayList.FactoryByLazyParse<{typeName}>({dataAccessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, _package, countLength: {(byte)list.CustomData[CounterByteLength]}, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    default:
                        if (data.HasTrigger)
                        {
                            fg.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} {{ get; private set; }}{(typeGen.Nullable ? null : $" = ListExt.Empty<{typeName}>();")}");
                        }
                        else
                        {
                            fg.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => BinaryOverlayList.FactoryByLazyParse<{typeName}>({dataAccessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, _package, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                        }
                        break;
                }
            }
            else if (data.HasTrigger)
            {
                fg.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} {{ get; private set; }}{(typeGen.Nullable ? null : $" = ListExt.Empty<{list.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}>();")}");
            }
            else
            {
                var typeName = list.SubTypeGeneration.TypeName(getter: true, needsCovariance: true);
                switch (listBinaryType)
                {
                    case ListBinaryType.CounterRecord:
                        throw new NotImplementedException();
                    case ListBinaryType.PrependCount:
                        if (expLen.HasValue)
                        {
                            fg.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => BinaryOverlayList.FactoryByCountLength<{typeName}>({dataAccessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, _package, {expLen}, countLength: {(byte)list.CustomData[CounterByteLength]}, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                        }
                        else if (list.SubTypeGeneration is StringType str
                            && (str.BinaryType == StringBinaryType.PrependLength
                            || str.BinaryType == StringBinaryType.PrependLengthUShort))
                        {
                            fg.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => BinaryOverlayList.FactoryByCountLength<{typeName}>({dataAccessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, _package, countLength: {(byte)list.CustomData[CounterByteLength]}, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                        }
                        break;
                    default:
                        fg.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{(typeGen.Nullable ? "?" : null)} {typeGen.Name} => BinaryOverlayList.FactoryByStartIndex<{list.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}>({dataAccessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, _package, {expLen}, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                        break;
                }
            }
        }

        public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            return null;
        }

        public override async Task GenerateWrapperRecordTypeParse(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor locationAccessor,
            Accessor packageAccessor,
            Accessor converterAccessor)
        {
            ListType list = typeGen as ListType;
            var data = list.GetFieldData();
            switch (data.BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    break;
                case BinaryGenerationType.NoGeneration:
                    return;
                case BinaryGenerationType.Custom:
                    using (var args = new ArgsWrapper(fg,
                        $"{typeGen.Name}CustomParse"))
                    {
                        args.AddPassArg($"stream");
                        args.AddPassArg($"finalPos");
                        args.AddPassArg($"offset");
                        args.AddPassArg($"type");
                        args.AddPassArg($"lastParsed");
                    }
                    return;
                default:
                    throw new NotImplementedException();
            }

            if (data.MarkerType.HasValue)
            {
                fg.AppendLine($"stream.Position += {packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength; // Skip marker");
            }
            var subData = list.SubTypeGeneration.GetFieldData();
            var subGenTypes = subData.GenerationTypes.ToList();
            ListBinaryType listBinaryType = GetListType(list, data, subData);
            var subGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
            string typeName;
            LoquiType loqui = list.SubTypeGeneration as LoquiType;
            if (loqui != null)
            {
                typeName = this.Module.BinaryOverlayClassName(loqui);
            }
            else
            {
                typeName = list.SubTypeGeneration.TypeName(getter: true, needsCovariance: true);
            }
            var expectedLen = await subGen.ExpectedLength(objGen, list.SubTypeGeneration);
            switch (listBinaryType)
            {
                case ListBinaryType.SubTrigger:
                    if (loqui != null)
                    {
                        if (loqui.TargetObjectGeneration.IsTypelessStruct())
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"this.{typeGen.Name} = this.{nameof(BinaryOverlay.ParseRepeatedTypelessSubrecord)}<{typeName}>"))
                            {
                                args.AddPassArg("stream");
                                args.Add($"recordTypeConverter: {converterAccessor}");
                                args.Add($"trigger: {subData.TriggeringRecordSetAccessor}");
                                if (subGenTypes.Count <= 1)
                                {
                                    args.Add($"factory:  {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory");
                                }
                                else
                                {
                                    args.Add((subFg) =>
                                    {
                                        subFg.AppendLine("factory: (s, r, p, recConv) =>");
                                        using (new BraceWrapper(subFg))
                                        {
                                            subFg.AppendLine("switch (r.TypeInt)");
                                            using (new BraceWrapper(subFg))
                                            {
                                                foreach (var item in subGenTypes)
                                                {
                                                    foreach (var trigger in item.Key)
                                                    {
                                                        subFg.AppendLine($"case 0x{trigger.TypeInt.ToString("X")}: // {trigger.Type}");
                                                    }
                                                    using (new DepthWrapper(subFg))
                                                    {
                                                        LoquiType specificLoqui = item.Value as LoquiType;
                                                        subFg.AppendLine($"return {this.Module.BinaryOverlayClassName(specificLoqui.TargetObjectGeneration)}.{specificLoqui.TargetObjectGeneration.Name}Factory(s, p);");
                                                    }
                                                }
                                                subFg.AppendLine("default:");
                                                using (new DepthWrapper(subFg))
                                                {
                                                    subFg.AppendLine("throw new NotImplementedException();");
                                                }
                                            }
                                        }
                                    });
                                }
                                if (loqui != null
                                    && !loqui.TargetObjectGeneration.Abstract
                                    && loqui.TargetObjectGeneration.GetObjectData().TriggeringSource == null)
                                {
                                    args.Add("skipHeader: true");
                                }
                            }
                        }
                        else
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"this.{typeGen.Name} = BinaryOverlayList.FactoryByArray<{typeName}>"))
                            {
                                args.Add($"mem: stream.RemainingMemory");
                                args.Add($"package: _package");
                                args.Add($"recordTypeConverter: {converterAccessor}");
                                args.Add($"getter: (s, p, recConv) => {typeName}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(OverlayStream)}(s, p), p, recConv)");
                                args.Add(subFg =>
                                {
                                    using (var subArgs = new FunctionWrapper(subFg,
                                        $"locs: {nameof(BinaryOverlay.ParseRecordLocations)}"))
                                    {
                                        subArgs.AddPassArg("stream");
                                        subArgs.Add("trigger: type");
                                        switch (loqui.TargetObjectGeneration.GetObjectType())
                                        {
                                            case ObjectType.Subrecord:
                                                subArgs.Add($"constants: _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}");
                                                break;
                                            case ObjectType.Record:
                                                subArgs.Add($"constants: _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.MajorConstants)}");
                                                break;
                                            case ObjectType.Group:
                                                subArgs.Add($"constants: _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.GroupConstants)}");
                                                break;
                                            case ObjectType.Mod:
                                            default:
                                                throw new NotImplementedException();
                                        }
                                        subArgs.Add("skipHeader: false");
                                    }
                                });
                            }
                        }
                    }
                    else if (expectedLen.HasValue)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"this.{typeGen.Name} = BinaryOverlayList.FactoryByArray<{typeName}>"))
                        {
                            args.Add($"mem: stream.RemainingMemory");
                            args.Add($"package: _package");
                            args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")}");
                            args.Add(subFg =>
                            {
                                using (var subArgs = new FunctionWrapper(subFg,
                                    $"locs: {nameof(BinaryOverlay.ParseRecordLocations)}"))
                                {
                                    subArgs.AddPassArg("stream");
                                    subArgs.Add($"constants: _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}");
                                    subArgs.Add("trigger: type");
                                    subArgs.Add("skipHeader: true");
                                    subArgs.Add($"recordTypeConverter: {converterAccessor}");
                                }
                            });
                        }
                    }
                    else
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"this.{typeGen.Name} = BinaryOverlayList.FactoryByArray<{typeName}>"))
                        {
                            args.Add($"mem: stream.RemainingMemory");
                            args.Add($"package: _package");
                            args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, $"p.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubrecordFrame(s).Content", "p")}");
                            args.Add(subFg =>
                            {
                                using (var subArgs = new FunctionWrapper(subFg,
                                    $"locs: {nameof(BinaryOverlay.ParseRecordLocations)}"))
                                {
                                    subArgs.AddPassArg("stream");
                                    subArgs.Add($"constants: _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}");
                                    subArgs.Add("trigger: type");
                                    subArgs.Add("skipHeader: false");
                                    subArgs.Add($"recordTypeConverter: {converterAccessor}");
                                }
                            });
                        }
                    }
                    break;
                case ListBinaryType.Trigger:
                    fg.AppendLine($"var subMeta = stream.ReadSubrecord();");
                    fg.AppendLine("var subLen = subMeta.ContentLength;");
                    if (expectedLen.HasValue)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"this.{typeGen.Name} = BinaryOverlayList.FactoryByStartIndex<{typeName}>"))
                        {
                            args.Add($"mem: stream.RemainingMemory.Slice(0, subLen)");
                            args.Add($"package: _package");
                            args.Add($"itemLength: {await subGen.ExpectedLength(objGen, list.SubTypeGeneration)}");
                            if (subGenTypes.Count <= 1)
                            {
                                args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")}");
                            }
                            else
                            {
                                args.Add((subFg) =>
                                {
                                    subFg.AppendLine("getter: (s, r, p) =>");
                                    using (new BraceWrapper(subFg))
                                    {
                                        subFg.AppendLine("switch (r.TypeInt)");
                                        using (new BraceWrapper(subFg))
                                        {
                                            foreach (var item in subGenTypes)
                                            {
                                                foreach (var trigger in item.Key)
                                                {
                                                    subFg.AppendLine($"case 0x{trigger.TypeInt.ToString("X")}: // {trigger.Type}");
                                                }
                                                using (new DepthWrapper(subFg))
                                                {
                                                    LoquiType specificLoqui = item.Value as LoquiType;
                                                    subFg.AppendLine($"return {subGen.GenerateForTypicalWrapper(objGen, specificLoqui, "s", "p")}");
                                                }
                                            }
                                            subFg.AppendLine("default:");
                                            using (new DepthWrapper(subFg))
                                            {
                                                subFg.AppendLine("throw new NotImplementedException();");
                                            }
                                        }
                                    }
                                });
                            }
                        }
                    }
                    else
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"this.{typeGen.Name} = BinaryOverlayList.FactoryByLazyParse<{typeName}>"))
                        {
                            args.Add($"mem: stream.RemainingMemory.Slice(0, subLen)");
                            args.Add($"package: _package");
                            if (subGenTypes.Count <= 1)
                            {
                                args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")}");
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                    }
                    fg.AppendLine("stream.Position += subLen;");
                    break;
                case ListBinaryType.CounterRecord:
                    var counterLen = (byte)list.CustomData[CounterByteLength];
                    if (expectedLen.HasValue)
                    {
                        var nullIfEmpty = list.CustomData.TryGetValue(NullIfCounterZero, out var nullIf) && (bool)nullIf;
                        using (var args = new ArgsWrapper(fg,
                            $"this.{typeGen.Name} = BinaryOverlayList.FactoryByCount{(subData.HasTrigger ? "PerItem" : null)}{(nullIfEmpty ? "NullIfZero" : null)}<{typeName}>"))
                        {
                            args.AddPassArg($"stream");
                            args.Add($"package: _package");
                            args.Add($"itemLength: 0x{expectedLen:X}");
                            args.Add($"countLength: {counterLen}");
                            args.Add($"countType: {objGen.RecordTypeHeaderName(new RecordType((string)typeGen.CustomData[CounterRecordType]))}");
                            if (subData.HasTrigger)
                            {
                                args.Add($"subrecordType: {subData.TriggeringRecordSetAccessor}");
                            }
                            else
                            {
                                args.Add($"subrecordType: {objGen.RecordTypeHeaderName(data.RecordType.Value)}");
                            }
                            if (subGenTypes.Count <= 1)
                            {
                                args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")}");
                            }
                            else
                            {
                                args.Add((subFg) =>
                                {
                                    subFg.AppendLine("getter: (s, r, p) =>");
                                    using (new BraceWrapper(subFg))
                                    {
                                        subFg.AppendLine("switch (r.TypeInt)");
                                        using (new BraceWrapper(subFg))
                                        {
                                            foreach (var item in subGenTypes)
                                            {
                                                foreach (var trigger in item.Key)
                                                {
                                                    subFg.AppendLine($"case 0x{trigger.TypeInt:X}: // {trigger.Type}");
                                                }
                                                using (new DepthWrapper(subFg))
                                                {
                                                    LoquiType specificLoqui = item.Value as LoquiType;
                                                    subFg.AppendLine($"return {subGen.GenerateForTypicalWrapper(objGen, specificLoqui, "s", "p")};");
                                                }
                                            }
                                            subFg.AppendLine("default:");
                                            using (new DepthWrapper(subFg))
                                            {
                                                subFg.AppendLine("throw new NotImplementedException();");
                                            }
                                        }
                                    }
                                });
                            }
                            if (subData.HasTrigger && subData.HandleTrigger)
                            {
                                args.Add("skipHeader: false");
                            }
                        }
                    }
                    else
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"this.{typeGen.Name} = BinaryOverlayList.FactoryByCountPerItem<{typeName}>"))
                        {
                            args.AddPassArg($"stream");
                            args.Add($"package: _package");
                            args.Add($"countLength: {counterLen}");
                            args.Add($"subrecordType: {subData.TriggeringRecordSetAccessor}");
                            args.Add($"countType: {objGen.RecordTypeHeaderName(new RecordType((string)typeGen.CustomData[CounterRecordType]))}");
                            args.Add($"recordTypeConverter: {converterAccessor}");
                            args.Add($"getter: (s, p, recConv) => {typeName}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(OverlayStream)}(s, p), p, recConv)");
                            args.Add("skipHeader: false");
                        }
                    }
                    break;
                case ListBinaryType.PrependCount:
                    if (data.HasTrigger)
                    {
                        fg.AppendLine($"stream.Position += _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength;");
                    }
                    byte countLen = (byte)list.CustomData[CounterByteLength];
                    switch (countLen)
                    {
                        case 2:
                            fg.AppendLine($"var count = stream.ReadUInt16();");
                            break;
                        case 4:
                            fg.AppendLine($"var count = stream.ReadUInt32();");
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    using (var args = new ArgsWrapper(fg,
                        $"this.{typeGen.Name} = BinaryOverlayList.FactoryByCount<{typeName}>"))
                    {
                        args.AddPassArg($"stream");
                        args.Add($"package: _package");
                        if (expectedLen != null)
                        {
                            args.Add($"itemLength: {expectedLen}");
                        }
                        args.AddPassArg($"count");
                        if (subGenTypes.Count <= 1)
                        {
                            args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")}");
                        }
                        else
                        {
                            args.Add((subFg) =>
                            {
                                subFg.AppendLine("getter: (s, r, p) =>");
                                using (new BraceWrapper(subFg))
                                {
                                    subFg.AppendLine("switch (r.TypeInt)");
                                    using (new BraceWrapper(subFg))
                                    {
                                        foreach (var item in subGenTypes)
                                        {
                                            foreach (var trigger in item.Key)
                                            {
                                                subFg.AppendLine($"case 0x{trigger.TypeInt:X}: // {trigger.Type}");
                                            }
                                            using (new DepthWrapper(subFg))
                                            {
                                                LoquiType specificLoqui = item.Value as LoquiType;
                                                subFg.AppendLine($"return {subGen.GenerateForTypicalWrapper(objGen, specificLoqui, "s", "p")};");
                                            }
                                        }
                                        subFg.AppendLine("default:");
                                        using (new DepthWrapper(subFg))
                                        {
                                            subFg.AppendLine("throw new NotImplementedException();");
                                        }
                                    }
                                }
                            });
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override async Task GenerateWrapperUnknownLengthParse(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            int? passedLength,
            string passedLengthAccessor)
        {
            ListType list = typeGen as ListType;
            ListBinaryType listBinaryType = GetListType(list, list.GetFieldData(), list.SubTypeGeneration.GetFieldData());
            var subGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
            var subExpLen = await subGen.ExpectedLength(objGen, list.SubTypeGeneration);
            switch (listBinaryType)
            {
                case ListBinaryType.SubTrigger:
                    break;
                case ListBinaryType.Trigger:
                    break;
                case ListBinaryType.CounterRecord:
                    break;
                case ListBinaryType.PrependCount:
                    {
                        var len = (byte)list.CustomData[CounterByteLength];
                        string readStr;
                        switch (len)
                        {
                            case 0:
                                return;
                            case 2:
                                readStr = $"ReadUInt16LittleEndian";
                                break;
                            case 4:
                                readStr = $"ReadInt32LittleEndian";
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        if (subExpLen.HasValue)
                        {
                            fg.AppendLine($"ret.{typeGen.Name}EndingPos = {(passedLengthAccessor == null ? null : $"{passedLengthAccessor} + ")}BinaryPrimitives.{readStr}(ret._data{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}) * {subExpLen.Value} + {len};");
                        }
                        else if (objGen.Fields.Last() != typeGen)
                        {
                            throw new NotImplementedException();
                        }
                    }
                    break;
                case ListBinaryType.Frame:
                    if (!list.SubTypeGeneration.GetFieldData().HasTrigger)
                    {
                        fg.AppendLine($"ret.{typeGen.Name}EndingPos = ret._data.Length;");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
