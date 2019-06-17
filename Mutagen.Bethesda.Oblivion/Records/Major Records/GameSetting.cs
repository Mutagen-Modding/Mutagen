using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog.Notifying;
using System;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class GameSetting : IGameSettingCommon
    {
        public abstract GameSettingType SettingType { get; }

        partial void CustomCtor()
        {
            this.WhenAny(x => x.EditorID)
                .Skip(1)
                .DistinctUntilChanged()
                .Subscribe(edid =>
                {
                    this.EditorID = GameSettingUtility.CorrectEDID(edid, this.SettingType);
                });
        }

        public static GameSetting CreateFromBinary(
            MutagenFrame frame,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask)
        {
            var settingType = GameSettingUtility.GetGameSettingType(frame);
            if (settingType.Failed)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"EDID was not located in expected position: {frame.Position}"));
                return null;
            }
            switch (settingType.Value)
            {
                case GameSettingType.Float:
                    return GameSettingFloat.CreateFromBinary(frame, masterReferences, recordTypeConverter, errorMask);
                case GameSettingType.Int:
                    return GameSettingInt.CreateFromBinary(frame, masterReferences, recordTypeConverter, errorMask);
                case GameSettingType.String:
                    return GameSettingString.CreateFromBinary(frame, masterReferences, recordTypeConverter, errorMask);
                default:
                    errorMask.ReportExceptionOrThrow(
                        new ArgumentException($"Unknown game type: {settingType.Value}"));
                    return null;
            }
        }
    }
}
