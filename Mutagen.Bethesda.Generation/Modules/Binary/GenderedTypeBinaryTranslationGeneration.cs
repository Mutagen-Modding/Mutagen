using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class GenderedTypeBinaryTranslationGeneration : BinaryTranslationGeneration
{
    public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
    {
        GenderedType gender = typeGen as GenderedType;

        if (!this.Module.TryGetTypeGeneration(gender.SubTypeGeneration.GetType(), out var subTransl))
        {
            throw new ArgumentException("Unsupported type generator: " + gender.SubTypeGeneration);
        }

        var expected = await subTransl.ExpectedLength(objGen, gender.SubTypeGeneration);
        if (expected == null) return null;
        return expected.Value * 2;
    }

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb,
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
            sb.AppendLine($"{readerAccessor}.Position += {readerAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
        }
        else if (data.MarkerType.HasValue && !gender.MarkerPerGender)
        {
            sb.AppendLine($"{readerAccessor}.Position += {readerAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)} + contentLength; // Skip marker");
        }

        bool notNull = gender.ItemNullable && !gender.SubTypeGeneration.IsNullable;

        string parseSuffix = string.Empty;
        if (gender.MarkerPerGender)
        {
            parseSuffix = "MarkerAheadOfItem";
        }
        else if (gender.SubTypeGeneration.GetFieldData().MarkerType.HasValue)
        {
            parseSuffix = "MarkerWithinItem";
        }
        
        using (var args = sb.Call(
                   $"{itemAccessor} = {this.NamespacePrefix}GenderedItemBinaryTranslation.Parse{parseSuffix}<{gender.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)}>"))
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
                    args.Add($"translationParams: {objGen.RegistrationName}.{typeGen.Name}Converter");
                }
            }
            if (gender.FemaleConversions != null)
            {
                args.Add($"femaleRecordConverter: {objGen.RegistrationName}.{typeGen.Name}FemaleConverter");
            }
            if (gender.MaleConversions != null)
            {
                args.Add($"maleRecordConverter: {objGen.RegistrationName}.{typeGen.Name}MaleConverter");
            }

            if (!gender.ShortCircuit)
            {
                args.Add("shortCircuit: false");
            }

            bool needsRecordConv = gender.SubTypeGeneration.NeedsRecordConverter();
            if (subTransl.AllowDirectParse(objGen, gender.SubTypeGeneration, false))
            {
                if (loqui != null)
                {
                    args.Add($"transl: {loqui.ObjectTypeName}{loqui.GenericTypes(getter: false)}.TryCreateFromBinary");
                }
                else
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(gender.SubTypeGeneration, getter: false)}.Parse");
                    if (gender.ItemNullable)
                    {
                        args.Add($"skipMarker: false");
                    }
                }
            }
            else
            {
                args.Add(gen =>
                {
                    gen.AppendLine($"transl: (MutagenFrame r, [MaybeNullWhen(false)] out {gender.SubTypeGeneration.TypeName(getter: false, needsCovariance: true)} genSubItem{(needsRecordConv ? $", {nameof(RecordTypeConverter)}? conv" : null)}) =>");
                    using (gen.CurlyBrace())
                    {
                        subTransl.GenerateCopyInRet(
                            sb: gen,
                            objGen: objGen,
                            targetGen: gender.SubTypeGeneration,
                            typeGen: gender.SubTypeGeneration,
                            readerAccessor: "r",
                            translationAccessor: null,
                            retAccessor: "return ",
                            outItemAccessor: new Accessor("genSubItem"),
                            asyncMode: AsyncMode.Off,
                            errorMaskAccessor: "listErrMask",
                            converterAccessor: "conv",
                            inline: false);
                    }
                    if (gender.ItemNullable)
                    {
                        args.Add($"skipMarker: false");
                    }
                });
            }
            if (notNull)
            {
                args.Add($"fallback: {gender.SubTypeGeneration.GetDefault(getter: false)}");
            }
        }
    }

    public override async Task GenerateCopyInRet(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration targetGen,
        TypeGeneration typeGen,
        Accessor readerAccessor,
        AsyncMode asyncMode,
        Accessor retAccessor,
        Accessor outItemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationAccessor,
        Accessor converterAccessor,
        bool inline)
    {
        throw new NotImplementedException();
    }

    public override async Task GenerateWrite(
        StructuredStringBuilder sb,
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
        var loqui = gendered.SubTypeGeneration as LoquiType; 
        bool needsMasters = gendered.SubTypeGeneration is FormLinkType || (loqui != null && loqui.GetFieldData().HasTrigger); 
        var typeName = gendered.SubTypeGeneration.TypeName(getter: true, needsCovariance: true);
        if (loqui != null)
        {
            typeName = loqui.TypeNameInternal(getter: true, internalInterface: true);
        }

        var generics = gendered.SubTypeGeneration is FormLinkType linkType && linkType.Nullable ? $"<{linkType.GenericString}>" : string.Empty;

        string writeSuffix = string.Empty;
        if (gendered.MarkerPerGender)
        {
            writeSuffix = "MarkerAheadOfItem";
        }
        else if (gendered.SubTypeGeneration.GetFieldData().MarkerType.HasValue)
        {
            writeSuffix = "MarkerWithinItem";
        }
        
        using (var args = sb.Call(
                   $"GenderedItemBinaryTranslation.Write{writeSuffix}{generics}"))
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
            if (gendered.MaleConversions != null)
            {
                args.Add($"maleRecordConverter: {objGen.RegistrationName}.{typeGen.Name}MaleConverter");
            }
            if (allowDirectWrite)
            {
                args.Add($"transl: {subTransl.GetTranslatorInstance(gendered.SubTypeGeneration, getter: true)}.Write{(gendered.SubTypeGeneration.Nullable ? "Nullable" : string.Empty)}");
            }
            else
            {
                await args.Add(async (gen) =>
                {
                    var listTranslMask = this.MaskModule.GetMaskModule(gendered.SubTypeGeneration.GetType()).GetTranslationMaskTypeStr(gendered.SubTypeGeneration);
                    gen.AppendLine($"transl: (MutagenWriter subWriter, {typeName}{gendered.SubTypeGeneration.NullChar} subItem{(needsMasters ? $", {nameof(TypedWriteParams)} conv" : null)}) =>");
                    using (gen.CurlyBrace())
                    {
                        await subTransl.GenerateWrite(
                            sb: gen,
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

    public override async Task GenerateWrapperFields(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor structDataAccessor,
        Accessor recordDataAccessor,
        int? currentPosition,
        string passedLengthAccessor,
        DataType dataType = null)
    {
        var data = typeGen.GetFieldData();
        switch (data.BinaryOverlayFallback)
        {
            case BinaryGenerationType.Normal:
                break;
            case BinaryGenerationType.NoGeneration:
            case BinaryGenerationType.CustomWrite:
                return;
            case BinaryGenerationType.Custom:
                await this.Module.CustomLogic.GenerateForCustomFlagWrapperFields(
                    sb,
                    objGen,
                    typeGen,
                    currentPosition,
                    passedLengthAccessor,
                    dataType);
                return;
            default:
                throw new NotImplementedException();
        }

        var gendered = typeGen as GenderedType;
        this.Module.TryGetTypeGeneration(gendered.SubTypeGeneration.GetType(), out var subBin);
        var typeName = $"{gendered.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}{gendered.SubTypeGeneration.NullChar}";

        if (data.HasTrigger
            && !gendered.ItemNullable)
        {
            var subLen = (await subBin.ExpectedLength(objGen, gendered.SubTypeGeneration)).Value;
            if (data.HasTrigger)
            {
                sb.AppendLine($"private int? _{typeGen.Name}Location;");
            }
            sb.AppendLine($"public IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}>{(typeGen.Nullable ? "?" : null)} {typeGen.Name}");
            using (sb.CurlyBrace())
            {
                sb.AppendLine("get");
                using (sb.CurlyBrace())
                {
                    var subTypeDefault = gendered.SubTypeGeneration.GetDefault(getter: true);
                    sb.AppendLine($"if (!_{typeGen.Name}Location.HasValue) return {(typeGen.Nullable ? "default" : $"new GenderedItem<{typeName}>({subTypeDefault}, {subTypeDefault})")};");
                    sb.AppendLine($"var data = HeaderTranslation.ExtractSubrecordMemory(_recordData, _{typeGen.Name}Location.Value, _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)});");
                    using (var args = sb.Call(
                               $"return new GenderedItem<{gendered.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}>"))
                    {
                        args.Add($"{subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, "data", "_package")}");
                        args.Add($"{subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, $"data.Slice({subLen})", "_package")}");
                    }
                }
            }
        }
        else if (!data.HasTrigger
                 && !gendered.ItemNullable)
        {
            var subLen = (await subBin.ExpectedLength(objGen, gendered.SubTypeGeneration)).Value;
            if (dataType == null)
            {
                if (data.HasTrigger)
                {
                    throw new NotImplementedException();
                    //sb.AppendLine($"public {typeGen.TypeName(getter: true)}? {typeGen.Name} => {dataAccessor}.Length >= {(currentPosition + this.ExpectedLength(objGen, typeGen).Value)} ? {GenerateForTypicalWrapper(objGen, typeGen, $"{dataAccessor}.Span.Slice({currentPosition}, {this.ExpectedLength(objGen, typeGen).Value})", "_package")} : {typeGen.GetDefault()};");
                }
                else
                {
                    sb.AppendLine($"public IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}> {typeGen.Name}");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine("get");
                        using (sb.CurlyBrace())
                        {
                            if (data.HasTrigger)
                            {
                                sb.AppendLine($"if (!_{typeGen.Name}Location.HasValue) return {typeGen.GetDefault(getter: true)};");
                            }
                            sb.AppendLine($"var data = {structDataAccessor}.Span.Slice({passedLengthAccessor}, {subLen * 2});");
                            using (var args = sb.Call(
                                       $"return new GenderedItem<{gendered.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}>"))
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
                DataBinaryTranslationGeneration.GenerateWrapperExtraMembers(sb, dataType, objGen, typeGen, passedLengthAccessor);
                sb.AppendLine($"public IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}>{(typeGen.Nullable ? "?" : null)} {typeGen.Name}");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("get");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine($"if (!_{typeGen.Name}_IsSet) return new GenderedItem<{gendered.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}>({gendered.SubTypeGeneration.GetDefault(getter: true)}, {gendered.SubTypeGeneration.GetDefault(getter: true)});");
                        sb.AppendLine($"var data = {recordDataAccessor}.Slice(_{typeGen.Name}Location);");
                        using (var args = sb.Call(
                                   $"return new GenderedItem<{gendered.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}>"))
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
            if (data.HasTrigger)
            {
                sb.AppendLine($"private IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}{gendered.SubTypeGeneration.NullChar}>? _{typeGen.Name}Overlay;");
            }
            sb.AppendLine($"public IGenderedItemGetter<{typeName}>{typeGen.NullChar} {typeGen.Name} => _{typeGen.Name}Overlay{(typeGen.Nullable ? null : $" ?? new GenderedItem<{typeName}>(default, default)")};");
        }
    }

    public override async Task GenerateWrapperRecordTypeParse(
        StructuredStringBuilder sb,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor locationAccessor,
        Accessor packageAccessor,
        Accessor converterAccessor)
    {
        var gendered = typeGen as GenderedType;
        if (typeGen.GetFieldData().MarkerType.HasValue && !gendered.MarkerPerGender)
        {
            sb.AppendLine($"stream.Position += _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength; // Skip marker");
        }
        switch (typeGen.GetFieldData().BinaryOverlayFallback)
        {
            case BinaryGenerationType.Normal:
                if (gendered.ItemNullable)
                {
                    string callName;
                    if (gendered.SubTypeGeneration is LoquiType
                        && gendered.MaleMarker.HasValue
                        || gendered.GetFieldData().MarkerType.HasValue)
                    {
                        callName = "FactorySkipMarkersPreRead";
                    }
                    else
                    {
                        callName = "Factory";
                    }
                    bool notNull = gendered.ItemNullable && !gendered.SubTypeGeneration.IsNullable;
                    using (var args = sb.Call(
                               $"_{typeGen.Name}Overlay = GenderedItemBinaryOverlay.{callName}<{gendered.SubTypeGeneration.TypeName(getter: true, needsCovariance: true)}>"))
                    {
                        args.Add("package: _package");
                        if (gendered.MaleMarker.HasValue)
                        {
                            args.Add($"male: {objGen.RecordTypeHeaderName(gendered.MaleMarker.Value)}");
                            args.Add($"female: {objGen.RecordTypeHeaderName(gendered.FemaleMarker.Value)}");
                        }
                        if (gendered.MarkerPerGender)
                        {
                            args.Add($"marker: {objGen.RecordTypeHeaderName(typeGen.GetFieldData().MarkerType.Value)}");
                        }
                        if (gendered.SubTypeGeneration is LoquiType loqui)
                        {
                            args.AddPassArg("stream");
                            args.Add($"creator: static (s, p, r) => {this.Module.BinaryOverlayClassName(loqui)}.{loqui.TargetObjectGeneration.Name}Factory(s, p, r)");
                            var subData = loqui.GetFieldData();
                            if (subData?.RecordTypeConverter != null
                                && subData.RecordTypeConverter.FromConversions.Count > 0)
                            {
                                args.Add($"translationParams: {objGen.RegistrationName}.{(typeGen.Name ?? typeGen.Parent?.Name)}Converter");
                            }
                            else if (converterAccessor != null
                                     && gendered.FemaleConversions == null
                                     && gendered.MaleConversions == null)
                            {
                                args.Add($"translationParams: {converterAccessor}");
                            }
                        }
                        else
                        {
                            args.AddPassArg("stream");
                            this.Module.TryGetTypeGeneration(gendered.SubTypeGeneration.GetType(), out var subGen);
                            args.Add($"creator: static (m, p) => {subGen.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, $"{nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordMemory)}(m, p.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)})", "p")}");
                        }
                        if (gendered.FemaleConversions != null)
                        {
                            args.Add($"femaleRecordConverter: {objGen.RegistrationName}.{typeGen.Name}FemaleConverter");
                        }
                        if (gendered.MaleConversions != null)
                        {
                            args.Add($"maleRecordConverter: {objGen.RegistrationName}.{typeGen.Name}MaleConverter");
                        }

                        if (!gendered.ShortCircuit)
                        {
                            args.Add("shortCircuit: false");
                        }

                        if (gendered.ParseNonConvertedItems)
                        {
                            args.Add("parseNonConvertedItems: true");
                        }
                        
                        if (notNull)
                        {
                            args.Add($"fallback: {gendered.SubTypeGeneration.GetDefault(getter: false)}");
                        }
                    }
                }
                else
                {
                    await base.GenerateWrapperRecordTypeParse(sb, objGen, typeGen, locationAccessor, packageAccessor, converterAccessor);
                }
                break;
            default:
                await base.GenerateWrapperRecordTypeParse(sb, objGen, typeGen, locationAccessor, packageAccessor, converterAccessor);
                break;
        }
    }
}