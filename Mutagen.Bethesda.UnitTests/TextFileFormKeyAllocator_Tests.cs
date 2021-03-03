using Mutagen.Bethesda.Core.Persistance;
using Mutagen.Bethesda.Oblivion;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class TextFileFormKeyAllocator_Tests : IPersistentFormKeyAllocator_Tests
    {
        protected override IFormKeyAllocator CreateFormKeyAllocator(IMod mod) => new TextFileFormKeyAllocator(mod);

        protected override void DisposeFormKeyAllocator(IFormKeyAllocator allocator) { }

        protected override IFormKeyAllocator LoadFormKeyAllocator(IMod mod)
        {
            if (tempFile.Value.File.Exists)
                return TextFileFormKeyAllocator.FromFile(mod, tempFile.Value.File.Path);
            return CreateFormKeyAllocator(mod);
        }

        protected override void SaveFormKeyAllocator(IFormKeyAllocator allocator)
        {
            TextFileFormKeyAllocator.WriteToFile(tempFile.Value.File.Path, ((TextFileFormKeyAllocator)allocator).Mod);
        }

        [Fact]
        public void StaticExport()
        {
            uint nextID = 123;
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            TextFileFormKeyAllocator.WriteToFile(
                file.File.Path,
                nextID,
                new KeyValuePair<string, FormKey>[]
                {
                    new KeyValuePair<string, FormKey>(Utility.Edid1, Utility.Form1),
                    new KeyValuePair<string, FormKey>(Utility.Edid2, Utility.Form2),
                });
            var lines = File.ReadAllLines(file.File.Path);
            Assert.Equal(
                new string[]
                {
                    nextID.ToString(),
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
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            uint nextID = 123;
            File.WriteAllLines(
                file.File.Path,
                new string[]
                {
                    nextID.ToString(),
                    Utility.Edid1,
                    Utility.Form1.ToString(),
                    Utility.Edid2,
                    Utility.Form2.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = TextFileFormKeyAllocator.FromFile(mod, file.File.Path);
            var formID = allocator.GetNextFormKey();
            Assert.Equal(nextID, formID.ID);
            Assert.Equal(Utility.PluginModKey, formID.ModKey);
            formID = allocator.GetNextFormKey(Utility.Edid1);
            Assert.Equal(formID, Utility.Form1);
            formID = allocator.GetNextFormKey(Utility.Edid2);
            Assert.Equal(formID, Utility.Form2);
        }

        [Fact]
        public void TypicalReimport()
        {
            uint nextID = 123;
            var list = new KeyValuePair<string, FormKey>[]
            {
                new KeyValuePair<string, FormKey>(Utility.Edid1, Utility.Form1),
                new KeyValuePair<string, FormKey>(Utility.Edid2, Utility.Form2),
            };
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            TextFileFormKeyAllocator.WriteToFile(
                file.File.Path,
                nextID,
                list);
            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = TextFileFormKeyAllocator.FromFile(mod, file.File.Path);
            var formID = allocator.GetNextFormKey();
            Assert.Equal(nextID, formID.ID);
            Assert.Equal(Utility.PluginModKey, formID.ModKey);
            formID = allocator.GetNextFormKey(Utility.Edid1);
            Assert.Equal(formID, Utility.Form1);
            formID = allocator.GetNextFormKey(Utility.Edid2);
            Assert.Equal(formID, Utility.Form2);
        }
    }
}
