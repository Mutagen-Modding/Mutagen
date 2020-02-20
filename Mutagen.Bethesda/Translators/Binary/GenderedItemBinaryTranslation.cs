using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class GenderedItemBinaryTranslation<TItem>
    {
        public readonly static GenderedItemBinaryTranslation<TItem> Instance = new GenderedItemBinaryTranslation<TItem>();

        public static GenderedItem<TItem> Parse(
            MutagenFrame frame,
            UtilityTranslation.BinarySubParseDelegate<TItem> transl)
        {
            if (!transl(frame, out var male))
            {
                throw new ArgumentException();
            }
            if (!transl(frame, out var female))
            {
                throw new ArgumentException();
            }
            return new GenderedItem<TItem>(male, female);
        }

        public static GenderedItem<TItem> Parse(
            MutagenFrame frame,
            MasterReferences masterReferences,
            UtilityTranslation.BinaryMasterParseDelegate<TItem> transl)
        {
            if (!transl(frame, out var male, masterReferences))
            {
                throw new ArgumentException();
            }
            if (!transl(frame, out var female, masterReferences))
            {
                throw new ArgumentException();
            }
            return new GenderedItem<TItem>(male, female);
        }
    }
}
