using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins.Binary.Headers
{
    /// <summary>
    /// A struct that overlays on top of bytes that is able to retrieve Major Record header data on demand.
    /// </summary>
    public struct MajorRecordHeader
    {
        /// <summary>
        /// Game metadata to use as reference for alignment
        /// </summary>
        public GameConstants Meta { get; }
        
        /// <summary>
        /// Bytes overlaid onto
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderData { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Major Record's header</param>
        public MajorRecordHeader(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            Meta = meta;
            HeaderData = span.Slice(0, meta.MajorConstants.HeaderLength);
        }

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => Meta.Release;
        
        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public byte HeaderLength => Meta.MajorConstants.HeaderLength;
        
        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => new(BinaryPrimitives.ReadInt32LittleEndian(HeaderData.Slice(0, 4)));
        
        /// <summary>
        /// The length of the content of the MajorRecord, excluding the header bytes.
        /// </summary>
        public uint ContentLength => BinaryPrimitives.ReadUInt32LittleEndian(HeaderData.Slice(4, Meta.MajorConstants.LengthLength));

        /// <summary>
        /// The integer representing a Major Record's flags enum.
        /// Since each game has its own flag Enum, this field is offered as an int that should
        /// be casted to the appropriate enum for use.
        /// </summary>
        public int MajorRecordFlags => BinaryPrimitives.ReadInt32LittleEndian(HeaderData.Slice(8, 4));

        /// <summary>
        /// FormID of the Major Record
        /// </summary>
        public FormID FormID => FormID.Factory(BinaryPrimitives.ReadUInt32LittleEndian(HeaderData.Slice(12, 4)));

        /// <summary>
        /// Version control of the Major Record
        /// </summary>
        public int VersionControl => BinaryPrimitives.ReadInt32LittleEndian(HeaderData.Slice(16, 4));

        /// <summary>
        /// Total length of the Major Record, including the header and its content.
        /// </summary>
        public long TotalLength => HeaderLength + ContentLength;
        
        /// <summary>
        /// Whether the compression flag is on
        /// </summary>
        public bool IsCompressed => (MajorRecordFlags & Constants.CompressedFlag) > 0;

        /// <summary>
        /// Returns the Form Version of the Major Record
        /// </summary>
        public short? FormVersion
        {
            get
            {
                if (!Meta.MajorConstants.FormVersionLocationOffset.HasValue) return null;
                return BinaryPrimitives.ReadInt16LittleEndian(
                    HeaderData.Slice(Meta.MajorConstants.FormVersionLocationOffset.Value));
            }
        }

        /// <summary>
        /// Returns the second Version Control of the Major Record
        /// </summary>
        public short? VersionControl2
        {
            get
            {
                if (!Meta.MajorConstants.FormVersionLocationOffset.HasValue) return null;
                return BinaryPrimitives.ReadInt16LittleEndian(
                    HeaderData.Slice(Meta.MajorConstants.FormVersionLocationOffset.Value + 2));
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"{RecordType.ToString()} =>0x{ContentLength.ToString("X")}";
    }

    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrieve Major Record header data on demand.
    /// It requires to be overlaid on writable bytes, so that values can also be set and modified on the source bytes.
    /// </summary>
    public ref struct MajorRecordHeaderWritable
    {
        /// <summary>
        /// Game metadata to use as reference for alignment
        /// </summary>
        public GameConstants Meta { get; }
        
        /// <summary>
        /// Bytes overlaid onto
        /// </summary>
        public Span<byte> HeaderData { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Major Record's header</param>
        public MajorRecordHeaderWritable(GameConstants meta, Span<byte> span)
        {
            Meta = meta;
            HeaderData = span.Slice(0, meta.MajorConstants.HeaderLength);
        }

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => Meta.Release;
        
        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public byte HeaderLength => Meta.MajorConstants.HeaderLength;
        
        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType
        {
            get => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(HeaderData.Slice(0, 4)));
            set => BinaryPrimitives.WriteInt32LittleEndian(HeaderData.Slice(0, 4), value.TypeInt);
        }
        
        /// <summary>
        /// The length of the content of the MajorRecord, excluding the header bytes.
        /// </summary>
        public uint ContentLength
        {
            get => BinaryPrimitives.ReadUInt32LittleEndian(HeaderData.Slice(4, 4));
            set => BinaryPrimitives.WriteUInt32LittleEndian(HeaderData.Slice(4, 4), value);
        }
        
        /// <summary>
        /// The integer representing a Major Record's flags enum.
        /// Since each game has its own flag Enum, this field is offered as an int that should
        /// be casted to the appropriate enum for use.
        /// </summary>
        public int MajorRecordFlags
        {
            get => BinaryPrimitives.ReadInt32LittleEndian(HeaderData.Slice(8, 4));
            set => BinaryPrimitives.WriteInt32LittleEndian(HeaderData.Slice(8, 4), value);
        }
        
        /// <summary>
        /// FormID of the Major Record
        /// </summary>
        public FormID FormID
        {
            get => FormID.Factory(BinaryPrimitives.ReadUInt32LittleEndian(HeaderData.Slice(12, 4)));
            set => BinaryPrimitives.WriteUInt32LittleEndian(HeaderData.Slice(12, 4), value.Raw);
        }

        /// <summary>
        /// Version control of the Major Record
        /// </summary>
        public int VersionControl
        {
            get => BinaryPrimitives.ReadInt32LittleEndian(HeaderData.Slice(16, 4));
            set => BinaryPrimitives.WriteInt32LittleEndian(HeaderData.Slice(16, 4), value);
        }

        /// <summary>
        /// Total length of the Major Record, including the header and its content.
        /// </summary>
        public long TotalLength => HeaderLength + ContentLength;

        /// <summary>
        /// Form Version of the Major Record
        /// </summary>
        [DisallowNull]
        public short? FormVersion
        {
            get
            {
                if (!Meta.MajorConstants.FormVersionLocationOffset.HasValue) return null;
                return BinaryPrimitives.ReadInt16LittleEndian(
                    HeaderData.Slice(Meta.MajorConstants.FormVersionLocationOffset.Value));
            }
            set
            {
                if (!Meta.MajorConstants.FormVersionLocationOffset.HasValue)
                {
                    throw new ArgumentException("Attempted to set Form Version on a non-applicable game.");
                }
                BinaryPrimitives.WriteInt16LittleEndian(
                    HeaderData.Slice(Meta.MajorConstants.FormVersionLocationOffset.Value, 2),
                    value.Value);
            }
        }

        /// <summary>
        /// Second Version Control of the Major Record
        /// </summary>
        [DisallowNull]
        public short? VersionControl2
        {
            get
            {
                if (!Meta.MajorConstants.FormVersionLocationOffset.HasValue) return null;
                return BinaryPrimitives.ReadInt16LittleEndian(
                    HeaderData.Slice(Meta.MajorConstants.FormVersionLocationOffset.Value + 2));
            }
            set
            {
                if (!Meta.MajorConstants.FormVersionLocationOffset.HasValue)
                {
                    throw new ArgumentException("Attempted to set Form Version on a non-applicable game.");
                }
                BinaryPrimitives.WriteInt16LittleEndian(
                    HeaderData.Slice(Meta.MajorConstants.FormVersionLocationOffset.Value + 2, 2),
                    value.Value);
            }
        }

        /// <summary>
        /// Whether the compression flag is on
        /// </summary>
        public bool IsCompressed
        {
            get => (MajorRecordFlags & Constants.CompressedFlag) > 0;
            set
            {
                if (value)
                {
                    MajorRecordFlags |= Constants.CompressedFlag;
                }
                else
                {
                    MajorRecordFlags &= ~Constants.CompressedFlag;
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"{RecordType.ToString()} => 0x{ContentLength.ToString("X")}";
    }

    /// <summary>
    /// A struct that overlays on top of bytes that is able to retrive Major Record header and content data on demand.
    /// </summary>
    public struct MajorRecordFrame : IEnumerable<SubrecordPinFrame>
    {
        public readonly MajorRecordHeader Header;
        
        /// <summary>
        /// Raw bytes of both header and content data
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }
        
        /// <summary>
        /// Raw bytes of the content data, excluding the header
        /// </summary>
        public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(Header.HeaderLength, checked((int)this.Header.ContentLength));

        /// <summary>
        /// Total length of the Major Record, including the header and its content.
        /// </summary>
        public long TotalLength => HeaderAndContentData.Length;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public MajorRecordFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            Header = meta.MajorRecord(span);
            HeaderAndContentData = span.Slice(0, checked((int)Header.TotalLength));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing MajorRecordHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public MajorRecordFrame(MajorRecordHeader header, ReadOnlyMemorySlice<byte> span)
        {
            Header = header;
            HeaderAndContentData = span.Slice(0, checked((int)Header.TotalLength));
        }

        /// <inheritdoc/>
        public override string ToString() => Header.ToString();

        /// <inheritdoc/>
        public IEnumerator<SubrecordPinFrame> GetEnumerator() => HeaderExt.EnumerateSubrecords(this).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Header Forwarding
        /// <summary>
        /// Raw bytes of header
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderData => Header.HeaderData;

        /// <summary>
        /// Game metadata to use as reference for alignment
        /// </summary>
        public GameConstants Meta => Header.Meta;

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => Header.Release;

        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public byte HeaderLength => Header.HeaderLength;

        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => Header.RecordType;

        /// <summary>
        /// The length of the content of the MajorRecord, excluding the header bytes.
        /// </summary>
        public uint ContentLength => (uint)Content.Length;

        /// <summary>
        /// The integer representing a Major Record's flags enum.
        /// Since each game has its own flag Enum, this field is offered as an int that should
        /// be casted to the appropriate enum for use.
        /// </summary>
        public int MajorRecordFlags => Header.MajorRecordFlags;

        /// <summary>
        /// FormID of the Major Record
        /// </summary>
        public FormID FormID => Header.FormID;

        /// <summary>
        /// Version control of the Major Record
        /// </summary>
        public int VersionControl => Header.VersionControl;

        /// <summary>
        /// Whether the compression flag is on
        /// </summary>
        public bool IsCompressed => Header.IsCompressed;

        /// <summary>
        /// Returns the Form Version of the Major Record
        /// </summary>
        public short? FormVersion => Header.FormVersion;

        /// <summary>
        /// Returns the second Version Control of the Major Record
        /// </summary>
        public short? VersionControl2 => Header.VersionControl2;
        #endregion
    }

    /// <summary>
    /// A struct that overlays on top of bytes that is able to retrive Major Record data on demand.
    /// In addition, it keeps track of its location relative to its parent MajorRecordFrame
    /// </summary>
    public struct MajorRecordPinFrame
    {
        /// <summary>
        /// Frame struct contained in the pin
        /// </summary>
        public MajorRecordFrame Frame { get; }

        /// <summary>
        /// Location of the major record relative to the parent GroupFrame's data.<br/>
        /// E.g., relative to the position of the RecordType of the parent MajorRecord.
        /// </summary>
        public int Location { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        /// <param name="pinLocation">Location pin tracker relative to parent MajorRecordFrame</param>
        public MajorRecordPinFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span, int pinLocation)
        {
            Frame = new MajorRecordFrame(meta, span);
            Location = pinLocation;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing MajorRecordHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        /// <param name="pinLocation">Location pin tracker relative to parent MajorRecordFrame</param>
        public MajorRecordPinFrame(MajorRecordHeader header, ReadOnlyMemorySlice<byte> span, int pinLocation)
        {
            Frame = new MajorRecordFrame(header, span);
            Location = pinLocation;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Frame.ToString()} @ {Location.ToString()}";

        #region Header Forwarding
        /// <summary>
        /// Header struct contained in the pin
        /// </summary>
        public MajorRecordHeader Header => Frame.Header;

        /// <summary>
        /// Total length of the Major Record, including the header and its content.
        /// </summary>
        public long TotalLength => Header.TotalLength;

        /// <summary>
        /// Raw bytes of header
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderData => Header.HeaderData;

        /// <summary>
        /// Game metadata to use as reference for alignment
        /// </summary>
        public GameConstants Meta => Header.Meta;

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => Header.Release;

        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public byte HeaderLength => Header.HeaderLength;

        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => Header.RecordType;

        /// <summary>
        /// The length of the content of the MajorRecord, excluding the header bytes.
        /// </summary>
        public uint ContentLength => Frame.ContentLength;

        /// <summary>
        /// The integer representing a Major Record's flags enum.
        /// Since each game has its own flag Enum, this field is offered as an int that should
        /// be casted to the appropriate enum for use.
        /// </summary>
        public int MajorRecordFlags => Header.MajorRecordFlags;

        /// <summary>
        /// FormID of the Major Record
        /// </summary>
        public FormID FormID => Header.FormID;

        /// <summary>
        /// Version control of the Major Record
        /// </summary>
        public int VersionControl => Header.VersionControl;

        /// <summary>
        /// Whether the compression flag is on
        /// </summary>
        public bool IsCompressed => Header.IsCompressed;

        /// <summary>
        /// Returns the Form Version of the Major Record
        /// </summary>
        public short? FormVersion => Header.FormVersion;

        /// <summary>
        /// Returns the second Version Control of the Major Record
        /// </summary>
        public short? VersionControl2 => Header.VersionControl2;
        #endregion

        public static implicit operator MajorRecordHeader(MajorRecordPinFrame pin)
        {
            return pin.Header;
        }

        public static implicit operator MajorRecordFrame(MajorRecordPinFrame pin)
        {
            return pin.Frame;
        }
    }
}
