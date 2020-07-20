using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A struct representing a four character header for a record.
    /// These are used commonly in the binary format to delineate records and subrecords.
    /// </summary>
    [DebuggerDisplay("{Type}")]
    public struct RecordType : IEquatable<RecordType>, IEquatable<string>
    {
        /// <summary>
        /// The common length for all RecordTypes
        /// </summary>
        public const byte Length = 4;

        /// <summary>
        /// A static readonly singleton string representing a null RecordType
        /// </summary>
        public static readonly RecordType Null = new RecordType("\0\0\0\0");
        
        /// <summary>
        /// The type as an integer
        /// </summary>
        public readonly int TypeInt;

        /// <summary>
        /// The type as a four character string
        /// </summary>
        public string Type => GetStringType(this.TypeInt);

        /// <summary>
        /// The type as a four character string, checking that all characters are printable
        /// </summary>
        public string CheckedType => GetCheckedStringType(this.TypeInt);

        /// <summary>
        /// Constructor taking in an integer
        /// </summary>
        [DebuggerStepThrough]
        public RecordType (int type)
        {
            this.TypeInt = type;
        }

        /// <summary>
        /// Constructor taking in a string
        /// The integer constructor is preferable in most cases, as it is faster and can never throw an exception.
        /// </summary>
        /// <param name="typeStr">String of four characters</param>
        /// <exception cref="ArgumentException">If string does not contain exactly four characters</exception>
        [DebuggerStepThrough]
        public RecordType(ReadOnlySpan<char> typeStr)
        {
            if (typeStr.Length != Length)
            {
                throw new ArgumentException($"Type String not expected length: {typeStr.Length} != {Length}.");
            }
            this.TypeInt = GetTypeInt(typeStr);
        }

        /// <summary>
        /// Attempts to construct a RecordType from a string.
        /// Must be of size 4 to succeed.
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <param name="recType">RecordType if successfully converted</param>
        /// <returns>True if conversion successful</returns>
        public static bool TryFactory(ReadOnlySpan<char> str, out RecordType recType)
        {
            if (str.Length != Length)
            {
                recType = default;
                return false;
            }
            recType = new RecordType(GetTypeInt(str));
            return true;
        }

        /// <summary>
        /// Default equality operator
        /// </summary>
        /// <param name="other">object to compare to</param>
        /// <returns>True if RecordType with equal TypeInt</returns>
        public override bool Equals(object other)
        {
            if (!(other is RecordType rhs)) return false;
            return Equals(rhs);
        }

        /// <summary>
        /// RecordType equality operator
        /// </summary>
        /// <param name="other">RecordType to compare to</param>
        /// <returns>True if equal TypeInt value</returns>
        public bool Equals(RecordType other)
        {
            return this.TypeInt == other.TypeInt;
        }

        /// <summary>
        /// String equality operator
        /// </summary>
        /// <param name="other">String to compare to</param>
        /// <returns>True if equal Type string value</returns>
        public bool Equals(string other)
        {
            if (string.IsNullOrWhiteSpace(other)) return false;
            if (other.Length != 4) return false;
            return this.TypeInt == GetTypeInt(other);
        }

        public static bool operator ==(RecordType r1, RecordType r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(RecordType r1, RecordType r2)
        {
            return !r1.Equals(r2);
        }

        /// <summary>
        /// Hashcode retrieved from TypeInt value.
        /// </summary>
        /// <returns>Hashcode retrieved from TypeInt value.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(TypeInt);
        }

        /// <summary>
        /// Converts to a four character string.
        /// </summary>
        /// <returns>String representation of RecordType</returns>
        public override string ToString()
        {
            return this.Type;
        }

        /// <summary>
        /// Converts an integer to its string RecordType representation
        /// </summary>
        /// <param name="typeInt">Integer to convert</param>
        /// <returns>Four character string</returns>
        [DebuggerStepThrough]
        public static string GetStringType(int typeInt)
        {
            return string.Create(4, typeInt, (chars, state) =>
            {
                chars[0] = (char)(state & 0x000000FF);
                chars[1] = (char)(state >> 8 & 0x000000FF);
                chars[2] = (char)(state >> 16 & 0x000000FF);
                chars[3] = (char)(state >> 24 & 0x000000FF);
            });
        }

        /// <summary>
        /// Converts an integer to its string RecordType representation.
        /// If unprintable characters are encountered, they will be modified
        /// </summary>
        /// <param name="typeInt">Integer to convert</param>
        /// <returns>Character string</returns>
        public static string GetCheckedStringType(int typeInt)
        {
            var ret = GetStringType(typeInt);
            for (int i = ret.Length - 1; i >= 0; i--)
            {
                var b = (byte)ret[i];
                if (b > 0x14) continue;
                ret = ret.Remove(i, 1);
                ret = ret.Insert(i, $"_{b:X}_");
            }
            return ret;
        }

        /// <summary>
        /// Converts an string to its int RecordType representation
        /// </summary>
        /// <param name="typeStr">Four character string to convert</param>
        /// <returns>Integer representing the record type</returns>
        /// <exception cref="ArgumentException">If string does not contain exactly four characters</exception>
        [DebuggerStepThrough]
        public static int GetTypeInt(ReadOnlySpan<char> typeStr)
        {
            if (typeStr.Length != Length)
            {
                throw new ArgumentException($"Type String not expected length: {Length}.");
            }
            Span<byte> b = stackalloc byte[4];
            for (int i = 0; i < Length; i++)
            {
                b[i] = (byte)typeStr[i];
            }
            return BinaryPrimitives.ReadInt32LittleEndian(b);
        }

        public static implicit operator RecordType(ReadOnlySpan<char> str)
        {
            return new RecordType(str);
        }

        public static implicit operator RecordType(string str)
        {
            return new RecordType(str);
        }
    }
}
