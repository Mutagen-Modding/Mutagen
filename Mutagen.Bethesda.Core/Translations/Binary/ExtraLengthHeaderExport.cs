using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class ExtraLengthHeaderExport : IDisposable
    {
        MemoryTributary _tributary = new MemoryTributary();
        MutagenWriter _writer;
        RecordType _mainRecord;
        RecordType _extraLengthRecord;

        public MutagenWriter PrepWriter { get; }

        public ExtraLengthHeaderExport(
            MutagenWriter writer,
            RecordType mainRecord,
            RecordType extraLengthRecord)
        {
            this._writer = writer;
            this._mainRecord = mainRecord;
            this._extraLengthRecord = extraLengthRecord;
            PrepWriter = new MutagenWriter(_tributary, writer.MetaData);
        }

        public void Dispose()
        {
            _tributary.Position = 0;
            if (_tributary.Length <= ushort.MaxValue)
            {
                using (HeaderExport.ExportSubrecordHeader(_writer, _mainRecord))
                {
                    _tributary.CopyTo(_writer.BaseStream);
                }
            }
            else
            {
                using (HeaderExport.ExportSubrecordHeader(_writer, _extraLengthRecord))
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
