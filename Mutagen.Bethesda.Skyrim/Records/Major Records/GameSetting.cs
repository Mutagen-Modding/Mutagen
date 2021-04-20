using System;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Records.Binary.Streams;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class GameSetting : IGameSettingCommon
    {
        public abstract GameSettingType SettingType { get; }

        public override string? EditorID
        {
            get => base.EditorID;
            set => base.EditorID = value == null ? null : GameSettingUtility.CorrectEDID(value, this.SettingType);
        }

        public static GameSetting CreateFromBinary(
            MutagenFrame frame,
            RecordTypeConverter recordTypeConverter)
        {
            var majorMeta = frame.GetMajorRecord();
            var settingType = GameSettingUtility.GetGameSettingType(frame.GetMemory(checked((int)majorMeta.TotalLength)), frame.MetaData.Constants);
            if (settingType.Failed)
            {
                throw new ArgumentException($"Error splitting to desired GameSetting type at position {frame.Position}: {settingType.Reason}");
            }
            switch (settingType.Value)
            {
                case GameSettingType.Float:
                    return GameSettingFloat.CreateFromBinary(frame, recordTypeConverter);
                case GameSettingType.Int:
                    return GameSettingInt.CreateFromBinary(frame, recordTypeConverter);
                case GameSettingType.String:
                    return GameSettingString.CreateFromBinary(frame, recordTypeConverter);
                case GameSettingType.Bool:
                    return GameSettingBool.CreateFromBinary(frame, recordTypeConverter);
                default:
                    throw new ArgumentException($"Unknown game type: {settingType.Value}");
            }
        }
    }

    namespace Internals
    {
        public partial class GameSettingBinaryOverlay
        {
            public static GameSettingBinaryOverlay GameSettingFactory(
                OverlayStream stream,
                BinaryOverlayFactoryPackage package,
                RecordTypeConverter recordTypeConverter)
            {
                var settingType = GameSettingUtility.GetGameSettingType(stream.RemainingMemory, package.MetaData.Constants);
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
                    case GameSettingType.Bool:
                        return GameSettingBoolBinaryOverlay.GameSettingBoolFactory(stream, package);
                    default:
                        throw new ArgumentException($"Unknown game type: {settingType.Value}");
                }
            }
        }
    }
}
