using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class ContainerParentModule : GenerationModule
    {
        public override async Task PostLoad(ObjectGeneration obj)
        {
            try
            {
                foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.TrueAndInclude, nonIntegrated: true))
                {
                    MutagenFieldData subData;
                    switch (field)
                    {
                        case WrapperType wrapper:
                            subData = wrapper.SubTypeGeneration.GetFieldData();
                            break;
                        case DictType_KeyedValue dict:
                            subData = dict.ValueTypeGen.GetFieldData();
                            break;
                        default:
                            continue;
                    }
                    subData.Parent = field;
                }
            }
            catch (Exception ex)
            {
                obj.GetObjectData().WiringComplete.SetException(ex);
                throw;
            }
        }
    }
}
