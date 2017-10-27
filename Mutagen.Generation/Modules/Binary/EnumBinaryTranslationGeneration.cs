using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public class EnumBinaryTranslationGeneration : BinaryTranslationGeneration
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
            var eType = typeGen as EnumType;
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{Namespace}EnumBinaryTranslation<{eType.NoNullTypeName}>.Instance.Write"))
            {
                args.Add(writerAccessor);
                args.Add($"{itemAccessor.PropertyOrDirectAccess}");
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"length: new ContentLength({eType.ByteLength})");
                if (typeGen.HasIndex)
                {
                    args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                    args.Add($"errorMask: {maskAccessor}");
                }
                else
                {
                    args.Add($"errorMask: out {maskAccessor}");
                }
                if (data.TriggeringRecordAccessor != null)
                {
                    args.Add($"header: {data.TriggeringRecordAccessor}");
                    args.Add($"nullable: {(data.Optional ? "true" : "false")}");
                }
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
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            var eType = typeGen as EnumType;
            if (data.TriggeringRecordAccessor != null)
            {
                fg.AppendLine($"{nodeAccessor}.Position += Constants.SUBRECORD_LENGTH;");
            }
            using (var args = new ArgsWrapper(fg,
                $"var {typeGen.Name}tryGet = {this.Namespace}EnumBinaryTranslation<{eType.NoNullTypeName}>.Instance.Parse"))
            {
                if (data.TriggeringRecordAccessor != null)
                {
                    args.Add($"{nodeAccessor}.Spawn(contentLength)");
                }
                else
                {
                    args.Add($"frame: {nodeAccessor}.Spawn(new ContentLength({eType.ByteLength}))");
                }
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: {maskAccessor}");
            }
            if (itemAccessor.PropertyAccess != null)
            {
                fg.AppendLine($"{itemAccessor.PropertyAccess}.{nameof(INotifyingCollectionExt.SetIfSucceeded)}({typeGen.Name}tryGet);");
            }
            else
            {
                fg.AppendLine("if (tryGet.Succeeded)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"{itemAccessor.DirectAccess} = tryGet.Value;");
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
            var eType = typeGen as EnumType;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}EnumBinaryTranslation<{eType.NoNullTypeName}>.Instance.Parse"))
            {
                args.Add($"frame: {nodeAccessor}.Spawn(new ContentLength({eType.ByteLength}))");
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
            }
        }
    }
}
