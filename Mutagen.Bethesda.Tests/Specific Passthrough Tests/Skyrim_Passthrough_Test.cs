using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace Mutagen.Bethesda.Tests
{
    public class Skyrim_Passthrough_Test : PassthroughTest
    {
        public Skyrim_Passthrough_Test(TestingSettings settings, Passthrough passthrough) 
            : base(settings, passthrough)
        {
        }

        public override ModRecordAligner.AlignmentRules GetAlignmentRules()
        {
            return new ModRecordAligner.AlignmentRules();
        }

        protected override async Task<IMod> ImportBinary(FilePath path, ModKey modKey)
        {
            return await SkyrimMod.Create_Binary(path.Path, modKey);
        }
    }
}
