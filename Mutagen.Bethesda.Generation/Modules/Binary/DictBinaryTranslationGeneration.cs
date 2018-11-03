using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class DictBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public virtual string TranslatorName => "DictBinaryTranslation";

        public override string GetTranslatorInstance(TypeGeneration typeGen)
        {
            var dictType = typeGen as DictType;
            var keyMask = this.MaskModule.GetMaskModule(dictType.KeyTypeGen.GetType()).GetErrorMaskTypeStr(dictType.KeyTypeGen);
            var valMask = this.MaskModule.GetMaskModule(dictType.ValueTypeGen.GetType()).GetErrorMaskTypeStr(dictType.ValueTypeGen);
            return $"{TranslatorName}<{dictType.KeyTypeGen.TypeName}, {dictType.ValueTypeGen.TypeName}, {keyMask}, {valMask}>.Instance";
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
            string writerAccessor,
            Accessor itemAccessor,
            string maskAccessor,
            string translationMask)
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
                $"{this.Namespace}ListBinaryTranslation<{dict.ValueTypeGen.TypeName}>.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"items: {itemAccessor.PropertyOrDirectAccess}");
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                if (listBinaryType == ListBinaryType.Trigger)
                {
                    args.Add($"recordType: {objGen.RecordTypeHeaderName(data.RecordType.Value)}");
                }
                args.Add($"errorMask: {maskAccessor}");
                if (subTransl.AllowDirectWrite(objGen, typeGen))
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(dict.ValueTypeGen)}.Write");
                }
                else
                {
                    args.Add((gen) =>
                    {
                        gen.AppendLine($"transl: (MutagenWriter r, {dict.ValueTypeGen.TypeName} dictSubItem, ErrorMaskBuilder dictSubMask) =>");
                        using (new BraceWrapper(gen))
                        {
                            LoquiType targetLoqui = dict.ValueTypeGen as LoquiType;
                            using (new DepthWrapper(gen))
                            {
                                subTransl.GenerateWrite(
                                    fg: gen,
                                    objGen: objGen,
                                    typeGen: targetLoqui,
                                    itemAccessor: new Accessor("dictSubItem"),
                                    writerAccessor: "r",
                                    translationAccessor: "dictTranslMask",
                                    maskAccessor: "dictSubMask");
                            }
                        }
                    });
                }
            }
        }

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string nodeAccessor,
            Accessor itemAccessor,
            string maskAccessor,
            string translationMaskAccessor)
        {
            var dict = typeGen as DictType;
            var data = dict.GetFieldData();
            var subData = dict.ValueTypeGen.GetFieldData();
            if (!this.Module.TryGetTypeGeneration(dict.ValueTypeGen.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + dict.ValueTypeGen);
            }

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
                $"{this.Namespace}ListBinaryTranslation<{dict.ValueTypeGen.TypeName}>.Instance.ParseRepeatedItem"))
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
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
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
                args.Add($"errorMask: {maskAccessor}");
                var subGenTypes = subData.GenerationTypes.ToList();
                var subGen = this.Module.GetTypeGeneration(dict.ValueTypeGen.GetType());
                if (subGenTypes.Count <= 1
                    && subTransl.AllowDirectParse(objGen, typeGen, squashedRepeatedList: false))
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(dict.ValueTypeGen)}.Parse");
                }
                else if (subGenTypes.Count > 1)
                {
                    args.Add((gen) =>
                    {
                        gen.AppendLine($"transl: (MutagenFrame r, RecordType header, out {dict.ValueTypeGen.TypeName} dictSubItem, ErrorMaskBuilder dictSubMask) =>");
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
                                            squashedRepeatedList: false,
                                            retAccessor: "return ",
                                            translationAccessor: "dictTranslMask",
                                            outItemAccessor: new Accessor("dictSubItem"),
                                            maskAccessor: "dictSubMask");
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
                        gen.AppendLine($"transl: (MutagenFrame r, out {dict.ValueTypeGen.TypeName} dictSubItem, ErrorMaskBuilder dictSubMask) =>");
                        using (new BraceWrapper(gen))
                        {
                            LoquiType targetLoqui = dict.ValueTypeGen as LoquiType;
                            subGen.GenerateCopyInRet(
                                fg: gen,
                                objGen: objGen,
                                targetGen: dict.ValueTypeGen,
                                typeGen: targetLoqui,
                                readerAccessor: "r",
                                squashedRepeatedList: false,
                                retAccessor: "return ",
                                translationAccessor: "dictTranslMask",
                                outItemAccessor: new Accessor("dictSubItem"),
                                maskAccessor: "dictSubMask");
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
            string nodeAccessor,
            bool squashedRepeatedList,
            string retAccessor,
            Accessor outItemAccessor,
            string maskAccessor,
            string translationMaskAccessor)
        {
            throw new NotImplementedException();
        }
    }
}
