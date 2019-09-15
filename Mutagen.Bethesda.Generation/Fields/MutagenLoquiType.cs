using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class MutagenLoquiType : LoquiType
    {
        private ObjectType _interfObjectType;

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            if (this.RefType == LoquiRefType.Interface)
            {
                if (!node.TryGetAttribute<ObjectType>(Mutagen.Bethesda.Generation.Constants.ObjectType, out _interfObjectType))
                {
                    throw new ArgumentException("Interface Ref Type was specified without supplying object type");
                }
            }
        }

        public ObjectType? GetObjectType()
        {
            if (this.RefType == LoquiRefType.Interface) return _interfObjectType;
            return this.TargetObjectGeneration?.GetObjectType();
        }
    }
}
