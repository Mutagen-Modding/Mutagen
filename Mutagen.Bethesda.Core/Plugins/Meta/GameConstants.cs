using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Strings;
using Noggog;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Strings.DI;

namespace Mutagen.Bethesda.Plugins.Meta;

/// <summary> 
/// Reference for all the alignment and length constants related to a specific game 
/// </summary> 
public sealed record GameConstants
{
    /// <summary> 
    /// Associated game type 
    /// </summary> 
    public GameRelease Release { get; init; }

    /// <summary> 
    /// Length of the Mod header's metadata, excluding content 
    /// </summary> 
    public sbyte ModHeaderLength { get; }

    /// <summary> 
    /// Length of the Mod header's non-fundamental metadata 
    /// </summary> 
    public sbyte ModHeaderFluffLength { get; }

    /// <summary> 
    /// Group constants 
    /// </summary> 
    public GroupConstants GroupConstants { get; }

    /// <summary> 
    /// Major Record constants 
    /// </summary> 
    public MajorRecordConstants MajorConstants { get; }

    /// <summary> 
    /// Sub Record constants 
    /// </summary> 
    public RecordHeaderConstants SubConstants { get; }

    public ReadOnlyMemorySlice<Language> Languages { get; }
    public EncodingBundle Encodings { get; }

    public IReadOnlyCollection<RecordType> HeaderOverflow { get; } = new SingleCollection<RecordType>(RecordTypes.XXXX);

    public bool HasEnabledMarkers { get; init; }
    
    public ushort? DefaultFormVersion { get; init; }
    
    public string? MyDocumentsString { get; init; }
    
    public string IniName { get; init; }
    
    public StringsLanguageFormat? StringsLanguageFormat { get; init; }
    
    /// <summary> 
    /// Constructor 
    /// </summary> 
    /// <param name="release">Game Release to associate with the constants</param> 
    /// <param name="modHeaderLength">Length of the ModHeader</param> 
    /// <param name="modHeaderFluffLength">Length of the ModHeader excluding initial recordtype and length bytes.</param> 
    /// <param name="groupConstants">Constants defining Groups</param> 
    /// <param name="majorConstants">Constants defining Major Records</param> 
    /// <param name="subConstants">Constants defining Sub Records</param>
    /// <param name="languages">Languages supported</param>
    public GameConstants(
        GameRelease release,
        sbyte modHeaderLength,
        sbyte modHeaderFluffLength,
        GroupConstants groupConstants,
        MajorRecordConstants majorConstants,
        RecordHeaderConstants subConstants,
        Language[] languages,
        StringsLanguageFormat? languageFormat,
        EncodingBundle encodings,
        bool hasEnabledMarkers,
        ushort? defaultFormVersion,
        string? myDocumentsString,
        string iniName)
    {
        Release = release;
        ModHeaderLength = modHeaderLength;
        ModHeaderFluffLength = modHeaderFluffLength;
        GroupConstants = groupConstants;
        MajorConstants = majorConstants;
        SubConstants = subConstants;
        Languages = languages;
        Encodings = encodings;
        HasEnabledMarkers = hasEnabledMarkers;
        DefaultFormVersion = defaultFormVersion;
        MyDocumentsString = myDocumentsString;
        StringsLanguageFormat = languageFormat;
        IniName = iniName;
    }

