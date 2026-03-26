using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Noggog;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Fallout3;

public partial class Perk
{
    public enum EffectType
    {
        Quest,
        Ability,
        EntryPoint,
    }
}

partial class PerkBinaryCreateTranslation
{
    public record Payload
    {
        public SubrecordFrame? DATA { get; set; }
        public SubrecordFrame? EPF2 { get; set; }
        public SubrecordFrame? EPF3 { get; set; }
        public SubrecordFrame? EPFD { get; set; }
        public SubrecordFrame? EPFT { get; set; }
        public List<PerkCondition>? Conditions { get; set; }
        // Embedded script subrecords (inside Entry Point Function Parameters per xEdit)
        public List<(RecordType Type, ReadOnlyMemorySlice<byte> Data)>? EmbeddedScriptSubrecords { get; set; }
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
                // Embedded script subrecords (part of Entry Point Function Parameters per xEdit)
                case RecordTypeInts.SCHR:
                case RecordTypeInts.SCDA:
                case RecordTypeInts.SCTX:
                case RecordTypeInts.SLSD:
                case RecordTypeInts.SCVR:
                case RecordTypeInts.SCRO:
                case RecordTypeInts.SCRV:
                    ret.EmbeddedScriptSubrecords ??= new();
                    ret.EmbeddedScriptSubrecords.Add((subFrame.RecordType, subFrame.Content.ToArray()));
                    break;
                default:
                    stream.Position -= subFrame.TotalLength;
                    return ret;
            }
        }

        return ret;
    }

    public static APerkEffect ParseEffect<TStream>(TStream stream, SubrecordFrame prkeFrame)
        where TStream : IMutagenReadStream
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
                        Stage = payload.DATA.Value.Content[4],
                        Unknown = payload.DATA.Value.Content.Slice(5, 3).ToArray(),
                    };
                    break;
                case Perk.EffectType.Ability:
                    effect = new PerkAbilityEffect()
                    {
                        Ability = FormLinkBinaryTranslation.Instance.Factory<ISpellGetter>(stream.MetaData, payload.DATA.Value.Content),
                    };
                    break;
                case Perk.EffectType.EntryPoint:
                    var entryPt = (APerkEntryPointEffect.EntryType)payload.DATA.Value.Content[0];
                    var func = payload.DATA.Value.Content[1];
                    var tabCount = payload.DATA.Value.Content[2];
                    APerkEntryPointEffect entryPointEffect;
                    // FNV uses a simpler function type model
                    // func values: 1=SetValue, 2=AddValue, 3=MultiplyValue
                    switch (func)
                    {
                        case 1: // SetValue
                        case 2: // AddValue
                        case 3: // MultiplyValue
                            if (payload.EPFT == null && payload.EPFD == null)
                            {
                                entryPointEffect = new PerkEntryPointModifyValue()
                                {
                                    Value = null,
                                    Modification = func switch
                                    {
                                        1 => PerkEntryPointModifyValue.ModificationType.Set,
                                        3 => PerkEntryPointModifyValue.ModificationType.Multiply,
                                        2 => PerkEntryPointModifyValue.ModificationType.Add,
                                        _ => throw new MalformedDataException(),
                                    }
                                };
                            }
                            else if (payload.EPFT.HasValue && payload.EPFD.HasValue
                                     && payload.EPFT.Value.Content[0] == 2 && payload.EPFD.Value.Content.Length >= 8)
                            {
                                entryPointEffect = new PerkEntryPointModifyValues()
                                {
                                    Value = payload.EPFD.Value.Content.Float(),
                                    Value2 = payload.EPFD.Value.Content.Slice(4).Float(),
                                    Modification = func switch
                                    {
                                        1 => PerkEntryPointModifyValue.ModificationType.Set,
                                        3 => PerkEntryPointModifyValue.ModificationType.Multiply,
                                        2 => PerkEntryPointModifyValue.ModificationType.Add,
                                        _ => throw new MalformedDataException(),
                                    }
                                };
                            }
                            else
                            {
                                entryPointEffect = new PerkEntryPointModifyValue()
                                {
                                    Value = payload.EPFD.HasValue ? payload.EPFD.Value.Content.Float() : null,
                                    Modification = func switch
                                    {
                                        1 => PerkEntryPointModifyValue.ModificationType.Set,
                                        3 => PerkEntryPointModifyValue.ModificationType.Multiply,
                                        2 => PerkEntryPointModifyValue.ModificationType.Add,
                                        _ => throw new MalformedDataException(),
                                    }
                                };
                            }
                            break;
                        case 4: // AddRangeToValue
                            entryPointEffect = new PerkEntryPointAddRangeToValue()
                            {
                                From = payload.EPFD.HasValue ? payload.EPFD.Value.Content.Float() : 0f,
                                To = payload.EPFD.HasValue ? payload.EPFD.Value.Content.Slice(4).Float() : 0f,
                            };
                            break;
                        case 5: // AddActorValueMult
                            entryPointEffect = new PerkEntryPointModifyActorValue()
                            {
                                ActorValue = payload.EPFD.HasValue ? (ActorValue)BinaryPrimitives.ReadSingleLittleEndian(payload.EPFD.Value.Content) : default,
                                Value = payload.EPFD.HasValue ? payload.EPFD.Value.Content.Slice(4).Float() : 0f,
                                Modification = PerkEntryPointModifyActorValue.ModificationType.AddAVMult,
                            };
                            break;
                        case 6: // AbsoluteValue
                            entryPointEffect = new PerkEntryPointAbsoluteValue()
                            {
                                Negative = false
                            };
                            break;
                        case 7: // NegativeAbsoluteValue
                            entryPointEffect = new PerkEntryPointAbsoluteValue()
                            {
                                Negative = true
                            };
                            break;
                        case 8: // AddLeveledList
                            entryPointEffect = new PerkEntryPointAddLeveledItem()
                            {
                                Item = FormLinkBinaryTranslation.Instance.Factory<ILeveledItemGetter>(stream.MetaData, payload.EPFD?.Content)
                            };
                            break;
                        case 9: // AddActivateChoice
                            entryPointEffect = new PerkEntryPointAddActivateChoice()
                            {
                                Spell = FormLinkBinaryTranslation.Instance.FactoryNullable<ISpellGetter>(stream.MetaData, payload.EPFD?.Content),
                            };
                            break;
                        case 10: // SelectSpell
                            entryPointEffect = new PerkEntryPointSelectSpell()
                            {
                                Spell = FormLinkBinaryTranslation.Instance.Factory<ISpellGetter>(stream.MetaData, payload.EPFD?.Content),
                            };
                            break;
                        case 11: // SelectText
                            entryPointEffect = new PerkEntryPointSelectText()
                            {
                                Text = payload.EPFD.HasValue ? BinaryStringUtility.ProcessWholeToZString(payload.EPFD.Value.Content, stream.MetaData.Encodings.NonTranslated) : string.Empty
                            };
                            break;
                        case 12: // SetText
                            entryPointEffect = new PerkEntryPointSetText()
                            {
                                Text = payload.EPFD.HasValue ? BinaryStringUtility.ProcessWholeToZString(payload.EPFD.Value.Content, stream.MetaData.Encodings.NonTranslated) : string.Empty
                            };
                            break;
                        default:
                            throw new NotImplementedException($"Unknown perk entry point function type: {func}");
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
            effect.ButtonLabel = BinaryStringUtility.ProcessWholeToZString(payload.EPF2.Value.Content, stream.MetaData.Encodings.NonTranslated);
        }
        if (payload.EPF3 != null && payload.EPF3.Value.Content.Length >= 4)
        {
            effect.Flags = new PerkScriptFlag()
            {
                Flags = (PerkScriptFlag.Flag)BinaryPrimitives.ReadInt16LittleEndian(payload.EPF3.Value.Content),
                FragmentIndex = BinaryPrimitives.ReadUInt16LittleEndian(payload.EPF3.Value.Content.Slice(2))
            };
        }

        if (payload.EmbeddedScriptSubrecords != null)
        {
            effect.EmbeddedScriptSubrecords = payload.EmbeddedScriptSubrecords
                .Select(s => (s.Type, s.Data.ToArray()))
                .ToList();
        }

        if (stream.TryReadSubrecord(RecordTypes.EPFT, out var epftFrame)
            && epftFrame.ContentLength != 1
            && epftFrame.Content[0] != 0)
        {
            throw new MalformedDataException($"Encountered an unexpected epft frame.");
        }
        stream.TryReadSubrecord(RecordTypes.PRKF, out var _);
        return effect;
    }

    public static List<APerkEffect> ParseEffects(IMutagenReadStream stream)
    {
        var effects = new List<APerkEffect>();
        while (stream.TryReadSubrecord(RecordTypes.PRKE, out var prkeFrame))
        {
            effects.Add(ParseEffect(stream, prkeFrame));
        }
        // Consume trailing record-level PRKF if present in source data
        // (xEdit FO3 doesn't define one, but some ESMs have it)
        stream.TryReadSubrecord(RecordTypes.PRKF, out var _);
        return effects;
    }

    public static partial void FillBinaryEffectsCustom(MutagenFrame frame, IPerkInternal item, PreviousParse lastParsed)
    {
        item.Effects.SetTo(ParseEffects(frame.Reader));
    }
}

