using System.Linq;
using Mutagen.Bethesda.Plugins.Masters;

namespace Mutagen.Bethesda.Plugins.Binary.Parameters
{
    /// <summary>
    /// An abstract class representing a logic choice for ordering masters
    /// </summary>
    public abstract class AMastersListOrderingOption
    {
        public static implicit operator AMastersListOrderingOption(MastersListOrderingOption option)
        {
            return new MastersListOrderingEnumOption()
            {
                Option = option
            };
        }

        public static AMastersListOrderingOption ByMasters(IMasterReferenceReader reader)
        {
            return new MastersListOrderingByLoadOrder(reader.Masters.Select(m => m.Master));
        }
    }
}