using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout4;

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
        TypedParseParams translationParams)
    {
        var majorMeta = frame.GetMajorRecordHeader();
        var settingType = GameSettingUtility.GetGameSettingType(frame.GetMemory(checked((int)majorMeta.TotalLength)), frame.MetaData.Constants);
        if (settingType.Failed)
        {
            throw new ArgumentException($"Error splitting to desired GameSetting type at position {frame.Position}: {settingType.Reason}");
        }
        switch (settingType.Value)
        {
            case GameSettingType.Float:
                return GameSettingFloat.CreateFromBinary(frame, translationParams);
            case GameSettingType.Int:
                return GameSettingInt.CreateFromBinary(frame, translationParams);
            case GameSettingType.String:
                return GameSettingString.CreateFromBinary(frame, translationParams);
            case GameSettingType.Bool:
                return GameSettingBool.CreateFromBinary(frame, translationParams);
            case GameSettingType.UInt:
                return GameSettingUInt.CreateFromBinary(frame, translationParams);
            default:
                throw new ArgumentException($"Unknown game type: {settingType.Value}");
        }
    }
}

partial class GameSettingBinaryOverlay
{
    public static IGameSettingGetter GameSettingFactory(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        TypedParseParams translationParams)
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
            case GameSettingType.UInt:
                return GameSettingUIntBinaryOverlay.GameSettingUIntFactory(stream, package);
            default:
                throw new ArgumentException($"Unknown game type: {settingType.Value}");
        }
    }
}