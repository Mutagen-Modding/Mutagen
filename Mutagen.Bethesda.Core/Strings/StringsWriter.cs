using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Mutagen.Bethesda.Strings.DI;

namespace Mutagen.Bethesda.Strings
{
    /// <summary>
    /// Class for compiling strings of various languages, and exporting them to a .strings file
    /// </summary>
    public class StringsWriter : IDisposable
    {
        public DirectoryPath WriteDir { get; }
        private readonly GameRelease _release;
        public ModKey ModKey { get; }
        public IMutagenEncodingProvider EncodingProvider { get; }
        public IFileSystem FileSystem { get; }
        public StringsLanguageFormat LanguageFormat { get; }
        private readonly List<KeyValuePair<Language, string>[]> _strings = new();
        private readonly List<KeyValuePair<Language, string>[]> _ilStrings = new();
        private readonly List<KeyValuePair<Language, string>[]> _dlStrings = new();

        public StringsWriter(
            GameRelease release,
            ModKey modKey, 
            DirectoryPath writeDirectory,
            IMutagenEncodingProvider encodingProvider,
            IFileSystem? fileSystem = null)
        {
            _release = release;
            ModKey = modKey;
            EncodingProvider = encodingProvider;
            FileSystem = fileSystem.GetOrDefault();
            LanguageFormat = release.GetLanguageFormat();
            WriteDir = writeDirectory;
        }

        public uint Register(ITranslatedStringGetter str, StringsSource source)
        {
            // ToDo
            // Add Count member to TranslatedString, or something similar to short circuit array creation if unnecessary
            return Register(source, str);
        }

        public uint Register(string str, Language language, StringsSource source)
        {
            return Register(source, new KeyValuePair<Language, string>(language, str));
        }

        public uint Register(StringsSource source, params KeyValuePair<Language, string>[] strs)
        {
            return Register(source, (IEnumerable<KeyValuePair<Language, string>>)strs);
        }

        public uint Register(StringsSource source, IEnumerable<KeyValuePair<Language, string>> strs)
        {
            List<KeyValuePair<Language, string>[]> strsList = source switch
            {
                StringsSource.Normal => _strings,
                StringsSource.IL => _ilStrings,
                StringsSource.DL => _dlStrings,
                _ => throw new NotImplementedException(),
            };
            lock (strsList)
            {
                strsList.Add(strs.ToArray());
                try
                {
                    return checked((uint)strsList.Count);
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
            FileSystem.Directory.CreateDirectory(WriteDir);

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
                var encoding = EncodingProvider.GetEncoding(_release, language.Key);
                using var writer = new MutagenWriter(
                    FileSystem.FileStream.Create(
                        Path.Combine(WriteDir.Path, StringsUtility.GetFileName(LanguageFormat, ModKey, language.Key, source)), 
                        FileMode.Create, FileAccess.Write),
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
                    var strLen = encoding.GetByteCount(item.String);
                    switch (source)
                    {
                        case StringsSource.Normal:
                            writer.Write(size);
                            size += strLen + 1;
                            break;
                        case StringsSource.IL:
                        case StringsSource.DL:
                            writer.Write(size);
                            size += strLen + 5;
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
                            writer.Write(item.String, StringBinaryType.NullTerminate, encoding);
                            break;
                        case StringsSource.IL:
                        case StringsSource.DL:
                            writer.Write(item.String.Length + 1);
                            writer.Write(item.String, StringBinaryType.NullTerminate, encoding);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }
    }
}
