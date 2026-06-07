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
    internal enum EntryPointFunction : byte
    {
        SetValue = 1,
        AddValue = 2,
        MultiplyValue = 3,
        AddRangeToValue = 4,
        AddActorValueMult = 5,
        AddLeveledList = 8,
        AddActivateChoice = 9,
    }

    public record Payload
    {
        public SubrecordFrame? DATA { get; set; }
        public SubrecordFrame? EPF2 { get; set; }
        public SubrecordFrame? EPF3 { get; set; }
        public SubrecordFrame? EPFD { get; set; }
        public SubrecordFrame? EPFT { get; set; }
        public List<PerkCondition>? Conditions { get; set; }
        public ScriptFields? Script { get; set; }
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
                case RecordTypeInts.SCHR:
                    stream.Position -= subFrame.TotalLength;
                    ret.Script = ScriptFields.CreateFromBinary(new MutagenFrame(stream));
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
        switch (type)
        {
            case Perk.EffectType.Quest:
                if (payload.DATA is { } questData)
                {
                    effect = new PerkQuestEffect()
                    {
                        Quest = FormLinkBinaryTranslation.Instance.Factory<IQuestGetter>(stream.MetaData, questData.Content),
                        Stage = questData.Content[4],
                        Unused = questData.Content.Slice(5, 3).ToArray(),
                    };
                }
                else
                {
                    effect = new PerkQuestEffect();
                }
                break;
            case Perk.EffectType.Ability:
                effect = new PerkAbilityEffect()
                {
                    Ability = FormLinkBinaryTranslation.Instance.FactoryNullable<ISpellGetter>(stream.MetaData, payload.DATA?.Content),
                };
                break;
            case Perk.EffectType.EntryPoint:
                if (payload.DATA is not { } entryData)
                {
                    throw new MalformedDataException("Entry point perk effect lacked a DATA subrecord.");
                }
                var entryPt = (APerkEntryPointEffect.EntryType)entryData.Content[0];
                var func = (EntryPointFunction)entryData.Content[1];
                var tabCount = entryData.Content[2];
                APerkEntryPointEffect entryPointEffect;
                switch (func)
                {
                    case EntryPointFunction.SetValue:
                    case EntryPointFunction.AddValue:
                    case EntryPointFunction.MultiplyValue:
                        entryPointEffect = new PerkEntryPointModifyValue()
                        {
                            Value = payload.EPFD?.Content.Float() ?? 0f,
                            Modification = func switch
                            {
                                EntryPointFunction.SetValue => PerkEntryPointModifyValue.ModificationType.Set,
                                EntryPointFunction.AddValue => PerkEntryPointModifyValue.ModificationType.Add,
                                _ => PerkEntryPointModifyValue.ModificationType.Multiply,
                            }
                        };
                        break;
                    case EntryPointFunction.AddRangeToValue:
                        entryPointEffect = new PerkEntryPointAddRangeToValue()
                        {
                            From = payload.EPFD.HasValue ? payload.EPFD.Value.Content.Float() : 0f,
                            To = payload.EPFD.HasValue ? payload.EPFD.Value.Content.Slice(4).Float() : 0f,
                        };
                        break;
                    case EntryPointFunction.AddActorValueMult:
                        entryPointEffect = new PerkEntryPointModifyActorValue()
                        {
                            ActorValue = payload.EPFD.HasValue ? (ActorValue)BinaryPrimitives.ReadUInt32LittleEndian(payload.EPFD.Value.Content) : default,
                            Value = payload.EPFD.HasValue ? payload.EPFD.Value.Content.Slice(4).Float() : 0f,
                        };
                        break;
                    case EntryPointFunction.AddLeveledList:
                        entryPointEffect = new PerkEntryPointAddLeveledItem()
                        {
                            Item = FormLinkBinaryTranslation.Instance.Factory<ILeveledItemGetter>(stream.MetaData, payload.EPFD?.Content)
                        };
                        break;
                    case EntryPointFunction.AddActivateChoice:
                        var choice = new PerkEntryPointAddActivateChoice();
                        if (payload.EPF2 != null)
                        {
                            choice.ButtonLabel = BinaryStringUtility.ProcessWholeToZString(payload.EPF2.Value.Content, stream.MetaData.Encodings.NonTranslated);
                        }
                        if (payload.EPF3 != null)
                        {
                            choice.RunImmediately = BinaryPrimitives.ReadUInt16LittleEndian(payload.EPF3.Value.Content) != 0;
                        }
                        if (payload.Script != null)
                        {
                            choice.Script.DeepCopyIn(payload.Script);
                        }
                        entryPointEffect = choice;
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

        effect.Rank = rank;
        effect.Priority = priority;
        if (payload.Conditions != null)
        {
            effect.Conditions.SetTo(payload.Conditions);
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
                    IPerkQuestEffectGetter => Perk.EffectType.Quest,
                    IPerkAbilityEffectGetter => Perk.EffectType.Ability,
                    IAPerkEntryPointEffectGetter => Perk.EffectType.EntryPoint,
                    _ => throw new NotImplementedException()
                }));
                writer.Write(effect.Rank);
                writer.Write(effect.Priority);
            }
            switch (effect)
            {
                case IPerkQuestEffectGetter quest:
                    using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
                    {
                        FormKeyBinaryTranslation.Instance.Write(writer, quest.Quest);
                        writer.Write(quest.Stage);
                        writer.Write(quest.Unused);
                    }
                    break;
                case IPerkAbilityEffectGetter ability:
                    if (ability.Ability.FormKeyNullable.HasValue)
                    {
                        using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
                        {
                            FormKeyBinaryTranslation.Instance.Write(writer, ability.Ability);
                        }
                    }
                    break;
                case IAPerkEntryPointEffectGetter entryPt:
                    using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
                    {
                        writer.Write((byte)entryPt.EntryPoint);
                        writer.Write((byte)(entryPt switch
                        {
                            IPerkEntryPointModifyValueGetter modVal => modVal.Modification switch
                            {
                                PerkEntryPointModifyValue.ModificationType.Set => PerkBinaryCreateTranslation.EntryPointFunction.SetValue,
                                PerkEntryPointModifyValue.ModificationType.Add => PerkBinaryCreateTranslation.EntryPointFunction.AddValue,
                                PerkEntryPointModifyValue.ModificationType.Multiply => PerkBinaryCreateTranslation.EntryPointFunction.MultiplyValue,
                                _ => throw new NotImplementedException()
                            },
                            IPerkEntryPointAddRangeToValueGetter => PerkBinaryCreateTranslation.EntryPointFunction.AddRangeToValue,
                            IPerkEntryPointModifyActorValueGetter => PerkBinaryCreateTranslation.EntryPointFunction.AddActorValueMult,
                            IPerkEntryPointAddLeveledItemGetter => PerkBinaryCreateTranslation.EntryPointFunction.AddLeveledList,
                            IPerkEntryPointAddActivateChoiceGetter => PerkBinaryCreateTranslation.EntryPointFunction.AddActivateChoice,
                            _ => throw new NotImplementedException()
                        }));
                        writer.Write(entryPt.PerkConditionTabCount);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            ListBinaryTranslation<IPerkConditionGetter>.Instance.Write(
                writer,
                effect.Conditions,
                (w, i) => i.WriteToBinary(w));
            if (effect is IAPerkEntryPointEffectGetter entryPointEffect)
            {
                using (HeaderExport.Subrecord(writer, RecordTypes.EPFT))
                {
                    writer.Write((byte)(entryPointEffect switch
                    {
                        IPerkEntryPointModifyValueGetter => 1, // Float
                        IPerkEntryPointAddRangeToValueGetter => 2, // Float, Float
                        IPerkEntryPointModifyActorValueGetter => 2, // Actor Value, Float
                        IPerkEntryPointAddLeveledItemGetter => 3, // Leveled Item
                        IPerkEntryPointAddActivateChoiceGetter => 4, // None (Script)
                        _ => throw new NotImplementedException()
                    }));
                }
                switch (entryPointEffect)
                {
                    case IPerkEntryPointModifyValueGetter modVal:
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                        {
                            writer.Write(modVal.Value);
                        }
                        break;
                    case IPerkEntryPointAddRangeToValueGetter range:
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                        {
                            writer.Write(range.From);
                            writer.Write(range.To);
                        }
                        break;
                    case IPerkEntryPointModifyActorValueGetter actorVal:
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                        {
                            writer.Write((uint)actorVal.ActorValue);
                            writer.Write(actorVal.Value);
                        }
                        break;
                    case IPerkEntryPointAddLeveledItemGetter lev:
                        FormKeyBinaryTranslation.Instance.Write(writer, lev.Item, RecordTypes.EPFD);
                        break;
                    case IPerkEntryPointAddActivateChoiceGetter choice:
                        if (choice.ButtonLabel != null)
                        {
                            using (HeaderExport.Subrecord(writer, RecordTypes.EPF2))
                            {
                                writer.Write(choice.ButtonLabel, StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);
                            }
                        }
                        if (choice.RunImmediately is { } runImmediately)
                        {
                            using (HeaderExport.Subrecord(writer, RecordTypes.EPF3))
                            {
                                writer.Write((ushort)(runImmediately ? 1 : 0));
                            }
                        }
                        choice.Script.WriteToBinary(writer);
                        break;
                    default:
                        throw new NotImplementedException();
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
