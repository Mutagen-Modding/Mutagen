using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class CustomLogic : NothingType
    {
        public int? ExpectedLength;

        public override string ToString()
        {
            return "Custom";
        }

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            ExpectedLength = node.GetAttribute<int?>(Constants.EXPECTED_LENGTH, null);
        }
    }
}
