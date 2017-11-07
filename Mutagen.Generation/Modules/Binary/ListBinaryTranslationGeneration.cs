using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public class ListBinaryTranslationGeneration : BinaryTranslationGeneration
    {
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

            var subMaskStr = subTransl.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration);
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ListBinaryTranslation<{list.SubTypeGeneration.TypeName}, {subMaskStr}>.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor.PropertyOrDirectAccess}");
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                args.Add($"doMasks: {doMaskAccessor}");
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
                            doMaskAccessor: doMaskAccessor,
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
            GenerateCopyInRet(fg, objGen, typeGen, nodeAccessor, $"var {typeGen.Name}tryGet = ", doMaskAccessor, maskAccessor);
            if (itemAccessor.PropertyAccess != null)
            {
                fg.AppendLine($"{itemAccessor.PropertyAccess}.{nameof(INotifyingCollectionExt.SetIfSucceeded)}({typeGen.Name}tryGet);");
            }
            else
            {
                fg.AppendLine("if ({typeGen.Name}tryGet.Succeeded)");
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
            string retAccessor,
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

            if (data.MarkerType.HasValue)
            {
                fg.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH + contentLength; // Skip marker");
            }

            var subMaskStr = subTransl.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration);
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}ListBinaryTranslation<{list.SubTypeGeneration.TypeName}, {subMaskStr}>.Instance.ParseRepeatedItem"))
            {
                args.Add($"frame: frame");
                if (list.MaxValue.HasValue)
                {
                    args.Add($"amount: {list.MaxValue.Value}");
                }
                else if (!string.IsNullOrWhiteSpace(subData.TriggeringRecordAccessor))
                {
                    args.Add($"triggeringRecord: {subData.TriggeringRecordAccessor}");
                }
                else
                {
                    throw new ArgumentException();
                }
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                args.Add($"doMasks: {doMaskAccessor}");
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
                        var xmlGen = this.Module.GetTypeGeneration(list.SubTypeGeneration.GetType());
                        xmlGen.GenerateCopyInRet(gen, objGen, list.SubTypeGeneration, "r", "return ", "listDoMasks", "listSubMask");
                    }
                });
            }
        }
    }
}
