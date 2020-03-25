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
    public class TextFileFormKeyAllocator_Tests
    {
        [Fact]
        public void StaticExport()
        {
            uint nextID = 123;
            using var file = new TempFile();
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
            using var file = new TempFile();
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
            var mod = new OblivionMod(Utility.ModKey);
            var allocator = TextFileFormKeyAllocator.FromFile(mod, file.File.Path);
            var formID = allocator.GetNextFormKey();
            Assert.Equal(nextID, formID.ID);
            Assert.Equal(Utility.ModKey, formID.ModKey);
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
            using var file = new TempFile();
            TextFileFormKeyAllocator.WriteToFile(
                file.File.Path,
                nextID,
                list);
            var mod = new OblivionMod(Utility.ModKey);
            var allocator = TextFileFormKeyAllocator.FromFile(mod, file.File.Path);
            var formID = allocator.GetNextFormKey();
            Assert.Equal(nextID, formID.ID);
            Assert.Equal(Utility.ModKey, formID.ModKey);
            formID = allocator.GetNextFormKey(Utility.Edid1);
            Assert.Equal(formID, Utility.Form1);
            formID = allocator.GetNextFormKey(Utility.Edid2);
            Assert.Equal(formID, Utility.Form2);
        }
    }
}