    /// <summary> 
    /// Readonly singleton of Oblivion game constants 
    /// </summary> 
    public static readonly GameConstants Oblivion = new GameConstants(
        release: GameRelease.Oblivion,
        modHeaderLength: 20,
        modHeaderFluffLength: 12,
        groupConstants: new GroupConstants(
            ObjectType.Group,
            headerLength: 20,
            lengthLength: 4,
            cell: new GroupCellConstants(6, SubTypes: new[] { 8, 9, 10 }),
            world: new GroupWorldConstants(
                TopGroupType: 1,
                CellGroupTypes: new[] { 2, 4 },
                CellSubGroupTypes: new[] { 3, 5 }),
            topic: new GroupTopicConstants(7),
            hasSubGroups: new int[] { 1, 2, 4, 6, 7 },
            new GroupNesting[]
            {
                new GroupNesting(2,
                    new GroupNesting(HasTopLevelRecordType: true, 3,
                        new GroupNesting(6,
                            new GroupNesting(8),
                            new GroupNesting(9),
                            new GroupNesting(10)))),
                new GroupNesting(GroupType: 7),
                new GroupNesting(
                    HasTopLevelRecordType: true, GroupType: 1,
                    new GroupNesting(
                        GroupType: 6,
                        new GroupNesting(8),
                        new GroupNesting(9),
                        new GroupNesting(10)),
                    new GroupNesting(4,
                        new GroupNesting(HasTopLevelRecordType: true, 5,
                            new GroupNesting(
                                GroupType: 6,
                                new GroupNesting(8),
                                new GroupNesting(9),
                                new GroupNesting(10))))),
            }),
        majorConstants: new MajorRecordConstants(
            headerLength: 20,
            lengthLength: 4,
            flagsLoc: 8,
            formIDloc: 12,
            formVersionLoc: null),
        subConstants: new RecordHeaderConstants(
            ObjectType.Subrecord,
            headerLength: 6,
            lengthLength: 2),
        languages: Array.Empty<Language>(),
        languageFormat: null,
        hasEnabledMarkers: false,
        defaultFormVersion: null,
        myDocumentsString: "Oblivion",
        iniName: "Oblivion",
        encodings: new(NonTranslated: MutagenEncoding._1252, NonLocalized: MutagenEncoding._1252));

    /// <summary> 
    /// Readonly singleton of Skyrim LE game constants 
    /// </summary> 
    public static readonly GameConstants SkyrimLE = new GameConstants(
        release: GameRelease.SkyrimLE,
        modHeaderLength: 24,
        modHeaderFluffLength: 16,
        groupConstants: new GroupConstants(
            ObjectType.Group,
            headerLength: 24,
            lengthLength: 4,
            cell: new GroupCellConstants(6, SubTypes: new[] { 8, 9 }),
            world: new GroupWorldConstants(
                TopGroupType: 1,
                CellGroupTypes: new[] { 2, 4 },
                CellSubGroupTypes: new[] { 3, 5 }),
            topic: new GroupTopicConstants(7),
            hasSubGroups: new int[] { 1, 2, 4, 6, 7 },
            new GroupNesting[]
            {
                new GroupNesting(2,
                    new GroupNesting(HasTopLevelRecordType: true, 3,
                        new GroupNesting(6,
                            new GroupNesting(8),
                            new GroupNesting(9)))),
                new GroupNesting(GroupType: 7),
                new GroupNesting(
                    HasTopLevelRecordType: true, GroupType: 1,
                    new GroupNesting(
                        GroupType: 6,
                        new GroupNesting(8),
                        new GroupNesting(9)),
                    new GroupNesting(4,
                        new GroupNesting(HasTopLevelRecordType: true, 5,
                            new GroupNesting(
                                GroupType: 6,
                                new GroupNesting(8),
                                new GroupNesting(9))))),
            }),
        majorConstants: new MajorRecordConstants(
            headerLength: 24,
            lengthLength: 4,
            flagsLoc: 8,
            formIDloc: 12,
            formVersionLoc: 20),
        subConstants: new RecordHeaderConstants(
            ObjectType.Subrecord,
            headerLength: 6,
            lengthLength: 2),
        languages: new Language[]
        {
            Language.English,
            Language.French,
            Language.Italian,
            Language.German,
            Language.Spanish,
            Language.Polish,
            Language.Chinese,
            Language.Russian,
        },
        languageFormat: Strings.StringsLanguageFormat.FullName,
        hasEnabledMarkers: false,
        defaultFormVersion: 43,
        myDocumentsString: "Skyrim",
        iniName: "Skyrim",
        encodings: new(NonTranslated: MutagenEncoding._1252, NonLocalized: MutagenEncoding._1252));

    public static readonly GameConstants EnderalLE = SkyrimLE with
    {
        Release = GameRelease.EnderalLE,
        MyDocumentsString = "Enderal",
        IniName = "Enderal",
    };
    
