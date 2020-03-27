using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    /// <summary>
    /// Different types of numeric comparison styles allowed in Oblivion
    /// </summary>
    public enum CompareOperator
    {
        EqualTo = 0,
        NotEqualTo = 2,
        GreaterThan = 4,
        GreaterThanOrEqualTo = 6,
        LessThan = 8,
        LessThanOrEqualTo = 10
    }
}
