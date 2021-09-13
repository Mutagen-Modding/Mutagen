using System.IO;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Skyrim.Records
{
    public class SkyrimPerk_Test
    {
        [Fact]
        public void FunctionParametersTypeNone()
        {
            var frame = new MutagenFrame(
                new MutagenBinaryReadStream(
                    File.OpenRead(TestDataPathing.SkyrimPerkFunctionParametersTypeNone), 
                    new ParsingBundle(
                        GameRelease.SkyrimSE, 
                        new MasterReferenceReader(Constants.Skyrim))));

            var perk = Perk.CreateFromBinary(frame);
            perk.Effects.Should().HaveCount(2);
        }
    }
}