﻿using System.Buffers.Binary;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using Noggog.Testing.Extensions;
using Reloaded.Memory.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Translations.Binary;

public class FloatBinaryTranslationTests
{
    #region Write

    private byte[] GetWriteArray(Action<FloatBinaryTranslation<MutagenFrame, MutagenWriter>, MutagenWriter> toDo)
    {
        var memStream = new MemoryStream();
        using var writer = new MutagenWriter(memStream, GameConstants.SkyrimSE);
        toDo(FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance, writer);
        memStream.Position = 0;
        return memStream.ToArray();
    }

    private float? RunTypicalWriteTest(Action<FloatBinaryTranslation<MutagenFrame, MutagenWriter>, MutagenWriter> toDo)
    {
        var content = GetWriteArray(toDo);
        if (content.Length == 4)
        {
            return BinaryPrimitives.ReadSingleLittleEndian(content);
        }
        else if (content.Length == 0)
        {
            return default;
        }

        throw new ArgumentException();
    }
    
    [Fact]
    public void WriteEpsilon()
    {
        RunTypicalWriteTest((transl, writer) =>
        {
            transl.Write(writer, float.Epsilon);
        }).ShouldBe(0);
    }
    
    [Fact]
    public void WriteZero()
    {
        RunTypicalWriteTest((transl, writer) =>
        {
            transl.Write(writer, 0f);
        }).ShouldBe(0);
    }
    
    [Fact]
    public void WriteTypical()
    {
        RunTypicalWriteTest((transl, writer) =>
        {
            transl.Write(writer, 1.234f);
        }).ShouldBe(1.234f);
    }
    
    [Fact]
    public void WriteNegative()
    {
        RunTypicalWriteTest((transl, writer) =>
        {
            transl.Write(writer, -1.234f);
        }).ShouldBe(-1.234f);
    }

    [Fact]
    public void WriteWithDivisor()
    {
        RunTypicalWriteTest((transl, writer) =>
        {
            transl.Write(writer, 100f, multiplier: null, divisor: 10);
        }).ShouldBe(10f);
    }

    [Fact]
    public void WriteWithMultiplier()
    {
        RunTypicalWriteTest((transl, writer) =>
        {
            transl.Write(writer, 100f, multiplier: 10, divisor: null);
        }).ShouldBe(1000f);
    }

    [Fact]
    public void WriteWithComplex()
    {
        RunTypicalWriteTest((transl, writer) =>
        {
            transl.Write(writer, 100f, multiplier: 10, divisor: 2);
        }).ShouldBe(500f);
    }

