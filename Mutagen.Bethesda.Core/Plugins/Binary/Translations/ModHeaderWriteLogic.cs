using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

internal sealed class ModHeaderWriteLogic
{
    private readonly List<Action<IMajorRecordGetter>> _recordIterationActions = new();
    private readonly List<Action<IModContext<IMajorRecordGetter>>> _recordContextIterationActions = new();
    private readonly List<Action<FormKey, IFormLinkGetter>> _formLinkIterationActions = new();
    private readonly BinaryWriteParameters _params;
    private static readonly RecordCompactionCompatibilityDetector _recordCompactionDetector = new();

    private readonly ModKey _modKey;
    private readonly Dictionary<ModKey, FormKey?> _modKeys = new();
    private uint _numRecords;
    private uint? _nextFormID;
    private readonly HashSet<FormKey> _formKeyUniqueness = new();
    private readonly GameCategory _category;
    private FormKey? _disallowedLowerFormKey;
    private uint _higherFormIDRange;
    private GameConstants _constants;
    private readonly IModGetter _mod;
    private readonly HashSet<FormKey> _overriddenForms = new();
    private RangeUInt32? _recordRange;

    private ModHeaderWriteLogic(
        BinaryWriteParameters param,
        IModGetter mod)
    {
        _mod = mod;
        _params = param;
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
        param ??= BinaryWriteParameters.Default;
        var modHeaderWriter = new ModHeaderWriteLogic(
            param: param,
            mod: mod);
        modHeaderWriter.AddProcessors(mod, modHeader);
        modHeaderWriter.RunProcessors(mod);
        modHeaderWriter.PostProcessAdjustments(writer, mod, modHeader, 
            modHeaderWriter._constants.SeparateMasterLoadOrders
                ? param.MasterFlagsLookup 
                : null);
        modHeader.WriteToBinary(writer);
    }

    private void AddProcessors(
        IModGetter mod, 
        IModHeaderCommon modHeader)
    {
        AddMasterCollectionActions(mod);
        AddRecordCount();
        AddNextFormIDActions();
        AddFormIDUniqueness();
        AddFormIDCompactionLogic();
        AddCompactionTracking(modHeader);
        AddDisallowedLowerFormIDs();
        RegisterOverriddenFormsFishing();
    }

    private void RunProcessors(IModGetter mod)
    {
        // Do any major record iteration work
        if (_recordIterationActions.Count > 0
            || _formLinkIterationActions.Count > 0
            || _recordContextIterationActions.Count > 0)
        {
            if (_recordContextIterationActions.Count > 0)
            {
                foreach (var context in mod.EnumerateMajorRecordSimpleContexts())
                {
                    RunProcessorsOnModContexts(context);
                }
            }
            else
            {
                foreach (var maj in mod.EnumerateMajorRecords())
                {
                    RunProcessorsOnMajorRecord(maj);
                }
            }
        }
    }

    private void RunProcessorsOnMajorRecord(IMajorRecordGetter maj)
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

    private void RunProcessorsOnModContexts(IModContext<IMajorRecordGetter> context)
    {
        RunProcessorsOnMajorRecord(context.Record);
        foreach (var majAction in _recordContextIterationActions)
        {
            majAction(context);
        }
    }

    private IReadOnlyMasterReferenceCollection ConstructWriteMasters(IModGetter mod)
    {
        MasterReferenceCollection ret = new MasterReferenceCollection(mod.ModKey);
        _modKeys.Remove(mod.ModKey);
        _modKeys.Remove(ModKey.Null);
        var modKeysList = _modKeys.Keys.ToList();
        SortMasters(modKeysList);
        if (_params.MastersContentCustomOverride != null)
        {
            modKeysList = _params.MastersContentCustomOverride(modKeysList).ToList();
            SortMasters(modKeysList);
        }
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
        IModHeaderCommon modHeader,
        IReadOnlyCache<IModMasterStyledGetter, ModKey>? masterFlagLookup)
    {
        HandleDisallowedLowerFormIDs(); 
        SetOutgoingMasters(writer, mod, modHeader, masterFlagLookup);
        SetNumRecords(mod, modHeader);
        SetNextFormID(mod, modHeader);
        SetOverriddenForms(modHeader);
    }
    
    private void SetOutgoingMasters(MutagenWriter writer, IModGetter mod, IModHeaderCommon modHeader,
        IReadOnlyCache<IModMasterStyledGetter, ModKey>? masterFlagLookup)
    {
        writer.MetaData.MasterReferences = ConstructWriteMasters(mod);
        writer.MetaData.SeparatedMasterPackage = SeparatedMasterPackage.Factory(
            mod.GameRelease,
            mod.ModKey,
            mod.GetMasterStyle(),
            writer.MetaData.MasterReferences,
            masterFlagLookup);
        
        modHeader.MasterReferences.SetTo(writer.MetaData.MasterReferences!.Masters.Select(m => m.DeepCopy()));
    }

