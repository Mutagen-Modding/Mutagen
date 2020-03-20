using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Tests.Benchmarks
{
    public class LoquiTranslationReflection
    {
        LoquiBinaryTranslation<Ammo>.CREATE_FUNC _create = LoquiBinaryTranslation<Ammo>.CREATE;
        byte[] _data = new byte[0x14];
        MutagenFrame _frame;
        MasterReferenceReader _masterRefs = new MasterReferenceReader(Mutagen.Bethesda.Oblivion.Constants.Oblivion);

        [GlobalSetup]
        public void Setup()
        {
            _data[0] = (byte)'A';
            _data[1] = (byte)'M';
            _data[2] = (byte)'M';
            _data[3] = (byte)'O';
            _frame = new MutagenFrame(new MutagenMemoryReadStream(_data, GameConstants.Oblivion, _masterRefs));
        }

        [Benchmark]
        public Ammo Direct()
        {
            _frame.Position = 0;
            return Ammo.CreateFromBinary(
                frame: _frame,
                recordTypeConverter: null);
        }

        [Benchmark]
        public Ammo Wrapped()
        {
            _frame.Position = 0;
            return _create(
                reader: _frame,
                recordTypeConverter: null);
        }
    }
}
