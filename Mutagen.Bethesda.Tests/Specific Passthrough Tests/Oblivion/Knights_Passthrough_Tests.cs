using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public class Knights_Passthrough_Tests : Oblivion_Passthrough_Test
    {
        public override string Nickname => TestingConstants.KNIGHTS_ESP;

        public Knights_Passthrough_Tests(TestingSettings settings)
            : base(settings.KnightsESP)
        {
        }
    }
}
