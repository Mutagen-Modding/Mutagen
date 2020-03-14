using Loqui;
using Noggog;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class DictBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public virtual string TranslatorName => "DictBinaryTranslation";
        
        const string ThreadKey = "DictThread";

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

        private ListBinaryType GetDictType(
            DictType dict,
            MutagenFieldData data,
            MutagenFieldData subData)
        {
            if (subData.HasTrigger)
            {
                return ListBinaryType.SubTrigger;
            }
            else if (data.RecordType.HasValue)
            {
                return ListBinaryType.Trigger;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor writerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMask)
        {
            var dict = typeGen as DictType;
            if (dict.Mode != DictMode.KeyedValue)
            {
                throw new NotImplementedException();
            }
            if (!this.Module.TryGetTypeGeneration(dict.ValueTypeGen.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + dict.ValueTypeGen);
            }

            if (typeGen.TryGetFieldData(out var data)
                && data.MarkerType.HasValue)
            {
                fg.AppendLine($"using (HeaderExport.ExportHeader(writer, {objGen.RegistrationName}.{data.MarkerType.Value.Type}_HEADER, ObjectType.Subrecord)) {{ }}");
            }

            var subData = dict.ValueTypeGen.GetFieldData();

            ListBinaryType listBinaryType = GetDictType(dict, data, subData);

            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ListBinaryTranslation<{dict.ValueTypeGen.TypeName(getter: true)}>.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"items: {itemAccessor.PropertyOrDirectAccess}.Items");
                if (listBinaryType == ListBinaryType.Trigger)
                {
                    args.Add($"recordType: {objGen.RecordTypeHeaderName(data.RecordType.Value)}");
                }
                if (subTransl.AllowDirectWrite(objGen, typeGen))
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(dict.ValueTypeGen, getter: true)}.Write");
                }
                else
                {
                    args.Add((gen) =>
                    {
                        gen.AppendLine($"transl: (MutagenWriter r, {dict.ValueTypeGen.TypeName(getter: true)} dictSubItem) =>");
                        using (new BraceWrapper(gen))
                        {
                            LoquiType targetLoqui = dict.ValueTypeGen as LoquiType;
                            subTransl.GenerateWrite(
                                fg: gen,
                                objGen: objGen,
                                typeGen: targetLoqui,
                                itemAccessor: new Accessor("dictSubItem"),
                                writerAccessor: "r",
                                translationAccessor: "dictTranslMask",
                                errorMaskAccessor: null);
                        }
                    });
                }
            }
        }

        public override void GenerateCopyIn(
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

            ListBinaryType listBinaryType = GetDictType(dict, data, subData);

            if (data.MarkerType.HasValue)
            {
                fg.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH + long; // Skip marker");
            }
            else if (listBinaryType == ListBinaryType.Trigger)
            {
                fg.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH;");
            }

            using (var args = new ArgsWrapper(fg,
                $"{Loqui.Generation.Utility.Await(isAsync)}{this.Namespace}List{(isAsync ? "Async" : null)}BinaryTranslation<{dict.ValueTypeGen.TypeName(getter: false)}>.Instance.ParseRepeatedItem",
                suffixLine: Loqui.Generation.Utility.ConfigAwait(isAsync)))
            {
                if (listBinaryType == ListBinaryType.SubTrigger)
                {
                    args.Add($"frame: frame");
                    args.Add($"triggeringRecord: {subData.TriggeringRecordSetAccessor}");
                }
                else if (listBinaryType == ListBinaryType.Trigger)
                {
                    args.Add($"frame: frame.Spawn(long)");
                }
                else
                {
                    throw new NotImplementedException();
                }
                args.Add($"item: {itemAccessor.PropertyAccess}");
                if (dict.CustomData.TryGetValue("lengthLength", out object len))
                {
                    args.Add($"lengthLength: {len}");
                }
                else if (dict.ValueTypeGen is LoquiType loqui)
                {
                    switch (loqui.TargetObjectGeneration.GetObjectType())
                    {
                        case ObjectType.Subrecord:
                            args.Add($"lengthLength: Mutagen.Bethesda.Constants.SUBRECORD_LENGTHLENGTH");
                            break;
                        case ObjectType.Group:
                        case ObjectType.Record:
                            args.Add($"lengthLength: Mutagen.Bethesda.Constants.RECORD_LENGTHLENGTH");
                            break;
                        case ObjectType.Mod:
                        default:
                            throw new ArgumentException();
                    }
                }
                else
                {
                    args.Add($"lengthLength: Mutagen.Bethesda.Constants.SUBRECORD_LENGTHLENGTH");
                }
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
                                            mastersAccessor: "masterReferences",
                                            asyncMode: AsyncMode.Direct,
                                            errorMaskAccessor: null);
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
                                mastersAccessor: "masterReferences",
                                asyncMode: AsyncMode.Direct,
                                errorMaskAccessor: null);
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
            Accessor mastersAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor dataAccessor,
            int? passedLength,
            DataType data = null)
        {
        }

        public override int? GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen) => 0;

        public override int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen) => null;
    }
}
