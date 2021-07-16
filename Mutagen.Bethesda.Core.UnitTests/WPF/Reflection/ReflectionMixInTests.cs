using FluentAssertions;
using Mutagen.Bethesda.WPF.Reflection;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Xunit;

namespace Mutagen.Bethesda.Core.UnitTests.WPF.Reflection
{
    public class ReflectionMixInTests
    {
        [Fact]
        public void IsNamedTypical()
        {
            ReflectionMixIn.IsNamed(typeof(Tooltip), nameof(Tooltip))
                .Should().BeTrue();
        }

        class OtherAttribute : Tooltip
        {
            public OtherAttribute(string text) : base(text)
            {
            }
        }
        
        [Fact]
        public void IsNamedInherit()
        {
            ReflectionMixIn.IsNamed(typeof(OtherAttribute), nameof(Tooltip))
                .Should().BeTrue();
        }
    }
}