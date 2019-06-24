using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public class MetaData
    {
        public sbyte ModHeaderFluffLength { get; private set; }
        public sbyte GrupHeaderLength { get; private set; }
        public sbyte GrupMetaLengthAfterType { get; private set; }
        public sbyte RecordMetaLengthAfterRecordLength { get; private set; }

        public static readonly MetaData Oblivion = new MetaData()
        {
            ModHeaderFluffLength = 12,
            GrupHeaderLength = 20,
            GrupMetaLengthAfterType = 4,
            RecordMetaLengthAfterRecordLength = 12,
        };

        public static readonly MetaData Skyrim = new MetaData()
        {
            ModHeaderFluffLength = 16,
            GrupHeaderLength = 24,
            GrupMetaLengthAfterType = 8,
            RecordMetaLengthAfterRecordLength = 16,
        };

        public static MetaData Get(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Oblivion:
                    return Oblivion;
                case GameMode.Skyrim:
                    return Skyrim;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
