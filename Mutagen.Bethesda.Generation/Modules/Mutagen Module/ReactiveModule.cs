using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{ 
    public class ReactiveModule : GenerationModule
    {
        public override Task PostLoad(ObjectGeneration obj)
        {
            if (!obj.HasLoquiBaseObject)
            {
                obj.NonLoquiBaseClass = "LoquiNotifyingObject";
            }
            obj.RequiredNamespaces.Add("ReactiveUI");
            return base.PostLoad(obj);
        }
    }
}
