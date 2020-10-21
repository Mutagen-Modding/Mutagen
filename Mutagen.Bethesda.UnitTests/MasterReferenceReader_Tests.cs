using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class MasterReferenceReader_Tests
    {
        [Fact]
        public void Typical()
        {
            MasterReferenceReader reader = MasterReferenceReader.FromPath(
                @"D:\Games\steamapps\common\Skyrim Special Edition\Data\Unofficial Skyrim Special Edition Patch.esp",
                GameRelease.SkyrimSE);
            reader.Masters.ToList();
        }
    }
}
