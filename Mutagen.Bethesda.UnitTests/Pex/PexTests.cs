using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Pex;
using Mutagen.Bethesda.Pex.Binary.Translations;
using Mutagen.Bethesda.Pex.Records;
using Mutagen.Bethesda.Skyrim.Pex;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Pex
{
    public class PexTests
    {
        public static readonly IEnumerable<object[]> TestDataFiles = new List<object[]>
        {
            //from SKSE https://skse.silverlock.org
            new object[]{ "Actor.pex", GameRelease.SkyrimSE },
            new object[]{ "Art.pex", GameRelease.SkyrimSE },
            new object[]{ "FormType.pex", GameRelease.SkyrimSE },
            new object[]{ "Game.pex", GameRelease.SkyrimSE },
            new object[]{ "ObjectReference.pex", GameRelease.SkyrimSE },
            new object[]{ "Outfit.pex", GameRelease.SkyrimSE },
            new object[]{ "SoulGem.pex", GameRelease.SkyrimSE },
            
            //from F4SE https://f4se.silverlock.org
            //new object[]{ "ActorBase-F04.pex", GameCategory.Fallout4 },
            
            //from https://github.com/mwilsnd/SkyrimSE-SmoothCam/blob/master/CodeGen/MCM/SmoothCamMCM.pex
            new object[]{ "SmoothCamMCM.pex", GameRelease.SkyrimSE },
            
            //from https://www.nexusmods.com/skyrimspecialedition/mods/18076
            new object[]{ "nwsFollowerMCMExScript.pex", GameRelease.SkyrimSE },
            new object[]{ "nwsFollowerMCMScript.pex", GameRelease.SkyrimSE },
        };


        [Theory]
        [MemberData(nameof(TestDataFiles))]
        public void TestPexParsing(string file, GameRelease gameCategory)
        {
            var path = Path.Combine("Pex", "files", file);
            Assert.True(File.Exists(path));

            var pex = PexFileInstantiator.Importer(path, gameCategory);
            Assert.NotNull(pex);
        }

        [Theory]
        [MemberData(nameof(TestDataFiles))]
        public void TestPexWriting(string file, GameRelease gameCategory)
        {
            var inputFile = Path.Combine("Pex", "files", file);
            Assert.True(File.Exists(inputFile));

            var inputPex = PexFileInstantiator.Importer(inputFile, gameCategory);

            var outputFile = Path.Combine("output", file);
            inputPex.WriteToBinary(outputFile);
            Assert.True(File.Exists(outputFile));

            var outputPex = PexFileInstantiator.Importer(outputFile, gameCategory);
            inputPex.Equals(outputPex).Should().BeTrue();
        }

        private PexFile GetExpectedArtPex()
        {
            var expectedPex = new PexFile()
            {
                CompilationTime = new DateTime(0x08d833e452f0d480),
                MajorVersion = 0x03,
                MinorVersion = 0x02,
                GameId = 0x01,
                SourceFileName = "Art.psc",
                DebugInfo = new DebugInfo()
                {
                    ModificationTime = new DateTime(0x08d833e44bc9c680),
                    Functions = new ExtendedList<DebugFunction>()
                    {
                        new()
                        {
                            FunctionName  = "GetState",
                            ObjectName = "Art",
                        },
                        new()
                        {
                            FunctionName  = "GotoState",
                            ObjectName = "Art",
                        },
                        new()
                        {
                            FunctionName  = "GetModelPath",
                            ObjectName = "Art",
                        },
                        new()
                        {
                            FunctionName  = "SetModelPath",
                            ObjectName = "Art",
                        },
                    }
                },
                Objects = new()
                {
                    new Skyrim.Pex.Object()
                    {
                        Name = "Art",
                        ParentClassName = "Form",
                        RawUserFlags = 0x1,
                        States = new ExtendedList<State>()
                        {
                            new State()
                            {
                                 Functions = new ExtendedList<NamedFunction>()
                                 {
                                     new NamedFunction()
                                     {
                                         Function = new Function()
                                         {
                                             DocString = "Function that returns the current state",
                                             Flags = FunctionFlags.GlobalFunction,
                                             Instructions = new ExtendedList<Instruction>()
                                             {
                                                 new ReturnInstruction()
                                                 {
                                                     Identifier = new IdentifierVariable()
                                                     {
                                                         Value =  "::State",
                                                     },
                                                 }
                                             },
                                             ReturnTypeName = "String",
                                         },
                                         FunctionName = "GetState",
                                     },
                                     new NamedFunction()
                                     {
                                         Function = new Function()
                                         {
                                             Flags = (FunctionFlags)0x02, // ??
                                             Parameters = new ExtendedList<FunctionVariable>()
                                             {
                                                 new FunctionVariable()
                                                 {
                                                     Name = "path",
                                                     TypeName = "String"
                                                 }
                                             },
                                             ReturnTypeName = "None",
                                         },
                                         FunctionName = "SetModelPath",
                                     },
                                     new NamedFunction()
                                     {
                                         Function = new Function()
                                         {
                                             DocString = "Function that switches this object to the specified state",
                                             Flags = FunctionFlags.GlobalFunction,
                                             Instructions = new ExtendedList<Instruction>()
                                             {
                                                 new CallMethodInstruction()
                                                 {
                                                     String1 = new IdentifierVariable()
                                                     {
                                                         Value = "onEndState"
                                                     },
                                                     String2 = new IdentifierVariable()
                                                     {
                                                         Value = "self"
                                                     },
                                                     String3 = new IdentifierVariable()
                                                     {
                                                         Value = "::NoneVar"
                                                     },
                                                     ExtraVariables = new ExtendedList<AVariable>()
                                                     {
                                                         new IntVariable()
                                                         {
                                                             Value = 0
                                                         }
                                                     }
                                                 },
                                                 new AssignInstruction()
                                                 {
                                                     Identifier = new IdentifierVariable()
                                                     {
                                                         Value = "::State"
                                                     },
                                                     Value = new IdentifierVariable()
                                                     {
                                                         Value = "newState"
                                                     }
                                                 },
                                                 new CallMethodInstruction()
                                                 {
                                                     String1 = new IdentifierVariable()
                                                     {
                                                         Value = "onBeginState"
                                                     },
                                                     String2 = new IdentifierVariable()
                                                     {
                                                         Value = "self"
                                                     },
                                                     String3 = new IdentifierVariable()
                                                     {
                                                         Value = "::NoneVar"
                                                     },
                                                     ExtraVariables = new ExtendedList<AVariable>()
                                                     {
                                                         new IntVariable()
                                                         {
                                                             Value = 0
                                                         }
                                                     }
                                                 }
                                             },
                                             Locals = new ExtendedList<FunctionVariable>()
                                             {
                                                 new FunctionVariable()
                                                 {
                                                     Name = "::NoneVar",
                                                     TypeName = "None"
                                                 }
                                             },
                                             Parameters = new ExtendedList<FunctionVariable>()
                                             {
                                                 new FunctionVariable()
                                                 {
                                                     Name = "newState",
                                                     TypeName = "String"
                                                 }
                                             },
                                             ReturnTypeName = "None",
                                         },
                                         FunctionName = "GotoState",
                                     },
                                     new NamedFunction()
                                     {
                                         Function = new Function()
                                         {
                                             Flags = (FunctionFlags)0x02,
                                             ReturnTypeName = "String"
                                         },
                                         FunctionName = "GetModelPath"
                                     }
                                 }
                            }
                        }
                    }
                },
            };
            expectedPex.UserFlags[0] = "hidden";
            expectedPex.UserFlags[1] = "conditional";
            return expectedPex;
        }

        [Fact]
        public void TestSinglePexParsing()
        {
            var path = Path.Combine("Pex", "files", "Art.pex");
            Assert.True(File.Exists(path));

            var pex = PexFile.CreateFromBinary(path);

            Assert.Equal(3, pex.MajorVersion);
            Assert.Equal(2, pex.MinorVersion);
            Assert.Equal(1, pex.GameId);
            Assert.Equal(((ulong)0x5F21B0ED).ToDateTime(), pex.CompilationTime);
            Assert.Equal("Art.psc", pex.SourceFileName);
            Assert.Equal(string.Empty, pex.Username);
            Assert.Equal(string.Empty, pex.MachineName);

            var debugInfo = pex.DebugInfo;
            Assert.NotNull(debugInfo);

            {
                Assert.Equal(4, debugInfo!.Functions.Count);
            }

            var objects = pex.Objects;
            Assert.Single(objects);

            var mainObject = objects.First();

            Assert.Equal("Art", mainObject.Name);
            Assert.Equal("Form", mainObject.ParentClassName);
            Assert.Equal(string.Empty, mainObject.DocString);
            Assert.Equal(string.Empty, mainObject.AutoStateName);

            Assert.Empty(mainObject.Properties);
            Assert.Empty(mainObject.Variables);
            Assert.Single(mainObject.States);

            var state = mainObject.States.First();
            Assert.Equal(string.Empty, state.Name);
            Assert.Equal(4, state.Functions.Count);

            var eqMask = pex.GetEqualsMask(GetExpectedArtPex());
            eqMask.All(b => b).Should().BeTrue();
        }

        [Fact]
        public void TestPexAddition()
        {
            var path = Path.Combine("Pex", "files", "Art.pex");
            Assert.True(File.Exists(path));

            var pex = PexFile.CreateFromBinary(path);
            //var functionToAdd = new DebugFunction()
            //{
            //    FunctionName = "HelloWorld",
            //    FunctionType = DebugFunctionType.Method,
            //};
            //pex.DebugInfo?.Functions.Add(functionToAdd);

            using var tempFolder = Utility.GetTempFolder(nameof(PexTests));
            var outPath = Path.Combine(tempFolder.Dir.Path, Path.GetRandomFileName());
            pex.WriteToBinary(outPath);

            var pex2 = PexFile.CreateFromBinary(outPath);

            var expected = GetExpectedArtPex();
            //expected.DebugInfo?.Functions.Add(functionToAdd);
            var eqMask = pex2.GetEqualsMask(expected);
            eqMask.All(b => b).Should().BeTrue();
        }

        [Fact]
        public void UserFlagSync()
        {
            var path = Path.Combine("Pex", "files", "Art.pex");
            Assert.True(File.Exists(path));

            var pex = PexFile.CreateFromBinary(path);
            var flagToAdd = new UserFlag("Random", 15);
            pex.UserFlags[flagToAdd.Index] = flagToAdd.Name;
            pex.Objects.First().SetFlag(pex, flagToAdd, true);

            using var tempFolder = Utility.GetTempFolder(nameof(PexTests));
            var outPath = Path.Combine(tempFolder.Dir.Path, Path.GetTempFileName());
            pex.WriteToBinary(outPath);

            var pex2 = PexFile.CreateFromBinary(outPath);
            pex2.DebugInfo.Should().NotBeNull();
            pex2.Objects[0].HasFlag(pex2, flagToAdd).Should().BeTrue();
        }
    }
}
