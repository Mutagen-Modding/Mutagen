using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class MajorRecordInstantiator_Tests
    {
        [Fact]
        public void Typical()
        {
            var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
            var ret = MajorRecordInstantiator<Ammunition>.Activator(form);
            Assert.IsType<Ammunition>(ret);
            Assert.Equal(form, ret.FormKey);
        }
    }
}
