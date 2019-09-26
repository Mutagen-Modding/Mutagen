using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public class LinkingPackage<M>
        where M : IMod
    {
        public M SourceMod { get; private set; }
        private IReadOnlyDictionary<FormKey, IMajorRecordCommon> _sourceModMajorRecords;
        private readonly IReadOnlyDictionary<FormKey, IMajorRecordCommon>[] _modListMajorRecords;
        public ModList<M> ModList { get; }

        public LinkingPackage(M sourceMod, ModList<M> modList)
        {
            this.SourceMod = sourceMod;
            this.ModList = modList;
            if (this.ModList != null)
            {
                this._modListMajorRecords = new IReadOnlyDictionary<FormKey, IMajorRecordCommon>[this.ModList.Count];
            }
        }

        public LinkingPackage(ModList<M> modList)
            : this(default, modList)
        {
        }

        public bool TryGetMajorRecords(ModKey modKey, out IReadOnlyDictionary<FormKey, IMajorRecordCommon> dict)
        {
            if (this.ModList != null && this.ModList.TryGetMod(modKey, out var mod))
            {
                dict = GetMajorRecords(mod.Index);
                return true;
            }
            else if (modKey == SourceMod.ModKey)
            {
                if (_sourceModMajorRecords == null)
                {
                    _sourceModMajorRecords = GetDictionary(SourceMod);
                }
                dict = _sourceModMajorRecords;
                return true;
            }
            dict = default;
            return false;
        }

        public void SetSourceMod(M sourceMod)
        {
            this.SourceMod = sourceMod;
            this._sourceModMajorRecords = null;
        }

        public IReadOnlyDictionary<FormKey, IMajorRecordCommon> GetMajorRecords(ModID modIndex)
        {
            if (!_modListMajorRecords.InRange(modIndex.ID))
            {
                throw new ArgumentException();
            }
            if (_modListMajorRecords[modIndex.ID] == null)
            {
                _modListMajorRecords[modIndex.ID] = GetDictionary(this.ModList.GetIndex(modIndex).Mod);
            }
            return _modListMajorRecords[modIndex.ID];
        }

        private IReadOnlyDictionary<FormKey, IMajorRecordCommon> GetDictionary(M mod)
        {
            Dictionary<FormKey, IMajorRecordCommon> majorRecords = new Dictionary<FormKey, IMajorRecordCommon>();
            foreach (var majorRec in mod.MajorRecords)
            {
                majorRecords[majorRec.Key] = majorRec.Value;
            }
            return majorRecords;
        }
    }
}
