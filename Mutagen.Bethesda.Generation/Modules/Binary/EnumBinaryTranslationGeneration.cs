using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
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
                args.Add($"length: {eType.ByteLength}");
                if (typeGen.HasIndex)
                {
                    args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                    args.Add($"errorMask: {maskAccessor}");
                }
                else
                {
                    args.Add($"doMasks: {doMaskAccessor}");
                    args.Add($"errorMask: out {maskAccessor}");
                }
                if (data.RecordType.HasValue)
                {
                    args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
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
            if (data.HasTrigger)
            {
                fg.AppendLine($"{nodeAccessor}.Position += Constants.SUBRECORD_LENGTH;");
            }
            ArgsWrapper args;
            if (itemAccessor.PropertyAccess != null)
            {
                args = new ArgsWrapper(fg, $"{itemAccessor.PropertyAccess}.{nameof(INotifyingCollectionExt.SetIfSucceededOrDefault)}({this.Namespace}EnumBinaryTranslation<{eType.NoNullTypeName}>.Instance.Parse",
                    suffixLine: ")");
            }
            else
            {
                args = new ArgsWrapper(fg, $"var {typeGen.Name}tryGet = {this.Namespace}EnumBinaryTranslation<{eType.NoNullTypeName}>.Instance.Parse");
            }
            using (args)
            {
                if (data.HasTrigger)
                {
                    args.Add($"{nodeAccessor}.SpawnWithLength(contentLength)");
                }
                else
                {
                    args.Add($"frame: {nodeAccessor}.SpawnWithLength({eType.ByteLength})");
                }
                args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                args.Add($"errorMask: {maskAccessor}");
            }
            if (itemAccessor.PropertyAccess == null)
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
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            string nodeAccessor,
            bool squashedRepeatedList,
            Accessor retAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var eType = typeGen as EnumType;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor.DirectAccess}{this.Namespace}EnumBinaryTranslation<{eType.NoNullTypeName}>.Instance.Parse"))
            {
                args.Add($"frame: {nodeAccessor}.SpawnWithLength({eType.ByteLength})");
                args.Add($"doMasks: {doMaskAccessor}");
                args.Add($"errorMask: out {maskAccessor}");
            }
        }
    }
}
