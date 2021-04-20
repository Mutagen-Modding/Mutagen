using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Strings;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Perk
    {
        [Flags]
        public enum MajorFlag
        {
            NonPlayable = 0x4
        }

        public enum EffectType
        {
            Quest,
            Ability,
            EntryPoint,
        }
    }

    namespace Internals
    {
        public partial class PerkBinaryCreateTranslation
        {
            static void FillBinaryConditionsCustom(MutagenFrame frame, IPerkInternal item)
            {
                ConditionBinaryCreateTranslation.FillConditionsList(item.Conditions, frame);
            }

            public static IEnumerable<APerkEffect> ParseEffects(IMutagenReadStream stream)
            {
                while (stream.TryReadSubrecordFrame(RecordTypes.PRKE, out var prkeFrame))
                {
                    var type = (Perk.EffectType)prkeFrame.Content[0];
                    var rank = prkeFrame.Content[1];
                    var priority = prkeFrame.Content[2];
                    APerkEffect effect;
                    if (stream.TryReadSubrecordFrame(RecordTypes.DATA, out var dataFrame))
                    {
                        switch (type)
                        {
                            case Perk.EffectType.Quest:
                                effect = new PerkQuestEffect()
                                {
                                    Quest = new FormLink<IQuestGetter>(FormKeyBinaryTranslation.Instance.Parse(dataFrame.Content, stream.MetaData.MasterReferences!)),
                                    Stage = dataFrame.Content[4],
                                    Unknown = dataFrame.Content.Slice(5, 3).ToArray(),
                                };
                                effect.Conditions.SetTo(
                                    Mutagen.Bethesda.Binary.ListBinaryTranslation<PerkCondition>.Instance.Parse(
                                        frame: new MutagenFrame(stream),
                                        transl: (MutagenFrame r, out PerkCondition listSubItem) =>
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
                                    Mutagen.Bethesda.Binary.ListBinaryTranslation<PerkCondition>.Instance.Parse(
                                        frame: new MutagenFrame(stream),
                                        transl: (MutagenFrame r, out PerkCondition listSubItem) =>
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
                                var conditions = Mutagen.Bethesda.Binary.ListBinaryTranslation<PerkCondition>.Instance.Parse(
                                    frame: new MutagenFrame(stream),
                                    transl: (MutagenFrame r, out PerkCondition listSubItem) =>
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
                                while (stream.TryReadSubrecordFrame(out var subFrame))
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
                                        if (!epft.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointModifyValue)} did not have expected EPFT record");
                                        if (!epfd.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointModifyValue)} did not have expected EPFD record");
                                        if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.Float)
                                        {
                                            throw new ArgumentException($"{nameof(PerkEntryPointModifyValue)} did not have expected parameter type flag: {epft.Value[0]}");
                                        }
                                        entryPointEffect = new PerkEntryPointModifyValue()
                                        {
                                            Value = epfd.Value.Float(),
                                            Modification = func switch
                                            {
                                                APerkEntryPointEffect.FunctionType.SetValue => PerkEntryPointModifyValue.ModificationType.Set,
                                                APerkEntryPointEffect.FunctionType.MultiplyValue => PerkEntryPointModifyValue.ModificationType.Multiply,
                                                APerkEntryPointEffect.FunctionType.AddValue => PerkEntryPointModifyValue.ModificationType.Add,
                                                _ => throw new ArgumentException(),
                                            }
                                        };
                                        break;
                                    case APerkEntryPointEffect.FunctionType.AddRangeToValue:
                                        if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointModifyValue)} had EPF2 unexpectedly");
                                        if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointModifyValue)} had EPF3 unexpectedly");
                                        if (!epft.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointAddRangeToValue)} did not have expected EPFT record");
                                        if (!epfd.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointAddRangeToValue)} did not have expected EPFD record");
                                        if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.FloatFloat)
                                        {
                                            throw new ArgumentException($"{nameof(PerkEntryPointAddRangeToValue)} did not have expected parameter type flag: {epft.Value[0]}");
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
                                        if (!epft.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFT record");
                                        if (!epfd.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected EPFD record");
                                        if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.FloatFloat)
                                        {
                                            throw new ArgumentException($"{nameof(PerkEntryPointModifyActorValue)} did not have expected parameter type flag: {epft.Value[0]}");
                                        }
                                        entryPointEffect = new PerkEntryPointModifyActorValue()
                                        {
                                            ActorValue = (ActorValue)BinaryPrimitives.ReadInt32LittleEndian(epfd.Value),
                                            Value = epfd.Value.Slice(4).Float(),
                                            Modification = func switch
                                            {
                                                APerkEntryPointEffect.FunctionType.SetToActorValueMult => PerkEntryPointModifyActorValue.ModificationType.SetToAVMult,
                                                APerkEntryPointEffect.FunctionType.AddActorValueMult => PerkEntryPointModifyActorValue.ModificationType.AddAVMult,
                                                APerkEntryPointEffect.FunctionType.MultiplyActorValueMult => PerkEntryPointModifyActorValue.ModificationType.MultiplyAVMult,
                                                APerkEntryPointEffect.FunctionType.MultiplyOnePlusActorValueMult => PerkEntryPointModifyActorValue.ModificationType.MultiplyOnePlusAVMult,
                                                _ => throw new ArgumentException(),
                                            }
                                        };
                                        break;
                                    case APerkEntryPointEffect.FunctionType.AbsoluteValue:
                                    case APerkEntryPointEffect.FunctionType.NegativeAbsoluteValue:
                                        if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointModifyValue)} had EPF2 unexpectedly");
                                        if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointModifyValue)} had EPF3 unexpectedly");
                                        if (epft.HasValue && epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.None)
                                        {
                                            throw new ArgumentException($"{nameof(PerkEntryPointAbsoluteValue)} did not have expected parameter type flag: {epft.Value[0]}");
                                        }
                                        entryPointEffect = new PerkEntryPointAbsoluteValue()
                                        {
                                            Negative = func == APerkEntryPointEffect.FunctionType.NegativeAbsoluteValue
                                        };
                                        break;
                                    case APerkEntryPointEffect.FunctionType.AddLeveledList:
                                        if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointModifyValue)} had EPF2 unexpectedly");
                                        if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointModifyValue)} had EPF3 unexpectedly");
                                        if (!epft.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointAddLeveledItem)} did not have expected EPFT record");
                                        if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.LeveledItem)
                                        {
                                            throw new ArgumentException($"{nameof(PerkEntryPointAddLeveledItem)} did not have expected parameter type flag: {epft.Value[0]}");
                                        }
                                        entryPointEffect = new PerkEntryPointAddLeveledItem()
                                        {
                                            Item = new FormLink<ILeveledItemGetter>(epfd.HasValue ? FormKeyBinaryTranslation.Instance.Parse(epfd.Value, stream.MetaData.MasterReferences!) : FormKey.Null)
                                        };
                                        break;
                                    case APerkEntryPointEffect.FunctionType.AddActivateChoice:
                                        if (!epft.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointAddActivateChoice)} did not have expected EPFT record");
                                        if (!epf3.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointAddActivateChoice)} did not have expected EPF3 record");
                                        if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.SpellWithStrings)
                                        {
                                            throw new ArgumentException($"{nameof(PerkEntryPointAddActivateChoice)} did not have expected parameter type flag: {epft.Value[0]}");
                                        }
                                        entryPointEffect = new PerkEntryPointAddActivateChoice()
                                        {
                                            Spell = new FormLinkNullable<ISpellGetter>(epfd.HasValue ? FormKeyBinaryTranslation.Instance.Parse(epfd.Value, stream.MetaData.MasterReferences!) : default(FormKey?)),
                                            ButtonLabel = epf2.HasValue ? StringBinaryTranslation.Instance.Parse(epf2.Value, StringsSource.Normal, stream.MetaData.StringsLookup!) : null,
                                            Flags = new PerkScriptFlag()
                                            {
                                                Flags = (PerkScriptFlag.Flag)BinaryPrimitives.ReadInt16LittleEndian(epf3.Value),
                                                FragmentIndex = BinaryPrimitives.ReadUInt16LittleEndian(epf3.Value.Slice(2))
                                            },
                                        };
                                        break;
                                    case APerkEntryPointEffect.FunctionType.SelectSpell:
                                        if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointModifyValue)} had EPF2 unexpectedly");
                                        if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointModifyValue)} had EPF3 unexpectedly");
                                        if (!epft.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointSelectSpell)} did not have expected EPFT record");
                                        if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.Spell)
                                        {
                                            throw new ArgumentException($"{nameof(PerkEntryPointSelectSpell)} did not have expected parameter type flag: {epft.Value[0]}");
                                        }
                                        entryPointEffect = new PerkEntryPointSelectSpell()
                                        {
                                            Spell = new FormLink<ISpellGetter>(epfd.HasValue ? FormKeyBinaryTranslation.Instance.Parse(epfd.Value, stream.MetaData.MasterReferences!) : FormKey.Null),
                                        };
                                        break;
                                    case APerkEntryPointEffect.FunctionType.SelectText:
                                        if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointModifyValue)} had EPF2 unexpectedly");
                                        if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointModifyValue)} had EPF3 unexpectedly");
                                        if (!epft.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointSelectText)} did not have expected EPFT record");
                                        if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.String)
                                        {
                                            throw new ArgumentException($"{nameof(PerkEntryPointSelectText)} did not have expected parameter type flag: {epft.Value[0]}");
                                        }
                                        entryPointEffect = new PerkEntryPointSelectText()
                                        {
                                            Text = epfd.HasValue ? BinaryStringUtility.ProcessWholeToZString(epfd.Value) : string.Empty
                                        };
                                        break;
                                    case APerkEntryPointEffect.FunctionType.SetText:
                                        if (epf2.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF2, $"{nameof(PerkEntryPointModifyValue)} had EPF2 unexpectedly");
                                        if (epf3.HasValue) stream.MetaData.ReportIssue(RecordTypes.EPF3, $"{nameof(PerkEntryPointModifyValue)} had EPF3 unexpectedly");
                                        if (!epft.HasValue) throw new ArgumentException($"{nameof(PerkEntryPointSetText)} did not have expected EPFT record");
                                        if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.LString)
                                        {
                                            throw new ArgumentException($"{nameof(PerkEntryPointSetText)} did not have expected parameter type flag: {epft.Value[0]}");
                                        }
                                        entryPointEffect = new PerkEntryPointSetText()
                                        {
                                            Text = epfd.HasValue ? StringBinaryTranslation.Instance.Parse(epfd.Value, StringsSource.Normal, stream.MetaData.StringsLookup!) : (TranslatedString)string.Empty,
                                        };
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                                entryPointEffect.EntryPoint = entryPt;
                                entryPointEffect.PerkConditionTabCount = tabCount;
                                entryPointEffect.Conditions.SetTo(conditions);
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
                            _ => throw new ArgumentException($"Expected DATA subrecord that did not exist."),
                        };
                    }
                    effect.Rank = rank;
                    effect.Priority = priority;
                    stream.TryReadSubrecordFrame(RecordTypes.PRKF, out var _);
                    yield return effect;
                }
            }

            static partial void FillBinaryEffectsCustom(MutagenFrame frame, IPerkInternal item)
            {
                item.Effects.SetTo(ParseEffects(frame.Reader));
            }
        }

        public partial class PerkBinaryWriteTranslation
        {
            static void WriteBinaryConditionsCustom(MutagenWriter writer, IPerkGetter item)
            {
                ConditionBinaryWriteTranslation.WriteConditionsList(item.Conditions, writer);
            }

            static partial void WriteBinaryEffectsCustom(MutagenWriter writer, IPerkGetter item)
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
                                writer.Write(quest.Unknown);
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
                                    _ => throw new NotImplementedException()
                                }));
                                writer.Write(entryPt.PerkConditionTabCount);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    Mutagen.Bethesda.Binary.ListBinaryTranslation<IPerkConditionGetter>.Instance.Write(
                        writer,
                        effect.Conditions,
                        (w, i) => i.WriteToBinary(w));
                    if (effect is APerkEntryPointEffect)
                    {
                        var paramType = effect switch
                        {
                            PerkEntryPointModifyValue modVal => APerkEntryPointEffect.ParameterType.Float,
                            PerkEntryPointAddRangeToValue modVal => APerkEntryPointEffect.ParameterType.FloatFloat,
                            PerkEntryPointModifyActorValue modActorVal => APerkEntryPointEffect.ParameterType.FloatFloat,
                            PerkEntryPointAbsoluteValue absVal => APerkEntryPointEffect.ParameterType.None,
                            PerkEntryPointAddLeveledItem levItem => APerkEntryPointEffect.ParameterType.LeveledItem,
                            PerkEntryPointAddActivateChoice activateChoice => APerkEntryPointEffect.ParameterType.SpellWithStrings,
                            PerkEntryPointSelectSpell spell => APerkEntryPointEffect.ParameterType.Spell,
                            PerkEntryPointSelectText txt => APerkEntryPointEffect.ParameterType.String,
                            PerkEntryPointSetText ltxt => APerkEntryPointEffect.ParameterType.LString,
                            _ => throw new NotImplementedException()
                        };
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFT))
                        {
                            writer.Write((byte)paramType);
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
                            choice.Flags.WriteToBinary(writer);
                        }
                        switch (effect)
                        {
                            case PerkEntryPointModifyValue modVal:
                                using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                                {
                                    writer.Write(modVal.Value);
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
                                    writer.Write((int)actorVal.ActorValue);
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
                                    writer.Write(text.Text, StringBinaryType.NullTerminate);
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
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    using (HeaderExport.Subrecord(writer, RecordTypes.PRKF)) { }
                }
            }
        }

        public partial class PerkBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

            public IReadOnlyList<IAPerkEffectGetter> Effects { get; private set; } = ListExt.Empty<IAPerkEffectGetter>();

            partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
            }

            partial void EffectsCustomParse(
                OverlayStream stream,
                long finalPos,
                int offset,
                RecordType type,
                int? lastParsed)
            {
                Effects = PerkBinaryCreateTranslation.ParseEffects(stream)
                    .ToList();
            }
        }
    }
}
