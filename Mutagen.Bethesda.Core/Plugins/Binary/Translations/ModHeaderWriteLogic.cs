using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Masters;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public class ModHeaderWriteLogic
{
    private readonly List<Action<IMajorRecordGetter>> _recordIterationActions = new();
    private readonly List<Action<FormKey, IFormLinkGetter>> _formLinkIterationActions = new();
    private readonly BinaryWriteParameters _params;

    private readonly ModKey _modKey;
    private readonly Dictionary<ModKey, FormKey> _modKeys = new();
    private uint _numRecords;
    private uint _nextFormID;
    private uint _uniqueRecordsFromMod;
    private readonly HashSet<FormKey> _formKeyUniqueness = new();

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
        AddCompressionCheck();
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
                foreach (var linkInfo in maj.EnumerateFormLinks())
                {
                    foreach (var formLinkAction in _formLinkIterationActions)
                    {
                        formLinkAction(maj.FormKey, linkInfo);
                    }
                }
            }
        }
    }

    private IReadOnlyMasterReferenceCollection ConstructWriteMasters(IModGetter mod)
    {
        MasterReferenceCollection ret = new MasterReferenceCollection(mod.ModKey);
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
        if (_params.RecordCount != RecordCountOption.NoCheck)
        {
            modHeader.NumRecords = _numRecords;
        }
        if (_params.NextFormID != NextFormIDOption.NoCheck)
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
            case MastersListContentOption.NoCheck:
                _modKeys.Set(mod.MasterReferences.Select(m => new KeyValuePair<ModKey, FormKey>(m.Master, FormKey.Null)));
                break;
            case MastersListContentOption.Iterate:
                _recordIterationActions.Add(maj =>
                {
                    var formKey = maj.FormKey;
                    if (mod.ModKey == formKey.ModKey) return;
                    if (_params.CleanNulls && formKey.IsNull) return;
                    _modKeys[formKey.ModKey] = formKey;
                });
                _formLinkIterationActions.Add((maj, formLink) =>
                {
                    if (mod.ModKey == formLink.FormKey.ModKey) return;
                    if (_params.CleanNulls && formLink.FormKey.IsNull) return;
                    _modKeys[formLink.FormKey.ModKey] = maj;
                });
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
            case RecordCountOption.NoCheck:
                break;
            case RecordCountOption.Iterate:
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
            case FormIDUniquenessOption.NoCheck:
                break;
            case FormIDUniquenessOption.Iterate:
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
            case NextFormIDOption.NoCheck:
                break;
            case NextFormIDOption.Iterate:
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
            case MastersListOrderingEnumOption e:
                switch (e.Option)
                {
                    case MastersListOrderingOption.NoCheck:
                        return;
                    case MastersListOrderingOption.MastersFirst:
                        modKeys.Sort(ModKey.AlphabeticalAndMastersFirst);
                        return;
                    default:
                        throw new NotImplementedException();
                }
            case MastersListOrderingByLoadOrder lo:
                try
                {
                    modKeys.Sort(ModKey.LoadOrderComparer(lo.LoadOrder));
                }
                catch (Exception ex)
                {
                    if (!(ex is ArgumentOutOfRangeException) && !(ex is InvalidOperationException)) throw;
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
            case MasterFlagOption.NoCheck:
                break;
            case MasterFlagOption.ChangeToMatchModKey:
                header.RawFlags = EnumExt.SetFlag(header.RawFlags, (int)ModHeaderCommonFlag.Master, _modKey.Type == ModType.Master);
                if (_modKey.Type != ModType.Plugin)
                {
                    header.RawFlags = EnumExt.SetFlag(header.RawFlags, (int)ModHeaderCommonFlag.Master, true);
                }
                break;
            case MasterFlagOption.ExceptionOnMismatch:
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

    #region Compression Check

    private void AddCompressionCheck()
    {
        _recordIterationActions.Add(maj =>
        {
            if (maj.IsCompressed)
            {
                throw new NotImplementedException(
                    "Writing with compression enabled is not currently supported.  https://github.com/Mutagen-Modding/Mutagen/issues/235");
            }
        });
    }

    #endregion
}