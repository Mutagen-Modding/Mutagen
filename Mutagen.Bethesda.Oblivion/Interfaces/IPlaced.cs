using Loqui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public interface IPlaced : IMajorRecordInternal, IPlacedGetter
    {
    }

    public interface IPlacedGetter : IMajorRecordInternalGetter, ILoquiObjectGetter, IXmlFolderItem
    {
    }
}
