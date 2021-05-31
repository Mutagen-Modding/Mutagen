using System;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Strings
{
    public static class StringsUtility
    {
        public const string StringsFileExtension = "STRINGS";
        public const string ILStringsFileExtension = "ILSTRINGS";
        public const string DLStringsFileExtension = "DLSTRINGS";

        public static bool TryConvertToStringSource(ReadOnlySpan<char> str, out StringsSource source)
        {
            if (str.Equals(StringsFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                source = StringsSource.Normal;
                return true;
            }
            if (str.Equals(ILStringsFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                source = StringsSource.IL;
                return true;
            }
            if (str.Equals(DLStringsFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                source = StringsSource.DL;
                return true;
            }
            source = default;
            return false;
        }

        public static string GetSourceString(StringsSource source)
        {
            switch (source)
            {
                case StringsSource.Normal:
                    return StringsFileExtension;
                case StringsSource.IL:
                    return ILStringsFileExtension;
                case StringsSource.DL:
                    return DLStringsFileExtension;
                default:
                    throw new NotImplementedException();
            }
        }

        public static StringsFileFormat GetFormat(StringsSource source)
        {
            switch (source)
            {
                case StringsSource.Normal:
                    return StringsFileFormat.Normal;
                case StringsSource.IL:
                case StringsSource.DL:
                    return StringsFileFormat.LengthPrepended;
                default:
                    throw new NotImplementedException();
            }
        }

        public static string GetFileName(
            StringsLanguageFormat languageFormat,
            ModKey modKey, 
            Language language,
            StringsSource source)
        {
            return $"{modKey.Name}_{GetLanguageString(languageFormat, language)}.{GetSourceString(source)}";
        }

        public static string GetLanguageString(StringsLanguageFormat format, Language language)
        {
            switch (format)
            {
                case StringsLanguageFormat.FullName:
                    return language.ToString();
                case StringsLanguageFormat.Iso:
                    return GetIsoLanguageString(language);
                default:
                    throw new NotImplementedException();
            }
        }

        public static string GetIsoLanguageString(Language language)
        {
            return language switch
            {
                Language.English => "en",
                Language.German => "de",
                Language.Italian => "it",
                Language.Spanish => "es",
                Language.Spanish_Mexico => "esmx",
                Language.French => "fr",
                Language.Polish => "pl",
                Language.Chinese => "cn",
                Language.Japanese => "ja",
                Language.Portuguese_Brazil => "ptbr",
                Language.Russian => "ru",
                _ => throw new NotImplementedException()
            };
        }

        public static bool TryGetIsoLanguage(ReadOnlySpan<char> chars, [MaybeNullWhen(false)] out Language lang)
        {
            switch (chars.ToString().ToLower())
            {
                case "en":
                    lang = Language.English;
                    return true;
                case "de":
                    lang = Language.German;
                    return true;
                case "it":
                    lang = Language.Italian;
                    return true;
                case "es":
                    lang = Language.Spanish;
                    return true;
                case "esmx":
                    lang = Language.Spanish_Mexico;
                    return true;
                case "fr":
                    lang = Language.French;
                    return true;
                case "pl":
                    lang = Language.Polish;
                    return true;
                case "cn":
                    lang = Language.Chinese;
                    return true;
                case "ja":
                    lang = Language.Japanese;
                    return true;
                case "ptbr":
                    lang = Language.Portuguese_Brazil;
                    return true;
                case "ru":
                    lang = Language.Russian;
                    return true;
                default:
                    lang = default;
                    return false;
            }
        }

        public static bool TryGetLanguage(StringsLanguageFormat languageFormat, ReadOnlySpan<char> span, [MaybeNullWhen(false)] out Language language)
        {
            switch (languageFormat)
            {
                case StringsLanguageFormat.FullName:
                    return Enum.TryParse(span.ToString(), ignoreCase: true, out language);
                case StringsLanguageFormat.Iso:
                    return TryGetIsoLanguage(span, out language);
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool TryRetrieveInfoFromString(
            StringsLanguageFormat languageFormat,
            ReadOnlySpan<char> name,
            out StringsSource source, 
            out Language language,
            out ReadOnlySpan<char> modName)
        {
            source = default;
            language = default;
            modName = default;
            var extensionIndex = name.LastIndexOf('.');
            if (extensionIndex == -1) return false;
            if (!TryConvertToStringSource(name.Slice(extensionIndex + 1), out source)) return false;
            var separatorIndex = name.LastIndexOf('_');
            if (separatorIndex == -1) return false;
            if (separatorIndex > extensionIndex) return false;
            var languageSpan = name.Slice(separatorIndex + 1, extensionIndex - separatorIndex - 1);
            modName = name.Slice(0, separatorIndex);
            return TryGetLanguage(languageFormat, languageSpan, out language);
        }
    }
}
