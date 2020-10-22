using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Reference for all the alignment and length constants related to a specific game
    /// </summary>
    public class GameConstants
    {
        /// <summary>
        /// Associated game type
        /// </summary>
        public GameRelease Release { get; }
        
        /// <summary>
        /// Length of the Mod header's metadata, excluding content
        /// </summary>
        public sbyte ModHeaderLength { get; }
        
        /// <summary>
        /// Length of the Mod header's non-fundamental metadata
        /// </summary>
        public sbyte ModHeaderFluffLength { get; }

        /// <summary>
        /// Group constants
        /// </summary>
        public RecordHeaderConstants GroupConstants { get; }
        
        /// <summary>
        /// Major Record constants
        /// </summary>
        public MajorRecordConstants MajorConstants { get; }
        
        /// <summary>
        /// Sub Record constants
        /// </summary>
        public RecordHeaderConstants SubConstants { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="release">Game Release to associate with the constants</param>
        /// <param name="modHeaderLength">Length of the ModHeader</param>
        /// <param name="modHeaderFluffLength">Length of the ModHeader excluding initial recordtype and length bytes.</param>
        /// <param name="groupConstants">Constants defining Groups</param>
        /// <param name="majorConstants">Constants defining Major Records</param>
        /// <param name="subConstants">Constants defining Sub Records</param>
        public GameConstants(
            GameRelease release,
            sbyte modHeaderLength,
            sbyte modHeaderFluffLength,
            RecordHeaderConstants groupConstants,
            MajorRecordConstants majorConstants,
            RecordHeaderConstants subConstants)
        {
            Release = release;
            ModHeaderLength = modHeaderLength;
            ModHeaderFluffLength = modHeaderFluffLength;
            GroupConstants = groupConstants;
            MajorConstants = majorConstants;
            SubConstants = subConstants;
        }

        public GameConstants(
            GameConstants rhs, 
            GameRelease releaseOverride)
        {
            Release = releaseOverride;
            ModHeaderLength = rhs.ModHeaderLength;
            ModHeaderFluffLength = rhs.ModHeaderFluffLength;
            GroupConstants = rhs.GroupConstants;
            MajorConstants = rhs.MajorConstants;
            SubConstants = rhs.SubConstants;
        }

        /// <summary>
        /// Readonly singleton of Oblivion game constants
        /// </summary>
        public static readonly GameConstants Oblivion = new GameConstants(
            release: GameRelease.Oblivion,
            modHeaderLength: 20,
            modHeaderFluffLength: 12,
            groupConstants: new RecordHeaderConstants(
                ObjectType.Group,
                headerLength: 20,
                lengthLength: 4),
            majorConstants: new MajorRecordConstants(
                headerLength: 20,
                lengthLength: 4,
                flagsLoc: 8,
                formIDloc: 12,
                formVersionLoc: null),
            subConstants: new RecordHeaderConstants(
                ObjectType.Subrecord,
                headerLength: 6,
                lengthLength: 2));

        /// <summary>
        /// Readonly singleton of Skyrim LE game constants
        /// </summary>
        public static readonly GameConstants SkyrimLE = new GameConstants(
            release: GameRelease.SkyrimLE,
            modHeaderLength: 24,
            modHeaderFluffLength: 16,
            groupConstants: new RecordHeaderConstants(
                ObjectType.Group,
                headerLength: 24,
                lengthLength: 4),
            majorConstants: new MajorRecordConstants(
                headerLength: 24,
                lengthLength: 4,
                flagsLoc: 8,
                formIDloc: 12,
                formVersionLoc: 20),
            subConstants: new RecordHeaderConstants(
                ObjectType.Subrecord,
                headerLength: 6,
                lengthLength: 2));

        /// <summary>
        /// Readonly singleton of Skyrim SE game constants
        /// </summary>
        public static readonly GameConstants SkyrimSE = new GameConstants(SkyrimLE, GameRelease.SkyrimSE);

        #region Header Factories
        public ModHeader ModHeader(ReadOnlyMemorySlice<byte> span) => new ModHeader(this, span);

        public GroupHeader Group(ReadOnlyMemorySlice<byte> span) => new GroupHeader(this, span);

        public GroupFrame GroupFrame(ReadOnlyMemorySlice<byte> span) => new GroupFrame(this, span);

        public MajorRecordHeader MajorRecord(ReadOnlyMemorySlice<byte> span) => new MajorRecordHeader(this, span);

        public MajorRecordHeaderWritable MajorRecordWritable(Span<byte> span) => new MajorRecordHeaderWritable(this, span);

        public MajorRecordFrame MajorRecordFrame(ReadOnlyMemorySlice<byte> span) => new MajorRecordFrame(this, span);

        public SubrecordHeader Subrecord(ReadOnlyMemorySlice<byte> span) => new SubrecordHeader(this, span);

        public SubrecordHeader Subrecord(ReadOnlyMemorySlice<byte> span, RecordType targetType)
        {
            var meta = new SubrecordHeader(this, span);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return meta;
        }

        public bool TrySubrecord(ReadOnlyMemorySlice<byte> span, RecordType targetType, out SubrecordHeader header)
        {
            if (span.Length < this.SubConstants.HeaderLength)
            {
                header = default;
                return false;
            }
            header = new SubrecordHeader(this, span);
            if (header.RecordType != targetType)
            {
                header = default;
                return false;
            }
            return true;
        }

        public SubrecordFrame SubrecordFrame(ReadOnlyMemorySlice<byte> span) => new SubrecordFrame(this, span);

        public SubrecordFrame SubrecordFrame(ReadOnlyMemorySlice<byte> span, RecordType targetType)
        {
            var meta = new SubrecordHeader(this, span);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return Binary.SubrecordFrame.Factory(meta, span);
        }

        public bool TrySubrecordFrame(ReadOnlyMemorySlice<byte> span, RecordType targetType, out SubrecordFrame header)
        {
            if (span.Length < this.SubConstants.HeaderLength)
            {
                header = default;
                return false;
            }
            var meta = new SubrecordHeader(this, span);
            if (meta.RecordType != targetType)
            {
                header = default;
                return false;
            }
            header = Binary.SubrecordFrame.Factory(meta, span);
            return true;
        }

        public VariableHeader NextRecordVariableMeta(ReadOnlyMemorySlice<byte> span)
        {
            RecordType rec = new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span));
            if (rec == Mutagen.Bethesda.Internals.Constants.Group)
            {
                return this.GroupConstants.VariableMeta(span);
            }
            else
            {
                return this.MajorConstants.VariableMeta(span);
            }
        }
        #endregion

        /// <summary>
        /// Returns record constants related to a certain ObjectType
        /// </summary>
        /// <param name="type">ObjectType to query</param>
        /// <returns>Record Constants associated with type</returns>
        public RecordHeaderConstants Constants(ObjectType type)
        {
            return type switch
            {
                ObjectType.Subrecord => SubConstants,
                ObjectType.Record => MajorConstants,
                ObjectType.Group => GroupConstants,
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Returns GameConstant readonly singleton associated with a game release 
        /// </summary>
        /// <param name="release">Game Release to query</param>
        /// <returns>GameConstant readonly singleton associated with mode</returns>
        public static GameConstants Get(GameRelease release)
        {
            switch (release)
            {
                case GameRelease.Oblivion:
                    return Oblivion;
                case GameRelease.SkyrimLE:
                    return SkyrimLE;
                case GameRelease.SkyrimSE:
                    return SkyrimSE;
                default:
                    throw new NotImplementedException();
            }
        }

        public static implicit operator GameConstants(GameRelease mode)
        {
            return Get(mode);
        }
    }
}
