using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Binary;
using Newtonsoft.Json;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests.GUI
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MainVM : ViewModel
    {
        [JsonProperty]
        public PathPickerVM SelectedConfigPath { get; } = new PathPickerVM()
        {
            PathType = PathPickerVM.PathTypeOptions.File,
            ExistCheckOption = PathPickerVM.CheckOptions.On,
        };

        private readonly ObservableAsPropertyHelper<TestingSettings?> _SelectedSettings;
        public TestingSettings? SelectedSettings => _SelectedSettings.Value;

        [Reactive]
        public bool TestNormal { get; set; }

        [Reactive]
        public bool TestOverlay { get; set; }

        [Reactive]
        public bool TestImport { get; set; }

        [Reactive]
        public bool TestCopyIn { get; set; }

        [Reactive]
        public bool TestEquals{ get; set; }

        [Reactive]
        public bool CacheReuseAny { get; set; }

        [Reactive]
        public bool CacheAlignment { get; set; }

        [Reactive]
        public bool CacheDecompression { get; set; }

        [Reactive]
        public bool CacheProcessing { get; set; }

        private readonly ObservableAsPropertyHelper<bool> _ValidTarget;
        public bool ValidTarget => _ValidTarget.Value;

        public ObservableCollectionExtended<PassthroughGroupVM> Groups { get; } = new ObservableCollectionExtended<PassthroughGroupVM>();

        public SourceCache<DataFolderVM, GameRelease> DataFolders { get; } = new SourceCache<DataFolderVM, GameRelease>(x => x.GameRelease);

        public IObservableCollection<DataFolderVM> DataFoldersDisplay { get; } = new ObservableCollectionExtended<DataFolderVM>();

        public ReactiveCommand<Unit, Unit> RunAllCommand { get; }

        public ReactiveCommand<Unit, Unit> AddPassthroughGroupCommand { get; }

        [Reactive]
        public RunningTestsVM? RunningTests { get; private set; }

        public MainVM()
        {
            // Set up selected config swapping and loading
            _SelectedSettings = this.WhenAnyValue(x => x.SelectedConfigPath.TargetPath)
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
            _ValidTarget = this.WhenAnyValue(x => x.SelectedConfigPath.ErrorState)
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
            this.TestEquals = settings.TestEquality;

            this.CacheAlignment = settings.PassthroughSettings.CacheReuse.ReuseAlignment;
            this.CacheDecompression = settings.PassthroughSettings.CacheReuse.ReuseDecompression;
            this.CacheProcessing = settings.PassthroughSettings.CacheReuse.ReuseProcessing;

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
        }

        public void SaveToSettings(TestingSettings settings)
        {
            settings.PassthroughSettings.TestNormal = this.TestNormal;
            settings.PassthroughSettings.TestImport = this.TestImport;
            settings.PassthroughSettings.TestBinaryOverlay = this.TestOverlay;
            settings.PassthroughSettings.TestCopyIn = this.TestCopyIn;
            settings.TestEquality = this.TestEquals;

            settings.PassthroughSettings.CacheReuse.ReuseDecompression = this.CacheDecompression;
            settings.PassthroughSettings.CacheReuse.ReuseAlignment = this.CacheAlignment;
            settings.PassthroughSettings.CacheReuse.ReuseProcessing = this.CacheProcessing;

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

            settings.DataFolderLocations.Oblivion = DataFolders.Get(GameRelease.Oblivion).DataFolder.TargetPath;
            settings.DataFolderLocations.Skyrim = DataFolders.Get(GameRelease.SkyrimLE).DataFolder.TargetPath;
            settings.DataFolderLocations.SkyrimSpecialEdition = DataFolders.Get(GameRelease.SkyrimSE).DataFolder.TargetPath;
            settings.DataFolderLocations.SkyrimVR = DataFolders.Get(GameRelease.SkyrimVR).DataFolder.TargetPath;
        }

        public PassthroughSettings GetPassthroughSettings()
        {
            return new PassthroughSettings()
            {
                DeleteCachesAfter = false,
                Parallel = true,
                CacheReuse = new CacheReuse()
                {
                    ReuseAlignment = CacheAlignment,
                    ReuseDecompression = CacheDecompression,
                    ReuseProcessing = CacheProcessing,
                },
                TestBinaryOverlay = TestOverlay,
                TestCopyIn = TestCopyIn,
                TestFolder = false,
                TestImport = TestImport,
                TestNormal = TestNormal,
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
}
