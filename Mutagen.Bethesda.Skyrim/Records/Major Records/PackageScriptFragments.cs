using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System;
using System.Buffers.Binary;
using static Mutagen.Bethesda.Skyrim.Internals.PackageScriptFragmentsBinaryCreateTranslation;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PackageScriptFragmentsBinaryCreateTranslation
        {
            [Flags]
            public enum Flag
            {
                OnBegin = 0x01,
                OnEnd = 0x02,
                OnChange = 0x04,
            }

            static partial void FillBinaryFlagsParseCustom(MutagenFrame frame, IPackageScriptFragments item)
            {
                var flag = (Flag)frame.ReadUInt8();
                item.FileName = StringBinaryTranslation.Instance.Parse(
                     reader: frame,
                     stringBinaryType: StringBinaryType.PrependLengthUShort);
                if (flag.HasFlag(Flag.OnBegin))
                {
                    item.OnBegin = ScriptFragment.CreateFromBinary(frame);
                }
                if (flag.HasFlag(Flag.OnEnd))
                {
                    item.OnEnd = ScriptFragment.CreateFromBinary(frame);
                }
                if (flag.HasFlag(Flag.OnChange))
                {
                    item.OnChange = ScriptFragment.CreateFromBinary(frame);
                }
            }
        }

        public partial class PackageScriptFragmentsBinaryWriteTranslation
        {
            static partial void WriteBinaryFlagsParseCustom(MutagenWriter writer, IPackageScriptFragmentsGetter item)
            {
                var begin = item.OnBegin;
                var end = item.OnEnd;
                var change = item.OnChange;
                Flag flag = default;
                if (begin != null)
                {
                    flag |= Flag.OnBegin;
                }
                if (end != null)
                {
                    flag |= Flag.OnEnd;
                }
                if (change != null)
                {
                    flag |= Flag.OnChange;
                }
                writer.Write((byte)flag);
                StringBinaryTranslation.Instance.Write(
                    writer: writer,
                    item: item.FileName,
                    binaryType: StringBinaryType.PrependLengthUShort);
                begin?.WriteToBinary(writer);
                end?.WriteToBinary(writer);
                change?.WriteToBinary(writer);
            }
        }

        public partial class PackageScriptFragmentsBinaryOverlay
        {
            Flag Flags => (Flag)_data.Span.Slice(0x1, 0x1)[0];

            public string FileName => BinaryStringUtility.ParsePrependedString(_data.Slice(0x2), lengthLength: 2);

            public IScriptFragmentGetter? OnBegin { get; private set; }

            public IScriptFragmentGetter? OnEnd { get; private set; }

            int _onEndEnd;
            public IScriptFragmentGetter? OnChange => Flags.HasFlag(Flag.OnChange) ? ScriptFragmentBinaryOverlay.ScriptFragmentFactory(_data.Slice(_onEndEnd), _package) : default;

            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                var fileNameEnd = 0x2 + BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(0x2)) + 2;
                stream.Position = fileNameEnd;
                int onBeginEnd;
                if (Flags.HasFlag(Flag.OnBegin))
                {
                    stream.Position = fileNameEnd;
                    OnBegin = ScriptFragmentBinaryOverlay.ScriptFragmentFactory(stream, _package);
                    onBeginEnd = stream.Position;
                }
                else
                {
                    onBeginEnd = fileNameEnd;
                }
                if (Flags.HasFlag(Flag.OnEnd))
                {
                    stream.Position = onBeginEnd;
                    OnEnd = ScriptFragmentBinaryOverlay.ScriptFragmentFactory(stream, _package);
                    _onEndEnd = stream.Position;
                }
                else
                {
                    _onEndEnd = onBeginEnd;
                }
            }
        }
    }
}
