using Loqui;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.Tests
{
    public class Misc_Tests
    {
        [Fact]
        public void GroupErrTest()
        {
            Group_ErrorMask<NPC_ErrorMask> group_ErrorMask = new Group_ErrorMask<NPC_ErrorMask>();
            group_ErrorMask.SetNthMask(
                (int)Group_FieldIndex.Items,
                new MaskItem<Exception, IEnumerable<MaskItem<Exception, NPC_ErrorMask>>>(null, null));
        }
    }
}
