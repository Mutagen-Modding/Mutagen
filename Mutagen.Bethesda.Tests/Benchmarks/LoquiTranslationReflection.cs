using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;

namespace Mutagen.Bethesda.Tests.Benchmarks
{
    public class LoquiTranslationReflection
    {
        LoquiBinaryTranslation<Ammunition>.CREATE_FUNC _create = LoquiBinaryTranslation<Ammunition>.CREATE;
        byte[] _data = new byte[0x14];
        MutagenFrame _frame;
        MasterReferenceCollection _masterRefs = new MasterReferenceCollection(Mutagen.Bethesda.Oblivion.Constants.Oblivion);
        ParsingBundle _parsingBundle;

        [GlobalSetup]
        public void Setup()
        {
            _data[0] = (byte)'A';
            _data[1] = (byte)'M';
            _data[2] = (byte)'M';
            _data[3] = (byte)'O';
            _parsingBundle = new ParsingBundle(GameRelease.Oblivion, _masterRefs);
            _frame = new MutagenFrame(new MutagenMemoryReadStream(_data, _parsingBundle));
        }

        [Benchmark]
        public Ammunition Direct()
        {
            _frame.Position = 0;
            return Ammunition.CreateFromBinary(
                frame: _frame,
                translationParams: null);
        }

        [Benchmark]
        public Ammunition Wrapped()
        {
            _frame.Position = 0;
            return _create(
                reader: _frame,
                translationParams: null);
        }
    }
}
