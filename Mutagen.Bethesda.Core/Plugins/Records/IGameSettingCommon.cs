using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using System;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Strings.DI;

namespace Mutagen.Bethesda.Plugins.Records;

/// <summary>
/// Various types that a GameSetting can contain
/// </summary>
public enum GameSettingType
{
    Float,
    Int,
    String,
    Bool,
    UInt
}

/// <summary>
/// An interface that GameSetting objects implement to hook into the common systems
/// </summary>
public interface IGameSettingCommon : IMajorRecord
{
    /// <summary>
    /// The type of data that the GameSetting contains
    /// </summary>
    GameSettingType SettingType { get; }
}

/// <summary>
/// An interface that numeric GameSetting objects implement to hook into the common systems
/// </summary>
public interface IGameSettingNumeric : IGameSettingCommon
{
    /// <summary>
    /// Raw float data representation of the numeric GameSetting's content
    /// </summary>
    float? RawData { get; set; }
}

/// <summary>
/// Static class to provide common GameSetting utility concepts
/// </summary>
public static class GameSettingUtility
{
    /// <summary>
    /// Character signal for Integer content
    /// </summary>
    public const char IntChar = 'i';
    /// <summary>
    /// Character signal for Float content
    /// </summary>
    public const char FloatChar = 'f';
    /// <summary>
    /// Character signal for String content
    /// </summary>
    public const char StringChar = 's';
    /// <summary>
    /// Character signal for Boolean content
    /// </summary>
    public const char BoolChar = 'b';
    /// <summary>
    /// Character signal for Unsigned Integer content
    /// </summary>
    public const char UIntChar = 'u';

    /// <summary>
    /// Tries to convert a character to a corresponding enum type value.
    /// </summary>
    /// <param name="c">Character to convert</param>
    /// <param name="type">Resulting enum type, if convertable</param>
    /// <returns>True if character was able to be converted to an enum value</returns>
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
            case UIntChar:
                type = GameSettingType.UInt;
                return true;
            default:
                type = default;
                return false;
        }
    }

    /// <summary>
    /// Tries to convert a character to a corresponding enum type value.
    /// </summary>
    /// <param name="type">The type enum to convert</param>
    /// <returns>Character paired with the type enum</returns>
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
            case GameSettingType.UInt:
                return UIntChar;
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Adjusts a string to be GameSetting compliant with the given type.
    /// This means having the correct character at the start of the string.
    /// </summary>
    /// <param name="input">String to check and correct</param>
    /// <param name="type">Game type to conform to</param>
    /// <returns>GameSetting compliant EditorID string</returns>
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

    /// <summary>
    /// Takes a span aligned to a major record, and attempts to locate the game setting type.
    /// Will throw if the span is misaligned or doesn't start at a valid major record header.
    /// </summary>
    /// <param name="span">Data beginning at the start of a major record</param>
    /// <param name="meta">Game meta information to use in parsing</param>
    /// <returns>A response of the GameSettingType if found, or a reason if not.</returns>
    public static GetResponse<GameSettingType> GetGameSettingType(ReadOnlyMemorySlice<byte> span, GameConstants meta)
    {
        var majorMeta = meta.MajorRecordFrame(span);
        var edidFrame = majorMeta.LocateSubrecordFrame(RecordTypes.EDID);
        var edid = edidFrame.AsString(MutagenEncodingProvider._1252);
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