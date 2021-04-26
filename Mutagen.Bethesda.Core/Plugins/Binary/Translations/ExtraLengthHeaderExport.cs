using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using System;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public class ExtraLengthHeaderExport : IDisposable
    {
        MemoryTributary _tributary = new MemoryTributary();
        MutagenWriter _writer;
        RecordType _mainRecord;
        RecordType _overflowRecord;

        public MutagenWriter PrepWriter { get; }

        public ExtraLengthHeaderExport(
            MutagenWriter writer,
            RecordType mainRecord,
            RecordType overflowRecord)
        {
            this._writer = writer;
            this._mainRecord = mainRecord;
            this._overflowRecord = overflowRecord;
            PrepWriter = new MutagenWriter(_tributary, writer.MetaData);
        }

        public void Dispose()
        {
            _tributary.Position = 0;
            if (_tributary.Length <= ushort.MaxValue)
            {
                using (HeaderExport.Subrecord(_writer, _mainRecord))
                {
                    _tributary.CopyTo(_writer.BaseStream);
                }
            }
            else
            {
                using (HeaderExport.Subrecord(_writer, _overflowRecord))
                {
                    _writer.Write(checked((uint)_tributary.Length));
                }
                _writer.Write(_mainRecord.TypeInt);
                _writer.WriteZeros(2);
                _tributary.CopyTo(_writer.BaseStream);
            }
        }
    }
}
