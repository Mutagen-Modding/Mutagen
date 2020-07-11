using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Perk
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string? INamedGetter.Name => this.Name?.String;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        TranslatedString ITranslatedNamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string INamedRequired.Name
        {
            get => this.Name?.String ?? string.Empty;
            set => this.Name = new TranslatedString(value);
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        TranslatedString ITranslatedNamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string? INamed.Name
        {
            get => this.Name?.String;
            set => this.Name = value == null ? null : new TranslatedString(value);
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IIconsGetter? IHasIconsGetter.Icons => this.Icons;
        #endregion

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
            static partial void FillBinaryConditionsCustom(MutagenFrame frame, IPerkInternal item)
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
                    var dataFrame = stream.ReadSubrecordFrame(RecordTypes.DATA);
                    APerkEffect effect;
                    switch (type)
                    {
                        case Perk.EffectType.Quest:
                            effect = new PerkQuestEffect()
                            {
                                Quest = FormKeyBinaryTranslation.Instance.Parse(dataFrame.Content, stream.MetaData.MasterReferences!),
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
                                Ability = FormKeyBinaryTranslation.Instance.Parse(dataFrame.Content, stream.MetaData.MasterReferences!),
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
                            while (stream.TryReadSubrecordMemoryFrame(out var subFrame, readSafe: true))
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
                                    if (!epfd.HasValue
                                        || epf2.HasValue
                                        || epf3.HasValue
                                        || !epft.HasValue)
                                    {
                                        throw new ArgumentException($"{nameof(PerkModifyValue)} did not have expected records");
                                    }
                                    if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.Float)
                                    {
                                        throw new ArgumentException($"{nameof(PerkModifyValue)} did not have expected parameter type flag: {epft.Value[0]}");
                                    }
                                    entryPointEffect = new PerkModifyValue()
                                    {
                                        Value = SpanExt.GetFloat(epfd.Value),
                                        Modification = func switch
                                        {
                                            APerkEntryPointEffect.FunctionType.SetValue => PerkModifyValue.ModificationType.Set,
                                            APerkEntryPointEffect.FunctionType.MultiplyValue => PerkModifyValue.ModificationType.Multiply,
                                            APerkEntryPointEffect.FunctionType.AddValue => PerkModifyValue.ModificationType.Add,
                                            _ => throw new ArgumentException(),
                                        }
                                    };
                                    break;
                                case APerkEntryPointEffect.FunctionType.AddRangeToValue:
                                    if (!epfd.HasValue
                                        || epf2.HasValue
                                        || epf3.HasValue
                                        || !epft.HasValue)
                                    {
                                        throw new ArgumentException($"{nameof(PerkAddRangeToValue)} did not have expected records");
                                    }
                                    if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.FloatFloat)
                                    {
                                        throw new ArgumentException($"{nameof(PerkAddRangeToValue)} did not have expected parameter type flag: {epft.Value[0]}");
                                    }
                                    entryPointEffect = new PerkAddRangeToValue()
                                    {
                                        From = SpanExt.GetFloat(epfd.Value),
                                        To = SpanExt.GetFloat(epfd.Value.Slice(4)),
                                    };
                                    break;
                                case APerkEntryPointEffect.FunctionType.SetToActorValueMult:
                                case APerkEntryPointEffect.FunctionType.MultiplyActorValueMult:
                                case APerkEntryPointEffect.FunctionType.MultiplyOnePlusActorValueMult:
                                case APerkEntryPointEffect.FunctionType.AddActorValueMult:
                                    if (!epfd.HasValue
                                        || epf2.HasValue
                                        || epf3.HasValue
                                        || !epft.HasValue)
                                    {
                                        throw new ArgumentException($"{nameof(PerkModifyActorValue)} did not have expected records");
                                    }
                                    if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.FloatFloat)
                                    {
                                        throw new ArgumentException($"{nameof(PerkModifyActorValue)} did not have expected parameter type flag: {epft.Value[0]}");
                                    }
                                    entryPointEffect = new PerkModifyActorValue()
                                    {
                                        ActorValue = (ActorValueExtended)BinaryPrimitives.ReadInt32LittleEndian(epfd.Value),
                                        Value = SpanExt.GetFloat(epfd.Value.Slice(4)),
                                        Modification = func switch
                                        {
                                            APerkEntryPointEffect.FunctionType.SetToActorValueMult => PerkModifyActorValue.ModificationType.SetToAVMult,
                                            APerkEntryPointEffect.FunctionType.AddActorValueMult => PerkModifyActorValue.ModificationType.AddAVMult,
                                            APerkEntryPointEffect.FunctionType.MultiplyActorValueMult => PerkModifyActorValue.ModificationType.MultiplyAVMult,
                                            APerkEntryPointEffect.FunctionType.MultiplyOnePlusActorValueMult => PerkModifyActorValue.ModificationType.MultiplyOnePlusAVMult,
                                            _ => throw new ArgumentException(),
                                        }
                                    };
                                    break;
                                case APerkEntryPointEffect.FunctionType.AbsoluteValue:
                                case APerkEntryPointEffect.FunctionType.NegativeAbsoluteValue:
                                    if (!epfd.HasValue
                                        || epf2.HasValue
                                        || epf3.HasValue
                                        || !epft.HasValue)
                                    {
                                        throw new ArgumentException($"{nameof(PerkAbsoluteValue)} did not have expected records");
                                    }
                                    if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.None)
                                    {
                                        throw new ArgumentException($"{nameof(PerkAbsoluteValue)} did not have expected parameter type flag: {epft.Value[0]}");
                                    }
                                    entryPointEffect = new PerkAbsoluteValue()
                                    {
                                        Negative = func == APerkEntryPointEffect.FunctionType.NegativeAbsoluteValue
                                    };
                                    break;
                                case APerkEntryPointEffect.FunctionType.AddLeveledList:
                                    if (!epfd.HasValue
                                        || epf2.HasValue
                                        || epf3.HasValue
                                        || !epft.HasValue)
                                    {
                                        throw new ArgumentException($"{nameof(PerkAddLeveledItem)} did not have expected records");
                                    }
                                    if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.LeveledItem)
                                    {
                                        throw new ArgumentException($"{nameof(PerkAddLeveledItem)} did not have expected parameter type flag: {epft.Value[0]}");
                                    }
                                    entryPointEffect = new PerkAddLeveledItem()
                                    {
                                        Item = FormKeyBinaryTranslation.Instance.Parse(epfd.Value, stream.MetaData.MasterReferences!)
                                    };
                                    break;
                                case APerkEntryPointEffect.FunctionType.AddActivateChoice:
                                    if (!epf3.HasValue
                                        || !epft.HasValue)
                                    {
                                        throw new ArgumentException($"{nameof(PerkAddActivateChoice)} did not have expected records");
                                    }
                                    if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.SpellWithStrings)
                                    {
                                        throw new ArgumentException($"{nameof(PerkAddActivateChoice)} did not have expected parameter type flag: {epft.Value[0]}");
                                    }
                                    entryPointEffect = new PerkAddActivateChoice()
                                    {
                                        Spell = epfd.HasValue ? FormKeyBinaryTranslation.Instance.Parse(epfd.Value, stream.MetaData.MasterReferences!) : default(FormKey?),
                                        ButtonLabel = epf2.HasValue ? StringBinaryTranslation.Instance.Parse(epf2.Value, StringsSource.Normal, stream.MetaData.StringsLookup!) : null,
                                        Flags = new PerkScriptFlag()
                                        {
                                            Flags = (PerkScriptFlag.Flag)BinaryPrimitives.ReadInt16LittleEndian(epf3.Value),
                                            FragmentIndex = BinaryPrimitives.ReadUInt16LittleEndian(epf3.Value.Slice(2))
                                        },
                                    };
                                    break;
                                case APerkEntryPointEffect.FunctionType.SelectSpell:
                                    if (!epfd.HasValue
                                        || epf2.HasValue
                                        || epf3.HasValue
                                        || !epft.HasValue)
                                    {
                                        throw new ArgumentException($"{nameof(PerkSelectSpell)} did not have expected records");
                                    }
                                    if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.Spell)
                                    {
                                        throw new ArgumentException($"{nameof(PerkSelectSpell)} did not have expected parameter type flag: {epft.Value[0]}");
                                    }
                                    entryPointEffect = new PerkSelectSpell()
                                    {
                                        Spell = FormKeyBinaryTranslation.Instance.Parse(epfd.Value, stream.MetaData.MasterReferences!),
                                    };
                                    break;
                                case APerkEntryPointEffect.FunctionType.SelectText:
                                    if (!epfd.HasValue
                                        || epf2.HasValue
                                        || epf3.HasValue
                                        || !epft.HasValue)
                                    {
                                        throw new ArgumentException($"{nameof(PerkSelectText)} did not have expected records");
                                    }
                                    if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.String)
                                    {
                                        throw new ArgumentException($"{nameof(PerkSelectText)} did not have expected parameter type flag: {epft.Value[0]}");
                                    }
                                    entryPointEffect = new PerkSelectText()
                                    {
                                        Text = BinaryStringUtility.ProcessWholeToZString(epfd.Value)
                                    };
                                    break;
                                case APerkEntryPointEffect.FunctionType.SetText:
                                    if (!epfd.HasValue
                                        || epf2.HasValue
                                        || epf3.HasValue
                                        || !epft.HasValue)
                                    {
                                        throw new ArgumentException($"{nameof(PerkSetText)} did not have expected records");
                                    }
                                    if (epft.Value[0] != (byte)APerkEntryPointEffect.ParameterType.LString)
                                    {
                                        throw new ArgumentException($"{nameof(PerkSetText)} did not have expected parameter type flag: {epft.Value[0]}");
                                    }
                                    entryPointEffect = new PerkSetText()
                                    {
                                        Text = StringBinaryTranslation.Instance.Parse(epfd.Value, StringsSource.Normal, stream.MetaData.StringsLookup!),
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
                    };
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
            static partial void WriteBinaryConditionsCustom(MutagenWriter writer, IPerkGetter item)
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
                                    PerkModifyValue modVal => modVal.Modification switch
                                    {
                                        PerkModifyValue.ModificationType.Add => APerkEntryPointEffect.FunctionType.AddValue,
                                        PerkModifyValue.ModificationType.Set => APerkEntryPointEffect.FunctionType.SetValue,
                                        PerkModifyValue.ModificationType.Multiply => APerkEntryPointEffect.FunctionType.MultiplyValue,
                                        _ => throw new NotImplementedException()
                                    },
                                    PerkAddRangeToValue addRange => APerkEntryPointEffect.FunctionType.AddRangeToValue,
                                    PerkModifyActorValue modActorVal => modActorVal.Modification switch
                                    {
                                        PerkModifyActorValue.ModificationType.AddAVMult => APerkEntryPointEffect.FunctionType.AddActorValueMult,
                                        PerkModifyActorValue.ModificationType.MultiplyAVMult => APerkEntryPointEffect.FunctionType.MultiplyActorValueMult,
                                        PerkModifyActorValue.ModificationType.MultiplyOnePlusAVMult => APerkEntryPointEffect.FunctionType.MultiplyOnePlusActorValueMult,
                                        PerkModifyActorValue.ModificationType.SetToAVMult => APerkEntryPointEffect.FunctionType.SetToActorValueMult,
                                        _ => throw new NotImplementedException()
                                    },
                                    PerkAbsoluteValue absVal => absVal.Negative ? APerkEntryPointEffect.FunctionType.NegativeAbsoluteValue : APerkEntryPointEffect.FunctionType.AbsoluteValue,
                                    PerkAddLeveledItem levItem => APerkEntryPointEffect.FunctionType.AddLeveledList,
                                    PerkAddActivateChoice activeChoice => APerkEntryPointEffect.FunctionType.AddActivateChoice,
                                    PerkSelectSpell spell => APerkEntryPointEffect.FunctionType.SelectSpell,
                                    PerkSelectText text => APerkEntryPointEffect.FunctionType.SelectText,
                                    PerkSetText ltext => APerkEntryPointEffect.FunctionType.SetText,
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
                            PerkModifyValue modVal => APerkEntryPointEffect.ParameterType.Float,
                            PerkAddRangeToValue modVal => APerkEntryPointEffect.ParameterType.FloatFloat,
                            PerkModifyActorValue modActorVal => APerkEntryPointEffect.ParameterType.FloatFloat,
                            PerkAbsoluteValue absVal => APerkEntryPointEffect.ParameterType.None,
                            PerkAddLeveledItem levItem => APerkEntryPointEffect.ParameterType.LeveledItem,
                            PerkAddActivateChoice activateChoice => APerkEntryPointEffect.ParameterType.SpellWithStrings,
                            PerkSelectSpell spell => APerkEntryPointEffect.ParameterType.Spell,
                            PerkSelectText txt => APerkEntryPointEffect.ParameterType.String,
                            PerkSetText ltxt => APerkEntryPointEffect.ParameterType.LString,
                            _ => throw new NotImplementedException()
                        };
                        using (HeaderExport.Subrecord(writer, RecordTypes.EPFT))
                        {
                            writer.Write((byte)paramType);
                        }
                        if (effect is PerkAddActivateChoice choice)
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
                            case PerkModifyValue modVal:
                                using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                                {
                                    writer.Write(modVal.Value);
                                }
                                break;
                            case PerkAddRangeToValue range:
                                using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                                {
                                    writer.Write(range.From);
                                    writer.Write(range.To);
                                }
                                break;
                            case PerkModifyActorValue actorVal:
                                using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                                {
                                    writer.Write((int)actorVal.ActorValue);
                                    writer.Write(actorVal.Value);
                                }
                                break;
                            case PerkAddLeveledItem lev:
                                using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                                {
                                    FormKeyBinaryTranslation.Instance.Write(writer, lev.Item.FormKey);
                                }
                                break;
                            case PerkAddActivateChoice activateChoice:
                                if (activateChoice.Spell.FormKey != null)
                                {
                                    using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                                    {
                                        FormKeyBinaryTranslation.Instance.Write(writer, activateChoice.Spell.FormKey.Value);
                                    }
                                }
                                break;
                            case PerkSelectSpell spell:
                                using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                                {
                                    FormKeyBinaryTranslation.Instance.Write(writer, spell.Spell.FormKey);
                                }
                                break;
                            case PerkSelectText text:
                                using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                                {
                                    writer.Write(text.Text, StringBinaryType.NullTerminate);
                                }
                                break;
                            case PerkSetText ltext:
                                using (HeaderExport.Subrecord(writer, RecordTypes.EPFD))
                                {
                                    StringBinaryTranslation.Instance.Write(writer, ltext.Text, StringBinaryType.NullTerminate, StringsSource.Normal);
                                }
                                break;
                            case PerkAbsoluteValue abs:
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
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string? INamedGetter.Name => this.Name?.String;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            TranslatedString ITranslatedNamedRequiredGetter.Name => this.Name ?? string.Empty;
            #endregion

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
