using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Internals
{
    /// <summary>
    /// A registry of master listings.
    /// Generally used for reference when converting FormIDs to FormKeys
    /// </summary>
    public class MasterReferenceReader
    {
        private readonly Dictionary<ModKey, ModIndex> _masterIndices = new Dictionary<ModKey, ModIndex>();
        
        /// <summary>
        /// A static singleton that is an empty registry containing no masters
        /// </summary>
        public static MasterReferenceReader Empty { get; } = new MasterReferenceReader(ModKey.Null);

        /// <summary>
        /// List of masters in the registry
        /// </summary>
        public IReadOnlyList<IMasterReferenceGetter> Masters { get; private set; } = ListExt.Empty<IMasterReferenceGetter>();
        
        /// <summary>
        /// ModKey that should be considered to be the current mod
        /// </summary>
        public ModKey CurrentMod;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="modKey">Mod to associate as the "current" mod</param>
        public MasterReferenceReader(ModKey modKey)
        {
            this.CurrentMod = modKey;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="modKey">Mod to associate as the "current" mod</param>
        /// <param name="masters">Masters to add to the registrar</param>
        public MasterReferenceReader(ModKey modKey, IEnumerable<IMasterReferenceGetter> masters)
        {
            this.CurrentMod = modKey;
            SetTo(masters);
        }

        /// <summary>
        /// Clears and sets contained masters to given enumerable's contents
        /// </summary>
        /// <param name="masters">Masters to set to</param>
        public void SetTo(IEnumerable<IMasterReferenceGetter> masters)
        {
            this.Masters = new List<IMasterReferenceGetter>(masters);
            this._masterIndices.Clear();

            byte index = 0;
            foreach (var master in masters)
            {
                var modKey = master.Master;
                if (this._masterIndices.ContainsKey(modKey))
                {
                    throw new ArgumentException($"{CurrentMod} masters list cannot contain duplicate entries: {modKey}");
                }
                if (index >= 0xFE)
                {
                    throw new ArgumentException($"{CurrentMod} has too many masters on masters list");
                }
                if (modKey == this.CurrentMod)
                {
                    throw new ArgumentException($"Cannot add mod to itself as a master: {this.CurrentMod}");
                }
                this._masterIndices[modKey] = new ModIndex(index++);
            }

            // Add current mod
            this._masterIndices[this.CurrentMod] = new ModIndex(index);
        }

        /// <summary>
        /// Converts a FormKey to a FormID representation, with its mod index calibrated
        /// against the contents of the registrar.
        /// </summary>
        /// <param name="key">FormKey to convert</param>
        /// <returns>FormID calibrated to registrar contents</returns>
        /// <exception cref="ArgumentException">If FormKey's ModKey is not present in registrar</exception>
        public FormID GetFormID(FormKey key)
        {
            if (this._masterIndices.TryGetValue(key.ModKey, out var index))
            {
                return new FormID(
                    index,
                    key.ID);
            }
            if (key == FormKey.Null)
            {
                return FormID.Null;
            }
            throw new ArgumentException($"Could not map FormKey to a master index: {key}");
        }

        public static MasterReferenceReader FromPath(ModPath path, GameRelease release)
        {
            using var stream = new MutagenBinaryReadStream(path, new ParsingBundle(release, masterReferences: null!)
            {
                ModKey = path.ModKey
            });
            return FromStream(stream);
        }

        public static MasterReferenceReader FromStream(Stream stream, ModKey modKey, GameRelease release)
        {
            using var interf = new MutagenInterfaceReadStream(new BinaryReadStream(stream), new ParsingBundle(release, masterReferences: null!));
            return FromStream(interf);
        }

        public static MasterReferenceReader FromStream(IMutagenReadStream stream)
        {
            var mutaFrame = new MutagenFrame(stream);
            var header = stream.ReadModHeaderFrame(readSafe: true);
            return new MasterReferenceReader(
                stream.MetaData.ModKey,
                header
                    .Masters()
                    .Select(mastPin =>
                    {
                        stream.Position = mastPin.Location;
                        return MasterReference.CreateFromBinary(mutaFrame);
                    }));
        }
    }
}
