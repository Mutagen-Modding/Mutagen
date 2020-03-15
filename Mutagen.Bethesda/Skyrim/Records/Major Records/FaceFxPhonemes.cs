using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim.Internals
{
    public partial class FaceFxPhonemesBinaryOverlay
    {
        private List<ReadOnlyMemorySlice<byte>>? faceFx;

        public IPhonemeTargetGetter IY => Get(0);
        public IPhonemeTargetGetter IH => Get(1);
        public IPhonemeTargetGetter EH => Get(2);
        public IPhonemeTargetGetter EY => Get(3);
        public IPhonemeTargetGetter AE => Get(4);
        public IPhonemeTargetGetter AA => Get(5);
        public IPhonemeTargetGetter AW => Get(6);
        public IPhonemeTargetGetter AY => Get(7);
        public IPhonemeTargetGetter AH => Get(8);
        public IPhonemeTargetGetter AO => Get(9);
        public IPhonemeTargetGetter OY => Get(10);
        public IPhonemeTargetGetter OW => Get(11);
        public IPhonemeTargetGetter UH => Get(12);
        public IPhonemeTargetGetter UW => Get(13);
        public IPhonemeTargetGetter ER => Get(14);
        public IPhonemeTargetGetter AX => Get(15);
        public IPhonemeTargetGetter S => Get(16);
        public IPhonemeTargetGetter SH => Get(17);
        public IPhonemeTargetGetter Z => Get(18);
        public IPhonemeTargetGetter ZH => Get(19);
        public IPhonemeTargetGetter F => Get(20);
        public IPhonemeTargetGetter TH => Get(21);
        public IPhonemeTargetGetter V => Get(22);
        public IPhonemeTargetGetter DH => Get(23);
        public IPhonemeTargetGetter M => Get(24);
        public IPhonemeTargetGetter N => Get(25);
        public IPhonemeTargetGetter NG => Get(26);
        public IPhonemeTargetGetter L => Get(27);
        public IPhonemeTargetGetter R => Get(28);
        public IPhonemeTargetGetter W => Get(29);
        public IPhonemeTargetGetter Y => Get(30);
        public IPhonemeTargetGetter HH => Get(31);
        public IPhonemeTargetGetter B => Get(32);
        public IPhonemeTargetGetter D => Get(33);
        public IPhonemeTargetGetter JH => Get(34);
        public IPhonemeTargetGetter G => Get(35);
        public IPhonemeTargetGetter P => Get(36);
        public IPhonemeTargetGetter T => Get(37);
        public IPhonemeTargetGetter K => Get(38);
        public IPhonemeTargetGetter CH => Get(39);
        public IPhonemeTargetGetter SIL => Get(40);
        public IPhonemeTargetGetter SHOTSIL => Get(41);
        public IPhonemeTargetGetter FLAP => Get(42);

        partial void ParsingCustomParse(BinaryMemoryReadStream stream, int offset)
        {
            faceFx = new List<ReadOnlyMemorySlice<byte>>();
            var subRecord = _package.Meta.GetSubRecord(stream);
            while (subRecord.RecordType == FaceFxPhonemes_Registration.PHWT_HEADER)
            {
                faceFx.Add(stream.ReadMemory(subRecord.TotalLength));
                if (stream.Complete) break;
                subRecord = _package.Meta.GetSubRecord(stream);
            }

            if (faceFx.Count != 43)
            {
                throw new ArgumentException();
            }
        }

        private IPhonemeTargetGetter Get(int index)
        {
            return PhonemeTargetBinaryOverlay.PhonemeTargetFactory(new BinaryMemoryReadStream(faceFx![0]), _package);
        }
    }
}
