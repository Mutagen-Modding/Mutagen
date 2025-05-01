using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

partial class PerkRankBinaryCreateTranslation
{
    public record Payload
    {
        public SubrecordFrame? DATA { get; set; }
        public SubrecordFrame? EPF2 { get; set; }
        public SubrecordFrame? EPF3 { get; set; }
        public SubrecordFrame? EPFD { get; set; }
        public SubrecordFrame? EPFT { get; set; }
        public SubrecordFrame? EPFB { get; set; }
        public List<PerkCondition>? Conditions { get; set; }
    }

    private static Payload ReadPayload<TStream>(TStream stream)
        where TStream : IMutagenReadStream
    {
        var ret = new Payload();
        while (stream.TryReadSubrecord(out var subFrame))
        {
            switch (subFrame.RecordTypeInt)
            {
                case RecordTypeInts.DATA:
                    ret.DATA = subFrame;
                    break;
                case RecordTypeInts.EPF2:
                    ret.EPF2 = subFrame;
                    break;
                case RecordTypeInts.EPF3:
                    ret.EPF3 = subFrame;
                    break;
                case RecordTypeInts.EPFD:
                    ret.EPFD = subFrame;
                    break;
                case RecordTypeInts.EPFT:
                    ret.EPFT = subFrame;
                    break;
                case RecordTypeInts.EPFB:
                    ret.EPFB = subFrame;
                    break;
                case RecordTypeInts.PRKC:
                case RecordTypeInts.CTDA:
                    stream.Position -= subFrame.TotalLength;
                    ret.Conditions = ListBinaryTranslation<PerkCondition>.Instance.Parse(
                        reader: new MutagenFrame(stream),
                        transl: (MutagenFrame r, [MaybeNullWhen(false)] out PerkCondition listSubItem) =>
                        {
                            return LoquiBinaryTranslation<PerkCondition>.Instance.Parse(
                                frame: r,
                                item: out listSubItem!);
                        });
                    break;
                default:
                    stream.Position -= subFrame.TotalLength;
                    return ret;
            }
        }

        return ret;
    }
    public static IEnumerable<APerkEffect> ParseEffects(IMutagenReadStream stream)
    {
        while (stream.TryReadSubrecord(RecordTypes.PRKE, out var prkeFrame))
        {
            var type = (Perk.EffectType)prkeFrame.Content[0];
            var rank = prkeFrame.Content[1];
            var priority = prkeFrame.Content[2];
            
            var payload = ReadPayload(stream);

            APerkEffect effect;
            if (payload.DATA != null)
            {
                switch (type)
                {
                    case Perk.EffectType.Quest:
                        effect = new PerkQuestEffect()
                        {
                            Quest = FormLinkBinaryTranslation.Instance.Factory<IQuestGetter>(stream.MetaData, payload.DATA.Value.Content),
                            Stage = BinaryPrimitives.ReadUInt16LittleEndian(payload.DATA.Value.Content.Slice(4)),
                        };
                        effect.Conditions.SetTo(
                            Plugins.Binary.Translations.ListBinaryTranslation<PerkCondition>.Instance.Parse(
                                reader: new MutagenFrame(stream),
                                transl: (MutagenFrame r, [MaybeNullWhen(false)] out PerkCondition listSubItem) =>
                                {
                                    return LoquiBinaryTranslation<PerkCondition>.Instance.Parse(
                                        frame: r,
                                        item: out listSubItem!);
                                }));
                        break;
                    case Perk.EffectType.Ability:
                        effect = new PerkAbilityEffect()
                        {
                            Ability = FormLinkBinaryTranslation.Instance.Factory<ISpellGetter>(stream.MetaData, payload.DATA.Value.Content),
                        };
                        effect.Conditions.SetTo(
                            Plugins.Binary.Translations.ListBinaryTranslation<PerkCondition>.Instance.Parse(
                                reader: new MutagenFrame(stream),
                                transl: (MutagenFrame r, [MaybeNullWhen(false)] out PerkCondition listSubItem) =>
                                {
                                    return LoquiBinaryTranslation<PerkCondition>.Instance.Parse(
                                        frame: r,
                                        item: out listSubItem!);
                                }));
                        break;
                    case Perk.EffectType.EntryPoint:
                        var entryPt = (APerkEntryPointEffect.EntryType)payload.DATA.Value.Content[0];
                        var func = (APerkEntryPointEffect.FunctionType)payload.DATA.Value.Content[1];
                        var tabCount = payload.DATA.Value.Content[2];
                        APerkEntryPointEffect entryPointEffect;
                        switch (func)
                        {
                            case APerkEntryPointEffect.FunctionType.SetValue:
                            case APerkEntryPointEffect.FunctionType.AddValue:
                            case APerkEntryPointEffect.FunctionType.MultiplyValue:
                                if (payload.EPFT == null && payload.EPFD == null)
                                {
                                    entryPointEffect = new PerkEntryPointModifyValue()
                                    {
                                        Value = null,
                                        Modification = func switch
                                        {
                                            APerkEntryPointEffect.FunctionType.SetValue => PerkEntryPointModifyValue.ModificationType.Set,
                                            APerkEntryPointEffect.FunctionType.MultiplyValue => PerkEntryPointModifyValue.ModificationType.Multiply,
                                            APerkEntryPointEffect.FunctionType.AddValue => PerkEntryPointModifyValue.ModificationType.Add,
                                            _ => throw new MalformedDataException(),
                                        }
                                    };
                                }
                                else
                                {
                                    if (!payload.EPFT.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyValue)} did not have expected EPFT record");
                                    if (!payload.EPFD.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyValue)} did not have expected EPFD record");
                                    var epftType = (APerkEntryPointEffect.ParameterType)payload.EPFT.Value.Content[0];
                                    if (epftType != APerkEntryPointEffect.ParameterType.Float
                                        && epftType != APerkEntryPointEffect.ParameterType.FloatFloat)
                                    {
                                        throw new MalformedDataException($"{nameof(PerkEntryPointModifyValue)} did not have expected parameter type flag: {(APerkEntryPointEffect.ParameterType)payload.EPFT.Value.Content[0]}");
                                    }

                                    if (epftType == APerkEntryPointEffect.ParameterType.Float)
                                    {
                                        entryPointEffect = new PerkEntryPointModifyValue()
                                        {
                                            Value = payload.EPFD.Value.Content.Float(),
                                            Modification = func switch
                                            {
                                                APerkEntryPointEffect.FunctionType.SetValue => PerkEntryPointModifyValue.ModificationType.Set,
                                                APerkEntryPointEffect.FunctionType.MultiplyValue => PerkEntryPointModifyValue.ModificationType.Multiply,
                                                APerkEntryPointEffect.FunctionType.AddValue => PerkEntryPointModifyValue.ModificationType.Add,
                                                _ => throw new MalformedDataException(),
                                            }
                                        };
                                    }
                                    else
                                    {
                                        entryPointEffect = new PerkEntryPointModifyValues()
                                        {
                                            Value = payload.EPFD.Value.Content.Float(),
                                            Value2 = payload.EPFD.Value.Content.Slice(4).Float(),
                                            Modification = func switch
                                            {
                                                APerkEntryPointEffect.FunctionType.SetValue => PerkEntryPointModifyValue.ModificationType.Set,
                                                APerkEntryPointEffect.FunctionType.MultiplyValue => PerkEntryPointModifyValue.ModificationType.Multiply,
                                                APerkEntryPointEffect.FunctionType.AddValue => PerkEntryPointModifyValue.ModificationType.Add,
                                                _ => throw new MalformedDataException(),
                                            }
                                        };
                                    }
                                }
                                break;
                            case APerkEntryPointEffect.FunctionType.AddRangeToValue:
                                if (!payload.EPFT.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointAddRangeToValue)} did not have expected EPFT record");
                                if (!payload.EPFD.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointAddRangeToValue)} did not have expected EPFD record");
                                if (payload.EPFT.Value.Content[0] != (byte)APerkEntryPointEffect.ParameterType.FloatFloat)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointAddRangeToValue)} did not have expected parameter type flag: {(APerkEntryPointEffect.ParameterType)payload.EPFT.Value.Content[0]}");
                                }
                                entryPointEffect = new PerkEntryPointAddRangeToValue()
                                {
                                    From = payload.EPFD.Value.Content.Float(),
                                    To = payload.EPFD.Value.Content.Slice(4).Float(),
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.SetToActorValueMult:
                            case APerkEntryPointEffect.FunctionType.MultiplyActorValueMult:
                            case APerkEntryPointEffect.FunctionType.MultiplyOnePlusActorValueMult:
                            case APerkEntryPointEffect.FunctionType.AddActorValueMult:
                                if (!payload.EPFT.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFT record");
                                if (!payload.EPFD.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFD record");
                                if (payload.EPFT.Value.Content[0] != (byte)APerkEntryPointEffect.ParameterType.ActorValue)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected parameter type flag: {(APerkEntryPointEffect.ParameterType)payload.EPFT.Value.Content[0]}");
                                }
                                entryPointEffect = new PerkEntryPointModifyActorValue()
                                {
                                    ActorValue = FormLinkBinaryTranslation.Instance.Factory<IActorValueInformationGetter>(stream.MetaData, payload.EPFD.Value.Content),
                                    Value = payload.EPFD.Value.Content.Slice(4).Float(),
                                    Modification = func switch
                                    {
                                        APerkEntryPointEffect.FunctionType.SetToActorValueMult => PerkEntryPointModifyActorValue.ModificationType.SetToAVMult,
                                        APerkEntryPointEffect.FunctionType.AddActorValueMult => PerkEntryPointModifyActorValue.ModificationType.AddAVMult,
                                        APerkEntryPointEffect.FunctionType.MultiplyActorValueMult => PerkEntryPointModifyActorValue.ModificationType.MultiplyAVMult,
                                        APerkEntryPointEffect.FunctionType.MultiplyOnePlusActorValueMult => PerkEntryPointModifyActorValue.ModificationType.MultiplyOnePlusAVMult,
                                        _ => throw new MalformedDataException(),
                                    }
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.AbsoluteValue:
                            case APerkEntryPointEffect.FunctionType.NegativeAbsoluteValue:
                                if (payload.EPFT.HasValue && payload.EPFT.Value.Content[0] != (byte)APerkEntryPointEffect.ParameterType.None)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointAbsoluteValue)} did not have expected parameter type flag: {(APerkEntryPointEffect.ParameterType)payload.EPFT.Value.Content[0]}");
                                }
                                entryPointEffect = new PerkEntryPointAbsoluteValue()
                                {
                                    Negative = func == APerkEntryPointEffect.FunctionType.NegativeAbsoluteValue
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.AddLeveledList:
                                if (!payload.EPFT.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFT record");
                                if (payload.EPFT.Value.Content[0] != (byte)APerkEntryPointEffect.ParameterType.LeveledItem)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointAddLeveledItem)} did not have expected parameter type flag: {(APerkEntryPointEffect.ParameterType)payload.EPFT.Value.Content[0]}");
                                }
                                entryPointEffect = new PerkEntryPointAddLeveledItem()
                                {
                                    Item = FormLinkBinaryTranslation.Instance.Factory<ILeveledItemGetter>(stream.MetaData, payload.EPFD?.Content)
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.AddActivateChoice:
                                if (!payload.EPFT.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFT record");
                                if (!payload.EPF3.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPF3 record");
                                if (payload.EPFT.Value.Content[0] != (byte)APerkEntryPointEffect.ParameterType.SpellWithStrings)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointAddActivateChoice)} did not have expected parameter type flag: {(APerkEntryPointEffect.ParameterType)payload.EPFT.Value.Content[0]}");
                                }
                                entryPointEffect = new PerkEntryPointAddActivateChoice()
                                {
                                    Spell = FormLinkBinaryTranslation.Instance.FactoryNullable<ISpellGetter>(stream.MetaData, payload.EPFD?.Content),
                                    ButtonLabel = payload.EPF2.HasValue ? StringBinaryTranslation.Instance.Parse(payload.EPF2.Value.Content, StringsSource.Normal, stream.MetaData, eager: true) : null,
                                    Flags = (PerkEntryPointAddActivateChoice.Flag)BinaryPrimitives.ReadInt16LittleEndian(payload.EPF3.Value.Content),
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.SelectSpell:
                                if (!payload.EPFT.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFT record");
                                if (payload.EPFT.Value.Content[0] != (byte)APerkEntryPointEffect.ParameterType.Spell)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointSelectSpell)} did not have expected parameter type flag: {(APerkEntryPointEffect.ParameterType)payload.EPFT.Value.Content[0]}");
                                }
                                entryPointEffect = new PerkEntryPointSelectSpell()
                                {
                                    Spell = FormLinkBinaryTranslation.Instance.Factory<ISpellGetter>(stream.MetaData, payload.EPFD?.Content),
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.SelectText:
                                if (!payload.EPFT.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFT record");
                                if (payload.EPFT.Value.Content[0] != (byte)APerkEntryPointEffect.ParameterType.String)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointSelectText)} did not have expected parameter type flag: {(APerkEntryPointEffect.ParameterType)payload.EPFT.Value.Content[0]}");
                                }
                                entryPointEffect = new PerkEntryPointSelectText()
                                {
                                    Text = payload.EPFD.HasValue ? BinaryStringUtility.ProcessWholeToZString(payload.EPFD.Value.Content, stream.MetaData.Encodings.NonTranslated) : string.Empty
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.SetText:
                                if (!payload.EPFT.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFT record");
                                if (payload.EPFT.Value.Content[0] != (byte)APerkEntryPointEffect.ParameterType.LString)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointSetText)} did not have expected parameter type flag: {(APerkEntryPointEffect.ParameterType)payload.EPFT.Value.Content[0]}");
                                }
                                entryPointEffect = new PerkEntryPointSetText()
                                {
                                    Text = payload.EPFD.HasValue ? StringBinaryTranslation.Instance.Parse(payload.EPFD.Value.Content, StringsSource.Normal, stream.MetaData, eager: true) : (TranslatedString)string.Empty,
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.LegendaryMagicEffectEvent:
                                if (!payload.EPFT.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFT record");
                                if (payload.EPFT.Value.Content[0] != (byte)APerkEntryPointEffect.ParameterType.ReplacementProjectile)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointLegendaryMagicEffectEvent)} did not have expected parameter type flag: {(APerkEntryPointEffect.ParameterType)payload.EPFT.Value.Content[0]}");
                                }

                                entryPointEffect = new PerkEntryPointLegendaryMagicEffectEvent()
                                {
                                    Projectile = FormLinkBinaryTranslation.Instance.Factory<IProjectileGetter>(stream.MetaData, payload.EPFD?.Content),
                                };
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        entryPointEffect.EntryPoint = entryPt;
                        entryPointEffect.PerkConditionTabCount = tabCount;
                        effect = entryPointEffect;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                effect = type switch
                {
                    Perk.EffectType.Quest => new PerkQuestEffect(),
                    Perk.EffectType.Ability => new PerkAbilityEffect(),
                    _ => throw new MalformedDataException($"Expected DATA subrecord that did not exist."),
                };
            }
            effect.Rank = rank;
            effect.Priority = priority;
            if (payload.Conditions != null)
            {
                effect.Conditions.SetTo(payload.Conditions);
            }
            if (payload.EPF2 != null)
            {
                effect.ButtonLabel = StringBinaryTranslation.Instance.Parse(payload.EPF2.Value.Content, StringsSource.Normal, stream.MetaData, eager: true);
            }
            if (payload.EPF3 != null)
            {
                effect.Flags = (APerkEffect.Flag)payload.EPF3.Value.AsInt16();
            }
            if (payload.EPFB != null)
            {
                effect.PerkEntryID = payload.EPFB.Value.AsUInt16();
            }

            if (stream.TryReadSubrecord(RecordTypes.EPFT, out var epftFrame)
                && epftFrame.ContentLength != 1
                && epftFrame.Content[0] != (byte)APerkEntryPointEffect.ParameterType.None)
            {
                throw new MalformedDataException($"Encountered an unexpected epft frame.");
            }
            stream.TryReadSubrecord(RecordTypes.PRKF, out var _);
            yield return effect;
        }
    }

    public static partial void FillBinaryEffectsCustom(MutagenFrame frame, IPerkRank item, PreviousParse lastParsed)
    {
        item.Effects.SetTo(ParseEffects(frame.Reader));
    }
}

