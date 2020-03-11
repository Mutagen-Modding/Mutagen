using Mutagen.Bethesda.Oblivion;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class MasterSync_Tests
    {
        [Fact]
        public void MasterFlagSync_Correct()
        {
            using var folder = new TempFolder();
            var masterMod = new OblivionMod(new ModKey("Test", master: true));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esm");
            masterMod.WriteToBinary(masterPath,
                new BinaryWriteParameters()
                {
                    MasterFlagMatch = BinaryWriteParameters.MasterFlagMatchOption.ThrowIfMisaligned
                });
            var childMod = new OblivionMod(new ModKey("Test", master: false));
            var childPath = Path.Combine(folder.Dir.Path, "Test.esp");
            childMod.WriteToBinary(childPath,
                new BinaryWriteParameters()
                {
                    MasterFlagMatch = BinaryWriteParameters.MasterFlagMatchOption.ThrowIfMisaligned
                });
        }

        [Fact]
        public void MasterFlagSync_MasterThrow()
        {
            using var folder = new TempFolder();
            var masterMod = new OblivionMod(new ModKey("Test", master: true));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esp");
            Assert.Throws<ArgumentException>(() =>
            {
                masterMod.WriteToBinary(masterPath,
                    new BinaryWriteParameters()
                    {
                        MasterFlagMatch = BinaryWriteParameters.MasterFlagMatchOption.ThrowIfMisaligned
                    });
            });
        }

        [Fact]
        public void MasterFlagSync_ChildThrow()
        {
            using var folder = new TempFolder();
            var masterMod = new OblivionMod(new ModKey("Test", master: false));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esm");
            Assert.Throws<ArgumentException>(() =>
            {
                masterMod.WriteToBinary(masterPath,
                    new BinaryWriteParameters()
                    {
                        MasterFlagMatch = BinaryWriteParameters.MasterFlagMatchOption.ThrowIfMisaligned
                    });
            });
        }

        [Fact]
        public void MasterFlagSync_MasterCorrect()
        {
            using var folder = new TempFolder();
            var masterMod = new OblivionMod(new ModKey("Test", master: true));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esp");
            masterMod.WriteToBinary(masterPath,
                new BinaryWriteParameters()
                {
                    MasterFlagMatch = BinaryWriteParameters.MasterFlagMatchOption.CorrectToPath
                });
            var reimport = OblivionMod.CreateFromBinaryOverlay(masterPath);
            Assert.False(reimport.ModHeader.Flags.HasFlag(ModHeader.HeaderFlag.Master));
        }

        [Fact]
        public void MasterFlagSync_ChildCorrect()
        {
            using var folder = new TempFolder();
            var masterMod = new OblivionMod(new ModKey("Test", master: false));
            var masterPath = Path.Combine(folder.Dir.Path, "Test.esm");
            masterMod.WriteToBinary(masterPath,
                new BinaryWriteParameters()
                {
                    MasterFlagMatch = BinaryWriteParameters.MasterFlagMatchOption.CorrectToPath
                });
            var reimport = OblivionMod.CreateFromBinaryOverlay(masterPath);
            Assert.True(reimport.ModHeader.Flags.HasFlag(ModHeader.HeaderFlag.Master));
        }
    }
}