    private void SetNextFormID(IModGetter mod, IModHeaderCommon modHeader)
    {
        if (_params.NextFormID != NextFormIDOption.NoCheck)
        {
            bool? forceLowerBound = null;
            if (_params.MinimumFormID is ForceLowerFormIdRangeOption force)
            {
                forceLowerBound = force.ForceLowerRangeSetting;
            }
            modHeader.NextFormID = _nextFormID.HasValue ? _nextFormID.Value + 1 : mod.GetDefaultInitialNextFormID(forceLowerBound);
        }
    }

    private void SetNumRecords(IModGetter mod, IModHeaderCommon modHeader)
    {
        if (_params.RecordCount != RecordCountOption.NoCheck)
        {
            // Can't use raw count, as more gets considered in this tally
            modHeader.NumRecords = mod.GetRecordCount();
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

    #region FormID Compaction Logic
    private void AddFormIDCompactionLogic()
    {
        switch (_params.FormIDCompaction)
        {
            case FormIDCompactionOption.NoCheck:
                break;
            case FormIDCompactionOption.Iterate:
            {
                var formIdRange = _recordCompactionDetector.GetAllowedRange(_mod, potential: true);
                if (formIdRange != null)
                {
                    _recordIterationActions.Add(maj =>
                    {
                        _recordCompactionDetector.ThrowIfIncompatible(_mod, formIdRange.Value, maj);
                    });
                }
                break;
            }
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
            case null:
                modKeys.Sort(ModKey.AlphabeticalAndMastersFirst);
                return;
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

    #region Compaction
    private void AddCompactionTracking(IModHeaderCommon header)
    {
        _recordIterationActions.Add(maj =>
        {
            if (maj.FormKey.ModKey == _modKey)
            {
                if (_recordRange == null)
                {
                    _recordRange = new RangeUInt32(maj.FormKey.ID);
                }
                else if (maj.FormKey.ID > _recordRange.Value.Max)
                {
                    _recordRange = new RangeUInt32(_recordRange.Value.Min, maj.FormKey.ID);
                }
                else if (maj.FormKey.ID < _recordRange.Value.Min)
                {
                    _recordRange = new RangeUInt32(maj.FormKey.ID, _recordRange.Value.Max);
                }
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
                        _disallowedLowerFormKey = maj.FormKey;
                    }
                });
                break;
        }
    }

    private void HandleDisallowedLowerFormIDs()
    {
        if (_numRecords == 0) return;
        if (_disallowedLowerFormKey == null) return;

        switch (_params.LowerRangeDisallowedHandler)
        {
            case NoCheckIfLowerRangeDisallowed:
                return;
            case ThrowIfLowerRangeDisallowed:
                throw new LowerFormKeyRangeDisallowedException(_disallowedLowerFormKey.Value);
            case AddPlaceholderMasterIfLowerRangeDisallowed placeholder:
                if (_modKeys.Count > 0) return;
                if (placeholder.ModKey == null)
                {
                    throw new LowerFormKeyRangeDisallowedException(_disallowedLowerFormKey.Value);
                }
                _modKeys[placeholder.ModKey.Value] = null;
                break;
        }
    }

    #endregion

    #region OverriddenForms

    private void RegisterOverriddenFormsFishing()
    {
        if (!_mod.ListsOverriddenForms) return;
        switch (_params.OverriddenFormsOption)
        {
            case OverriddenFormsOption.NoCheck:
                break;
            case OverriddenFormsOption.Iterate:
                _recordContextIterationActions.Add(context =>
                {
                    if (context.Record.FormKey.ModKey == _modKey) return;
                    if (Enums.HasFlag(context.Record.MajorRecordFlagsRaw, Constants.Persistent)) return;
                    if (context.Parent?.Record is not IMajorRecordGetter parentMaj) return;
                    if (parentMaj.Registration.Name == "Worldspace") return;
                    _overriddenForms.Add(context.Record.FormKey);
                });
                break;
        }
    }

    private void SetOverriddenForms(
        IModHeaderCommon modHeader)
    {
        if (!_mod.ListsOverriddenForms) return;
        switch (_params.OverriddenFormsOption)
        {
            case OverriddenFormsOption.NoCheck:
                break;
            case OverriddenFormsOption.Iterate:
                if (_overriddenForms.Count == 0)
                {
                    modHeader.SetOverriddenForms(null);
                }
                else
                {
                    modHeader.SetOverriddenForms(
                        _overriddenForms.OrderBy(x => x.ModKey.FileName.String)
                            .ThenBy(x => x.ID));
                }
                break;
        }
    }

    #endregion
}