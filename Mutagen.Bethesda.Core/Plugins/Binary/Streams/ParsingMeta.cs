using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Streams;

/// <summary>
/// Class containing all the extra meta bits for parsing
/// </summary>
public sealed class ParsingMeta
{
    /// <summary>
    /// Game constants meta object to reference for header length measurements
    /// </summary>
    public GameConstants Constants { get; }

    /// <summary>
    /// Masters to reference while reading
    /// </summary>
    public IReadOnlySeparatedMasterPackage MasterReferences { get; set; }

    /// <summary>
    /// Optional RecordInfoCache to reference while reading
    /// </summary>
    public RecordTypeInfoCacheReader? RecordInfoCache { get; set; }

    /// <summary>
    /// Optional strings lookup to reference while reading
    /// </summary>
    public IStringsFolderLookup? StringsLookup { get; set; }

    /// <summary>
    /// Whether to do parallel work when possible
    /// </summary>
    public bool Parallel { get; set; }

    /// <summary>
    /// Tracker of whether within worldspace data section
    /// </summary>
    public bool InWorldspace { get; set; }

    /// <summary>
    /// Tracker of current major record version
    /// </summary>
    public ushort? FormVersion { get; set; }

    /// <summary>
    /// ModKey of the mod being parsed
    /// </summary>
    public ModKey ModKey { get; }

    public EncodingBundle Encodings { get; set; } = new(MutagenEncoding._1252, MutagenEncoding._1252);

    public Language TranslatedTargetLanguage { get; set; } = Language.English;

    public bool ThrowOnUnknown { get; set; }

    public IFileSystem FileSystem { get; set; } = IFileSystemExt.DefaultFilesystem;

    internal ParsingMeta(
        GameConstants constants,
        ModKey modKey,
        IReadOnlySeparatedMasterPackage masterReferences)
    {
        Constants = constants;
        ModKey = modKey;
        MasterReferences = masterReferences;
    }

    public static implicit operator GameConstants(ParsingMeta bundle)
    {
        return bundle.Constants;
    }

    public void ReportIssue(RecordType? recordType, string note)
    {
        // Nothing for now.  Need to implement
    }

    private void Absorb(StringsReadParameters? stringsReadParameters)
    {
        if (stringsReadParameters == null) return;
        if (stringsReadParameters.TargetLanguage != null)
        {
            TranslatedTargetLanguage = stringsReadParameters.TargetLanguage.Value;
        }

        if (stringsReadParameters.NonLocalizedEncodingOverride == null)
        {
            var encodingProv = stringsReadParameters.EncodingProvider ?? MutagenEncoding.Default;
            Encodings = Encodings with
            {
                NonLocalized = encodingProv.GetEncoding(Constants.Release, TranslatedTargetLanguage)
            };
        }
        else
        {
            Encodings = Encodings with
            {
                NonLocalized = stringsReadParameters.NonLocalizedEncodingOverride
            };
        }

        if (stringsReadParameters.NonTranslatedEncodingOverride != null)
        {
            Encodings = Encodings with
            {
                NonTranslated = stringsReadParameters.NonTranslatedEncodingOverride
            };
        }
    }

    private void Absorb(BinaryReadParameters? readParameters)
    {
        if (readParameters == null) return;
        if (Constants.UsesStrings)
        {
            Absorb(readParameters.StringsParam);
        }
        ThrowOnUnknown = readParameters.ThrowOnUnknownSubrecord;
        Parallel = readParameters.Parallel;
        FileSystem = readParameters.FileSystem.GetOrDefault();
    }

    public static ParsingMeta Factory(
        BinaryReadParameters param,
        GameRelease release,
        ModPath modPath)
    {
        var rawMasters = MasterReferenceCollection.FromPath(modPath, release, param.FileSystem);
        var masters = SeparatedMasterPackage.Factory(release, modPath, rawMasters, param.LoadOrder);
        var meta = new ParsingMeta(GameConstants.Get(release), modPath.ModKey, masters);
        meta.Absorb(param);
        return meta;
    }

    public static ParsingMeta Factory(
        BinaryReadParameters param,
        GameRelease release,
        ModKey modKey,
        Stream stream)
    {
        var rawMasters = MasterReferenceCollection.FromStream(stream, modKey, release);
        stream.Position = 0;
        var masters = SeparatedMasterPackage.Factory(release, modKey, rawMasters, param.LoadOrder);
        var meta = new ParsingMeta(GameConstants.Get(release), modKey, masters);
        meta.Absorb(param);
        return meta;
    }

    // public static ParsingMeta Factory(GameRelease release, ModPath path)
    // {
    //     var constants = GameConstants.Get(release);
    //     return new ParsingMeta(
    //         constants,
    //         path.ModKey,
    //         MasterReferenceCollection.FromPath(path, release));
    // }
}