using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using static Mutagen.Bethesda.Skyrim.Internals.DialogResponsesScriptFragmentsBinaryCreateTranslation;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class DialogResponsesScriptFragmentsBinaryCreateTranslation
        {
            [Flags]
            public enum Flag
            {
                OnBegin = 0x01,
                OnEnd = 0x02,
            }

            static partial void FillBinaryFlagsCustom(MutagenFrame frame, IDialogResponsesScriptFragments item)
            {
                var flag = (Flag)frame.ReadUInt8();
                item.FileName = Mutagen.Bethesda.Binary.StringBinaryTranslation.Instance.Parse(
                     frame: frame,
                     stringBinaryType: StringBinaryType.PrependLengthUShort);
                if (flag.HasFlag(Flag.OnBegin))
                {
                    item.OnBegin = ScriptFragment.CreateFromBinary(frame);
                }
                if (flag.HasFlag(Flag.OnEnd))
                {
                    item.OnEnd = ScriptFragment.CreateFromBinary(frame);
                }
            }
        }

        public partial class DialogResponsesScriptFragmentsBinaryWriteTranslation
        {
            static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IDialogResponsesScriptFragmentsGetter item)
            {
                var begin = item.OnBegin;
                var end = item.OnEnd;
                Flag flag = default;
                if (begin != null)
                {
                    flag |= Flag.OnBegin;
                }
                if (end != null)
                {
                    flag |= Flag.OnEnd;
                }
                writer.Write((byte)flag);
                Mutagen.Bethesda.Binary.StringBinaryTranslation.Instance.Write(
                    writer: writer,
                    item: item.FileName,
                    binaryType: StringBinaryType.PrependLengthUShort);
                begin?.WriteToBinary(writer);
                end?.WriteToBinary(writer);
            }
        }

        public partial class DialogResponsesScriptFragmentsBinaryOverlay
        {
            Flag Flags => (Flag)_data.Span.Slice(0x1, 0x1)[0];

            public string FileName => BinaryStringUtility.ParsePrependedString(_data.Slice(0x2), lengthLength: 2);

            public IScriptFragmentGetter? OnBegin { get; private set; }

            int _onBeginEnd;
            public IScriptFragmentGetter? OnEnd => Flags.HasFlag(Flag.OnEnd) ? ScriptFragmentBinaryOverlay.ScriptFragmentFactory(_data.Slice(_onBeginEnd), _package) : default;

            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                var fileNameEnd = 0x2 + BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(0x2)) + 2;
                stream.Position = fileNameEnd;
                if (Flags.HasFlag(Flag.OnBegin))
                {
                    stream.Position = fileNameEnd;
                    OnBegin = ScriptFragmentBinaryOverlay.ScriptFragmentFactory(stream, _package);
                    _onBeginEnd = stream.Position;
                }
                else
                {
                    _onBeginEnd = fileNameEnd;
                }
            }
        }
    }
}
