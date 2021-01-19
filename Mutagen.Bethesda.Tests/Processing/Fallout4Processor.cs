using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public class Fallout4Processor : Processor
    {
        public Fallout4Processor(bool multithread) : base(multithread)
        {
        }

        public override GameRelease GameRelease => GameRelease.Fallout4;

    }
}
