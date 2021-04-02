using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules
{
    public class MapsToGetterModule : GenerationModule
    {
        public override async Task PostLoad(ObjectGeneration obj)
        {
            await base.PostLoad(obj);
            if (obj.Abstract || !await obj.IsMajorRecord()) return;
            obj.Interfaces.Add(LoquiInterfaceDefinitionType.IGetter, $"IMapsToGetter<{obj.Interface(getter: true, internalInterface: false)}>");
        }
    }
}
