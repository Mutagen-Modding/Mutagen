using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public struct RecordType : IEquatable<RecordType>, IEquatable<string>
    {
        public readonly int TypeInt;
        public const byte HEADER_LENGTH = 4;
        public string Type => GetStringType(this.TypeInt);

        public static readonly RecordType Null = new RecordType("\0\0\0\0");

        [DebuggerStepThrough]
        public RecordType (int type)
        {
            this.TypeInt = type;
        }
        
        public RecordType(string type)
        {
            if (type == null
                || type.Length != HEADER_LENGTH)
            {
                throw new ArgumentException($"Type String not expected length: {HEADER_LENGTH}.");
            }
            this.TypeInt = GetTypeInt(type);
        }

        public static bool TryFactory(string str, out RecordType recType)
        {
            if (str == null
                || str.Length != HEADER_LENGTH)
            {
                recType = default;
                return false;
            }
            recType = new RecordType(GetTypeInt(str));
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RecordType rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(RecordType other)
        {
            return this.TypeInt == other.TypeInt;
        }

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

        public override int GetHashCode()
        {
            return this.TypeInt.GetHashCode();
        }

        public override string ToString()
        {
            return this.Type;
        }

        public string GetStringType(int typeInt)
        {
            char[] chars = new char[HEADER_LENGTH];
            chars[0] = (char)(typeInt & 0x000000FF);
            chars[1] = (char)(typeInt >> 8 & 0x000000FF);
            chars[2] = (char)(typeInt >> 16 & 0x000000FF);
            chars[3] = (char)(typeInt >> 24 & 0x000000FF);
            return new string(chars);
        }

        public static int GetTypeInt(string type)
        {
            byte[] b = new byte[4];
            for (int i = 0; i < HEADER_LENGTH; i++)
            {
                b[i] = (byte)type[i];
            }
            return BitConverter.ToInt32(b, 0);
        }
    }
}
