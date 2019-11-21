using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public class LinkingPackage<TMod>
        where TMod : IModGetter
    {
        public TMod SourceMod { get; private set; }
        private IReadOnlyDictionary<FormKey, IMajorRecordCommonGetter> _sourceModMajorRecords;
        private readonly IReadOnlyDictionary<FormKey, IMajorRecordCommonGetter>[] _modListMajorRecords;
        public ModList<TMod> ModList { get; }

        public LinkingPackage(TMod sourceMod, ModList<TMod> modList)
        {
            this.SourceMod = sourceMod;
            this.ModList = modList;
            if (this.ModList != null)
            {
                this._modListMajorRecords = new IReadOnlyDictionary<FormKey, IMajorRecordCommonGetter>[this.ModList.Count];
            }
        }

        public LinkingPackage(ModList<TMod> modList)
            : this(default, modList)
        {
        }

        public bool TryGetMajorRecords(ModKey modKey, out IReadOnlyDictionary<FormKey, IMajorRecordCommonGetter> dict)
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

        public void SetSourceMod(TMod sourceMod)
        {
            this.SourceMod = sourceMod;
            this._sourceModMajorRecords = null;
        }

        public IReadOnlyDictionary<FormKey, IMajorRecordCommonGetter> GetMajorRecords(ModID modIndex)
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

        private IReadOnlyDictionary<FormKey, IMajorRecordCommonGetter> GetDictionary(TMod mod)
        {
            Dictionary<FormKey, IMajorRecordCommonGetter> majorRecords = new Dictionary<FormKey, IMajorRecordCommonGetter>();
            foreach (var majorRec in mod.EnumerateMajorRecords())
            {
                majorRecords[majorRec.FormKey] = majorRec;
            }
            return majorRecords;
        }
    }
}
