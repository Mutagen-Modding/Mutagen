using Loqui;
using Loqui.Generation;
using Loqui.Internal;
using Mutagen.Bethesda.Constants;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Records;
using Mutagen.Bethesda.Records.Binary;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Records.Binary.Translations;
using Mutagen.Bethesda.Records.Binary.Utility;
using Mutagen.Bethesda.Strings;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Plugin
{
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
                resolutionString: $"{nameof(RecordInfoCache)} infoCache",
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
            var gameRelease = new APILine(
                nicknameKey: "GameRelease",
                resolver: (obj) => $"{ModModule.ReleaseEnumName(obj)} release",
                when: (obj, dir) =>
                {
                    if (dir == TranslationDirection.Writer) return false;
                    if (obj.GetObjectType() != ObjectType.Mod) return false;
                    return obj.GetObjectData().GameReleaseOptions != null;
                });
            var recTypeConverter = new APILine(
                "RecordTypeConverter",
                $"{nameof(RecordTypeConverter)}? recordTypeConverter = null",
                when: (obj, dir) =>
                {
                    return obj.GetObjectType() != ObjectType.Mod;
                });
            this.MainAPI = new TranslationModuleAPI(
                writerAPI: new MethodAPI(
                    majorAPI: new APILine[] { new APILine(WriterClass, $"{WriterClass} {WriterMemberName}") },
                    optionalAPI: modAPILines
                        .And(recTypeConverter)
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
                        .And(recTypeConverter)
                        .And(writeParamOptional)
                        .ToArray(),
                    customAPI: new CustomMethodAPI[]
                    {
                        CustomMethodAPI.FactoryPublic(gameRelease),
                        CustomMethodAPI.FactoryPublic(modKey),
                        CustomMethodAPI.FactoryPrivate(modKeyWriter, "modKey"),
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
                        majorAPI: new APILine[] { new APILine("Path", $"string path") },
                        customAPI: new CustomMethodAPI[]
                        {
                            CustomMethodAPI.FactoryPublic(gameRelease)
                        },
                        optionalAPI: writeParamOptional
                            .AsEnumerable()
                            .And(modAPILines)
                            .And(stringsReadParamOptional)
                            .And(parallel)
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
        private void ConvertFromStreamOut(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
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
            fg.AppendLine("var modKey = item.ModKey;");
            using (var args = new ArgsWrapper(fg,
                $"using (var writer = new MutagenWriter",
                suffixLine: ")",
                semiColon: false))
            {
                args.AddPassArg("stream");
                args.Add($"new {nameof(WritingBundle)}({gameReleaseStr})");
                args.Add("dispose: false");
            }
            using (new BraceWrapper(fg))
            {
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Writer).ToArray());
            }
        }

        private void ConvertFromStreamIn(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("try");
            using (new BraceWrapper(fg))
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
                fg.AppendLine($"using (var reader = new {nameof(MutagenBinaryReadStream)}(stream, modKey, {gameReleaseStr}))");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("var frame = new MutagenFrame(reader);");
                    fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.RecordInfoCache)} = infoCache;");
                    fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Parallel)} = parallel;");
                    internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Reader).ToArray());
                }
            }
            fg.AppendLine("catch (Exception ex)"); 
            using (new BraceWrapper(fg)) 
            { 
                fg.AppendLine("throw RecordException.Enrich(ex, modKey);"); 
            } 
        }

        private void ConvertFromPathOut(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
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

            fg.AppendLine($"param ??= {nameof(BinaryWriteParameters)}.{nameof(BinaryWriteParameters.Default)};");
            using (var args = new ArgsWrapper(fg,
                $"var modKey = param.{nameof(BinaryWriteParameters.RunMasterMatch)}"))
            {
                args.Add("mod: item");
                args.AddPassArg("path");
            }
            if (objData.UsesStringFiles)
            {
                fg.AppendLine("bool disposeStrings = param.StringsWriter == null;");
                fg.AppendLine($"var stringsWriter = param.StringsWriter ?? (EnumExt.HasFlag((int)item.ModHeader.Flags, (int)ModHeaderCommonFlag.Localized) ? new StringsWriter({gameReleaseStr}, modKey, Path.Combine(Path.GetDirectoryName(path)!, \"Strings\")) : null);");
            }
            fg.AppendLine($"var bundle = new {nameof(WritingBundle)}({gameReleaseStr})");
            using (var prop = new PropertyCtorWrapper(fg))
            {
                if (objData.UsesStringFiles)
                {
                    prop.Add($"{nameof(WritingBundle.StringsWriter)} = stringsWriter");
                }
                prop.Add($"{nameof(WritingBundle.CleanNulls)} = param.{nameof(BinaryWriteParameters.CleanNulls)}");
            }
            fg.AppendLine("using var memStream = new MemoryTributary();");
            using (var args = new ArgsWrapper(fg,
                $"using (var writer = new MutagenWriter",
                suffixLine: ")")
            {
                SemiColon = false
            })
            {
                args.Add("memStream");
                args.Add("bundle");
                args.Add("dispose: false");
            }
            using (new BraceWrapper(fg))
            {
                internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Writer).ToArray());
            }
            fg.AppendLine($"using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"memStream.Position = 0;");
                fg.AppendLine($"memStream.CopyTo(fs);");
            }
            if (objData.UsesStringFiles)
            {
                fg.AppendLine("if (disposeStrings)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("param.StringsWriter?.Dispose();");
                }
            }
        }

        private void ConvertFromPathIn(ObjectGeneration obj, FileGeneration fg, InternalTranslation internalToDo)
        {
            fg.AppendLine("try");
            using (new BraceWrapper(fg))
            {
                string gameReleaseStr;
                if (obj.GetObjectData().GameReleaseOptions == null)
                {
                    gameReleaseStr = $"{nameof(GameRelease)}.{obj.GetObjectData().GameCategory}";
                }
                else
                {
                    fg.AppendLine($"var gameRelease = release.ToGameRelease();");
                    gameReleaseStr = $"gameRelease";
                }
                fg.AppendLine($"using (var reader = new {nameof(MutagenBinaryReadStream)}(path, {gameReleaseStr}))");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("var modKey = path.ModKey;");
                    fg.AppendLine("var frame = new MutagenFrame(reader);");
                    fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordInfoCache)}(() => new {nameof(MutagenBinaryReadStream)}(path, {gameReleaseStr}));");
                    fg.AppendLine($"frame.{nameof(MutagenFrame.MetaData)}.{nameof(ParsingBundle.Parallel)} = parallel;");
                    if (obj.GetObjectData().UsesStringFiles)
                    {
                        fg.AppendLine("if (reader.Remaining < 12)");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"throw new ArgumentException(\"File stream was too short to parse flags\");");
                        }
                        fg.AppendLine($"var flags = reader.GetInt32(offset: 8);");
                        fg.AppendLine($"if (EnumExt.HasFlag(flags, (int)ModHeaderCommonFlag.Localized))");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"frame.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.StringsLookup)} = StringsFolderLookupOverlay.TypicalFactory({gameReleaseStr}, path.{nameof(ModPath.ModKey)}, Path.GetDirectoryName(path.{nameof(ModPath.Path)})!, stringsParam);");
                        }
                    }
                    internalToDo(this.MainAPI.PublicMembers(obj, TranslationDirection.Reader).ToArray());
                }
            }
            fg.AppendLine("catch (Exception ex)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("throw RecordException.Enrich(ex, path.ModKey);");
            }
        }

        #endregion

        public override bool WantsTryCreateFromBinary(ObjectGeneration obj)
        {
            return base.WantsTryCreateFromBinary(obj) && obj.GetObjectType() != ObjectType.Mod;
        }

        protected override async Task GenerateBinaryOverlayCreates(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            var objData = obj.GetObjectData();
            using (var args = new FunctionWrapper(fg,
                $"public{obj.NewOverride()}static I{obj.Name}DisposableGetter {CreateFromPrefix}{ModuleNickname}Overlay"))
            {
                args.Add($"ReadOnlyMemorySlice<byte> bytes");
                if (objData.GameReleaseOptions != null)
                {
                    args.Add($"{ModModule.ReleaseEnumName(obj)} release");
                }
                args.Add($"ModKey modKey");
                if (objData.UsesStringFiles)
                {
                    args.Add($"{nameof(IStringsFolderLookup)}? stringsLookup = null");
                }
            }
            using (new BraceWrapper(fg))
            {
                string gameReleaseStr;
                if (objData.GameReleaseOptions == null)
                {
                    gameReleaseStr = $"{nameof(GameRelease)}.{obj.GetObjectData().GameCategory}";
                }
                else
                {
                    gameReleaseStr = $"release.ToGameRelease()";
                }
                fg.AppendLine($"var meta = new {nameof(ParsingBundle)}({gameReleaseStr}, new {nameof(MasterReferenceReader)}(modKey));");
                fg.AppendLine($"meta.{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordInfoCache)}(() => new {nameof(MutagenMemoryReadStream)}(bytes, meta));");
                if (objData.UsesStringFiles)
                {
                    fg.AppendLine($"meta.{nameof(ParsingBundle.StringsLookup)} = stringsLookup;");
                }
                using (var args = new ArgsWrapper(fg,
                    $"return {BinaryOverlayClass(obj)}.{obj.Name}Factory"))
                {
                    args.Add(subFg =>
                    {
                        using (var subArgs = new FunctionWrapper(subFg,
                            $"stream: new {nameof(MutagenMemoryReadStream)}"))
                        {
                            subArgs.Add("data: bytes");
                            subArgs.Add($"metaData: meta");
                        }
                    });
                    if (objData.GameReleaseOptions != null)
                    {
                        args.AddPassArg("release");
                    }
                    args.AddPassArg("modKey");
                    args.Add("shouldDispose: false");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
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
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
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
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public{obj.NewOverride()}static I{obj.Name}DisposableGetter {CreateFromPrefix}{ModuleNickname}Overlay"))
            {
                args.Add($"{nameof(Stream)} stream");
                if (objData.GameReleaseOptions != null)
                {
                    args.Add($"{ModModule.ReleaseEnumName(obj)} release");
                }
                args.Add($"ModKey modKey");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
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
            fg.AppendLine();
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);

            if (obj.GetObjectType() == ObjectType.Mod)
            {
                using (var args = new FunctionWrapper(fg,
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
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("try");
                    using (new BraceWrapper(fg))
                    {
                        await GenerateNewSnippet(obj, fg);
                        using (var args = new ArgsWrapper(fg,
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
                        fg.AppendLine("return ret;");
                    }
                    fg.AppendLine("catch (Exception ex)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("throw RecordException.Enrich(ex, modKey);");
                    }
                }
                fg.AppendLine();
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
                yield return "Mutagen.Bethesda.Constants";
                yield return "Mutagen.Bethesda.Records.Binary";
                yield return "Mutagen.Bethesda.Records.Binary.Utility";
            }

            if (HasRecordTypeFields(obj))
            {
                yield return "Mutagen.Bethesda.Records.Internals";
            }

            yield return "Mutagen.Bethesda.Records";
            yield return "Mutagen.Bethesda.Records.Binary.Overlay";
            yield return "Mutagen.Bethesda.Records.Binary.Translations";
            yield return "Mutagen.Bethesda.Records.Binary.Streams";
        }

        public override async Task GenerateInTranslationCreateClass(ObjectGeneration obj, FileGeneration fg)
        {
            await GenerateCreateExtras(obj, fg);
            await base.GenerateInTranslationCreateClass(obj, fg);
        }

        private async Task GenerateCreateExtras(ObjectGeneration obj, FileGeneration fg)
        {
            var data = obj.GetObjectData();
            bool typelessStruct = obj.IsTypelessStruct();

            if (await obj.IsMajorRecord())
            {
                if (data.TriggeringRecordTypes.Count == 1
                    && !obj.Abstract)
                {
                    fg.AppendLine($"public{obj.FunctionOverride()}RecordType RecordType => {obj.GetTriggeringSource()};");
                }
                else
                {
                    fg.AppendLine($"public{obj.FunctionOverride()}RecordType RecordType => throw new ArgumentException();");
                }
            }

            if ((!obj.Abstract && obj.BaseClassTrail().All((b) => b.Abstract)) || HasEmbeddedFields(obj))
            {
                var async = HasAsyncStructs(obj, self: true);
                using (var args = new FunctionWrapper(fg,
                    $"public static {Loqui.Generation.Utility.TaskReturn(async)} Fill{ModuleNickname}Structs"))
                {
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                    args.Add($"{ReaderClass} {ReaderMemberName}");
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any((b) => HasEmbeddedFields(b)))
                    {
                        using (var args = new ArgsWrapper(fg,
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
                            fg.AppendLine($"if (frame.Complete) return;");
                        }

                        if (field is BreakType)
                        {
                            fg.AppendLine("if (frame.Complete)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"item.{VersioningModule.VersioningFieldName} |= {obj.Name}.{VersioningModule.VersioningEnumName}.Break{breakIndex++};");
                                fg.AppendLine("return;");
                            }
                            continue;
                        }
                        await GenerateFillSnippet(obj, fg, field, generator, "frame");
                    }
                }
                fg.AppendLine();
            }

            if (HasRecordTypeFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public static {Loqui.Generation.Utility.TaskWrap(nameof(ParseResult), HasAsyncRecords(obj, self: true))} Fill{ModuleNickname}RecordTypes"))
                {
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                    args.Add($"{ReaderClass} {ReaderMemberName}");
                    if (obj.GetObjectType() == ObjectType.Subrecord)
                    {
                        args.Add($"int? lastParsed");
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
                    args.Add($"{nameof(RecordTypeConverter)}? recordTypeConverter = null");
                }
                using (new BraceWrapper(fg))
                {
                    var mutaObjType = obj.GetObjectType();
                    fg.AppendLine($"nextRecordType = recordTypeConverter.ConvertToStandard(nextRecordType);");
                    fg.AppendLine("switch (nextRecordType.TypeInt)");
                    using (new BraceWrapper(fg))
                    {
                        var fields = new List<(int, int, TypeGeneration Field)>();
                        foreach (var field in obj.IterateFieldIndices(
                            expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                            nonIntegrated: true))
                        {
                            var fieldData = field.Field.GetFieldData();
                            if (fieldData.GenerationTypes.Count() == 0) continue;
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
                                    fg.AppendLine($"case RecordTypeInts.{trigger.CheckedType}:");
                                }
                                using (new BraceWrapper(fg))
                                {
                                    if (doubles == null)
                                    {
                                        await GenerateLastParsedShortCircuit(
                                            obj: obj,
                                            fg: fg,
                                            field: field,
                                            doublesPotential: false,
                                            nextRecAccessor: "nextRecordType",
                                            toDo: async () =>
                                            {
                                                var groupMask = data.ObjectType == ObjectType.Mod && (loqui?.TargetObjectGeneration?.GetObjectType() == ObjectType.Group);
                                                if (groupMask)
                                                {
                                                    fg.AppendLine($"if (importMask?.{field.Field.Name} ?? true)");
                                                }
                                                using (new BraceWrapper(fg, doIt: groupMask))
                                                {
                                                    await GenerateFillSnippet(obj, fg, gen.Value, generator, ReaderMemberName);
                                                }
                                                if (groupMask)
                                                {
                                                    fg.AppendLine("else");
                                                    using (new BraceWrapper(fg))
                                                    {
                                                        fg.AppendLine($"{ReaderMemberName}.Position += contentLength;");
                                                    }
                                                }
                                            });
                                    }
                                    else
                                    {
                                        fg.AppendLine($"switch (recordParseCount?.GetOrAdd(nextRecordType) ?? 0)");
                                        using (new BraceWrapper(fg))
                                        {
                                            int count = 0;
                                            foreach (var doublesField in doubles)
                                            {
                                                if (!this.TryGetTypeGeneration(doublesField.Field.GetType(), out var doubleGen))
                                                {
                                                    throw new ArgumentException("Unsupported type generator: " + doublesField.Field);
                                                }
                                                fg.AppendLine($"case {count++}:");
                                                using (new DepthWrapper(fg))
                                                {
                                                    await GenerateLastParsedShortCircuit(
                                                        obj: obj,
                                                        fg: fg,
                                                        field: doublesField,
                                                        doublesPotential: true,
                                                        nextRecAccessor: "nextRecordType",
                                                        toDo: async () =>
                                                        {
                                                            var groupMask = data.ObjectType == ObjectType.Mod && (loqui?.TargetObjectGeneration?.GetObjectType() == ObjectType.Group);
                                                            if (groupMask)
                                                            {
                                                                fg.AppendLine($"if (importMask?.{doublesField.Field.Name} ?? true)");
                                                            }
                                                            using (new BraceWrapper(fg, doIt: groupMask))
                                                            {
                                                                await GenerateFillSnippet(obj, fg, doublesField.Field, doubleGen, ReaderMemberName);
                                                            }
                                                            if (groupMask)
                                                            {
                                                                fg.AppendLine("else");
                                                                using (new BraceWrapper(fg))
                                                                {
                                                                    fg.AppendLine($"{ReaderMemberName}.Position += contentLength;");
                                                                }
                                                            }
                                                        });
                                                }
                                            }
                                            fg.AppendLine($"default:");
                                            using (new DepthWrapper(fg))
                                            {
                                                fg.AppendLine($"throw new NotImplementedException();");
                                            }
                                        }
                                        doubles.Clear();
                                    }
                                }
                            }
                        }

                        if (data.EndMarkerType.HasValue)
                        {
                            fg.AppendLine($"case RecordTypeInts.{data.EndMarkerType}: // End Marker");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"{ReaderMemberName}.ReadSubrecordFrame();");
                                fg.AppendLine($"return {nameof(ParseResult)}.Stop;");
                            }
                        }
                        fg.AppendLine($"default:");
                        using (new DepthWrapper(fg))
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
                                    using (var args = new IfWrapper(fg, ANDs: true, first: first))
                                    {
                                        foreach (var trigger in fieldData.TriggeringRecordAccessors)
                                        {
                                            args.Add($"nextRecordType.Equals({trigger})");
                                        }
                                    }
                                    first = false;
                                    using (new BraceWrapper(fg))
                                    {
                                        await GenerateFillSnippet(obj, fg, field.Field, generator, ReaderMemberName);
                                        fg.AppendLine($"return {nameof(ParseResult)}.Stop;");
                                    }
                                }
                            }

                            // Default case 
                            if (obj.GetObjectData().CustomRecordFallback)
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"return CustomRecordFallback"))
                                {
                                    args.AddPassArg($"item");
                                    args.AddPassArg(ReaderMemberName);
                                    if (obj.GetObjectType() == ObjectType.Subrecord)
                                    {
                                        args.AddPassArg($"lastParsed");
                                    }
                                    args.AddPassArg("recordParseCount");
                                    args.AddPassArg("nextRecordType");
                                    args.AddPassArg("contentLength");
                                    args.AddPassArg($"recordTypeConverter");
                                }
                            }
                            else if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any((b) => HasRecordTypeFields(b)))
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"return {Loqui.Generation.Utility.Await(HasAsyncRecords(obj, self: false))}{TranslationCreateClass(obj.BaseClass)}.Fill{ModuleNickname}RecordTypes"))
                                {
                                    args.AddPassArg("item");
                                    args.AddPassArg(ReaderMemberName);
                                    if (obj.BaseClass.GetObjectType() == ObjectType.Subrecord)
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
                                        args.Add($"recordTypeConverter: {obj.RegistrationName}.BaseConverter");
                                    }
                                }
                            }
                            else
                            {
                                var failOnUnknown = obj.GetObjectData().FailOnUnknown;
                                if (mutaObjType == ObjectType.Subrecord)
                                {
                                    fg.AppendLine($"return {nameof(ParseResult)}.Stop;");
                                }
                                else if (failOnUnknown)
                                {
                                    fg.AppendLine($"throw new ArgumentException($\"Unexpected header {{nextRecordType.Type}} at position {{{ReaderMemberName}.Position}}\");");
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
                                    fg.AppendLine($"{ReaderMemberName}.Position += contentLength{addString};");
                                    fg.AppendLine($"return default(int?);");
                                }
                            }
                        }
                    }
                }
                fg.AppendLine();
            }

            foreach (var field in obj.Fields)
            {
                if (field.GetFieldData().CustomVersion != null)
                {
                    using (var args = new ArgsWrapper(fg,
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
                    fg.AppendLine();
                }
            }
        }

        protected override async Task GenerateFillSnippet(ObjectGeneration obj, FileGeneration fg, TypeGeneration field, BinaryTranslationGeneration generator, string frameAccessor)
        {
            if (field is DataType set)
            {
                fg.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
                fg.AppendLine($"var dataFrame = {frameAccessor}.SpawnWithLength(contentLength);");
                if (set.Nullable)
                {
                    fg.AppendLine($"if (!dataFrame.Complete)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"item.{set.StateName} = {obj.ObjectName}.{set.EnumName}.Has;");
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
                        fg.AppendLine($"if (dataFrame.Complete)");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"item.{set.StateName} |= {obj.ObjectName}.{set.EnumName}.Break{subField.BreakIndex};");
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
                            fg.AppendLine($"return {enumName ?? "default(int?)"};");
                        }
                    }
                    if (subField.Range != null && !isInRange)
                    {
                        isInRange = true;
                        fg.AppendLine($"if (dataFrame.TotalLength > {subField.Range.DataSetSizeMin})");
                        fg.AppendLine("{");
                        fg.Depth++;
                        fg.AppendLine($"item.{set.StateName} |= {obj.Name}.{set.EnumName}.Range{subField.RangeIndex};");
                    }
                    if (subField.Range == null && isInRange)
                    {
                        isInRange = false;
                        fg.Depth--;
                        fg.AppendLine("}");
                    }
                    await GenerateFillSnippet(obj, fg, subField.Field, subGenerator, "dataFrame");
                }
                if (isInRange)
                {
                    isInRange = false;
                    fg.AppendLine("}");
                    fg.Depth--;
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
                        fg,
                        field,
                        frameAccessor,
                        isAsync: false);
                    return;
                default:
                    throw new NotImplementedException();
            }

            if (data.CustomVersion != null)
            {
                fg.AppendLine($"if (item.FormVersion <= {data.CustomVersion})");
                using (new BraceWrapper(fg))
                {
                    await GenerateLastParsedShortCircuit(
                        obj: obj,
                        fg: fg,
                        field: field.GetIndexData(),
                        doublesPotential: false,
                        nextRecAccessor: "nextRecordType",
                        toDo: async () =>
                        {
                            using (var args = new ArgsWrapper(fg,
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
                fg.AppendLine($"{ReaderMemberName}.Position += {ReaderMemberName}.MetaData.SubConstants.HeaderLength + contentLength;");
                // read in target record type. 
                fg.AppendLine($"var nextRec = {ReaderMemberName}.MetaData.GetSubrecord({ReaderMemberName});");
                // Return if it's not there 
                fg.AppendLine($"if (nextRec.RecordType != {obj.RecordTypeHeaderName(data.RecordType.Value)}) throw new ArgumentException(\"Marker was read but not followed by expected subrecord.\");");
                fg.AppendLine("contentLength = nextRec.RecordLength;");
            }

            if (data.OverflowRecordType.HasValue)
            {
                fg.AppendLine($"if (nextRecordType == {obj.RecordTypeHeaderName(data.OverflowRecordType.Value)})");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"var overflowHeader = {ReaderMemberName}.ReadSubrecordFrame();");
                    fg.AppendLine("contentLength = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(overflowHeader.Content));");
                }
            }

            if (data.HasVersioning)
            {
                fg.AppendLine($"if ({VersioningModule.GetVersionIfCheck(data, $"{ReaderMemberName}.MetaData.FormVersion!.Value")})");
            }
            using (new BraceWrapper(fg, doIt: data.HasVersioning))
            {
                await generator.GenerateCopyIn(
                    fg: fg,
                    objGen: obj,
                    typeGen: field,
                    readerAccessor: frameAccessor,
                    itemAccessor: Accessor.FromType(field, "item"),
                    translationAccessor: null,
                    errorMaskAccessor: null);
            }
        }
         
        public override async Task GenerateInVoid(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInVoid(obj, fg);
            using (new NamespaceWrapper(fg, obj.InternalNamespace))
            {
                await GenerateImportWrapper(obj, fg);
            }
        }

        protected bool OverlayRootObject(ObjectGeneration obj)
        {
            return obj.GetObjectType() == ObjectType.Mod;
        }

        private static async Task GenerateLastParsedShortCircuit(
            ObjectGeneration obj,
            FileGeneration fg,
            (int PublicIndex, int InternalIndex, TypeGeneration Field) field,
            bool doublesPotential,
            Accessor nextRecAccessor,
            Func<Task> toDo)
        {
            var dataSet = field.Field as DataType;
            var typelessStruct = obj.IsTypelessStruct();
            if (typelessStruct && field.Field.GetFieldData().IsTriggerForObject)
            {
                if (dataSet != null)
                {
                    fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){dataSet.SubFields.Last().IndexEnumName}) return {nameof(ParseResult)}.Stop;");
                }
                else if (field.Field is CustomLogic)
                {
                    var objFields = obj.IterateFieldIndices(nonIntegrated: false).ToList();
                    var nextField = objFields.FirstOrDefault((i) => i.InternalIndex > field.InternalIndex);
                    var prevField = objFields.LastOrDefault((i) => i.InternalIndex < field.InternalIndex);
                    if (nextField.Field != null)
                    {
                        fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){nextField.Field.IndexEnumName}) return {nameof(ParseResult)}.Stop;");
                    }
                    else if (prevField.Field != null)
                    {
                        fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){prevField.Field.IndexEnumName}) return {nameof(ParseResult)}.Stop;");
                    }
                }
                else if (!(field.Field is MarkerType))
                {
                    fg.AppendLine($"if (lastParsed.HasValue && lastParsed.Value >= (int){field.Field.IndexEnumName}) return {nameof(ParseResult)}.Stop;");
                }
            }
            await toDo();
            if (dataSet != null)
            {
                fg.AppendLine($"return (int){dataSet.SubFields.Last(f => f.IntegrateField && f.Enabled).IndexEnumName};");
            }
            else if (field.Field is CustomLogic)
            {
                fg.AppendLine($"return {(typelessStruct ? "lastParsed" : "null")};");
            }
            else if (field.Field is MarkerType marker)
            {
                if (marker.EndMarker)
                {
                    fg.AppendLine($"return {nameof(ParseResult)}.{nameof(ParseResult.Stop)};");
                }
                else
                {
                    if (doublesPotential)
                    {
                        fg.AppendLine($"return new {nameof(ParseResult)}(default(int?), {nextRecAccessor});");
                    }
                    else
                    {
                        fg.AppendLine($"return default(int?);");
                    }
                }
            }
            else
            {
                if (doublesPotential)
                {
                    fg.AppendLine($"return new {nameof(ParseResult)}((int){field.Field.IndexEnumName}, {nextRecAccessor});");
                }
                else
                {
                    fg.AppendLine($"return (int){field.Field.IndexEnumName};");
                }
            }
        }

        protected async Task GenerateOverlayExtras(ObjectGeneration obj, FileGeneration fg)
        {
            if (HasRecordTypeFields(obj))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public{await obj.FunctionOverride(async b => HasRecordTypeFields(b))}{nameof(ParseResult)} FillRecordType"))
                {
                    args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? nameof(IBinaryReadStream) : nameof(OverlayStream))} stream");
                    args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "long" : "int")} finalPos");
                    args.Add($"int offset");
                    args.Add("RecordType type");
                    args.Add("int? lastParsed");
                    if (obj.GetObjectType() != ObjectType.Mod)
                    {
                        args.Add("Dictionary<RecordType, int>? recordParseCount");
                    }
                    args.Add("RecordTypeConverter? recordTypeConverter = null");
                }
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"type = recordTypeConverter.ConvertToStandard(type);");
                    fg.AppendLine("switch (type.TypeInt)");
                    using (new BraceWrapper(fg))
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
                                    fg.AppendLine($"case RecordTypeInts.{trigger.CheckedType}:");
                                }
                                using (new BraceWrapper(fg))
                                {
                                    if (doubles == null)
                                    {
                                        await GenerateLastParsedShortCircuit(
                                            obj: obj,
                                            fg: fg,
                                            field: field,
                                            doublesPotential: false,
                                            nextRecAccessor: "type",
                                            toDo: async () =>
                                            {
                                                string recConverter = "recordTypeConverter";
                                                if (fieldData?.RecordTypeConverter != null
                                                    && fieldData.RecordTypeConverter.FromConversions.Count > 0)
                                                {
                                                    recConverter = $"{obj.RegistrationName}.{field.Field.Name}Converter";
                                                }
                                                await generator.GenerateWrapperRecordTypeParse(
                                                    fg: fg,
                                                    objGen: obj,
                                                    typeGen: gen.Value,
                                                    locationAccessor: "(stream.Position - offset)",
                                                    packageAccessor: "_package",
                                                    converterAccessor: recConverter);
                                                if (obj.GetObjectType() == ObjectType.Mod
                                                    && field.Field.Name == "ModHeader")
                                                {
                                                    using (var args = new ArgsWrapper(fg,
                                                        $"_package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.MasterReferences)}!.SetTo"))
                                                    {
                                                        args.Add(subFg =>
                                                        {
                                                            subFg.AppendLine("this.ModHeader.MasterReferences.Select(");
                                                            using (new DepthWrapper(subFg))
                                                            {
                                                                subFg.AppendLine($"master => new {nameof(MasterReference)}()");
                                                                using (new BraceWrapper(subFg) { AppendParenthesis = true })
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
                                        fg.AppendLine($"switch (recordParseCount?.GetOrAdd(type) ?? 0)");
                                        using (new BraceWrapper(fg))
                                        {
                                            int count = 0;
                                            foreach (var doublesField in doubles)
                                            {
                                                if (!this.TryGetTypeGeneration(doublesField.Field.GetType(), out var doubleGen))
                                                {
                                                    throw new ArgumentException("Unsupported type generator: " + doublesField.Field);
                                                }
                                                var doublesFieldData = doublesField.Field.GetFieldData();
                                                fg.AppendLine($"case {count++}:");
                                                using (new DepthWrapper(fg))
                                                {
                                                    await GenerateLastParsedShortCircuit(
                                                        obj: obj,
                                                        fg: fg,
                                                        field: doublesField,
                                                        doublesPotential: true,
                                                        nextRecAccessor: "type",
                                                        toDo: async () =>
                                                        {
                                                            string recConverter = "recordTypeConverter";
                                                            if (doublesFieldData.RecordTypeConverter != null
                                                                && doublesFieldData.RecordTypeConverter.FromConversions.Count > 0)
                                                            {
                                                                recConverter = $"{obj.RegistrationName}.{doublesField.Field.Name}Converter";
                                                            }
                                                            await doubleGen.GenerateWrapperRecordTypeParse(
                                                                fg: fg,
                                                                objGen: obj,
                                                                typeGen: doublesField.Field,
                                                                locationAccessor: "(stream.Position - offset)",
                                                                packageAccessor: "_package",
                                                                converterAccessor: recConverter);
                                                            if (obj.GetObjectType() == ObjectType.Mod
                                                                && doublesField.Field.Name == "ModHeader")
                                                            {
                                                                using (var args = new ArgsWrapper(fg,
                                                                    $"_package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.MasterReferences)}!.SetTo"))
                                                                {
                                                                    args.Add(subFg =>
                                                                    {
                                                                        subFg.AppendLine("this.ModHeader.MasterReferences.Select(");
                                                                        using (new DepthWrapper(subFg))
                                                                        {
                                                                            subFg.AppendLine($"master => new {nameof(MasterReference)}()");
                                                                            using (new BraceWrapper(subFg) { AppendParenthesis = true })
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
                                            fg.AppendLine($"default:");
                                            using (new DepthWrapper(fg))
                                            {
                                                fg.AppendLine($"throw new NotImplementedException();");
                                            }
                                        }
                                        doubles.Clear();
                                    }
                                }
                            }
                        }
                        var endMarkerType = obj.GetObjectData().EndMarkerType;
                        if (endMarkerType.HasValue)
                        {
                            fg.AppendLine($"case RecordTypeInts.{endMarkerType}: // End Marker");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine($"stream.ReadSubrecordFrame();");
                                fg.AppendLine($"return {nameof(ParseResult)}.Stop;");
                            }
                        }
                        fg.AppendLine("default:");
                        using (new DepthWrapper(fg))
                        {
                            if (obj.GetObjectData().CustomRecordFallback)
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"return CustomRecordFallback"))
                                {
                                    args.AddPassArg("stream");
                                    args.AddPassArg("finalPos");
                                    args.AddPassArg("offset");
                                    args.AddPassArg("type");
                                    args.AddPassArg("lastParsed");
                                    if (obj.GetObjectData().BaseRecordTypeConverter?.FromConversions.Count > 0)
                                    {
                                        args.Add($"recordTypeConverter: {obj.RegistrationName}.BaseConverter");
                                    }
                                }
                            }
                            else if (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b)))
                            {
                                using (var args = new ArgsWrapper(fg,
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
                                        args.Add($"recordTypeConverter: {obj.RegistrationName}.BaseConverter");
                                    }
                                }
                            }
                            else
                            {
                                var failOnUnknown = obj.GetObjectData().FailOnUnknown;
                                if (obj.GetObjectType() == ObjectType.Subrecord)
                                {
                                    fg.AppendLine($"return {nameof(ParseResult)}.Stop;");
                                }
                                else if (failOnUnknown)
                                {
                                    fg.AppendLine("throw new ArgumentException($\"Unexpected header {nextRecordType.Type} at position {frame.Position}\");");
                                }
                                else
                                {
                                    fg.AppendLine($"return default(int?);");
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override async Task GenerateCopyInSnippet(ObjectGeneration obj, FileGeneration fg, Accessor accessor)
        {
            var data = obj.GetObjectData();

            bool typelessStruct = obj.IsTypelessStruct();
            ObjectType objType = obj.GetObjectType();

            if (await obj.IsMajorRecord())
            {
                bool async = this.HasAsync(obj, self: true);
                using (var args = new ArgsWrapper(fg,
                    $"{Loqui.Generation.Utility.Await(async)}Utility{(async ? "Async" : null)}Translation.MajorRecordParse<{obj.Interface(getter: false, internalInterface: true)}>"))
                {
                    args.Add($"record: {accessor}");
                    args.AddPassArg(ReaderMemberName);
                    args.Add($"recordTypeConverter: recordTypeConverter");
                    args.Add($"fillStructs: {TranslationCreateClass(obj)}.FillBinaryStructs");
                    args.Add($"fillTyped: {TranslationCreateClass(obj)}.FillBinaryRecordTypes");
                }
                if (data.CustomBinaryEnd != CustomEnd.Off)
                {
                    using (var args = new ArgsWrapper(fg,
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
                                    fg.AppendLine($"{ReaderMemberName}.Position += {ReaderMemberName}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.HeaderLength;");
                                }
                                else
                                {
                                    using (var args = new ArgsWrapper(fg,
                                        $"{ReaderMemberName} = {ReaderMemberName}.SpawnWithFinalPosition({nameof(HeaderTranslation)}.ParseSubrecord",
                                        suffixLine: ")"))
                                    {
                                        args.Add($"{ReaderMemberName}.Reader");
                                        args.Add(GetRecordTypeString(obj, $"{ReaderMemberName}.MetaData.Constants.Release", $"{ReaderMemberName}.MetaData.FormVersion"));
                                    }
                                }
                            }
                            break;
                        case ObjectType.Record:
                            using (var args = new ArgsWrapper(fg,
                                $"{ReaderMemberName} = {ReaderMemberName}.SpawnWithFinalPosition({nameof(HeaderTranslation)}.ParseRecord",
                                suffixLine: ")"))
                            {
                                args.Add($"{ReaderMemberName}.Reader");
                                args.Add(GetRecordTypeString(obj, $"{ReaderMemberName}.MetaData.Constants.Release", $"{ReaderMemberName}.MetaData.FormVersion"));
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
                var utilityTranslation = $"{Loqui.Generation.Utility.Await(async)}Utility{(async ? "Async" : null)}Translation";
                switch (objType)
                {
                    case ObjectType.Subrecord:
                        using (var args = new ArgsWrapper(fg,
                            $"{utilityTranslation}.SubrecordParse",
                            suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                        {
                            args.Add($"record: {accessor}");
                            args.Add("frame: frame");
                            args.Add("recordTypeConverter: recordTypeConverter");
                            args.Add($"fillStructs: {TranslationCreateClass(obj)}.Fill{ModuleNickname}Structs");
                            if (HasRecordTypeFields(obj)
                                || (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b))))
                            {
                                args.Add($"fillTyped: {TranslationCreateClass(obj)}.Fill{ModuleNickname}RecordTypes");
                            }
                        }
                        break;
                    case ObjectType.Record:
                        using (var args = new ArgsWrapper(fg,
                            $"{utilityTranslation}.RecordParse",
                            suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                        {
                            args.Add($"record: {accessor}");
                            args.AddPassArg(ReaderMemberName);
                            args.Add("recordTypeConverter: recordTypeConverter");
                            args.Add($"fillStructs: {TranslationCreateClass(obj)}.Fill{ModuleNickname}Structs");
                            if (HasRecordTypeFields(obj)
                                || (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b))))
                            {
                                args.Add($"fillTyped: {TranslationCreateClass(obj)}.Fill{ModuleNickname}RecordTypes");
                            }
                        }
                        break;
                    case ObjectType.Group:
                        using (var args = new ArgsWrapper(fg,
                            $"{utilityTranslation}.GroupParse",
                            suffixLine: Loqui.Generation.Utility.ConfigAwait(async)))
                        {
                            args.Add($"record: {accessor}");
                            args.AddPassArg(ReaderMemberName);
                            args.AddPassArg("recordTypeConverter");
                            args.Add($"fillStructs: {TranslationCreateClass(obj)}.Fill{ModuleNickname}Structs");
                            if (HasRecordTypeFields(obj)
                                || (obj.HasLoquiBaseObject && obj.BaseClassTrail().Any(b => HasRecordTypeFields(b))))
                            {
                                args.Add($"fillTyped: {TranslationCreateClass(obj)}.Fill{ModuleNickname}RecordTypes");
                            }
                        }
                        break;
                    case ObjectType.Mod:
                        using (var args = new ArgsWrapper(fg,
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
                GenerateStructStateSubscriptions(obj, fg);
                if (data.CustomBinaryEnd != CustomEnd.Off)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{Loqui.Generation.Utility.Await(data.CustomBinaryEnd == CustomEnd.Async)}{this.TranslationCreateClass(obj)}.CustomBinaryEndImportPublic"))
                    {
                        args.AddPassArg(ReaderMemberName);
                        args.Add($"obj: {accessor}");
                    }
                }
            }
        }

        private void GenerateStructStateSubscriptions(ObjectGeneration obj, FileGeneration fg)
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

        public override void CustomMainWriteMixInPreLoad(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            fg.AppendLine("var modKey = item.ModKey;");
        }

        protected async Task GenerateImportWrapper(ObjectGeneration obj, FileGeneration fg)
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
                fg.AppendLine("[DebuggerDisplay(\"{GameRelease} {ModKey.ToString()}\")]");
            }
            using (var args = new ClassWrapper(fg, $"{BinaryOverlayClass(obj)}"))
            {
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
            using (new BraceWrapper(fg))
            {
                await obj.GenerateRouting(fg, getterOnly: true);
                obj.GenerateGetterInterfaceImplementations(fg);
                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    if (objData.GameReleaseOptions != null)
                    {
                        fg.AppendLine($"public {ModModule.ReleaseEnumName(obj)} {ModModule.ReleaseEnumName(obj)} {{ get; }}");
                        fg.AppendLine($"public {nameof(GameRelease)} GameRelease => {ModModule.ReleaseEnumName(obj)}.ToGameRelease();");
                    }
                    else
                    {
                        fg.AppendLine($"public {nameof(GameRelease)} GameRelease => {nameof(GameRelease)}.{obj.GetObjectData().GameCategory};");
                    }
                    fg.AppendLine($"IReadOnlyCache<T, FormKey> {nameof(IModGetter)}.{nameof(IModGetter.GetTopLevelGroupGetter)}<T>() => this.{nameof(IModGetter.GetTopLevelGroupGetter)}<T>();");
                    fg.AppendLine($"void IModGetter.WriteToBinary(string path, {nameof(BinaryWriteParameters)}? param) => this.WriteToBinary(path, importMask: null, param: param);");
                    fg.AppendLine($"void IModGetter.WriteToBinaryParallel(string path, {nameof(BinaryWriteParameters)}? param) => this.WriteToBinaryParallel(path, param: param);");
                    fg.AppendLine($"IReadOnlyList<{nameof(IMasterReferenceGetter)}> {nameof(IModGetter)}.MasterReferences => this.ModHeader.MasterReferences;");
                    if (obj.GetObjectData().UsesStringFiles)
                    {
                        fg.AppendLine($"public bool CanUseLocalization => true;");
                        fg.AppendLine($"public bool UsingLocalization => this.ModHeader.Flags.HasFlag({obj.GetObjectData().GameCategory}ModHeader.HeaderFlag.Localized);");
                    }
                    else
                    {
                        fg.AppendLine($"public bool CanUseLocalization => false;");
                        fg.AppendLine($"public bool UsingLocalization => false;");
                    }

                }

                if (obj.GetObjectType() == ObjectType.Mod
                    || (await LinkModule.HasLinks(obj, includeBaseClass: false) != LinkModule.LinkCase.No))
                {
                    await LinkModule.GenerateInterfaceImplementation(obj, fg, getter: true);
                }

                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    MajorRecordContextEnumerationModule.GenerateClassImplementation(obj, fg, onlyGetter: true);
                }

                if (await MajorRecordModule.HasMajorRecordsInTree(obj, includeBaseClass: false) != Case.No)
                {
                    MajorRecordEnumerationModule.GenerateClassImplementation(obj, fg, onlyGetter: true);
                }

                foreach (var transl in obj.ProtoGen.Gen.GenerationModules
                    .WhereCastable<IGenerationModule, ITranslationModule>())
                {
                    if (transl.DoTranslationInterface(obj))
                    {
                        await transl.GenerateTranslationInterfaceImplementation(obj, fg);
                    }
                }

                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    fg.AppendLine($"uint IModGetter.NextFormID => ModHeader.Stats.NextFormID;");
                    fg.AppendLine($"public {nameof(ModKey)} ModKey {{ get; }}");
                    fg.AppendLine($"private readonly {nameof(BinaryOverlayFactoryPackage)} _package;");
                    fg.AppendLine($"private readonly {nameof(IBinaryReadStream)} _data;");
                    fg.AppendLine($"private readonly bool _shouldDispose;");

                    fg.AppendLine("public void Dispose()");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("if (!_shouldDispose) return;");
                        fg.AppendLine("_data.Dispose();");
                    }
                }

                if (obj.GetObjectData().MajorRecordFlags)
                {
                    fg.AppendLine($"public {obj.ObjectName}.MajorFlag MajorFlags => ({obj.ObjectName}.MajorFlag)this.MajorRecordFlagsRaw;");
                }

                fg.AppendLine();
                if (obj.Fields.Any(f => f is BreakType))
                {
                    fg.AppendLine($"public {obj.ObjectName}.{VersioningModule.VersioningEnumName} {VersioningModule.VersioningFieldName} {{ get; private set; }}");
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
                    using (new RegionWrapper(fg, lengths.Field.Name)
                    {
                        AppendExtraLine = false,
                        SkipIfOnlyOneLine = true
                    })
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
                            fg,
                            obj,
                            lengths.Field,
                            dataAccessor,
                            lengths.PassedLength,
                            lengths.PassedAccessor);
                        if (data.HasVersioning
                            && !lengths.Field.Nullable)
                        {
                            VersioningModule.AddVersionOffset(fg, lengths.Field, lengths.FieldLength.Value, lastVersionedField, $"_package.FormVersion!.FormVersion!.Value");
                            lastVersionedField = lengths.Field;
                        }
                        if (!data.HasTrigger)
                        {
                            if (lengths.CurLength == null)
                            {
                                fg.AppendLine($"protected int {lengths.Field.Name}EndingPos;");
                                if (data.BinaryOverlayFallback == BinaryGenerationType.Custom)
                                {
                                    fg.AppendLine($"partial void Custom{lengths.Field.Name}EndPos();");
                                }
                            }
                        }
                        foreach (var mod in Gen.GenerationModules)
                        {
                            await mod.GenerateInField(obj, lengths.Field, fg, LoquiInterfaceType.IGetter);
                        }
                    }
                }

                if (obj.GetObjectType() != ObjectType.Mod)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"partial void CustomFactoryEnd"))
                    {
                        args.Add($"{nameof(OverlayStream)} stream");
                        args.Add($"{(obj.GetObjectType() == ObjectType.Mod ? "long" : "int")} finalPos");
                        args.Add($"int offset");
                    }
                    if (objData.CustomBinaryEnd != CustomEnd.Off)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"partial void CustomEnd"))
                        {
                            args.Add($"{nameof(OverlayStream)} stream");
                            args.Add($"int finalPos");
                            args.Add($"int offset");
                        }
                    }
                    fg.AppendLine();
                    using (var args = new ArgsWrapper(fg,
                        $"partial void CustomCtor"))
                    {
                    }
                }

                using (var args = new FunctionWrapper(fg,
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
                    using (new DepthWrapper(fg))
                    {
                        using (var args = new FunctionWrapper(fg,
                            ": base"))
                        {
                            args.AddPassArg("bytes");
                            args.AddPassArg($"package");
                        }
                    }
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.GetObjectType() == ObjectType.Mod)
                    {
                        fg.AppendLine("this.ModKey = modKey;");
                        if (objData.GameReleaseOptions != null)
                        {
                            fg.AppendLine($"this.{ModModule.ReleaseEnumName(obj)} = release;");
                        }
                        fg.AppendLine("this._data = stream;");
                        using (var args = new ArgsWrapper(fg,
                            $"this._package = new {nameof(BinaryOverlayFactoryPackage)}"))
                        {
                            args.Add($"stream.{nameof(IMutagenReadStream.MetaData)}");
                        }
                        fg.AppendLine("this._shouldDispose = shouldDispose;");
                    }
                    foreach (var field in obj.IterateFields(
                        expandSets: SetMarkerType.ExpandSets.FalseAndInclude,
                        nonIntegrated: true))
                    {
                        if (!this.TryGetTypeGeneration(field.GetType(), out var typeGen)) continue;
                        typeGen.GenerateWrapperCtor(
                            fg: fg,
                            objGen: obj,
                            typeGen: field);
                    }
                    if (obj.GetObjectType() != ObjectType.Mod)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"this.CustomCtor"))
                        {
                        }
                    }
                }
                fg.AppendLine();

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
                        using (var args = new FunctionWrapper(fg,
                            $"public static {this.BinaryOverlayClass(obj)} {obj.Name}Factory"))
                        {
                            args.Add($"ReadOnlyMemorySlice<byte> data");
                            args.Add("ModKey modKey");
                            if (objData.GameReleaseOptions != null)
                            {
                                args.Add($"{ModModule.ReleaseEnumName(obj)} release");
                            }
                            if (objData.UsesStringFiles)
                            {
                                args.Add($"{nameof(IStringsFolderLookup)}? stringsLookup = null");
                            }
                        }
                        using (new BraceWrapper(fg))
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"return {obj.Name}Factory"))
                            {
                                fg.AppendLine($"var meta = new {nameof(ParsingBundle)}({gameReleaseStr}, new {nameof(MasterReferenceReader)}(modKey));");
                                fg.AppendLine($"meta.{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordInfoCache)}(() => new {nameof(MutagenMemoryReadStream)}(data, meta));");
                                if (objData.UsesStringFiles)
                                {
                                    fg.AppendLine($"meta.{nameof(ParsingBundle.StringsLookup)} = stringsLookup;");
                                }
                                if (objData.GameReleaseOptions != null)
                                {
                                    args.AddPassArg("release");
                                }
                                args.Add(subFg =>
                                {
                                    using (var subArgs = new FunctionWrapper(subFg,
                                        $"stream: new {nameof(MutagenMemoryReadStream)}"))
                                    {
                                        subArgs.AddPassArg("data");
                                        subArgs.Add($"metaData: meta");
                                    }
                                });
                                args.AddPassArg("modKey");
                                args.Add("shouldDispose: false");
                            }
                        }
                        fg.AppendLine();

                        using (var args = new FunctionWrapper(fg,
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
                        }
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"var meta = new {nameof(ParsingBundle)}({gameReleaseStr}, new {nameof(MasterReferenceReader)}(path.ModKey))");
                            using (new BraceWrapper(fg) { AppendSemicolon = true })
                            {
                                fg.AppendLine($"{nameof(ParsingBundle.RecordInfoCache)} = new {nameof(RecordInfoCache)}(() => new {nameof(MutagenBinaryReadStream)}(path, {gameReleaseStr}))");
                            }
                            using (var args = new ArgsWrapper(fg,
                                $"var stream = new {nameof(MutagenBinaryReadStream)}"))
                            {
                                args.Add($"path: path.{nameof(ModPath.Path)}");
                                args.Add($"metaData: meta");
                            }
                            fg.AppendLine("try");
                            using (new BraceWrapper(fg))
                            {
                                if (objData.UsesStringFiles)
                                {
                                    fg.AppendLine("if (stream.Remaining < 12)");
                                    using (new BraceWrapper(fg))
                                    {
                                        fg.AppendLine($"throw new ArgumentException(\"File stream was too short to parse flags\");");
                                    }
                                    fg.AppendLine($"var flags = stream.GetInt32(offset: 8);");
                                    fg.AppendLine($"if (EnumExt.HasFlag(flags, (int)ModHeaderCommonFlag.Localized))");
                                    using (new BraceWrapper(fg))
                                    {
                                        fg.AppendLine($"meta.StringsLookup = StringsFolderLookupOverlay.TypicalFactory({gameReleaseStr}, path.{nameof(ModPath.ModKey)}, Path.GetDirectoryName(path.{nameof(ModPath.Path)})!, stringsParam);");
                                    }
                                }

                                using (var args = new ArgsWrapper(fg,
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
                            fg.AppendLine("catch (Exception)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("stream.Dispose();");
                                fg.AppendLine("throw;");
                            }
                        }
                        fg.AppendLine();
                    }

                    using (var args = new FunctionWrapper(fg,
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
                            args.Add($"{nameof(RecordTypeConverter)}? recordTypeConverter = null");
                        }
                    }
                    using (new BraceWrapper(fg))
                    {
                        if (await obj.IsMajorRecord())
                        {
                            if (objData.CustomBinaryEnd != CustomEnd.Off)
                            {
                                fg.AppendLine("var origStream = stream;");
                            }
                            fg.AppendLine($"stream = {nameof(UtilityTranslation)}.{nameof(UtilityTranslation.DecompressStream)}(stream);");
                        }
                        if (obj.TryGetCustomRecordTypeTriggers(out var customLogicTriggers))
                        {
                            fg.AppendLine($"var nextRecord = recordTypeConverter.ConvertToCustom(stream.Get{(obj.GetObjectType() == ObjectType.Subrecord ? "Subrecord" : "MajorRecord")}().RecordType);");
                            fg.AppendLine($"switch (nextRecord.TypeInt)");
                            using (new BraceWrapper(fg))
                            {
                                foreach (var item in customLogicTriggers)
                                {
                                    fg.AppendLine($"case {item.TypeInt}: // {item.Type}");
                                }
                                using (new DepthWrapper(fg))
                                {
                                    using (var args = new ArgsWrapper(fg,
                                        "return CustomRecordTypeTrigger"))
                                    {
                                        args.AddPassArg($"stream");
                                        args.Add("recordType: nextRecord");
                                        args.AddPassArg("package");
                                        args.AddPassArg("recordTypeConverter");
                                    }
                                }
                                fg.AppendLine("default:");
                                using (new DepthWrapper(fg))
                                {
                                    fg.AppendLine("break;");
                                }
                            }
                        }
                        using (var args = new ArgsWrapper(fg,
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
                                        args.Add($"bytes: {nameof(HeaderTranslation)}.{nameof(HeaderTranslation.ExtractSubrecordMemory)}(stream.RemainingMemory, package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)})");
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
                            fg.AppendLine($"int offset = stream.Position;");
                        }
                        else
                        {
                            switch (obj.GetObjectType())
                            {
                                case ObjectType.Subrecord:
                                    fg.AppendLine($"var finalPos = checked((int)(stream.Position + stream.GetSubrecord().TotalLength));");
                                    fg.AppendLine($"int offset = stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.TypeAndLengthLength;");
                                    break;
                                case ObjectType.Record:
                                    fg.AppendLine($"var finalPos = checked((int)(stream.Position + stream.GetMajorRecord().TotalLength));");
                                    fg.AppendLine($"int offset = stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.MajorConstants.TypeAndLengthLength;");
                                    break;
                                case ObjectType.Group:
                                    fg.AppendLine($"var finalPos = checked((int)(stream.Position + stream.GetGroup().TotalLength));");
                                    fg.AppendLine($"int offset = stream.Position + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.GroupConstants.TypeAndLengthLength;");
                                    break;
                                case ObjectType.Mod:
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }

                        if (await obj.IsMajorRecord())
                        {
                            fg.AppendLine("ret._package.FormVersion = ret;");
                        }

                        // Parse struct section ending positions 
                        string structPassedAccessor = null;
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
                                    fg.AppendLine($"ret.Custom{lengths.Field.Name}EndPos();");
                                    break;
                                case BinaryGenerationType.NoGeneration:
                                    break;
                                case BinaryGenerationType.Normal:
                                    await typeGen.GenerateWrapperUnknownLengthParse(
                                        fg,
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
                                            fg.AppendLine($"stream.Position += {structPassedAccessor} + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.SubConstants.TypeAndLengthLength;");
                                            break;
                                        case ObjectType.Record:
                                            fg.AppendLine($"stream.Position += {structPassedAccessor} + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.MajorConstants.TypeAndLengthLength;");
                                            break;
                                        case ObjectType.Group:
                                            fg.AppendLine($"stream.Position += {structPassedAccessor} + package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.Constants)}.GroupConstants.TypeAndLengthLength;");
                                            break;
                                        case ObjectType.Mod:
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
                                }
                                using (var args = new ArgsWrapper(fg,
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
                            using (var args = new ArgsWrapper(fg,
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
                                    args.AddPassArg($"recordTypeConverter");
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
                                            fg.AppendLine($"if (ret._data.Length <= {lengths.PassedAccessor})");
                                            using (new BraceWrapper(fg))
                                            {
                                                fg.AppendLine($"ret.{VersioningModule.VersioningFieldName} |= {obj.ObjectName}.{VersioningModule.VersioningEnumName}.Break{breakIndex++};");
                                            }
                                        }
                                    }
                                    // Not advancing stream position, but only because breaks only occur in situations 
                                    // that stream position doesn't matter 
                                }
                                else if (structPassedAccessor != null)
                                {
                                    fg.AppendLine($"stream.Position += {structPassedAccessor};");
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
                                            fg.AppendLine($"if (ret._data.Length <= {lengths.PassedAccessor})");
                                            using (new BraceWrapper(fg))
                                            {
                                                fg.AppendLine($"ret.{VersioningModule.VersioningFieldName} |= {obj.ObjectName}.{VersioningModule.VersioningEnumName}.Break{breakIndex++};");
                                            }
                                        }
                                    }
                                    // Not advancing stream position, but only because breaks only occur in situations 
                                    // that stream position doesn't matter 
                                }
                                else if (totalPassedLength != null)
                                {
                                    fg.AppendLine($"stream.Position += 0x{totalPassedLength.Value:X}{headerAddition};");
                                }
                            }
                            using (var args = new ArgsWrapper(fg,
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
                                    fg,
                                    obj,
                                    lengths.Field,
                                    lengths.PassedLength,
                                    lengths.PassedAccessor);
                            }
                        }

                        if (objData.CustomBinaryEnd != CustomEnd.Off)
                        {
                            using (var args = new ArgsWrapper(fg,
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
                        fg.AppendLine("return ret;");
                    }
                    fg.AppendLine();

                    if (obj.GetObjectType() != ObjectType.Mod)
                    {
                        using (var args = new FunctionWrapper(fg,
                            $"public static {this.BinaryOverlayClass(obj)} {obj.Name}Factory"))
                        {
                            args.Add($"ReadOnlyMemorySlice<byte> slice");
                            args.Add($"{nameof(BinaryOverlayFactoryPackage)} package");
                            args.Add($"{nameof(RecordTypeConverter)}? recordTypeConverter = null");
                        }
                        using (new BraceWrapper(fg))
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"return {obj.Name}Factory"))
                            {
                                args.Add($"stream: new {nameof(OverlayStream)}(slice, package)");
                                args.AddPassArg("package");
                                if (obj.IsVariableLengthStruct())
                                {
                                    args.Add($"finalPos: slice.Length");
                                }
                                args.AddPassArg("recordTypeConverter");
                            }
                        }
                    }
                }
                fg.AppendLine();

                await GenerateOverlayExtras(obj, fg);

                await obj.GenerateToStringCode(fg);

                obj.GenerateEqualsSection(fg);
                await MajorRecordLinkEqualityModule.Generate(obj, fg);

                if (obj.GetObjectType() == ObjectType.Mod)
                {
                    fg.AppendLine($"IMask<bool> {nameof(IEqualsMask)}.{nameof(IEqualsMask.GetEqualsMask)}(object rhs, EqualsMaskHelper.Include include = EqualsMaskHelper.Include.OnlyFailures) => {obj.MixInClassName}.GetEqualsMask(this, ({obj.Interface(getter: true, internalInterface: true)})rhs, include);");
                }

            }
            fg.AppendLine();
        }

    }
}
