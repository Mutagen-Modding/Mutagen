using Loqui;
using Loqui.Generation;
using Loqui.Internal;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Strings;
using Noggog;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Strings.DI;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using BoolType = Mutagen.Bethesda.Generation.Fields.BoolType;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;
using EnumType = Mutagen.Bethesda.Generation.Fields.EnumType;
using FloatType = Mutagen.Bethesda.Generation.Fields.FloatType;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;
using PercentType = Mutagen.Bethesda.Generation.Fields.PercentType;
using StringType = Mutagen.Bethesda.Generation.Fields.StringType;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class PluginTranslationModule : BinaryTranslationModule
{
    public override string WriterClass => nameof(MutagenWriter);
    public override string WriterMemberName => "writer";
    public override string ReaderClass => nameof(MutagenFrame);
    public override string ReaderMemberName => "frame";

    public override bool DoTranslationInterface(ObjectGeneration obj) => obj.GetObjectType() != ObjectType.Mod;

    public override bool DirectTranslationReference(ObjectGeneration obj) => obj.GetObjectType() == ObjectType.Mod;

    public override string TranslatorReference(ObjectGeneration obj, Accessor item)
    {
        if (obj.GetObjectType() != ObjectType.Mod)
        {
            return base.TranslatorReference(obj, item);
        }
        return $"{this.TranslationWriteClass(obj)}.Instance";
    }

    public PluginTranslationModule(LoquiGenerator gen)
        : base(gen)
    {
        this.DoErrorMasks = false;
        this.TranslationMaskParameter = false;
        this._typeGenerations[typeof(LoquiType)] = new LoquiBinaryTranslationGeneration(TranslationTerm);
        this._typeGenerations[typeof(BoolType)] = new BooleanBinaryTranslationGeneration();
        this._typeGenerations[typeof(CharType)] = new PrimitiveBinaryTranslationGeneration<char>(expectedLen: 1);
        this._typeGenerations[typeof(DateTimeType)] = new PrimitiveBinaryTranslationGeneration<DateTime>(expectedLen: null);
        this._typeGenerations[typeof(DoubleType)] = new PrimitiveBinaryTranslationGeneration<double>(expectedLen: 8);
        this._typeGenerations[typeof(EnumType)] = new EnumBinaryTranslationGeneration();
        this._typeGenerations[typeof(FloatType)] = new FloatBinaryTranslationGeneration();
        this._typeGenerations[typeof(PercentType)] = new PercentBinaryTranslationGeneration();
        this._typeGenerations[typeof(Int8Type)] = new SByteBinaryTranslationGeneration();
        this._typeGenerations[typeof(Int16Type)] = new PrimitiveBinaryTranslationGeneration<short>(expectedLen: 2);
        this._typeGenerations[typeof(Int32Type)] = new PrimitiveBinaryTranslationGeneration<int>(expectedLen: 4);
        this._typeGenerations[typeof(Int64Type)] = new PrimitiveBinaryTranslationGeneration<long>(expectedLen: 8);
        this._typeGenerations[typeof(P3UInt8Type)] = new PointBinaryTranslationGeneration<P3UInt8>(expectedLen: 3);
        this._typeGenerations[typeof(P3UInt16Type)] = new PointBinaryTranslationGeneration<P3UInt16>(expectedLen: 6);
        this._typeGenerations[typeof(P2FloatType)] = new PointBinaryTranslationGeneration<P2Float>(expectedLen: 8);
        this._typeGenerations[typeof(P3FloatType)] = new PointBinaryTranslationGeneration<P3Float>(expectedLen: 12);
        this._typeGenerations[typeof(P2Int32Type)] = new PointBinaryTranslationGeneration<P2Int>(expectedLen: 8);
        this._typeGenerations[typeof(P3IntType)] = new PointBinaryTranslationGeneration<P3Int>(expectedLen: 12);
        this._typeGenerations[typeof(P2Int16Type)] = new PointBinaryTranslationGeneration<P2Int16>(expectedLen: 4);
        this._typeGenerations[typeof(P3Int16Type)] = new PointBinaryTranslationGeneration<P3Int16>(expectedLen: 6);
        this._typeGenerations[typeof(P2FloatType)] = new PointBinaryTranslationGeneration<P2Float>(expectedLen: 8);
        this._typeGenerations[typeof(StringType)] = new StringBinaryTranslationGeneration()
        {
            PreferDirectTranslation = false
        };
        this._typeGenerations[typeof(FilePathType)] = new FilePathBinaryTranslationGeneration();
        this._typeGenerations[typeof(UInt8Type)] = new ByteBinaryTranslationGeneration();
        this._typeGenerations[typeof(UInt16Type)] = new PrimitiveBinaryTranslationGeneration<ushort>(expectedLen: 2);
        this._typeGenerations[typeof(UInt32Type)] = new PrimitiveBinaryTranslationGeneration<uint>(expectedLen: 4);
        this._typeGenerations[typeof(UInt64Type)] = new PrimitiveBinaryTranslationGeneration<ulong>(expectedLen: 8);
        this._typeGenerations[typeof(FormIDType)] = new PrimitiveBinaryTranslationGeneration<FormID>(expectedLen: 4)
        {
            PreferDirectTranslation = false
        };
        this._typeGenerations[typeof(FormKeyType)] = new FormKeyBinaryTranslationGeneration();
        this._typeGenerations[typeof(ModKeyType)] = new ModKeyBinaryTranslationGeneration();
        this._typeGenerations[typeof(RecordTypeType)] = new RecordTypeBinaryTranslationGeneration();
        this._typeGenerations[typeof(FormLinkType)] = new FormLinkBinaryTranslationGeneration();
        this._typeGenerations[typeof(ListType)] = new PluginListBinaryTranslationGeneration();
        this._typeGenerations[typeof(Array2dType)] = new Array2dBinaryTranslationGeneration();
        this._typeGenerations[typeof(ArrayType)] = new PluginArrayBinaryTranslationGeneration();
        this._typeGenerations[typeof(DictType)] = new DictBinaryTranslationGeneration();
        this._typeGenerations[typeof(ByteArrayType)] = new ByteArrayBinaryTranslationGeneration();
        this._typeGenerations[typeof(BufferType)] = new BufferBinaryTranslationGeneration();
        this._typeGenerations[typeof(DataType)] = new DataBinaryTranslationGeneration();
        this._typeGenerations[typeof(ColorType)] = new ColorBinaryTranslationGeneration()
        {
            PreferDirectTranslation = false
        };
        this._typeGenerations[typeof(ZeroType)] = new ZeroBinaryTranslationGeneration();
        this._typeGenerations[typeof(NothingType)] = new NothingBinaryTranslationGeneration();
        this._typeGenerations[typeof(CustomLogic)] = new CustomLogicTranslationGeneration();
        this._typeGenerations[typeof(GenderedType)] = new GenderedTypeBinaryTranslationGeneration();
        this._typeGenerations[typeof(BreakType)] = new BreakBinaryTranslationGeneration();
        this._typeGenerations[typeof(MarkerType)] = new MarkerBinaryTranslationGeneration();
        APILine[] modAPILines = new APILine[]
        {
            new APILine(
                nicknameKey: "GroupMask",
                resolutionString: "GroupMask? importMask = null",
                when: (obj, dir) => obj.GetObjectType() == ObjectType.Mod)
        };
        var modKey = new APILine(
            nicknameKey: "ModKey",
            resolutionString: "ModKey modKey",
            when: (obj, dir) =>
            {
                if (dir == TranslationDirection.Writer) return false;
                return obj.GetObjectType() == ObjectType.Mod;
            });
        var modKeyWriter = new APILine(
            nicknameKey: "ModKeyWriter",
            resolutionString: "ModKey modKey",
            when: (obj, dir) =>
            {
                if (dir == TranslationDirection.Reader) return false;
                return obj.GetObjectType() == ObjectType.Mod;
            });
        var writeParamOptional = new APILine(
            nicknameKey: "WriteParamOptional",
            resolutionString: $"{nameof(BinaryWriteParameters)}? param = null",
            when: (obj, dir) =>
            {
                if (dir == TranslationDirection.Reader) return false;
                return obj.GetObjectType() == ObjectType.Mod;
            });
        var stringsReadParamOptional = new APILine(
            nicknameKey: "StringsParamsOptional",
            resolutionString: $"{nameof(StringsReadParameters)}? stringsParam = null",
            when: (obj, dir) =>
            {
                if (dir == TranslationDirection.Writer) return false;
                return obj.GetObjectType() == ObjectType.Mod
                       && obj.GetObjectData().UsesStringFiles;
            });
        var recordInfoCache = new APILine(
            nicknameKey: "RecordInfoCache",
            resolutionString: $"{nameof(RecordTypeInfoCacheReader)} infoCache",
            when: (obj, dir) =>
            {
                if (dir != TranslationDirection.Reader) return false;
                return obj.GetObjectType() == ObjectType.Mod;
            });
        var parallel = new APILine(
            nicknameKey: "Parallel",
            resolutionString: "bool parallel = true",
            when: (obj, dir) =>
            {
                if (dir == TranslationDirection.Writer) return false;
                return obj.GetObjectType() == ObjectType.Mod;
            });
        var fileSystem = new APILine(
            nicknameKey: "FileSystem",
            resolutionString: "IFileSystem? fileSystem = null",
            when: (obj, dir) =>
            {
                return obj.GetObjectType() == ObjectType.Mod;
            });
        var gameRelease = new APILine(
            nicknameKey: "GameRelease",
            resolver: (obj) => $"{ModModule.ReleaseEnumName(obj)} release",
            when: (obj, dir) =>
            {
                if (dir == TranslationDirection.Writer) return false;
                if (obj.GetObjectType() != ObjectType.Mod) return false;
                return obj.GetObjectData().GameReleaseOptions != null;
            });
        var typedWriteParams = new APILine(
            "TypedWriteParams",
            $"{nameof(TypedWriteParams)}? translationParams = null",
            when: (obj, dir) =>
            {
                return dir == TranslationDirection.Writer 
                       && obj.GetObjectType() != ObjectType.Mod;
            });
        var typedParseParams = new APILine(
            "TypedParseParams",
            $"{nameof(TypedParseParams)}? translationParams = null",
            when: (obj, dir) =>
            {
                return dir == TranslationDirection.Reader
                       && obj.GetObjectType() != ObjectType.Mod;
            });
        this.MainAPI = new TranslationModuleAPI(
            writerAPI: new MethodAPI(
                majorAPI: new APILine[] { new APILine(WriterClass, $"{WriterClass} {WriterMemberName}") },
                optionalAPI: modAPILines
                    .And(typedWriteParams)
                    .And(typedParseParams)
                    .And(writeParamOptional)
                    .ToArray(),
                customAPI: new CustomMethodAPI[]
                {
                    CustomMethodAPI.FactoryPublic(modKey),
                    CustomMethodAPI.FactoryPrivate(modKeyWriter, "modKey"),
                }),
            readerAPI: new MethodAPI(
                majorAPI: new APILine[] { new APILine(ReaderClass, $"{ReaderClass} {ReaderMemberName}") },
                optionalAPI: modAPILines
                    .And(typedWriteParams)
                    .And(typedParseParams)
                    .And(writeParamOptional)
                    .ToArray(),
                customAPI: new CustomMethodAPI[]
                {
                    CustomMethodAPI.FactoryPublic(gameRelease),
                }));
        this.MinorAPIs.Add(
            new TranslationModuleAPI(
                new MethodAPI(
                    majorAPI: new APILine[] { new APILine("Path", $"{nameof(ModPath)} path") },
                    customAPI: new CustomMethodAPI[]
                    {
                        CustomMethodAPI.FactoryPublic(gameRelease)
                    },
                    optionalAPI: writeParamOptional
                        .AsEnumerable()
                        .And(modAPILines)
                        .And(stringsReadParamOptional)
                        .And(parallel)
                        .And(fileSystem)
                        .ToArray()))
            {
                Funnel = new TranslationFunnel(
                    this.MainAPI,
                    ConvertFromPathOut,
                    ConvertFromPathIn),
                When = (o, d) => d == TranslationDirection.Reader && o.GetObjectType() == ObjectType.Mod
            });
        this.MinorAPIs.Add(
            new TranslationModuleAPI(
                new MethodAPI(
                    majorAPI: new APILine[] { new APILine("Path", $"{nameof(FilePath)} path") },
                    customAPI: new CustomMethodAPI[]
                    {
                        CustomMethodAPI.FactoryPublic(gameRelease)
                    },
                    optionalAPI: writeParamOptional
                        .AsEnumerable()
                        .And(modAPILines)
                        .And(stringsReadParamOptional)
                        .And(parallel)
                        .And(fileSystem)
                        .ToArray()))
            {
                Funnel = new TranslationFunnel(
                    this.MainAPI,
                    ConvertFromPathOut,
                    ConvertFromPathIn),
                When = (o, d) => d == TranslationDirection.Writer && o.GetObjectType() == ObjectType.Mod
            });
        this.MinorAPIs.Add(
            new TranslationModuleAPI(
                new MethodAPI(
                    majorAPI: new APILine[] { new APILine("Stream", "Stream stream") },
                    customAPI: new CustomMethodAPI[]
                    {
                        CustomMethodAPI.FactoryPublic(modKey),
                        CustomMethodAPI.FactoryPublic(gameRelease),
                        CustomMethodAPI.FactoryPublic(recordInfoCache),
                        CustomMethodAPI.FactoryPublic(writeParamOptional),
                    },
                    optionalAPI: modAPILines
                        .And(parallel)
                        .ToArray()))
            {
                Funnel = new TranslationFunnel(
                    this.MainAPI,
                    ConvertFromStreamOut,
                    ConvertFromStreamIn),
                When = (o, _) => o.GetObjectType() == ObjectType.Mod
            });
        this.CustomLogic = new CustomLogicTranslationGeneration() { Module = this };
    }

    #region Minor API Translations
    private void ConvertFromStreamOut(ObjectGeneration obj, StructuredStringBuilder sb, InternalTranslation internalToDo)
    {
        var objData = obj.GetObjectData();
        string gameReleaseStr;
        if (objData.GameReleaseOptions == null)
        {
            gameReleaseStr = $"{nameof(GameRelease)}.{objData.GameCategory}";
        }
        else
        {
            gameReleaseStr = $"item.{ModModule.ReleaseEnumName(obj)}.ToGameRelease()";
        }
        sb.AppendLine("var modKey = item.ModKey;");
        using (var args = sb.Call(
                   $"using (var writer = new MutagenWriter",
                   suffixLine: ")",
                   semiColon: false))
        {
            args.AddPassArg("stream");
            args.Add($"new {nameof(WritingBundle)}({gameReleaseStr})");
            args.Add("dispose: false");
        }
        using (sb.CurlyBrace())
        {
            internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Writer).ToArray());
        }
    }

    private void ConvertFromStreamIn(ObjectGeneration obj, StructuredStringBuilder sb, InternalTranslation internalToDo)
    {
        sb.AppendLine("try");
        using (sb.CurlyBrace())
        {
            string gameReleaseStr;
            if (obj.GetObjectData().GameReleaseOptions == null)
            {
                gameReleaseStr = $"{nameof(GameRelease)}.{obj.GetObjectData().GameCategory}";
            }
            else
            {
                gameReleaseStr = $"release.ToGameRelease()";
            }
            sb.AppendLine($"using (var reader = new {nameof(MutagenBinaryReadStream)}(stream, modKey, {gameReleaseStr}))");
            using (sb.CurlyBrace())
            {
                sb.AppendLine("var frame = new MutagenFrame(reader);");
                sb.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.RecordInfoCache)} = infoCache;");
                sb.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Parallel)} = parallel;");
                sb.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.ModKey)} = modKey;");
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Reader).ToArray());
            }
        }
        sb.AppendLine("catch (Exception ex)"); 
        using (sb.CurlyBrace()) 
        { 
            sb.AppendLine("throw RecordException.Enrich(ex, modKey);"); 
        } 
    }

    private void ConvertFromPathOut(ObjectGeneration obj, StructuredStringBuilder sb, InternalTranslation internalToDo)
    {
        var objData = obj.GetObjectData();
        string gameReleaseStr;
        if (objData.GameReleaseOptions == null)
        {
            gameReleaseStr = $"{nameof(GameRelease)}.{objData.GameCategory}";
        }
        else
        {
            gameReleaseStr = $"item.{ModModule.ReleaseEnumName(obj)}.ToGameRelease()";
        }

        sb.AppendLine($"param ??= {nameof(BinaryWriteParameters)}.{nameof(BinaryWriteParameters.Default)};");
        using (var args = sb.Call(
                   $"var modKey = param.{nameof(BinaryWriteParameters.RunMasterMatch)}"))
        {
            args.Add("mod: item");
            args.AddPassArg("path");
        }
        if (objData.UsesStringFiles)
        {
            sb.AppendLine($"param.StringsWriter ??= (EnumExt.HasFlag((int)item.ModHeader.Flags, (int)ModHeaderCommonFlag.Localized) ? new StringsWriter({gameReleaseStr}, modKey, Path.Combine(Path.GetDirectoryName(path)!, \"Strings\"), {nameof(MutagenEncodingProvider)}.{nameof(MutagenEncodingProvider.Instance)}) : null);");
            sb.AppendLine("bool disposeStrings = param.StringsWriter != null;");
        }
        sb.AppendLine($"var bundle = new {nameof(WritingBundle)}({gameReleaseStr})");
        using (var prop = sb.PropertyCtor())
        {
            if (objData.UsesStringFiles)
            {
                prop.Add($"{nameof(WritingBundle.StringsWriter)} = param.StringsWriter");
            }
            prop.Add($"{nameof(WritingBundle.CleanNulls)} = param.{nameof(BinaryWriteParameters.CleanNulls)}");
            prop.Add($"{nameof(WritingBundle.TargetLanguageOverride)} = param.{nameof(BinaryWriteParameters.TargetLanguageOverride)}");
        }
        sb.AppendLine("using var memStream = new MemoryTributary();");
        using (var args = sb.Call(
                   $"using (var writer = new MutagenWriter",
                   suffixLine: ")",
                   semiColon: false))
        {
            args.Add("memStream");
            args.Add("bundle");
            args.Add("dispose: false");
        }
        using (sb.CurlyBrace())
        {
            internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Writer).ToArray());
        }
        sb.AppendLine($"using (var fs = fileSystem.GetOrDefault().FileStream.Create(path, FileMode.Create, FileAccess.Write))");
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"memStream.Position = 0;");
            sb.AppendLine($"memStream.CopyTo(fs);");
        }
        if (objData.UsesStringFiles)
        {
            sb.AppendLine("if (disposeStrings)");
            using (sb.CurlyBrace())
            {
                sb.AppendLine("param.StringsWriter?.Dispose();");
            }
        }
    }

    private void ConvertFromPathIn(ObjectGeneration obj, StructuredStringBuilder sb, InternalTranslation internalToDo)
    {
        sb.AppendLine("try");
        using (sb.CurlyBrace())
        {
            string gameReleaseStr;
            if (obj.GetObjectData().GameReleaseOptions == null)
            {
                gameReleaseStr = $"{nameof(GameRelease)}.{obj.GetObjectData().GameCategory}";
            }
            else
            {
                sb.AppendLine($"var gameRelease = release.ToGameRelease();");
                gameReleaseStr = $"gameRelease";
            }
            sb.AppendLine($"using (var reader = new {nameof(MutagenBinaryReadStream)}(path, {gameReleaseStr}, fileSystem: fileSystem))");
            using (sb.CurlyBrace())
            {
                sb.AppendLine("var frame = new MutagenFrame(reader);");
                sb.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordTypeInfoCacheReader)}(() => new {nameof(MutagenBinaryReadStream)}(path, {gameReleaseStr}));");
                sb.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Parallel)} = parallel;");
                sb.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.ModKey)} = path.ModKey;");
                if (obj.GetObjectData().UsesStringFiles)
                {
                    sb.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Absorb)}(stringsParam);");
                    sb.AppendLine("if (reader.Remaining < 12)");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine($"throw new ArgumentException(\"File stream was too short to parse flags\");");
                    }
                    sb.AppendLine($"var flags = reader.GetInt32(offset: 8);");
                    sb.AppendLine($"if (EnumExt.HasFlag(flags, (int)ModHeaderCommonFlag.Localized))");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine($"frame.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.StringsLookup)} = StringsFolderLookupOverlay.TypicalFactory({gameReleaseStr}, path.{nameof(ModPath.ModKey)}, Path.GetDirectoryName(path.{nameof(ModPath.Path)})!, stringsParam);");
                    }
                }
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Reader).ToArray());
            }
        }
        sb.AppendLine("catch (Exception ex)");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("throw RecordException.Enrich(ex, path.ModKey);");
        }
    }

    #endregion

    public override bool WantsTryCreateFromBinary(ObjectGeneration obj)
    {
        return base.WantsTryCreateFromBinary(obj) && obj.GetObjectType() != ObjectType.Mod;
    }

    protected override async Task GenerateBinaryOverlayCreates(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (obj.GetObjectType() != ObjectType.Mod) return;
        var objData = obj.GetObjectData();
            
        using (var args = sb.Function(
                   $"public{obj.NewOverride()}static I{obj.Name}DisposableGetter {CreateFromPrefix}{ModuleNickname}Overlay"))
        {
            args.Add($"{nameof(ModPath)} path");
            if (objData.GameReleaseOptions != null)
            {
                args.Add($"{ModModule.ReleaseEnumName(obj)} release");
            }
            if (objData.UsesStringFiles)
            {
                args.Add($"{nameof(StringsReadParameters)}? stringsParam = null");
            }
            args.Add($"IFileSystem? fileSystem = null");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"return {BinaryOverlayClass(obj)}.{obj.Name}Factory"))
            {
                args.AddPassArg("path");
                if (objData.UsesStringFiles)
                {
                    args.AddPassArg("stringsParam");
                }
                if (objData.GameReleaseOptions != null)
                {
                    args.AddPassArg("release");
                }
                args.AddPassArg("fileSystem");
            }
        }
        sb.AppendLine();

        using (var args = sb.Function(
                   $"public{obj.NewOverride()}static I{obj.Name}DisposableGetter {CreateFromPrefix}{ModuleNickname}Overlay"))
        {
            args.Add($"{nameof(Stream)} stream");
            if (objData.GameReleaseOptions != null)
            {
                args.Add($"{ModModule.ReleaseEnumName(obj)} release");
            }
            args.Add($"ModKey modKey");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       $"return {BinaryOverlayClass(obj)}.{obj.Name}Factory"))
            {
                string gameReleaseStr;
                if (obj.GetObjectData().GameReleaseOptions == null)
                {
                    gameReleaseStr = $"{nameof(GameRelease)}.{obj.GetObjectData().GameCategory}";
                }
                else
                {
                    gameReleaseStr = $"release.ToGameRelease()";
                }
                args.Add($"stream: new {nameof(MutagenBinaryReadStream)}(stream, modKey, {gameReleaseStr})");
                args.AddPassArg("modKey");
                if (objData.GameReleaseOptions != null)
                {
                    args.AddPassArg("release");
                }
                args.Add("shouldDispose: false");
            }
        }
        sb.AppendLine();
    }

    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        await base.GenerateInClass(obj, sb);

        if (obj.GetObjectType() == ObjectType.Mod)
        {
            using (var args = sb.Function(
                       $"public{obj.NewOverride()}static {await this.ObjectReturn(obj, maskReturn: false)} {CreateFromPrefix}{TranslationTerm}"))
            {
                foreach (var (API, Public) in this.MainAPI.ReaderAPI.IterateAPI(obj,
                             TranslationDirection.Reader,
                             this.DoErrorMasks ? new APILine(ErrorMaskKey, "ErrorMaskBuilder? errorMask") : null,
                             this.DoErrorMasks ? new APILine(TranslationMaskKey, $"{nameof(TranslationCrystal)}? translationMask", (o, i) => this.TranslationMaskParameter) : null))
                {
                    args.Add(API.Result);
                }
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("try");
                using (sb.CurlyBrace())
                {
                    await GenerateNewSnippet(obj, sb);
                    using (var args = sb.Call(
                               $"{Loqui.Generation.Utility.Await(await AsyncImport(obj))}{obj.CommonClassInstance("ret", LoquiInterfaceType.ISetter, CommonGenerics.Class)}.{CopyInFromPrefix}{TranslationTerm}"))
                    {
                        args.Add("item: ret");
                        foreach (var arg in this.MainAPI.PassArgs(obj, TranslationDirection.Reader))
                        {
                            args.Add(arg);
                        }
                        foreach (var arg in this.MainAPI.InternalPassArgs(obj, TranslationDirection.Reader))
                        {
                            args.Add(arg);
                        }
                        if (this.DoErrorMasks)
                        {
                            args.AddPassArg("errorMask");
                        }
                        if (this.TranslationMaskParameter)
                        {
                            args.AddPassArg("translationMask");
                        }
                    }
                    sb.AppendLine("return ret;");
                }
                sb.AppendLine("catch (Exception ex)");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("throw RecordException.Enrich(ex, frame.MetaData.ModKey);");
                }
            }
            sb.AppendLine();
        }
    }

    public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
    {
        await foreach (var u in base.RequiredUsingStatements(obj))
        {
            yield return u;
        }

        if (obj.GetObjectType() == ObjectType.Mod)
        {
            yield return "Mutagen.Bethesda.Strings";
            yield return "Mutagen.Bethesda.Plugins.Binary";
            yield return "Mutagen.Bethesda.Plugins.Meta";
            yield return "Mutagen.Bethesda.Plugins.Utility";
            yield return "Mutagen.Bethesda.Plugins.Cache.Internals";
            yield return "Mutagen.Bethesda.Strings.DI";
        }

        if (HasRecordTypeFields(obj))
        {
            yield return "Mutagen.Bethesda.Plugins.Internals";
        }

        yield return "Mutagen.Bethesda.Plugins";
        yield return "Mutagen.Bethesda.Plugins.Records";
        yield return "Mutagen.Bethesda.Plugins.Records.Internals";
        yield return "Mutagen.Bethesda.Plugins.Binary.Overlay";
        yield return "Mutagen.Bethesda.Plugins.Binary.Translations";
        yield return "Mutagen.Bethesda.Plugins.Binary.Streams";
        yield return "Mutagen.Bethesda.Plugins.Exceptions";
        yield return "Mutagen.Bethesda.Plugins.Binary.Headers";
            
        if (obj.GetObjectType() == ObjectType.Group && !obj.IsListGroup())
        {
            yield return $"Mutagen.Bethesda.{obj.ProtoGen.Protocol.Namespace}.Records";
        }

        if (await LinkModule.HasLinks(obj, includeBaseClass: false) != Case.No
            || obj.IterateFields().Any(f => f is FormKeyType))
        {
            yield return "Mutagen.Bethesda.Plugins.Cache";
            yield return "Mutagen.Bethesda.Plugins.Internals";
        }
    }

    public override async Task GenerateInTranslationCreateClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        await GenerateCreateExtras(obj, sb);
        await base.GenerateInTranslationCreateClass(obj, sb);
    }

    private async Task GenerateCreateExtras(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        var data = obj.GetObjectData();
        bool typelessStruct = obj.IsTypelessStruct();

        if (await obj.IsMajorRecord())
        {
            if (data.TriggeringRecordTypes.Count == 1
                && !obj.Abstract)
            {
                sb.AppendLine($"public{obj.FunctionOverride()}RecordType RecordType => {obj.GetTriggeringSource()};");
            }
            else
            {
                sb.AppendLine($"public{obj.FunctionOverride()}RecordType RecordType => throw new ArgumentException();");
            }
        }

        if ((!obj.Abstract && obj.BaseClassTrail().All((b) => b.Abstract)) || HasEmbeddedFields(obj))
        {
            var async = HasAsyncStructs(obj, self: true);
            using (var args = sb.Function(
                       $"public static {Loqui.Generation.Utility.TaskReturn(async)} Fill{ModuleNickname}Structs"))
            {
                args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                args.Add($"{ReaderClass} {ReaderMemberName}");
            }
            using (sb.CurlyBrace())
            {
                if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any((b) => HasEmbeddedFields(b)))
                {
                    using (var args = sb.Call(
                               $"{Loqui.Generation.Utility.Await(async)}{TranslationCreateClass(obj.BaseClass)}.Fill{ModuleNickname}Structs"))
                    {
                        args.AddPassArg("item");
                        args.AddPassArg(ReaderMemberName);
                    }
                }
                int breakIndex = 0;
                foreach (var field in obj.IterateFields(
                             nonIntegrated: true,
                             expandSets: SetMarkerType.ExpandSets.False))
                {
                    if (field is SetMarkerType) continue;
                    if (field is CustomLogic logic && logic.IsRecordType) continue;
                    var fieldData = field.GetFieldData();
                    if (fieldData.HasTrigger) continue;
                    if (fieldData.Binary == BinaryGenerationType.NoGeneration) continue;
                    if (field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
                    if (!field.Enabled) continue;
                    if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                    {
                        if (!field.IntegrateField) continue;
                        throw new ArgumentException("Unsupported type generator: " + field);
                    }
                    if (field.Nullable)
                    {
                        sb.AppendLine($"if (frame.Complete) return;");
                    }

                    if (field is BreakType)
                    {
                        sb.AppendLine("if (frame.Complete)");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine($"item.{VersioningModule.VersioningFieldName} |= {obj.Name}.{VersioningModule.VersioningEnumName}.Break{breakIndex++};");
                            sb.AppendLine("return;");
                        }
                        continue;
                    }
                    await GenerateFillSnippet(obj, sb, field, generator, "frame");
                }
            }
            sb.AppendLine();
        }

        if (HasRecordTypeFields(obj))
        {
            using (var args = sb.Function(
                       $"public static {Loqui.Generation.Utility.TaskWrap(nameof(ParseResult), HasAsyncRecords(obj, self: true))} Fill{ModuleNickname}RecordTypes"))
            {
                args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                args.Add($"{ReaderClass} {ReaderMemberName}");
                if (obj.GetObjectType() == ObjectType.Subrecord
                    || obj.GetObjectType() == ObjectType.Record)
                {
                    args.Add($"{nameof(PreviousParse)} lastParsed");
                }
                if (obj.GetObjectType() != ObjectType.Mod)
                {
                    args.Add("Dictionary<RecordType, int>? recordParseCount");
                }
                args.Add("RecordType nextRecordType");
                args.Add("int contentLength");
                if (data.ObjectType == ObjectType.Mod)
                {
                    args.Add($"GroupMask? importMask");
                }
                args.Add($"{nameof(TypedParseParams)}? translationParams = null");
            }
            using (sb.CurlyBrace())
            {
                var mutaObjType = obj.GetObjectType();
                sb.AppendLine($"nextRecordType = translationParams.ConvertToStandard(nextRecordType);");
                sb.AppendLine("switch (nextRecordType.TypeInt)");
                using (sb.CurlyBrace())
                {
                    var fields = new List<(int, int, TypeGeneration Field)>();
                    foreach (var field in obj.IterateFieldIndices(
                                 expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                                 nonIntegrated: true))
                    {
                        var fieldData = field.Field.GetFieldData();
                        if (!fieldData.GenerationTypes.Any()) continue;
                        if (fieldData.Binary == BinaryGenerationType.NoGeneration) continue;
                        if (field.Field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
                        if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field.Field);
                        }

                        if (!generator.ShouldGenerateCopyIn(field.Field)) continue;
                        fields.Add(field);
                    }

                    var doubleUsages = new Dictionary<RecordType, List<(int, int, TypeGeneration)>>();
                    foreach (var field in fields)
                    {
                        var fieldData = field.Field.GetFieldData();
                        if (fieldData.GenerationTypes.Count() > 1) continue;
                        foreach (var gen in fieldData.GenerationTypes)
                        {
                            if (gen.Key.Count() > 1) continue;
                            LoquiType loqui = gen.Value as LoquiType;
                            if (loqui?.TargetObjectGeneration?.Abstract ?? false) continue;
                            doubleUsages.GetOrAdd(gen.Key.First()).Add(field);
                        }
                    }
                    foreach (var item in doubleUsages.ToList())
                    {
                        if (item.Value.Count <= 1)
                        {
                            doubleUsages.Remove(item.Key);
                        }
                    }

                    foreach (var field in fields)
                    {
                        var fieldData = field.Field.GetFieldData();
                        if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field.Field);
                        }
                        foreach (var gen in fieldData.GenerationTypes)
                        {
                            LoquiType loqui = gen.Value as LoquiType;
                            if (loqui?.TargetObjectGeneration?.Abstract ?? false) continue;

                            List<(int, int, TypeGeneration Field)> doubles = null;
                            if (gen.Key.Count() == 1)
                            {
                                if (doubleUsages.TryGetValue(gen.Key.First(), out doubles))
                                {
                                    // Means we handled earlier, break out 
                                    if (doubles.Count == 0) continue;
                                }
                            }

                            foreach (var trigger in gen.Key)
                            {
                                sb.AppendLine($"case RecordTypeInts.{trigger.CheckedType}:");
                            }
                            using (sb.CurlyBrace())
                            {
                                if (doubles == null)
                                {
                                    await GenerateLastParsedShortCircuit(
                                        obj: obj,
                                        sb: sb,
                                        field: field,
                                        doublesPotential: false,
                                        lastRequiredIndex: obj.GetObjectData().GetLastRequiredFieldIndexToUse(),
                                        nextRecAccessor: "nextRecordType",
                                        toDo: async () =>
                                        {
                                            var groupMask = data.ObjectType == ObjectType.Mod && (loqui?.TargetObjectGeneration?.GetObjectType() == ObjectType.Group);
                                            if (groupMask)
                                            {
                                                sb.AppendLine($"if (importMask?.{field.Field.Name} ?? true)");
                                            }
                                            using (sb.CurlyBrace(doIt: groupMask))
                                            {
                                                await GenerateFillSnippet(obj, sb, gen.Value, generator, ReaderMemberName);
                                            }
                                            if (groupMask)
                                            {
                                                sb.AppendLine("else");
                                                using (sb.CurlyBrace())
                                                {
                                                    sb.AppendLine($"{ReaderMemberName}.Position += contentLength;");
                                                }
                                            }
                                        });
                                }
                                else
                                {
                                    sb.AppendLine($"switch (recordParseCount?.GetOrAdd(nextRecordType) ?? 0)");
                                    using (sb.CurlyBrace())
                                    {
                                        int count = 0;
                                        foreach (var doublesField in doubles)
                                        {
                                            if (!this.TryGetTypeGeneration(doublesField.Field.GetType(), out var doubleGen))
                                            {
                                                throw new ArgumentException("Unsupported type generator: " + doublesField.Field);
                                            }
                                            sb.AppendLine($"case {count++}:");
                                            using (sb.IncreaseDepth())
                                            {
                                                await GenerateLastParsedShortCircuit(
                                                    obj: obj,
                                                    sb: sb,
                                                    field: doublesField,
                                                    doublesPotential: true,
                                                    lastRequiredIndex: obj.GetObjectData().GetLastRequiredFieldIndexToUse(),
                                                    nextRecAccessor: "nextRecordType",
                                                    toDo: async () =>
                                                    {
                                                        var groupMask = data.ObjectType == ObjectType.Mod && (loqui?.TargetObjectGeneration?.GetObjectType() == ObjectType.Group);
                                                        if (groupMask)
                                                        {
                                                            sb.AppendLine($"if (importMask?.{doublesField.Field.Name} ?? true)");
                                                        }
                                                        using (sb.CurlyBrace(doIt: groupMask))
                                                        {
                                                            await GenerateFillSnippet(obj, sb, doublesField.Field, doubleGen, ReaderMemberName);
                                                        }
                                                        if (groupMask)
                                                        {
                                                            sb.AppendLine("else");
                                                            using (sb.CurlyBrace())
                                                            {
                                                                sb.AppendLine($"{ReaderMemberName}.Position += contentLength;");
                                                            }
                                                        }
                                                    });
                                            }
                                        }
                                        sb.AppendLine($"default:");
                                        using (sb.IncreaseDepth())
                                        {
                                            sb.AppendLine($"throw new NotImplementedException();");
                                        }
                                    }
                                    doubles.Clear();
                                }
                            }
                        }
                    }

                    AttachOverflowCases(sb, fields, "frame");

                    if (data.EndMarkerType.HasValue)
                    {
                        sb.AppendLine($"case RecordTypeInts.{data.EndMarkerType}: // End Marker");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine($"{ReaderMemberName}.ReadSubrecord();");
                            sb.AppendLine($"return {nameof(ParseResult)}.Stop;");
                        }
                    }
                    sb.AppendLine($"default:");
                    using (sb.IncreaseDepth())
                    {
                        bool first = true;
                        // Generic options 
                        foreach (var field in obj.IterateFieldIndices())
                        {
                            var fieldData = field.Field.GetFieldData();
                            if (!fieldData.HasTrigger
                                || fieldData.TriggeringRecordTypes.Count > 0
                                || fieldData.GenerationTypes.Count() > 0) continue;
                            if (field.Field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
                            if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                            {
                                throw new ArgumentException("Unsupported type generator: " + field.Field);
                            }

                            if (generator.ShouldGenerateCopyIn(field.Field))
                            {
                                using (var args = sb.If(ands: true, first: first))
                                {
                                    foreach (var trigger in fieldData.TriggeringRecordAccessors)
                                    {
                                        args.Add($"nextRecordType.Equals({trigger})");
                                    }
                                }
                                first = false;
                                using (sb.CurlyBrace())
                                {
                                    await GenerateFillSnippet(obj, sb, field.Field, generator, ReaderMemberName);
                                    sb.AppendLine($"return {nameof(ParseResult)}.Stop;");
                                }
                            }
                        }

                        // Default case 
                        if (obj.GetObjectData().CustomRecordFallback)
                        {
                            using (var args = sb.Call(
                                       $"return CustomRecordFallback"))
                            {
                                args.AddPassArg($"item");
                                args.AddPassArg(ReaderMemberName);
                                if (obj.GetObjectType() == ObjectType.Subrecord
                                    || obj.BaseClass.GetObjectType() == ObjectType.Record)
                                {
                                    args.AddPassArg($"lastParsed");
                                }
                                args.AddPassArg("recordParseCount");
                                args.AddPassArg("nextRecordType");
                                args.AddPassArg("contentLength");
                                args.AddPassArg($"translationParams");
                            }
                        }
                        else if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any((b) => HasRecordTypeFields(b)))
                        {
                            using (var args = sb.Call(
                                       $"return {Loqui.Generation.Utility.Await(HasAsyncRecords(obj, self: false))}{TranslationCreateClass(obj.BaseClass)}.Fill{ModuleNickname}RecordTypes"))
                            {
                                args.AddPassArg("item");
                                args.AddPassArg(ReaderMemberName);
                                if (obj.BaseClass.GetObjectType() == ObjectType.Subrecord
                                    || obj.BaseClass.GetObjectType() == ObjectType.Record)
                                {
                                    args.AddPassArg($"lastParsed");
                                }
                                if (obj.GetObjectType() != ObjectType.Mod)
                                {
                                    args.AddPassArg("recordParseCount");
                                }
                                args.AddPassArg("nextRecordType");
                                args.AddPassArg("contentLength");
                                if (data.BaseRecordTypeConverter?.FromConversions.Count > 0)
                                {
                                    args.Add($"translationParams: translationParams.With({obj.RegistrationName}.BaseConverter)");
                                }
                            }
                        }
                        else
                        {
                            var failOnUnknown = obj.GetObjectData().FailOnUnknown;
                            if (mutaObjType == ObjectType.Subrecord)
                            {
                                sb.AppendLine($"return {nameof(ParseResult)}.Stop;");
                            }
                            else if (failOnUnknown)
                            {
                                sb.AppendLine($"throw new ArgumentException($\"Unexpected header {{nextRecordType.Type}} at position {{{ReaderMemberName}.Position}}\");");
                            }
                            else
                            {
                                string addString;
                                switch (obj.GetObjectType())
                                {
                                    case ObjectType.Mod:
                                        addString = null;
                                        break;
                                    case ObjectType.Subrecord:
                                    case ObjectType.Record:
                                        addString = $" + {ReaderMemberName}.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)}";
                                        break;
                                    case ObjectType.Group:
                                        addString = $" + {ReaderMemberName}.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.MajorConstants)}.{nameof(GameConstants.SubConstants.HeaderLength)}";
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                                sb.AppendLine($"{ReaderMemberName}.Position += contentLength{addString};");
                                sb.AppendLine($"return default(int?);");
                            }
                        }
                    }
                }
            }
            sb.AppendLine();
        }

        foreach (var field in obj.Fields)
        {
            if (field.GetFieldData().CustomVersion != null)
            {
                using (var args = sb.Call(
                           $"static partial void {field.Name}CustomVersionParse"))
                {
                    foreach (var (API, Public) in this.MainAPI.ReaderAPI.IterateAPI(
                                 obj,
                                 TranslationDirection.Reader,
                                 new APILine(ItemKey, $"{obj.Interface(getter: false, internalInterface: true)} item"),
                                 new APILine(NextRecordTypeKey, $"{nameof(RecordType)} nextRecordType"),
                                 new APILine(ContentLengthKey, $"int contentLength")))
                    {
                        if (Public)
                        {
                            args.Add(API.Result);
                        }
                    }
                }
                sb.AppendLine();
            }
        }
    }

    private static void AttachOverflowCases(StructuredStringBuilder sb, List<(int, int, TypeGeneration Field)> fields, Accessor streamAccessor)
    {
        var overflowGroups = fields.Select(x => x.Field)
            .Where(x => x.GetFieldData().OverflowRecordType != null)
            .GroupBy(x => x.GetFieldData().OverflowRecordType.Value)
            .ToArray();
        if (overflowGroups.Length > 0)
        {
            foreach (var overflows in overflowGroups)
            {
                sb.AppendLine($"case RecordTypeInts.{overflows.Key}:");
            }

            using (sb.CurlyBrace())
            {
                sb.AppendLine($"var overflowHeader = {streamAccessor}.ReadSubrecord();");
                sb.AppendLine(
                    $"return {nameof(ParseResult)}.{nameof(ParseResult.OverrideLength)}(BinaryPrimitives.ReadUInt32LittleEndian(overflowHeader.Content));");
            }
        }
    }

    protected override async Task GenerateFillSnippet(ObjectGeneration obj, StructuredStringBuilder sb, TypeGeneration field, BinaryTranslationGeneration generator, string frameAccessor)
    {
        var fieldData = field.GetFieldData();
        if (field is DataType set)
        {
            sb.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
            sb.AppendLine($"var dataFrame = {frameAccessor}.SpawnWithLength(contentLength);");
            if (set.Nullable)
            {
                sb.AppendLine($"if (!dataFrame.Complete)");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"item.{set.StateName} = {obj.ObjectName}.{set.EnumName}.Has;");
                }
            }
            bool isInRange = false;
            foreach (var subField in set.IterateFieldsWithMeta())
            {
                if (!this.TryGetTypeGeneration(subField.Field.GetType(), out var subGenerator))
                {
                    throw new ArgumentException("Unsupported type generator: " + subField.Field);
                }

                if (!subGenerator.ShouldGenerateCopyIn(subField.Field)) continue;
                if (subField.BreakIndex != -1)
                {
                    sb.AppendLine($"if (dataFrame.Complete)");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine($"item.{set.StateName} |= {obj.ObjectName}.{set.EnumName}.Break{subField.BreakIndex};");
                        string enumName = null;
                        for (int i = subField.FieldIndex - 1; i >= 0; i--)
                        {
                            if (!set.SubFields.TryGet(i, out var prevField)) continue;
                            if (!prevField.IntegrateField) continue;
                            enumName = prevField.IndexEnumName;
                            break;
                        }
                        if (enumName != null)
                        {
                            enumName = $"(int){enumName}";
                        }
                        sb.AppendLine($"return {enumName ?? "default(int?)"};");
                    }
                }
                if (subField.Range != null && !isInRange)
                {
                    isInRange = true;
                    sb.AppendLine($"if (dataFrame.TotalLength > {subField.Range.DataSetSizeMin})");
                    sb.AppendLine("{");
                    sb.Depth++;
                    sb.AppendLine($"item.{set.StateName} |= {obj.Name}.{set.EnumName}.Range{subField.RangeIndex};");
                }
                if (subField.Range == null && isInRange)
                {
                    isInRange = false;
                    sb.Depth--;
                    sb.AppendLine("}");
                }
                await GenerateFillSnippet(obj, sb, subField.Field, subGenerator, "dataFrame");
            }
            if (isInRange)
            {
                isInRange = false;
                sb.AppendLine("}");
                sb.Depth--;
            }
            return;
        }

        var data = field.GetFieldData();
        switch (data.Binary)
        {
            case BinaryGenerationType.Normal:
                break;
            case BinaryGenerationType.NoGeneration:
                return;
            case BinaryGenerationType.Custom:
                CustomLogic.GenerateFill(
                    sb,
                    obj,
                    field,
                    frameAccessor,
                    isAsync: false,
                    useReturnValue: false);
                return;
            default:
                throw new NotImplementedException();
        }

        if (data.CustomVersion != null)
        {
            sb.AppendLine($"if (item.FormVersion <= {data.CustomVersion})");
            using (sb.CurlyBrace())
            {
                await GenerateLastParsedShortCircuit(
                    obj: obj,
                    sb: sb,
                    field: field.GetIndexData(),
                    lastRequiredIndex: obj.GetObjectData().GetLastRequiredFieldIndexToUse(),
                    doublesPotential: false,
                    nextRecAccessor: "nextRecordType",
                    toDo: async () =>
                    {
                        using (var args = sb.Call(
                                   $"{field.Name}CustomVersionParse"))
                        {
                            args.AddPassArg($"item");
                            args.AddPassArg(ReaderMemberName);
                            args.AddPassArg($"nextRecordType");
                            args.AddPassArg($"contentLength");
                            args.AddPassArg($"recordTypeConverter");
                        }
                    });
            }
        }

        if (data.MarkerType != null && data.RecordType != null)
        {
            // Skip marker 
            sb.AppendLine($"{ReaderMemberName}.Position += {ReaderMemberName}.MetaData.SubConstants.HeaderLength + contentLength;");
            // read in target record type. 
            sb.AppendLine($"var nextRec = {ReaderMemberName}.MetaData.GetSubrecord({ReaderMemberName});");
            // Return if it's not there 
            sb.AppendLine($"if (nextRec.RecordType != {obj.RecordTypeHeaderName(data.RecordType.Value)}) throw new ArgumentException(\"Marker was read but not followed by expected subrecord.\");");
            sb.AppendLine("contentLength = nextRec.RecordLength;");
        }

        if (data.HasVersioning)
        {
            sb.AppendLine($"if ({VersioningModule.GetVersionIfCheck(data, $"{ReaderMemberName}.MetaData.FormVersion!.Value")})");
        }
        using (sb.CurlyBrace(doIt: data.HasVersioning))
        {
            await generator.GenerateCopyIn(
                sb: sb,
                objGen: obj,
                typeGen: field,
                readerAccessor: frameAccessor,
                itemAccessor: Accessor.FromType(field, "item"),
                translationAccessor: null,
                errorMaskAccessor: null);
        }
    }
         
    public override async Task GenerateInVoid(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        await base.GenerateInVoid(obj, sb);
        using (sb.Namespace(obj.Namespace, fileScoped: false))
        {
            await GenerateImportWrapper(obj, sb);
        }
    }

    protected bool OverlayRootObject(ObjectGeneration obj)
    {
        return obj.GetObjectType() == ObjectType.Mod;
    }

    private static async Task GenerateLastParsedShortCircuit(
        ObjectGeneration obj,
        StructuredStringBuilder sb,
        (int PublicIndex, int InternalIndex, TypeGeneration Field) field,
        bool doublesPotential,
        int? lastRequiredIndex,
        Accessor nextRecAccessor,
        Func<Task> toDo)
    {
        var fieldData = field.Field.GetFieldData();
        var dataSet = field.Field as DataType;
        var typelessStruct = obj.IsTypelessStruct();
        if (typelessStruct && field.Field.GetFieldData().IsTriggerForObject)
        {
            var lastReqField = lastRequiredIndex == null ? null : obj.Fields[lastRequiredIndex.Value];
            if (field.PublicIndex >= lastRequiredIndex)
            {
                lastReqField = null;
            }
            if (dataSet != null)
            {
                sb.AppendLine($"if (lastParsed.{nameof(PreviousParse.ParsedIndex)}.HasValue && lastParsed.{nameof(PreviousParse.ParsedIndex)}.Value >= (int){dataSet.SubFields.Last().IndexEnumName}) return {nameof(ParseResult)}.Stop;");
            }
            else if (field.Field is CustomLogic)
            {
                var objFields = obj.IterateFieldIndices(nonIntegrated: false).ToList();
                var nextField = objFields.FirstOrDefault((i) => i.InternalIndex > field.InternalIndex);
                var prevField = objFields.LastOrDefault((i) => i.InternalIndex < field.InternalIndex);
                var enumName = lastReqField?.IndexEnumName ?? nextField.Field?.IndexEnumName ?? prevField.Field?.IndexEnumName;
                if (enumName != null)
                {
                    sb.AppendLine($"if (lastParsed.{nameof(PreviousParse.ParsedIndex)}.HasValue && lastParsed.{nameof(PreviousParse.ParsedIndex)}.Value >= (int){enumName}) return {nameof(ParseResult)}.Stop;");
                }
            }
            else if (field.Field is not MarkerType)
            {
                sb.AppendLine($"if (lastParsed.{nameof(PreviousParse.ParsedIndex)}.HasValue && lastParsed.{nameof(PreviousParse.ParsedIndex)}.Value >= (int){lastReqField?.IndexEnumName ?? field.Field.IndexEnumName}) return {nameof(ParseResult)}.Stop;");
            }
        }
        await toDo();
        if (dataSet != null)
        {
            sb.AppendLine($"return (int){dataSet.SubFields.Last(f => f.IntegrateField && f.Enabled).IndexEnumName};");
        }
        else if (field.Field is CustomLogic)
        {
            if (!fieldData.HasTrigger)
            {
                sb.AppendLine($"return {(typelessStruct ? "lastParsed" : "null")};");
            }
        }
        else if (field.Field is MarkerType marker)
        {
            if (marker.EndMarker)
            {
                sb.AppendLine($"return {nameof(ParseResult)}.{nameof(ParseResult.Stop)};");
            }
            else
            {
                if (doublesPotential)
                {
                    sb.AppendLine($"return new {nameof(ParseResult)}(default(int?), {nextRecAccessor});");
                }
                else
                {
                    sb.AppendLine($"return default(int?);");
                }
            }
        }
        else
        {
            if (doublesPotential)
            {
                sb.AppendLine($"return new {nameof(ParseResult)}((int){field.Field.IndexEnumName}, {nextRecAccessor});");
            }
            else
            {
                sb.AppendLine($"return (int){field.Field.IndexEnumName};");
            }
        }
    }

    protected async Task GenerateOverlayExtras(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (HasRecordTypeFields(obj))
        {
            using (var args = sb.Function(
                       $"public{await obj.FunctionOverride(async b => HasRecordTypeFields(b))}{nameof(ParseResult)} FillRecordType"))
            {
                args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? nameof(IBinaryReadStream) : nameof(OverlayStream))} stream");
                args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "long" : "int")} finalPos");
                args.Add($"int offset");
                args.Add("RecordType type");
                args.Add($"{nameof(PreviousParse)} lastParsed");
                if (obj.GetObjectType() != ObjectType.Mod)
                {
                    args.Add("Dictionary<RecordType, int>? recordParseCount");
                }
                args.Add($"{nameof(TypedParseParams)}? parseParams = null");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"type = parseParams.ConvertToStandard(type);");
                sb.AppendLine("switch (type.TypeInt)");
                using (sb.CurlyBrace())
                {
                    var fields = new List<(int, int, TypeGeneration Field)>();
                    foreach (var field in obj.IterateFieldIndices(
                                 expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                                 nonIntegrated: true))
                    {
                        var fieldData = field.Field.GetFieldData();
                        if (!fieldData.HasTrigger
                            || fieldData.GenerationTypes.Count() == 0) continue;
                        if (fieldData.BinaryOverlayFallback == BinaryGenerationType.NoGeneration) continue;
                        if (field.Field.Derivative && fieldData.BinaryOverlayFallback != BinaryGenerationType.Custom) continue;
                        if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field.Field);
                        }

                        if (!generator.ShouldGenerateCopyIn(field.Field)) continue;
                        fields.Add(field);
                    }

                    var doubleUsages = new Dictionary<RecordType, List<(int, int, TypeGeneration)>>();
                    foreach (var field in fields)
                    {
                        var fieldData = field.Field.GetFieldData();
                        if (fieldData.GenerationTypes.Count() > 1) continue;
                        foreach (var gen in fieldData.GenerationTypes)
                        {
                            if (gen.Key.Count() > 1) continue;
                            LoquiType loqui = gen.Value as LoquiType;
                            if (loqui?.TargetObjectGeneration?.Abstract ?? false) continue;
                            doubleUsages.GetOrAdd(gen.Key.First()).Add(field);
                        }
                    }
                    foreach (var item in doubleUsages.ToList())
                    {
                        if (item.Value.Count <= 1)
                        {
                            doubleUsages.Remove(item.Key);
                        }
                    }

                    foreach (var field in fields)
                    {
                        var fieldData = field.Field.GetFieldData();
                        if (!this.TryGetTypeGeneration(field.Field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field.Field);
                        }
                        foreach (var gen in fieldData.GenerationTypes)
                        {
                            LoquiType loqui = gen.Value as LoquiType;
                            if (loqui?.TargetObjectGeneration?.Abstract ?? false) continue;

                            List<(int, int, TypeGeneration Field)> doubles = null;
                            if (gen.Key.Count() == 1)
                            {
                                if (doubleUsages.TryGetValue(gen.Key.First(), out doubles))
                                {
                                    // Means we handled earlier, break out 
                                    if (doubles.Count == 0) continue;
                                }
                            }

                            foreach (var trigger in gen.Key)
                            {
                                sb.AppendLine($"case RecordTypeInts.{trigger.CheckedType}:");
                            }
                            using (sb.CurlyBrace())
                            {
                                if (doubles == null)
                                {
                                    await GenerateLastParsedShortCircuit(
                                        obj: obj,
                                        sb: sb,
                                        field: field,
                                        doublesPotential: false,
                                        lastRequiredIndex: obj.GetObjectData().GetLastRequiredFieldIndexToUse(),
                                        nextRecAccessor: "type",
                                        toDo: async () =>
                                        {
                                            string recConverter = "parseParams";
                                            if (fieldData?.RecordTypeConverter != null
                                                && fieldData.RecordTypeConverter.FromConversions.Count > 0)
                                            {
                                                recConverter = $"{obj.RegistrationName}.{field.Field.Name}Converter";
                                            }
                                            await generator.GenerateWrapperRecordTypeParse(
                                                sb: sb,
                                                objGen: obj,
                                                typeGen: gen.Value,
                                                locationAccessor: "(stream.Position - offset)",
                                                packageAccessor: "_package",
                                                converterAccessor: recConverter);
                                            if (obj.GetObjectType() == ObjectType.Mod
                                                && field.Field.Name == "ModHeader")
                                            {
                                                using (var args = sb.Call(
                                                           $"_package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.MasterReferences)}!.SetTo"))
                                                {
                                                    args.Add(subFg =>
                                                    {
                                                        subFg.AppendLine("this.ModHeader.MasterReferences.Select(");
                                                        using (subFg.IncreaseDepth())
                                                        {
                                                            subFg.AppendLine($"master => new {nameof(MasterReference)}()");
                                                            using (subFg.CurlyBrace(appendParenthesis: true))
                                                            {
                                                                subFg.AppendLine("Master = master.Master,");
                                                                subFg.AppendLine("FileSize = master.FileSize,");
                                                            }
                                                        }
                                                    });
                                                }
                                            }
                                        });
                                }
                                else
                                {
                                    sb.AppendLine($"switch (recordParseCount?.GetOrAdd(type) ?? 0)");
                                    using (sb.CurlyBrace())
                                    {
                                        int count = 0;
                                        foreach (var doublesField in doubles)
                                        {
                                            if (!this.TryGetTypeGeneration(doublesField.Field.GetType(), out var doubleGen))
                                            {
                                                throw new ArgumentException("Unsupported type generator: " + doublesField.Field);
                                            }
                                            var doublesFieldData = doublesField.Field.GetFieldData();
                                            sb.AppendLine($"case {count++}:");
                                            using (sb.IncreaseDepth())
                                            {
                                                await GenerateLastParsedShortCircuit(
                                                    obj: obj,
                                                    sb: sb,
                                                    field: doublesField,
                                                    doublesPotential: true,
                                                    lastRequiredIndex: obj.GetObjectData().GetLastRequiredFieldIndexToUse(),
                                                    nextRecAccessor: "type",
                                                    toDo: async () =>
                                                    {
                                                        string recConverter = "parseParams";
                                                        if (doublesFieldData.RecordTypeConverter != null
                                                            && doublesFieldData.RecordTypeConverter.FromConversions.Count > 0)
                                                        {
                                                            recConverter = $"{obj.RegistrationName}.{doublesField.Field.Name}Converter";
                                                        }
                                                        await doubleGen.GenerateWrapperRecordTypeParse(
                                                            sb: sb,
                                                            objGen: obj,
                                                            typeGen: doublesField.Field,
                                                            locationAccessor: "(stream.Position - offset)",
                                                            packageAccessor: "_package",
                                                            converterAccessor: recConverter);
                                                        if (obj.GetObjectType() == ObjectType.Mod
                                                            && doublesField.Field.Name == "ModHeader")
                                                        {
                                                            using (var args = sb.Call(
                                                                       $"_package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.MasterReferences)}!.SetTo"))
                                                            {
                                                                args.Add(subFg =>
                                                                {
                                                                    subFg.AppendLine("this.ModHeader.MasterReferences.Select(");
                                                                    using (subFg.IncreaseDepth())
                                                                    {
                                                                        subFg.AppendLine($"master => new {nameof(MasterReference)}()");
                                                                        using (subFg.CurlyBrace(appendParenthesis: true))
                                                                        {
                                                                            subFg.AppendLine("Master = master.Master,");
                                                                            subFg.AppendLine("FileSize = master.FileSize,");
                                                                        }
                                                                    }
                                                                });
                                                            }
                                                        }
                                                    });
                                            }
                                        }
                                        sb.AppendLine($"default:");
                                        using (sb.IncreaseDepth())
                                        {
                                            sb.AppendLine($"throw new NotImplementedException();");
                                        }
                                    }
                                    doubles.Clear();
                                }
                            }
                        }
                    }
                        
                    AttachOverflowCases(sb, fields, "stream");
                        
                    var endMarkerType = obj.GetObjectData().EndMarkerType;
                    if (endMarkerType.HasValue)
                    {
                        sb.AppendLine($"case RecordTypeInts.{endMarkerType}: // End Marker");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine($"stream.ReadSubrecord();");
                            sb.AppendLine($"return {nameof(ParseResult)}.Stop;");
                        }
                    }
                    sb.AppendLine("default:");
                    using (sb.IncreaseDepth())
                    {
                        if (obj.GetObjectData().CustomRecordFallback)
                        {
                            using (var args = sb.Call(
                                       $"return CustomRecordFallback"))
                            {
                                args.AddPassArg("stream");
                                args.AddPassArg("finalPos");
                                args.AddPassArg("offset");
                                args.AddPassArg("type");
                                args.AddPassArg("lastParsed");
                                if (obj.GetObjectData().BaseRecordTypeConverter?.FromConversions.Count > 0)
                                {
                                    args.Add($"parseParams: {obj.RegistrationName}.BaseConverter");
                                }
                            }
                        }
                        else if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b)))
                        {
                            using (var args = sb.Call(
                                       "return base.FillRecordType"))
                            {
                                args.AddPassArg("stream");
                                args.AddPassArg("finalPos");
                                args.AddPassArg("offset");
                                args.AddPassArg("type");
                                args.AddPassArg("lastParsed");
                                args.AddPassArg("recordParseCount");
                                if (obj.GetObjectData().BaseRecordTypeConverter?.FromConversions.Count > 0)
                                {
                                    args.Add($"parseParams: {obj.RegistrationName}.BaseConverter");
                                }
                            }
                        }
                        else
                        {
                            var failOnUnknown = obj.GetObjectData().FailOnUnknown;
                            if (obj.GetObjectType() == ObjectType.Subrecord)
                            {
                                sb.AppendLine($"return {nameof(ParseResult)}.Stop;");
                            }
                            else if (failOnUnknown)
                            {
                                sb.AppendLine("throw new ArgumentException($\"Unexpected header {nextRecordType.Type} at position {frame.Position}\");");
                            }
                            else
                            {
                                sb.AppendLine($"return default(int?);");
                            }
                        }
                    }
                }
            }
        }
    }

    protected override async Task GenerateCopyInSnippet(ObjectGeneration obj, StructuredStringBuilder sb, Accessor accessor)
    {
        var data = obj.GetObjectData();

        bool typelessStruct = obj.IsTypelessStruct();
        ObjectType objType = obj.GetObjectType();

        if (await obj.IsMajorRecord())
        {
            bool async = this.HasAsync(obj, self: true);
            using (var args = sb.Call(
                       $"{nameof(PluginUtilityTranslation)}.MajorRecordParse<{obj.Interface(getter: false, internalInterface: true)}>"))
            {
                args.Add($"record: {accessor}");
                args.AddPassArg(ReaderMemberName);
                args.AddPassArg($"translationParams");
                args.Add($"fillStructs: {TranslationCreateClass(obj)}.FillBinaryStructs");
                args.Add($"fillTyped: {TranslationCreateClass(obj)}.FillBinaryRecordTypes");
            }
            if (data.CustomBinaryEnd != CustomEnd.Off)
            {
                using (var args = sb.Call(
                           $"{Loqui.Generation.Utility.Await(data.CustomBinaryEnd == CustomEnd.Async)}{this.TranslationCreateClass(obj)}.CustomBinaryEndImport{(await this.AsyncImport(obj) ? null : "Public")}"))
                {
                    args.AddPassArg(ReaderMemberName);
                    args.Add($"obj: {accessor}");
                }
            }
        }
        else
        {
            IEnumerable<RecordType> recordTypes = await obj.GetTriggeringRecordTypes();
            var frameMod = (objType != ObjectType.Subrecord || recordTypes.Any())
                           && objType != ObjectType.Mod;
            if (frameMod)
            {
                switch (objType)
                {
                    case ObjectType.Subrecord:
                        if (obj.TryGetRecordType(out var recType))
                        {
                            if (obj.Fields.Any(f => f.GetFieldData().HasTrigger))
                            {
                                sb.AppendLine($"{ReaderMemberName}.Position += {ReaderMemberName}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength;");
                            }
                            else
                            {
                                using (var args = sb.Call(
                                           $"{ReaderMemberName} = {ReaderMemberName}.SpawnWithFinalPosition({nameof(HeaderTranslation)}.ParseSubrecord",
                                           suffixLine: ")"))
                                {
                                    args.Add($"{ReaderMemberName}.Reader");
                                    args.Add(GetRecordTypeString(
                                        obj, 
                                        "translationParams",
                                        $"{ReaderMemberName}.MetaData.Constants.Release",
                                        $"{ReaderMemberName}.MetaData.FormVersion"));
                                    args.Add("translationParams?.LengthOverride");
                                }
                            }
                        }
                        break;
                    case ObjectType.Record:
                        using (var args = sb.Call(
                                   $"{ReaderMemberName} = {ReaderMemberName}.SpawnWithFinalPosition({nameof(HeaderTranslation)}.ParseRecord",
                                   suffixLine: ")"))
                        {
                            args.Add($"{ReaderMemberName}.Reader");
                            args.Add(GetRecordTypeString(
                                obj, 
                                "translationParams",
                                $"{ReaderMemberName}.MetaData.Constants.Release", 
                                $"{ReaderMemberName}.MetaData.FormVersion"));
                        }
                        break;
                    case ObjectType.Group:
                        break;
                    case ObjectType.Mod:
                    default:
                        throw new NotImplementedException();
                }
            }
            bool async = this.HasAsync(obj, self: true);
            var utilityTranslation = nameof(PluginUtilityTranslation);
            switch (objType)
            {
                case ObjectType.Subrecord:
                    using (var args = sb.Call(
                               $"{utilityTranslation}.SubrecordParse",
                               suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                    {
                        args.Add($"record: {accessor}");
                        args.AddPassArg("frame");
                        args.AddPassArg("translationParams");
                        args.Add($"fillStructs: {TranslationCreateClass(obj)}.Fill{ModuleNickname}Structs");
                        if (HasRecordTypeFields(obj)
                            || (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b))))
                        {
                            args.Add($"fillTyped: {TranslationCreateClass(obj)}.Fill{ModuleNickname}RecordTypes");
                        }
                    }
                    break;
                case ObjectType.Record:
                    using (var args = sb.Call(
                               $"{utilityTranslation}.RecordParse",
                               suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                    {
                        args.Add($"record: {accessor}");
                        args.AddPassArg(ReaderMemberName);
                        args.AddPassArg("translationParams");
                        args.Add($"fillStructs: {TranslationCreateClass(obj)}.Fill{ModuleNickname}Structs");
                        if (HasRecordTypeFields(obj)
                            || (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b))))
                        {
                            args.Add($"fillTyped: {TranslationCreateClass(obj)}.Fill{ModuleNickname}RecordTypes");
                        }
                    }
                    break;
                case ObjectType.Group:
                    using (var args = sb.Call(
                               $"{utilityTranslation}.GroupParse",
                               suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                    {
                        args.Add($"record: {accessor}");
                        args.AddPassArg(ReaderMemberName);
                        args.AddPassArg($"translationParams");
                        args.Add($"fillStructs: {TranslationCreateClass(obj)}.Fill{ModuleNickname}Structs");
                        if (HasRecordTypeFields(obj)
                            || (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b))))
                        {
                            args.Add($"fillTyped: {TranslationCreateClass(obj)}.Fill{ModuleNickname}RecordTypes");
                        }
                    }
                    break;
                case ObjectType.Mod:
                    using (var args = sb.Call(
                               $"{utilityTranslation}.ModParse",
                               suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                    {
                        args.Add($"record: {accessor}");
                        args.AddPassArg(ReaderMemberName);
                        args.AddPassArg("importMask");
                        args.Add($"fillStructs: {TranslationCreateClass(obj)}.Fill{ModuleNickname}Structs");
                        if (HasRecordTypeFields(obj))
                        {
                            args.Add($"fillTyped: {TranslationCreateClass(obj)}.Fill{ModuleNickname}RecordTypes");
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            GenerateStructStateSubscriptions(obj, sb);
            if (data.CustomBinaryEnd != CustomEnd.Off)
            {
                using (var args = sb.Call(
                           $"{Loqui.Generation.Utility.Await(data.CustomBinaryEnd == CustomEnd.Async)}{this.TranslationCreateClass(obj)}.CustomBinaryEndImportPublic"))
                {
                    args.AddPassArg(ReaderMemberName);
                    args.Add($"obj: {accessor}");
                }
            }
        }
    }

    private void GenerateStructStateSubscriptions(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (!obj.StructNullable()) return;
        List<TypeGeneration> affectedFields = new List<TypeGeneration>();
        foreach (var field in obj.IterateFields())
        {
            var data = field.GetFieldData();
            if (data.HasTrigger) break;
            if (field.Nullable)
            {
                affectedFields.Add(field);
                continue;
            }
        }
    }

    public override void CustomMainWriteMixInPreLoad(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (obj.GetObjectType() != ObjectType.Mod) return;
        sb.AppendLine("var modKey = item.ModKey;");
    }

    protected async Task GenerateImportWrapper(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        var objData = obj.GetObjectData();
        if (objData.BinaryOverlay != BinaryGenerationType.Normal) return;

        var dataAccessor = new Accessor("_data");
        var packageAccessor = new Accessor("_package");
        var metaAccessor = new Accessor($"_package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}");
        var needsMasters = await obj.GetNeedsMasters();
        var anyHasRecordTypes = (await obj.EntireClassTree()).Any(c => HasRecordTypeFields(c));

        if (obj.GetObjectType() == ObjectType.Mod)
        {
            sb.AppendLine("[DebuggerDisplay(\"{GameRelease} {ModKey.ToString()}\")]");
        }
        using (var args = sb.Class($"{BinaryOverlayClass(obj)}"))
        {
            args.AccessModifier = AccessModifier.Internal;
            args.Abstract = obj.Abstract;
            args.Partial = true;
            var block = obj.GetObjectType() == ObjectType.Mod
                        || (obj.GetObjectType() == ObjectType.Group && obj.Generics.Count > 0);
            args.BaseClass = obj.HasLoquiBaseObject ? BinaryOverlayClass(obj.BaseClass) : (block ? null : nameof(PluginBinaryOverlay));
            if (obj.GetObjectType() == ObjectType.Mod)
            {
                args.Interfaces.Add($"I{obj.Name}DisposableGetter");
            }
            else
            {
                args.Interfaces.Add(obj.Interface(getter: true, internalInterface: true));
            }
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
        }
        using (sb.CurlyBrace())
        {
            await obj.GenerateRouting(sb, getterOnly: true);
            obj.GenerateGetterInterfaceImplementations(sb);
            if (obj.GetObjectType() == ObjectType.Mod)
            {
                if (objData.GameReleaseOptions != null)
                {
                    sb.AppendLine($"public {ModModule.ReleaseEnumName(obj)} {ModModule.ReleaseEnumName(obj)} {{ get; }}");
                    sb.AppendLine($"public {nameof(GameRelease)} GameRelease => {ModModule.ReleaseEnumName(obj)}.ToGameRelease();");
                }
                else
                {
                    sb.AppendLine($"public {nameof(GameRelease)} GameRelease => {nameof(GameRelease)}.{obj.GetObjectData().GameCategory};");
                }
                sb.AppendLine($"IGroupGetter<T> {nameof(IModGetter)}.{nameof(IModGetter.GetTopLevelGroup)}<T>() => this.{nameof(IModGetter.GetTopLevelGroup)}<T>();");
                sb.AppendLine($"IGroupGetter {nameof(IModGetter)}.{nameof(IModGetter.GetTopLevelGroup)}(Type type) => this.{nameof(IModGetter.GetTopLevelGroup)}(type);");
                sb.AppendLine($"void IModGetter.WriteToBinary({nameof(FilePath)} path, {nameof(BinaryWriteParameters)}? param, IFileSystem? fileSystem) => this.WriteToBinary(path, importMask: null, param: param, fileSystem: fileSystem);");
                sb.AppendLine($"void IModGetter.WriteToBinaryParallel({nameof(FilePath)} path, {nameof(BinaryWriteParameters)}? param, IFileSystem? fileSystem, {nameof(ParallelWriteParameters)}? parallelWriteParams) => this.WriteToBinaryParallel(path, param: param, fileSystem: fileSystem, parallelParam: parallelWriteParams);");
                sb.AppendLine($"IReadOnlyList<{nameof(IMasterReferenceGetter)}> {nameof(IModGetter)}.MasterReferences => this.ModHeader.MasterReferences;");
                if (obj.GetObjectData().UsesStringFiles)
                {
                    sb.AppendLine($"public bool CanUseLocalization => true;");
                    sb.AppendLine($"public bool UsingLocalization => this.ModHeader.Flags.HasFlag({obj.GetObjectData().GameCategory}ModHeader.HeaderFlag.Localized);");
                }
                else
                {
                    sb.AppendLine($"public bool CanUseLocalization => false;");
                    sb.AppendLine($"public bool UsingLocalization => false;");
                }

            }

            if (obj.GetObjectType() == ObjectType.Mod
                || (await LinkModule.HasLinks(obj, includeBaseClass: false) != Case.No))
            {
                await LinkModule.GenerateInterfaceImplementation(obj, sb, getter: true);
            }

            if (obj.GetObjectType() == ObjectType.Mod)
            {
                MajorRecordContextEnumerationModule.GenerateClassImplementation(obj, sb, onlyGetter: true);
            }

            if (await MajorRecordModule.HasMajorRecordsInTree(obj, includeBaseClass: false) != Case.No)
            {
                MajorRecordEnumerationModule.GenerateClassImplementation(obj, sb, onlyGetter: true);
            }

            foreach (var transl in obj.ProtoGen.Gen.GenerationModules
                         .WhereCastable<IGenerationModule, ITranslationModule>())
            {
                if (transl.DoTranslationInterface(obj))
                {
                    await transl.GenerateTranslationInterfaceImplementation(obj, sb);
                }
            }

            if (obj.GetObjectType() == ObjectType.Mod)
            {
                sb.AppendLine($"uint IModGetter.NextFormID => ModHeader.Stats.NextFormID;");
                sb.AppendLine($"public {nameof(ModKey)} ModKey {{ get; }}");
                sb.AppendLine($"private readonly {nameof(BinaryOverlayFactoryPackage)} _package;");
                sb.AppendLine($"private readonly {nameof(IBinaryReadStream)} _data;");
                sb.AppendLine($"private readonly bool _shouldDispose;");

                sb.AppendLine("public void Dispose()");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("if (!_shouldDispose) return;");
                    sb.AppendLine("_data.Dispose();");
                }
            }

            if (await obj.IsMajorRecord() && !obj.Abstract)
            {
                sb.AppendLine($"protected override Type LinkType => typeof({obj.Interface(getter: false)});");
                sb.AppendLine();
            }

            if (obj.GetObjectData().MajorRecordFlags)
            {
                sb.AppendLine($"public {obj.ObjectName}.MajorFlag MajorFlags => ({obj.ObjectName}.MajorFlag)this.MajorRecordFlagsRaw;");
            }

            sb.AppendLine();
            if (obj.Fields.Any(f => f is BreakType))
            {
                sb.AppendLine($"public {obj.ObjectName}.{VersioningModule.VersioningEnumName} {VersioningModule.VersioningFieldName} {{ get; private set; }}");
            }

            int? totalPassedLength = 0;
            TypeGeneration lastVersionedField = null;
            await foreach (var lengths in IteratePassedLengths(obj, forOverlay: true))
            {
                if (totalPassedLength != null)
                {
                    if (lengths.CurLength == null)
                    {
                        totalPassedLength = null;
                    }
                    else
                    {
                        totalPassedLength = lengths.CurLength;
                    }
                }
                if (!this.TryGetTypeGeneration(lengths.Field.GetType(), out var typeGen))
                {
                    if (!lengths.Field.IntegrateField) continue;
                    throw new NotImplementedException();
                }
                using (sb.Region(lengths.Field.Name, appendExtraLine: false, skipIfOnlyOneLine: true))
                {
                    var data = lengths.Field.GetFieldData();
                    switch (data.BinaryOverlayFallback)
                    {
                        case BinaryGenerationType.NoGeneration:
                            continue;
                        default:
                            break;
                    }
                    await typeGen.GenerateWrapperFields(
                        sb,
                        obj,
                        lengths.Field,
                        dataAccessor,
                        lengths.PassedLength,
                        lengths.PassedAccessor);
                    if (data.HasVersioning
                        && !lengths.Field.Nullable)
                    {
                        VersioningModule.AddVersionOffset(sb, lengths.Field, lengths.FieldLength.Value, lastVersionedField, $"_package.FormVersion!.FormVersion!.Value");
                        lastVersionedField = lengths.Field;
                    }
                    if (!data.HasTrigger)
                    {
                        if (lengths.CurLength == null)
                        {
                            sb.AppendLine($"protected int {lengths.Field.Name}EndingPos;");
                            if (data.BinaryOverlayFallback == BinaryGenerationType.Custom)
                            {
                                sb.AppendLine($"partial void Custom{lengths.Field.Name}EndPos();");
                            }
                        }
                    }
                    foreach (var mod in Gen.GenerationModules)
                    {
                        await mod.GenerateInField(obj, lengths.Field, sb, LoquiInterfaceType.IGetter);
                    }
                }
            }

            if (obj.GetObjectType() != ObjectType.Mod)
            {
                using (var args = sb.Call(
                           $"partial void CustomFactoryEnd"))
                {
                    args.Add($"{nameof(OverlayStream)} stream");
                    args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "long" : "int")} finalPos");
                    args.Add($"int offset");
                }
                if (objData.CustomBinaryEnd != CustomEnd.Off)
                {
                    using (var args = sb.Call(
                               $"partial void CustomEnd"))
                    {
                        args.Add($"{nameof(OverlayStream)} stream");
                        args.Add($"int finalPos");
                        args.Add($"int offset");
                    }
                }
                sb.AppendLine();
                using (var args = sb.Call(
                           $"partial void CustomCtor"))
                {
                }
            }

            using (var args = sb.Function(
                       $"protected {BinaryOverlayClassName(obj)}"))
            {
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    args.Add($"{nameof(IMutagenReadStream)} stream");
                    args.Add("ModKey modKey");
                    args.Add($"bool shouldDispose");
                    if (objData.GameReleaseOptions != null)
                    {
                        args.Add($"{ModModule.ReleaseEnumName(obj)} release");
                    }
                }
                else
                {
                    args.Add($"ReadOnlyMemorySlice<byte> bytes");
                    args.Add($"{nameof(BinaryOverlayFactoryPackage)} package");
                }
            }
            if (obj.GetObjectType() != ObjectType.Mod)
            {
                using (sb.IncreaseDepth())
                {
                    using (var args = sb.Function(
                               ": base"))
                    {
                        args.AddPassArg("bytes");
                        args.AddPassArg($"package");
                    }
                }
            }
            using (sb.CurlyBrace())
            {
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    sb.AppendLine("this.ModKey = modKey;");
                    if (objData.GameReleaseOptions != null)
                    {
                        sb.AppendLine($"this.{ModModule.ReleaseEnumName(obj)} = release;");
                    }
                    sb.AppendLine("this._data = stream;");
                    using (var args = sb.Call(
                               $"this._package = new {nameof(BinaryOverlayFactoryPackage)}"))
                    {
                        args.Add($"stream.{nameof(IMutagenReadStream.MetaData)}");
                    }
                    sb.AppendLine("this._shouldDispose = shouldDispose;");
                }
                foreach (var field in obj.IterateFields(
                             expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                             nonIntegrated: true))
                {
                    if (!this.TryGetTypeGeneration(field.GetType(), out var typeGen)) continue;
                    await typeGen.GenerateWrapperCtor(
                        sb: sb,
                        objGen: obj,
                        typeGen: field);
                }
                if (obj.GetObjectType() != ObjectType.Mod)
                {
                    using (var args = sb.Call(
                               $"this.CustomCtor"))
                    {
                    }
                }
            }
            sb.AppendLine();

            if (!obj.Abstract)
            {
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    string gameReleaseStr;
                    if (obj.GetObjectData().GameReleaseOptions == null)
                    {
                        gameReleaseStr = $"{nameof(GameRelease)}.{obj.GetObjectData().GameCategory}";
                    }
                    else
                    {
                        gameReleaseStr = "release.ToGameRelease()";
                    }

                    using (var args = sb.Function(
                               $"public static {this.BinaryOverlayClass(obj)} {obj.Name}Factory"))
                    {
                        args.Add($"{nameof(ModPath)} path");
                        if (objData.GameReleaseOptions != null)
                        {
                            args.Add($"{ModModule.ReleaseEnumName(obj)} release");
                        }
                        if (objData.UsesStringFiles)
                        {
                            args.Add($"{nameof(StringsReadParameters)}? stringsParam = null");
                        }
                        args.Add("IFileSystem? fileSystem = null");
                    }
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine($"var meta = new {nameof(ParsingBundle)}({gameReleaseStr}, new {nameof(MasterReferenceCollection)}(path.ModKey))");
                        using (sb.CurlyBrace(appendSemiColon: true))
                        {
                            sb.AppendLine($"{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordTypeInfoCacheReader)}(() => new {nameof(MutagenBinaryReadStream)}(path, {gameReleaseStr}))");
                        }
                        using (var args = sb.Call(
                                   $"var stream = new {nameof(MutagenBinaryReadStream)}"))
                        {
                            args.Add($"path: path.{nameof(ModPath.Path)}");
                            args.Add($"metaData: meta");
                            args.AddPassArg("fileSystem");
                        }
                        sb.AppendLine("try");
                        using (sb.CurlyBrace())
                        {
                            if (objData.UsesStringFiles)
                            {
                                sb.AppendLine($"meta.{nameof(ParsingBundle.Absorb)}(stringsParam);");
                                sb.AppendLine("if (stream.Remaining < 12)");
                                using (sb.CurlyBrace())
                                {
                                    sb.AppendLine($"throw new ArgumentException(\"File stream was too short to parse flags\");");
                                }
                                sb.AppendLine($"var flags = stream.GetInt32(offset: 8);");
                                sb.AppendLine($"if (EnumExt.HasFlag(flags, (int)ModHeaderCommonFlag.Localized))");
                                using (sb.CurlyBrace())
                                {
                                    sb.AppendLine($"meta.StringsLookup = StringsFolderLookupOverlay.TypicalFactory({gameReleaseStr}, path.{nameof(ModPath.ModKey)}, Path.GetDirectoryName(path.{nameof(ModPath.Path)})!, stringsParam);");
                                }
                            }

                            using (var args = sb.Call(
                                       $"return {obj.Name}Factory"))
                            {
                                args.AddPassArg("stream");
                                args.Add($"path.{nameof(ModPath.ModKey)}");
                                if (objData.GameReleaseOptions != null)
                                {
                                    args.AddPassArg("release");
                                }
                                args.Add("shouldDispose: true");
                            }
                        }
                        sb.AppendLine("catch (Exception)");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine("stream.Dispose();");
                            sb.AppendLine("throw;");
                        }
                    }
                    sb.AppendLine();
                }

                if (obj.IsTopLevelGroup())
                {
                    using (var args = sb.Function(
                               $"public static {obj.Interface(getter: true)} {obj.Name}Factory"))
                    {
                        args.Add($"{nameof(IBinaryReadStream)} stream");
                        args.Add("IReadOnlyList<RangeInt64> locs");
                        args.Add($"{nameof(BinaryOverlayFactoryPackage)} package");
                    }
                    using (sb.CurlyBrace())
                    {
                        using (var args = sb.Call(
                                   $"var subGroups = locs.Select(x => {obj.ProtoGen.Protocol.Namespace}GroupFactory",
                                   suffixLine: ").ToArray()"))
                        {
                            args.Add("new OverlayStream(LockExtractMemory(stream, x.Min, x.Max), package)");
                            args.Add("package");
                        }
                        using (var args = sb.Call(
                                   $"return new {obj.ProtoGen.Protocol.Namespace}GroupWrapper<T>"))
                        {
                            args.Add($"new GroupMergeGetter<I{obj.ProtoGen.Protocol.Namespace}GroupGetter<T>, T>(subGroups)");
                        }
                    }
                    sb.AppendLine();
                }

                using (var args = sb.Function(
                           $"public static {this.BinaryOverlayClass(obj)} {obj.Name}Factory"))
                {
                    if (obj.GetObjectType() == ObjectType.Mod)
                    {
                        args.Add($"{nameof(IMutagenReadStream)} stream");
                        args.Add("ModKey modKey");
                        if (objData.GameReleaseOptions != null)
                        {
                            args.Add($"{ModModule.ReleaseEnumName(obj)} release");
                        }
                        args.Add("bool shouldDispose");
                    }
                    else
                    {
                        args.Add($"{nameof(OverlayStream)} stream");
                        args.Add($"{nameof(BinaryOverlayFactoryPackage)} package");
                        if (obj.IsVariableLengthStruct())
                        {
                            args.Add($"int finalPos");
                        }
                        args.Add($"{nameof(TypedParseParams)}? parseParams = null");
                    }
                }
                using (sb.CurlyBrace())
                {
                    if (await obj.IsMajorRecord())
                    {
                        if (objData.CustomBinaryEnd != CustomEnd.Off)
                        {
                            sb.AppendLine("var origStream = stream;");
                        }
                        sb.AppendLine($"stream = {nameof(Decompression)}.{nameof(Decompression.DecompressStream)}(stream);");
                    }
                    if (obj.TryGetCustomRecordTypeTriggers(out var customLogicTriggers))
                    {
                        sb.AppendLine($"var nextRecord = parseParams.ConvertToCustom(stream.Get{(obj.GetObjectType() == ObjectType.Subrecord ? "SubrecordHeader" : "MajorRecordHeader")}().RecordType);");
                        sb.AppendLine($"switch (nextRecord.TypeInt)");
                        using (sb.CurlyBrace())
                        {
                            foreach (var item in customLogicTriggers)
                            {
                                sb.AppendLine($"case {item.TypeInt}: // {item.Type}");
                            }
                            using (sb.IncreaseDepth())
                            {
                                using (var args = sb.Call(
                                           "return CustomRecordTypeTrigger"))
                                {
                                    args.AddPassArg($"stream");
                                    args.Add("recordType: nextRecord");
                                    args.AddPassArg("package");
                                    args.AddPassArg("parseParams");
                                }
                            }
                            sb.AppendLine("default:");
                            using (sb.IncreaseDepth())
                            {
                                sb.AppendLine("break;");
                            }
                        }
                    }
                    using (var args = sb.Call(
                               $"var ret = new {BinaryOverlayClassName(obj)}{obj.GetGenericTypes(MaskType.Normal)}"))
                    {
                        if (obj.IsTypelessStruct())
                        {
                            if (anyHasRecordTypes
                                || totalPassedLength == null
                                || totalPassedLength.Value == 0)
                            {
                                args.Add($"bytes: stream.RemainingMemory");
                            }
                            else if (obj.IsVariableLengthStruct())
                            {
                                args.Add($"bytes: stream.RemainingMemory.Slice(0, finalPos - stream.Position)");
                            }
                            else
                            {
                                args.Add($"bytes: stream.RemainingMemory.Slice(0, 0x{totalPassedLength:X})");
                            }
                        }
                        else
                        {
                            switch (obj.GetObjectType())
                            {
                                case ObjectType.Record:
                                    args.Add($"bytes: {nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractRecordMemory)}(stream.RemainingMemory, package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)})");
                                    break;
                                case ObjectType.Group:
                                    args.Add($"bytes: {nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractGroupMemory)}(stream.RemainingMemory, package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)})");
                                    break;
                                case ObjectType.Subrecord:
                                    args.Add($"bytes: {nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordMemory)}(stream.RemainingMemory, package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}, parseParams)");
                                    break;
                                case ObjectType.Mod:
                                    args.AddPassArg($"stream");
                                    if (objData.GameReleaseOptions != null)
                                    {
                                        args.AddPassArg($"release");
                                    }
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        if (obj.GetObjectType() == ObjectType.Mod)
                        {
                            args.AddPassArg("modKey");
                            args.AddPassArg("shouldDispose");
                        }
                        else
                        {
                            args.AddPassArg("package");
                        }
                    }
                    if (obj.IsTypelessStruct())
                    {
                        sb.AppendLine($"int offset = stream.Position;");
                    }
                    else
                    {
                        switch (obj.GetObjectType())
                        {
                            case ObjectType.Subrecord:
                                sb.AppendLine($"var finalPos = checked((int)(stream.Position + stream.GetSubrecordHeader().TotalLength));");
                                sb.AppendLine($"int offset = stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.TypeAndLengthLength;");
                                break;
                            case ObjectType.Record:
                                sb.AppendLine($"var finalPos = checked((int)(stream.Position + stream.GetMajorRecordHeader().TotalLength));");
                                sb.AppendLine($"int offset = stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.MajorConstants.TypeAndLengthLength;");
                                break;
                            case ObjectType.Group:
                                sb.AppendLine($"var finalPos = checked((int)(stream.Position + stream.GetGroupHeader().TotalLength));");
                                sb.AppendLine($"int offset = stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.GroupConstants.TypeAndLengthLength;");
                                break;
                            case ObjectType.Mod:
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }

                    if (await obj.IsMajorRecord())
                    {
                        sb.AppendLine("ret._package.FormVersion = ret;");
                    }

                    // Parse struct section ending positions 
                    string? structPassedAccessor = null;
                    int? structPassedLen = 0;
                    await foreach (var lengths in IteratePassedLengths(
                                       obj,
                                       forOverlay: true,
                                       includeBaseClass: true,
                                       passedLenPrefix: "ret."))
                    {
                        if (!this.TryGetTypeGeneration(lengths.Field.GetType(), out var typeGen)) continue;
                        var data = lengths.Field.GetFieldData();
                        if (lengths.Field is CustomLogic) continue;
                        switch (data.BinaryOverlayFallback)
                        {
                            case BinaryGenerationType.Normal:
                            case BinaryGenerationType.Custom:
                                break;
                            default:
                                continue;
                        }
                        if (!data.HasTrigger)
                        {
                            structPassedLen = lengths.CurLength;
                            structPassedAccessor = lengths.CurAccessor;
                        }
                    }

                    // Parse ending positions  
                    await foreach (var lengths in IteratePassedLengths(obj, forOverlay: true, passedLenPrefix: "ret."))
                    {
                        if (!this.TryGetTypeGeneration(lengths.Field.GetType(), out var typeGen)) continue;
                        var data = lengths.Field.GetFieldData();
                        switch (data.BinaryOverlayFallback)
                        {
                            case BinaryGenerationType.Normal:
                            case BinaryGenerationType.Custom:
                                break;
                            default:
                                continue;
                        }
                        if (data.HasTrigger) continue;
                        var amount = await typeGen.GetPassedAmount(obj, lengths.Field);
                        if (amount != null) continue;
                        if (lengths.Field is CustomLogic) continue;
                        switch (data.BinaryOverlayFallback)
                        {
                            case BinaryGenerationType.Custom:
                                sb.AppendLine($"ret.Custom{lengths.Field.Name}EndPos();");
                                break;
                            case BinaryGenerationType.NoGeneration:
                                break;
                            case BinaryGenerationType.Normal:
                                await typeGen.GenerateWrapperUnknownLengthParse(
                                    sb,
                                    obj,
                                    lengths.Field,
                                    lengths.PassedLength,
                                    lengths.PassedAccessor);
                                break;
                        }
                    }

                    if (anyHasRecordTypes)
                    {
                        if (obj.GetObjectType() != ObjectType.Mod
                            && !obj.IsTypelessStruct())
                        {
                            if (structPassedAccessor != null)
                            {
                                switch (obj.GetObjectType())
                                {
                                    case ObjectType.Subrecord:
                                        sb.AppendLine($"stream.Position += {structPassedAccessor} + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.TypeAndLengthLength;");
                                        break;
                                    case ObjectType.Record:
                                        sb.AppendLine($"stream.Position += {structPassedAccessor} + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.MajorConstants.TypeAndLengthLength;");
                                        break;
                                    case ObjectType.Group:
                                        sb.AppendLine($"stream.Position += {structPassedAccessor} + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.GroupConstants.TypeAndLengthLength;");
                                        break;
                                    case ObjectType.Mod:
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            using (var args = sb.Call(
                                       $"ret.CustomFactoryEnd"))
                            {
                                args.AddPassArg($"stream");
                                args.AddPassArg($"finalPos");
                                args.AddPassArg($"offset");
                            }
                        }

                        string call;
                        switch (obj.GetObjectType())
                        {
                            case ObjectType.Subrecord:
                            case ObjectType.Record:
                                if (obj.IsTypelessStruct())
                                {
                                    call = $"ret.{nameof(PluginBinaryOverlay.FillTypelessSubrecordTypes)}";
                                }
                                else
                                {
                                    call = $"ret.{nameof(PluginBinaryOverlay.FillSubrecordTypes)}";
                                }
                                break;
                            case ObjectType.Group:
                                var grupLoqui = await obj.GetGroupLoquiType();
                                if (grupLoqui.TargetObjectGeneration != null && await grupLoqui.TargetObjectGeneration.IsMajorRecord())
                                {
                                    call = $"ret.{nameof(PluginBinaryOverlay.FillMajorRecords)}";
                                }
                                else
                                {
                                    call = $"ret.{nameof(PluginBinaryOverlay.FillGroupRecordsForWrapper)}";
                                }
                                break;
                            case ObjectType.Mod:
                                call = $"{nameof(PluginBinaryOverlay)}.{nameof(PluginBinaryOverlay.FillModTypes)}";
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        using (var args = sb.Call(
                                   $"{call}"))
                        {
                            if (await obj.IsMajorRecord())
                            {
                                args.Add("majorReference: ret");
                            }
                            args.Add($"stream: stream");
                            if (obj.GetObjectType() != ObjectType.Mod)
                            {
                                if (obj.IsTypelessStruct())
                                {
                                    args.Add($"finalPos: stream.Length");
                                }
                                else
                                {
                                    args.AddPassArg($"finalPos");
                                }
                                args.Add($"offset: offset");
                                args.AddPassArg($"parseParams");
                            }
                            else
                            {
                                args.Add("package: ret._package");
                            }
                            args.Add($"fill: ret.FillRecordType");
                        }
                    }
                    else
                    {

                        if (obj.IsTypelessStruct())
                        {
                            var breaks = obj.Fields.WhereCastable<TypeGeneration, BreakType>().ToList();
                            if (breaks.Count > 0)
                            {
                                int breakIndex = 0;
                                await foreach (var lengths in IteratePassedLengths(obj,
                                                   forOverlay: true,
                                                   includeBaseClass: true,
                                                   passedLenPrefix: "ret."))
                                {
                                    if (lengths.Field is BreakType breakType)
                                    {
                                        sb.AppendLine($"if (ret._data.Length <= {lengths.PassedAccessor})");
                                        using (sb.CurlyBrace())
                                        {
                                            sb.AppendLine($"ret.{VersioningModule.VersioningFieldName} |= {obj.ObjectName}.{VersioningModule.VersioningEnumName}.Break{breakIndex++};");
                                        }
                                    }
                                }
                                // Not advancing stream position, but only because breaks only occur in situations 
                                // that stream position doesn't matter 
                            }
                            else if (structPassedAccessor != null)
                            {
                                sb.AppendLine($"stream.Position += {structPassedAccessor};");
                            }
                        }
                        else
                        {
                            string headerAddition = null;
                            switch (obj.GetObjectType())
                            {
                                case ObjectType.Record:
                                    headerAddition = $" + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.MajorConstants.HeaderLength";
                                    break;
                                case ObjectType.Group:
                                    headerAddition = $" + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.GroupConstants.HeaderLength";
                                    break;
                                case ObjectType.Subrecord:
                                    headerAddition = $" + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength";
                                    break;
                                case ObjectType.Mod:
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                            var breaks = obj.Fields.WhereCastable<TypeGeneration, BreakType>().ToList();
                            if (breaks.Count > 0)
                            {
                                int breakIndex = 0;
                                await foreach (var lengths in IteratePassedLengths(obj,
                                                   forOverlay: true,
                                                   includeBaseClass: true,
                                                   passedLenPrefix: "ret."))
                                {
                                    if (lengths.Field is BreakType breakType)
                                    {
                                        sb.AppendLine($"if (ret._data.Length <= {lengths.PassedAccessor})");
                                        using (sb.CurlyBrace())
                                        {
                                            sb.AppendLine($"ret.{VersioningModule.VersioningFieldName} |= {obj.ObjectName}.{VersioningModule.VersioningEnumName}.Break{breakIndex++};");
                                        }
                                    }
                                }
                                // Not advancing stream position, but only because breaks only occur in situations 
                                // that stream position doesn't matter 
                            }
                            else if (totalPassedLength != null)
                            {
                                sb.AppendLine($"stream.Position += 0x{totalPassedLength.Value:X}{headerAddition};");
                            }
                        }
                        using (var args = sb.Call(
                                   $"ret.CustomFactoryEnd"))
                        {
                            args.AddPassArg($"stream");
                            args.Add($"finalPos: stream.Length");
                            args.AddPassArg($"offset");
                        }
                    }

                    // Parse ending positions  
                    await foreach (var lengths in IteratePassedLengths(obj, forOverlay: true, passedLenPrefix: "ret."))
                    {
                        if (!this.TryGetTypeGeneration(lengths.Field.GetType(), out var typeGen)) continue;
                        var data = lengths.Field.GetFieldData();
                        switch (data.BinaryOverlayFallback)
                        {
                            case BinaryGenerationType.Normal:
                            case BinaryGenerationType.Custom:
                                break;
                            default:
                                continue;
                        }
                        if (lengths.Field is DataType)
                        {
                            await typeGen.GenerateWrapperUnknownLengthParse(
                                sb,
                                obj,
                                lengths.Field,
                                lengths.PassedLength,
                                lengths.PassedAccessor);
                        }
                    }

                    if (objData.CustomBinaryEnd != CustomEnd.Off)
                    {
                        using (var args = sb.Call(
                                   "ret.CustomEnd"))
                        {
                            if (obj.GetObjectType() == ObjectType.Record)
                            {
                                args.Add("stream: origStream");
                            }
                            else
                            {
                                args.AddPassArg("stream");
                            }
                            args.Add("finalPos: stream.Length");
                            args.AddPassArg("offset");
                        }
                    }
                    sb.AppendLine("return ret;");
                }
                sb.AppendLine();

                if (obj.GetObjectType() != ObjectType.Mod)
                {
                    using (var args = sb.Function(
                               $"public static {this.BinaryOverlayClass(obj)} {obj.Name}Factory"))
                    {
                        args.Add($"ReadOnlyMemorySlice<byte> slice");
                        args.Add($"{nameof(BinaryOverlayFactoryPackage)} package");
                        args.Add($"{nameof(TypedParseParams)}? parseParams = null");
                    }
                    using (sb.CurlyBrace())
                    {
                        using (var args = sb.Call(
                                   $"return {obj.Name}Factory"))
                        {
                            args.Add($"stream: new {nameof(OverlayStream)}(slice, package)");
                            args.AddPassArg("package");
                            if (obj.IsVariableLengthStruct())
                            {
                                args.Add($"finalPos: slice.Length");
                            }
                            args.AddPassArg("parseParams");
                        }
                    }
                }
            }
            sb.AppendLine();

            await GenerateOverlayExtras(obj, sb);

            await obj.GenerateToStringCode(sb);

            if (await obj.IsMajorRecord())
            {
                sb.AppendLine($"public override string ToString()");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"return MajorRecordPrinter<{obj.Name}>.ToString(this);");
                }
                sb.AppendLine();
            }

            obj.GenerateEqualsSection(sb);
            await MajorRecordLinkEqualityModule.Generate(obj, sb);

            if (obj.GetObjectType() == ObjectType.Mod)
            {
                sb.AppendLine($"IMask<bool> {nameof(IEqualsMask)}.{nameof(IEqualsMask.GetEqualsMask)}(object rhs, EqualsMaskHelper.Include include = EqualsMaskHelper.Include.OnlyFailures) => {obj.MixInClassName}.GetEqualsMask(this, ({obj.Interface(getter: true, internalInterface: true)})rhs, include);");
            }

        }
        sb.AppendLine();
    }

    public override async Task GenerateInTranslationWriteClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        await GenerateWriteExtras(obj, sb);
        await base.GenerateInTranslationWriteClass(obj, sb);
    }

    protected override async Task GenerateWriteSnippet(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        var data = obj.GetObjectData();
        var isMajor = await obj.IsMajorRecord();
        var hasRecType = obj.TryGetRecordType(out var recType);
        var writerNameToUse = WriterMemberName;
        if (hasRecType)
        {
            if (obj.GetObjectType() == ObjectType.Subrecord)
            {
                using (var args = sb.Call(
                           $"using ({nameof(HeaderExport)}.Subrecord",
                           ")",
                           semiColon: false))
                {
                    args.AddPassArg(writerNameToUse);
                    args.Add($"record: {GetRecordTypeString(obj, "translationParams", "writer.MetaData.Constants.Release", "writer.MetaData.FormVersion")}");
                    args.Add($"overflowRecord: translationParams?.{nameof(TypedWriteParams.OverflowRecordType)}");
                    args.Add("out var writerToUse");
                }

                writerNameToUse = "writerToUse";
            }
            else
            {
                using (var args = sb.Call(
                           $"using ({nameof(HeaderExport)}.{obj.GetObjectType()}",
                           ")",
                           semiColon: false))
                {
                    args.AddPassArg(writerNameToUse);
                    args.Add($"record: {GetRecordTypeString(obj, "translationParams", "writer.MetaData.Constants.Release", "writer.MetaData.FormVersion")}");
                }
            }
        }
        using (sb.CurlyBrace(doIt: hasRecType))
        {
            if (isMajor)
            {
                sb.AppendLine("try");
            }
            if (obj.GetObjectType() == ObjectType.Mod)
            {
                using (var args = sb.Call(
                           $"{nameof(ModHeaderWriteLogic)}.{nameof(ModHeaderWriteLogic.WriteHeader)}"))
                {
                    args.AddPassArg("param");
                    args.Add($"writer: {writerNameToUse}");
                    args.Add("mod: item");
                    args.Add("modHeader: item.ModHeader.DeepCopy()");
                    args.AddPassArg("modKey");
                }
            }
            using (sb.CurlyBrace(doIt: isMajor))
            {
                if (HasEmbeddedFields(obj))
                {
                    using (var args = sb.Call(
                               $"WriteEmbedded"))
                    {
                        args.AddPassArg($"item");
                        args.Add($"writer: {writerNameToUse}");
                    }
                }
                else
                {
                    var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasEmbeddedFields(b));
                    if (firstBase != null)
                    {
                        using (var args = sb.Call(
                                   $"{this.TranslationWriteClass(firstBase)}.WriteEmbedded"))
                        {
                            args.AddPassArg($"item");
                            args.Add($"writer: {writerNameToUse}");
                        }
                    }
                }
                if (HasRecordTypeFields(obj))
                {
                    if (await obj.IsMajorRecord())
                    {
                        sb.AppendLine($"{writerNameToUse}.{nameof(MutagenWriter.MetaData)}.{nameof(WritingBundle.FormVersion)} = item.FormVersion;");
                    }
                    using (var args = sb.Call(
                               $"WriteRecordTypes"))
                    {
                        args.AddPassArg($"item");
                        args.Add($"writer: {writerNameToUse}");
                        if (obj.GetObjectType() == ObjectType.Mod)
                        {
                            args.AddPassArg($"importMask");
                        }
                        else
                        {
                            args.AddPassArg($"translationParams");
                        }
                    }
                    if (await obj.IsMajorRecord())
                    {
                        sb.AppendLine($"{writerNameToUse}.{nameof(MutagenWriter.MetaData)}.{nameof(WritingBundle.FormVersion)} = null;");
                    }
                }
                else
                {
                    var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasRecordTypeFields(b));
                    if (firstBase != null)
                    {
                        using (var args = sb.Call(
                                   $"{this.TranslationWriteClass(firstBase)}.WriteRecordTypes"))
                        {
                            args.AddPassArg($"item");
                            args.Add($"writer: {writerNameToUse}");
                            args.AddPassArg($"translationParams");
                        }
                    }
                }
            }
            if (isMajor)
            {
                sb.AppendLine("catch (Exception ex)");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"throw RecordException.Enrich(ex, item);");
                }
            }
        }
        if (data.CustomBinaryEnd != CustomEnd.Off)
        {
            using (var args = sb.Call(
                       $"CustomBinaryEndExportInternal"))
            {
                args.AddPassArg(WriterMemberName);
                args.Add("obj: item");
            }
        }
    }

    private async Task GenerateWriteExtras(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        var data = obj.GetObjectData();
        if (HasEmbeddedFields(obj))
        {
            using (var args = sb.Function(
                       $"public static void WriteEmbedded{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
                args.Add($"{obj.Interface(internalInterface: true, getter: true)} item");
                args.Add($"{WriterClass} {WriterMemberName}");
            }
            using (sb.CurlyBrace())
            {
                if (obj.HasLoquiBaseObject)
                {
                    var firstBase = obj.BaseClassTrail().FirstOrDefault((b) => HasEmbeddedFields(b));
                    if (firstBase != null)
                    {
                        using (var args = sb.Call(
                                   $"{TranslationWriteClass(firstBase)}.WriteEmbedded"))
                        {
                            args.AddPassArg("item");
                            args.AddPassArg(WriterMemberName);
                        }
                    }
                }
                foreach (var field in obj.IterateFields(nonIntegrated: true, expandSets: SetMarkerType.ExpandSets.False))
                {
                    var fieldData = field.GetFieldData();
                    if (fieldData.HasTrigger) continue;
                    if (field is CustomLogic logic && logic.IsRecordType) continue;
                    if (fieldData.Binary == BinaryGenerationType.NoGeneration) continue;
                    if (field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
                    if (field is BreakType breakType)
                    {
                        sb.AppendLine($"if (!item.{VersioningModule.VersioningFieldName}.HasFlag({obj.Name}.{VersioningModule.VersioningEnumName}.Break{breakType.Index}))");
                        sb.AppendLine("{");
                        sb.Depth++;
                    }
                    if (!field.Enabled) continue;
                    List<string> conditions = new List<string>();
                    if (conditions.Count > 0)
                    {
                        using (var args = sb.If(ands: true))
                        {
                            foreach (var item in conditions)
                            {
                                args.Add(item);
                            }
                        }
                    }
                    using (sb.CurlyBrace(doIt: conditions.Count > 0))
                    {
                        var maskType = this.Gen.MaskModule.GetMaskModule(field.GetType()).GetErrorMaskTypeStr(field);
                        if (fieldData.Binary == BinaryGenerationType.Custom)
                        {
                            CustomLogic.GenerateWrite(
                                sb: sb,
                                obj: obj,
                                field: field,
                                writerAccessor: WriterMemberName);
                            continue;
                        }
                        if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                        {
                            if (!field.IntegrateField) continue;
                            throw new ArgumentException("Unsupported type generator: " + field);
                        }
                        if (fieldData.HasVersioning)
                        {
                            sb.AppendLine($"if ({VersioningModule.GetVersionIfCheck(fieldData, "writer.MetaData.FormVersion!.Value")})");
                        }
                        using (sb.CurlyBrace(doIt: fieldData.HasVersioning))
                        {
                            await generator.GenerateWrite(
                                sb: sb,
                                objGen: obj,
                                typeGen: field,
                                writerAccessor: WriterMemberName,
                                itemAccessor: Accessor.FromType(field, "item"),
                                translationAccessor: null,
                                errorMaskAccessor: null,
                                converterAccessor: null);
                        }
                    }
                }
                for (int i = 0; i < obj.Fields.WhereCastable<TypeGeneration, BreakType>().Count(); i++)
                {
                    sb.Depth--;
                    sb.AppendLine("}");
                }
            }
            sb.AppendLine();
        }

        if (HasRecordTypeFields(obj))
        {
            using (var args = sb.Function(
                       $"public static void WriteRecordTypes{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
                args.Add($"{obj.Interface(internalInterface: true, getter: true)} item");
                args.Add($"{WriterClass} {WriterMemberName}");
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    args.Add($"GroupMask? importMask");
                }
                args.Add($"{nameof(TypedWriteParams)}? translationParams{(obj.GetObjectType() == ObjectType.Mod ? " = null" : null)}");
            }
            using (sb.CurlyBrace())
            {
                if (obj.HasLoquiBaseObject)
                {
                    var firstBase = obj.BaseClassTrail().FirstOrDefault((f) => HasRecordTypeFields(f));
                    if (firstBase != null)
                    {
                        using (var args = sb.Call(
                                   $"{TranslationWriteClass(firstBase)}.WriteRecordTypes"))
                        {
                            args.AddPassArg($"item");
                            args.AddPassArg(WriterMemberName);
                            if (data.BaseRecordTypeConverter?.FromConversions.Count > 0)
                            {
                                args.Add($"translationParams: translationParams.Combine({obj.RegistrationName}.BaseConverter)");
                            }
                            else
                            {
                                args.AddPassArg("translationParams");
                            }
                        }
                    }
                }
                foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
                {
                    var fieldData = field.GetFieldData();
                    if (!fieldData.HasTrigger)
                    {
                        if (field is not CustomLogic custom)
                        {
                            continue;
                        }
                        if (!custom.IsRecordType)
                        {
                            continue;
                        }
                    }
                    if (field.Derivative && fieldData.Binary != BinaryGenerationType.Custom) continue;
                    switch (fieldData.Binary)
                    {
                        case BinaryGenerationType.Normal:
                            break;
                        case BinaryGenerationType.NoGeneration:
                            continue;
                        case BinaryGenerationType.Custom:
                            CustomLogic.GenerateWrite(
                                sb: sb,
                                obj: obj,
                                field: field,
                                writerAccessor: WriterMemberName);
                            continue;
                        default:
                            throw new NotImplementedException();
                    }
                    if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                    {
                        throw new ArgumentException("Unsupported type generator: " + field);
                    }

                    Func<Task> generate = async () =>
                    {
                        var accessor = Accessor.FromType(field, "item");
                        if (field is DataType dataType)
                        {
                            if (dataType.Nullable)
                            {
                                sb.AppendLine($"if (item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Has))");
                            }
                            using (sb.CurlyBrace(doIt: dataType.Nullable))
                            {
                                sb.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}({WriterMemberName}, translationParams.ConvertToCustom({obj.RecordTypeHeaderName(fieldData.RecordType.Value)})))");
                                using (sb.CurlyBrace())
                                {
                                    bool isInRange = false;
                                    foreach (var subField in dataType.IterateFieldsWithMeta())
                                    {
                                        if (!this.TryGetTypeGeneration(subField.Field.GetType(), out var subGenerator))
                                        {
                                            throw new ArgumentException("Unsupported type generator: " + subField.Field);
                                        }

                                        var subData = subField.Field.GetFieldData();
                                        if (!subGenerator.ShouldGenerateCopyIn(subField.Field)) continue;
                                        switch (subData.Binary)
                                        {
                                            case BinaryGenerationType.Normal:
                                                break;
                                            case BinaryGenerationType.NoGeneration:
                                                continue;
                                            case BinaryGenerationType.Custom:
                                                using (var args = sb.Call(
                                                           $"{TranslationWriteClass(obj)}.WriteBinary{subField.Field.Name}"))
                                                {
                                                    args.AddPassArg(WriterMemberName);
                                                    args.AddPassArg("item");
                                                }
                                                continue;
                                            default:
                                                throw new NotImplementedException();
                                        }
                                        if (subField.BreakIndex != -1)
                                        {
                                            sb.AppendLine($"if (!item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Break{subField.BreakIndex}))");
                                            sb.AppendLine("{");
                                            sb.Depth++;
                                        }
                                        if (subField.Range != null && !isInRange)
                                        {
                                            isInRange = true;
                                            sb.AppendLine($"if (item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Range{subField.RangeIndex}))");
                                            sb.AppendLine("{");
                                            sb.Depth++;
                                        }
                                        if (subField.Range == null && isInRange)
                                        {
                                            isInRange = false;
                                            sb.Depth--;
                                            sb.AppendLine("}");
                                        }
                                        if (subData.HasVersioning)
                                        {
                                            sb.AppendLine($"if ({VersioningModule.GetVersionIfCheck(subData, $"{WriterMemberName}.MetaData.FormVersion!.Value")})");
                                        }
                                        using (sb.CurlyBrace(doIt: subData.HasVersioning))
                                        {
                                            await subGenerator.GenerateWrite(
                                                sb: sb,
                                                objGen: obj,
                                                typeGen: subField.Field,
                                                writerAccessor: WriterMemberName,
                                                translationAccessor: null,
                                                itemAccessor: Accessor.FromType(subField.Field, "item"),
                                                errorMaskAccessor: null,
                                                converterAccessor: "translationParams");
                                        }
                                    }
                                    for (int i = 0; i < dataType.BreakIndices.Count; i++)
                                    {
                                        sb.Depth--;
                                        sb.AppendLine("}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!generator.ShouldGenerateWrite(field)) return;
                            if (fieldData.Binary == BinaryGenerationType.NoGeneration) return;

                            var loqui = field as LoquiType;

                            // Skip modheader
                            if (loqui != null
                                && loqui.Name == "ModHeader")
                            {
                                return;
                            }

                            bool doIf = true;
                            if (loqui != null
                                && loqui.TargetObjectGeneration?.GetObjectType() == ObjectType.Group
                                && obj.GetObjectType() == ObjectType.Mod)
                            {
                                sb.AppendLine($"if (importMask?.{field.Name} ?? true)");
                            }
                            else
                            {
                                doIf = false;
                            }
                            using (sb.CurlyBrace(doIt: doIf))
                            {
                                await generator.GenerateWrite(
                                    sb: sb,
                                    objGen: obj,
                                    typeGen: field,
                                    writerAccessor: WriterMemberName,
                                    itemAccessor: accessor,
                                    translationAccessor: null,
                                    errorMaskAccessor: null,
                                    converterAccessor: "translationParams");
                            }
                        }
                    };

                    if (fieldData.CustomVersion == null)
                    {
                        if (fieldData.HasVersioning)
                        {
                            sb.AppendLine($"if ({VersioningModule.GetVersionIfCheck(fieldData, $"{WriterMemberName}.MetaData.FormVersion!.Value")})");
                        }
                        using (sb.CurlyBrace(doIt: fieldData.HasVersioning))
                        {
                            await generate();
                        }
                    }
                    else
                    {
                        sb.AppendLine($"if (item.FormVersion <= {fieldData.CustomVersion})");
                        using (sb.CurlyBrace())
                        {
                            using (var args = sb.Call(
                                       $"{field.Name}CustomVersionWrite"))
                            {
                                args.AddPassArg($"item");
                                args.AddPassArg(WriterMemberName);
                                args.AddPassArg($"recordTypeConverter");
                            }
                        }
                        sb.AppendLine("else");
                        using (sb.CurlyBrace())
                        {
                            await generate();
                        }
                    }
                }

                if (data.EndMarkerType.HasValue)
                {
                    sb.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.Subrecord)}({WriterMemberName}, {obj.RecordTypeHeaderName(data.EndMarkerType.Value)})) {{ }} // End Marker");
                }
            }
            sb.AppendLine();
        }

        foreach (var field in obj.Fields)
        {
            var fieldData = field.GetFieldData();
            if (fieldData.CustomVersion != null)
            {
                using (var args = sb.Call(
                           $"static partial void {field.Name}CustomVersionWrite"))
                {
                    args.Add($"{obj.Interface(getter: true, internalInterface: true)} item");
                    args.Add($"{WriterClass} {WriterMemberName}");
                    args.Add($"{nameof(RecordTypeConverter)}? recordTypeConverter");
                }
                sb.AppendLine();
            }
        }
    }
}