partial class PerkRankBinaryWriteTranslation
{
    public static partial void WriteBinaryEffectsCustom(MutagenWriter writer, IPerkRankGetter item)
    {
        foreach (var effect in item.Effects)
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.PRKE))
            {
                writer.Write((byte)(effect switch
                {
                    PerkQuestEffect quest => Perk.EffectType.Quest,
                    PerkAbilityEffect ab => Perk.EffectType.Ability,
                    APerkEntryPointEffect ep => Perk.EffectType.EntryPoint,
                    _ => throw new NotImplementedException()
                }));
                writer.Write(effect.Rank);
                writer.Write(effect.Priority);
            }
            using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
            {
                switch (effect)
                {
                    case PerkQuestEffect quest:
                        FormKeyBinaryTranslation.Instance.Write(writer, quest.Quest);
                        writer.Write(quest.Stage);
                        break;
                    case PerkAbilityEffect ability:
                        FormKeyBinaryTranslation.Instance.Write(writer, ability.Ability);
                        break;
                    case APerkEntryPointEffect entryPt:
                        writer.Write((byte)entryPt.EntryPoint);
                        writer.Write((byte)(entryPt switch
                        {
                            PerkEntryPointModifyValue modVal => modVal.Modification switch
                            {
                                PerkEntryPointModifyValue.ModificationType.Add => APerkEntryPointEffect.FunctionType.AddValue,
                                PerkEntryPointModifyValue.ModificationType.Set => APerkEntryPointEffect.FunctionType.SetValue,
                                PerkEntryPointModifyValue.ModificationType.Multiply => APerkEntryPointEffect.FunctionType.MultiplyValue,
                                _ => throw new NotImplementedException()
                            },
                            PerkEntryPointModifyValues modVals => modVals.Modification switch
                            {
                                PerkEntryPointModifyValue.ModificationType.Add => APerkEntryPointEffect.FunctionType.AddValue,
                                PerkEntryPointModifyValue.ModificationType.Set => APerkEntryPointEffect.FunctionType.SetValue,
                                PerkEntryPointModifyValue.ModificationType.Multiply => APerkEntryPointEffect.FunctionType.MultiplyValue,
                                _ => throw new NotImplementedException()
                            },
                            PerkEntryPointAddRangeToValue addRange => APerkEntryPointEffect.FunctionType.AddRangeToValue,
                            PerkEntryPointModifyActorValue modActorVal => modActorVal.Modification switch
                            {
                                PerkEntryPointModifyActorValue.ModificationType.AddAVMult => APerkEntryPointEffect.FunctionType.AddActorValueMult,
                                PerkEntryPointModifyActorValue.ModificationType.MultiplyAVMult => APerkEntryPointEffect.FunctionType.MultiplyActorValueMult,
                                PerkEntryPointModifyActorValue.ModificationType.MultiplyOnePlusAVMult => APerkEntryPointEffect.FunctionType.MultiplyOnePlusActorValueMult,
                                PerkEntryPointModifyActorValue.ModificationType.SetToAVMult => APerkEntryPointEffect.FunctionType.SetToActorValueMult,
                                _ => throw new NotImplementedException()
                            },
                            PerkEntryPointAbsoluteValue absVal => absVal.Negative ? APerkEntryPointEffect.FunctionType.NegativeAbsoluteValue : APerkEntryPointEffect.FunctionType.AbsoluteValue,
                            PerkEntryPointAddLeveledItem levItem => APerkEntryPointEffect.FunctionType.AddLeveledList,
                            PerkEntryPointAddActivateChoice activeChoice => APerkEntryPointEffect.FunctionType.AddActivateChoice,
                            PerkEntryPointSelectSpell spell => APerkEntryPointEffect.FunctionType.SelectSpell,
                            PerkEntryPointSelectText text => APerkEntryPointEffect.FunctionType.SelectText,
                            PerkEntryPointSetText ltext => APerkEntryPointEffect.FunctionType.SetText,
                            PerkEntryPointLegendaryMagicEffectEvent leg => APerkEntryPointEffect.FunctionType.LegendaryMagicEffectEvent,
                            _ => throw new NotImplementedException()
                        }));
                        writer.Write(entryPt.PerkConditionTabCount);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            Plugins.Binary.Translations.ListBinaryTranslation<IPerkConditionGetter>.Instance.Write(
                writer,
                effect.Conditions,
                (w, i) => i.WriteToBinary(w));
            if (effect is APerkEntryPointEffect ptEffect)
            {
                var paramType = effect switch
                {
                    PerkEntryPointModifyValue _ => APerkEntryPointEffect.ParameterType.Float,
                    PerkEntryPointModifyValues _ => APerkEntryPointEffect.ParameterType.FloatFloat,
                    PerkEntryPointAddRangeToValue _ => APerkEntryPointEffect.ParameterType.FloatFloat,
                    PerkEntryPointModifyActorValue _ => APerkEntryPointEffect.ParameterType.ActorValue,
                    PerkEntryPointAbsoluteValue _ => APerkEntryPointEffect.ParameterType.None,
                    PerkEntryPointAddLeveledItem _ => APerkEntryPointEffect.ParameterType.LeveledItem,
                    PerkEntryPointAddActivateChoice _ => APerkEntryPointEffect.ParameterType.SpellWithStrings,
                    PerkEntryPointSelectSpell _ => APerkEntryPointEffect.ParameterType.Spell,
                    PerkEntryPointSelectText _ => APerkEntryPointEffect.ParameterType.String,
                    PerkEntryPointSetText _ => APerkEntryPointEffect.ParameterType.LString,
                    PerkEntryPointLegendaryMagicEffectEvent _ => APerkEntryPointEffect.ParameterType.ReplacementProjectile,
                    _ => throw new NotImplementedException()
                };
                if ((effect is not PerkEntryPointModifyValue modValEpft
                    || modValEpft.Value.HasValue)
                    && paramType != APerkEntryPointEffect.ParameterType.None)
                {
                    using (HeaderExport.Subrecord(writer, RecordTypes.EPFT))
                    {
                        writer.Write((byte)paramType);
                    }
                }

                if (ptEffect.PerkEntryID is { } id)
                {
                    using (HeaderExport.Subrecord(writer, RecordTypes.EPFB))
                    {
                        writer.Write(id);
                    }
                }

                if (effect is PerkEntryPointAddActivateChoice choice)
                {
                    if (choice.ButtonLabel != null)
                    {
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPF2))
                        {
                            StringBinaryTranslation.Instance.Write(writer, choice.ButtonLabel, StringBinaryType.NullTerminate, StringsSource.Normal);
                        }
                    }
                    using (HeaderExport.Subrecord(writer, RecordTypes.EPF3))
                    {
                        EnumBinaryTranslation<PerkEntryPointAddActivateChoice.Flag, MutagenFrame, MutagenWriter>.Instance.Write(
                            writer,
                            choice.Flags,
                            2);
                    }
                }
                switch (effect)
                {
                    case PerkEntryPointModifyValue modVal:
                    {
                        if (modVal.Value is {} f)
                        {
                            using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                            {
                                writer.Write(f);
                            }
                        }
                    }
                        break;
                    case PerkEntryPointModifyValues modVal:
                    {
                        if (modVal.Value is not null || modVal.Value2 is not null)
                        {
                            using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                            {
                                writer.Write(modVal.Value ?? 0f);
                                writer.Write(modVal.Value2 ?? 0f);
                            }
                        }
                    }
                        break;
                    case PerkEntryPointAddRangeToValue range:
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                        {
                            writer.Write(range.From);
                            writer.Write(range.To);
                        }
                        break;
                    case PerkEntryPointModifyActorValue actorVal:
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                        {
                            FormLinkBinaryTranslation.Instance.Write(writer, actorVal.ActorValue);
                            writer.Write(actorVal.Value);
                        }
                        break;
                    case PerkEntryPointAddLeveledItem lev:
                        FormKeyBinaryTranslation.Instance.Write(writer, lev.Item, RecordTypes.EPFD);
                        break;
                    case PerkEntryPointAddActivateChoice activateChoice:
                        FormKeyBinaryTranslation.Instance.Write(writer, activateChoice.Spell, RecordTypes.EPFD);
                        break;
                    case PerkEntryPointSelectSpell spell:
                        FormKeyBinaryTranslation.Instance.Write(writer, spell.Spell, RecordTypes.EPFD);
                        break;
                    case PerkEntryPointSelectText text:
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                        {
                            writer.Write(text.Text, StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);
                        }
                        break;
                    case PerkEntryPointSetText ltext:
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                        {
                            StringBinaryTranslation.Instance.Write(writer, ltext.Text, StringBinaryType.NullTerminate, StringsSource.Normal);
                        }
                        break;
                    case PerkEntryPointAbsoluteValue abs:
                        break;
                    case PerkEntryPointLegendaryMagicEffectEvent leg:
                        FormKeyBinaryTranslation.Instance.Write(writer, leg.Projectile, RecordTypes.EPFD);
                        
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            using (HeaderExport.Subrecord(writer, RecordTypes.PRKF)) { }
        }
    }
}

partial class PerkRankBinaryOverlay
{
    public IReadOnlyList<IAPerkEffectGetter> Effects { get; private set; } = Array.Empty<IAPerkEffectGetter>();

    partial void EffectsCustomParse(
        OverlayStream stream,
        int finalPos,
        int offset,
        RecordType type,
        PreviousParse lastParsed)
    {
        Effects = PerkRankBinaryCreateTranslation.ParseEffects(stream)
            .ToList();
    }
}