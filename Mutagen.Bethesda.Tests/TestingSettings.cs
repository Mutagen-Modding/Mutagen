using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
#nullable enable

namespace Mutagen.Bethesda.Tests
{
    public class TestingSettings
    {
        public bool TestGroupMasks { get; set; }
        public bool TestFlattenedMod { get; set; }
        public bool TestBenchmarks { get; set; }
        public bool TestRecordEnumerables { get; set; }
        public bool TestEquality { get; set; }
        public DataFolderLocations DataFolderLocations { get; set; } = new DataFolderLocations();
        public PassthroughSettings PassthroughSettings { get; set; } = new PassthroughSettings();
        public List<TargetGroup> TargetGroups { get; set; } = new List<TargetGroup>();
    }

    public class DataFolderLocations
    {
        public string Oblivion { get; set; } = string.Empty;
        public string Skyrim { get; set; } = string.Empty;
        public string SkyrimSpecialEdition { get; set; } = string.Empty;
        public string SkyrimVR { get; set; } = string.Empty;

        public string Get(GameRelease mode)
        {
            switch (mode)
            {
                case GameRelease.Oblivion:
                    return Oblivion;
                case GameRelease.SkyrimLE:
                    return Skyrim;
                case GameRelease.SkyrimSE:
                    return SkyrimSpecialEdition;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class PassthroughSettings
    {
        public CacheReuse CacheReuse { get; set; } = new CacheReuse();
        public bool DeleteCachesAfter { get; set; } = true;
        public bool TestNormal { get; set; }
        public bool TestBinaryOverlay { get; set; }
        public bool TestImport { get; set; }
        public bool TestFolder { get; set; }
        public bool TestCopyIn { get; set; }
        public bool Parallel { get; set; }

        public bool HasAnyToRun => TestNormal
            || TestBinaryOverlay
            || TestFolder
            || TestCopyIn;
    }

    public class CacheReuse
    {
        public bool ReuseDecompression { get; set; }
        public bool ReuseAlignment { get; set; }
        public bool ReuseProcessing { get; set; }
    }

    public class TargetGroup
    {
        public bool Do { get; set; }
        public string NicknameSuffix { get; set; } = string.Empty;
        public GameRelease GameRelease { get; set; }
        public List<Target> Targets { get; set; } = new List<Target>();
    }

    public class Target
    {
        public bool Do { get; set; }
        public string Path { get; set; } = string.Empty;
        public byte NumMasters { get; set; }
        public byte? ExpectedBaseGroupCount { get; set; }
    }
}
