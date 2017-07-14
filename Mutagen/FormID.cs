using System;
using System.Collections.Generic;
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
            FormID? idNull;
            if (TryFactory(hexString, out idNull))
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
                    new ModID(Convert.ToUInt16(hexString.Substring(0, 4), 16)),
                    Convert.ToUInt32(hexString.Substring(4, 8), 16));
                return true;
            }
            catch (Exception)
            {
                id = null;
                return false;
            }
        }

        public string ToHex()
        {
            return ModID + ID.ToString("X8");
        }

        public override string ToString()
        {
            return "(" + ModID + ")" + ID.ToString("X8");
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
    }
}
