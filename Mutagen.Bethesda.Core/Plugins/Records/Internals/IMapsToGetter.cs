using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Plugins.Records.Internals
{
    /// <summary>
    /// Helper interface that helps map an interface to the getter version of a major record interface
    /// </summary>
    public interface IMapsToGetter<IGetter> : IMajorRecordGetter
    {
    }
}
