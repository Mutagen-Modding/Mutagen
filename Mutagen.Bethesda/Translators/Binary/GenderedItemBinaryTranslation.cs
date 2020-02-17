using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class GenderedItemBinaryTranslation<TItem>
    {
        public readonly static GenderedItemBinaryTranslation<TItem> Instance = new GenderedItemBinaryTranslation<TItem>();

        public GenderedItem<TItem> Parse(
            MutagenFrame frame)
        {
            throw new NotImplementedException();
        }
    }
}
