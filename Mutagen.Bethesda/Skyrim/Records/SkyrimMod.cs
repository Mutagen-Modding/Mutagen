using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class SkyrimMod
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IList<MasterReference> IMod.MasterReferences => this.ModHeader.MasterReferences;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IMasterReferenceGetter> IModGetter.MasterReferences => this.ModHeader.MasterReferences;

        public ModKey ModKey { get; } = ModKey.Null;

        public SkyrimMod(ModKey modKey)
            : this()
        {
            this.ModKey = modKey;
        }

        public FormKey GetNextFormKey()
        {
            return new FormKey(
                this.ModKey,
                this.ModHeader.Stats.NextObjectID++);
        }
    }

    namespace Internals
    {
        public partial class SkyrimModBinaryWriteTranslation
        {
            public static void WriteModHeader(
                IModHeaderGetter header,
                MutagenWriter writer,
                ModKey modKey,
                MasterReferenceReader masterReferences)
            {
                var modHeader = header.DeepCopy() as ModHeader;
                modHeader.Flags = modHeader.Flags.SetFlag(ModHeader.HeaderFlag.Master, modKey.Master);
                modHeader.WriteToBinary(
                    writer: writer,
                    masterReferences: masterReferences);
            }
        }
    }
}
