using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class ModList<Mod> : IEnumerable<Mod>
        where Mod : IMod
    {
        private readonly Dictionary<string, ModID> _modsByName = new Dictionary<string, ModID>();
        private readonly List<Mod> _modsByLoadOrder = new List<Mod>();

        public bool TryGetModByName(string name, out (ModID Index, Mod Mod) result)
        {
            if (!_modsByName.TryGetValue(name, out var index))
            {
                result = default(ValueTuple<ModID, Mod>);
                return false;
            }
            result = (index, _modsByLoadOrder[index.ID]);
            return true;
        }

        public bool TryGetMod(ModID index, out Mod result)
        {
            if (!_modsByLoadOrder.InRange(index.ID))
            {
                result = default(Mod);
                return false;
            }
            result = _modsByLoadOrder[index.ID];
            return true;
        }

        private void Link()
        {
            foreach (var mod in this._modsByLoadOrder)
            {
                foreach (var link in mod.Links)
                {
                    if (link.Linked) continue;
                    link.Link(this, mod);
                }
            }
        }

        public IEnumerator<Mod> GetEnumerator()
        {
            return _modsByLoadOrder.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
