using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Core;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Internals
{
    public class ModHeaderWriteLogic
    {
        private readonly List<Action<IMajorRecordCommonGetter>> _recordIterationActions = new List<Action<IMajorRecordCommonGetter>>();
        private readonly List<Action<FormKey, FormKey>> _formLinkIterationActions = new List<Action<FormKey, FormKey>>();
        private readonly BinaryWriteParameters _params;

        private readonly ModKey _modKey;
        private readonly Dictionary<ModKey, FormKey> _modKeys = new Dictionary<ModKey, FormKey>();
        private uint _numRecords;
        private uint _nextFormID;
        private uint _uniqueRecordsFromMod;
        private readonly HashSet<FormKey> _formKeyUniqueness = new HashSet<FormKey>();

        private ModHeaderWriteLogic(
            BinaryWriteParameters? param,
            IModGetter mod,
            IModHeaderCommon modHeader)
        {
            _params = param ?? BinaryWriteParameters.Default;
            _modKey = mod.ModKey;
            _nextFormID = modHeader.MinimumCustomFormID;
        }

        public static void WriteHeader(
            BinaryWriteParameters? param,
            MutagenWriter writer,
            IModGetter mod,
            IModHeaderCommon modHeader,
            ModKey modKey)
        {
            var modHeaderWriter = new ModHeaderWriteLogic(
                param: param,
                mod: mod,
                modHeader: modHeader);
            modHeaderWriter.AddProcessors(mod, modHeader);
            modHeaderWriter.RunProcessors(mod);
            modHeaderWriter.PostProcessAdjustments(writer, mod, modHeader);
            modHeader.WriteToBinary(writer);
        }

        private void AddProcessors(
            IModGetter mod, 
            IModHeaderCommon modHeader)
        {
            ModifyMasterFlags(modHeader);
            AddMasterCollectionActions(mod);
            AddRecordCount();
            AddNextFormIDActions();
            AddFormIDUniqueness();
            AddLightMasterFormLimit(modHeader);
        }

        private void RunProcessors(IModGetter mod)
        {
            // Do any major record iteration work
            if (_recordIterationActions.Count > 0
                || _formLinkIterationActions.Count > 0)
            {
                foreach (var maj in mod.EnumerateMajorRecords())
                {
                    foreach (var majAction in _recordIterationActions)
                    {
                        majAction(maj);
                    }
                    foreach (var formKey in maj.LinkFormKeys)
                    {
                        foreach (var formLinkAction in _formLinkIterationActions)
                        {
                            formLinkAction(maj.FormKey, formKey);
                        }
                    }
                }
            }
        }

        private MasterReferenceReader ConstructWriteMasters(IModGetter mod)
        {
            MasterReferenceReader ret = new MasterReferenceReader(mod.ModKey);
            _modKeys.Remove(mod.ModKey);
            _modKeys.Remove(ModKey.Null);
            var modKeysList = _modKeys.Keys.ToList();
            SortMasters(modKeysList);
            ret.SetTo(modKeysList.Select(m => new MasterReference()
            {
                Master = m
            }));
            return ret;
        }

        private void PostProcessAdjustments(
            MutagenWriter writer,
            IModGetter mod,
            IModHeaderCommon modHeader)
        {
            writer.MetaData.MasterReferences = ConstructWriteMasters(mod);
            modHeader.MasterReferences.SetTo(writer.MetaData.MasterReferences!.Masters.Select(m => m.DeepCopy()));
            if (_params.RecordCount != BinaryWriteParameters.RecordCountOption.NoCheck)
            {
                modHeader.NumRecords = _numRecords;
            }
            if (_params.NextFormID != BinaryWriteParameters.NextFormIDOption.NoCheck)
            {
                modHeader.NextFormID = _nextFormID + 1;
            }
            if (EnumExt.HasFlag(modHeader.RawFlags, (int)ModHeaderCommonFlag.LightMaster)
                && _uniqueRecordsFromMod > Constants.LightMasterLimit)
            {
                throw new ArgumentException($"Light Master Mod contained more originating records than allowed. {_uniqueRecordsFromMod} > {Constants.LightMasterLimit}");
            }
        }

        #region Master Content Sync Logic
        private void AddMasterCollectionActions(IModGetter mod)
        {
            switch (_params.MastersListContent)
            {
                case BinaryWriteParameters.MastersListContentOption.NoCheck:
                    _modKeys.Set(mod.MasterReferences.Select(m => new KeyValuePair<ModKey, FormKey>(m.Master, FormKey.Null)));
                    break;
                case BinaryWriteParameters.MastersListContentOption.Iterate:
                    _recordIterationActions.Add(maj => _modKeys[maj.FormKey.ModKey] = maj.FormKey);
                    _formLinkIterationActions.Add((maj, formLink) => _modKeys[formLink.ModKey] = maj);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Record Count Logic
        private void AddRecordCount()
        {
            switch (_params.RecordCount)
            {
                case BinaryWriteParameters.RecordCountOption.NoCheck:
                    break;
                case BinaryWriteParameters.RecordCountOption.Iterate:
                    _recordIterationActions.Add(maj => _numRecords++);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region FormID Uniqueness Logic
        private void AddFormIDUniqueness()
        {
            switch (_params.FormIDUniqueness)
            {
                case BinaryWriteParameters.FormIDUniquenessOption.NoCheck:
                    break;
                case BinaryWriteParameters.FormIDUniquenessOption.Iterate:
                    _recordIterationActions.Add(maj =>
                    {
                        if (!_formKeyUniqueness.Add(maj.FormKey))
                        {
                            throw new ArgumentException($"Two records with the same FormKey were encountered: {maj.FormKey}");
                        }
                    });
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region Next Form ID
        private void AddNextFormIDActions()
        {
            switch (_params.NextFormID)
            {
                case BinaryWriteParameters.NextFormIDOption.NoCheck:
                    break;
                case BinaryWriteParameters.NextFormIDOption.Iterate:
                    _recordIterationActions.Add(maj =>
                    {
                        var fk = maj.FormKey;
                        if (fk.ModKey != _modKey) return;
                        if (fk.ID > _nextFormID)
                        {
                            _nextFormID = fk.ID;
                        }
                    });
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region Master Order Sync Logic
        private void SortMasters(List<ModKey> modKeys)
        {
            switch (_params.MastersListOrdering)
            {
                case BinaryWriteParameters.MastersListOrderingEnumOption e:
                    switch (e.Option)
                    {
                        case BinaryWriteParameters.MastersListOrderingOption.NoCheck:
                            return;
                        case BinaryWriteParameters.MastersListOrderingOption.MastersFirst:
                            modKeys.Sort(ModKey.AlphabeticalAndMastersFirst);
                            return;
                        default:
                            throw new NotImplementedException();
                    }
                case BinaryWriteParameters.MastersListOrderingByLoadOrder lo:
                    try
                    {
                        modKeys.Sort(ModKey.LoadOrderComparer(lo.LoadOrder));
                    }
                    catch (InvalidOperationException)
                    {
                        var keys = modKeys.ToHashSet();
                        keys.Remove(lo.LoadOrder);
                        var modToComplainAbout = keys.First();
                        throw new MissingModException(modToComplainAbout, $"A referenced mod was not present on the load order being sorted against: {modToComplainAbout}.  This mod was referenced by MajorRecord: {_modKeys[modToComplainAbout]}");
                    }
                    return;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region Master Flags
        public void ModifyMasterFlags(IModHeaderCommon header)
        {
            switch (_params.MasterFlag)
            {
                case BinaryWriteParameters.MasterFlagOption.NoCheck:
                    break;
                case BinaryWriteParameters.MasterFlagOption.ChangeToMatchModKey:
                    header.RawFlags = EnumExt.SetFlag(header.RawFlags, (int)ModHeaderCommonFlag.Master, _modKey.Type == ModType.Master);
                    if (_modKey.Type != ModType.Plugin)
                    {
                        header.RawFlags = EnumExt.SetFlag(header.RawFlags, (int)ModHeaderCommonFlag.Master, true);
                    }
                    break;
                case BinaryWriteParameters.MasterFlagOption.ExceptionOnMismatch:
                    if ((_modKey.Type == ModType.Master) != EnumExt.HasFlag(header.RawFlags, (int)ModHeaderCommonFlag.Master))
                    {
                        throw new ArgumentException($"Master flag did not match ModKey type. ({_modKey})");
                    }
                    if ((_modKey.Type == ModType.LightMaster) != EnumExt.HasFlag(header.RawFlags, (int)ModHeaderCommonFlag.LightMaster))
                    {
                        throw new ArgumentException($"LightMaster flag did not match ModKey type. ({_modKey})");
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Light Master Form Limit
        private void AddLightMasterFormLimit(IModHeaderCommon header)
        {
            if (!EnumExt.HasFlag(header.RawFlags, (int)ModHeaderCommonFlag.LightMaster)) return;
            _recordIterationActions.Add(maj =>
            {
                if (maj.FormKey.ModKey == _modKey)
                {
                    _uniqueRecordsFromMod++;
                }
            });
        }
        #endregion
    }
}
