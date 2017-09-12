using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public struct FormID
    {
        public readonly ModID ModID;
        public readonly uint ID;

        public FormID(ModID modID, uint id)
        {
            this.ModID = modID;
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

        public static bool TryFactory(string hexString, out FormID? id)
        {
            if (hexString.Length != 12)
            {
                id = null;
                return false;
            }

            try
            {
                id = new FormID(
                    new ModID(Convert.ToByte(hexString.Substring(0, 2), 16)),
                    Convert.ToUInt32(hexString.Substring(2, 8), 16));
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
            return $"{ModID}{ID.ToString("X8")}";
        }

        public override string ToString()
        {
            return $"({ModID}){ID.ToString("X8")}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FormID)) return false;
            FormID rhs = (FormID)obj;
            return this.ModID.Equals(rhs.ModID)
                && this.ID == rhs.ID;
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
