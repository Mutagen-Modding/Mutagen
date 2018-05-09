using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IFormID
    {
        FormID FormID { get; }
    }

    public struct FormID : IEquatable<FormID>
    {
        public static readonly FormID NULL = new FormID();
        public readonly ModID ModID;
        public readonly uint ID;

        public FormID(ModID modID, uint id)
        {
            this.ModID = modID;
            this.ID = id;
        }

        public FormID(uint id)
        {
            this.ModID = new ModID(0);
            this.ID = id;
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
                    new ModID(Convert.ToByte(hexString.Substring(0, 2), 16)),
                    Convert.ToUInt32(hexString.Substring(2, 6), 16));
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
            byte[] arr = new byte[4];
            Array.Copy(bytes, 0, arr, 0, 3);
            var i = BitConverter.ToUInt32(arr, 0);
            return new FormID(
                new ModID(bytes[3]),
                i);
        }

        public byte[] ToBytes()
        {
            byte[] arr = new byte[4];
            var bytes = BitConverter.GetBytes(this.ID);
            Array.Copy(bytes, 0, arr, 0, 3);
            bytes[3] = this.ModID.ID;
            return bytes;
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
            return ID.ToString("X8");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FormID formID)) return false;
            return Equals(formID);
        }

        public bool Equals(FormID other)
        {
            return this.ModID.Equals(other.ModID)
                && this.ID == other.ID;
        }

        public override int GetHashCode()
        {
            return this.ModID.GetHashCode()
                .CombineHashCode(this.ID.GetHashCode());
        }

        public static bool operator ==(FormID a, FormID b)
        {
            return a.ModID == b.ModID
                && a.ID == b.ID;
        }

        public static bool operator !=(FormID a, FormID b)
        {
            return !(a == b);
        }
    }
}
