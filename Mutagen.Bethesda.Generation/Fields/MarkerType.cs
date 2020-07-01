using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Generation
{
    public class MarkerType : TypicalTypeGeneration
    {
        public bool EndMarker => IsEndMarker();

        public override bool IsEnumerable => false;

        public override bool IsClass => false;

        public MarkerType()
        {
            IntegrateField = false;
        }

        public override string GetDuplicate(Accessor accessor)
        {
            throw new NotImplementedException();
        }

        public override Type Type(bool getter)
        {
            throw new NotImplementedException();
        }

        private bool IsEndMarker()
        {
            return this.ObjectGen.Fields.Last() == this;
        }
    }
}
