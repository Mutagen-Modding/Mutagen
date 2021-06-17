using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Allocators;
using Mutagen.Bethesda.Plugins.Records;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Allocators
{
    public class TextFileFormKeyAllocator_Tests : IPersistentFormKeyAllocator_Tests<TextFileFormKeyAllocator>
    {
        protected override TextFileFormKeyAllocator CreateAllocator(IFileSystem fileSystem, IMod mod, string path)
        {
            return new(mod, path, preload: true, fileSystem: fileSystem);
        }

        protected override string ConstructTypicalPath(IFileSystem fileSystem)
        {
            return "C:/SomeFile";
        }

        [Fact]
        public void StaticExport()
        {
            var fileSystem = new MockFileSystem();
            var someFile = "C:/SomeFile";
            TextFileFormKeyAllocator.WriteToFile(
                someFile,
                new KeyValuePair<string, FormKey>[]
                {
                    new KeyValuePair<string, FormKey>(Utility.Edid1, Utility.Form1),
                    new KeyValuePair<string, FormKey>(Utility.Edid2, Utility.Form2),
                },
                fileSystem);

            var lines = fileSystem.File.ReadAllLines(someFile);
            Assert.Equal(
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ID.ToString(),
                    Utility.Edid2,
                    Utility.Form2.ID.ToString(),
                },
                lines);
        }

        [Fact]
        public void TypicalImport()
        {
            var fileSystem = new MockFileSystem();
            var someFile = "C:/SomeFile";
            fileSystem.File.WriteAllLines(
                someFile,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ID.ToString(),
                    Utility.Edid2,
                    Utility.Form2.ID.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            using var allocator = new TextFileFormKeyAllocator(mod, someFile, fileSystem: fileSystem);
            var formID = allocator.GetNextFormKey(Utility.Edid1);
            Assert.Equal(Utility.PluginModKey, formID.ModKey);
            Assert.Equal(formID, Utility.Form1);
            formID = allocator.GetNextFormKey(Utility.Edid2);
            Assert.Equal(formID, Utility.Form2);
        }

        [Fact]
        public void FailedImportTruncatedFile()
        {
            var fileSystem = new MockFileSystem();
            var someFile = "C:/SomeFile";

            fileSystem.File.WriteAllLines(
                someFile,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ID.ToString(),
                    Utility.Edid2,
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.Throws<ArgumentException>(() => new TextFileFormKeyAllocator(mod, someFile, preload: true, fileSystem:fileSystem));
        }

        [Fact]
        public void FailedImportDuplicateFormKey()
        {
            var fileSystem = new MockFileSystem();
            var someFile = "C:/SomeFile";

            fileSystem.File.WriteAllLines(
                someFile,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ID.ToString(),
                    Utility.Edid2,
                    Utility.Form1.ID.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.Throws<ArgumentException>(() => new TextFileFormKeyAllocator(mod, someFile, preload: true, fileSystem: fileSystem));
        }

        [Fact]
        public void FailedImportDuplicateEditorID()
        {
            var someFile = "C:/SomeFile";

            var fileSystem = new MockFileSystem();
            fileSystem.File.WriteAllLines(
                someFile,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ID.ToString(),
                    Utility.Edid1,
                    Utility.Form2.ID.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.Throws<ArgumentException>(() => new TextFileFormKeyAllocator(mod, someFile, preload: true, fileSystem: fileSystem));
        }

        [Fact]
        public void TypicalReimport()
        {
            var fileSystem = new MockFileSystem();
            var someFile = "C:/SomeFile";
            TextFileFormKeyAllocator.WriteToFile(
                someFile,
                new KeyValuePair<string, FormKey>[]
                {
                    new KeyValuePair<string, FormKey>(Utility.Edid1, Utility.Form1),
                    new KeyValuePair<string, FormKey>(Utility.Edid2, Utility.Form2),
                },
                fileSystem);
            var mod = new OblivionMod(Utility.PluginModKey);
            using var allocator = new TextFileFormKeyAllocator(mod, someFile, preload: true, fileSystem: fileSystem);
            var formID = allocator.GetNextFormKey(Utility.Edid1);
            Assert.Equal(Utility.PluginModKey, formID.ModKey);
            Assert.Equal(formID, Utility.Form1);
            formID = allocator.GetNextFormKey(Utility.Edid2);
            Assert.Equal(formID, Utility.Form2);
        }
    }
}