    /// <summary> 
    /// Readonly singleton of Skyrim SE game constants 
    /// </summary> 
    public static readonly GameConstants SkyrimSE = SkyrimLE with
    {
        Release = GameRelease.SkyrimSE,
        HasEnabledMarkers = true,
        DefaultFormVersion = 44,
        MyDocumentsString = "Skyrim Special Edition",
    };
    
    /// <summary> 
    /// Readonly singleton of Skyrim SE game constants 
    /// </summary> 
    public static readonly GameConstants SkyrimSEGog = SkyrimSE with
    {
        Release = GameRelease.SkyrimSEGog,
        MyDocumentsString = "Skyrim Special Edition GOG",
    };

    /// <summary> 
    /// Readonly singleton of Skyrim SE game constants 
    /// </summary> 
    public static readonly GameConstants SkyrimVR = SkyrimSE with
    {
        Release = GameRelease.SkyrimVR,
        MyDocumentsString = "Skyrim VR",
        IniName = "SkyrimVR",
    };

    public static readonly GameConstants EnderalSE = SkyrimSE with
    {
        Release = GameRelease.EnderalSE,
        MyDocumentsString = "Enderal Special Edition",
        IniName = "Enderal",
    };

    /// <summary> 
    /// Readonly singleton of Fallout4 game constants 
    /// </summary> 
    public static readonly GameConstants Fallout4 = new GameConstants(
        release: GameRelease.Fallout4,
        modHeaderLength: 24,
        modHeaderFluffLength: 16,
        groupConstants: new GroupConstants(
            ObjectType.Group,
            headerLength: 24,
            lengthLength: 4,
            cell: new GroupCellConstants(6, SubTypes: new[] { 8, 9 }),
            world: new GroupWorldConstants(
                TopGroupType: 1,
                CellGroupTypes: new[] { 2, 4 },
                CellSubGroupTypes: new[] { 3, 5 }),
            topic: new GroupTopicConstants(7),
            hasSubGroups: new int[] { 1, 2, 4, 6, 7, 10 },
            new GroupNesting[]
            {
                new GroupNesting(2,
                    new GroupNesting(HasTopLevelRecordType: true, 3,
                        new GroupNesting(6,
                            new GroupNesting(8),
                            new GroupNesting(9)))),
                new GroupNesting(HasTopLevelRecordType: true, GroupType: 10,
                    new GroupNesting(GroupType: 7)),
                new GroupNesting(
                    HasTopLevelRecordType: true, GroupType: 1,
                    new GroupNesting(
                        GroupType: 6,
                        new GroupNesting(8),
                        new GroupNesting(9)),
                    new GroupNesting(4,
                        new GroupNesting(HasTopLevelRecordType: true, 5,
                            new GroupNesting(
                                GroupType: 6,
                                new GroupNesting(8),
                                new GroupNesting(9))))),
            })
        {
            Quest = new GroupQuestConstants(10)
        },
        majorConstants: new MajorRecordConstants(
            headerLength: 24,
            lengthLength: 4,
            flagsLoc: 8,
            formIDloc: 12,
            formVersionLoc: 20),
        subConstants: new RecordHeaderConstants(
            ObjectType.Subrecord,
            headerLength: 6,
            lengthLength: 2),
        languages: new Language[]
        {
            Language.English,
            Language.German,
            Language.Italian,
            Language.Spanish,
            Language.Spanish_Mexico,
            Language.French,
            Language.Polish,
            Language.Portuguese_Brazil,
            Language.Chinese,
            Language.Russian,
            Language.Japanese,
        },
        languageFormat: Strings.StringsLanguageFormat.Iso,
        hasEnabledMarkers: true,
        defaultFormVersion: 131,
        myDocumentsString: "Fallout4",
        iniName: "Fallout4",
        encodings: new(NonTranslated: MutagenEncoding._1252, NonLocalized: MutagenEncoding._1252));

    public static readonly GameConstants Fallout4VR = Fallout4 with
    {
        Release = GameRelease.Fallout4VR,
        MyDocumentsString = null,
        IniName = "Fallout4",
    };

