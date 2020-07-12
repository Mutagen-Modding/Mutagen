using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// A struct that overlays on top of bytes that is able to retrive Major Record header data on demand.
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
            this.Meta = meta;
            this.HeaderData = span.Slice(0, meta.MajorConstants.HeaderLength);
        }

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => Meta.Release;
        
        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => Meta.MajorConstants.HeaderLength;
        
        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.HeaderData.Slice(0, 4)));
        
        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Major Records, this is equivalent to ContentLength
        /// </summary>
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.HeaderData.Slice(4, this.Meta.MajorConstants.LengthLength));
        
        /// <summary>
        /// The length of the content of the Group, excluding the header bytes.
        /// </summary>
        public uint ContentLength => RecordLength;
        
        /// <summary>
        /// The integer representing a Major Record's flags enum.
        /// Since each game has its own flag Enum, this field is offered as an int that should
        /// be casted to the appropriate enum for use.
        /// </summary>
        public int MajorRecordFlags => BinaryPrimitives.ReadInt32LittleEndian(this.HeaderData.Slice(8, 4));

        /// <summary>
        /// FormID of the Major Record
        /// </summary>
        public FormID FormID => FormID.Factory(BinaryPrimitives.ReadUInt32LittleEndian(this.HeaderData.Slice(12, 4)));

        /// <summary>
        /// Version control of the Major Record
        /// </summary>
        public int VersionControl => BinaryPrimitives.ReadInt32LittleEndian(this.HeaderData.Slice(16, 4));

        /// <summary>
        /// Total length of the Major Record, including the header and its content.
        /// </summary>
        public long TotalLength => this.HeaderLength + this.ContentLength;
        
        /// <summary>
        /// Whether the compression flag is on
        /// </summary>
        public bool IsCompressed => (this.MajorRecordFlags & Mutagen.Bethesda.Internals.Constants.CompressedFlag) > 0;

        /// <summary>
        /// Returns the Form Version of the Major Record
        /// </summary>
        public short? FormVersion
        {
            get
            {
                if (!this.Meta.MajorConstants.FormVersionLocationOffset.HasValue) return null;
                return BinaryPrimitives.ReadInt16LittleEndian(
                    this.HeaderData.Slice(this.Meta.MajorConstants.FormVersionLocationOffset.Value));
            }
        }

        /// <summary>
        /// Returns the second Version Control of the Major Record
        /// </summary>
        public short? VersionControl2
        {
            get
            {
                if (!this.Meta.MajorConstants.FormVersionLocationOffset.HasValue) return null;
                return BinaryPrimitives.ReadInt16LittleEndian(
                    this.HeaderData.Slice(this.Meta.MajorConstants.FormVersionLocationOffset.Value + 2));
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"{RecordType} =>0x{ContentLength:X}";
    }

    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Major Record header data on demand.
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
            this.Meta = meta;
            this.HeaderData = span.Slice(0, meta.MajorConstants.HeaderLength);
        }

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => Meta.Release;
        
        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => Meta.MajorConstants.HeaderLength;
        
        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType
        {
            get => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.HeaderData.Slice(0, 4)));
            set => BinaryPrimitives.WriteInt32LittleEndian(this.HeaderData.Slice(0, 4), value.TypeInt);
        }
        
        /// <summary>
        /// The length of the content of the Group, excluding the header bytes.
        /// </summary>
        public uint ContentLength
        {
            get => RecordLength;
            set => RecordLength = value;
        }
        
        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Major Records, this is equivalent to ContentLength
        /// </summary>
        public uint RecordLength
        {
            get => BinaryPrimitives.ReadUInt32LittleEndian(this.HeaderData.Slice(4, 4));
            set => BinaryPrimitives.WriteUInt32LittleEndian(this.HeaderData.Slice(4, 4), value);
        }
        
        /// <summary>
        /// The integer representing a Major Record's flags enum.
        /// Since each game has its own flag Enum, this field is offered as an int that should
        /// be casted to the appropriate enum for use.
        /// </summary>
        public int MajorRecordFlags
        {
            get => BinaryPrimitives.ReadInt32LittleEndian(this.HeaderData.Slice(8, 4));
            set => BinaryPrimitives.WriteInt32LittleEndian(this.HeaderData.Slice(8, 4), value);
        }
        
        /// <summary>
        /// FormID of the Major Record
        /// </summary>
        public FormID FormID
        {
            get => FormID.Factory(BinaryPrimitives.ReadUInt32LittleEndian(this.HeaderData.Slice(12, 4)));
            set => BinaryPrimitives.WriteUInt32LittleEndian(this.HeaderData.Slice(12, 4), value.Raw);
        }

        /// <summary>
        /// Version control of the Major Record
        /// </summary>
        public int VersionControl
        {
            get => BinaryPrimitives.ReadInt32LittleEndian(this.HeaderData.Slice(16, 4));
            set => BinaryPrimitives.WriteInt32LittleEndian(this.HeaderData.Slice(16, 4), value);
        }

        /// <summary>
        /// Total length of the Major Record, including the header and its content.
        /// </summary>
        public long TotalLength => this.HeaderLength + this.RecordLength;

        /// <summary>
        /// Form Version of the Major Record
        /// </summary>
        [DisallowNull]
        public short? FormVersion
        {
            get
            {
                if (!this.Meta.MajorConstants.FormVersionLocationOffset.HasValue) return null;
                return BinaryPrimitives.ReadInt16LittleEndian(
                    this.HeaderData.Slice(this.Meta.MajorConstants.FormVersionLocationOffset.Value));
            }
            set
            {
                if (!this.Meta.MajorConstants.FormVersionLocationOffset.HasValue)
                {
                    throw new ArgumentException("Attempted to set Form Version on a non-applicable game.");
                }
                BinaryPrimitives.WriteInt16LittleEndian(
                    this.HeaderData.Slice(this.Meta.MajorConstants.FormVersionLocationOffset.Value, 2),
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
                if (!this.Meta.MajorConstants.FormVersionLocationOffset.HasValue) return null;
                return BinaryPrimitives.ReadInt16LittleEndian(
                    this.HeaderData.Slice(this.Meta.MajorConstants.FormVersionLocationOffset.Value + 2));
            }
            set
            {
                if (!this.Meta.MajorConstants.FormVersionLocationOffset.HasValue)
                {
                    throw new ArgumentException("Attempted to set Form Version on a non-applicable game.");
                }
                BinaryPrimitives.WriteInt16LittleEndian(
                    this.HeaderData.Slice(this.Meta.MajorConstants.FormVersionLocationOffset.Value + 2, 2),
                    value.Value);
            }
        }

        /// <summary>
        /// Whether the compression flag is on
        /// </summary>
        public bool IsCompressed
        {
            get => (this.MajorRecordFlags & Mutagen.Bethesda.Internals.Constants.CompressedFlag) > 0;
            set
            {
                if (value)
                {
                    this.MajorRecordFlags |= Mutagen.Bethesda.Internals.Constants.CompressedFlag;
                }
                else
                {
                    this.MajorRecordFlags &= ~Mutagen.Bethesda.Internals.Constants.CompressedFlag;
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"{RecordType} => 0x{ContentLength:X}";
    }
    
    /// <summary>
    /// A struct that overlays on top of bytes that is able to retrive Major Record data on demand.
    /// </summary>
    public struct MajorRecordFrame : IEnumerable<SubrecordFrame>
    {
        private readonly MajorRecordHeader _header;
        
        /// <summary>
        /// Raw bytes of both header and content data
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }
        
        /// <summary>
        /// Raw bytes of the content data, excluding the header
        /// </summary>
        public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(this._header.HeaderLength, checked((int)this._header.ContentLength));

        /// <summary>
        /// Total length of the Major Record, including the header and its content.
        /// </summary>
        public long TotalLength => this._header.HeaderLength + Content.Length;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public MajorRecordFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this._header = meta.MajorRecord(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this._header.TotalLength));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing MajorRecordHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public MajorRecordFrame(MajorRecordHeader header, ReadOnlyMemorySlice<byte> span)
        {
            this._header = header;
            this.HeaderAndContentData = span.Slice(0, checked((int)this._header.TotalLength));
        }

        /// <inheritdoc/>
        public override string ToString() => this._header.ToString();

        public IEnumerable<int> EnumerateSubrecordLocations()
        {
            int loc = Meta.MajorConstants.HeaderLength;
            while (loc < HeaderAndContentData.Length)
            {
                yield return loc;
                var subHeader = new SubrecordHeader(Meta, HeaderAndContentData.Slice(loc));
                loc += subHeader.TotalLength;
            }
        }

        public IEnumerable<(int Location, SubrecordFrame Subrecord)> EnumerateSubrecordFrames()
        {
            foreach (var loc in EnumerateSubrecordLocations())
            {
                yield return (loc, new SubrecordFrame(Meta, HeaderAndContentData.Slice(loc)));
            }
        }

        public IEnumerable<(int Location, SubrecordHeader Subrecord)> EnumerateSubrecords()
        {
            foreach (var loc in EnumerateSubrecordLocations())
            {
                yield return (loc, new SubrecordHeader(Meta, HeaderAndContentData.Slice(loc)));
            }
        }

        public IEnumerator<SubrecordFrame> GetEnumerator()
        {
            foreach (var loc in EnumerateSubrecordLocations())
            {
                yield return new SubrecordFrame(Meta, HeaderAndContentData.Slice(loc));
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #region Header Forwarding
        /// <summary>
        /// Raw bytes of header
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderData => _header.HeaderData;

        /// <summary>
        /// Game metadata to use as reference for alignment
        /// </summary>
        public GameConstants Meta => _header.Meta;

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => _header.Release;

        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => _header.HeaderLength;

        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => _header.RecordType;

        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Major Records, this is equivalent to ContentLength
        /// </summary>
        public uint RecordLength => _header.RecordLength;

        /// <summary>
        /// The length of the content of the Group, excluding the header bytes.
        /// </summary>
        public uint ContentLength => (uint)Content.Length;

        /// <summary>
        /// The integer representing a Major Record's flags enum.
        /// Since each game has its own flag Enum, this field is offered as an int that should
        /// be casted to the appropriate enum for use.
        /// </summary>
        public int MajorRecordFlags => _header.MajorRecordFlags;

        /// <summary>
        /// FormID of the Major Record
        /// </summary>
        public FormID FormID => _header.FormID;

        /// <summary>
        /// Version control of the Major Record
        /// </summary>
        public int VersionControl => _header.VersionControl;

        /// <summary>
        /// Whether the compression flag is on
        /// </summary>
        public bool IsCompressed => _header.IsCompressed;

        /// <summary>
        /// Returns the Form Version of the Major Record
        /// </summary>
        public short? FormVersion => _header.FormVersion;

        /// <summary>
        /// Returns the second Version Control of the Major Record
        /// </summary>
        public short? VersionControl2 => _header.VersionControl2;
        #endregion
    }
}
