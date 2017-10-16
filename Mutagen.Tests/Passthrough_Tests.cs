using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen;
using Noggog.Utility;
using Xunit;
using Mutagen.Internals;
using Noggog;

namespace Mutagen.Tests
{
    public class Passthrough_Tests
    {
        [Fact]
        public void OblivionESM_Binary()
        {
            OblivionMod_ErrorMask inputErrMask, outputErrMask;
            var mod = OblivionMod.Create_Binary(
                Properties.Settings.Default.OblivionESM,
                out inputErrMask);
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_Binary"))))
            {
                var outputPath = Path.Combine(tmp.Dir.FullName, Constants.OBLIVION_ESM);
                mod.Write_Binary(
                    outputPath,
                    out outputErrMask);
                AssertFilesEqual(Properties.Settings.Default.OblivionESM, outputPath);
                Assert.Null(inputErrMask);
                Assert.Null(outputErrMask);
            }
        }

        [Fact]
        public void KnightsESP_Binary()
        {
            OblivionMod_ErrorMask inputErrMask, outputErrMask;
            var mod = OblivionMod.Create_Binary(
                Properties.Settings.Default.KnightsESP,
                out inputErrMask);
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Knights_Binary"))))
            {
                var outputPath = Path.Combine(tmp.Dir.FullName, Constants.KNIGHTS_ESP);
                mod.Write_Binary(
                    outputPath,
                    out outputErrMask);
                AssertFilesEqual(Properties.Settings.Default.KnightsESP, outputPath);
                Assert.Null(inputErrMask);
                Assert.Null(outputErrMask);
            }
        }
        [Fact]
        public void OblivionESM_XML()
        {
            var modFromBinary = OblivionMod.Create_Binary(
                Properties.Settings.Default.OblivionESM,
                out var binInputErrMask);
            Assert.Null(binInputErrMask);
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_XML"))))
            {
                var outputPath = Path.Combine(tmp.Dir.FullName, Path.GetRandomFileName());
                modFromBinary.Write_XML(
                    outputPath,
                    out var outputErrMask);
                var modFromXML = OblivionMod.Create_XML(
                    outputPath,
                    out var xmlInputErrMask);
                Assert.Equal(modFromBinary, modFromXML);
                Assert.Null(xmlInputErrMask);
                Assert.Null(outputErrMask);
            }
        }

        [Fact]
        public void KnightsESP_XML()
        {
            var modFromBinary = OblivionMod.Create_Binary(
                Properties.Settings.Default.KnightsESP,
                out var binInputErrMask);
            Assert.Null(binInputErrMask);
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Knights_XML"))))
            {
                var outputPath = Path.Combine(tmp.Dir.FullName, Path.GetRandomFileName());
                modFromBinary.Write_XML(
                    outputPath,
                    out var outputErrMask);
                var modFromXML = OblivionMod.Create_XML(
                    outputPath,
                    out var xmlInputErrMask);
                Assert.Equal(modFromBinary, modFromXML);
                Assert.Null(xmlInputErrMask);
                Assert.Null(outputErrMask);
            }
        }

        private void AssertFilesEqual(string path1, string path2)
        {
            List<RangeInt32> errorRanges = new List<RangeInt32>();
            using (var reader1 = new FileStream(path1, FileMode.Open, FileAccess.Read))
            {
                using (var reader2 = new FileStream(path2, FileMode.Open, FileAccess.Read))
                {
                    var errs = RangeInt64.ConstructRanges(
                        GetDifferences(reader1, reader2),
                        b => !b).First(5).ToArray();
                    if (errs.Length > 0)
                    {
                        throw new ArgumentException($"Bytes did not match at positions: {string.Join(" ", errs)}");
                    }
                    if (reader1.Position != reader1.Length)
                    {
                        throw new ArgumentException($"Stream {path1} had more data past position {reader1.Position} than {path2}");
                    }
                    if (reader2.Position != reader2.Length)
                    {
                        throw new ArgumentException($"Stream {path2} had more data past position {reader2.Position} than {path1}");
                    }
                }
            }
        }

        private IEnumerable<KeyValuePair<long, bool>> GetDifferences(FileStream reader1, FileStream reader2)
        {
            while (reader1.Position < reader1.Length
                && reader2.Position < reader2.Length)
            {
                var b1 = reader1.ReadByte();
                var b2 = reader2.ReadByte();
                yield return new KeyValuePair<long, bool>(
                    reader1.Position - 1, 
                    b1 == b2);
            }
        }
    }
}
