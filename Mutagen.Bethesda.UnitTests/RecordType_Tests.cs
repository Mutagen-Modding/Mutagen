using System.Collections.Generic;
using Mutagen.Bethesda.Records;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class RecordType_Tests
    {
        [Fact]
        public void StringToInt()
        {
            var type = new RecordType("NPC_");
            Assert.Equal("NPC_", type.Type);
            Assert.Equal(0x5F43504E, type.TypeInt);
        }

        [Fact]
        public void IntToString()
        {
            var type = new RecordType(0x5F43504E);
            Assert.Equal("NPC_", type.Type);
            Assert.Equal(0x5F43504E, type.TypeInt);
        }

        [Fact]
        public void CaseInvarianceLookup()
        {
            var type = new RecordType("NPC_");
            var type2 = new RecordType("NPc_");
            var set = new HashSet<RecordType>()
            {
                type
            };
            Assert.DoesNotContain(type2, set);
        }
    }
}
