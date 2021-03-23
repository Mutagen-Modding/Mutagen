using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Mutagen.Bethesda.Pex.Extensions;

namespace Mutagen.Bethesda.Pex.Tests
{
    public class PexTests
    {
        public static readonly IEnumerable<object[]> TestDataFiles = new List<object[]>
        {
            //from SKSE https://skse.silverlock.org/
            new object[]{ "Actor.pex", true },
            new object[]{ "Art.pex", true },
            new object[]{ "FormType.pex", true },
            new object[]{ "Game.pex", true },
            new object[]{ "ObjectReference.pex", true },
            new object[]{ "Outfit.pex", true },
            new object[]{ "SoulGem.pex", true },
            
            //from https://github.com/mwilsnd/SkyrimSE-SmoothCam/blob/master/CodeGen/MCM/SmoothCamMCM.pex
            new object[]{ "SmoothCamMCM.pex", true },
            
            //from https://www.nexusmods.com/skyrimspecialedition/mods/18076
            new object[]{ "nwsFollowerMCMExScript.pex", true },
            new object[]{ "nwsFollowerMCMScript.pex", true },
    };
        
        [Theory]
        [MemberData(nameof(TestDataFiles))]
        public void TestPexParsing(string file, bool isBigEndian)
        {
            var path = Path.Combine("files", file);
            Assert.True(File.Exists(path));

            var pex = PexParser.ParsePexFile(path, isBigEndian);
            Assert.NotNull(pex);
        }

        [Theory]
        [MemberData(nameof(TestDataFiles))]
        public void TestPexWriting(string file, bool isBigEndian)
        {
            var inputFile = Path.Combine("files", file);
            Assert.True(File.Exists(inputFile));

            var inputPex = PexParser.ParsePexFile(inputFile, isBigEndian);

            var outputFile = Path.Combine("output", file);
            inputPex.WritePexFile(outputFile, isBigEndian);
            Assert.True(File.Exists(outputFile));

            var outputPex = PexParser.ParsePexFile(outputFile, isBigEndian);
            Assert.NotNull(outputPex);
            
            var inputFi = new FileInfo(inputFile);
            var outputFi = new FileInfo(outputFile);
            
            Assert.Equal(inputFi.Length, outputFi.Length);
        }

        [Fact]
        public void TestSinglePexParsing()
        {
            var path = Path.Combine("files", "Art.pex");
            Assert.True(File.Exists(path));

            var pex = PexParser.ParsePexFile(path, true);
            
            Assert.Equal(0xFA57C0DE, pex.Magic);
            Assert.Equal(3, pex.MajorVersion);
            Assert.Equal(2, pex.MinorVersion);
            Assert.Equal(1, pex.GameId);
            Assert.Equal(((ulong) 0x5F21B0ED).ToDateTime(), pex.CompilationTime);
            Assert.Equal("Art.psc", pex.SourceFileName);
            Assert.Equal(string.Empty, pex.Username);
            Assert.Equal(string.Empty, pex.MachineName);

            var stringTable = pex.StringTable;
            var debugInfo = pex.DebugInfo;
            var userFlagsTable = pex.UserFlags;
            
            Assert.NotNull(stringTable);
            Assert.NotNull(debugInfo);
            Assert.NotNull(userFlagsTable);

            {
                Assert.True(debugInfo.HasDebugInfo);
                Assert.Equal(4, debugInfo.Functions.Count);
            }

            var objects = pex.Objects;
            Assert.Single(objects);

            var mainObject = objects.First();
            
            Assert.Equal("Art", mainObject.GetName(stringTable));
            Assert.Equal("Form", mainObject.GetParentClassName(stringTable));
            Assert.Equal(string.Empty, mainObject.GetDocString(stringTable));
            Assert.Equal(string.Empty, mainObject.GetAutoStateName(stringTable));
            
            Assert.Empty(mainObject.Properties);
            Assert.Empty(mainObject.Variables);
            Assert.Single(mainObject.States);

            var state = mainObject.States.First();
            Assert.Equal(string.Empty, state.GetName(stringTable));
            Assert.Equal(4, state.Functions.Count);
        }
    }
}
