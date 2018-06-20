using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public enum ListBinaryType
    {
        Amount,
        SubTrigger,
        Trigger,
        Frame
    }

    public class ListBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public virtual string TranslatorName => $"ListBinaryTranslation";

        public override string GetTranslatorInstance(TypeGeneration typeGen)
        {
            var list = typeGen as ListType;
            if (!Module.TryGetTypeGeneration(list.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + list.SubTypeGeneration);
            }

            var subMaskStr = subTransl.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration);
            return $"{TranslatorName}<{list.SubTypeGeneration.TypeName}, {subMaskStr}>.Instance";
        }

        private ListBinaryType GetListType(
            ListType list,
            MutagenFieldData data,
            MutagenFieldData subData)
        {
            if (list.MaxValue.HasValue)
            {
                return ListBinaryType.Amount;
            }
            else if (subData.HasTrigger)
            {
                return ListBinaryType.SubTrigger;
            }
            else if (data.RecordType.HasValue)
            {
                return ListBinaryType.Trigger;
            }
            else
            {
                return ListBinaryType.Frame;
            }
        }

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string writerAccessor,
            Accessor itemAccessor,
            string maskAccessor)
        {
            var list = typeGen as ListType;
            if (!this.Module.TryGetTypeGeneration(list.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + list.SubTypeGeneration);
            }

            if (typeGen.TryGetFieldData(out var data)
                && data.MarkerType.HasValue)
            {
                fg.AppendLine($"using (HeaderExport.ExportHeader(writer, {objGen.RegistrationName}.{data.MarkerType.Value.Type}_HEADER, ObjectType.Subrecord)) {{ }}");
            }

            var subData = list.SubTypeGeneration.GetFieldData();

            ListBinaryType listBinaryType = GetListType(list, data, subData);

            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ListBinaryTranslation<{list.SubTypeGeneration.TypeName}>.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"items: {itemAccessor.PropertyOrDirectAccess}");
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                if (listBinaryType == ListBinaryType.Trigger)
                {
                    args.Add($"recordType: {objGen.RecordTypeHeaderName(data.RecordType.Value)}");
                }
                args.Add($"errorMask: {maskAccessor}");
                if (subTransl.AllowDirectWrite)
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(list.SubTypeGeneration)}.Write");
                }
                else
                {
                    args.Add((gen) =>
                    {
                        gen.AppendLine($"transl: (MutagenWriter subWriter, {list.SubTypeGeneration.TypeName} subItem, ErrorMaskBuilder listErrorMask) =>");
                        using (new BraceWrapper(gen))
                        {
                            subTransl.GenerateWrite(
                                fg: gen,
                                objGen: objGen,
                                typeGen: list.SubTypeGeneration,
                                writerAccessor: "subWriter",
                                itemAccessor: new Accessor($"subItem"),
                                maskAccessor: $"listErrorMask");
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
            string maskAccessor)
        {
            var list = typeGen as ListType;
            var data = list.GetFieldData();
            var subData = list.SubTypeGeneration.GetFieldData();
            if (!this.Module.TryGetTypeGeneration(list.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + list.SubTypeGeneration);
            }

            ListBinaryType listBinaryType = GetListType(list, data, subData);

            if (data.MarkerType.HasValue)
            {
                fg.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH + contentLength; // Skip marker");
            }
            else if (listBinaryType == ListBinaryType.Trigger)
            {
                fg.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH;");
            }

            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ListBinaryTranslation<{list.SubTypeGeneration.TypeName}>.Instance.ParseRepeatedItem"))
            {
                if (listBinaryType == ListBinaryType.Amount)
                {
                    args.Add($"frame: frame");
                    args.Add($"amount: {list.MaxValue.Value}");
                }
                else if (listBinaryType == ListBinaryType.SubTrigger)
                {
                    args.Add($"frame: frame");
                    args.Add($"triggeringRecord: {subData.TriggeringRecordSetAccessor}");
                }
                else if (listBinaryType == ListBinaryType.Trigger)
                {
                    args.Add($"frame: frame.SpawnWithLength(contentLength)");
                }
                else if (listBinaryType == ListBinaryType.Frame)
                {
                    args.Add($"frame: frame");
                }
                else
                {
                    throw new NotImplementedException();
                }
                args.Add($"item: {itemAccessor.PropertyAccess}");
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                if (list.CustomData.TryGetValue("lengthLength", out object len))
                {
                    args.Add($"lengthLength: {len}");
                }
                else if (!list.MaxValue.HasValue)
                {
                    if (list.SubTypeGeneration is LoquiType loqui)
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
                }
                args.Add($"errorMask: {maskAccessor}");
                var subGenTypes = subData.GenerationTypes.ToList();
                var subGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
                if (subGenTypes.Count <= 1 && subTransl.AllowDirectParse)
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(list.SubTypeGeneration)}.Parse");
                }
                else
                {
                    args.Add((gen) =>
                    {
                        gen.AppendLine($"transl: (MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}, out {list.SubTypeGeneration.TypeName} listSubItem, ErrorMaskBuilder listErrMask) =>");
                        using (new BraceWrapper(gen))
                        {
                            if (subGenTypes.Count <= 1)
                            {
                                subGen.GenerateCopyInRet(
                                fg: gen,
                                objGen: objGen,
                                targetGen: list.SubTypeGeneration,
                                typeGen: list.SubTypeGeneration,
                                readerAccessor: "r",
                                squashedRepeatedList: listBinaryType == ListBinaryType.Trigger,
                                retAccessor: "return ",
                                outItemAccessor: new Accessor("listSubItem"),
                                maskAccessor: "listErrMask");
                            }
                            else
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
                                        LoquiType targetLoqui = list.SubTypeGeneration as LoquiType;
                                        LoquiType specificLoqui = item.Value as LoquiType;
                                        using (new DepthWrapper(gen))
                                        {
                                            subGen.GenerateCopyInRet(
                                                fg: gen,
                                                objGen: objGen,
                                                targetGen: list.SubTypeGeneration,
                                                typeGen: item.Value,
                                                readerAccessor: "r",
                                                squashedRepeatedList: listBinaryType == ListBinaryType.Trigger,
                                                retAccessor: "return ",
                                                outItemAccessor: new Accessor("listSubItem"),
                                                maskAccessor: $"listErrMask");
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
            string maskAccessor)
        {
            throw new NotImplementedException();
        }
    }
}
