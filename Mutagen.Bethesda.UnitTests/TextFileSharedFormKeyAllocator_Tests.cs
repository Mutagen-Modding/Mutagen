using Mutagen.Bethesda.Core.Persistance;
using Mutagen.Bethesda.Oblivion;
using Noggog.Utility;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.IO;
using System.Linq;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class TextFileSharedFormKeyAllocator_Tests : ISharedFormKeyAllocator_Tests
    {
        protected override IFormKeyAllocator CreateFormKeyAllocator(IMod mod)
        {
            return new TextFileSharedFormKeyAllocator(mod);
        }

        protected override ISharedFormKeyAllocator CreateFormKeyAllocator(IMod mod, string patcherName)
        {
            return new TextFileSharedFormKeyAllocator(mod, patcherName);
        }

        protected override IFormKeyAllocator LoadFormKeyAllocator(IMod mod)
        {
            var file = tempFile.Value;
            return TextFileSharedFormKeyAllocator.FromFile(mod, file.File.Path);
        }

        protected override ISharedFormKeyAllocator LoadFormKeyAllocator(IMod mod, string patcherName)
        {
            var folder = tempFolder.Value;
            return TextFileSharedFormKeyAllocator.FromFolder(mod, folder.Dir.Path, patcherName);
        }

        protected override void DisposeFormKeyAllocator(IFormKeyAllocator allocator)
        {
            if (tempFile.IsValueCreated)
                tempFile.Value.Dispose();
        }

        protected override void DisposeFormKeyAllocator(ISharedFormKeyAllocator allocator, string patcherName)
        {
            if (tempFolder.IsValueCreated)
                tempFolder.Value.Dispose();
        }

        protected override void SaveFormKeyAllocator(IFormKeyAllocator allocator)
        {
            var file = tempFile.Value;
            ((TextFileSharedFormKeyAllocator)allocator).WriteToFile(file.File.Path);
        }

        protected override void SaveFormKeyAllocator(ISharedFormKeyAllocator allocator, string patcherName)
        {
            var folder = tempFolder.Value;
            ((TextFileSharedFormKeyAllocator)allocator).WriteToFolder(folder.Dir.Path);
        }

        [Fact]
        public void StaticExport()
        {
            using var file = tempFile.Value;
            TextFileSharedFormKeyAllocator.WriteToFile(
                file.File.Path,
                new (string, FormKey)[]
                {
                    (Utility.Edid1, Utility.Form1),
                    (Utility.Edid2, Utility.Form2),
                });
            var lines = File.ReadAllLines(file.File.Path);
            Assert.Equal(
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ToString(),
                    Utility.Edid2,
                    Utility.Form2.ToString(),
                },
                lines);
        }

        [Fact]
        public void TypicalImport()
        {
            using var file = tempFile.Value;
            File.WriteAllLines(
                file.File.Path,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ToString(),
                    Utility.Edid2,
                    Utility.Form2.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = TextFileSharedFormKeyAllocator.FromFile(mod, file.File.Path);
            var formID = allocator.GetNextFormKey(Utility.Edid1);
            Assert.Equal(Utility.PluginModKey, formID.ModKey);
            Assert.Equal(formID, Utility.Form1);
            formID = allocator.GetNextFormKey(Utility.Edid2);
            Assert.Equal(formID, Utility.Form2);
        }

        [Fact]
        public void FailedImportTruncatedFile()
        {
            using var file = tempFile.Value;
            File.WriteAllLines(
                file.File.Path,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ToString(),
                    Utility.Edid2,
                    //Utility.Form2.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.Throws<ArgumentException>(() =>
            {
                TextFileSharedFormKeyAllocator.FromFile(mod, file.File.Path);
            });
        }

        [Fact]
        public void FailedImportDuplicateFormKey()
        {
            using var file = tempFile.Value;
            File.WriteAllLines(
                file.File.Path,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ToString(),
                    Utility.Edid2,
                    Utility.Form1.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.Throws<ArgumentException>(() =>
            {
                TextFileSharedFormKeyAllocator.FromFile(mod, file.File.Path);
            });
        }

        [Fact]
        public void FailedImportDuplicateEditorID()
        {
            using var file = tempFile.Value;
            File.WriteAllLines(
                file.File.Path,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ToString(),
                    Utility.Edid1,
                    Utility.Form2.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.Throws<ArgumentException>(() =>
            {
                TextFileSharedFormKeyAllocator.FromFile(mod, file.File.Path);
            });
        }

        [Fact]
        public void TypicalReimport()
        {
            var list = new (string, FormKey)[]
            {
                (Utility.Edid1, Utility.Form1),
                (Utility.Edid2, Utility.Form2),
            };
            using var file = tempFile.Value;
            TextFileSharedFormKeyAllocator.WriteToFile(file.File.Path, list);
            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = TextFileSharedFormKeyAllocator.FromFile(mod, file.File.Path);
            var formID = allocator.GetNextFormKey();
            Assert.Equal(Utility.PluginModKey, formID.ModKey);
            formID = allocator.GetNextFormKey(Utility.Edid1);
            Assert.Equal(formID, Utility.Form1);
            formID = allocator.GetNextFormKey(Utility.Edid2);
            Assert.Equal(formID, Utility.Form2);
        }

        [Fact]
        public void WritingMultiplePatcherStateToFileFails()
        {
            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = CreateFormKeyAllocator(mod, Patcher1);

            Assert.Throws<InvalidOperationException>(() =>
            {
                SaveFormKeyAllocator(allocator);
            });

            DisposeFormKeyAllocator(allocator, Patcher1);
        }
    }
}
