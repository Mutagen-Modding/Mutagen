using System;
using System.Collections.Generic;
using System.Text;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Tests
{
    public class SkyrimProcessor : Processor
    {
        public override GameMode GameMode => GameMode.Skyrim;
    }
}
