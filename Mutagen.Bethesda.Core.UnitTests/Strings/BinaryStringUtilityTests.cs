using System;
using System.IO;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.Core.UnitTests.Strings
{
    public class BinaryStringUtilityTests
    {
        [Fact]
        public void ToZStringFrenchExample()
        {
            var ret = new MutagenEncodingProvider().GetEncoding(GameRelease.SkyrimSE, Language.French)
                .GetString(File.ReadAllBytes(TestDataPathing.FrenchString));
            ret.Should().Be("Livre de sort - Paralysie générale");
        }

        [Fact]
        public void ParsePrependedStringTypical()
        {
            BinaryStringUtility.ParsePrependedString(File.ReadAllBytes(TestDataPathing.PrependedString), 2)
                .Should().Be("HelloWorld");
        }

        [Fact]
        public void ParsePrependedStringZeroLength()
        {
            BinaryStringUtility.ParsePrependedString(File.ReadAllBytes(TestDataPathing.ZeroContentPrependedString), 2)
                .Should().Be("");
        }

        [Fact]
        public void ProcessNullTerminationOnEmpty()
        {
            var span = BinaryStringUtility.ProcessNullTermination(Array.Empty<byte>().AsSpan());
            span.Length.Should().Be(0);
        }
    }
}