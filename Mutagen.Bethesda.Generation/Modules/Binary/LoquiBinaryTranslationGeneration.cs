using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class LoquiBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public const string AsyncOverrideKey = "AsyncOverride";

        public string ModNickname;
        public override bool DoErrorMasks => true;
        public override bool IsAsync(TypeGeneration gen, bool read)
        {
            if (!read) return false;
            LoquiType loqui = gen as LoquiType;
            if (loqui.CustomData.TryGetValue(AsyncOverrideKey, out var asyncOverride))
            {
                return (bool)asyncOverride;
            }
            if (loqui.TargetObjectGeneration != null)
            {
                if (loqui.TargetObjectGeneration.GetObjectData().CustomBinaryEnd == CustomEnd.Async) return true;
                return this.Module.HasAsync(loqui.TargetObjectGeneration, self: true);
            }
            return false;
        }

        public override bool AllowDirectParse(ObjectGeneration objGen, TypeGeneration typeGen, bool squashedRepeatedList)
        {
            return false;
        }

        public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            return false;
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen)
        {
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.CanStronglyType)
            {
                return $"LoquiBinaryTranslation<{loquiGen.TypeName}>.Instance";
            }
            else
            {
                return $"LoquiBinaryTranslation.Instance";
            }
        }

        public LoquiBinaryTranslationGeneration(string modNickname)
        {
            this.ModNickname = modNickname;
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
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TryGetFieldData(out var data)
                && data.MarkerType.HasValue)
            {
                fg.AppendLine($"using (HeaderExport.ExportHeader(writer, {objGen.RegistrationName}.{data.MarkerType.Value.Type}_HEADER, ObjectType.Subrecord)) {{ }}");
            }
            bool isGroup = objGen.GetObjectType() == ObjectType.Mod
                && loquiGen.TargetObjectGeneration.GetObjectData().ObjectType == ObjectType.Group;
            if (isGroup)
            {
                fg.AppendLine($"if ({itemAccessor.PropertyOrDirectAccess}.Items.Count > 0)");
            }
            string line;
            if (loquiGen.TargetObjectGeneration != null)
            {
                line = $"(({this.Module.TranslationClassName(loquiGen.TargetObjectGeneration)}{loquiGen.GenericTypes})(({nameof(IBinaryItem)}){itemAccessor.DirectAccess}).{this.Module.ModuleNickname}Translator)";
            }
            else
            {
                line = $"(({nameof(IBinaryItem)}){itemAccessor.DirectAccess}).{this.Module.ModuleNickname}Translator";
            }
            using (new BraceWrapper(fg, doIt: isGroup))
            {
                using (var args = new ArgsWrapper(fg, $"{line}.Write"))
                {
                    args.Add($"item: {itemAccessor.DirectAccess}");
                    args.Add($"writer: {writerAccessor}");
                    args.Add($"errorMask: {errorMaskAccessor}");
                    args.Add($"masterReferences: masterReferences");
                    if (data?.RecordTypeConverter != null
                        && data.RecordTypeConverter.FromConversions.Count > 0)
                    {
                        args.Add($"recordTypeConverter: {objGen.RegistrationName}.{typeGen.Name}Converter");
                    }
                    else
                    {
                        args.Add($"recordTypeConverter: null");
                    }
                }
            }
        }

        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen)
        {
            var loquiGen = typeGen as LoquiType;
            return loquiGen.SingletonType != SingletonLevel.Singleton || loquiGen.InterfaceType != LoquiInterfaceType.IGetter;
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
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TargetObjectGeneration != null)
            {
                if (loquiGen.TryGetFieldData(out var data)
                    && data.MarkerType.HasValue)
                {
                    fg.AppendLine("frame.Position += Mutagen.Bethesda.Constants.SUBRECORD_LENGTH + contentLength; // Skip marker");
                }

                if (loquiGen.InterfaceType == LoquiInterfaceType.IGetter) return;
                MaskGenerationUtility.WrapErrorFieldIndexPush(fg,
                    () =>
                    {
                        if (loquiGen.SingletonType == SingletonLevel.Singleton)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"var tmp{typeGen.Name} = {Loqui.Generation.Utility.Await(this.IsAsync(typeGen, read: true))}{loquiGen.TypeName}.Create_{ModNickname}"))
                            {
                                args.Add($"frame: {frameAccessor}");
                                args.Add($"errorMask: {errorMaskAccessor}");
                                args.Add($"recordTypeConverter: null");
                                args.Add($"masterReferences: masterReferences");
                            }
                            using (var args = new ArgsWrapper(fg,
                                $"{itemAccessor.DirectAccess}.CopyFieldsFrom{loquiGen.GetGenericTypes(MaskType.Copy)}"))
                            {
                                args.Add($"rhs: tmp{typeGen.Name}");
                                args.Add("def: null");
                                args.Add("copyMask: null");
                                args.Add($"errorMask: {errorMaskAccessor}");
                            }
                        }
                        else
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{itemAccessor.DirectAccess} = {loquiGen.TargetObjectGeneration.Namespace}.{loquiGen.ObjectTypeName}.Create_{this.Module.ModuleNickname}"))
                            {
                                args.Add($"frame: {frameAccessor}");
                                if (data?.RecordTypeConverter != null
                                    && data.RecordTypeConverter.FromConversions.Count > 0)
                                {
                                    args.Add($"recordTypeConverter: {objGen.RegistrationName}.{typeGen.Name}Converter");
                                }
                                else
                                {
                                    args.Add("recordTypeConverter: null");
                                }
                                args.Add($"masterReferences: masterReferences");
                                if (this.DoErrorMasks)
                                {
                                    args.Add("errorMask: errorMask");
                                }
                            }
                        }
                    },
                    errorMaskAccessor: "errorMask",
                    indexAccessor: $"(int){typeGen.IndexEnumName}");
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor readerAccessor,
            bool squashedRepeatedList,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor)
        {
            var targetLoquiGen = targetGen as LoquiType;
            var loquiGen = typeGen as LoquiType;
            var data = loquiGen.GetFieldData();
            asyncMode = this.IsAsync(typeGen, read: true) ? asyncMode : AsyncMode.Off;
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{Loqui.Generation.Utility.Await(asyncMode)}LoquiBinary{(asyncMode == AsyncMode.Off ? null : "Async")}Translation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}>.Instance.Parse",
                suffixLine: Loqui.Generation.Utility.ConfigAwait(asyncMode)))
            {
                args.Add($"frame: {readerAccessor}");
                if (loquiGen.HasIndex)
                {
                    args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                }
                if (asyncMode == AsyncMode.Off)
                {
                    args.Add($"item: out {outItemAccessor.DirectAccess}");
                }
                args.Add($"errorMask: {errorMaskAccessor}");
                if (objGen.GetObjectType() == ObjectType.Mod)
                {
                    args.Add($"masterReferences: item.TES4.MasterReferences");
                }
                else
                {
                    args.Add($"masterReferences: masterReferences");
                }
                if (data?.RecordTypeConverter != null
                    && data.RecordTypeConverter.FromConversions.Count > 0)
                {
                    args.Add($"recordTypeConverter: {objGen.RegistrationName}.{typeGen.Name}Converter");
                }
            }
        }
    }
}
