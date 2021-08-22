using FluentAssertions;
using Mutagen.Bethesda.Strings;
using Xunit;

namespace Mutagen.Bethesda.Core.UnitTests.Strings
{
    public class BinaryStringUtilityTests
    {
        private static byte[] StringBytes = new byte[]
        {
            0x4C, 0x69, 0x76, 0x72, 0x65, 0x20, 0x64, 0x65, 0x20, 0x73, 0x6F,
            0x72, 0x74, 0x20, 0x2D, 0x20, 0x50, 0x61, 0x72, 0x61, 0x6C, 0x79,
            0x73, 0x69, 0x65, 0x20, 0x67, 0xC3, 0xA9, 0x6E, 0xC3, 0xA9, 0x72,
            0x61, 0x6C, 0x65
        };
        
        [Fact]
        public void ToZStringFrenchExample()
        {
            var ret = Encodings.Get(GameRelease.SkyrimSE, Language.French)
                .GetString(StringBytes);
            ret.Should().Be("Livre de sort - Paralysie générale");
        }
    }
}