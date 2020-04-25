using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class InterfaceCrossPollination_Tests
    {
        [Fact]
        public void CopyIn()
        {
            WeightValue wv = new WeightValue()
            {
                Weight = 0.5f,
                Value = 6
            };
            LightData ld = new LightData();
            ld.DeepCopyIn(wv);
            Assert.Equal(wv.Weight, ld.Weight);
            Assert.Equal(wv.Value, ld.Value);
        }

        [Fact]
        public void TypicalEquals()
        {
            IWeightValueGetter wv = new WeightValue()
            {
                Weight = 0.5f,
                Value = 6
            };
            LightData ld = new LightData();
            Assert.NotEqual(wv, ld);
            ld.Weight = 0.5f;
            ld.Value = 6;
            Assert.True(wv.Equals(ld));
            Assert.False(((IWeightValueGetter)ld).Equals(wv));
        }
    }
}
