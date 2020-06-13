using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class SkyrimMajorRecord
    {
        protected override int? VersionAbstract => this.FormVersion;
    }

    namespace Internals
    {
        public partial class SkyrimMajorRecordBinaryOverlay
        {
            protected override int? VersionAbstract => this.FormVersion;
        }
    }
}
