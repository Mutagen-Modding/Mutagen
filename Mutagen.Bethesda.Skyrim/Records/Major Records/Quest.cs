using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Quest
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
        #endregion

        [Flags]
        public enum Flag
        {
            StartGameEnabled = 0x0001,
            AllowRepeatedStages = 0x0008,
            RunOnce = 0x0100,
            ExcludeFromDialogExport = 0x0200,
            WarnOnAliasFillFailure = 0x0400
        }

        public enum TypeEnum
        { 
            None = 0,
            MainQuest = 1,
            MageGuild = 2,
            ThievesGuild = 3,
            DarkBrotherhood = 4,
            CompanionQuests = 5,
            Misc = 6,
            Daedric = 7,
            SideQuest = 8,
            CivilWar = 9,
            Vampire = 10,
            Dragonborn = 11,
        }

        [Flags]
        public enum TargetFlag
        {
            CompassMarkerIgnoresLocks = 0x1,
        }
    }

    namespace Internals
    {
        public partial class QuestBinaryCreateTranslation
        {
            static partial void FillBinaryDialogConditionsCustom(MutagenFrame frame, IQuestInternal item)
            {
                ConditionBinaryCreateTranslation.FillConditionsList(item.DialogConditions, frame);
            }

            static partial void FillBinaryUnusedConditionsLogicCustom(MutagenFrame frame, IQuestInternal item)
            {
                var nextHeader = frame.ReadSubrecordFrame();
                if (nextHeader.Header.RecordType != RecordTypes.NEXT
                    || nextHeader.Content.Length != 0)
                {
                    throw new ArgumentException("Unexpected NEXT header");
                }
                ConditionBinaryCreateTranslation.FillConditionsList(item.UnusedConditions, frame);
            }

            static partial void FillBinaryNextAliasIDCustom(MutagenFrame frame, IQuestInternal item)
            {
                // Skip
                frame.ReadSubrecordFrame();
            }
        }

        public partial class QuestBinaryWriteTranslation
        {
            static partial void WriteBinaryDialogConditionsCustom(MutagenWriter writer, IQuestGetter item)
            {
                ConditionBinaryWriteTranslation.WriteConditionsList(item.DialogConditions, writer);
            }

            static partial void WriteBinaryUnusedConditionsLogicCustom(MutagenWriter writer, IQuestGetter item)
            {
                using (HeaderExport.Subrecord(writer, RecordTypes.NEXT)) { }
                ConditionBinaryWriteTranslation.WriteConditionsList(item.UnusedConditions, writer);
            }

            static partial void WriteBinaryNextAliasIDCustom(MutagenWriter writer, IQuestGetter item)
            {
                var aliases = item.Aliases;
                using (HeaderExport.Subrecord(writer, RecordTypes.ANAM))
                {
                    if (aliases.Count == 0)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(aliases.Select(x => x.ID).Max() + 1);
                    }
                }
            }
        }

        public partial class QuestBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string? INamedGetter.Name => this.Name?.String;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            TranslatedString ITranslatedNamedRequiredGetter.Name => this.Name ?? string.Empty;
            #endregion
            public IReadOnlyList<IConditionGetter> DialogConditions { get; private set; } = ListExt.Empty<IConditionGetter>();
            public IReadOnlyList<IConditionGetter> UnusedConditions { get; private set; } = ListExt.Empty<IConditionGetter>();

            partial void DialogConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                DialogConditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
            }

            partial void UnusedConditionsLogicCustomParse(OverlayStream stream, int offset)
            {
                var nextHeader = stream.ReadSubrecordFrame();
                if (nextHeader.Header.RecordType != RecordTypes.NEXT
                    || nextHeader.Content.Length != 0)
                {
                    throw new ArgumentException("Unexpected NEXT header");
                }
                UnusedConditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
            }

            partial void NextAliasIDCustomParse(OverlayStream stream, int offset)
            {
                stream.ReadSubrecordFrame();
            }
        }
    }
}
