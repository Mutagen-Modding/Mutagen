using Mutagen.Bethesda.Persistance;
using Mutagen.Bethesda.Oblivion;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class SimpleFormKeyAllocator_Tests : IFormKeyAllocator_Tests
    {
        protected override IFormKeyAllocator CreateFormKeyAllocator(IMod mod) => new SimpleFormKeyAllocator(mod);

        protected override void DisposeFormKeyAllocator(IFormKeyAllocator allocator) { }
    }
}
