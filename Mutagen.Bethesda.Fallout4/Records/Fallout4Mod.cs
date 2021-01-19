using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System.IO;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Fallout4Mod : AMod
    {
        public const uint DefaultInitialNextFormID = 0x800;
        private uint GetDefaultInitialNextFormID() => DefaultInitialNextFormID;

        partial void CustomCtor()
        {
            this.ModHeader.FormVersion = GameRelease.Fallout4.GetDefaultFormVersion()!.Value;
        }
    }
}
