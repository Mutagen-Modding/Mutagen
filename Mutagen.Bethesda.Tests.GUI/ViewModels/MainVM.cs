using DynamicData;
using DynamicData.Binding;
using Newtonsoft.Json;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Tests.GUI;

[JsonObject(MemberSerialization.OptIn)]
public class MainVM : ViewModel
{
    [JsonProperty]
    public PathPickerVM SelectedConfigPath { get; } = new()
    {
        PathType = PathPickerVM.PathTypeOptions.File,
        ExistCheckOption = PathPickerVM.CheckOptions.On,
    };

    private readonly ObservableAsPropertyHelper<TestingSettings?> _selectedSettings;
    public TestingSettings? SelectedSettings => _selectedSettings.Value;

    [Reactive]
    public bool TestNormal { get; set; }

    [Reactive]
    public bool TestOverlay { get; set; }

    [Reactive]
    public bool TestImport { get; set; }

    [Reactive]
    public bool TestCopyIn { get; set; }

    [Reactive]
    public bool TestEquals { get; set; }

    [Reactive]
    public bool TestParallel { get; set; }

    [Reactive]
    public bool TestPex { get; set; }

    [Reactive]
    public bool CacheReuseAny { get; set; }

    [Reactive]
    public bool CacheMerging { get; set; }

    [Reactive]
    public bool CacheTrimming { get; set; }

    [Reactive]
    public bool CacheAlignment { get; set; }

    [Reactive]
    public bool CacheDecompression { get; set; }

    [Reactive]
    public bool CacheProcessing { get; set; }

    private readonly ObservableAsPropertyHelper<bool> _validTarget;
    public bool ValidTarget => _validTarget.Value;

    public ObservableCollectionExtended<PassthroughGroupVM> Groups { get; } = new();

    public ObservableCollectionExtended<RecordTypeVm> SkippedRecordTypes { get; } = new();
    public ObservableCollectionExtended<RecordTypeVm> InterestingRecordTypes { get; } = new();
    public ICommand AddSkipCommand { get; }
    public ICommand AddIncludeCommand { get; }

    [Reactive] 
    public string SkipInput { get; set; } = string.Empty;

    public SourceCache<DataFolderVM, GameRelease> DataFolders { get; } = new(x => x.GameRelease);

    public IObservableCollection<DataFolderVM> DataFoldersDisplay { get; } = new ObservableCollectionExtended<DataFolderVM>();

    public ReactiveCommand<Unit, Unit> RunAllCommand { get; }

    public ReactiveCommand<Unit, Unit> AddPassthroughGroupCommand { get; }

    [Reactive]
    public RunningTestsVM? RunningTests { get; private set; }
    
    [Reactive]
    public bool TrimmingEnabled { get; set; }

