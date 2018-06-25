using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class FormIDLinkBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<FormID>
    {
        public override string Typename(TypeGeneration typeGen)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    return base.Typename(typeGen);
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    return "RecordType";
                default:
                    throw new NotImplementedException();
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
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine("r.Position += Constants.SUBRECORD_LENGTH;");
            }
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    using (var args = new ArgsWrapper(fg,
                        $"{retAccessor}{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse",
                        suffixLine: $".Bubble((o) => new {linkType.TypeName}(o))"))
                    {
                        args.Add(nodeAccessor);
                        args.Add($"errorMask: {maskAccessor}");
                        foreach (var arg in AdditionCopyInRetParameters(
                            fg: fg,
                            objGen: objGen,
                            typeGen: typeGen,
                            nodeAccessor: nodeAccessor,
                            retAccessor: retAccessor,
                            outItemAccessor: outItemAccessor,
                            maskAccessor: maskAccessor))
                        {
                            args.Add(arg);
                        }
                    }
                    break;
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    fg.AppendLine($"{maskAccessor} = null;");
                    fg.AppendLine($"return TryGet<{linkType.TypeName}>.Succeed(new {linkType.TypeName}(HeaderTranslation.ReadNextRecordType(r.Reader)));");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void GenerateCopyIn(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string frameAccessor, Accessor itemAccessor, string maskAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine($"{frameAccessor}.Position += Constants.SUBRECORD_LENGTH;");
            }
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.ParseInto"))
            {
                args.Add($"frame: {frameAccessor}.Spawn(snapToFinalPosition: false)");
                args.Add($"fieldIndex: {typeGen.IndexEnumInt}");
                args.Add($"item: {itemAccessor.PropertyAccess}");
                args.Add($"errorMask: {maskAccessor}");
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
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    base.GenerateWrite(fg, objGen, typeGen, writerAccessor, itemAccessor, maskAccessor);
                    break;
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
                    using (var args = new ArgsWrapper(fg,
                        $"{this.Namespace}RecordTypeBinaryTranslation.Instance.Write"))
                    {
                        args.Add($"writer: {writerAccessor}");
                        args.Add($"item: {ItemWriteAccess(itemAccessor)}");
                        if (typeGen.HasIndex)
                        {
                            args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                        }
                        args.Add($"errorMask: {maskAccessor}");
                        if (data.RecordType.HasValue)
                        {
                            args.Add($"header: recordTypeConverter.Convert({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                            args.Add($"nullable: {(data.Optional ? "true" : "false")}");
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
