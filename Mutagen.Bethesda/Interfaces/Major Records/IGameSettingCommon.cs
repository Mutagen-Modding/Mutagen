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

        public static GetResponse<GameSettingType> GetGameSettingType(ReadOnlySpan<byte> span, MetaDataConstants meta)
        {
            span = span.Slice(meta.MajorConstants.HeaderLength);
            var subRecordMeta = meta.SubRecord(span);
            if (Constants.EditorID != subRecordMeta.RecordType)
            {
                return GetResponse<GameSettingType>.Fail($"EDID was not located");
            }
            var edid = BinaryStringUtility.ProcessWholeToZString(span.Slice(subRecordMeta.HeaderLength, subRecordMeta.RecordLength));
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
