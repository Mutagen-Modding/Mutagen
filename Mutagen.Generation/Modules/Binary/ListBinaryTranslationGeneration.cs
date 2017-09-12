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
            string itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var list = typeGen as ListType;
            if (!this.Module.TryGetTypeGeneration(list.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + list.SubTypeGeneration);
            }

            var subMaskStr = subTransl.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration);
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}ListBinaryTranslation<{list.SubTypeGeneration.TypeName}, {subMaskStr}>.Instance.Write"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor}");
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"maskObj: out {maskAccessor}");
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
                            itemAccessor: $"subItem", 
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
            GenerateCopyInRet(fg, objGen, typeGen, nodeAccessor, "var listTryGet = ", doMaskAccessor, maskAccessor);
            if (itemAccessor.PropertyAccess != null)
            {
                fg.AppendLine($"{itemAccessor.PropertyAccess}.{nameof(INotifyingCollectionExt.SetIfSucceeded)}(listTryGet);");
            }
            else
            {
                fg.AppendLine("if (listTryGet.Succeeded)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"{itemAccessor.DirectAccess} = listTryGet.Value;");
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
            if (!this.Module.TryGetTypeGeneration(list.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + list.SubTypeGeneration);
            }
            var subMaskStr = subTransl.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration);
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}ListBinaryTranslation<{list.SubTypeGeneration.TypeName}, {subMaskStr}>.Instance.ParseRepeatedItem"))
            {
                args.Add($"reader: reader");
                args.Add($"triggeringRecord: {objGen.Name}.{data.RecordType.Value.HeaderName}");
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"maskObj: out {maskAccessor}");
                args.Add((gen) =>
                {
                    gen.AppendLine($"transl: (BinaryReader r, bool listDoMasks, out {typeGen.ProtoGen.Gen.MaskModule.GetMaskModule(list.SubTypeGeneration.GetType()).GetErrorMaskTypeStr(list.SubTypeGeneration)} listSubMask) =>");
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
