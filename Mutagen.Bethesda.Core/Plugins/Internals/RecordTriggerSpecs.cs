namespace Mutagen.Bethesda.Plugins.Internals
{
    public class RecordTriggerSpecs
    {
        public IRecordCollection AllRecordTypes { get; }
        public IRecordCollection TriggeringRecordTypes { get; }
        public bool AllAreTriggers { get; }

        public RecordTriggerSpecs(IRecordCollection allRecordTypes, IRecordCollection triggeringRecordTypes)
        {
            AllRecordTypes = allRecordTypes;
            TriggeringRecordTypes = triggeringRecordTypes;
            AllAreTriggers = false;
        }

        public RecordTriggerSpecs(IRecordCollection allRecordTypes)
        {
            AllRecordTypes = allRecordTypes;
            TriggeringRecordTypes = allRecordTypes;
            AllAreTriggers = true;
        }
    }
}
