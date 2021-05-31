using FluentAssertions;
using Mutagen.Bethesda.Fallout4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public class BipedObjectConverter_Tests
    {
        [Fact]
        public void CreateFromSlot()
        {
            var e = BipedObjectConverter.BipedObjectBySlot(51);
            Assert.Equal(BipedObject.Ring,e);
        }

        [Fact]
        public void CreateFlagFromSlot()
        {
            var e = BipedObjectConverter.BipedObjectFlagBySlot(51);
            Assert.Equal(BipedObjectFlag.Ring, e);
        }

        [Fact]
        public void ConvertToSlot()
        {
            Assert.Equal(51,BipedObject.Ring.ToSlot());
        }

        [Fact]
        public void ConvertFlagToSlot()
        {
            Assert.Equal(51, BipedObjectFlag.Ring.ToSlot());
        }


        [Fact]
        public void ConvertObjectEnumToFlag()
        {
            Assert.Equal(BipedObjectFlag.RightArmArmor, BipedObject.RightArmArmor.ToFlagEnum());
        }

        [Fact]
        public void ConvertFlagEnumToObjectEnum()
        {
            Assert.Equal(BipedObject.FaceGenHead, BipedObjectFlag.FaceGenHead.ToObjectEnum());
        }

    }
}
