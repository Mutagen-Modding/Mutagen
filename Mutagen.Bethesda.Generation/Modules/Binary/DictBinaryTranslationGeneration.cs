using Noggog;
using Loqui.Generation;
using System.Xml.Linq;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;
using EnumType = Mutagen.Bethesda.Generation.Fields.EnumType;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class DictBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public virtual string TranslatorName => "DictBinaryTranslation";
        
    const string ThreadKey = "DictThread";

    public enum DictBinaryType
    {
        SubTrigger,
        Trigger,
        EnumMap,
    }

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        var dictType = typeGen as DictType;
        var keyMask = this.MaskModule.GetMaskModule(dictType.KeyTypeGen.GetType()).GetErrorMaskTypeStr(dictType.KeyTypeGen);
        var valMask = this.MaskModule.GetMaskModule(dictType.ValueTypeGen.GetType()).GetErrorMaskTypeStr(dictType.ValueTypeGen);
        return $"{TranslatorName}<{dictType.KeyTypeGen.TypeName(getter: getter)}, {dictType.ValueTypeGen.TypeName(getter: getter)}, {keyMask}, {valMask}>.Instance";
    }

    public override bool IsAsync(TypeGeneration gen, bool read)
    {
        var dictType = gen as DictType;
        if (dictType.CustomData.TryGetValue(ThreadKey, out var val) && ((bool)val)) return true;
        if (this.Module.TryGetTypeGeneration(dictType.KeyTypeGen.GetType(), out var keyGen)
            && keyGen.IsAsync(dictType.KeyTypeGen, read)) return true;
        if (this.Module.TryGetTypeGeneration(dictType.ValueTypeGen.GetType(), out var valGen)
            && valGen.IsAsync(dictType.ValueTypeGen, read)) return true;
        return false;
    }

    public override void Load(ObjectGeneration obj, TypeGeneration field, XElement node)
    {
        var asyncItem = node.GetAttribute<bool>("asyncItems", false);
        var thread = node.GetAttribute<bool>("thread", false);
        var dictType = field as DictType;
        dictType.CustomData[ThreadKey] = thread;
        if (asyncItem && dictType.ValueTypeGen is LoquiType loqui)
        {
            loqui.CustomData[LoquiBinaryTranslationGeneration.AsyncOverrideKey] = asyncItem;
        }
    }

    private DictBinaryType GetDictType(DictType dict)
    {
        var data = dict.GetFieldData();
        var subData = dict.ValueTypeGen.GetFieldData();
        if (subData.HasTrigger)
        {
            return DictBinaryType.SubTrigger;
        }
        else if (data.RecordType.HasValue)
        {
            return DictBinaryType.Trigger;
        }
        else if (dict.Mode == DictMode.KeyValue
                 && dict.KeyTypeGen is EnumType)
        {
            return DictBinaryType.EnumMap;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public override async Task GenerateWrite(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor writerAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMask,
        Accessor converterAccessor)
    {
        var dict = typeGen as DictType;
        var subData = dict.ValueTypeGen.GetFieldData();

        var data = typeGen.GetFieldData();
        if (data.MarkerType.HasValue)
        {
            sb.AppendLine($"using (HeaderExport.ExportHeader(writer, {objGen.RegistrationName}.{data.MarkerType.Value.Type}_HEADER, ObjectType.Subrecord)) {{ }}");
        }
        var binaryType = GetDictType(dict);
        if (!this.Module.TryGetTypeGeneration(dict.ValueTypeGen.GetType(), out var valTransl))
        {
            throw new ArgumentException("Unsupported type generator: " + dict.ValueTypeGen);
        }

        if (dict.Mode == DictMode.KeyedValue
            || binaryType == DictBinaryType.EnumMap)
        {
            var term = binaryType == DictBinaryType.EnumMap ? "Dict" : "List";
            using (var args = sb.Call(
                       $"{this.NamespacePrefix}{term}BinaryTranslation<{dict.ValueTypeGen.TypeName(getter: true)}>.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"items: {itemAccessor}{(binaryType == DictBinaryType.EnumMap ? null : ".Items")}");
                if (binaryType == DictBinaryType.Trigger)
                {
                    args.Add($"recordType: {objGen.RecordTypeHeaderName(data.RecordType.Value)}");
                }
                if (valTransl.AllowDirectWrite(objGen, typeGen))
                {
                    args.Add($"transl: {valTransl.GetTranslatorInstance(dict.ValueTypeGen, getter: true)}.Write");
                }
                else
                {
                    await args.Add(async (gen) =>
                    {
                        gen.AppendLine($"transl: (MutagenWriter r, {dict.ValueTypeGen.TypeName(getter: true)} dictSubItem) =>");
                        using (gen.CurlyBrace())
                        {
                            LoquiType targetLoqui = dict.ValueTypeGen as LoquiType;
                            await valTransl.GenerateWrite(
                                sb: gen,
                                objGen: objGen,
                                typeGen: targetLoqui,
                                itemAccessor: new Accessor("dictSubItem"),
                                writerAccessor: "r",
                                translationAccessor: "dictTranslMask",
                                errorMaskAccessor: null,
                                converterAccessor: null);
                        }
                    });
                }
            }
        }
        else
        {
            throw new NotImplementedException();
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
        var dict = typeGen as DictType;
        var data = dict.GetFieldData();
        var subData = dict.ValueTypeGen.GetFieldData();
        if (!this.Module.TryGetTypeGeneration(dict.ValueTypeGen.GetType(), out var subTransl))
        {
            throw new ArgumentException("Unsupported type generator: " + dict.ValueTypeGen);
        }
        var isAsync = subTransl.IsAsync(dict.ValueTypeGen, read: true);

        var binaryType = GetDictType(dict);

        if (data.MarkerType.HasValue)
        {
            sb.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH + long; // Skip marker");
        }
        else if (binaryType == DictBinaryType.Trigger)
        {
            sb.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH;");
        }

        var term = binaryType == DictBinaryType.EnumMap ? "Dict" : "List";

        using (var args = sb.Call(
                   $"{Loqui.Generation.Utility.Await(isAsync)}{this.NamespacePrefix}{term}{(isAsync ? "Async" : null)}BinaryTranslation<{dict.ValueTypeGen.TypeName(getter: false)}>.Instance.Parse{(binaryType == DictBinaryType.EnumMap ? $"<{dict.KeyTypeGen.TypeName(false)}>" : null)}",
                   suffixLine: Loqui.Generation.Utility.ConfigAwait(isAsync)))
        {
            switch (binaryType)
            {
                case DictBinaryType.SubTrigger:
                    args.Add($"reader: {Module.ReaderMemberName}");
                    args.Add($"triggeringRecord: {subData.TriggeringRecordSetAccessor}");
                    break;
                case DictBinaryType.Trigger:
                    args.Add($"reader: {Module.ReaderMemberName}.Spawn(long)");
                    break;
                case DictBinaryType.EnumMap:
                    args.Add($"reader: {Module.ReaderMemberName}");
                    break;
                default:
                    throw new NotImplementedException();
            }
            args.Add($"item: {itemAccessor}");
            var subGenTypes = subData.GenerationTypes.ToList();
            var subGen = this.Module.GetTypeGeneration(dict.ValueTypeGen.GetType());
            if (subGenTypes.Count <= 1
                && subTransl.AllowDirectParse(objGen, typeGen, squashedRepeatedList: false))
            {
                args.Add($"transl: {subTransl.GetTranslatorInstance(dict.ValueTypeGen, getter: false)}.Parse");
            }
            else if (subGenTypes.Count > 1)
            {
                args.Add((gen) =>
                {
                    gen.AppendLine($"transl: (MutagenFrame r, RecordType header{(isAsync ? null : $", out {dict.ValueTypeGen.TypeName(getter: false)} dictSubItem")}) =>");
                    using (gen.CurlyBrace())
                    {
                        gen.AppendLine("switch (header.Type)");
                        using (gen.CurlyBrace())
                        {
                            foreach (var item in subGenTypes)
                            {
                                foreach (var trigger in item.Key)
                                {
                                    gen.AppendLine($"case \"{trigger.Type}\":");
                                }
                                LoquiType targetLoqui = dict.ValueTypeGen as LoquiType;
                                LoquiType specificLoqui = item.Value as LoquiType;
                                using (gen.IncreaseDepth())
                                {
                                    subGen.GenerateCopyInRet(
                                        sb: gen,
                                        objGen: objGen,
                                        targetGen: dict.ValueTypeGen,
                                        typeGen: item.Value,
                                        readerAccessor: "r",
                                        retAccessor: "return ",
                                        outItemAccessor: new Accessor("dictSubItem"),
                                        translationAccessor: "dictTranslMask",
                                        asyncMode: AsyncMode.Off,
                                        errorMaskAccessor: null,
                                        converterAccessor: null,
                                        inline: true);
                                }
                            }
                            gen.AppendLine("default:");
                            using (gen.IncreaseDepth())
                            {
                                gen.AppendLine("throw new NotImplementedException();");
                            }
                        }
                    }
                });
            }
            else
            {
                args.Add((gen) =>
                {
                    LoquiType targetLoqui = dict.ValueTypeGen as LoquiType;
                    subGen.GenerateCopyInRet(
                        sb: gen,
                        objGen: objGen,
                        targetGen: dict.ValueTypeGen,
                        typeGen: targetLoqui,
                        readerAccessor: "r",
                        retAccessor: "transl: ",
                        outItemAccessor: new Accessor("dictSubItem"),
                        translationAccessor: "dictTranslMask",
                        asyncMode: AsyncMode.Off,
                        errorMaskAccessor: null,
                        converterAccessor: null,
                        inline: true);
                });
            }
        }
    }

    public override void GenerateCopyInRet(
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
        throw new NotImplementedException();
    }

    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb,
        ObjectGeneration objGen, 
        TypeGeneration typeGen,
        Accessor dataAccessor,
        int? passedLength,
        string passedLengthAccessor,
        DataType data = null)
    {
        DictType dict = typeGen as DictType;
        if (GetDictType(dict) != DictBinaryType.EnumMap)
        {
            // Special case for Groups
            return;
        }

        if (!this.Module.TryGetTypeGeneration(dict.ValueTypeGen.GetType(), out var subTransl))
        {
            throw new ArgumentException("Unsupported type generator: " + dict.ValueTypeGen);
        }

        var posStr = data == null ? passedLengthAccessor : $"_{typeGen.Name}Location";
        if (data != null)
        {
            DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(sb, data, objGen, typeGen, passedLengthAccessor);
        }

        using (var args = sb.Call(
                   $"public IReadOnlyDictionary<{dict.KeyTypeGen.TypeName(getter: true)}, {dict.ValueTypeGen.TypeName(getter: true)}> {typeGen.Name} => DictBinaryTranslation<{dict.ValueTypeGen.TypeName(getter: false)}>.Instance.Parse<{dict.KeyTypeGen.TypeName(false)}>"))
        {
            args.Add($"new {nameof(MutagenFrame)}(new {nameof(MutagenMemoryReadStream)}({dataAccessor}{(posStr == null ? null : $".Slice({posStr})")}, _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}))");
            args.Add($"new Dictionary<{dict.KeyTypeGen.TypeName(getter: true)}, {dict.ValueTypeGen.TypeName(getter: true)}>()");
            args.Add($"{subTransl.GetTranslatorInstance(dict.ValueTypeGen, getter: true)}.Parse");
        }
    }

    public override async Task<int?> GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen) => await ExpectedLength(objGen, typeGen);

    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        DictType dict = typeGen as DictType;
        if (GetDictType(dict) != DictBinaryType.EnumMap) return null;
        if (!this.Module.TryGetTypeGeneration(dict.ValueTypeGen.GetType(), out var subTransl))
        {
            throw new ArgumentException("Unsupported type generator: " + dict.ValueTypeGen);
        }
        var perLength = await subTransl.ExpectedLength(objGen, dict.ValueTypeGen);
        return dict.NumEnumKeys.Value * perLength;
    }
}