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
        public bool PreferDirectTranslation = true;
        public Action<FileGeneration, Accessor, Accessor> customRead;
        public Action<FileGeneration, Accessor, Accessor> customWrite;

        public override string GetTranslatorInstance(TypeGeneration typeGen)
        {
            return $"{Typename(typeGen)}BinaryTranslation.Instance";
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
            if (customWrite != null)
            {
                customWrite(fg, writerAccessor, itemAccessor);
            }
            else if (data.HasTrigger || !PreferDirectTranslation)
            {
                using (var args = new ArgsWrapper(fg,
                    $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Write"))
                {
                    args.Add($"writer: {writerAccessor}");
                    args.Add($"item: {ItemWriteAccess(itemAccessor)}");
                    if (this.DoErrorMasks)
                    {
                        if (typeGen.HasIndex)
                        {
                            args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                        }
                        args.Add($"errorMask: {errorMaskAccessor}");
                    }
                    if (data.RecordType.HasValue)
                    {
                        args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                        args.Add($"nullable: {(data.Optional ? "true" : "false")}");
                    }
                    foreach (var writeParam in this.AdditionalWriteParams)
                    {
                        var get = writeParam(
                            objGen: objGen,
                            typeGen: typeGen);
                        if (get.Failed) continue;
                        args.Add(get.Value);
                    }
                }
            }
            else
            {
                fg.AppendLine($"{writerAccessor.DirectAccess}.Write({itemAccessor.DirectAccess});");
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
            extraArgs.Add($"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : "")}");
            foreach (var writeParam in this.AdditionalCopyInParams)
            {
                var get = writeParam(
                    objGen: objGen,
                    typeGen: typeGen);
                if (get.Failed) continue;
                extraArgs.Add(get.Value);
            }

            if (customRead != null)
            {
                customRead(fg, frameAccessor, itemAccessor);
            }
            else if (PreferDirectTranslation)
            {
                fg.AppendLine($"{itemAccessor.DirectAccess} = {frameAccessor.DirectAccess}.Read{typeName}();");
            }
            else
            {
                TranslationGeneration.WrapParseCall(
                    new TranslationWrapParseArgs()
                    {
                        FG = fg,
                        TypeGen = typeGen,
                        TranslatorLine = $"{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance",
                        MaskAccessor = errorMaskAccessor,
                        ItemAccessor = itemAccessor,
                        TranslationMaskAccessor = null,
                        AsyncMode = AsyncMode.Off,
                        IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                        ExtraArgs = extraArgs.ToArray(),
                        SkipErrorMask = !this.DoErrorMasks
                    });
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            bool squashedRepeatedList,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine("r.Position += Constants.SUBRECORD_LENGTH;");
            }
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor.DirectAccess);
                if (this.DoErrorMasks)
                {
                    args.Add($"errorMask: {errorMaskAccessor}");
                }
                args.Add($"translationMask: {translationMaskAccessor}");
                foreach (var writeParam in this.AdditionalCopyInRetParams)
                {
                    var get = writeParam(
                        objGen: objGen,
                        typeGen: typeGen);
                    if (get.Failed) continue;
                    args.Add(get.Value);
                }
            }
        }
    }
}
