using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using System;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using Noggog;

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
            var majorMeta = frame.MetaData.GetMajorRecord(frame);
            var settingType = GameSettingUtility.GetGameSettingType(frame.GetSpan(checked((int)majorMeta.TotalLength)), frame.MetaData);
            if (settingType.Failed)
            {
                errorMask.ReportExceptionOrThrow(new ArgumentException($"Error splitting to desired GameSetting type at position {frame.Position}: {settingType.Reason}"));
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

    namespace Internals
    {
        public partial class GameSettingBinaryOverlay
        {
            public static GameSettingBinaryOverlay GameSettingFactory(
                BinaryMemoryReadStream stream,
                BinaryOverlayFactoryPackage package,
                RecordTypeConverter recordTypeConverter)
            {
                var settingType = GameSettingUtility.GetGameSettingType(stream.RemainingSpan, package.Meta);
                if (settingType.Failed)
                {
                    throw new ArgumentException($"Error splitting to desired GameSetting type: {settingType.Reason}");
                }
                switch (settingType.Value)
                {
                    case GameSettingType.Float:
                        return GameSettingFloatBinaryOverlay.GameSettingFloatFactory(stream, package);
                    case GameSettingType.Int:
                        return GameSettingIntBinaryOverlay.GameSettingIntFactory(stream, package);
                    case GameSettingType.String:
                        return GameSettingStringBinaryOverlay.GameSettingStringFactory(stream, package);
                    default:
                        throw new ArgumentException($"Unknown game type: {settingType.Value}");
                }
            }
        }
    }
}
