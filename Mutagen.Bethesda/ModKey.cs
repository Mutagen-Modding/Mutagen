using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public struct ModKey : IEquatable<ModKey>
    {
        public static readonly ModKey NULL = new ModKey(string.Empty, master: true);
        public StringCaseAgnostic Name { get; private set; }
        public bool Master { get; private set; }
        public string FileName => this.ToString();
        
        public ModKey(
            string name,
            bool master)
        {
            this.Name = name;
            this.Master = master;
        }

        public bool Equals(ModKey other)
        {
            return string.Equals(this.Name, other.Name)
                && this.Master == other.Master;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ModKey key)) return false;
            return Equals(key);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode()
                .CombineHashCode(Master.GetHashCode());
        }

        public override string ToString()
        {
            return $"{Name}.{(this.Master ? "esm" : "esp")}";
        }

        public static bool TryFactory(string str, out ModKey modKey)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                modKey = default(ModKey);
                return false;
            }
            var index = str.LastIndexOf('.');
            if (index == -1
                || index != str.Length - 4)
            {
                modKey = default(ModKey);
                return false;
            }
            var modString = str.Substring(0, index);
            var endString = str.Substring(index + 1);
            bool master;
            switch (endString.ToLower())
            {
                case "esm":
                    master = true;
                    break;
                case "esp":
                    master = false;
                    break;
                default:
                    modKey = default(ModKey);
                    return false;
            }
            modKey = new ModKey(
                name: modString,
                master: master);
            return true;
        }

        public static ModKey Factory(string str)
        {
            if (TryFactory(str, out var key))
            {
                return key;
            }
            if (TryFactory(Path.GetFileName(str), out key))
            {
                return key;
            }
            throw new ArgumentException("Could not construct ModKey.");
        }

        public static bool operator ==(ModKey a, ModKey b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ModKey a, ModKey b)
        {
            return !a.Equals(b);
        }
    }
}
