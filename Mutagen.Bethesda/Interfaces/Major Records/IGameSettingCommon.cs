using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public enum GameSettingType
    {
        Float,
        Int,
        String
    }

    public interface IGameSettingCommon : IMajorRecordCommon
    {
        GameSettingType SettingType { get; }
    }

    public interface IGameSettingNumeric : IGameSettingCommon
    {
        float RawData { get; set; }
    }

    public static class GameSettingUtility
    {
        public const char IntChar = 'i';
        public const char FloatChar = 'f';
        public const char StringChar = 's';

        public static bool TryGetGameSettingType(char c, out GameSettingType type)
        {
            switch (c)
            {
                case IntChar:
                    type = GameSettingType.Int;
                    return true;
                case StringChar:
                    type = GameSettingType.String;
                    return true;
                case FloatChar:
                    type = GameSettingType.Float;
                    return true;
                default:
                    type = default;
                    return false;
            }
        }

        public static char GetChar(this GameSettingType type)
        {
            switch (type)
            {
                case GameSettingType.Float:
                    return FloatChar;
                case GameSettingType.Int:
                    return IntChar;
                case GameSettingType.String:
                    return StringChar;
                default:
                    throw new NotImplementedException();
            }
        }

        public static string CorrectEDID(string input, GameSettingType type)
        {
            char triggerChar = type.GetChar();
            input = input.Trim();
            if (input.Length == 0)
            {
                return string.Empty + triggerChar;
            }
            else if (!triggerChar.Equals(input[0]))
            {
                return triggerChar + input;
            }
            return input;
        }

        public static GetResponse<GameSettingType> GetGameSettingType(MutagenFrame frame)
        {
            var initialPos = frame.Position;
            frame.Position += 20;
            if (!Constants.EditorID.Equals(HeaderTranslation.GetNextSubRecordType(frame.Reader, out var edidLength)))
            {
                return GetResponse<GameSettingType>.Fail($"EDID was not located in expected position: {frame.Position}");
            }
            frame.Position += 6;
            if (!StringBinaryTranslation.Instance.Parse(
                frame.SpawnWithLength(edidLength),
                out var edid))
            {
                return GetResponse<GameSettingType>.Fail($"EDID was parsed in expected position: {frame.Position}");
            }
            if (edid.Length == 0)
            {
                return GetResponse<GameSettingType>.Fail("No EDID parsed.");
            }
            frame.Position = initialPos;
            if (!TryGetGameSettingType(edid[0], out var settingType))
            {
                return GetResponse<GameSettingType>.Fail($"Unknown game setting type: {edid[0]}");
            }
            return GetResponse<GameSettingType>.Succeed(settingType);
        }
    }
}
