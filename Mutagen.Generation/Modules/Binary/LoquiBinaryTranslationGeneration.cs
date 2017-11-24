using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
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
            if (loquiGen.TargetObjectGeneration != null)
            {
                if (loquiGen.TryGetFieldData(out var data)
                    && data.MarkerType.HasValue)
                {
                    fg.AppendLine($"using (HeaderExport.ExportHeader(writer, {objGen.RegistrationName}.{data.MarkerType.Value.Type}_HEADER, ObjectType.Subrecord)) {{ }}");
                }
                using (var args = new ArgsWrapper(fg,
                    $"LoquiBinaryTranslation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}, {loquiGen.MaskItemString(MaskType.Error)}>.Instance.Write"))
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
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen)
        {
            var loquiGen = typeGen as LoquiType;
            return loquiGen.SingletonType != LoquiType.SingletonLevel.Singleton || loquiGen.InterfaceType != LoquiInterfaceType.IGetter;
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
                if (loquiGen.SingletonType == LoquiType.SingletonLevel.Singleton)
                {
                    if (loquiGen.InterfaceType == LoquiInterfaceType.IGetter) return;
                    fg.AppendLine($"{frameAccessor}.Position -= Constants.SUBRECORD_LENGTH;");
                    using (var args = new ArgsWrapper(fg,
                        $"var tmp = {loquiGen.TargetObjectGeneration.Name}.Create_{ModNickname}"))
                    {
                        args.Add($"frame: {frameAccessor}");
                        args.Add($"doMasks: errorMask != null");
                        args.Add($"errorMask: out {loquiGen.MaskItemString(MaskType.Error)} {loquiGen.Name}createMask");
                    }
                    using (var args = new ArgsWrapper(fg,
                        $"{loquiGen.TargetObjectGeneration.ExtCommonName}.CopyFieldsFrom"))
                    {
                        args.Add($"item: {itemAccessor.DirectAccess}");
                        args.Add("rhs: tmp");
                        args.Add("def: null");
                        args.Add("cmds: null");
                        args.Add("copyMask: null");
                        args.Add("doMasks: errorMask != null");
                        args.Add($"errorMask: out var {loquiGen.Name}errorMask");
                    }
                    using (var args = new ArgsWrapper(fg,
                        $"ErrorMask.HandleErrorMask"))
                    {
                        args.Add($"creator: {maskAccessor}");
                        args.Add($"index: (int){typeGen.IndexEnumName}");
                        args.Add($"errMaskObj: {loquiGen.MaskItemString(MaskType.Error)}.Combine({loquiGen.Name}createMask, {loquiGen.Name}errorMask)");
                    }
                }
                else
                {
                    if (loquiGen.TryGetFieldData(out var data)
                        && data.MarkerType.HasValue)
                    {
                        fg.AppendLine("frame.Position += Constants.SUBRECORD_LENGTH + contentLength; // Skip marker");
                    }

                    ArgsWrapper args;
                    if (itemAccessor.PropertyAccess != null)
                    {
                        args = new ArgsWrapper(fg, $"{itemAccessor.PropertyAccess}.{nameof(INotifyingCollectionExt.SetIfSucceeded)}(LoquiBinaryTranslation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}, {loquiGen.MaskItemString(MaskType.Error)}>.Instance.Parse",
                            suffixLine: ")");
                    }
                    else
                    {
                        args = new ArgsWrapper(fg, $"var {typeGen.Name}tryGet = LoquiBinaryTranslation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}, {loquiGen.MaskItemString(MaskType.Error)}>.Instance.Parse");
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
                    ErrMaskString = $"MaskItem<Exception, {loquiGen.MaskItemString(MaskType.Error)}>"
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
            TypeGeneration typeGen,
            string readerAccessor,
            string retAccessor,
            string doMaskAccessor,
            string maskAccessor)
        {
            var loquiGen = typeGen as LoquiType;
            if (loquiGen.TargetObjectGeneration != null)
            {
                using (var args = new ArgsWrapper(fg,
                    $"{retAccessor}LoquiBinaryTranslation<{loquiGen.ObjectTypeName}{loquiGen.GenericTypes}, {loquiGen.MaskItemString(MaskType.Error)}>.Instance.Parse"))
                {
                    args.Add($"frame: {readerAccessor}{(loquiGen.TargetObjectGeneration.GetObjectType() == ObjectType.Subrecord ? ".Spawn(snapToFinalPosition: false)" : null)}");
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
                }
            }
            else
            {
                UnsafeXmlTranslationGeneration unsafeXml = new UnsafeXmlTranslationGeneration()
                {
                    ErrMaskString = $"MaskItem<Exception, {loquiGen.MaskItemString(MaskType.Error)}>"
                };
                unsafeXml.GenerateCopyInRet(
                    fg: fg,
                    typeGen: typeGen,
                    nodeAccessor: readerAccessor,
                    retAccessor: retAccessor,
                    doMaskAccessor: doMaskAccessor,
                    maskAccessor: maskAccessor);
            }
        }
    }
}
