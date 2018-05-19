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
            string doMaskAccessor,
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

            var subMaskStr = subTransl.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration);
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ListBinaryTranslation<{list.SubTypeGeneration.TypeName}, {subMaskStr}>.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor.PropertyOrDirectAccess}");
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                if (listBinaryType == ListBinaryType.Trigger)
                {
                    args.Add($"recordType: {objGen.RecordTypeHeaderName(data.RecordType.Value)}");
                }
                args.Add($"errorMask: {maskAccessor}");
                args.Add((gen) =>
                {
                    gen.AppendLine($"transl: (MutagenWriter subWriter, {list.SubTypeGeneration.TypeName} subItem, bool listDoMasks, out {subMaskStr} listSubMask) =>");
                    using (new BraceWrapper(gen))
                    {
                        subTransl.GenerateWrite(
                            fg: gen,
                            objGen: objGen,
                            typeGen: list.SubTypeGeneration,
                            writerAccessor: "subWriter",
                            itemAccessor: new Accessor($"subItem"),
                            doMaskAccessor: $"listDoMasks",
                            maskAccessor: $"listSubMask");
                    }
                });
            }
        }

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string nodeAccessor,
            Accessor itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            GenerateCopyInRet(
                fg: fg, 
                objGen: objGen, 
                targetGen: typeGen, 
                typeGen: typeGen, 
                nodeAccessor: nodeAccessor, 
                squashedRepeatedList: false,
                retAccessor: new Accessor(typeGen, "item."), 
                doMaskAccessor: doMaskAccessor, 
                maskAccessor: maskAccessor);
            if (itemAccessor.PropertyAccess == null)
            {
                fg.AppendLine($"if ({typeGen.Name}tryGet.Succeeded)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"{itemAccessor.DirectAccess}.SetTo({typeGen.Name}tryGet.Value);");
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
            Accessor retAccessor,
            string doMaskAccessor,
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

            var subMaskStr = subTransl.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration);
            ArgsWrapper args;
            if (retAccessor.PropertyAccess != null)
            {
                args = new ArgsWrapper(fg, $"{retAccessor.PropertyAccess}.{nameof(INotifyingCollectionExt.SetIfSucceededOrDefault)}({this.Namespace}ListBinaryTranslation<{list.SubTypeGeneration.TypeName}, {subMaskStr}>.Instance.ParseRepeatedItem",
                    suffixLine: ")");
            }
            else
            {
                args = new ArgsWrapper(fg, $"var {typeGen.Name}tryGet = {this.Namespace}ListBinaryTranslation<{list.SubTypeGeneration.TypeName}, {subMaskStr}>.Instance.ParseRepeatedItem");
            }
            using (args)
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
                args.Add((gen) =>
                {
                    var subGenTypes = subData.GenerationTypes.ToList();
                    gen.AppendLine($"transl: (MutagenFrame r{(subGenTypes.Count <= 1 ? string.Empty : ", RecordType header")}, bool listDoMasks, out {typeGen.ProtoGen.Gen.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration)} listSubMask) =>");
                    using (new BraceWrapper(gen))
                    {
                        var subGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
                        if (subGenTypes.Count <= 1)
                        {
                            subGen.GenerateCopyInRet(
                                fg: gen,
                                objGen: objGen,
                                targetGen: list.SubTypeGeneration,
                                typeGen: list.SubTypeGeneration,
                                readerAccessor: "r",
                                squashedRepeatedList: listBinaryType == ListBinaryType.Trigger,
                                retAccessor: new Accessor("return "),
                                doMaskAccessor: "listDoMasks",
                                maskAccessor: "listSubMask");
                        }
                        else
                        {
                            gen.AppendLine($"TryGet<{list.SubTypeGeneration.TypeName}> ret;");
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
                                    var submaskName = $"{item.Key.First()}SubMask";
                                    using (new DepthWrapper(gen))
                                    {
                                        subGen.GenerateCopyInRet(
                                            fg: gen,
                                            objGen: objGen,
                                            targetGen: list.SubTypeGeneration,
                                            typeGen: item.Value,
                                            readerAccessor: "r",
                                            squashedRepeatedList: listBinaryType == ListBinaryType.Trigger,
                                            retAccessor: new Accessor("ret = "),
                                            doMaskAccessor: "listDoMasks",
                                            maskAccessor: $"var {submaskName}");
                                        gen.AppendLine($"listSubMask = {submaskName}.Bubble<{specificLoqui.Mask(MaskType.Error)}, {targetLoqui.Mask(MaskType.Error)}>();");
                                        gen.AppendLine($"break;");
                                    }
                                }
                                gen.AppendLine("default:");
                                using (new DepthWrapper(gen))
                                {
                                    gen.AppendLine("throw new NotImplementedException();");
                                }
                            }
                            gen.AppendLine($"return ret;");
                        }
                    }
                });
            }
        }
    }
}
