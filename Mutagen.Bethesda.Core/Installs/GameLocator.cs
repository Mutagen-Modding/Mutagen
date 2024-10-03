using System.Diagnostics.CodeAnalysis;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.GOG;
using GameFinder.StoreHandlers.Steam;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using Microsoft.Win32;
using Noggog;
using FileSystem = NexusMods.Paths.FileSystem;

namespace Mutagen.Bethesda.Installs;

public sealed class GameLocator
{
    internal static readonly GameLocator Instance = new();

    private readonly Lazy<SteamHandler?> _steam;
    private readonly Lazy<GOGHandler?> _gog;

    public GameLocator()
    {
        _steam = new Lazy<SteamHandler?>(() =>
        {
            if (OperatingSystem.IsWindows())
            {
                return new SteamHandler(FileSystem.Shared, WindowsRegistry.Shared);
            }

            if (OperatingSystem.IsLinux())
            {
                return new SteamHandler(FileSystem.Shared, null);
            }

            return default;
        });
        _gog = new Lazy<GOGHandler?>(() =>
        {
            if (OperatingSystem.IsWindows())
            {
                return new GOGHandler(WindowsRegistry.Shared, FileSystem.Shared);
            }

            return default;
        });
    }

    public IEnumerable<DirectoryPath> GetAllGameDirectories(GameRelease release)
    {
        foreach (var source in Games[release].GameSources)
        {
            if (TryGetGameDirectory(source, out var path))
            {
                yield return path;
            }
        }
    }

    public bool TryGetGameDirectory(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        foreach (var source in GetAllGameDirectories(release))
        {
            path = source;
            return true;
        }

        path = default;
        return false;
    }

    public bool TryGetGameDirectory(IGameSource gameSource, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        switch (gameSource)
        {
            case RegistryGameSource registryGameSource:
                return TryGetGameDirectoryFromRegistry(registryGameSource, out path);
            case SteamGameSource steam:
                return TryGetSteamGameFolder(steam, out path);
            case GogGameSource gog:
                return TryGetGogGameFolder(gog, out path);
            default:
                throw new NotImplementedException();
        }
    }

    public DirectoryPath GetGameDirectory(GameRelease release)
    {
        if (TryGetGameDirectory(release, out var path))
        {
            return path;
        }
        throw new DirectoryNotFoundException($"Game folder for {release} cannot be found automatically");
    }

    public bool TryGetDataDirectory(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        if (TryGetGameDirectory(release, out path))
        {
            path = Path.Combine(path, "Data");
            return true;
        }
        path = default;
        return false;
    }

    public DirectoryPath GetDataDirectory(GameRelease release)
    {
        if (TryGetDataDirectory(release, out var path))
        {
            return path;
        }
        throw new DirectoryNotFoundException($"Data folder for {release} cannot be found automatically");
    }

    private bool TryGetSteamGameFolder(SteamGameSource steamGameSource, out DirectoryPath directoryPath)
    {
        if (_steam.Value != null)
        {
            var find = _steam.Value.FindOneGameById(AppId.From(steamGameSource.Id), out var err);
            if (find != null)
            {
                directoryPath = find.Path.GetFullPath();
                return true;
            }
        }

        directoryPath = default;
        return false;
    }

    private bool TryGetGogGameFolder(GogGameSource gogGameSource, out DirectoryPath directoryPath)
    {
        if (gogGameSource.Id.HasValue && _gog.Value != null)
        {
            var find = _gog.Value.FindOneGameById(
                GOGGameId.From(gogGameSource.Id.Value),
                out var err);
            if (find != null)
            {
                directoryPath = find.Path.GetFullPath();
                return true;
            }
        }

        directoryPath = default;
        return false;
    }

    private bool TryGetGameDirectoryFromRegistry(
        RegistryGameSource registryGameSource,
        [MaybeNullWhen(false)] out DirectoryPath path)
    {
        try
        {
            using var regKey = Registry.LocalMachine.OpenSubKey(registryGameSource.RegistryPath);
            if (regKey == null)
            {
                path = default;
                return false;
            }

            var regRes = RegistryHelper.GetStringValueFromRegistry(regKey, registryGameSource.RegistryKey);
            if (regRes.Failed)
            {
                path = default;
                return false;
            }

            path = regRes.Value;
            return true;
        }
        catch (Exception)
        {
            path = default;
            return false;
        }
    }

    internal static IReadOnlyDictionary<GameRelease, GameMetaData> Games { get; }

