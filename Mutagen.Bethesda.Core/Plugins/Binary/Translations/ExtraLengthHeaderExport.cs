using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public sealed class ExtraLengthHeaderExport : IDisposable
{
    private readonly MemoryTributary _tributary = new();
    private readonly MutagenWriter _writer;
    private readonly RecordType _mainRecord;
    private readonly RecordType _overflowRecord;

    public MutagenWriter PrepWriter { get; }

    public ExtraLengthHeaderExport(
        MutagenWriter writer,
        RecordType mainRecord,
        RecordType overflowRecord)
    {
        _writer = writer;
        _mainRecord = mainRecord;
        _overflowRecord = overflowRecord;
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
