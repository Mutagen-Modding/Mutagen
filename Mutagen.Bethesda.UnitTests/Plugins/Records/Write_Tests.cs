using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.Utility;
using System;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public class Write_Tests
    {
        public static readonly ModKey WriteKey = new ModKey("Write", ModType.Plugin);
        public static readonly ModKey BadWriteKey = new ModKey("BadWrite", ModType.Plugin);

        public static TempFile GetFile()
        {
            return new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath, suffix: ".esp");
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
                    ModKey = ModKeyOption.NoCheck,
                    MastersListContent = MastersListContentOption.NoCheck
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
                    ModKey = ModKeyOption.NoCheck,
                    MastersListContent = MastersListContentOption.NoCheck
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
                        ModKey = ModKeyOption.ThrowIfMisaligned,
                        MastersListContent = MastersListContentOption.NoCheck,
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
                        ModKey = ModKeyOption.ThrowIfMisaligned,
                        MastersListContent = MastersListContentOption.NoCheck,
                    }));
        }

        [Fact]
        public void ParallelWrite_MasterListSync_Throw()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            mod.Weapons.RecordCache.Set(
                new Weapon(FormKey.Factory("012345:Skyrim.esm"), SkyrimRelease.SkyrimLE));
            Assert.Throws<AggregateException>(
                () => mod.WriteToBinaryParallel(
                    tmp.File.Path,
                    new BinaryWriteParameters()
                    {
                        ModKey = ModKeyOption.NoCheck,
                        MastersListContent = MastersListContentOption.NoCheck,
                    }));
        }

        [Fact]
        public void Write_MasterListSync_Throw()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            mod.Weapons.RecordCache.Set(
                new Weapon(FormKey.Factory("012345:Skyrim.esm"), SkyrimRelease.SkyrimLE));
            Assert.Throws<RecordException>(
                () =>
                {
                    try
                    {
                        mod.WriteToBinary(
                            tmp.File.Path,
                            new BinaryWriteParameters()
                            {
                                ModKey = ModKeyOption.NoCheck,
                                MastersListContent = MastersListContentOption.NoCheck,
                            });
                    }
                    catch (RecordException ex)
                    {
                        Assert.IsType<ArgumentException>(ex.InnerException);
                        throw;
                    }
                });
        }

        [Fact]
        public void ParallelWrite_MasterListSync()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            mod.Weapons.RecordCache.Set(
                new Weapon(FormKey.Factory("012345:Skyrim.esm"), SkyrimRelease.SkyrimLE));
            mod.WriteToBinaryParallel(
                tmp.File.Path,
                new BinaryWriteParameters()
                {
                    ModKey = ModKeyOption.NoCheck,
                    MastersListContent = MastersListContentOption.Iterate,
                });
        }

        [Fact]
        public void Write_MasterListSync()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            mod.Weapons.RecordCache.Set(
                new Weapon(FormKey.Factory("012345:Skyrim.esm"), SkyrimRelease.SkyrimLE));
            mod.WriteToBinary(
                tmp.File.Path,
                new BinaryWriteParameters()
                {
                    ModKey = ModKeyOption.NoCheck,
                    MastersListContent = MastersListContentOption.Iterate,
                });
        }

        [Fact]
        public void Write_ModNotOnLoadOrder()
        {
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            mod.Weapons.RecordCache.Set(
                new Weapon(FormKey.Factory("012345:Skyrim.esm"), SkyrimRelease.SkyrimLE));
            mod.Weapons.RecordCache.Set(
                new Weapon(FormKey.Factory("012345:SomeOtherMod.esp"), SkyrimRelease.SkyrimLE));
            Assert.Throws<MissingModException>(() =>
            {
                mod.WriteToBinary(
                   tmp.File,
                   new BinaryWriteParameters()
                   {
                       ModKey = ModKeyOption.NoCheck,
                       MastersListContent = MastersListContentOption.Iterate,
                       MastersListOrdering = new MastersListOrderingByLoadOrder(
                           Constants.Skyrim.AsEnumerable())
                   });
            });
        }

        [Fact]
        public void WriteWithCounterLists()
        {
            var param = new BinaryWriteParameters()
            {
                ModKey = ModKeyOption.NoCheck,
                MastersListContent = MastersListContentOption.Iterate,
            };
            using var tmp = GetFile();
            var mod = new SkyrimMod(WriteKey, SkyrimRelease.SkyrimLE);
            var armor = mod.Armors.AddNew();
            mod.WriteToBinaryParallel(tmp.File.Path, param);
            armor.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
            mod.WriteToBinaryParallel(tmp.File.Path, param);
            armor.Keywords.Add(FormKey.Null);
            mod.WriteToBinaryParallel(tmp.File.Path, param);
            for (int i = 0; i < 20000; i++)
            {
                armor.Keywords.Add(FormKey.Null);
            }
            Assert.Throws<AggregateException>(() =>
            {
                mod.WriteToBinaryParallel(tmp.File.Path, param);
            });
        }
    }
}