    static GameLocator()
    {
        var games = new Dictionary<GameRelease, GameMetaData>
        {
            {
                GameRelease.Oblivion, new GameMetaData(
                    GameRelease.Oblivion,
                    NexusName: "oblivion",
                    NexusGameId: 101,
                    GameSources: new IGameSource[]
                    {
                        new RegistryGameSource()
                        {
                            RegistryPath = @"SOFTWARE\WOW6432Node\Bethesda Softworks\Oblivion",
                            RegistryKey = @"installed path"
                        },
                        new SteamGameSource()
                        {
                            Id = 22330
                        },
                        new GogGameSource()
                        {
                            Id = 1458058109
                        },
                    },
                    RequiredFiles: new string[]
                    {
                        "oblivion.exe"
                    })
            },
            {
                GameRelease.SkyrimLE, new GameMetaData(
                    GameRelease.SkyrimLE,
                    NexusName: "skyrim",
                    NexusGameId: 110,
                    GameSources: new IGameSource[]
                    {
                        new RegistryGameSource()
                        {
                            RegistryPath = @"SOFTWARE\WOW6432Node\Bethesda Softworks\Skyrim",
                            RegistryKey = @"installed path"
                        },
                        new SteamGameSource()
                        {
                            Id = 72850
                        },
                    },
                    RequiredFiles: new string[]
                    {
                        "tesv.exe"
                    })
            },
            {
                GameRelease.SkyrimSE, new GameMetaData(
                    GameRelease.SkyrimSE,
                    NexusName: "skyrimspecialedition",
                    NexusGameId: 1704,
                    GameSources: new IGameSource[]
                    {
                        new RegistryGameSource()
                        {
                            RegistryPath = @"SOFTWARE\WOW6432Node\Bethesda Softworks\Skyrim Special Edition",
                            RegistryKey = @"installed path"
                        },
                        new SteamGameSource()
                        {
                            Id = 489830
                        },
                    },
                    RequiredFiles: new string[]
                    {
                        "SkyrimSE.exe"
                    })
            },
            {
                GameRelease.Fallout4, new GameMetaData(
                    GameRelease.Fallout4,
                    NexusName: "fallout4",
                    NexusGameId: 1151,
                    GameSources: new IGameSource[]
                    {
                        new RegistryGameSource()
                        {
                            RegistryPath = @"SOFTWARE\WOW6432Node\Bethesda Softworks\Fallout4",
                            RegistryKey = @"installed path",
                        },
                        new SteamGameSource()
                        {
                            Id = 377160
                        },
                    },
                    RequiredFiles: new string[]
                    {
                        "Fallout4.exe"
                    })
            },
            {
                GameRelease.Fallout4VR, new GameMetaData(
                    GameRelease.Fallout4VR,
                    NexusName: "fallout4",
                    NexusGameId: 1151,
                    GameSources: new IGameSource[]
                    {
                        new RegistryGameSource()
                        {
                            RegistryPath = @"SOFTWARE\WOW6432Node\Bethesda Softworks\Fallout 4 VR",
                            RegistryKey = @"installed path",
                        },
                        new SteamGameSource()
                        {
                            Id = 611660
                        },
                    },
                    RequiredFiles: new string[]
                    {
                        "Fallout4VR.exe"
                    })
            },
            {
                GameRelease.SkyrimVR, new GameMetaData(
                    GameRelease.SkyrimVR,
                    NexusName: "skyrimspecialedition",
                    NexusGameId: 1704,
                    GameSources: new IGameSource[]
                    {
                        new RegistryGameSource()
                        {
                            RegistryPath = @"SOFTWARE\WOW6432Node\Bethesda Softworks\Skyrim VR",
                            RegistryKey = @"installed path",
                        },
                        new SteamGameSource()
                        {
                            Id = 611670
                        },
                    },
                    RequiredFiles: new string[]
                    {
                        "SkyrimVR.exe"
                    })
            },
            {
            GameRelease.Starfield, new GameMetaData(
                GameRelease.Starfield,
                NexusName: "starfield",
                NexusGameId: 4187,
                GameSources: new IGameSource[]
                {
                    new SteamGameSource()
                    {
                        Id = 1716740
                    },
                },
                RequiredFiles: new string[]
                {
                    "Starfield.exe"
                })
        },
        };
        games[GameRelease.EnderalLE] = games[GameRelease.SkyrimLE] with
        {
            GameSources = new IGameSource[]
            {
                new SteamGameSource()
                {
                    Id = 72850
                },
            },
        };
        games[GameRelease.EnderalSE] = games[GameRelease.SkyrimSE] with
        {
            GameSources = new IGameSource[]
            {
                new RegistryGameSource()
                {
                    RegistryPath = @"SOFTWARE\WOW6432Node\SureAI\Enderal SE",
                    RegistryKey = @"installed path",
                },
                new SteamGameSource()
                {
                    Id = 489830
                },
            },
        };
        games[GameRelease.SkyrimSEGog] = games[GameRelease.SkyrimSE] with
        {
            GameSources = new IGameSource[]
            {
                new GogGameSource()
                {
                    Id = 1711230643
                },
            },
        };
        Games = games;
    }
}