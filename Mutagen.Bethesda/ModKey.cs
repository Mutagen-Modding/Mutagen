using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// ModKey defines a key struct of the name and file type that a mod intends to be stored on-disk with.
    /// 
    /// The proper factory format is: [ModName].es[p/m], depending on whether it is a master file or not.
    /// 
    /// A correct ModKey is very important if a mod's contents will ever be added to another mod (as an override).
    /// Otherwise, records will become mis-linked.
    /// 
    /// General practice is:
    ///  - Use ModKey.TryFactory on a mod's file name when at all possible
    ///  - Use the Dummy singleton only when it is unknown, and no record cross pollination is planned to occur.
    public struct ModKey : IEquatable<ModKey>
    {
        public static readonly ModKey NULL = new ModKey(string.Empty, master: true);
        public StringCaseAgnostic Name { get; private set; }
        public bool Master { get; private set; }
        public string FileName => this.ToString();

        /// </summary>
        /// <summary>
        /// A convenience singleton that represents an unimportant ModKey.
        /// 
        /// Refer to ModKey overall docs for when/how it should be used.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static readonly ModKey Dummy = new ModKey("MutagenDummyKey", master: false);
        
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

        /// <summary>
        /// A convenience function that attempts to turn a given string into a ModKey object.
        /// If it fails, it falls back and uses the Dummy singleton.
        /// 
        /// Refer to ModKey overall docs for when/how it should be used.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ModKey TryFactoryDummyFallback(string str)
        {
            return ModKey.TryFactory(str, out var modKey) ? modKey : Dummy;
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
