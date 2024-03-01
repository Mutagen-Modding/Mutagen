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
    public static IEnumerable<APerkEffect> ParseEffects(IMutagenReadStream stream)
    {
        while (stream.TryReadSubrecord(RecordTypes.PRKE, out var prkeFrame))
        {
            var type = (Perk.EffectType)prkeFrame.Content[0];
            var rank = prkeFrame.Content[1];
            var priority = prkeFrame.Content[2];
            APerkEffect effect;
            if (stream.TryReadSubrecord(RecordTypes.DATA, out var dataFrame))
            {
                switch (type)
                {
                    case Perk.EffectType.Quest:
                        effect = new PerkQuestEffect()
                        {
                            Quest = new FormLink<IQuestGetter>(FormKeyBinaryTranslation.Instance.Parse(dataFrame.Content, stream.MetaData.MasterReferences!)),
                            Stage = BinaryPrimitives.ReadUInt16LittleEndian(dataFrame.Content.Slice(4)),
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
                            Ability = new FormLink<ISpellGetter>(FormKeyBinaryTranslation.Instance.Parse(dataFrame.Content, stream.MetaData.MasterReferences!)),
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
                        var entryPt = (APerkEntryPointEffect.EntryType)dataFrame.Content[0];
                        var func = (APerkEntryPointEffect.FunctionType)dataFrame.Content[1];
                        var tabCount = dataFrame.Content[2];
                        var conditions = Plugins.Binary.Translations.ListBinaryTranslation<PerkCondition>.Instance.Parse(
                                reader: new MutagenFrame(stream),
                                triggeringRecord: PerkCondition_Registration.TriggerSpecs,
                                transl: (MutagenFrame r, [MaybeNullWhen(false)] out PerkCondition listSubItem) =>
                                {
                                    return LoquiBinaryTranslation<PerkCondition>.Instance.Parse(
                                        frame: r,
                                        item: out listSubItem!);
                                })
                            .ToList();
                        ReadOnlyMemorySlice<byte>? epf2 = null;
                        ReadOnlyMemorySlice<byte>? epf3 = null;
                        ReadOnlyMemorySlice<byte>? epfd = null;
                        ReadOnlyMemorySlice<byte>? epft = null;
                        SubrecordFrame? epfb = null;
                        while (stream.TryReadSubrecord(out var subFrame))
                        {
                            switch (subFrame.RecordTypeInt)
                            {
                                case RecordTypeInts.EPF2:
                                    epf2 = subFrame.Content;
                                    break;
                                case RecordTypeInts.EPF3:
                                    epf3 = subFrame.Content;
                                    break;
                                case RecordTypeInts.EPFD:
                                    epfd = subFrame.Content;
                                    break;
                                case RecordTypeInts.EPFT:
                                    epft = subFrame.Content;
                                    break;
                                case RecordTypeInts.EPFB:
                                    epfb = subFrame;
                                    break;
                                default:
                                    stream.Position -= subFrame.Content.Length;
                                    goto searchDone;
                            }
                        }
                    searchDone:
                        APerkEntryPointEffect entryPointEffect;
                        switch (func)
                        {
                            case APerkEntryPointEffect.FunctionType.SetValue:
                            case APerkEntryPointEffect.FunctionType.AddValue:
                            case APerkEntryPointEffect.FunctionType.MultiplyValue:
                                if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointModifyValue)} had EPF2 unexpectedly");
                                if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointModifyValue)} had EPF3 unexpectedly");
                                if (epft == null && epfd == null)
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
                                    if (!epft.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyValue)} did not have expected EPFT record");
                                    if (!epfd.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyValue)} did not have expected EPFD record");
                                    var epftType = (APerkEntryPointEffect.ParameterType)epft.Value[0];
                                    if (epftType != APerkEntryPointEffect.ParameterType.Float
                                        && epftType != APerkEntryPointEffect.ParameterType.FloatFloat)
                                    {
                                        throw new MalformedDataException($"{nameof(PerkEntryPointModifyValue)} did not have expected parameter type flag: {epft.Value[0]}");
                                    }

                                    if (epftType == APerkEntryPointEffect.ParameterType.Float)
                                    {
                                        entryPointEffect = new PerkEntryPointModifyValue()
                                        {
                                            Value = epfd.Value.Float(),
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
                                            Value = epfd.Value.Float(),
                                            Value2 = epfd.Value.Slice(4).Float(),
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
                                if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointModifyValue)} had EPF2 unexpectedly");
                                if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointModifyValue)} had EPF3 unexpectedly");
                                if (!epft.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointAddRangeToValue)} did not have expected EPFT record");
                                if (!epfd.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointAddRangeToValue)} did not have expected EPFD record");
                                if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.FloatFloat)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointAddRangeToValue)} did not have expected parameter type flag: {epft.Value[0]}");
                                }
                                entryPointEffect = new PerkEntryPointAddRangeToValue()
                                {
                                    From = epfd.Value.Float(),
                                    To = epfd.Value.Slice(4).Float(),
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.SetToActorValueMult:
                            case APerkEntryPointEffect.FunctionType.MultiplyActorValueMult:
                            case APerkEntryPointEffect.FunctionType.MultiplyOnePlusActorValueMult:
                            case APerkEntryPointEffect.FunctionType.AddActorValueMult:
                                if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointModifyValue)} had EPF2 unexpectedly");
                                if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointModifyValue)} had EPF3 unexpectedly");
                                if (!epft.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFT record");
                                if (!epfd.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFD record");
                                if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.ActorValue)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected parameter type flag: {epft.Value[0]}");
                                }
                                entryPointEffect = new PerkEntryPointModifyActorValue()
                                {
                                    ActorValue = new FormLinkNullable<IActorValueInformationGetter>(epfd.HasValue ? FormKeyBinaryTranslation.Instance.Parse(epfd.Value, stream.MetaData.MasterReferences!) : default(FormKey?)),
                                    Value = epfd.Value.Slice(4).Float(),
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
                                if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointModifyValue)} had EPF2 unexpectedly");
                                if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointModifyValue)} had EPF3 unexpectedly");
                                if (epft.HasValue && epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.None)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointAbsoluteValue)} did not have expected parameter type flag: {epft.Value[0]}");
                                }
                                entryPointEffect = new PerkEntryPointAbsoluteValue()
                                {
                                    Negative = func == APerkEntryPointEffect.FunctionType.NegativeAbsoluteValue
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.AddLeveledList:
                                if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointModifyValue)} had EPF2 unexpectedly");
                                if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointModifyValue)} had EPF3 unexpectedly");
                                if (!epft.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointAddLeveledItem)} did not have expected EPFT record");
                                if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.LeveledItem)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointAddLeveledItem)} did not have expected parameter type flag: {epft.Value[0]}");
                                }
                                entryPointEffect = new PerkEntryPointAddLeveledItem()
                                {
                                    Item = new FormLink<ILeveledItemGetter>(epfd.HasValue ? FormKeyBinaryTranslation.Instance.Parse(epfd.Value, stream.MetaData.MasterReferences!) : FormKey.Null)
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.AddActivateChoice:
                                if (!epft.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointAddActivateChoice)} did not have expected EPFT record");
                                if (!epf3.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointAddActivateChoice)} did not have expected EPF3 record");
                                if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.SpellWithStrings)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointAddActivateChoice)} did not have expected parameter type flag: {epft.Value[0]}");
                                }
                                entryPointEffect = new PerkEntryPointAddActivateChoice()
                                {
                                    Spell = new FormLinkNullable<ISpellGetter>(epfd.HasValue ? FormKeyBinaryTranslation.Instance.Parse(epfd.Value, stream.MetaData.MasterReferences!) : default(FormKey?)),
                                    ButtonLabel = epf2.HasValue ? StringBinaryTranslation.Instance.Parse(epf2.Value, StringsSource.Normal, stream.MetaData) : null,
                                    Flags = (PerkEntryPointAddActivateChoice.Flag)BinaryPrimitives.ReadInt16LittleEndian(epf3.Value),
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.SelectSpell:
                                if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointSelectSpell)} had EPF2 unexpectedly");
                                if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointSelectSpell)} had EPF3 unexpectedly");
                                if (!epft.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointSelectSpell)} did not have expected EPFT record");
                                if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.Spell)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointSelectSpell)} did not have expected parameter type flag: {epft.Value[0]}");
                                }
                                entryPointEffect = new PerkEntryPointSelectSpell()
                                {
                                    Spell = new FormLink<ISpellGetter>(epfd.HasValue ? FormKeyBinaryTranslation.Instance.Parse(epfd.Value, stream.MetaData.MasterReferences!) : FormKey.Null),
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.SelectText:
                                if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointSelectText)} had EPF2 unexpectedly");
                                if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointSelectText)} had EPF3 unexpectedly");
                                if (!epft.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointSelectText)} did not have expected EPFT record");
                                if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.String)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointSelectText)} did not have expected parameter type flag: {epft.Value[0]}");
                                }
                                entryPointEffect = new PerkEntryPointSelectText()
                                {
                                    Text = epfd.HasValue ? BinaryStringUtility.ProcessWholeToZString(epfd.Value, stream.MetaData.Encodings.NonTranslated) : string.Empty
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.SetText:
                                if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointSetText)} had EPF2 unexpectedly");
                                if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointSetText)} had EPF3 unexpectedly");
                                if (!epft.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointSetText)} did not have expected EPFT record");
                                if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.LString)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointSetText)} did not have expected parameter type flag: {epft.Value[0]}");
                                }
                                entryPointEffect = new PerkEntryPointSetText()
                                {
                                    Text = epfd.HasValue ? StringBinaryTranslation.Instance.Parse(epfd.Value, StringsSource.Normal, stream.MetaData) : (TranslatedString)string.Empty,
                                };
                                break;
                            case APerkEntryPointEffect.FunctionType.LegendaryMagicEffectEvent:
                                if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointLegendaryMagicEffectEvent)} had EPF2 unexpectedly");
                                if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointLegendaryMagicEffectEvent)} had EPF3 unexpectedly");
                                if (!epft.HasValue) throw new MalformedDataException($"{nameof(PerkEntryPointLegendaryMagicEffectEvent)} did not have expected EPFT record");
                                if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.ReplacementProjectile)
                                {
                                    throw new MalformedDataException($"{nameof(PerkEntryPointLegendaryMagicEffectEvent)} did not have expected parameter type flag: {epft.Value[0]}");
                                }

                                entryPointEffect = new PerkEntryPointLegendaryMagicEffectEvent()
                                {
                                    Projectile = new FormLink<IProjectileGetter>(epfd.HasValue ? FormKeyBinaryTranslation.Instance.Parse(epfd.Value, stream.MetaData.MasterReferences!) : FormKey.Null),
                                };
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        entryPointEffect.EntryPoint = entryPt;
                        entryPointEffect.PerkConditionTabCount = tabCount;
                        entryPointEffect.Conditions.SetTo(conditions);
                        if (epfb.HasValue)
                        {
                            entryPointEffect.PerkEntryID = epfb.Value.AsUInt16();
                        }
                        else if (epft.HasValue)
                        {
                            throw new MalformedDataException($"Did not have expected EPFB record to go with the EPFT record");
                        }
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
                        FormKeyBinaryTranslation.Instance.Write(writer, quest.Quest.FormKey);
                        writer.Write(quest.Stage);
                        break;
                    case PerkAbilityEffect ability:
                        FormKeyBinaryTranslation.Instance.Write(writer, ability.Ability.FormKey);
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
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                        {
                            FormKeyBinaryTranslation.Instance.Write(writer, lev.Item.FormKey);
                        }
                        break;
                    case PerkEntryPointAddActivateChoice activateChoice:
                        if (activateChoice.Spell.FormKeyNullable != null)
                        {
                            using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                            {
                                FormKeyBinaryTranslation.Instance.Write(writer, activateChoice.Spell.FormKeyNullable.Value);
                            }
                        }
                        break;
                    case PerkEntryPointSelectSpell spell:
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                        {
                            FormKeyBinaryTranslation.Instance.Write(writer, spell.Spell.FormKey);
                        }
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
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                        {
                            FormKeyBinaryTranslation.Instance.Write(writer, leg.Projectile.FormKey);
                        }
                        
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