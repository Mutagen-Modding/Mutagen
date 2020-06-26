using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.UnitTests
{
    public static class Utility
    {
        public static readonly ModKey ModKey = new ModKey("MutagenDummyKey", master: false);
        public static readonly ModKey ModKey2 = new ModKey("MutagenDummyKey2", master: false);
        public static readonly string Edid1 = "AnEdid1";
        public static readonly string Edid2 = "AnEdid2";
        public static readonly string Edid3 = "AnEdid2";
        public static readonly FormKey Form1 = new FormKey(ModKey, 0x123456);
        public static readonly FormKey Form2 = new FormKey(ModKey, 0x12345F);
        public static readonly FormKey Form3 = new FormKey(ModKey, 0x223456);
    }
}
