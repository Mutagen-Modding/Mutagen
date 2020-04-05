using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
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

        public override void GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor readerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor, 
            Accessor translationAccessor)
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
            else if (data.MarkerType.HasValue && !gender.MarkerPerGender)
            {
                fg.AppendLine($"{readerAccessor}.Position += {readerAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)} + contentLength; // Skip marker");
            }

            using (var args = new ArgsWrapper(fg,
                $"{itemAccessor} = {this.Namespace}GenderedItemBinaryTranslation.Parse{(gender.MarkerPerGender ? "MarkerPerItem" : null)}<{gender.SubTypeGeneration.TypeName(getter: false)}>"))
            {
                args.AddPassArg($"frame");
                if (gender.MaleMarker.HasValue)
                {
                    args.Add($"maleMarker: {objGen.RecordTypeHeaderName(gender.MaleMarker.Value)}");
                    args.Add($"femaleMarker: {objGen.RecordTypeHeaderName(gender.FemaleMarker.Value)}");
                }
                if (data.MarkerType.HasValue && gender.MarkerPerGender)
                {
                    args.Add($"marker: {objGen.RecordTypeHeaderName(data.MarkerType.Value)}");
                }
                var subData = gender.SubTypeGeneration.GetFieldData();
                if (subData.RecordType.HasValue
                    && !(gender.SubTypeGeneration is LoquiType))
                {
                    args.Add($"contentMarker: {objGen.RecordTypeHeaderName(subData.RecordType.Value)}");
                }
                LoquiType loqui = gender.SubTypeGeneration as LoquiType;
                if (loqui != null)
                {
                    if (subData?.RecordTypeConverter != null
                        && subData.RecordTypeConverter.FromConversions.Count > 0)
                    {
                        args.Add($"recordTypeConverter: {objGen.RegistrationName}.{typeGen.Name}Converter");
                    }
                }
                if (gender.FemaleConversions != null)
                {
                    args.Add($"femaleRecordConverter: {objGen.RegistrationName}.{typeGen.Name}FemaleConverter");
                }
                if (loqui != null
                    && !loqui.CanStronglyType)
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(gender.SubTypeGeneration, getter: false)}.Parse<{loqui.TypeName(getter: false)}>");
                }
                else
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(gender.SubTypeGeneration, getter: false)}.Parse");
                    if (gender.ItemHasBeenSet && loqui == null)
                    {
                        args.Add($"skipMarker: false");
                    }
                }
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg, 
            ObjectGeneration objGen,
            TypeGeneration targetGen, 
            TypeGeneration typeGen,
            Accessor readerAccessor, 
            AsyncMode asyncMode,
            Accessor retAccessor, 
            Accessor outItemAccessor, 
            Accessor errorMaskAccessor,
            Accessor translationAccessor,
            Accessor converterAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrite(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor writerAccessor, 
            Accessor itemAccessor, 
            Accessor errorMaskAccessor,
            Accessor translationAccessor,
            Accessor converterAccessor)
        {
            GenderedType gendered = typeGen as GenderedType;
            var gen = this.Module.GetTypeGeneration(gendered.SubTypeGeneration.GetType());
            var data = typeGen.GetFieldData();
            if (!this.Module.TryGetTypeGeneration(gendered.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + gendered.SubTypeGeneration);
            }
            var allowDirectWrite = subTransl.AllowDirectWrite(objGen, gendered.SubTypeGeneration);
            bool needsMasters = gendered.SubTypeGeneration is FormLinkType || gendered.SubTypeGeneration is LoquiType;
            var typeName = gendered.SubTypeGeneration.TypeName(getter: true);
            var loqui = gendered.SubTypeGeneration as LoquiType;
            if (loqui != null)
            {
                typeName = loqui.TypeName(getter: true, internalInterface: true);
            }
            using (var args = new ArgsWrapper(fg,
                $"GenderedItemBinaryTranslation.Write{(gendered.MarkerPerGender ? "MarkerPerItem" : null)}"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor}");
                if (data.RecordType.HasValue)
                {
                    args.Add($"recordType: {objGen.RecordTypeHeaderName(data.RecordType.Value)}");
                }
                else if (data.MarkerType.HasValue)
                {
                    args.Add($"markerType: {objGen.RecordTypeHeaderName(data.MarkerType.Value)}");
                }
                if (gendered.MaleMarker.HasValue)
                {
                    args.Add($"maleMarker: {objGen.RecordTypeHeaderName(gendered.MaleMarker.Value)}");
                }
                if (gendered.FemaleMarker.HasValue)
                {
                    args.Add($"femaleMarker: {objGen.RecordTypeHeaderName(gendered.FemaleMarker.Value)}");
                }
                if (gendered.MaleMarker.HasValue
                    && loqui != null)
                {
                    args.Add("markerWrap: false");
                }
                if (gendered.FemaleConversions != null)
                {
                    args.Add($"femaleRecordConverter: {objGen.RegistrationName}.{typeGen.Name}FemaleConverter");
                }
                if (allowDirectWrite)
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(gendered.SubTypeGeneration, getter: true)}.Write{(gendered.SubTypeGeneration.HasBeenSet ? "Nullable" : string.Empty)}");
                }
                else
                {
                    args.Add((gen) =>
                    {
                        var listTranslMask = this.MaskModule.GetMaskModule(gendered.SubTypeGeneration.GetType()).GetTranslationMaskTypeStr(gendered.SubTypeGeneration);
                        gen.AppendLine($"transl: (MutagenWriter subWriter, {typeName}{(gendered.SubTypeGeneration.HasBeenSet ? "?" : null)} subItem{(needsMasters ? $", {nameof(RecordTypeConverter)}? conv" : null)}) =>");
                        using (new BraceWrapper(gen))
                        {
                            subTransl.GenerateWrite(
                                fg: gen,
                                objGen: objGen,
                                typeGen: gendered.SubTypeGeneration,
                                writerAccessor: "subWriter",
                                translationAccessor: "subTranslMask",
                                itemAccessor: new Accessor($"subItem"),
                                errorMaskAccessor: null,
                                converterAccessor: needsMasters ? "conv" : null);
                        }
                    });
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
                                if (typeGen.HasBeenSet)
                                {
                                    fg.AppendLine($"if (!_{typeGen.Name}Location.HasValue) return {typeGen.GetDefault()};");
                                }
                                fg.AppendLine($"var data = {dataAccessor}.Span.Slice({currentPosition}, {subLen * 2});");
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
                    fg.AppendLine($"private IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true)}{(gendered.SubTypeGeneration.HasBeenSet ? "?" : null)}>? _{typeGen.Name}Overlay;");
                }
                fg.AppendLine($"public IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true)}{(gendered.SubTypeGeneration.HasBeenSet ? "?" : null)}>? {typeGen.Name} => _{typeGen.Name}Overlay;");
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
            bool isLoqui = gendered.SubTypeGeneration is LoquiType;
            if (typeGen.GetFieldData().MarkerType.HasValue && !gendered.MarkerPerGender)
            {
                fg.AppendLine($"stream.Position += _package.Meta.SubConstants.HeaderLength; // Skip marker");
            }
            switch (typeGen.GetFieldData().BinaryOverlayFallback)
            {
                case BinaryGenerationType.Normal:
                    if (gendered.ItemHasBeenSet)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"_{typeGen.Name}Overlay = GenderedItemBinaryOverlay.{(isLoqui ? "FactorySkipMarkersPreRead" : "Factory")}<{gendered.SubTypeGeneration.TypeName(getter: true)}>"))
                        {
                            args.Add("package: _package");
                            args.Add($"male: {objGen.RecordTypeHeaderName(gendered.MaleMarker.Value)}");
                            args.Add($"female: {objGen.RecordTypeHeaderName(gendered.FemaleMarker.Value)}");
                            if (gendered.MarkerPerGender)
                            {
                                args.Add($"marker: {objGen.RecordTypeHeaderName(typeGen.GetFieldData().MarkerType.Value)}");
                            }
                            if (gendered.SubTypeGeneration is LoquiType loqui)
                            {
                                args.AddPassArg("stream");
                                args.Add($"creator: (s, p, r) => {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(s, p, r)");
                                var subData = loqui.GetFieldData();
                                if (subData?.RecordTypeConverter != null
                                    && subData.RecordTypeConverter.FromConversions.Count > 0)
                                {
                                    args.Add($"recordTypeConverter: {objGen.RegistrationName}.{(typeGen.Name ?? typeGen.Parent?.Name)}Converter");
                                }
                                else if (converterAccessor != null)
                                {
                                    args.Add($"recordTypeConverter: {converterAccessor}");
                                }
                            }
                            else
                            {
                                args.AddPassArg("stream");
                                this.Module.TryGetTypeGeneration(gendered.SubTypeGeneration.GetType(), out var subGen);
                                args.Add($"creator: (m, p) => {subGen.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordMemory)}(m, p.Meta)", "p")}");
                            }
                            if (gendered.FemaleConversions != null)
                            {
                                args.Add($"femaleRecordConverter: {objGen.RegistrationName}.{typeGen.Name}FemaleConverter");
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
