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
        String,
        Bool
    }

    public interface IGameSettingCommon : IMajorRecordCommon
    {
        GameSettingType SettingType { get; }
    }

    public interface IGameSettingNumeric : IGameSettingCommon
    {
        float? RawData { get; set; }
    }

    public static class GameSettingUtility
    {
        public const char IntChar = 'i';
        public const char FloatChar = 'f';
        public const char StringChar = 's';
        public const char BoolChar = 'b';

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
                case BoolChar:
                    type = GameSettingType.Bool;
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
                case GameSettingType.Bool:
                    return BoolChar;
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

        public static GetResponse<GameSettingType> GetGameSettingType(ReadOnlySpan<byte> span, GameConstants meta)
        {
            var majorMeta = meta.MajorRecordFrame(span);
            var edidLoc = UtilityTranslation.FindFirstSubrecord(majorMeta.Content, meta, Constants.EditorID);
            if (edidLoc == -1)
            {
                return GetResponse<GameSettingType>.Fail($"EDID was not located");
            }
            var edidMeta = meta.SubRecordFrame(majorMeta.Content.Slice(edidLoc));
            var edid = BinaryStringUtility.ProcessWholeToZString(edidMeta.Content);
            if (edid.Length == 0)
            {
                return GetResponse<GameSettingType>.Fail("No EDID parsed.");
            }
            if (!TryGetGameSettingType(edid[0], out var settingType))
            {
                return GetResponse<GameSettingType>.Fail($"Unknown game setting type: {edid[0]}");
            }
            return GetResponse<GameSettingType>.Succeed(settingType);
        }
    }
}
