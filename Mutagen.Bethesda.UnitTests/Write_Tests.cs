using Mutagen.Bethesda.Skyrim;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Write_Tests
    {
        public static readonly ModKey WriteKey = new ModKey("Write", false);
        public static readonly ModKey BadWriteKey = new ModKey("BadWrite", false);

        public static TempFile GetFile()
        {
            return new TempFile(extraDirectoryPaths: Utility.TempFolderPath, suffix: ".esp");
        }

        [Fact]
        public void BasicWrite()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            var weap = mod.Weapons.AddNew();
            mod.WriteToBinary(
                tmp.File.Path,
                new BinaryWriteParameters()
                {
                    ModKeySync = BinaryWriteParameters.ModKeySyncOption.NoCheck,
                    MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck
                });
        }

        [Fact]
        public void BasicParallelWrite()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            var weap = mod.Weapons.AddNew();
            mod.WriteToBinaryParallel(
                tmp.File.Path,
                new BinaryWriteParameters()
                {
                    ModKeySync = BinaryWriteParameters.ModKeySyncOption.NoCheck,
                    MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck
                });
        }

        [Fact]
        public void ParallelWrite_MasterFlagSync_Throw()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(BadWriteKey, SkyrimRelease.SkyrimLE);
            var weap = mod.Weapons.AddNew();
            Assert.Throws<ArgumentException>(
                () => mod.WriteToBinaryParallel(
                    tmp.File.Path,
                    new BinaryWriteParameters()
                    {
                        ModKeySync = BinaryWriteParameters.ModKeySyncOption.ThrowIfMisaligned,
                        MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
                    }));
        }

        [Fact]
        public void Write_MasterFlagSync_Throw()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(BadWriteKey, SkyrimRelease.SkyrimLE);
            var weap = mod.Weapons.AddNew();
            Assert.Throws<ArgumentException>(
                () => mod.WriteToBinaryParallel(
                    tmp.File.Path,
                    new BinaryWriteParameters()
                    {
                        ModKeySync = BinaryWriteParameters.ModKeySyncOption.ThrowIfMisaligned,
                        MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
                    }));
        }

        [Fact]
        public void ParallelWrite_MasterListSync_Throw()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            mod.Weapons.RecordCache.Set(
                new Weapon(FormKey.Factory("012345:Skyrim.esm")));
            Assert.Throws<AggregateException>(
                () => mod.WriteToBinaryParallel(
                    tmp.File.Path,
                    new BinaryWriteParameters()
                    {
                        ModKeySync = BinaryWriteParameters.ModKeySyncOption.NoCheck,
                        MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
                    }));
        }

        [Fact]
        public void Write_MasterListSync_Throw()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            mod.Weapons.RecordCache.Set(
                new Weapon(FormKey.Factory("012345:Skyrim.esm")));
            Assert.Throws<ArgumentException>(
                () => mod.WriteToBinary(
                    tmp.File.Path,
                    new BinaryWriteParameters()
                    {
                        ModKeySync = BinaryWriteParameters.ModKeySyncOption.NoCheck,
                        MastersListSync = BinaryWriteParameters.MastersListSyncOption.NoCheck,
                    }));
        }

        [Fact]
        public void ParallelWrite_MasterListSync()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            mod.Weapons.RecordCache.Set(
                new Weapon(FormKey.Factory("012345:Skyrim.esm")));
            mod.WriteToBinaryParallel(
                tmp.File.Path,
                new BinaryWriteParameters()
                {
                    ModKeySync = BinaryWriteParameters.ModKeySyncOption.NoCheck,
                    MastersListSync = BinaryWriteParameters.MastersListSyncOption.Iterate,
                });
        }

        [Fact]
        public void Write_MasterListSync()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            mod.Weapons.RecordCache.Set(
                new Weapon(FormKey.Factory("012345:Skyrim.esm")));
            mod.WriteToBinary(
                tmp.File.Path,
                new BinaryWriteParameters()
                {
                    ModKeySync = BinaryWriteParameters.ModKeySyncOption.NoCheck,
                    MastersListSync = BinaryWriteParameters.MastersListSyncOption.Iterate,
                });
        }
    }
}