partial class PerkBinaryWriteTranslation
{
    public static partial void WriteBinaryEffectsCustom(MutagenWriter writer, IPerkGetter item)
    {
        foreach (var effect in item.Effects)
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.PRKE))
            {
                writer.Write((byte)(effect switch
                {
                    PerkQuestEffect => Perk.EffectType.Quest,
                    PerkAbilityEffect => Perk.EffectType.Ability,
                    APerkEntryPointEffect => Perk.EffectType.EntryPoint,
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
                        writer.Write(quest.Unknown);
                        break;
                    case PerkAbilityEffect ability:
                        FormKeyBinaryTranslation.Instance.Write(writer, ability.Ability);
                        break;
                    case APerkEntryPointEffect entryPt:
                        writer.Write((byte)entryPt.EntryPoint);
                        byte funcByte = entryPt switch
                        {
                            PerkEntryPointModifyValue modVal => modVal.Modification switch
                            {
                                PerkEntryPointModifyValue.ModificationType.Add => 2,
                                PerkEntryPointModifyValue.ModificationType.Set => 1,
                                PerkEntryPointModifyValue.ModificationType.Multiply => 3,
                                _ => throw new NotImplementedException()
                            },
                            PerkEntryPointModifyValues modVals => modVals.Modification switch
                            {
                                PerkEntryPointModifyValue.ModificationType.Add => 2,
                                PerkEntryPointModifyValue.ModificationType.Set => 1,
                                PerkEntryPointModifyValue.ModificationType.Multiply => 3,
                                _ => throw new NotImplementedException()
                            },
                            PerkEntryPointAddRangeToValue => 4,
                            PerkEntryPointModifyActorValue => 5,
                            PerkEntryPointAbsoluteValue absVal => (byte)(absVal.Negative ? 7 : 6),
                            PerkEntryPointAddLeveledItem => 8,
                            PerkEntryPointAddActivateChoice => 9,
                            PerkEntryPointSelectSpell => 10,
                            PerkEntryPointSelectText => 11,
                            PerkEntryPointSetText => 12,
                            _ => throw new NotImplementedException()
                        };
                        writer.Write(funcByte);
                        writer.Write(entryPt.PerkConditionTabCount);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            ListBinaryTranslation<IPerkConditionGetter>.Instance.Write(
                writer,
                effect.Conditions,
                (w, i) => i.WriteToBinary(w));
            if (effect is APerkEntryPointEffect)
            {
                byte paramType = effect switch
                {
                    PerkEntryPointModifyValue => 1,
                    PerkEntryPointModifyValues => 2,
                    PerkEntryPointAddRangeToValue => 2,
                    PerkEntryPointModifyActorValue => 2,
                    PerkEntryPointAbsoluteValue => 0,
                    PerkEntryPointAddLeveledItem => 3,
                    PerkEntryPointAddActivateChoice => 4,
                    PerkEntryPointSelectSpell => 5,
                    PerkEntryPointSelectText => 6,
                    PerkEntryPointSetText => 6,
                    _ => throw new NotImplementedException()
                };
                if (effect is not PerkEntryPointModifyValue modValEpft
                    || modValEpft.Value.HasValue)
                {
                    using (HeaderExport.Subrecord(writer, RecordTypes.EPFT))
                    {
                        writer.Write(paramType);
                    }
                }

                if (effect is PerkEntryPointAddActivateChoice choice)
                {
                    if (choice.ButtonLabel != null)
                    {
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPF2))
                        {
                            writer.Write(choice.ButtonLabel, StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);
                        }
                    }
                    choice.Flags.WriteToBinary(writer);
                }
                switch (effect)
                {
                    case PerkEntryPointModifyValue modVal:
                        if (modVal.Value is {} f)
                        {
                            using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                            {
                                writer.Write(f);
                            }
                        }
                        break;
                    case PerkEntryPointModifyValues modVal:
                        if (modVal.Value is not null || modVal.Value2 is not null)
                        {
                            using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                            {
                                writer.Write(modVal.Value ?? 0f);
                                writer.Write(modVal.Value2 ?? 0f);
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
                            writer.Write((float)actorVal.ActorValue);
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
                            writer.Write(ltext.Text, StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);
                        }
                        break;
                    case PerkEntryPointAbsoluteValue:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            // Write embedded script subrecords inside the effect, before PRKF (per xEdit FO3 definition)
            if (effect is APerkEffect concreteEffect && concreteEffect.EmbeddedScriptSubrecords != null)
            {
                foreach (var (recType, data) in concreteEffect.EmbeddedScriptSubrecords)
                {
                    using (HeaderExport.Subrecord(writer, recType))
                    {
                        writer.Write(data);
                    }
                }
            }
            using (HeaderExport.Subrecord(writer, RecordTypes.PRKF)) { }
        }
    }
}

partial class PerkBinaryOverlay
{
    public IReadOnlyList<IAPerkEffectGetter> Effects { get; private set; } = [];

    private static RecordTriggerSpecs _effectSpecs = new(
        new RecordCollection()
        {
            RecordTypes.PRKE,
            RecordTypes.DATA,
            RecordTypes.EPFT,
            RecordTypes.EPFD,
            RecordTypes.EPF2,
            RecordTypes.EPF3,
            RecordTypes.PRKC,
            RecordTypes.CTDA,
            RecordTypes.SCHR,
            RecordTypes.SCDA,
            RecordTypes.SCTX,
            RecordTypes.SLSD,
            RecordTypes.SCVR,
            RecordTypes.SCRO,
            RecordTypes.SCRV,
            RecordTypes.PRKF
        },
        triggeringRecordTypes: new RecordCollection()
        {
            RecordTypes.PRKE
        });

    partial void EffectsCustomParse(
        OverlayStream stream,
        int finalPos,
        int offset,
        RecordType type,
        PreviousParse lastParsed)
    {
        Effects = BinaryOverlayList.FactoryByArray(
            stream.RemainingMemory,
            _package,
            getter: (s, p) =>
            {
                var stream = new OverlayStream(s, p.MetaData);
                var prke = stream.ReadSubrecord(RecordTypes.PRKE);
                return PerkBinaryCreateTranslation.ParseEffect(stream, prke);
            },
            locs: ParseRecordLocations(
                stream: stream,
                constants: _package.MetaData.Constants.SubConstants,
                trigger: _effectSpecs,
                skipHeader: false));
    }
}
