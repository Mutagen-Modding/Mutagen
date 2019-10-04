using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using ReactiveUI;
using System.Reactive.Linq;
using Noggog;
using Noggog.Notifying;
using CSharpExt.Rx;
using DynamicData;
using System.Collections.ObjectModel;
using System.Threading;
using Loqui.Internal;
using Mutagen.Bethesda.Internals;
using System.IO;
using System.Xml.Linq;
using Loqui.Xml;
using Mutagen.Bethesda.Skyrim.Internals;

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
        IList<MasterReference> IMod.MasterReferences => this.ModHeader.MasterReferences;
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
