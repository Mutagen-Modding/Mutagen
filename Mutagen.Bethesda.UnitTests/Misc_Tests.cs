using Loqui;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Misc_Tests
    {
        [Fact]
        public void GroupErrTest()
        {
            Group.ErrorMask<Npc.ErrorMask> group_ErrorMask = new Group.ErrorMask<Npc.ErrorMask>();
            group_ErrorMask.SetNthMask(
                (int)Group_FieldIndex.RecordCache,
                new MaskItem<Exception?, IEnumerable<MaskItem<Exception, Npc.ErrorMask>>?>(null, null));
        }
    }
}