    public MainVM()
    {
        // Set up selected config swapping and loading
        _selectedSettings = this.WhenAnyValue(x => x.SelectedConfigPath.TargetPath)
            .Skip(1)
            .Select(x =>
            {
                TestingSettings? settings = null;
                try
                {
                    if (File.Exists(x))
                    {
                        settings = JsonConvert.DeserializeObject<TestingSettings>(File.ReadAllText(x));
                    }
                }
                catch (Exception)
                {
                    // ToDo
                    // Log
                }
                return (Path: x, Settings: settings);
            })
            .Select(p =>
            {
                if (p.Settings != null && !p.Path.IsNullOrWhitespace())
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(p.Path)!);
                    File.WriteAllText(p.Path, JsonConvert.SerializeObject(p.Settings, Formatting.Indented));
                }
                ReadInSettings(p.Settings ?? new TestingSettings());
                return p.Settings;
            })
            .ToGuiProperty(this, nameof(SelectedSettings), default);

        // Set up additional file picker error to fire if settings couldn't parse
        SelectedConfigPath.AdditionalError = this.WhenAnyValue(x => x.SelectedSettings)
            .Select<TestingSettings?, ErrorResponse>(settings =>
            {
                if (settings == null) return ErrorResponse.Fail("Settings could not be serialized.");
                return ErrorResponse.Success;
            });

        // Funnel into convenient valid boolean for GUI use
        _validTarget = this.WhenAnyValue(x => x.SelectedConfigPath.ErrorState)
            .Select(err => err.Succeeded)
            .ToGuiProperty(this, nameof(ValidTarget));

        RunAllCommand = ReactiveCommand.CreateFromTask(Run);

        AddPassthroughGroupCommand = ReactiveCommand.Create(AddPassthroughGroup);

        DataFolders.AddOrUpdate(Enums<GameRelease>.Values
            .Select(r => new DataFolderVM(r)));
        DataFolders.Connect()
            .Bind(DataFoldersDisplay)
            .Subscribe()
            .DisposeWith(this);

        AddSkipCommand = ReactiveCommand.Create(
            () =>
            {
                SkippedRecordTypes.Add(new RecordTypeVm(this, new RecordType(SkipInput)));
                SkipInput = string.Empty;
            },
            this.WhenAnyValue(x => x.SkipInput)
                .Select(x => x.Length == 4));

        AddIncludeCommand = ReactiveCommand.Create(
            () =>
            {
                InterestingRecordTypes.Add(new RecordTypeVm(this, new RecordType(SkipInput)));
                SkipInput = string.Empty;
            },
            this.WhenAnyValue(x => x.SkipInput)
                .Select(x => x.Length == 4));
    }

    public override void Dispose()
    {
        base.Dispose();
        if (SelectedSettings != null)
        {
            SaveToSettings(SelectedSettings);
            SaveSettings(SelectedConfigPath.TargetPath, SelectedSettings);
        }
    }
    private static void SaveSettings(string path, TestingSettings settings)
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(settings, Formatting.Indented));
    }

    public void AddPassthroughGroup()
    {
        HashSet<GameRelease> games = new HashSet<GameRelease>()
        {
            Enums<GameRelease>.Values
        };
        foreach (var group in Groups)
        {
            games.Remove(group.GameRelease);
        }
        GameRelease release = GameRelease.SkyrimSE;
        if (games.Count > 0)
        {
            release = games.First();
        }
        var groupVM = new PassthroughGroupVM(this)
        {
            GameRelease = release,
            Do = true,
        };
        groupVM.Passthroughs.Add(new PassthroughVM(groupVM));
        Groups.Add(groupVM);
    }

    public void FreshInitialize()
    {
        var gameReleases = Enum.GetValues<GameRelease>();
        var locator = new GameLocatorLookupCache();

        // Data folder locations
        var dataFolderLocations = new DataFolderLocations();
        foreach (var release in gameReleases)
        {
            try
            {
                var dataDir = locator.GetDataDirectory(release);
                dataFolderLocations.Set(release, dataDir);
            }
            catch (NotImplementedException)
            {
                continue;
            }
            catch (DirectoryNotFoundException)
            {
                continue;
            }
        }

        // Target groups
        var targetGroups = gameReleases
            .Where(x => !dataFolderLocations.Get(x).IsNullOrWhitespace())
            .Select(release => new TargetGroup
            {
                GameRelease = release,
                NicknameSuffix = string.Empty,
                Do = true,
                Targets = Implicits.Get(release).BaseMasters
                    .Select(implicitMods =>
                    {
                        try
                        {
                            return new Target
                            {
                                Do = true,
                                Path = Path.Combine(locator.GetDataDirectory(release), implicitMods.FileName)
                            };
                        }
                        catch (DirectoryNotFoundException)
                        {
                            return null;
                        }
                    })
                    .WhereNotNull()
                    .ToList()
            })
            .ToList();

        var settings = new TestingSettings
        {
            PassthroughSettings = new PassthroughSettings
            {
                CacheReuse = new CacheReuse(true),
                TestNormal = true,
                TestBinaryOverlay = true,
                DeleteCachesAfter = false,
                TestImport = false,
                ParallelModTranslations = false,
                TestCopyIn = false,
                Trimming = new TrimmingSettings()
                {
                    Enabled = false
                }
            },
            TargetGroups = targetGroups,
            TestFlattenedMod = false,
            TestBenchmarks = false,
            TestEquality = false,
            TestPex = false,
            TestGroupMasks = false,
            TestRecordEnumerables = false,
            DataFolderLocations = dataFolderLocations
        };
        
        var mutagenFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()))));
        if (mutagenFolder is not null)
        {
            var testPath = Path.Combine(mutagenFolder, "Mutagen.Bethesda.Tests", "TestingSettings.json");
            SaveSettings(testPath, settings);
            SelectedConfigPath.TargetPath = testPath;
        }
    }

    public void ReadInSettings(TestingSettings settings)
    {
        this.TestNormal = settings.PassthroughSettings.TestNormal;
        this.TestCopyIn = settings.PassthroughSettings.TestCopyIn;
        this.TestImport = settings.PassthroughSettings.TestImport;
        this.TestOverlay = settings.PassthroughSettings.TestBinaryOverlay;
        this.TestParallel = settings.PassthroughSettings.ParallelModTranslations;
        this.TestEquals = settings.TestEquality;
        this.TestPex = settings.TestPex;

        this.CacheTrimming = settings.PassthroughSettings.CacheReuse.ReuseTrimming;
        this.CacheMerging = settings.PassthroughSettings.CacheReuse.ReuseMerge;
        this.CacheAlignment = settings.PassthroughSettings.CacheReuse.ReuseAlignment;
        this.CacheDecompression = settings.PassthroughSettings.CacheReuse.ReuseDecompression;
        this.CacheProcessing = settings.PassthroughSettings.CacheReuse.ReuseProcessing;

        TrimmingEnabled = settings.PassthroughSettings.Trimming.Enabled;
        this.SkippedRecordTypes.Clear();
        this.SkippedRecordTypes.SetTo(settings.PassthroughSettings.Trimming.TypesToTrim.Select(x => new RecordTypeVm(this, x)));
        this.InterestingRecordTypes.Clear();
        this.InterestingRecordTypes.SetTo(settings.PassthroughSettings.Trimming.TypesToInclude.Select(x => new RecordTypeVm(this, x)));

        this.Groups.Clear();
        this.Groups.AddRange(settings.TargetGroups
            .Select(g => new PassthroughGroupVM(this, g)));

        if (DataFolders.TryGetValue(GameRelease.Oblivion, out var df))
        {
            df.DataFolder.TargetPath = settings.DataFolderLocations.Oblivion;
        }

        if (DataFolders.TryGetValue(GameRelease.OblivionRE, out df))
        {
            df.DataFolder.TargetPath = settings.DataFolderLocations.OblivionRE;
        }

        if (DataFolders.TryGetValue(GameRelease.SkyrimLE, out df))
        {
            df.DataFolder.TargetPath = settings.DataFolderLocations.Skyrim;
        }

        if (DataFolders.TryGetValue(GameRelease.SkyrimSE, out df))
        {
            df.DataFolder.TargetPath = settings.DataFolderLocations.SkyrimSpecialEdition;
        }

        if (DataFolders.TryGetValue(GameRelease.SkyrimVR, out df))
        {
            df.DataFolder.TargetPath = settings.DataFolderLocations.SkyrimVR;
        }

        if (DataFolders.TryGetValue(GameRelease.Fallout4, out df))
        {
            df.DataFolder.TargetPath = settings.DataFolderLocations.Fallout4;
        }

        if (DataFolders.TryGetValue(GameRelease.Starfield, out df))
        {
            df.DataFolder.TargetPath = settings.DataFolderLocations.Starfield;
        }

    }

    public void SaveToSettings(TestingSettings settings)
    {
        settings.PassthroughSettings.TestNormal = this.TestNormal;
        settings.PassthroughSettings.TestImport = this.TestImport;
        settings.PassthroughSettings.TestBinaryOverlay = this.TestOverlay;
        settings.PassthroughSettings.TestCopyIn = this.TestCopyIn;
        settings.PassthroughSettings.ParallelModTranslations = this.TestParallel;
        settings.TestEquality = this.TestEquals;
        settings.TestPex = this.TestPex;

        settings.PassthroughSettings.CacheReuse.ReuseDecompression = this.CacheDecompression;
        settings.PassthroughSettings.CacheReuse.ReuseAlignment = this.CacheAlignment;
        settings.PassthroughSettings.CacheReuse.ReuseProcessing = this.CacheProcessing;
        settings.PassthroughSettings.CacheReuse.ReuseTrimming = this.CacheTrimming;
        settings.PassthroughSettings.CacheReuse.ReuseMerge = this.CacheMerging;

        settings.TargetGroups = Groups
            .Select(g => new TargetGroup()
            {
                Do = g.Do,
                GameRelease = g.GameRelease,
                NicknameSuffix = g.NicknameSuffix,
                Targets = g.Passthroughs
                    .Select(p =>
                    {
                        return new Target()
                        {
                            Do = p.Do,
                            Path = p.Path.TargetPath
                        };
                    })
                    .ToList(),
            })
            .ToList();
        settings.PassthroughSettings.Trimming.Enabled = TrimmingEnabled;
        settings.PassthroughSettings.Trimming.TypesToTrim = SkippedRecordTypes.Select(x => x.RecordType.Type).ToList();
        settings.PassthroughSettings.Trimming.TypesToInclude = InterestingRecordTypes.Select(x => x.RecordType.Type).ToList();

        settings.DataFolderLocations.Oblivion = DataFolders.Get(GameRelease.Oblivion).DataFolder.TargetPath;
        settings.DataFolderLocations.OblivionRE = DataFolders.Get(GameRelease.OblivionRE).DataFolder.TargetPath;
        settings.DataFolderLocations.Skyrim = DataFolders.Get(GameRelease.SkyrimLE).DataFolder.TargetPath;
        settings.DataFolderLocations.SkyrimSpecialEdition = DataFolders.Get(GameRelease.SkyrimSE).DataFolder.TargetPath;
        settings.DataFolderLocations.SkyrimVR = DataFolders.Get(GameRelease.SkyrimVR).DataFolder.TargetPath;
        settings.DataFolderLocations.Fallout4 = DataFolders.Get(GameRelease.Fallout3).DataFolder.TargetPath;
        settings.DataFolderLocations.Fallout3 = DataFolders.Get(GameRelease.FalloutNV).DataFolder.TargetPath;
        settings.DataFolderLocations.FalloutNV = DataFolders.Get(GameRelease.Fallout4).DataFolder.TargetPath;
        settings.DataFolderLocations.Starfield = DataFolders.Get(GameRelease.Starfield).DataFolder.TargetPath;
    }

    public PassthroughSettings GetPassthroughSettings()
    {
        return new PassthroughSettings()
        {
            DeleteCachesAfter = false,
            CacheReuse = new CacheReuse()
            {
                ReuseAlignment = CacheAlignment,
                ReuseDecompression = CacheDecompression,
                ReuseProcessing = CacheProcessing,
                ReuseTrimming = CacheTrimming,
                ReuseMerge = CacheMerging
            },
            TestBinaryOverlay = TestOverlay,
            TestCopyIn = TestCopyIn,
            TestImport = TestImport,
            TestNormal = TestNormal,
            ParallelModTranslations = TestParallel,
            Trimming = new TrimmingSettings()
            {
                TypesToTrim = SkippedRecordTypes.Select(x => x.RecordType.Type).ToList(),
                TypesToInclude = InterestingRecordTypes.Select(x => x.RecordType.Type).ToList(),
                Enabled = TrimmingEnabled
            }
        };
    }

    public async Task Run()
    {
        RunningTests = new RunningTestsVM();
        try
        {
            await RunningTests.Run(this);
        }
        catch (Exception ex)
        {
            RunningTests.Error = ex;
            // ToDo
            // Add display
            throw;
        }
    }
}