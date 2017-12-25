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
                return ListBinaryType.Trigger;
            }
            else if (data.RecordType.HasValue)
            {
                return ListBinaryType.Frame;
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
                if (listBinaryType == ListBinaryType.Frame)
                {
                    args.Add($"recordType: {objGen.RecordTypeHeaderName(data.RecordType.Value)}");
                }
                args.Add($"errorMask: {maskAccessor}");
                args.Add((gen) =>
                {
                    gen.AppendLine($"transl: ({list.SubTypeGeneration.TypeName} subItem, bool listDoMasks, out {subMaskStr} listSubMask) =>");
                    using (new BraceWrapper(gen))
                    {
                        subTransl.GenerateWrite(
                            fg: gen,
                            objGen: objGen,
                            typeGen: list.SubTypeGeneration, 
                            writerAccessor: "writer", 
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
            GenerateCopyInRet(fg, objGen, typeGen, nodeAccessor, new Accessor($"var {typeGen.Name}tryGet = "), doMaskAccessor, maskAccessor);
            if (itemAccessor.PropertyAccess != null)
            {
                fg.AppendLine($"{itemAccessor.PropertyAccess}.{nameof(INotifyingCollectionExt.SetIfSucceeded)}({typeGen.Name}tryGet);");
            }
            else
            {
                fg.AppendLine($"if ({typeGen.Name}tryGet.Succeeded)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"{itemAccessor.DirectAccess} = {typeGen.Name}tryGet.Value;");
                }
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string nodeAccessor, 
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
            else if (listBinaryType == ListBinaryType.Frame)
            {
                fg.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH;");
            }

            var subMaskStr = subTransl.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration);
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor.DirectAccess}{this.Namespace}ListBinaryTranslation<{list.SubTypeGeneration.TypeName}, {subMaskStr}>.Instance.ParseRepeatedItem"))
            {
                if (listBinaryType == ListBinaryType.Amount)
                {
                    args.Add($"frame: frame");
                    args.Add($"amount: {list.MaxValue.Value}");
                }
                else if (listBinaryType == ListBinaryType.Trigger)
                {
                    args.Add($"frame: frame");
                    args.Add($"triggeringRecord: {subData.TriggeringRecordSetAccessor}");
                }
                else if (listBinaryType == ListBinaryType.Frame)
                {
                    args.Add($"frame: frame.Spawn(contentLength)");
                }
                else
                {
                    throw new NotImplementedException();
                }
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                if (!list.MaxValue.HasValue)
                {
                    if (list.SubTypeGeneration is LoquiType loqui)
                    {
                        args.Add($"objType: {nameof(ObjectType)}.{loqui.TargetObjectGeneration.GetObjectType()}");
                    }
                    else
                    {
                        args.Add($"objType: {nameof(ObjectType)}.{ObjectType.Subrecord}");
                    }
                }
                args.Add($"errorMask: {maskAccessor}");
                args.Add((gen) =>
                {
                    gen.AppendLine($"transl: (MutagenFrame r, bool listDoMasks, out {typeGen.ProtoGen.Gen.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration)} listSubMask) =>");
                    using (new BraceWrapper(gen))
                    {
                        var subGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
                        subGen.GenerateCopyInRet(gen, objGen, list.SubTypeGeneration, "r", new Accessor("return "), "listDoMasks", "listSubMask");
                    }
                });
            }
        }
    }
}
