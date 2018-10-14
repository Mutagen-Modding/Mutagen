using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public struct FormKey : IEquatable<FormKey>
    {
        public const string NULL_STR = "NULL";
        public static readonly FormKey NULL = new FormKey(ModKey.NULL, 0);
        public readonly uint ID;
        public readonly ModKey ModKey;

        public FormKey(ModKey modKey, uint id)
        {
            this.ModKey = modKey;
            this.ID = id & 0xFFFFFF;
        }

        public static FormKey Factory(MasterReferences masterReferences, uint id)
        {
            var modID = (byte)(id >> 24);

            if (modID >= masterReferences.Masters.Count)
            {
                return new FormKey(
                    masterReferences.CurrentMod,
                    id);
            }

            var master = masterReferences.Masters[modID];
            return new FormKey(
                master.Master,
                id);
        }

        public static bool TryFactory(string str, out FormKey formKey)
        {
            if (NULL_STR.Equals(str))
            {
                formKey = NULL;
                return true;
            }
            if (string.IsNullOrWhiteSpace(str))
            {
                formKey = default(FormKey);
                return false;
            }

            if (str.StartsWith("0x"))
            {
                str = str.Substring(2);
            }

            if (str.Length < 6)
            {
                formKey = default(FormKey);
                return false;
            }

            uint id;
            try
            {
                id = Convert.ToUInt32(str.Substring(0, 6), 16);
            }
            catch (Exception)
            {
                formKey = default(FormKey);
                return false;
            }

            var split = str
                .Substring(6)
                .Split('.');
            if (split.Length != 2)
            {
                formKey = default(FormKey);
                return false;
            }

            if (!ModKey.TryFactory(str.Substring(6), out var modKey))
            {
                formKey = default(FormKey);
                return false;
            }
            
            formKey = new FormKey(
                modKey: modKey,
                id: id);
            return true;
        }

        public override string ToString()
        {
            return $"{IDString()}{this.ModKey}";
        }

        public string IDString()
        {
            return ID.ToString("X6");
        }

        public FormID GetFormID(MasterReferences list)
        {
            for (byte i = 0; i < list.Masters.Count; i++)
            {
                if (list.Masters[i].Master == this.ModKey)
                {
                    return new FormID(
                        new ModID(i),
                        this.ID);
                }
            }

            return new FormID(
                new ModID((byte)list.Masters.Count),
                this.ID);
        }

        public override bool Equals(object other)
        {
            if (!(other is FormKey key)) return false;
            return Equals(key);
        }

        public bool Equals(FormKey other)
        {
            if (!this.ModKey.Equals(other.ModKey)) return false;
            if (this.ID != other.ID) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return this.ModKey.GetHashCode()
                .CombineHashCode(this.ID.GetHashCode());
        }

        public static bool operator ==(FormKey a, FormKey b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FormKey a, FormKey b)
        {
            return !a.Equals(b);
        }
    }
}
