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
    public partial class SkyrimMod : AMod
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IList<MasterReference> IMod.MasterReferences => this.ModHeader.MasterReferences;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IMasterReferenceGetter> IModGetter.MasterReferences => this.ModHeader.MasterReferences;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        uint IMod.NextObjectID
        {
            get => this.ModHeader.Stats.NextObjectID;
            set => this.ModHeader.Stats.NextObjectID = value;
        }
    }

    namespace Internals
    {
        public partial class SkyrimModBinaryWriteTranslation
        {
            public static void WriteModHeader(
                ISkyrimModGetter mod,
                MutagenWriter writer,
                ModKey modKey,
                BinaryWriteParameters param)
            {
                var modHeader = mod.ModHeader.DeepCopy() as ModHeader;
                modHeader.Flags = modHeader.Flags.SetFlag(ModHeader.HeaderFlag.Master, modKey.Master);
                param ??= BinaryWriteParameters.Default;
                HashSet<ModKey> modKeys = new HashSet<ModKey>(); 
                switch (param.MastersListSync)
                {
                    case BinaryWriteParameters.MastersListSyncOption.NoCheck:
                        modKeys.Add(modHeader.MasterReferences.Select(m => m.Master));
                        break;
                    case BinaryWriteParameters.MastersListSyncOption.Iterate:
                        modKeys.Add(mod.Links.SelectWhere(l =>
                        {
                            if (l.TryGetModKey(out var modKey))
                            {
                                return TryGet<ModKey>.Succeed(modKey);
                            }
                            return TryGet<ModKey>.Failure;
                        }));
                        break;
                    default:
                        throw new NotImplementedException();
                }
                modHeader.WriteToBinary(writer);
            }
        }
    }
}
