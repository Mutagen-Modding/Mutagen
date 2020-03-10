using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Internals
{
    public class MasterReferenceReader
    {
        public static MasterReferenceReader Empty { get; } = new MasterReferenceReader(ModKey.Null);

        private Dictionary<ModKey, ModID> _masterIndices = new Dictionary<ModKey, ModID>();
        public IReadOnlyList<IMasterReferenceGetter> Masters { get; private set; } = ListExt.Empty<IMasterReferenceGetter>();
        public ModKey CurrentMod;

        public MasterReferenceReader(ModKey modKey)
        {
            this.CurrentMod = modKey;
        }

        public MasterReferenceReader(ModKey modKey, IEnumerable<IMasterReferenceGetter> masters)
        {
            this.CurrentMod = modKey;
            SetTo(masters);
        }

        public void SetTo(IEnumerable<IMasterReferenceGetter> masters)
        {
            this.Masters = new List<IMasterReferenceGetter>(masters);
            this._masterIndices.Clear();

            byte index = 0;
            foreach (var master in masters)
            {
                var modKey = master.Master;
                if (this._masterIndices.ContainsKey(modKey))
                {
                    throw new ArgumentException($"Masters list cannot contain duplicate entries: {modKey}");
                }
                if (index >= 0xFE)
                {
                    throw new ArgumentException($"Too many masters on masters list");
                }
                if (modKey == this.CurrentMod)
                {
                    throw new ArgumentException($"Cannot add mod to itself as a master: {this.CurrentMod}");
                }
                this._masterIndices[modKey] = new ModID(index++);
            }

            // Add current mod
            this._masterIndices[this.CurrentMod] = new ModID(index);
        }

        public FormID GetFormID(FormKey key)
        {
            if (this._masterIndices.TryGetValue(key.ModKey, out var index))
            {
                return new FormID(
                    index,
                    key.ID);
            }
            throw new ArgumentException($"Could not map FormKey to a master index: {key}");
        }
    }
}
