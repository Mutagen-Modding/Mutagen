using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public struct FormID : IEquatable<FormID>
    {
        public static readonly FormID NULL = new FormID();
        public readonly uint Raw;
        public ModID ModID => new ModID(ModID.GetModIDByteFromUInt(this.Raw));
        public uint ID => this.Raw & 0x00FFFFFF;

        public FormID(ModID modID, uint id)
        {
            this.Raw = (uint)(modID.ID << 24);
            this.Raw += this.Raw + id & 0x00FFFFFF;
        }

        public FormID(uint idWithModID)
        {
            this.Raw = idWithModID;
        }

        public static bool TryFactory(string hexString, out FormID id)
        {
            if (TryFactory(hexString, out FormID? idNull))
            {
                id = idNull ?? default(FormID);
                return true;
            }

            id = default(FormID);
            return false;
        }

        public static FormID Factory(string hexString)
        {
            if (!TryFactory(hexString, out FormID result))
            {
                throw new ArgumentException($"Invalid formID hex: {hexString}");
            }
            return result;
        }

        public static bool TryFactory(string hexString, out FormID? id)
        {
            if (hexString.StartsWith("0x"))
            {
                hexString = hexString.Substring(2);
            }

            if (hexString.Length != 8)
            {
                id = null;
                return false;
            }

            try
            {
                id = new FormID(
                    Convert.ToUInt32(hexString, 16));
                return true;
            }
            catch (Exception)
            {
                id = null;
                return false;
            }
        }

        public static FormID Factory(byte[] bytes)
        {
            return Factory(BitConverter.ToUInt32(bytes, 0));
        }

        public static FormID Factory(uint i)
        {
            return new FormID(i);
        }

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(this.Raw);
        }

        public string ToHex()
        {
            return $"{ModID}{IDString()}";
        }

        public override string ToString()
        {
            return $"({ModID}){IDString()}";
        }

        public string IDString()
        {
            return ID.ToString("X6");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FormID formID)) return false;
            return Equals(formID);
        }

        public bool Equals(FormID other)
        {
            return this.Raw == other.Raw;
        }

        public override int GetHashCode()
        {
            return this.Raw.GetHashCode();
        }

        public static bool operator ==(FormID a, FormID b)
        {
            return a.Raw == b.Raw;
        }

        public static bool operator !=(FormID a, FormID b)
        {
            return !(a == b);
        }
    }
}
