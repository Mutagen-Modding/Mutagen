using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class FormIDLinkBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<FormKey>
    {
        public FormIDLinkBinaryTranslationGeneration()
            : base(expectedLen: 4)
        {
            this.AdditionalWriteParams.Add(AdditionalParam);
            this.AdditionalCopyInParams.Add(AdditionalParam);
            this.AdditionalCopyInRetParams.Add(AdditionalParam);
            this.PreferDirectTranslation = false;
        }

        protected override string ItemWriteAccess(Accessor itemAccessor)
        {
            return itemAccessor.PropertyOrDirectAccess;
        }

        public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            return false;
        }

        public override string Typename(TypeGeneration typeGen)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    return "FormLink";
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    return "RecordType";
                default:
                    throw new NotImplementedException();
            }
        }

        private static TryGet<string> AdditionalParam(
           ObjectGeneration objGen,
           TypeGeneration typeGen)
        {
            return TryGet<string>.Succeed("masterReferences: masterReferences");
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
            Accessor translationAccessor)
        {
            if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                if (asyncMode == AsyncMode.Direct) throw new NotImplementedException();
                fg.AppendLine("r.Position += Mutagen.Bethesda.Constants.SUBRECORD_LENGTH;");
            }
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    using (var args = new ArgsWrapper(fg,
                        $"{retAccessor}{this.Namespace}{this.Typename(typeGen)}BinaryTranslation.Instance.Parse"))
                    {
                        args.Add(nodeAccessor.DirectAccess);
                        if (this.DoErrorMasks)
                        {
                            args.Add($"errorMask: {errorMaskAccessor}");
                        }
                        args.Add($"item: out {outItemAccessor.DirectAccess}");
                        foreach (var writeParam in this.AdditionalCopyInRetParams)
                        {
                            var get = writeParam(
                                objGen: objGen,
                                typeGen: typeGen);
                            if (get.Failed) continue;
                            args.Add(get.Value);
                        }
                    }
                    break;
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    fg.AppendLine($"{errorMaskAccessor} = null;");
                    fg.AppendLine($"{outItemAccessor.DirectAccess} = new {linkType.TypeName(getter: false)}(HeaderTranslation.ReadNextRecordType(r.Reader));");
                    fg.AppendLine($"return true;");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void GenerateCopyIn(
            FileGeneration fg, 
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor frameAccessor, 
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            if (typeGen.TryGetFieldData(out var data)
                && data.RecordType.HasValue)
            {
                fg.AppendLine($"{frameAccessor}.Position += Mutagen.Bethesda.Constants.SUBRECORD_LENGTH;");
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
                    ExtraArgs = $"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : "")}"
                        .Single()
                        .AndWhen("masterReferences: masterReferences", () => linkType.FormIDType == FormIDLinkType.FormIDTypeEnum.Normal)
                        .ToArray(),
                    SkipErrorMask = !this.DoErrorMasks,
                });
        }

        public override void GenerateWrite(
            FileGeneration fg, 
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor writerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            switch (linkType.FormIDType)
            {
                case FormIDLinkType.FormIDTypeEnum.Normal:
                    base.GenerateWrite(fg, objGen, typeGen, writerAccessor, itemAccessor, errorMaskAccessor,
                        translationMaskAccessor: translationMaskAccessor);
                    break;
                case FormIDLinkType.FormIDTypeEnum.EDIDChars:
                    var data = typeGen.CustomData[Constants.DATA_KEY] as MutagenFieldData;
                    using (var args = new ArgsWrapper(fg,
                        $"{this.Namespace}RecordTypeBinaryTranslation.Instance.Write"))
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
                            args.Add($"header: recordTypeConverter.Convert({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                            args.Add($"nullable: {(data.Optional ? "true" : "false")}");
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        protected override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor)
        {
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            return $"new FormIDLink<{linkType.LoquiType.TypeName(getter: true)}>(FormKey.Factory(_package.MasterReferences, BinaryPrimitives.ReadUInt32LittleEndian({dataAccessor})))";
        }

        public override void GenerateWrapperFields(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor, int currentPosition, DataType dataType = null)
        {
            var data = typeGen.GetFieldData();
            if (data.HasTrigger)
            {
                fg.AppendLine($"private int? _{typeGen.Name}Location;");
                fg.AppendLine($"public bool {typeGen.Name}_IsSet => _{typeGen.Name}Location.HasValue;");
            }
            FormIDLinkType linkType = typeGen as FormIDLinkType;
            
            if (data.RecordType.HasValue)
            {
                if (dataType != null)
                {
                    throw new ArgumentException();
                }
                dataAccessor = $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordSpan)}({dataAccessor}, _{typeGen.Name}Location.Value, _package.Meta)";
                fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Property} => _{typeGen.Name}Location.HasValue ? {GenerateForTypicalWrapper(objGen, typeGen, dataAccessor)} : default;");
            }
            else
            {
                if (this.ExpectedLength == null)
                {
                    throw new NotImplementedException();
                }
                if (dataType == null)
                {
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Property} => {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Span.Slice({currentPosition}, {this.ExpectedLength.Value})")};");
                }
                else
                {
                    DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, currentPosition);
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)} {typeGen.Property} => _{typeGen.Name}_IsSet ? {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Span.Slice(_{typeGen.Name}Location, {this.ExpectedLength.Value})")} : default;");
                }
            }
            fg.AppendLine($"public {linkType.LoquiType.TypeName(getter: true)} {linkType.Name} => default;");
        }
    }
}
