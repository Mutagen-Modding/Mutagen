using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class PrimitiveBinaryTranslationGeneration<T> : BinaryTranslationGeneration
    {
        private string typeName;
        protected bool? nullable;
        public bool Nullable => nullable ?? false || typeof(T).GetName().EndsWith("?");

        public override string GetTranslatorInstance(TypeGeneration typeGen)
        {
            return $"{typeName}BinaryTranslation.Instance";
        }

        public PrimitiveBinaryTranslationGeneration(string typeName = null, bool? nullable = null)
        {
            this.nullable = nullable;
            this.typeName = typeName ?? typeof(T).GetName().Replace("?", string.Empty);
        }

        protected virtual string ItemWriteAccess(Accessor itemAccessor)
        {
            return itemAccessor.PropertyOrDirectAccess;
        }

        public virtual string Typename(TypeGeneration typeGen) => typeName;

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string writerAccessor,
            Accessor itemAccessor,
            string maskAccessor,
            string translationMaskAccessor)
        {
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Write"))
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
                    args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                    args.Add($"nullable: {(data.Optional ? "true" : "false")}");
                }
                foreach (var arg in AdditionalWriteParameters(
                    fg: fg,
                    objGen: objGen,
                    typeGen: typeGen,
                    writerAccessor: writerAccessor,
                    itemAccessor: itemAccessor,
                    maskAccessor: maskAccessor))
                {
                    args.Add(arg);
                }
            }
        }

        protected virtual IEnumerable<string> AdditionalWriteParameters(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string writerAccessor,
            Accessor itemAccessor,
            string maskAccessor)
        {
            yield break;
        }

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string frameAccessor,
            Accessor itemAccessor,
            string maskAccessor,
            string translationMaskAccessor)
        {
            var prim = typeGen as PrimitiveType;
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            if (data.HasTrigger)
            {
                fg.AppendLine($"{frameAccessor}.Position += Mutagen.Bethesda.Constants.SUBRECORD_LENGTH;");
            }


            List<string> extraArgs = new List<string>();
            extraArgs.Add($"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : ".Spawn(snapToFinalPosition: false)")}");
            foreach (var arg in AdditionalCopyInParameters(
                fg: fg,
                objGen: objGen,
                typeGen: typeGen,
                nodeAccessor: frameAccessor,
                itemAccessor: itemAccessor,
                maskAccessor: maskAccessor))
            {
                extraArgs.Add(arg);
            }

            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance",
                    MaskAccessor = maskAccessor,
                    ItemAccessor = itemAccessor,
                    TranslationMaskAccessor = null,
                    IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                    ExtraArgs = extraArgs.ToArray()
                });
        }

        protected virtual IEnumerable<string> AdditionalCopyInParameters(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string nodeAccessor,
            Accessor itemAccessor,
            string maskAccessor)
        {
            yield break;
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
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine("r.Position += Constants.SUBRECORD_LENGTH;");
            }
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor);
                args.Add($"errorMask: {maskAccessor}");
                args.Add($"translationMask: {translationMaskAccessor}");
                foreach (var arg in AdditionalCopyInRetParameters(
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
        }

        protected virtual IEnumerable<string> AdditionalCopyInRetParameters(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string nodeAccessor,
            string retAccessor,
            Accessor outItemAccessor,
            string maskAccessor)
        {
            yield break;
        }
    }
}