    /// <summary> 
    /// Readonly singleton of Starfield game constants 
    /// </summary> 
    public static readonly GameConstants Starfield = new GameConstants(
        release: GameRelease.Starfield,
        modHeaderLength: 24,
        modHeaderFluffLength: 16,
        groupConstants: new GroupConstants(
            ObjectType.Group,
            headerLength: 24,
            lengthLength: 4,
            cell: new GroupCellConstants(6, SubTypes: new[] { 8, 9 }),
            world: new GroupWorldConstants(
                TopGroupType: 1,
                CellGroupTypes: new[] { 2, 4 },
                CellSubGroupTypes: new[] { 3, 5 }),
            topic: new GroupTopicConstants(7),
            hasSubGroups: new int[] { 1, 2, 4, 6, 7, 10 },
            new GroupNesting[]
            {
                new GroupNesting(2,
                    new GroupNesting(HasTopLevelRecordType: true, 3,
                        new GroupNesting(6,
                            new GroupNesting(8),
                            new GroupNesting(9)))),
                new GroupNesting(HasTopLevelRecordType: true, GroupType: 10,
                    new GroupNesting(GroupType: 7)),
                new GroupNesting(
                    HasTopLevelRecordType: true, GroupType: 1,
                    new GroupNesting(
                        GroupType: 6,
                        new GroupNesting(8),
                        new GroupNesting(9)),
                    new GroupNesting(4,
                        new GroupNesting(HasTopLevelRecordType: true, 5,
                            new GroupNesting(
                                GroupType: 6,
                                new GroupNesting(8),
                                new GroupNesting(9))))),
            })
        {
            Quest = new GroupQuestConstants(10)
        },
        majorConstants: new MajorRecordConstants(
            headerLength: 24,
            lengthLength: 4,
            flagsLoc: 8,
            formIDloc: 12,
            formVersionLoc: 20),
        subConstants: new RecordHeaderConstants(
            ObjectType.Subrecord,
            headerLength: 6,
            lengthLength: 2),
        languages: new Language[]
        {
            Language.English,
            Language.German,
            Language.Italian,
            Language.Spanish,
            Language.Spanish_Mexico,
            Language.French,
            Language.Polish,
            Language.Portuguese_Brazil,
            Language.Chinese,
            Language.Russian,
            Language.Japanese,
        },
        languageFormat: Strings.StringsLanguageFormat.Iso,
        hasEnabledMarkers: true,
        defaultFormVersion: 555,
        myDocumentsString: null,
        iniName: "Starfield",
        encodings: new(NonTranslated: MutagenEncoding._1252, NonLocalized: MutagenEncoding._1252));

    /// <summary> 
    /// Returns record constants related to a certain ObjectType 
    /// </summary> 
    /// <param name="type">ObjectType to query</param> 
    /// <returns>Record Constants associated with type</returns> 
    public RecordHeaderConstants Constants(ObjectType type)
    {
        return type switch
        {
            ObjectType.Subrecord => SubConstants,
            ObjectType.Record => MajorConstants,
            ObjectType.Group => GroupConstants,
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary> 
    /// Returns GameConstant readonly singleton associated with a game release  
    /// </summary> 
    /// <param name="release">Game Release to query</param> 
    /// <returns>GameConstant readonly singleton associated with mode</returns> 
    public static GameConstants Get(GameRelease release)
    {
        return release switch
        {
            GameRelease.Oblivion => Oblivion,
            GameRelease.SkyrimLE => SkyrimLE,
            GameRelease.EnderalLE => EnderalLE,
            GameRelease.SkyrimSE => SkyrimSE,
            GameRelease.SkyrimSEGog => SkyrimSEGog,
            GameRelease.EnderalSE => EnderalSE,
            GameRelease.SkyrimVR => SkyrimVR,
            GameRelease.Fallout4 => Fallout4,
            GameRelease.Fallout4VR => Fallout4VR,
            GameRelease.Starfield => Starfield,
            _ => throw new NotImplementedException()
        };
    }

    public static implicit operator GameConstants(GameRelease mode)
    {
        return Get(mode);
    }
}