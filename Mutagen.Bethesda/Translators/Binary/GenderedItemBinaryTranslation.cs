using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class GenderedItemBinaryTranslation
    {
        public static GenderedItem<TItem> Parse<TItem>(
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

        public static GenderedItem<TItem> Parse<TItem>(
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

        public static GenderedItem<TItem?> Parse<TItem>(
            MutagenFrame frame,
            RecordType maleMarker,
            RecordType femaleMarker,
            UtilityTranslation.BinarySubParseDelegate<TItem> transl,
            bool skipMarker)
            where TItem : class
        {
            TItem? male = default, female = default;
            for (int i = 0; i < 2; i++)
            {
                var subHeader = frame.MetaData.GetSubRecord(frame);
                RecordType type = subHeader.RecordType;
                if (type == maleMarker)
                {
                    if (skipMarker)
                    {
                        frame.Position += subHeader.TotalLength;
                        if (!transl(frame, out male))
                        {
                            throw new ArgumentException();
                        }
                    }
                    else
                    {
                        frame.Position += subHeader.HeaderLength;
                        if (!transl(frame.SpawnWithLength(subHeader.RecordLength), out male))
                        {
                            throw new ArgumentException();
                        }
                    }
                }
                else if (type == femaleMarker)
                {
                    if (skipMarker)
                    {
                        frame.Position += subHeader.TotalLength;
                        if (!transl(frame, out female))
                        {
                            throw new ArgumentException();
                        }
                    }
                    else
                    {
                        frame.Position += subHeader.HeaderLength;
                        if (!transl(frame.SpawnWithLength(subHeader.RecordLength), out female))
                        {
                            throw new ArgumentException();
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return new GenderedItem<TItem?>(male, female);
        }

        public static GenderedItem<TItem?> Parse<TItem>(
            MutagenFrame frame,
            MasterReferences masterReferences,
            RecordType maleMarker,
            RecordType femaleMarker,
            UtilityTranslation.BinaryMasterParseDelegate<TItem> transl)
            where TItem : class
        {
            TItem? male = default, female = default;
            for (int i = 0; i < 2; i++)
            {
                var subHeader = frame.MetaData.GetSubRecord(frame);
                RecordType type = subHeader.RecordType;
                if (type == maleMarker)
                {
                    frame.Position += subHeader.TotalLength;
                    if (!transl(frame, out male, masterReferences))
                    {
                        throw new ArgumentException();
                    }
                }
                else if (type == femaleMarker)
                {
                    frame.Position += subHeader.TotalLength;
                    if (!transl(frame, out female, masterReferences))
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    break;
                }
            }
            return new GenderedItem<TItem?>(male, female);
        }
    }
}
