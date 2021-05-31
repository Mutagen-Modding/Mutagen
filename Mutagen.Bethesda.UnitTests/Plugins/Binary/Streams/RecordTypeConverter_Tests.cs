using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Internals;
using System.Collections.Generic;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Streams
{
    public class RecordTypeConverter_Tests
    {
        [Fact]
        public void NullCustom()
        {
            Assert.Equal(RecordTypes.DATA, RecordTypeConverterExt.ConvertToCustom(null, RecordTypes.DATA));
        }

        [Fact]
        public void NullStandard()
        {
            Assert.Equal(RecordTypes.DATA, RecordTypeConverterExt.ConvertToStandard(null, RecordTypes.DATA));
        }

        [Fact]
        public void TypicalCustom()
        {
            RecordTypeConverter conv = new RecordTypeConverter(
                new KeyValuePair<RecordType, RecordType>(RecordTypes.DATA, RecordTypes.MAST));
            Assert.Equal(RecordTypes.MAST, RecordTypeConverterExt.ConvertToCustom(conv, RecordTypes.DATA));
        }

        [Fact]
        public void UnrelatedCustom()
        {
            RecordTypeConverter conv = new RecordTypeConverter(
                new KeyValuePair<RecordType, RecordType>(RecordTypes.DATA, RecordTypes.MAST));
            Assert.Equal(RecordTypes.EDID, RecordTypeConverterExt.ConvertToCustom(conv, RecordTypes.EDID));
        }

        [Fact]
        public void CollidingCustom()
        {
            RecordTypeConverter conv = new RecordTypeConverter(
                new KeyValuePair<RecordType, RecordType>(RecordTypes.DATA, RecordTypes.MAST));
            Assert.Equal(RecordType.Null, RecordTypeConverterExt.ConvertToCustom(conv, RecordTypes.MAST));
        }

        [Fact]
        public void TypicalStandard()
        {
            RecordTypeConverter conv = new RecordTypeConverter(
                new KeyValuePair<RecordType, RecordType>(RecordTypes.DATA, RecordTypes.MAST));
            Assert.Equal(RecordTypes.DATA, RecordTypeConverterExt.ConvertToStandard(conv, RecordTypes.MAST));
        }

        [Fact]
        public void UnrelatedStandard()
        {
            RecordTypeConverter conv = new RecordTypeConverter(
                new KeyValuePair<RecordType, RecordType>(RecordTypes.DATA, RecordTypes.MAST));
            Assert.Equal(RecordTypes.EDID, RecordTypeConverterExt.ConvertToStandard(conv, RecordTypes.EDID));
        }

        [Fact]
        public void CollidingStandard()
        {
            RecordTypeConverter conv = new RecordTypeConverter(
                new KeyValuePair<RecordType, RecordType>(RecordTypes.DATA, RecordTypes.MAST));
            Assert.Equal(RecordType.Null, RecordTypeConverterExt.ConvertToStandard(conv, RecordTypes.DATA));
        }
    }
}
