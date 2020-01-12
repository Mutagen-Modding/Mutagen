using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Mutagen.Bethesda.Skyrim
{
    partial interface ISkyrimMod : IMod
    {
    }

    partial interface ISkyrimModGetter : IModGetter
    {
    }

    public partial class SkyrimMod
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IList<MasterReference> IMod.MasterReferences => this.ModHeader.MasterReferences;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IMasterReferenceGetter> IModGetter.MasterReferences => this.ModHeader.MasterReferences;

        public ModKey ModKey { get; }

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
}
