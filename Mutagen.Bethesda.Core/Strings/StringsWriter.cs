using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.IO.Abstractions;
using Mutagen.Bethesda.Strings.DI;

namespace Mutagen.Bethesda.Strings;

/// <summary>
/// Class for compiling strings of various languages, and exporting them to a .strings file
/// </summary>
public sealed class StringsWriter : IDisposable
{
    public DirectoryPath WriteDir { get; }
    private readonly GameRelease _release;
    public ModKey ModKey { get; }
    public IMutagenEncodingProvider EncodingProvider { get; }
    public IFileSystem FileSystem { get; }
    public StringsLanguageFormat LanguageFormat { get; }
    private readonly List<ValueTuple<Language, string, uint>[]> _strings = new();
    private readonly List<ValueTuple<Language, string, uint>[]> _ilStrings = new();
    private readonly List<ValueTuple<Language, string, uint>[]> _dlStrings = new();
    private uint _index = 0;

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
        if (!strs.Any()) return 0;
        List<ValueTuple<Language, string, uint>[]> strsList = source switch
        {
            StringsSource.Normal => _strings,
            StringsSource.IL => _ilStrings,
            StringsSource.DL => _dlStrings,
            _ => throw new NotImplementedException(),
        };
        try
        {
            var nextIndex = Interlocked.Increment(ref _index);
            if (nextIndex == 0)
            {
                throw new OverflowException();
            }
            lock (strsList)
            {
                strsList.Add(strs.Select(x => new ValueTuple<Language, string, uint>(x.Key, x.Value, nextIndex)).ToArray());
                return nextIndex;
            }
        }
        catch (OverflowException)
        {
            throw new OverflowException("Too many translated strings for current system to handle.");
        }
    }

    public void Dispose()
    {
        var languages = 
            _strings
                .Concat(_ilStrings)
                .Concat(_dlStrings)
                .SelectMany(x => x)
                .Select(x => x.Item1)
                .ToHashSet();
        WriteStrings(_strings, StringsSource.Normal, languages);
        WriteStrings(_ilStrings, StringsSource.IL, languages);
        WriteStrings(_dlStrings, StringsSource.DL, languages);
    }

    private void WriteStrings(List<ValueTuple<Language, string, uint>[]> strs, StringsSource source, IReadOnlyCollection<Language> requiredLanguages)
    {
        FileSystem.Directory.CreateDirectory(WriteDir);

        var subLists = new Dictionary<Language, List<(string String, uint Index)>>();
        foreach (var req in requiredLanguages)
        {
            subLists.GetOrAdd(req);
        }
        foreach (var item in strs)
        {
            foreach (var lang in item)
            {
                var list = subLists[lang.Item1];
                list.Add((lang.Item2, lang.Item3));
            }
        }

        foreach (var language in subLists)
        {
            var encoding = EncodingProvider.GetEncoding(_release, language.Key);
            using var writer = new MutagenWriter(
                FileSystem.FileStream.New(
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