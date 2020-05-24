using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class StringsUtility
    {
        public const string StringsFileExtension = "STRINGS";
        public const string ILStringsFileExtension = "DLSTRINGS";
        public const string DLStringsFileExtension = "ILSTRINGS";

        public static bool TryConvertToStringSource(ReadOnlySpan<char> str, out StringsSource source)
        {
            if (str.Equals(StringsFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                source = StringsSource.Normal;
                return true;
            }
            if (str.Equals(ILStringsFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                source = StringsSource.DL;
                return true;
            }
            if (str.Equals(DLStringsFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                source = StringsSource.IL;
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

        public static string GetFileName(ModKey modKey, Language language, StringsSource source)
        {
            return $"{modKey.Name}_{language}.{GetSourceString(source)}";
        }

        public static bool TryRetrieveInfoFromString(ReadOnlySpan<char> name, out StringsSource source, out Language language)
        {
            source = default;
            language = default;
            var extensionIndex = name.LastIndexOf('.');
            if (extensionIndex == -1) return false;
            if (!StringsUtility.TryConvertToStringSource(name.Slice(extensionIndex + 1), out source)) return false;
            var separatorIndex = name.LastIndexOf('_');
            if (separatorIndex == -1) return false;
            if (separatorIndex > extensionIndex) return false;
            var languageSpan = name.Slice(separatorIndex + 1, extensionIndex - separatorIndex - 1);
            return Enum.TryParse(languageSpan.ToString(), ignoreCase: true, out language);
        }
    }
}