    [Fact]
    public void WriteWithIntegerType()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.UInt, multiplier: null, divisor: null);
        });
        arr.Length.ShouldBe(4);
        BinaryPrimitives.ReadUInt32LittleEndian(arr).ShouldEqual(100);
    }

    [Fact]
    public void WriteWithIntegerTypeNull()
    {
        RunTypicalWriteTest((transl, writer) =>
        {
            transl.Write(writer, null, FloatIntegerType.UInt, multiplier: null, divisor: null);
        }).ShouldBeNull();
    }

    [Fact]
    public void WriteWithIntegerTypeWithDivisor()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.UInt, multiplier: null, divisor: 10);
        });
        arr.Length.ShouldBe(4);
        BinaryPrimitives.ReadUInt32LittleEndian(arr).ShouldEqual(10);
    }

    [Fact]
    public void WriteWithIntegerTypeWithMultiplier()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.UInt, multiplier: 10, divisor: null);
        });
        arr.Length.ShouldBe(4);
        BinaryPrimitives.ReadUInt32LittleEndian(arr).ShouldEqual(1000);
    }

    [Fact]
    public void WriteWithIntegerTypeWithComplex()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.UInt, multiplier: 10, divisor: 2);
        });
        arr.Length.ShouldBe(4);
        BinaryPrimitives.ReadUInt32LittleEndian(arr).ShouldEqual(500);
    }

    [Fact]
    public void WriteWithIntegerTypeRounded()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.UInt, multiplier: null, divisor: 7);
        });
        arr.Length.ShouldBe(4);
        BinaryPrimitives.ReadUInt32LittleEndian(arr).ShouldEqual(14);
    }

    [Fact]
    public void WriteWithIntegerTypeLimit()
    {
        Assert.Throws<FloatStoredAsIntegerOverflowException>(() =>
        {
            var arr = GetWriteArray((transl, writer) =>
            {
                transl.Write(writer, int.MaxValue, FloatIntegerType.UInt, multiplier: 2, divisor: null);
            });
            arr.Length.ShouldBe(4);
            BinaryPrimitives.ReadUInt32LittleEndian(arr).ShouldEqual(14);
        });
    }

    [Fact]
    public void WriteWithUShortType()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.UShort, multiplier: null, divisor: null);
        });
        arr.Length.ShouldBe(2);
        BinaryPrimitives.ReadUInt16LittleEndian(arr).ShouldEqual(100);
    }

    [Fact]
    public void WriteWithUShortTypeNull()
    {
        RunTypicalWriteTest((transl, writer) =>
        {
            transl.Write(writer, null, FloatIntegerType.UShort, multiplier: null, divisor: null);
        }).ShouldBeNull();
    }

    [Fact]
    public void WriteWithUShortTypeWithDivisor()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.UShort, multiplier: null, divisor: 10);
        });
        arr.Length.ShouldBe(2);
        BinaryPrimitives.ReadUInt16LittleEndian(arr).ShouldEqual(10);
    }

    [Fact]
    public void WriteWithUShortTypeWithMultiplier()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.UShort, multiplier: 10, divisor: null);
        });
        arr.Length.ShouldBe(2);
        BinaryPrimitives.ReadUInt16LittleEndian(arr).ShouldEqual(1000);
    }

    [Fact]
    public void WriteWithUShortTypeWithComplex()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.UShort, multiplier: 10, divisor: 2);
        });
        arr.Length.ShouldBe(2);
        BinaryPrimitives.ReadUInt16LittleEndian(arr).ShouldEqual(500);
    }

    [Fact]
    public void WriteWithUShortTypeRounded()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.UShort, multiplier: null, divisor: 7);
        });
        arr.Length.ShouldBe(2);
        BinaryPrimitives.ReadUInt16LittleEndian(arr).ShouldEqual(14);
    }

    [Fact]
    public void WriteWithUShortTypeLimit()
    {
        Assert.Throws<FloatStoredAsIntegerOverflowException>(() =>
        {
            var arr = GetWriteArray((transl, writer) =>
            {
                transl.Write(writer, ushort.MaxValue, FloatIntegerType.UShort, multiplier: 2, divisor: null);
            });
            arr.Length.ShouldBe(2);
            BinaryPrimitives.ReadUInt16LittleEndian(arr).ShouldEqual(14);
        });
    }

    [Fact]
    public void WriteWithByteType()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.Byte, multiplier: null, divisor: null);
        });
        arr.Length.ShouldBe(1);
        arr[0].ShouldEqual(100);
    }

    [Fact]
    public void WriteWithByteTypeNull()
    {
        RunTypicalWriteTest((transl, writer) =>
        {
            transl.Write(writer, null, FloatIntegerType.Byte, multiplier: null, divisor: null);
        }).ShouldBeNull();
    }

    [Fact]
    public void WriteWithByteTypeWithDivisor()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.Byte, multiplier: null, divisor: 10);
        });
        arr.Length.ShouldBe(1);
        arr[0].ShouldEqual(10);
    }

    [Fact]
    public void WriteWithByteTypeWithMultiplier()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 10f, FloatIntegerType.Byte, multiplier: 10, divisor: null);
        });
        arr.Length.ShouldBe(1);
        arr[0].ShouldEqual(100);
    }

    [Fact]
    public void WriteWithByteTypeWithComplex()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 10f, FloatIntegerType.Byte, multiplier: 10, divisor: 2);
        });
        arr.Length.ShouldBe(1);
        arr[0].ShouldEqual(50);
    }

    [Fact]
    public void WriteWithByteTypeRounded()
    {
        var arr = GetWriteArray((transl, writer) =>
        {
            transl.Write(writer, 100f, FloatIntegerType.Byte, multiplier: null, divisor: 7);
        });
        arr.Length.ShouldBe(1);
        arr[0].ShouldEqual(14);
    }

    [Fact]
    public void WriteWithByteTypeLimit()
    {
        Assert.Throws<FloatStoredAsIntegerOverflowException>(() =>
        {
            var arr = GetWriteArray((transl, writer) =>
            {
                transl.Write(writer, byte.MaxValue, FloatIntegerType.Byte, multiplier: 2, divisor: null);
            });
            arr.Length.ShouldBe(1);
            BinaryPrimitives.ReadUInt16LittleEndian(arr).ShouldEqual(14);
        });
    }

    #endregion

    #region Read

    public float? GetReadFloat(
        float f,
        Func<FloatBinaryTranslation<MutagenFrame, MutagenWriter>, MutagenFrame, float> toDo)
    {
        return GetReadFloat(
            f,
            (bytes, f) => BinaryPrimitives.WriteSingleLittleEndian(bytes, f),
            toDo);
    }

    public float? GetReadFloat(
        byte[] bytes,
        Func<FloatBinaryTranslation<MutagenFrame, MutagenWriter>, MutagenFrame, float> toDo)
    {
        var memStream = new MemoryStream(bytes);
        var mutagenFrame = new MutagenFrame(
            new MutagenInterfaceReadStream(
                new BinaryReadStream(memStream),
                new ParsingMeta(GameConstants.SkyrimSE, ModKey.Null, SeparatedMasterPackage.EmptyNull)));
        return toDo(FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance, mutagenFrame);
    }

    public float? GetReadFloat(
        float f,
        Action<byte[], float> writer,
        Func<FloatBinaryTranslation<MutagenFrame, MutagenWriter>, MutagenFrame, float> toDo)
    {
        var bytes = new byte[4];
        writer(bytes, f);
        return GetReadFloat(bytes, toDo);
    }

    [Fact]
    public void ReadTypical()
    {
        GetReadFloat(1.5f, (t, f) => t.Parse(f))
            .ShouldBe(1.5f);
    }

    [Fact]
    public void ReadEpsilon()
    {
        GetReadFloat(float.Epsilon, (t, f) => t.Parse(f))
            .ShouldBe(0f);
    }

    [Fact]
    public void ReadOne()
    {
        var bytes = new byte[]
        {
            1, 0, 0, 0
        };
        GetReadFloat(bytes, (t, f) => t.Parse(f))
            .ShouldBe(0f);
    }

    [Fact]
    public void ReadMultiplier()
    {
        GetReadFloat(1.5f, (t, f) => t.Parse(f, multiplier: 2f, divisor: null))
            .ShouldBe(3f);
    }

    [Fact]
    public void ReadDivisor()
    {
        GetReadFloat(3f, (t, f) => t.Parse(f, multiplier: null, divisor: 2f))
            .ShouldBe(1.5f);
    }

    [Fact]
    public void ReadComplex()
    {
        GetReadFloat(3f, (t, f) => t.Parse(f, multiplier: 4f, divisor: 2f))
            .ShouldBe(6f);
    }

    [Fact]
    public void ReadIntegerType()
    {
        GetReadFloat(100f,
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (int)f),
                (t, f) =>
                {
                    return t.Parse(f, FloatIntegerType.UInt, 1f);
                })
            .ShouldBe(100f);
    }

    [Fact]
    public void ReadIntegerTypeMultiplier()
    {
        GetReadFloat(100f, 
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (int)f),
                (t, f) =>
                {
                    return t.Parse(f, FloatIntegerType.UInt, 2f);
                })
            .ShouldBe(200f);
    }

    [Fact]
    public void ReadUShortType()
    {
        GetReadFloat(100f,
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (short)f),
                (t, f) =>
                {
                    return t.Parse(f, FloatIntegerType.UShort, 1f);
                })
            .ShouldBe(100f);
    }

    [Fact]
    public void ReadUShortTypeMultiplier()
    {
        GetReadFloat(100f, 
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (int)f),
                (t, f) =>
                {
                    return t.Parse(f, FloatIntegerType.UShort, 2f);
                })
            .ShouldBe(200f);
    }

    [Fact]
    public void ReadByteType()
    {
        GetReadFloat(100f,
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (short)f),
                (t, f) =>
                {
                    return t.Parse(f, FloatIntegerType.Byte, 1f);
                })
            .ShouldBe(100f);
    }

    [Fact]
    public void ReadByteTypeMultiplier()
    {
        GetReadFloat(100f, 
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (int)f),
                (t, f) =>
                {
                    return t.Parse(f, FloatIntegerType.Byte, 2f);
                })
            .ShouldBe(200f);
    }

    [Fact]
    public void GetIntegerType()
    {
        GetReadFloat(100f,
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (int)f),
                (t, f) =>
                {
                    return t.GetFloat(f.ReadBytes(4), FloatIntegerType.UInt, 1f);
                })
            .ShouldBe(100f);
    }

    [Fact]
    public void GetIntegerTypeMultiplier()
    {
        GetReadFloat(100f, 
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (int)f),
                (t, f) =>
                {
                    return t.GetFloat(f.ReadBytes(4), FloatIntegerType.UInt, 2f);
                })
            .ShouldBe(200f);
    }

    [Fact]
    public void GetUShortType()
    {
        GetReadFloat(100f,
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (short)f),
                (t, f) =>
                {
                    return t.GetFloat(f.ReadBytes(4), FloatIntegerType.UShort, 1f);
                })
            .ShouldBe(100f);
    }

    [Fact]
    public void GetUShortTypeMultiplier()
    {
        GetReadFloat(100f, 
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (int)f),
                (t, f) =>
                {
                    return t.GetFloat(f.ReadBytes(4), FloatIntegerType.UShort, 2f);
                })
            .ShouldBe(200f);
    }

    [Fact]
    public void GetByteType()
    {
        GetReadFloat(100f,
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (short)f),
                (t, f) =>
                {
                    return t.GetFloat(f.ReadBytes(4), FloatIntegerType.Byte, 1f);
                })
            .ShouldBe(100f);
    }

    [Fact]
    public void GetByteTypeMultiplier()
    {
        GetReadFloat(100f, 
                (b, f) => BinaryPrimitives.WriteInt32LittleEndian(b, (int)f),
                (t, f) =>
                {
                    return t.GetFloat(f.ReadBytes(4), FloatIntegerType.Byte, 2f);
                })
            .ShouldBe(200f);
    }

    #endregion
}