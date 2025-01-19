using System.IO.Abstractions;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Archives.Exceptions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Meta;

namespace Mutagen.Bethesda.Strings.DI;

public class StringsFolderLookupFactory
{
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IFileSystem _fileSystem;
    
    public StringsFolderLookupFactory(
        IDataDirectoryProvider dataDirectoryProvider,
        IFileSystem fileSystem)
    {
        _dataDirectoryProvider = dataDirectoryProvider;
        _fileSystem = fileSystem;
    }

    public IStringsFolderLookup Factory(
        GameRelease release,
        ModKey modKey,
        StringsReadParameters? instructions)
    {
        return InternalFactory(release, modKey, instructions);
    }

    internal StringsFolderLookupOverlay InternalFactory(
        GameRelease release, 
        ModKey modKey,
        StringsReadParameters? instructions)
    {
        var languageFormat = GameConstants.Get(release).StringsLanguageFormat ?? throw new ArgumentException($"Tried to get language format for an unsupported game: {release}", nameof(release));
        var encodings = instructions?.EncodingProvider ?? new MutagenEncodingProvider();
        var stringsFolderPath = instructions?.StringsFolderOverride;
        if (stringsFolderPath == null)
        {
            stringsFolderPath = Path.Combine(_dataDirectoryProvider.Path, "Strings");
        }

        var dataPath = _dataDirectoryProvider.Path;
        if (instructions?.BsaFolderOverride != null)
        {
            dataPath = instructions.BsaFolderOverride.Value;
        }
        
        return new StringsFolderLookupOverlay(new Lazy<StringsFolderLookupOverlay.DictionaryBundle>(
                isThreadSafe: true,
                valueFactory: () =>
                {
                    var bundle = new StringsFolderLookupOverlay.DictionaryBundle();
                    if (_fileSystem.Directory.Exists(stringsFolderPath.Value))
                    {
                        var bsaEnumer = stringsFolderPath.Value.EnumerateFiles(searchPattern: $"{modKey.Name}*{StringsUtility.StringsFileExtension}", fileSystem: _fileSystem);
                        foreach (var file in bsaEnumer)
                        {
                            if (!StringsUtility.TryRetrieveInfoFromString(
                                    languageFormat,
                                    file.Name.String, 
                                    out var type,
                                    out var lang, 
                                    out var modName)
                                || !modKey.Name.AsSpan().Equals(modName, StringComparison.InvariantCulture))
                            {
                                continue;
                            }
                            var dict = bundle.Get(type);
                            dict[lang] = new Lazy<StringsFolderLookupOverlay.LookupItem>(() =>
                                {
                                    return new StringsFolderLookupOverlay.LookupItem(
                                        new StringsLookupOverlay(file.Path, type, encodings.GetEncoding(release, lang), fileSystem: _fileSystem),
                                        file.Path);
                                },
                                LazyThreadSafetyMode.ExecutionAndPublication);
                        }
                    }
                    foreach (var bsaFile in Archive.GetApplicableArchivePaths(
                        release, dataPath, modKey, fileSystem: _fileSystem))
                    {
                        try
                        {
                            var bsaReader = Archive.CreateReader(release, bsaFile, _fileSystem);
                            if (!bsaReader.TryGetFolder("strings", out var stringsFolder)) continue;
                            try
                            {
                                foreach (var item in stringsFolder.Files)
                                {
                                    if (!StringsUtility.TryRetrieveInfoFromString(
                                            languageFormat, 
                                            Path.GetFileName(item.Path), 
                                            out var type, 
                                            out var lang,
                                            out var modName))
                                    {
                                        continue;
                                    }
                                    if (!MemoryExtensions.Equals(modKey.Name, modName, StringComparison.OrdinalIgnoreCase)) continue;
                                    var dict = bundle.Get(type);
                                    if (dict.ContainsKey(lang)) continue;
                                    dict[lang] = new Lazy<StringsFolderLookupOverlay.LookupItem>(() =>
                                    {
                                        try
                                        {
                                            return new StringsFolderLookupOverlay.LookupItem(
                                                new StringsLookupOverlay(item.GetMemorySlice(), type, encodings.GetEncoding(release, lang)),
                                                Path.Combine(bsaFile, item.Path));
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ArchiveException.EnrichWithFileAccessed("String file from BSA failed to parse", ex, item.Path);
                                        }
                                    }, LazyThreadSafetyMode.ExecutionAndPublication);
                                }
                            }
                            catch (Exception ex)
                                when (stringsFolder.Path != null)
                            {
                                throw ArchiveException.EnrichWithFolderAccessed("BSA folder failed to parse for string file", ex, stringsFolder.Path);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ArchiveException.EnrichWithArchivePath("BSA failed to parse for string file", ex, bsaFile);
                        }
                    }
                    return bundle;
                }),
            dataPath: dataPath,
            modKey: modKey);
    }
}