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

namespace Mutagen.Tests
{
    public class Passthrough_Tests
    {
        [Fact]
        public void OblivionESM()
        {
            OblivionMod_ErrorMask inputErrMask, outputErrMask;
            var mod = OblivionMod.Create_OblivionBinary(
                Properties.Settings.Default.OblivionESM,
                out inputErrMask);
            using (var tmp = new TempFolder())
            {
                var outputPath = Path.Combine(tmp.Dir.FullName, Path.GetRandomFileName());
                mod.Write_OblivionBinary(
                    outputPath,
                    out outputErrMask);
                AssertFilesEqual(Properties.Settings.Default.OblivionESM, outputPath);
                Assert.Null(inputErrMask);
                Assert.Null(outputErrMask);
            }
        }

        [Fact]
        public void KnightsESP()
        {
            OblivionMod_ErrorMask inputErrMask, outputErrMask;
            var mod = OblivionMod.Create_OblivionBinary(
                Properties.Settings.Default.KnightsESP,
                out inputErrMask);
            using (var tmp = new TempFolder())
            {
                var outputPath = Path.Combine(tmp.Dir.FullName, Path.GetRandomFileName());
                mod.Write_OblivionBinary(
                    outputPath,
                    out outputErrMask);
                AssertFilesEqual(Properties.Settings.Default.KnightsESP, outputPath);
                Assert.Null(inputErrMask);
                Assert.Null(outputErrMask);
            }
        }

        private void AssertFilesEqual(string path1, string path2)
        {
            using (var reader1 = new FileStream(path1, FileMode.Open, FileAccess.Read))
            {
                using (var reader2 = new FileStream(path2, FileMode.Open, FileAccess.Read))
                {
                    while (reader1.Position < reader1.Length
                        && reader2.Position < reader2.Length)
                    {
                        var b1 = reader1.ReadByte();
                        var b2 = reader2.ReadByte();
                        if (b1 != b2)
                        {
                            throw new ArgumentException($"Bytes did not match at position: {reader1.Position - 1}");
                        }
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
    }
}
