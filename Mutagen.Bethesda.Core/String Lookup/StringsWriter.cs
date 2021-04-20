using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Records.Binary.Streams;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mutagen.Bethesda.Strings
{
    /// <summary>
    /// Class for compiling strings of various languages, and exporting them to a .strings file
    /// </summary>
    public class StringsWriter : IDisposable
    {
        public DirectoryPath WriteDir { get; }
        private readonly ModKey _modKey;
        private readonly StringsLanguageFormat _languageFormat;
        private readonly List<KeyValuePair<Language, string>[]> _strings = new List<KeyValuePair<Language, string>[]>();
        private readonly List<KeyValuePair<Language, string>[]> _ilStrings = new List<KeyValuePair<Language, string>[]>();
        private readonly List<KeyValuePair<Language, string>[]> _dlStrings = new List<KeyValuePair<Language, string>[]>();

        public StringsWriter(GameRelease release, ModKey modKey, DirectoryPath writeDirectory)
        {
            _modKey = modKey;
            _languageFormat = release.GetLanguageFormat();
            WriteDir = writeDirectory;
        }

        public uint Register(ITranslatedStringGetter str, StringsSource source)
        {
            List<KeyValuePair<Language, string>[]> strs = source switch
            {
                StringsSource.Normal => _strings,
                StringsSource.IL => _ilStrings,
                StringsSource.DL => _dlStrings,
                _ => throw new NotImplementedException(),
            };
            lock (strs)
            {
                // ToDo
                // Add Count member to TranslatedString, or something similar to short circuit array creation if unnecessary
                var arr = str.ToArray();
                if (!arr.Any(x => x.Value != null))
                {
                    // Do not insert into strings writer
                    return 0;
                }
                strs.Add(arr);
                try
                {
                    return checked((uint)strs.Count);
                }
                catch (OverflowException)
                {
                    throw new OverflowException("Too many translated strings for current system to handle.");
                }
            }
        }

        public uint Register(string str, Language language, StringsSource source)
        {
            List<KeyValuePair<Language, string>[]> strs = source switch
            {
                StringsSource.Normal => _strings,
                StringsSource.IL => _ilStrings,
                StringsSource.DL => _dlStrings,
                _ => throw new NotImplementedException(),
            };
            lock (strs)
            {
                strs.Add(new KeyValuePair<Language, string>[]
                {
                    new KeyValuePair<Language, string>(language, str)
                });
                try
                {
                    return checked((uint)strs.Count);
                }
                catch (OverflowException)
                {
                    throw new OverflowException("Too many translated strings for current system to handle.");
                }
            }
        }

        public void Dispose()
        {
            WriteStrings(_strings, StringsSource.Normal);
            WriteStrings(_ilStrings, StringsSource.IL);
            WriteStrings(_dlStrings, StringsSource.DL);
        }

        private void WriteStrings(List<KeyValuePair<Language, string>[]> strs, StringsSource source)
        {
            if (strs.Count == 0) return;
            WriteDir.Create();

            var subLists = new Dictionary<Language, List<(string String, int Index)>>();
            for (int i = 0; i < strs.Count; i++)
            {
                var item = strs[i];
                foreach (var lang in item)
                {
                    if (!subLists.TryGetValue(lang.Key, out var list))
                    {
                        list = new List<(string String, int Index)>();
                        subLists[lang.Key] = list;
                    }
                    list.Add((lang.Value, i + 1));
                }
            }

            foreach (var language in subLists)
            {
                using var writer = new MutagenWriter(
                    Path.Combine(WriteDir.Path, StringsUtility.GetFileName(_languageFormat, _modKey, language.Key, source)),
                    meta: null!);
                // Write count
                writer.Write(language.Value.Count);
                // Write filler for length later
                writer.WriteZeros(4);
                // Write Directory
                int size = 0;
                foreach (var item in language.Value)
                {
                    writer.Write(item.Index);
                    switch (source)
                    {
                        case StringsSource.Normal:
                            writer.Write(size);
                            size += item.String.Length + 1;
                            break;
                        case StringsSource.IL:
                        case StringsSource.DL:
                            writer.Write(size);
                            size += item.String.Length + 5;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                // Go back and write content length;
                var pos = writer.Position;
                writer.Position = 4;
                writer.Write(size);
                writer.Position = pos;
                // Write strings
                foreach (var item in language.Value)
                {
                    switch (source)
                    {
                        case StringsSource.Normal:
                            writer.Write(item.String, StringBinaryType.NullTerminate);
                            break;
                        case StringsSource.IL:
                        case StringsSource.DL:
                            writer.Write(item.String.Length + 1);
                            writer.Write(item.String, StringBinaryType.NullTerminate);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }
    }
}
