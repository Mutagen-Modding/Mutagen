using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public partial class Substitution
    {
        public Substitution(long loc, byte data)
        {
            this.Location = loc;
            this.Data = new byte[] { data };
        }
    }
}
