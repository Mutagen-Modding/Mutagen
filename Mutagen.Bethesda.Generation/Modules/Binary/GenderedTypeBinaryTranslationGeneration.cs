using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class GenderedTypeBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            GenderedType gender = typeGen as GenderedType;

            if (!this.Module.TryGetTypeGeneration(gender.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + gender.SubTypeGeneration);
            }

            var expected = subTransl.ExpectedLength(objGen, gender.SubTypeGeneration);
            if (expected == null) return null;
            return expected.Value * 2;
        }

        public override void GenerateCopyIn(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor readerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
        {
            GenderedType gender = typeGen as GenderedType;
            var data = typeGen.GetFieldData();

            if (!this.Module.TryGetTypeGeneration(gender.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + gender.SubTypeGeneration);
            }

            if (data.RecordType.HasValue)
            {
                fg.AppendLine($"{readerAccessor}.Position += {readerAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
            }
            else if (data.MarkerType.HasValue)
            {
                fg.AppendLine($"{readerAccessor}.Position += {readerAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)} + contentLength; // Skip marker");
            }

            using (var args = new ArgsWrapper(fg,
                $"{itemAccessor} = {this.Namespace}GenderedItemBinaryTranslation.Parse<{gender.SubTypeGeneration.TypeName(getter: false)}>"))
            {
                args.AddPassArg($"frame");
                if (gender.SubTypeGeneration is FormLinkType
                    || gender.SubTypeGeneration is LoquiType)
                {
                    args.AddPassArg("masterReferences");
                }
                if (gender.MaleMarker.HasValue)
                {
                    args.Add($"maleMarker: {objGen.RecordTypeHeaderName(gender.MaleMarker.Value)}");
                    args.Add($"femaleMarker: {objGen.RecordTypeHeaderName(gender.FemaleMarker.Value)}");
                }
                if (gender.SubTypeGeneration is LoquiType loqui
                    && !loqui.CanStronglyType)
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(gender.SubTypeGeneration, getter: false)}.Parse<{loqui.TypeName(getter: false)}>");
                }
                else
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(gender.SubTypeGeneration, getter: false)}.Parse");
                }
            }
        }

        public override void GenerateCopyInRet(FileGeneration fg, ObjectGeneration objGen, TypeGeneration targetGen, TypeGeneration typeGen, Accessor readerAccessor, AsyncMode asyncMode, Accessor retAccessor, Accessor outItemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrite(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
        {
            GenderedType gendered = typeGen as GenderedType;
            var gen = this.Module.GetTypeGeneration(gendered.SubTypeGeneration.GetType());
            var data = typeGen.GetFieldData();
            if (typeGen.HasBeenSet)
            {
                fg.AppendLine($"if ({itemAccessor}.TryGet(out var {typeGen.Name}item))");
                itemAccessor = $"{typeGen.Name}item";
            }
            using (new BraceWrapper(fg, doIt: typeGen.HasBeenSet))
            {
                if (data.RecordType.HasValue)
                {
                    fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.ExportSubRecordHeader)}({writerAccessor}, recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})))");
                }
                else if (data.MarkerType.HasValue)
                {
                    fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.ExportSubRecordHeader)}({writerAccessor}, recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.MarkerType.Value)})))");
                    using (new BraceWrapper(fg))
                    {
                    }
                }
                using (new BraceWrapper(fg, doIt: data.RecordType.HasValue))
                {
                    if (gendered.MaleMarker.HasValue)
                    {
                        fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.ExportSubRecordHeader)}({writerAccessor}, recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(gendered.MaleMarker.Value)})))");
                        using (new BraceWrapper(fg))
                        {
                        }
                    }
                    using (new BraceWrapper(fg))
                    {
                        gen.GenerateWrite(fg, objGen, gendered.SubTypeGeneration, writerAccessor, $"{itemAccessor}.Male", errorMaskAccessor, translationAccessor);
                    }
                    if (gendered.FemaleMarker.HasValue)
                    {
                        fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.ExportSubRecordHeader)}({writerAccessor}, recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(gendered.FemaleMarker.Value)})))");
                        using (new BraceWrapper(fg))
                        {
                        }
                    }
                    using (new BraceWrapper(fg))
                    {
                        gen.GenerateWrite(fg, objGen, gendered.SubTypeGeneration, writerAccessor, $"{itemAccessor}.Female", errorMaskAccessor, translationAccessor);
                    }
                }
            }
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor dataAccessor,
            int? currentPosition, 
            DataType dataType = null)
        {
            var data = typeGen.GetFieldData();
            switch (data.BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    break;
                case BinaryGenerationType.DoNothing:
                case BinaryGenerationType.NoGeneration:
                    return;
                case BinaryGenerationType.Custom:
                    this.Module.CustomLogic.GenerateForCustomFlagWrapperFields(
                        fg,
                        objGen,
                        typeGen,
                        dataAccessor,
                        ref currentPosition,
                        dataType);
                    return;
                default:
                    throw new NotImplementedException();
            }

            var gendered = typeGen as GenderedType;
            this.Module.TryGetTypeGeneration(gendered.SubTypeGeneration.GetType(), out var subBin);

            if (typeGen.HasBeenSet
                && !gendered.ItemHasBeenSet)
            {
                var subLen = subBin.ExpectedLength(objGen, gendered.SubTypeGeneration).Value;
                if (typeGen.HasBeenSet)
                {
                    fg.AppendLine($"private int? _{typeGen.Name}Location;");
                }
                fg.AppendLine($"public IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true)}>? {typeGen.Name}");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("get");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"if (!_{typeGen.Name}Location.HasValue) return {typeGen.GetDefault()};");
                        fg.AppendLine($"var data = HeaderTranslation.ExtractSubrecordMemory(_data, _{typeGen.Name}Location.Value, _package.Meta);");
                        using (var args = new ArgsWrapper(fg,
                            $"return new GenderedItem<{gendered.SubTypeGeneration.TypeName(getter: true)}>"))
                        {
                            args.Add($"{subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, "data", "_package")}");
                            args.Add($"{subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, $"data.Slice({subLen})", "_package")}");
                        }
                    }
                }
            }
            else if (!typeGen.HasBeenSet
                && !gendered.ItemHasBeenSet)
            {
                var subLen = subBin.ExpectedLength(objGen, gendered.SubTypeGeneration).Value;
                if (dataType == null)
                {
                    if (typeGen.HasBeenSet)
                    {
                        throw new NotImplementedException();
                        //fg.AppendLine($"public {typeGen.TypeName(getter: true)}? {typeGen.Name} => {dataAccessor}.Length >= {(currentPosition + this.ExpectedLength(objGen, typeGen).Value)} ? {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Span.Slice({currentPosition}, {this.ExpectedLength(objGen, typeGen).Value})", "_package")} : {typeGen.GetDefault()};");
                    }
                    else
                    {
                        fg.AppendLine($"public IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true)}> {typeGen.Name}");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("get");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"if (!_{typeGen.Name}Location.HasValue) return {typeGen.GetDefault()};");
                                fg.AppendLine($"var data = {dataAccessor}.Span.Slice({currentPosition}, {subLen});");
                                using (var args = new ArgsWrapper(fg,
                                    $"return new GenderedItem<{gendered.SubTypeGeneration.TypeName(getter: true)}>"))
                                {
                                    args.Add($"{subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, "data", "_package")}");
                                    args.Add($"{subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, $"data.Slice({subLen})", "_package")}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(fg, dataType, objGen, typeGen, currentPosition);
                    fg.AppendLine($"public IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true)}>{(typeGen.HasBeenSet ? "?" : null)} {typeGen.Name}");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("get");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"if (!_{typeGen.Name}_IsSet) return new GenderedItem<{gendered.SubTypeGeneration.TypeName(getter: true)}>({gendered.SubTypeGeneration.GetDefault()}, {gendered.SubTypeGeneration.GetDefault()});");
                            fg.AppendLine($"var data = {dataAccessor}.Span.Slice(_{typeGen.Name}Location);");
                            using (var args = new ArgsWrapper(fg,
                                $"return new GenderedItem<{gendered.SubTypeGeneration.TypeName(getter: true)}>"))
                            {
                                args.Add($"{subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, "data", "_package")}");
                                args.Add($"{subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, $"data.Slice({subLen})", "_package")}");
                            }
                        }
                    }
                }
            }
            else
            {
                if (typeGen.HasBeenSet)
                {
                    fg.AppendLine($"private GenderedItemBinaryOverlay<{gendered.SubTypeGeneration.TypeName(getter: true)}>? _{typeGen.Name}Overlay;");
                }
                fg.AppendLine($"public IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true)}>? {typeGen.Name} => _{typeGen.Name}Overlay;");
            }
        }

        public override async Task GenerateWrapperRecordTypeParse(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor locationAccessor, 
            Accessor packageAccessor,
            Accessor converterAccessor)
        {
            var gendered = typeGen as GenderedType;
            switch (typeGen.GetFieldData().BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    if (gendered.ItemHasBeenSet)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"_{typeGen.Name}Overlay = new GenderedItemBinaryOverlay<{gendered.SubTypeGeneration.TypeName(getter: true)}>"))
                        {
                            args.Add("bytes: _data.Slice(stream.Position - offset)");
                            args.Add("package: _package");
                            args.Add($"male: {objGen.RecordTypeHeaderName(gendered.MaleMarker.Value)}");
                            args.Add($"female: {objGen.RecordTypeHeaderName(gendered.FemaleMarker.Value)}");
                            if (gendered.SubTypeGeneration is LoquiType loqui)
                            {
                                args.Add($"creator: (m, p) => {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(new {nameof(BinaryMemoryReadStream)}(m), p)");
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                    }
                    else
                    {
                        await base.GenerateWrapperRecordTypeParse(fg, objGen, typeGen, locationAccessor, packageAccessor, converterAccessor);
                    }
                    break;
                default:
                    await base.GenerateWrapperRecordTypeParse(fg, objGen, typeGen, locationAccessor, packageAccessor, converterAccessor);
                    break;
            }
        }
    }
}
