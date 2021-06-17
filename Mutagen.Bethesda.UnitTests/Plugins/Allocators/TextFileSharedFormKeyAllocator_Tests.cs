using Mutagen.Bethesda.Plugins.Allocators;
using Mutagen.Bethesda.Oblivion;
using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Plugins.Allocators
{
    public class TextFileSharedFormKeyAllocator_Tests : ISharedFormKeyAllocator_Tests<TextFileSharedFormKeyAllocator>
    {
        private const string DefaultName = "default";

        protected override TextFileSharedFormKeyAllocator CreateAllocator(IFileSystem fileSystem, IMod mod, string path) => new(mod, path, DefaultName, preload: true, fileSystem: fileSystem);

        protected override TextFileSharedFormKeyAllocator CreateNamedAllocator(IFileSystem fileSystem, IMod mod, string path, string patcherName) => new(mod, path, patcherName, preload: true, fileSystem: fileSystem);

        public const string SomeFolder = "C:/SomeFolder";
        
        protected override string ConstructTypicalPath(IFileSystem fileSystem)
        {
            fileSystem.Directory.CreateDirectory(SomeFolder);
            fileSystem.File.WriteAllText(Path.Combine(SomeFolder, TextFileSharedFormKeyAllocator.MarkerFileName), null);
            return SomeFolder;
        }

        [Fact]
        public void StaticExport()
        {
            var fileSystem = new MockFileSystem();
            var someFile = "C:/SomeFile";
            TextFileSharedFormKeyAllocator.WriteToFile(
                someFile,
                new (string, FormKey)[]
                {
                    (Utility.Edid1, Utility.Form1),
                    (Utility.Edid2, Utility.Form2),
                },
                fileSystem);
            var lines = fileSystem.File.ReadAllLines(someFile + ".txt");
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

        private IFileSystem GetTypicalFileSystem()
        {
            var fileSystem = new MockFileSystem();
            fileSystem.Directory.CreateDirectory(SomeFolder);
            return fileSystem;
        }

        [Fact]
        public void TypicalImport()
        {
            var fileSystem = GetTypicalFileSystem();
            fileSystem.File.WriteAllLines(
                Path.Combine(SomeFolder, Patcher1 + ".txt"),
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ID.ToString(),
                    Utility.Edid2,
                    Utility.Form2.ID.ToString(),
                });
            fileSystem.File.WriteAllText(Path.Combine(SomeFolder, TextFileSharedFormKeyAllocator.MarkerFileName), null);
            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = new TextFileSharedFormKeyAllocator(mod, SomeFolder, Patcher1, preload: true, fileSystem: fileSystem);
            var formID = allocator.GetNextFormKey(Utility.Edid1);
            Assert.Equal(Utility.PluginModKey, formID.ModKey);
            Assert.Equal(Utility.Form1, formID);
            formID = allocator.GetNextFormKey(Utility.Edid2);
            Assert.Equal(Utility.Form2, formID);
        }

        [Fact]
        public void FailedImportTruncatedFile()
        {
            var fileSystem = GetTypicalFileSystem();
            fileSystem.File.WriteAllLines(
                Path.Combine(SomeFolder, Patcher1 + ".txt"),
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ID.ToString(),
                    Utility.Edid2,
                    //Utility.Form2.ID.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.ThrowsAny<Exception>(() => new TextFileSharedFormKeyAllocator(mod, SomeFolder, DefaultName, preload: true, fileSystem: fileSystem));
        }

        [Fact]
        public void FailedImportDuplicateFormKey()
        {
            var fileSystem = GetTypicalFileSystem();
            fileSystem.File.WriteAllText(Path.Combine(SomeFolder, TextFileSharedFormKeyAllocator.MarkerFileName), null);
            fileSystem.File.WriteAllLines(
                Path.Combine(SomeFolder, Patcher1 + ".txt"),
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ID.ToString(),
                    Utility.Edid2,
                    Utility.Form1.ID.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.Throws<ArgumentException>(() => new TextFileSharedFormKeyAllocator(mod, SomeFolder, DefaultName, preload: true, fileSystem: fileSystem));
        }

        [Fact]
        public void FailedImportDuplicateEditorID()
        {
            var fileSystem = GetTypicalFileSystem();
            fileSystem.File.WriteAllText(Path.Combine(SomeFolder, TextFileSharedFormKeyAllocator.MarkerFileName), null);
            fileSystem.File.WriteAllLines(
                Path.Combine(SomeFolder, Patcher1 + ".txt"),
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ID.ToString(),
                    Utility.Edid1,
                    Utility.Form2.ID.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.Throws<ArgumentException>(() => new TextFileSharedFormKeyAllocator(mod, SomeFolder, Patcher1, preload: true, fileSystem: fileSystem));
        }

        [Fact]
        public void TypicalReimport()
        {
            var fileSystem = GetTypicalFileSystem();
            TextFileSharedFormKeyAllocator.WriteToFile(
                Path.Combine(SomeFolder, Patcher1),
                new (string, FormKey)[]
                {
                    (Utility.Edid1, Utility.Form1),
                    (Utility.Edid2, Utility.Form2),
                },
                fileSystem: fileSystem);
            fileSystem.File.WriteAllText(Path.Combine(SomeFolder, TextFileSharedFormKeyAllocator.MarkerFileName), null);
            var mod = new OblivionMod(Utility.PluginModKey);
            using var allocator = new TextFileSharedFormKeyAllocator(mod, SomeFolder, Patcher1, fileSystem: fileSystem);
            var formID = allocator.GetNextFormKey();
            Assert.Equal(Utility.PluginModKey, formID.ModKey);
            formID = allocator.GetNextFormKey(Utility.Edid1);
            Assert.Equal(formID, Utility.Form1);
            formID = allocator.GetNextFormKey(Utility.Edid2);
            Assert.Equal(formID, Utility.Form2);
        }
    }
}
