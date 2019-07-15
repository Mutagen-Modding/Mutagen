using BenchmarkDotNet.Attributes;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
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
        MasterReferences _masterRefs = new MasterReferences(new IMasterReferenceGetter[] { }, Mutagen.Bethesda.Oblivion.Constants.Oblivion);

        [GlobalSetup]
        public void Setup()
        {
            _data[0] = (byte)'A';
            _data[1] = (byte)'M';
            _data[2] = (byte)'M';
            _data[3] = (byte)'O';
            _frame = new MutagenFrame(new MutagenMemoryReadStream(_data, MetaDataConstants.Oblivion));
        }

        [Benchmark]
        public Ammo Direct()
        {
            _frame.Position = 0;
            return Ammo.CreateFromBinary(
                frame: _frame,
                masterReferences: _masterRefs,
                recordTypeConverter: null,
                errorMask: null);
        }

        [Benchmark]
        public Ammo Wrapped()
        {
            _frame.Position = 0;
            return _create(
                reader: _frame,
                masterReferences: _masterRefs,
                recordTypeConverter: null,
                errorMask: null);
        }
    }
}
