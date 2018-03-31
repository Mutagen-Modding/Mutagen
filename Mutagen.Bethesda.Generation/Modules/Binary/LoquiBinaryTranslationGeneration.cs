using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class LoquiBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public string ModNickname;

        public LoquiBinaryTranslationGeneration(string modNickname)
        {
            this.ModNickname = modNickname;
        }

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            string writerAccessor,
            Accessor itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
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
            using (new BraceWrapper(fg, doIt: isGroup))
            {
                using (var args = new ArgsWrapper(fg,
                $"LoquiBinaryTranslation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}, {loquiGen.Mask(MaskType.Error)}>.Instance.Write"))
                {
                    args.Add($"writer: {writerAccessor}");
                    args.Add($"item: {itemAccessor.PropertyOrDirectAccess}");
                    if (loquiGen.HasIndex)
                    {
                        args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                        args.Add($"errorMask: {maskAccessor}");
                    }
                    else
                    {
                        args.Add($"doMasks: {doMaskAccessor}");
                        args.Add($"errorMask: out {maskAccessor}");
                    }
                    if (data?.RecordTypeConverter != null
                        && data.RecordTypeConverter.FromConversions.Count > 0)
                    {
                        args.Add($"recordTypeConverter: {objGen.RegistrationName}.{typeGen.Name}Converter");
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
            string frameAccessor,
            Accessor itemAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TargetObjectGeneration != null)
            {
                if (loquiGen.TryGetFieldData(out var data)
                    && data.MarkerType.HasValue)
                {
                    fg.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH + contentLength; // Skip marker");
                }

                if (loquiGen.SingletonType == SingletonLevel.Singleton)
                {
                    if (loquiGen.InterfaceType == LoquiInterfaceType.IGetter) return;
                    using (var args = new ArgsWrapper(fg,
                        $"var tmp{typeGen.Name} = {loquiGen.TypeName}.Create_{ModNickname}"))
                    {
                        args.Add($"frame: {frameAccessor}");
                        args.Add($"doMasks: errorMask != null");
                        args.Add($"errorMask: out {loquiGen.Mask(MaskType.Error)} {loquiGen.Name}createMask");
                    }
                    using (var args = new ArgsWrapper(fg,
                        $"{itemAccessor.DirectAccess}.CopyFieldsFrom{loquiGen.GetGenericTypes(MaskType.Error, MaskType.Copy)}"))
                    {
                        args.Add($"rhs: tmp{typeGen.Name}");
                        args.Add("def: null");
                        args.Add("cmds: null");
                        args.Add("copyMask: null");
                        args.Add("doMasks: errorMask != null");
                        args.Add($"errorMask: out {loquiGen.Mask(MaskType.Error)} {loquiGen.Name}errorMask");
                    }
                    fg.AppendLine($"var combined{typeGen.Name} = {loquiGen.Mask(MaskType.Error)}.Combine({loquiGen.Name}createMask, {loquiGen.Name}errorMask);");
                    using (var args = new ArgsWrapper(fg,
                        $"ErrorMask.HandleErrorMask"))
                    {
                        args.Add($"creator: {maskAccessor}");
                        args.Add($"index: (int){typeGen.IndexEnumName}");
                        args.Add($"errMaskObj: combined{typeGen.Name} == null ? null : new MaskItem<Exception, {loquiGen.Mask(MaskType.Error)}>(null, combined{typeGen.Name})");
                    }
                }
                else
                {
                    ArgsWrapper args;
                    if (itemAccessor.PropertyAccess != null)
                    {
                        args = new ArgsWrapper(fg, $"{itemAccessor.PropertyAccess}.{nameof(INotifyingCollectionExt.SetIfSucceeded)}(LoquiBinaryTranslation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}, {loquiGen.Mask(MaskType.Error)}>.Instance.Parse",
                            suffixLine: ")");
                    }
                    else
                    {
                        args = new ArgsWrapper(fg, $"var {typeGen.Name}tryGet = LoquiBinaryTranslation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}, {loquiGen.Mask(MaskType.Error)}>.Instance.Parse");
                    }
                    using (args)
                    {
                        args.Add($"frame: {frameAccessor}{(loquiGen.TargetObjectGeneration.HasRecordType() ? null : ".Spawn(snapToFinalPosition: false)")}");
                        if (loquiGen.HasIndex)
                        {
                            args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                            args.Add($"errorMask: {maskAccessor}");
                        }
                        else
                        {
                            args.Add($"doMasks: {doMaskAccessor}");
                            args.Add($"errorMask: out {maskAccessor}");
                        }
                        if (data?.RecordTypeConverter != null
                            && data.RecordTypeConverter.FromConversions.Count > 0)
                        {
                            args.Add($"recordTypeConverter: {objGen.RegistrationName}.{typeGen.Name}Converter");
                        }
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
            }
            else
            {
                UnsafeXmlTranslationGeneration unsafeXml = new UnsafeXmlTranslationGeneration()
                {
                    ErrMaskString = $"MaskItem<Exception, {loquiGen.Mask(MaskType.Error)}>"
                };
                unsafeXml.GenerateCopyIn(
                    fg: fg,
                    typeGen: typeGen,
                    nodeAccessor: frameAccessor,
                    itemAccessor: itemAccessor,
                    doMaskAccessor: doMaskAccessor,
                    maskAccessor: "var unsafeMask");
                fg.AppendLine($"{maskAccessor} = unsafeMask == null ? null : new MaskItem<Exception, object>(null, unsafeMask);");
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            string readerAccessor,
            Accessor retAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var targetLoquiGen = targetGen as LoquiType;
            var loquiGen = typeGen as LoquiType;
            var data = loquiGen.GetFieldData();
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor.DirectAccess}LoquiBinaryTranslation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}, {loquiGen.Mask(MaskType.Error)}>.Instance.Parse",
                suffixLine: (targetGen == typeGen ? string.Empty : $".Bubble<{typeGen.TypeName}, {targetGen.TypeName}>()")))
            {
                args.Add($"frame: {readerAccessor}.Spawn(snapToFinalPosition: false)");
                if (loquiGen.HasIndex)
                {
                    args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                    args.Add($"errorMask: {maskAccessor}");
                }
                else
                {
                    args.Add($"doMasks: {doMaskAccessor}");
                    args.Add($"errorMask: out {maskAccessor}");
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
