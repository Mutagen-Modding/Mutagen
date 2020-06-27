using Loqui.Generation;
using System;
using Noggog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class CorrectnessModule : GenerationModule
    {
        public override async Task LoadWrapup(ObjectGeneration obj)
        {
            var objData = obj.GetObjectData();
            await Task.WhenAll(
                objData.WiringComplete.Task,
                objData.DataTypeModuleComplete.Task);
            Dictionary<string, TypeGeneration> triggerMapping = new Dictionary<string, TypeGeneration>();
            Dictionary<RecordType, TypeGeneration> triggerRecMapping = new Dictionary<RecordType, TypeGeneration>();
            foreach (var field in obj.IterateFields())
            {
                var mutaData = field.GetFieldData();
                if (!mutaData.HasTrigger) continue;
                if (mutaData.Binary == BinaryGenerationType.NoGeneration) continue;
                foreach (var trigger in mutaData.TriggeringRecordAccessors)
                {
                    if (triggerMapping.TryGetValue(trigger, out var existingField))
                    {
                        throw new ArgumentException($"{obj.Name} cannot have two fields that have the same trigger {trigger}: {existingField.Name} AND {field.Name}");
                    }
                    triggerMapping[trigger] = field;
                }
                foreach (var triggerRec in mutaData.TriggeringRecordTypes)
                {
                    if (triggerRecMapping.TryGetValue(triggerRec, out var existingField))
                    {
                        throw new ArgumentException($"{obj.Name} cannot have two fields that have the same trigger record {triggerRec}: {existingField.Name} AND {field.Name}");
                    }
                    triggerRecMapping[triggerRec] = field;
                }
                if (field is LoquiType loqui && loqui.GetFieldData().HasTrigger)
                {
                    if (loqui.Singleton) break;
                }
            }

            bool triggerEncountered = false;
            foreach (var field in obj.IterateFields(
                nonIntegrated: true,
                expandSets: SetMarkerType.ExpandSets.False))
            {
                if (field is SetMarkerType) continue;
                if (field.Derivative || !field.IntegrateField) continue;
                var data = field.GetFieldData();
                if (data.Binary == BinaryGenerationType.NoGeneration) continue;
                if (data.HasTrigger)
                {
                    triggerEncountered = true;
                }
                else if (triggerEncountered)
                {
                    throw new ArgumentException($"{obj.Name} cannot have an embedded field without a record type after ones with record types have been defined: {field.Name}");
                }
            }

            await base.LoadWrapup(obj);
        }
    }
}
