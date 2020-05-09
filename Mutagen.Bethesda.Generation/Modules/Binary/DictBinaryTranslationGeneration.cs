using Loqui;
using Noggog;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.WebSockets;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Generation
{
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

        public override void GenerateWrite(
            FileGeneration fg,
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

            if (typeGen.TryGetFieldData(out var data)
                && data.MarkerType.HasValue)
            {
                fg.AppendLine($"using (HeaderExport.ExportHeader(writer, {objGen.RegistrationName}.{data.MarkerType.Value.Type}_HEADER, ObjectType.Subrecord)) {{ }}");
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
                using (var args = new ArgsWrapper(fg,
                    $"{this.Namespace}{term}BinaryTranslation<{dict.ValueTypeGen.TypeName(getter: true)}>.Instance.Write"))
                {
                    args.Add($"writer: {writerAccessor}");
                    args.Add($"items: {itemAccessor.PropertyOrDirectAccess}{(binaryType == DictBinaryType.EnumMap ? null : ".Items")}");
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
                        args.Add((gen) =>
                        {
                            gen.AppendLine($"transl: (MutagenWriter r, {dict.ValueTypeGen.TypeName(getter: true)} dictSubItem) =>");
                            using (new BraceWrapper(gen))
                            {
                                LoquiType targetLoqui = dict.ValueTypeGen as LoquiType;
                                valTransl.GenerateWrite(
                                    fg: gen,
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
            FileGeneration fg,
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
                fg.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH + long; // Skip marker");
            }
            else if (binaryType == DictBinaryType.Trigger)
            {
                fg.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH;");
            }

            var term = binaryType == DictBinaryType.EnumMap ? "Dict" : "List";

            using (var args = new ArgsWrapper(fg,
                $"{Loqui.Generation.Utility.Await(isAsync)}{this.Namespace}{term}{(isAsync ? "Async" : null)}BinaryTranslation<{dict.ValueTypeGen.TypeName(getter: false)}>.Instance.Parse{(binaryType == DictBinaryType.EnumMap ? $"<{dict.KeyTypeGen.TypeName(false)}>" : null)}",
                suffixLine: Loqui.Generation.Utility.ConfigAwait(isAsync)))
            {
                switch (binaryType)
                {
                    case DictBinaryType.SubTrigger:
                        args.AddPassArg($"frame");
                        args.Add($"triggeringRecord: {subData.TriggeringRecordSetAccessor}");
                        break;
                    case DictBinaryType.Trigger:
                        args.Add($"frame: frame.Spawn(long)");
                        break;
                    case DictBinaryType.EnumMap:
                        args.AddPassArg($"frame");
                        break;
                    default:
                        throw new NotImplementedException();
                }
                args.Add($"item: {itemAccessor.PropertyAccess}");
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
                        using (new BraceWrapper(gen))
                        {
                            gen.AppendLine("switch (header.Type)");
                            using (new BraceWrapper(gen))
                            {
                                foreach (var item in subGenTypes)
                                {
                                    foreach (var trigger in item.Key)
                                    {
                                        gen.AppendLine($"case \"{trigger.Type}\":");
                                    }
                                    LoquiType targetLoqui = dict.ValueTypeGen as LoquiType;
                                    LoquiType specificLoqui = item.Value as LoquiType;
                                    using (new DepthWrapper(gen))
                                    {
                                        subGen.GenerateCopyInRet(
                                            fg: gen,
                                            objGen: objGen,
                                            targetGen: dict.ValueTypeGen,
                                            typeGen: item.Value,
                                            readerAccessor: "r",
                                            retAccessor: "return ",
                                            outItemAccessor: new Accessor("dictSubItem"),
                                            translationAccessor: "dictTranslMask",
                                            asyncMode: AsyncMode.Direct,
                                            errorMaskAccessor: null,
                                            converterAccessor: null);
                                    }
                                }
                                gen.AppendLine("default:");
                                using (new DepthWrapper(gen))
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
                        gen.AppendLine($"transl: (MutagenFrame r{(isAsync ? null : $", out {dict.ValueTypeGen.TypeName(getter: false)} dictSubItem")}) =>");
                        using (new BraceWrapper(gen))
                        {
                            LoquiType targetLoqui = dict.ValueTypeGen as LoquiType;
                            subGen.GenerateCopyInRet(
                                fg: gen,
                                objGen: objGen,
                                targetGen: dict.ValueTypeGen,
                                typeGen: targetLoqui,
                                readerAccessor: "r",
                                retAccessor: "return ",
                                outItemAccessor: new Accessor("dictSubItem"),
                                translationAccessor: "dictTranslMask",
                                asyncMode: AsyncMode.Direct,
                                errorMaskAccessor: null,
                                converterAccessor: null);
                        }
                    });
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
            Accessor converterAccessor)
        {
            throw new NotImplementedException();
        }

        public override async Task GenerateWrapperFields(
            FileGeneration fg,
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

            using (var args = new ArgsWrapper(fg,
                $"public IReadOnlyDictionary<{dict.KeyTypeGen.TypeName(getter: true)}, {dict.ValueTypeGen.TypeName(getter: true)}> {typeGen.Name} => DictBinaryTranslation<{dict.ValueTypeGen.TypeName(getter: false)}>.Instance.Parse<{dict.KeyTypeGen.TypeName(false)}>"))
            {
                args.Add($"new {nameof(MutagenFrame)}(new {nameof(MutagenMemoryReadStream)}({dataAccessor}.Slice({passedLengthAccessor}), _package.Meta, _package.MasterReferences))");
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
}
