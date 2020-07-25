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
        private readonly List<Action<FormKey>> _formLinkIterationActions = new List<Action<FormKey>>();
        private readonly BinaryWriteParameters _params;

        private readonly ModKey _modKey;
        private readonly HashSet<ModKey> _modKeys = new HashSet<ModKey>();
        private uint _numRecords;
        private uint _nextFormID;
        private readonly HashSet<FormKey> _formKeyUniqueness = new HashSet<FormKey>();

        private ModHeaderWriteLogic(
            BinaryWriteParameters? param,
            IModGetter mod,
            IModHeaderCommon modHeader)
        {
            _params = param ?? BinaryWriteParameters.Default;
            _modKey = mod.ModKey;
            _nextFormID = modHeader.MinimumCustomFormID;
            AddMasterCollectionActions(mod);
            AddRecordCount();
            AddNextFormIDActions();
            AddFormIDUniqueness();

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
                            formLinkAction(formKey);
                        }
                    }
                }
            }
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
            writer.MetaData.MasterReferences = modHeaderWriter.ConstructWriteMasters(mod);
            modHeaderWriter.WriteModHeader(
                modHeader: modHeader,
                writer: writer,
                modKey: modKey);
        }

        #region Master Content Sync Logic
        private void AddMasterCollectionActions(IModGetter mod)
        {
            switch (_params.MastersListContent)
            {
                case BinaryWriteParameters.MastersListContentOption.NoCheck:
                    _modKeys.Add(mod.MasterReferences.Select(m => m.Master));
                    break;
                case BinaryWriteParameters.MastersListContentOption.Iterate:
                    _recordIterationActions.Add(maj => _modKeys.Add(maj.FormKey.ModKey));
                    _formLinkIterationActions.Add(formLink => _modKeys.Add(formLink.ModKey));
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
                    modKeys.Sort(ModKey.LoadOrderComparer(lo.LoadOrder));
                    return;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        private MasterReferenceReader ConstructWriteMasters(IModGetter mod)
        {
            MasterReferenceReader ret = new MasterReferenceReader(mod.ModKey);
            _modKeys.Remove(mod.ModKey);
            _modKeys.Remove(ModKey.Null);
            var modKeysList = _modKeys.ToList();
            SortMasters(modKeysList);
            ret.SetTo(modKeysList.Select(m => new MasterReference()
            {
                Master = m
            }));
            return ret;
        }

        private void WriteModHeader(
            IModHeaderCommon modHeader,
            MutagenWriter writer,
            ModKey modKey)
        {
            modHeader.RawFlags = EnumExt.SetFlag(modHeader.RawFlags, (int)ModHeaderCommonFlag.Master, modKey.Type == ModType.Master);
            modHeader.MasterReferences.SetTo(writer.MetaData.MasterReferences!.Masters.Select(m => m.DeepCopy()));
            if (_params.RecordCount != BinaryWriteParameters.RecordCountOption.NoCheck)
            {
                modHeader.NumRecords = _numRecords;
            }
            if (_params.NextFormID != BinaryWriteParameters.NextFormIDOption.NoCheck)
            {
                modHeader.NextFormID = _nextFormID + 1;
            }
            modHeader.WriteToBinary(writer);
        }
    }
}
