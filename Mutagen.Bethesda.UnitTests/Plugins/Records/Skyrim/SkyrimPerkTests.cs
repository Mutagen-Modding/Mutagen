using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim
{
    public class SkyrimPerk_Test
    {
        [Fact]
        public void FunctionParametersTypeNone()
        {
            var perk = Perk.CreateFromBinary(
                TestDataPathing.GetReadFrame(
                    TestDataPathing.SkyrimPerkFunctionParametersTypeNone,
                    GameRelease.SkyrimSE));
            perk.Effects.Should().HaveCount(2);
        }
    }
}