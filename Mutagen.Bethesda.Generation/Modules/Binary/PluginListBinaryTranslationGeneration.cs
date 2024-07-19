using System.Xml.Linq;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;
using StringType = Mutagen.Bethesda.Generation.Fields.StringType;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public enum ListBinaryType
{
    SubTrigger,
    Trigger,
    CounterRecord,
    PrependCount,
    Frame
}

public class PluginListBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public virtual string TranslatorName => $"ListBinaryTranslation";
    const string ThreadKey = "ListThread";
    public const string CounterRecordType = "ListCounterRecordType";
    public const string CounterByteLength = "CounterByteLength";
    public const string NullIfCounterZero = "NullIfCounterZero";
    public const string AllowNoCounter = "AllowNoCounter";
    public const string EndMarker = "EndMarker";
    public const string AlwaysWriteEndMarker = "AlwaysWriteEndMarker";
    public const string Additive = "Additive";
    public const string ExpectedLengthLength = "ExpectedLengthLength";
    public const string ItemStartMarker = "ItemStartMarker";
    public const string ItemEndMarker = "ItemEndMarker";

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        return null;
    }

    public override void Load(ObjectGeneration obj, TypeGeneration field, XElement node)
    {
        var listType = field as ListType;
        listType.CustomData[ThreadKey] = node.GetAttribute<bool>("thread", false);
        listType.CustomData[CounterRecordType] = node.GetAttribute("counterRecType", null);
        listType.CustomData[CounterByteLength] = node.GetAttribute("counterLength", default(byte));
        listType.CustomData[ExpectedLengthLength] = node.GetAttribute("expectedLengthLength", default(byte?));
        listType.CustomData[NullIfCounterZero] = node.GetAttribute("nullIfCounterZero", false);
        listType.CustomData[AllowNoCounter] = node.GetAttribute("allowNoCounter", true);
        listType.CustomData[EndMarker] = node.GetAttribute("endMarker", null);
        listType.CustomData[AlwaysWriteEndMarker] = node.GetAttribute("alwaysWriteEndMarker", false);
        listType.CustomData[Additive] = node.GetAttribute("additive", false);
        listType.CustomData[ItemStartMarker] = node.GetAttribute("itemMarkerType", null);
        listType.CustomData[ItemEndMarker] = node.GetAttribute("itemEndMarkerType", null);
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

    public ListBinaryType GetListType(
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

    public override async Task GenerateWrite(
        StructuredStringBuilder sb,
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
            sb.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}(writer, {objGen.RecordTypeHeaderName(data.MarkerType.Value)})) {{ }}");
        }

        var subData = list.SubTypeGeneration.GetFieldData();

        ListBinaryType listBinaryType = GetListType(list, data, subData);

        var allowDirectWrite = subTransl.AllowDirectWrite(objGen, list.SubTypeGeneration);
        var loqui = list.SubTypeGeneration as LoquiType;
        var listOfRecords = loqui == null
                            && listBinaryType == ListBinaryType.SubTrigger
                            && allowDirectWrite;
        bool needsMasters = list.SubTypeGeneration is FormLinkType || list.SubTypeGeneration is LoquiType;

        var typeName = list.SubTypeGeneration.TypeName(getter: true, needsCovariance: true);
        if (loqui != null)
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

        using (var args = sb.Call(
                   $"{this.NamespacePrefix}ListBinaryTranslation<{typeName}>.Instance.Write{suffix}{(listOfRecords ? "PerItem" : null)}"))
        {
            args.Add($"writer: {writerAccessor}");
            args.Add($"items: {GetWriteAccessor(itemAccessor)}");
            switch (listBinaryType)
            {
                case ListBinaryType.SubTrigger:
                    break;
                case ListBinaryType.Trigger:
                    args.Add($"recordType: translationParams.ConvertToCustom({data.TriggeringRecordSetAccessor})");
                    if (data.OverflowRecordType.HasValue)
                    {
                        args.Add($"overflowRecord: {objGen.RecordTypeHeaderName(data.OverflowRecordType.Value)}");
                    }
                    break;
                case ListBinaryType.CounterRecord:
                    var counterType = new RecordType(list.CustomData[CounterRecordType] as string);
                    args.Add($"counterType: {objGen.RecordTypeHeaderName(counterType)}");
                    var counterLength = (byte)list.CustomData[CounterByteLength];
                    args.Add($"counterLength: {counterLength}");
                    if (subData.HasTrigger
                        && !subData.HandleTrigger)
                    {
                        args.Add($"recordType: translationParams.ConvertToCustom({subData.TriggeringRecordSetAccessor})");
                    }
                    else if (data.RecordType != null)
                    {
                        args.Add($"recordType: translationParams.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                    }
                    if (subData.HasTrigger && !subData.HandleTrigger)
                    {
                        args.Add($"subRecordPerItem: true");
                    }
                    break;
                case ListBinaryType.PrependCount:
                    if (data.HasTrigger && !subData.HasTrigger)
                    {
                        args.Add($"recordType: translationParams.ConvertToCustom({data.TriggeringRecordSetAccessor})");
                    }
                    byte countLen = (byte)list.CustomData[CounterByteLength];
                    switch (countLen)
                    {
                        case 1:
                            args.Add("countLengthLength: 1");
                            break;
                        case 2:
                            args.Add("countLengthLength: 2");
                            break;
                        case 4:
                            args.Add("countLengthLength: 4");
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    if (list.CustomData.TryGetValue(ExpectedLengthLength, out var expLenLenObj)
                                             && expLenLenObj is byte expLenLen)
                    {
                        if (loqui == null)
                        {
                            throw new NotImplementedException();
                        }
                        args.Add($"expectedLengthLength: {expLenLen}");
                        args.Add($"expectedLength: {await subTransl.ExpectedLength(loqui.TargetObjectGeneration, list.SubTypeGeneration) ?? throw new NotImplementedException()}");
                    }
                    break;
                case ListBinaryType.Frame:
                    break;
                default:
                    break;
            }
            if (listOfRecords)
            {
                args.Add($"recordType: translationParams.ConvertToCustom({subData.TriggeringRecordSetAccessor})");
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
            if (list.CustomData.TryGetValue(EndMarker, out var endMarkerObj) && endMarkerObj is string endMarker)
            {
                args.Add($"endMarker: RecordTypes.{endMarker}");
            }
            if (list.CustomData.TryGetValue(AlwaysWriteEndMarker, out var alwaysWriteEndMarkerObj)
                && alwaysWriteEndMarkerObj is bool alwaysWriteEndMarker
                && alwaysWriteEndMarker)
            {
                args.Add($"alwaysWriteEndMarker: true");
            }
            if (list.CustomData.TryGetValue(ItemStartMarker, out var itemStartMarkerObj) && itemStartMarkerObj is string itemStartMarker)
            {
                args.Add($"itemStartMarker: RecordTypes.{itemStartMarker}");
            }
            if (list.CustomData.TryGetValue(ItemEndMarker, out var itemEndMarkerObj) && itemEndMarkerObj is string itemEndMarker)
            {
                args.Add($"itemEndMarker: RecordTypes.{itemEndMarker}");
            }
            if (allowDirectWrite)
            {
                args.Add($"transl: {subTransl.GetTranslatorInstance(list.SubTypeGeneration, getter: true)}.Write");
            }
            else
            {
                await args.Add(async (gen) =>
                {
                    var listTranslMask = this.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetTranslationMaskTypeStr(list.SubTypeGeneration);
                    gen.AppendLine($"transl: ({nameof(MutagenWriter)} subWriter, {typeName} subItem{(needsMasters ? $", {nameof(TypedWriteParams)} conv" : null)}) =>");
                    using (gen.CurlyBrace())
                    {
                        var major = loqui != null && await loqui.TargetObjectGeneration.IsMajorRecord();
                        if (major)
                        {
                            gen.AppendLine("try");
                        }
                        using (gen.CurlyBrace(doIt: major))
                        {
                            await subTransl.GenerateWrite(
                                sb: gen,
                                objGen: objGen,
                                typeGen: list.SubTypeGeneration,
                                writerAccessor: "subWriter",
                                translationAccessor: "listTranslMask",
                                itemAccessor: new Accessor($"subItem"),
                                errorMaskAccessor: null,
                                converterAccessor: needsMasters ? "conv" : null);
                        }
                        if (major)
                        {
                            gen.AppendLine("catch (Exception ex)");
                            using (gen.CurlyBrace())
                            {
                                gen.AppendLine("throw RecordException.Enrich(ex, subItem);");
                            }
                        }
                    }
                });
            }
        }

    }

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor nodeAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
        var list = typeGen as ListType;
        var data = list.GetFieldData();
        var loqui = list.SubTypeGeneration as LoquiType;
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
            sb.AppendLine($"frame.Position += frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingMeta.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)} + contentLength; // Skip marker");
        }
        else if (listBinaryType == ListBinaryType.Trigger || (data.HasTrigger && !subData.HasTrigger))
        {
            sb.AppendLine($"frame.Position += frame.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingMeta.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
        }

        bool threading = list.CustomData.TryGetValue(ThreadKey, out var t)
                         && (bool)t;

        bool needsRecordConv = list.SubTypeGeneration.NeedsRecordConverter();

        var expectedLen = loqui == null ? default(int?) : await subTransl.ExpectedLength(loqui.TargetObjectGeneration, list.SubTypeGeneration);

        bool recordPerItem = false;
        if (listBinaryType == ListBinaryType.CounterRecord
            && subData.HasTrigger)
        {
            recordPerItem = true;
        }

        WrapSet(sb, itemAccessor, list, (wrapFg) =>
        {
            using (var args = wrapFg.Call(
                       $"{(isAsync ? "(" : null)}{Loqui.Generation.Utility.Await(isAsync)}{this.NamespacePrefix}List{(isAsync ? "Async" : null)}BinaryTranslation<{list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)}>.Instance.Parse{(recordPerItem ? "PerItem" : null)}",
                       suffixLine: $"{Loqui.Generation.Utility.ConfigAwait(isAsync)}{(isAsync ? ")" : null)}",
                       semiColon: false))
            {
                if (list is ArrayType arr
                    && arr.FixedSize.HasValue)
                {
                    args.Add($"reader: {Module.ReaderMemberName}");
                    args.Add($"amount: {arr.FixedSize.Value}");
                }
                else
                {
                    switch (listBinaryType)
                    {
                        case ListBinaryType.SubTrigger:
                            args.Add($"reader: {Module.ReaderMemberName}");
                            if (list.CustomData.TryGetValue(ItemStartMarker, out var itemStartMarkerObj) && itemStartMarkerObj is string itemStartMarker)
                            {
                                args.Add($"itemStartMarker: RecordTypes.{itemStartMarker}");
                            }
                            else
                            {
                                if (needsRecordConv)
                                {
                                    args.Add($"triggeringRecord: {subData.TriggeringRecordSetAccessor}");
                                }
                                else
                                {
                                    args.Add($"triggeringRecord: translationParams.ConvertToCustom({subData.TriggeringRecordSetAccessor})");
                                }
                            }
                            if (list.CustomData.TryGetValue(ItemEndMarker, out var itemEndMarkerObj) && itemEndMarkerObj is string itemEndMarker)
                            {
                                args.Add($"itemEndMarker: RecordTypes.{itemEndMarker}");
                            }
                            if (loqui != null
                                && !loqui.TargetObjectGeneration.Abstract
                                && loqui.TargetObjectGeneration.GetObjectData().TriggeringSource == null)
                            {
                                args.Add("skipHeader: true");
                            }
                            break;
                        case ListBinaryType.Trigger:
                            args.Add($"reader: {Module.ReaderMemberName}.SpawnWithLength(contentLength)");
                            break;
                        case ListBinaryType.CounterRecord:
                            args.Add($"reader: {Module.ReaderMemberName}");
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
                                    args.Add($"countRecord: translationParams.ConvertToCustom({objGen.RecordTypeHeaderName(counterRecType)})");
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
                                    args.Add($"triggeringRecord: translationParams.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
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
                                    args.Add($"triggeringRecord: translationParams.ConvertToCustom({subData.TriggeringRecordSetAccessor})");
                                }
                            }
                            if (list.CustomData.TryGetValue(NullIfCounterZero, out var nullIf)
                                && (bool)nullIf)
                            {
                                args.Add("nullIfZero: true");
                            }
                            break;
                        case ListBinaryType.PrependCount:
                            byte countLen = (byte)list.CustomData[CounterByteLength];
                            switch (countLen)
                            {
                                case 1:
                                    args.Add("amount: frame.ReadUInt8()");
                                    break;
                                case 2:
                                    args.Add("amount: frame.ReadUInt16()");
                                    break;
                                case 4:
                                    args.Add("amount: frame.ReadInt32()");
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                            if (list.CustomData.TryGetValue(ExpectedLengthLength, out var expLenLenObj)
                                && expLenLenObj is byte expLenLen)
                            {
                                if (loqui == null)
                                {
                                    throw new NotImplementedException();
                                }
                                args.Add($"expectedLengthLength: {expLenLen}");
                                args.Add($"expectedLength: {expectedLen ?? throw new NotImplementedException()}");
                            }
                            args.Add($"reader: {Module.ReaderMemberName}");
                            break;
                        case ListBinaryType.Frame:
                            args.Add($"reader: {Module.ReaderMemberName}");
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                if (list.CustomData.TryGetValue(EndMarker, out var endMarkerObj) && endMarkerObj is string endMarker)
                {
                    args.Add($"endMarker: RecordTypes.{endMarker}");
                }
                if (threading)
                {
                    args.Add($"thread: {Module.ReaderMemberName}.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingMeta.Parallel)}");
                }
                if (needsRecordConv)
                {
                    args.AddPassArg($"translationParams");
                }
                var subGenTypes = subData.GenerationTypes.ToList();
                var subGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
                if (subGenTypes.Count <= 1
                    && subTransl.AllowDirectParse(
                        objGen,
                        typeGen: list.SubTypeGeneration,
                        squashedRepeatedList: listBinaryType == ListBinaryType.Trigger))
                {
                    args.Add(subSb =>
                    {
                        subGen.GenerateCopyInRet(
                            sb: subSb,
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
                        if (subGenTypes.All(s => s.Value is LoquiType))
                        {
                            var subGenObjs = subGenTypes.Select(x => ((LoquiType)x.Value).TargetObjectGeneration).ToHashSet();
                            subGenTypes = subGenTypes.Where(s =>
                            {
                                return !((LoquiType)s.Value).TargetObjectGeneration.BaseClassTrail().Any(b =>
                                {
                                    if (!subGenObjs.Contains(b)) return false;
                                    var lookup = subGenTypes.FirstOrDefault(l => ((LoquiType)l.Value).TargetObjectGeneration == b);
                                    return s.Key.SequenceEqual(lookup.Key);
                                });
                            }).ToList();
                        }
                        if (subGenTypes.Count <= 1)
                        {
                            if (subGen.AllowDirectParse(
                                    objGen: objGen,
                                    typeGen: list.SubTypeGeneration,
                                    squashedRepeatedList: listBinaryType == ListBinaryType.Trigger))
                            {
                                subGen.GenerateCopyInRet(
                                    sb: gen,
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
                                gen.AppendLine($"transl: {Loqui.Generation.Utility.Async(isAsync)}(MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}{(isAsync ? null : $", [MaybeNullWhen(false)] out {list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)} listSubItem")}{(needsRecordConv ? $", {nameof(TypedParseParams)} translationParams" : null)}) =>");
                                using (gen.CurlyBrace())
                                {
                                    subGen.GenerateCopyInRet(
                                        sb: gen,
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
                            gen.AppendLine($"transl: {Loqui.Generation.Utility.Async(isAsync)}(MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}{(isAsync ? null : $", [MaybeNullWhen(false)] out {list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)} listSubItem")}{(needsRecordConv ? $", {nameof(TypedParseParams)} translationParams" : null)}) =>");
                            using (gen.CurlyBrace())
                            {
                                gen.AppendLine("switch (header.TypeInt)");
                                using (gen.CurlyBrace())
                                {
                                    foreach (var item in subGenTypes)
                                    {
                                        LoquiType specificLoqui = item.Value as LoquiType;
                                        if (specificLoqui.TargetObjectGeneration.Abstract) continue;
                                        foreach (var trigger in item.Key)
                                        {
                                            gen.AppendLine($"case RecordTypeInts.{trigger.Type}:");
                                        }
                                        LoquiType targetLoqui = list.SubTypeGeneration as LoquiType;
                                        using (gen.CurlyBrace())
                                        {
                                            subGen.GenerateCopyInRet(
                                                sb: gen,
                                                objGen: objGen,
                                                targetGen: list.SubTypeGeneration,
                                                typeGen: item.Value,
                                                readerAccessor: "r",
                                                translationAccessor: "listTranslMask",
                                                retAccessor: "return ",
                                                outItemAccessor: new Accessor("listSubItem"),
                                                asyncMode: AsyncMode.Async,
                                                errorMaskAccessor: $"listErrMask",
                                                converterAccessor: "translationParams",
                                                inline: false);
                                        }
                                    }
                                    gen.AppendLine("default:");
                                    using (gen.IncreaseDepth())
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

    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor structDataAccessor,
        Accessor recordDataAccessor,
        int? currentPosition,
        string passedLengthAccessor,
        DataType? dataType = null)
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
                    using (var args = sb.Call(
                               $"partial void {typeGen.Name}CustomParse"))
                    {
                        args.Add($"{nameof(OverlayStream)} stream");
                        args.Add($"int finalPos");
                        args.Add($"int offset");
                        args.Add($"{nameof(RecordType)} type");
                        args.Add($"{nameof(PreviousParse)} lastParsed");
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

        var accessor = dataType == null ? structDataAccessor : recordDataAccessor;

        if (list.SubTypeGeneration is LoquiType loqui)
        {
            var typeName = loqui.TypeName(getter: true, needsCovariance: true);
            switch (listBinaryType)
            {
                case ListBinaryType.PrependCount
                    when !data.HasTrigger:
                    if (expLen.HasValue)
                    {
                        string expectedLengthLengthAccessor = null;
                        if (list.CustomData.TryGetValue(ExpectedLengthLength, out var expectedLenLenObj) &&
                            expectedLenLenObj is byte expectedLenLen)
                        {
                            expectedLengthLengthAccessor = $", expectedLengthLength: {expectedLenLen}";
                        }

                        sb.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{typeGen.NullChar} {typeGen.Name} => BinaryOverlayList.FactoryByCountLength<{typeName}>({accessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, _package, {expLen}, countLength: {(byte)list.CustomData[CounterByteLength]}{expectedLengthLengthAccessor}, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                    }
                    else
                    {
                        if (dataType != null)
                        {
                            DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(sb, dataType, objGen, typeGen, passedLengthAccessor);
                        }
                        sb.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{typeGen.NullChar} {typeGen.Name} {{ get; private set; }} = null!;");
                    }
                    break;
                default:
                    if (data.HasTrigger)
                    {
                        if ((bool)(list.CustomData?.GetOrDefault(Additive) ?? false))
                        {
                            sb.AppendLine($"private ImmutableManyListWrapper<{list.ItemTypeName(getter: true)}>? _additive{typeGen.Name};");
                            sb.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{typeGen.NullChar} {typeGen.Name} => _additive{typeGen.Name}{(list.Nullable ? null : $" ?? Array.Empty<{list.ItemTypeName(getter: true)}>()")};");
                        }
                        else
                        {
                            sb.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{typeGen.NullChar} {typeGen.Name} {{ get; private set; }}{(typeGen.Nullable ? null : $" = Array.Empty<{typeName}>();")}");
                        }
                    }
                    else
                    {
                        sb.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{typeGen.NullChar} {typeGen.Name} => BinaryOverlayList.FactoryByLazyParse<{typeName}>({accessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, _package, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                    }
                    break;
            }
        }
        else if (data.HasTrigger)
        {
            sb.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{typeGen.NullChar} {typeGen.Name} {{ get; private set; }}{(typeGen.Nullable ? null : $" = Array.Empty<{list.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}>();")}");
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
                        sb.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{typeGen.NullChar} {typeGen.Name} => BinaryOverlayList.FactoryByCountLength<{typeName}>({accessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, _package, {expLen}, countLength: {(byte)list.CustomData[CounterByteLength]}, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                    }
                    else if (list.SubTypeGeneration is StringType str
                             && (str.BinaryType == StringBinaryType.PrependLength
                                 || str.BinaryType == StringBinaryType.PrependLengthUShort))
                    {
                        sb.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{typeGen.NullChar} {typeGen.Name} => BinaryOverlayList.FactoryByCountLengthWithItemLength<{typeName}>({accessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, _package, countLength: {(byte)list.CustomData[CounterByteLength]}, itemLengthLength: {(str.BinaryType == StringBinaryType.PrependLength ? 4 : 2)}, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                    }
                    break;
                default:
                    sb.AppendLine($"public {list.ListTypeName(getter: true, internalInterface: true)}{typeGen.NullChar} {typeGen.Name} => BinaryOverlayList.FactoryByStartIndex<{list.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}>({accessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}, _package, {expLen}, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                    break;
            }
        }
    }

    public override async Task GenerateWrapperRecordTypeParse(
        StructuredStringBuilder sb,
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
                using (var args = sb.Call(
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
        
        var additive = (bool)list.CustomData[Additive];
        string target;
        if (additive)
        {
            sb.AppendLine($"_additive{typeGen.Name} ??= new();");
            target = $"var {typeGen.Name}Tmp = ";
        }
        else
        {
            target = $"this.{typeGen.Name} = ";
        }

        await GenerateWrapperRecordTypeParseGeneral(
            sb, objGen, typeGen,
            "stream",
            locationAccessor, 
            packageAccessor, 
            target,
            converterAccessor);
        
        if (additive)
        {
            sb.AppendLine($"_additive{typeGen.Name}.AddList({typeGen.Name}Tmp);");
        }
    }

    public async Task GenerateWrapperRecordTypeParseGeneral(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor streamAccessor,
        Accessor locationAccessor,
        Accessor packageAccessor,
        Accessor retAccessor,
        Accessor converterAccessor)
    {
        ListType list = typeGen as ListType;
        var data = list.GetFieldData();
        if (data.MarkerType.HasValue)
        {
            sb.AppendLine($"{streamAccessor}.Position += {packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.Constants)}.SubConstants.HeaderLength; // Skip marker");
        }
        var subData = list.SubTypeGeneration.GetFieldData();
        var subGenTypes = subData.GenerationTypes.ToList();
        ListBinaryType listBinaryType = GetListType(list, data, subData);
        var subGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
        string typeName = list.SubTypeGeneration.TypeName(getter: true, needsCovariance: true);
        LoquiType loqui = list.SubTypeGeneration as LoquiType;
        var expectedLen = await subGen.ExpectedLength(objGen, list.SubTypeGeneration);
        switch (listBinaryType)
        {
            case ListBinaryType.SubTrigger:
                if (loqui != null)
                {
                    if (loqui.TargetObjectGeneration.IsTypelessStruct())
                    {
                        using (var args = sb.Call(
                                   $"{retAccessor}this.{nameof(PluginBinaryOverlay.ParseRepeatedTypelessSubrecord)}<{typeName}>"))
                        {
                            args.Add($"stream: {streamAccessor}");
                            args.Add($"translationParams: {converterAccessor}");
                            if (list.CustomData.TryGetValue(ItemStartMarker, out var itemStartMarkerObj) && itemStartMarkerObj is string itemStartMarker)
                            {
                                args.Add($"itemStartMarker: RecordTypes.{itemStartMarker}");
                            }
                            else
                            {
                                args.Add($"trigger: {subData.TriggeringRecordSetAccessor}");
                            }
                            if (list.CustomData.TryGetValue(ItemEndMarker, out var itemEndMarkerObj) && itemEndMarkerObj is string itemEndMarker)
                            {
                                args.Add($"itemEndMarker: RecordTypes.{itemEndMarker}");
                            }
                            if (subGenTypes.Count <= 1)
                            {
                                args.Add($"factory: {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory");
                            }
                            else
                            {
                                args.Add((subFg) =>
                                {
                                    subFg.AppendLine("factory: (s, r, p, recConv) =>");
                                    using (subFg.CurlyBrace())
                                    {
                                        subFg.AppendLine("switch (r.TypeInt)");
                                        using (subFg.CurlyBrace())
                                        {
                                            foreach (var item in subGenTypes)
                                            {
                                                foreach (var trigger in item.Key)
                                                {
                                                    subFg.AppendLine($"case RecordTypeInts.{trigger.Type}:");
                                                }
                                                using (subFg.IncreaseDepth())
                                                {
                                                    LoquiType specificLoqui = item.Value as LoquiType;
                                                    subFg.AppendLine($"return {this.Module.BinaryOverlayClassName(specificLoqui.TargetObjectGeneration)}.{specificLoqui.TargetObjectGeneration.Name}Factory(s, p);");
                                                }
                                            }
                                            subFg.AppendLine("default:");
                                            using (subFg.IncreaseDepth())
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
                        using (var args = sb.Call(
                                   $"{retAccessor}BinaryOverlayList.FactoryByArray<{typeName}>"))
                        {
                            args.Add($"mem: {streamAccessor}.RemainingMemory");
                            args.Add($"package: _package");
                            args.Add($"translationParams: {converterAccessor}");
                            args.Add($"getter: (s, p, recConv) => {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(OverlayStream)}(s, p), p, recConv)");
                            args.Add(subFg =>
                            {
                                using (var subArgs = subFg.Function(
                                           $"locs: {nameof(PluginBinaryOverlay.ParseRecordLocations)}"))
                                {
                                    subArgs.Add($"stream: {streamAccessor}");
                                    subArgs.Add($"trigger: {subData.TriggeringRecordSetAccessor}");
                                    subArgs.Add($"triggersAlwaysAreNewRecords: true");
                                    switch (loqui.TargetObjectGeneration.GetObjectType())
                                    {
                                        case ObjectType.Subrecord:
                                            subArgs.Add($"constants: _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.Constants)}.{nameof(GameConstants.SubConstants)}");
                                            break;
                                        case ObjectType.Record:
                                            subArgs.Add($"constants: _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.Constants)}.{nameof(GameConstants.MajorConstants)}");
                                            break;
                                        case ObjectType.Group:
                                            subArgs.Add($"constants: _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.Constants)}.{nameof(GameConstants.GroupConstants)}");
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
                    using (var args = sb.Call(
                               $"{retAccessor}BinaryOverlayList.FactoryByArray<{typeName}>"))
                    {
                        args.Add($"mem: {streamAccessor}.RemainingMemory");
                        args.Add($"package: _package");
                        args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")}");
                        args.Add(subFg =>
                        {
                            using (var subArgs = subFg.Function(
                                       $"locs: {nameof(PluginBinaryOverlay.ParseRecordLocations)}"))
                            {
                                subArgs.Add($"stream: {streamAccessor}");
                                subArgs.Add($"constants: _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.Constants)}.{nameof(GameConstants.SubConstants)}");
                                subArgs.Add($"trigger: {list.SubTypeGeneration.GetFieldData().TriggeringRecordAccessor}");
                                subArgs.Add("skipHeader: true");
                                subArgs.Add($"translationParams: {converterAccessor}");
                            }
                        });
                    }
                }
                else
                {
                    using (var args = sb.Call(
                               $"{retAccessor}BinaryOverlayList.FactoryByArray<{typeName}>"))
                    {
                        args.Add($"mem: {streamAccessor}.RemainingMemory");
                        args.Add($"package: _package");
                        args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, $"p.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.Constants)}.Subrecord(s).Content", "p")}");
                        args.Add(subFg =>
                        {
                            using (var subArgs = subFg.Function(
                                       $"locs: {nameof(PluginBinaryOverlay.ParseRecordLocations)}"))
                            {
                                subArgs.Add($"stream: {streamAccessor}");
                                subArgs.Add($"constants: _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.Constants)}.{nameof(GameConstants.SubConstants)}");
                                subArgs.Add($"trigger: {list.SubTypeGeneration.GetFieldData().TriggeringRecordAccessor}");
                                subArgs.Add("skipHeader: false");
                                subArgs.Add($"translationParams: {converterAccessor}");
                            }
                        });
                    }
                }
                break;
            case ListBinaryType.Trigger:
                if (expectedLen.HasValue)
                {
                    using (var args = sb.Call(
                               $"{retAccessor}BinaryOverlayList.FactoryByStartIndexWithTrigger<{typeName}>"))
                    {
                        args.Add($"stream: {streamAccessor}");
                        args.Add($"package: _package");
                        args.Add("finalPos: finalPos");
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
                                using (subFg.CurlyBrace())
                                {
                                    subFg.AppendLine("switch (r.TypeInt)");
                                    using (subFg.CurlyBrace())
                                    {
                                        foreach (var item in subGenTypes)
                                        {
                                            foreach (var trigger in item.Key)
                                            {
                                                subFg.AppendLine($"case RecordTypeInts.{trigger.Type}:");
                                            }
                                            using (subFg.IncreaseDepth())
                                            {
                                                LoquiType specificLoqui = item.Value as LoquiType;
                                                subFg.AppendLine($"return {subGen.GenerateForTypicalWrapper(objGen, specificLoqui, "s", "p")}");
                                            }
                                        }
                                        subFg.AppendLine("default:");
                                        using (subFg.IncreaseDepth())
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
                    using (var args = sb.Call(
                               $"{retAccessor}BinaryOverlayList.FactoryByLazyParseWithTrigger<{typeName}>"))
                    {
                        args.Add($"stream: {streamAccessor}");
                        args.Add($"package: _package");
                        args.Add("finalPos: finalPos");
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
                break;
            case ListBinaryType.CounterRecord:
                var counterLen = (byte)list.CustomData[CounterByteLength];
                if (expectedLen.HasValue)
                {
                    var nullIfEmpty = list.CustomData.TryGetValue(NullIfCounterZero, out var nullIf) && (bool)nullIf;
                    using (var args = sb.Call(
                               $"{retAccessor}BinaryOverlayList.FactoryByCount{(subData.HasTrigger ? "PerItem" : null)}{(nullIfEmpty ? "NullIfZero" : null)}<{typeName}>"))
                    {
                        args.Add($"stream: {streamAccessor}");
                        args.Add($"package: _package");
                        args.Add($"itemLength: 0x{expectedLen:X}");
                        args.Add($"countLength: {counterLen}");
                        args.Add($"countType: {objGen.RecordTypeHeaderName(new RecordType((string)typeGen.CustomData[CounterRecordType]))}");
                        if (subData.HasTrigger)
                        {
                            args.Add($"trigger: {subData.TriggeringRecordAccessor ?? subData.TriggeringRecordSetAccessor}");
                        }
                        else
                        {
                            args.Add($"trigger: {objGen.RecordTypeHeaderName(data.RecordType.Value)}");
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
                                using (subFg.CurlyBrace())
                                {
                                    subFg.AppendLine("switch (r.TypeInt)");
                                    using (subFg.CurlyBrace())
                                    {
                                        foreach (var item in subGenTypes)
                                        {
                                            foreach (var trigger in item.Key)
                                            {
                                                subFg.AppendLine($"case RecordTypeInts.{trigger.Type}:");
                                            }
                                            using (subFg.IncreaseDepth())
                                            {
                                                LoquiType specificLoqui = item.Value as LoquiType;
                                                subFg.AppendLine($"return {subGen.GenerateForTypicalWrapper(objGen, specificLoqui, "s", "p")};");
                                            }
                                        }
                                        subFg.AppendLine("default:");
                                        using (subFg.IncreaseDepth())
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
                    using (var args = sb.Call(
                               $"{retAccessor}BinaryOverlayList.FactoryByCountPerItem<{typeName}>"))
                    {
                        args.Add($"stream: {streamAccessor}");
                        args.Add($"package: _package");
                        args.Add($"countLength: {counterLen}");
                        args.Add($"trigger: {subData.TriggeringRecordSetAccessor}");
                        args.Add($"countType: {objGen.RecordTypeHeaderName(new RecordType((string)typeGen.CustomData[CounterRecordType]))}");
                        if (loqui != null)
                        {
                            args.Add($"translationParams: {converterAccessor}");
                            args.Add($"getter: (s, p, recConv) => {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(OverlayStream)}(s, p), p, recConv)");
                            args.Add("skipHeader: false");
                        }
                        else
                        {
                            args.Add($"getter: (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")}");
                            args.Add("skipHeader: true");
                        }
                        if (list.CustomData.TryGetValue(EndMarker, out var endMarkerObj) && endMarkerObj is string endMarker)
                        {
                            args.Add($"endMarker: RecordTypes.{endMarker}");
                        }
                    }
                }
                break;
            case ListBinaryType.PrependCount:
                if (data.HasTrigger)
                {
                    sb.AppendLine($"{streamAccessor}.Position += _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingMeta.Constants)}.SubConstants.HeaderLength;");
                }
                byte countLen = (byte)list.CustomData[CounterByteLength];
                switch (countLen)
                {
                    case 1:
                        sb.AppendLine($"var count = {streamAccessor}.ReadUInt8();");
                        break;
                    case 2:
                        sb.AppendLine($"var count = {streamAccessor}.ReadUInt16();");
                        break;
                    case 4:
                        sb.AppendLine($"var count = {streamAccessor}.ReadUInt32();");
                        break;
                    default:
                        throw new NotImplementedException();
                }
                using (var args = sb.Call(
                           $"{retAccessor}BinaryOverlayList.FactoryByCount{(expectedLen == null ? null : "Length")}<{typeName}>"))
                {
                    args.Add($"stream: {streamAccessor}");
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
                            using (subFg.CurlyBrace())
                            {
                                subFg.AppendLine("switch (r.TypeInt)");
                                using (subFg.CurlyBrace())
                                {
                                    foreach (var item in subGenTypes)
                                    {
                                        foreach (var trigger in item.Key)
                                        {
                                            subFg.AppendLine($"case RecordTypeInts.{trigger.Type}:");
                                        }
                                        using (subFg.IncreaseDepth())
                                        {
                                            LoquiType specificLoqui = item.Value as LoquiType;
                                            subFg.AppendLine($"return {subGen.GenerateForTypicalWrapper(objGen, specificLoqui, "s", "p")};");
                                        }
                                    }
                                    subFg.AppendLine("default:");
                                    using (subFg.IncreaseDepth())
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
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor dataAccessor, 
        int? passedLength,
        string? passedLengthAccessor,
        DataType? data = null)
    {
        ListType list = typeGen as ListType;
        var fieldData = list.GetFieldData();
        var subTypeFieldData = list.SubTypeGeneration.GetFieldData();
        ListBinaryType listBinaryType = GetListType(list, fieldData, subTypeFieldData);
        var subGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
        var subExpLen = await subGen.ExpectedLength(objGen, list.SubTypeGeneration);
        switch (listBinaryType)
        {
            case ListBinaryType.PrependCount:
            {
                var len = (byte)list.CustomData[CounterByteLength];
                var expectedLengthLength = list.CustomData.TryGetValue(ExpectedLengthLength, out var expLenLenObj)
                                           && expLenLenObj is byte expLenLen ? expLenLen : 0;
                if (subExpLen.HasValue)
                {
                    var accessorData = $"ret.{dataAccessor}";
                    string readStr;
                    switch (len)
                    {
                        case 0:
                            return;
                        case 1:
                            readStr = $"{accessorData}[{passedLengthAccessor ?? "0"}]";
                            break;
                        case 2:
                            readStr = $"BinaryPrimitives.ReadUInt16LittleEndian(ret.{dataAccessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")})";
                            break;
                        case 4:
                            readStr = $"BinaryPrimitives.ReadInt32LittleEndian(ret.{dataAccessor}{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")})";
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    sb.AppendLine($"ret.{typeGen.Name}EndingPos = {(passedLengthAccessor == null ? null : $"{passedLengthAccessor} + ")}{readStr} * {subExpLen.Value} + {len}{(expectedLengthLength == 0 ? null : $" + {expectedLengthLength}")};");
                }
                else if (list.SubTypeGeneration is StringType strType && strType.BinaryType == StringBinaryType.PrependLengthUShort)
                {
                    var accessorData = $"ret.{dataAccessor}";
                    if (passedLengthAccessor != null)
                    {
                        accessorData += $".Slice({passedLengthAccessor})";
                    }
                    sb.AppendLine($"ret.{typeGen.Name}EndingPos = {(passedLengthAccessor == null ? null : $"{passedLengthAccessor} + ")}StringBinaryTranslation.Instance.ExtractManyUInt16PrependedStringsLength({len}, {accessorData}) + {len};");
                }
                else
                {
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine($"var tempStream = new OverlayStream(ret.{dataAccessor}, package)");
                        using (sb.CurlyBrace(appendSemiColon: true))
                        {
                            if (data != null)
                            {
                                sb.AppendLine($"Position = ret._{typeGen.Name}Location");
                            }
                            else if (passedLengthAccessor != null)
                            {
                                sb.AppendLine($"Position = {passedLengthAccessor}");
                            }
                        }
                        var counterLength = (byte)list.CustomData[CounterByteLength];
                        sb.AppendLine($"ret.{typeGen.Name} = BinaryOverlayList.EagerFactoryByPrependedCount(tempStream, package, {counterLength}, (s, p) => {subGen.GenerateForTypicalWrapper(objGen, list.SubTypeGeneration, "s", "p")});");
                        sb.AppendLine($"ret.{typeGen.Name}EndingPos = tempStream.Position;");
                    }
                }
            }
                break;
            case ListBinaryType.Frame:
                if (!subTypeFieldData.HasTrigger)
                {
                    sb.AppendLine($"ret.{typeGen.Name}EndingPos = ret.{dataAccessor}.Length;");
                }
                break;
            case ListBinaryType.SubTrigger:
            case ListBinaryType.Trigger:
            case ListBinaryType.CounterRecord:
            default:
                break;
        }
    }

    public void WrapSet(StructuredStringBuilder sb, Accessor accessor, ListType list, Action<StructuredStringBuilder> a)
    {
        var additive = (bool)list.CustomData[Additive];
        if (list.Nullable)
        {
            if (additive)
            {
                using (var f = sb.Call($"({accessor} = ({accessor} ?? new())).AddRange"))
                {
                    f.Add(subSb => a(subSb));
                }
            }
            else
            {
                sb.AppendLine($"{accessor} = ");
                using (sb.IncreaseDepth())
                {
                    a(sb);
                    if (list.CustomData.TryGetValue(NullIfCounterZero, out var val)
                        && (bool)val)
                    {

                        sb.AppendLine($".CastExtendedListIfAny<{list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)}>();");
                    }
                    else
                    {
                        sb.AppendLine($".CastExtendedList<{list.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)}>();");
                    }
                }
            }
        }
        else
        {
            using (var args = sb.Call(
                       $"{accessor}.{(additive ? "AddRange" : "SetTo")}"))
            {
                args.Add(subFg => a(subFg));
            }
        }
    }
    
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

    protected virtual string GetWriteAccessor(Accessor itemAccessor)
    {
        return itemAccessor.Access;
    }

    public override async Task GenerateCopyInRet(
        StructuredStringBuilder sb,
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
        var list = typeGen as ListType;
        string typeName = list.SubTypeGeneration.TypeName(getter: true, needsCovariance: true);
        sb.AppendLine($"{outItemAccessor} = new ExtendedList<{typeName}>();");
        await GenerateCopyIn(sb, objGen, typeGen,
            nodeAccessor: nodeAccessor, 
            itemAccessor: outItemAccessor,
            errorMaskAccessor: errorMaskAccessor,
            translationMaskAccessor: translationAccessor);
        sb.AppendLine("return true;");
    }

    public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        return false;
    }

    public override bool AllowDirectParse(ObjectGeneration objGen, TypeGeneration typeGen, bool squashedRepeatedList)
    {
        return false;
    }
}
