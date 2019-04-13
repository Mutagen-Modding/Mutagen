using Loqui;
using Loqui.Generation;
using Noggog;
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
            return itemAccessor.DirectAccess;
        }

        public virtual string Typename(TypeGeneration typeGen) => typeName;

        public delegate TryGet<string> ParamTest(
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor accessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor);
        public List<ParamTest> AdditionalWriteParams = new List<ParamTest>();
        public List<ParamTest> AdditionalCopyInParams = new List<ParamTest>();
        public List<ParamTest> AdditionalCopyInRetParams = new List<ParamTest>();

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor writerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
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
                args.Add($"errorMask: {errorMaskAccessor}");
                if (data.RecordType.HasValue)
                {
                    args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                    args.Add($"nullable: {(data.Optional ? "true" : "false")}");
                }
                foreach (var writeParam in this.AdditionalWriteParams)
                {
                    var get = writeParam(
                        objGen: objGen,
                        typeGen: typeGen,
                        accessor: writerAccessor,
                        itemAccessor: itemAccessor,
                        errorMaskAccessor: errorMaskAccessor,
                        translationMaskAccessor: translationMaskAccessor);
                    if (get.Failed) continue;
                    args.Add(get.Value);
                }
            }
        }

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor frameAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
            if (data.HasTrigger)
            {
                fg.AppendLine($"{frameAccessor}.Position += Mutagen.Bethesda.Constants.SUBRECORD_LENGTH;");
            }


            List<string> extraArgs = new List<string>();
            extraArgs.Add($"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : ".Spawn(snapToFinalPosition: false)")}");
            foreach (var writeParam in this.AdditionalCopyInParams)
            {
                var get = writeParam(
                    objGen: objGen,
                    typeGen: typeGen,
                    accessor: frameAccessor,
                    itemAccessor: itemAccessor,
                    errorMaskAccessor: errorMaskAccessor,
                    translationMaskAccessor: translationMaskAccessor);
                if (get.Failed) continue;
                extraArgs.Add(get.Value);
            }

            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance",
                    MaskAccessor = errorMaskAccessor,
                    ItemAccessor = itemAccessor,
                    TranslationMaskAccessor = null,
                    IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                    ExtraArgs = extraArgs.ToArray()
                });
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            bool squashedRepeatedList,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine("r.Position += Constants.SUBRECORD_LENGTH;");
            }
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor.DirectAccess);
                args.Add($"errorMask: {errorMaskAccessor}");
                args.Add($"translationMask: {translationMaskAccessor}");
                foreach (var writeParam in this.AdditionalCopyInRetParams)
                {
                    var get = writeParam(
                        objGen: objGen,
                        typeGen: typeGen,
                        accessor: nodeAccessor,
                        itemAccessor: retAccessor,
                        errorMaskAccessor: errorMaskAccessor,
                        translationMaskAccessor: translationMaskAccessor);
                    if (get.Failed) continue;
                    args.Add(get.Value);
                }
            }
        }
    }
}
