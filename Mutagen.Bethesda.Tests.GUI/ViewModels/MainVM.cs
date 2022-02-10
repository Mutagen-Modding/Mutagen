using DynamicData;
using DynamicData.Binding;
using Newtonsoft.Json;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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
    public ICommand AddSkipCommand { get; }

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
            .Skip(1)
            .Pairwise()
            .Select(p =>
            {
                if (p.Previous.Settings != null)
                {
                    SaveToSettings(p.Previous.Settings);
                    File.WriteAllText(p.Previous.Path, JsonConvert.SerializeObject(p.Previous.Settings, Formatting.Indented));
                }
                ReadInSettings(p.Current.Settings ?? new TestingSettings());
                return p.Current.Settings;
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

        DataFolders.AddOrUpdate(EnumExt.GetValues<GameRelease>()
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
    }

    public override void Dispose()
    {
        base.Dispose();
        if (SelectedSettings != null)
        {
            SaveToSettings(SelectedSettings);
            File.WriteAllText(
                SelectedConfigPath.TargetPath,
                JsonConvert.SerializeObject(SelectedSettings, Formatting.Indented));
        }
    }

    public void AddPassthroughGroup()
    {
        HashSet<GameRelease> games = new HashSet<GameRelease>()
        {
            EnumExt.GetValues<GameRelease>()
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

    public void ReadInSettings(TestingSettings settings)
    {
        this.TestNormal = settings.PassthroughSettings.TestNormal;
        this.TestCopyIn = settings.PassthroughSettings.TestCopyIn;
        this.TestImport = settings.PassthroughSettings.TestImport;
        this.TestOverlay = settings.PassthroughSettings.TestBinaryOverlay;
        this.TestParallel = settings.PassthroughSettings.ParallelWriting;
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

        this.Groups.Clear();
        this.Groups.AddRange(settings.TargetGroups
            .Select(g => new PassthroughGroupVM(this, g)));

        if (DataFolders.TryGetValue(GameRelease.Oblivion, out var df))
        {
            df.DataFolder.TargetPath = settings.DataFolderLocations.Oblivion;
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

    }

    public void SaveToSettings(TestingSettings settings)
    {
        settings.PassthroughSettings.TestNormal = this.TestNormal;
        settings.PassthroughSettings.TestImport = this.TestImport;
        settings.PassthroughSettings.TestBinaryOverlay = this.TestOverlay;
        settings.PassthroughSettings.TestCopyIn = this.TestCopyIn;
        settings.PassthroughSettings.ParallelWriting = this.TestParallel;
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

        settings.DataFolderLocations.Oblivion = DataFolders.Get(GameRelease.Oblivion).DataFolder.TargetPath;
        settings.DataFolderLocations.Skyrim = DataFolders.Get(GameRelease.SkyrimLE).DataFolder.TargetPath;
        settings.DataFolderLocations.SkyrimSpecialEdition = DataFolders.Get(GameRelease.SkyrimSE).DataFolder.TargetPath;
        settings.DataFolderLocations.SkyrimVR = DataFolders.Get(GameRelease.SkyrimVR).DataFolder.TargetPath;
        settings.DataFolderLocations.Fallout4 = DataFolders.Get(GameRelease.Fallout4).DataFolder.TargetPath;
    }

    public PassthroughSettings GetPassthroughSettings()
    {
        return new PassthroughSettings()
        {
            DeleteCachesAfter = false,
            ParallelProccessingSteps = true,
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
            TestFolder = false,
            TestImport = TestImport,
            TestNormal = TestNormal,
            ParallelWriting = TestParallel,
            Trimming = new TrimmingSettings()
            {
                TypesToTrim = SkippedRecordTypes.Select(x => x.RecordType.Type).ToList(),
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