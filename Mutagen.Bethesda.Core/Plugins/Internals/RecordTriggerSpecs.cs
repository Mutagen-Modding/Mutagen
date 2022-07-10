namespace Mutagen.Bethesda.Plugins.Internals;

internal sealed class RecordTriggerSpecs
{
    public IReadOnlyRecordCollection AllRecordTypes { get; }
    public IReadOnlyRecordCollection TriggeringRecordTypes { get; }
    public bool AllAreTriggers { get; }

    public RecordTriggerSpecs(IReadOnlyRecordCollection allRecordTypes, IReadOnlyRecordCollection triggeringRecordTypes)
    {
        AllRecordTypes = allRecordTypes;
        TriggeringRecordTypes = triggeringRecordTypes;
        AllAreTriggers = false;
    }

    public RecordTriggerSpecs(IReadOnlyRecordCollection allRecordTypes)
    {
        AllRecordTypes = allRecordTypes;
        TriggeringRecordTypes = allRecordTypes;
        AllAreTriggers = true;
    }
}