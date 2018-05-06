using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class GameSetting
    {
        protected abstract char TriggerChar { get; }

        partial void CustomCtor()
        {
            this.EditorID_Property.Subscribe(CorrectEDID, cmds: NotifyingSubscribeParameters.NoFire);
        }

        private void CorrectEDID(Change<string> change)
        {
            if (change.New.Length == 0)
            {
                this.EditorID = string.Empty + this.TriggerChar;
            }
            else if (!this.TriggerChar.Equals(change.New[0]))
            {
                this.EditorID = this.TriggerChar + change.New;
            }
        }

        public static (GameSetting Object, GameSetting_ErrorMask ErrorMask) Create_Binary(
            MutagenFrame frame,
            RecordTypeConverter recordTypeConverter,
            bool doMasks)
        {
            var initialPos = frame.Position;
            frame.Position += 20;
            if (!MajorRecord_Registration.EDID_HEADER.Equals(HeaderTranslation.GetNextSubRecordType(frame.Reader, out var edidLength)))
            {
                throw new ArgumentException($"EDID was not located in expected position: {frame.Position}");
            }
            frame.Position += 6;
            var edid = StringBinaryTranslation.Instance.Parse(
                frame.SpawnWithLength(edidLength),
                doMasks,
                out var err);
            if (err != null)
            {
                throw new ArgumentException($"Error parsing EDID: {err}");
            }
            if (edid.Failed || edid.Value.Length == 0)
            {
                throw new ArgumentException("No EDID parsed.");
            }
            frame.Position = initialPos;
            switch (edid.Value[0])
            {
                case GameSettingInt.TRIGGER_CHAR:
                    return GameSettingInt.Create_Binary(frame, recordTypeConverter, doMasks);
                case GameSettingString.TRIGGER_CHAR:
                    return GameSettingString.Create_Binary(frame, recordTypeConverter, doMasks);
                case GameSettingFloat.TRIGGER_CHAR:
                    return GameSettingFloat.Create_Binary(frame, recordTypeConverter, doMasks);
                default:
                    throw new ArgumentException($"Unknown game setting type: {edid.Value[0]}");
            }
        }
    }
}
