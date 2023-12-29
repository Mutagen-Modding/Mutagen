namespace Mutagen.Bethesda.Plugins.Internals;

internal sealed class RecordTriggerSpecs
{
    public IReadOnlyRecordCollection AllRecordTypes { get; }
    public IReadOnlyRecordCollection TriggeringRecordTypes { get; }
    public IReadOnlyRecordCollection EndRecordTypes { get; }
    public bool AllAreTriggers { get; }

    public RecordTriggerSpecs(
        IReadOnlyRecordCollection allRecordTypes, 
        IReadOnlyRecordCollection triggeringRecordTypes,
        IReadOnlyRecordCollection endRecordTypes)
    {
        AllRecordTypes = allRecordTypes;
        TriggeringRecordTypes = triggeringRecordTypes;
        EndRecordTypes = endRecordTypes;
        AllAreTriggers = false;
    }

    public RecordTriggerSpecs(IReadOnlyRecordCollection allRecordTypes, IReadOnlyRecordCollection triggeringRecordTypes)
    {
        AllRecordTypes = allRecordTypes;
        TriggeringRecordTypes = triggeringRecordTypes;
        EndRecordTypes = EmptyRecordCollection.Instance;
        AllAreTriggers = false;
    }

    public RecordTriggerSpecs(IReadOnlyRecordCollection allRecordTypes)
    {
        AllRecordTypes = allRecordTypes;
        TriggeringRecordTypes = allRecordTypes;
        EndRecordTypes = EmptyRecordCollection.Instance;
        AllAreTriggers = true;
    }
}