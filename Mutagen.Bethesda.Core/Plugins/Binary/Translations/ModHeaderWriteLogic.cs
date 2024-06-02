using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Utility;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public sealed class ModHeaderWriteLogic
{
    private readonly List<Action<IMajorRecordGetter>> _recordIterationActions = new();
    private readonly List<Action<FormKey, IFormLinkGetter>> _formLinkIterationActions = new();
    private readonly BinaryWriteParameters _params;

    private readonly ModKey _modKey;
    private readonly Dictionary<ModKey, FormKey?> _modKeys = new();
    private uint _numRecords;
    private uint? _nextFormID;
    private uint _uniqueRecordsFromMod;
    private readonly HashSet<FormKey> _formKeyUniqueness = new();
    private readonly GameCategory _category;
    private FormKey? _disallowedFormKey;
    private uint _higherFormIDRange;
    private GameConstants _constants;

    private ModHeaderWriteLogic(
        BinaryWriteParameters? param,
        IModGetter mod)
    {
        _params = param ?? BinaryWriteParameters.Default;
        _modKey = mod.ModKey;
        _category = mod.GameRelease.ToCategory();
        _constants = GameConstants.Get(mod.GameRelease);
        _higherFormIDRange = _constants.DefaultHighRangeFormID;
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
            mod: mod);
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
        AddLightFormLimit(modHeader);
        AddCompressionCheck();
        AddDisallowedLowerFormIDs();
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

                if (_formLinkIterationActions.Count > 0)
                {
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
            Master = m,
            FileSize = mod.GameRelease.ToCategory().IncludesMasterReferenceDataSubrecords() ? 0 : null
        }));
        return ret;
    }

    private void PostProcessAdjustments(
        MutagenWriter writer,
        IModGetter mod,
        IModHeaderCommon modHeader)
    {
        HandleDisallowedLowerFormIDs();
        writer.MetaData.MasterReferences = ConstructWriteMasters(mod);
        modHeader.MasterReferences.SetTo(writer.MetaData.MasterReferences!.Masters.Select(m => m.DeepCopy()));
        if (_params.RecordCount != RecordCountOption.NoCheck)
        {
            modHeader.NumRecords = _numRecords;
        }
        if (_params.NextFormID != NextFormIDOption.NoCheck)
        {
            bool? forceLowerBound = null;
            if (_params.MinimumFormID is ForceLowerFormIdRangeOption force)
            {
                forceLowerBound = force.ForceLowerRangeSetting;
            }
            modHeader.NextFormID = _nextFormID.HasValue ? _nextFormID.Value + 1 : mod.MinimumCustomFormID(forceLowerBound);
        }

        var lightIndex = _category.GetLightFlagIndex();
        if (lightIndex.HasValue 
            && Enums.HasFlag(modHeader.RawFlags, lightIndex.Value)
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
                _modKeys.Set(mod.MasterReferences.Select(m => new KeyValuePair<ModKey, FormKey?>(m.Master, FormKey.Null)));
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
                    if (!_nextFormID.HasValue || fk.ID > _nextFormID)
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
                    if (ex is not ArgumentOutOfRangeException && ex is not InvalidOperationException) throw;
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
                header.RawFlags = Enums.SetFlag(header.RawFlags, _category.GetMasterFlagIndex(), _modKey.Type == ModType.Master);
                if (_modKey.Type != ModType.Plugin)
                {
                    header.RawFlags = Enums.SetFlag(header.RawFlags, _category.GetMasterFlagIndex(), true);
                }
                break;
            case MasterFlagOption.ExceptionOnMismatch:
                if ((_modKey.Type == ModType.Master) != Enums.HasFlag(header.RawFlags, _category.GetMasterFlagIndex()))
                {
                    throw new ArgumentException($"Master flag did not match ModKey type. ({_modKey})");
                }

                var lightIndex = _category.GetLightFlagIndex();
                if (lightIndex.HasValue && (_modKey.Type == ModType.Light) != Enums.HasFlag(header.RawFlags, lightIndex.Value))
                {
                    throw new ArgumentException($"Light flag did not match ModKey type. ({_modKey})");
                }
                
                break;
            default:
                break;
        }
    }
    #endregion

    #region Light Master Form Limit
    private void AddLightFormLimit(IModHeaderCommon header)
    {
        var lightIndex = _category.GetLightFlagIndex();
        if (!lightIndex.HasValue || !Enums.HasFlag(header.RawFlags, lightIndex.Value)) return;
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

    #region DisallowedLowerFormIDs

    private void AddDisallowedLowerFormIDs()
    {
        switch (_params.LowerRangeDisallowedHandler)
        {
            case NoCheckIfLowerRangeDisallowed:
                break;
            default:
                _recordIterationActions.Add(maj =>
                {
                    if (maj.FormKey.ModKey != _modKey) return;
                    if (_higherFormIDRange > maj.FormKey.ID)
                    {
                        _disallowedFormKey = maj.FormKey;
                    }
                });
                break;
        }
    }

    private void HandleDisallowedLowerFormIDs()
    {
        if (_numRecords == 0) return;
        if (_disallowedFormKey == null) return;

        switch (_params.LowerRangeDisallowedHandler)
        {
            case NoCheckIfLowerRangeDisallowed:
                return;
            case ThrowIfLowerRangeDisallowed:
                throw new LowerFormKeyRangeDisallowedException(_disallowedFormKey.Value);
            case AddPlaceholderMasterIfLowerRangeDisallowed placeholder:
                if (placeholder.ModKey == null)
                {
                    throw new LowerFormKeyRangeDisallowedException(_disallowedFormKey.Value);
                }
                _modKeys[placeholder.ModKey.Value] = null;
                break;
        }
    }

    #endregion
}