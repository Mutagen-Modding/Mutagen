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
