using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Mutagen.Bethesda.Plugins.Masters
{
    /// <summary>
    /// A registry of master listings.
    /// Generally used for reference when converting FormIDs to FormKeys
    /// </summary>
    public interface IMasterReferenceReader
    {
        /// <summary>
        /// List of masters in the registry
        /// </summary>
        IReadOnlyList<IMasterReferenceGetter> Masters { get; }
        
        /// <summary>
        /// ModKey that should be considered to be the current mod
        /// </summary>
        public ModKey CurrentMod { get; }

        /// <summary>
        /// Converts a FormKey to a FormID representation, with its mod index calibrated
        /// against the contents of the registrar.
        /// </summary>
        /// <param name="key">FormKey to convert</param>
        /// <returns>FormID calibrated to registrar contents</returns>
        /// <exception cref="ArgumentException">If FormKey's ModKey is not present in registrar</exception>
        FormID GetFormID(FormKey key);
    }

    public interface IMasterReferenceCollection : IMasterReferenceReader
    {
        /// <summary>
        /// Clears and sets contained masters to given enumerable's contents
        /// </summary>
        /// <param name="masters">Masters to set to</param>
        void SetTo(IEnumerable<IMasterReferenceGetter> masters);
    }

    /// <summary>
    /// A registry of master listings.
    /// Generally used for reference when converting FormIDs to FormKeys
    /// </summary>
    public class MasterReferenceReader : IMasterReferenceCollection
    {
        private readonly Dictionary<ModKey, ModIndex> _masterIndices = new Dictionary<ModKey, ModIndex>();
        
        /// <summary>
        /// A static singleton that is an empty registry containing no masters
        /// </summary>
        public static IMasterReferenceReader Empty { get; } = new MasterReferenceReader(ModKey.Null);

        /// <inheritdoc />
        public IReadOnlyList<IMasterReferenceGetter> Masters { get; private set; } = ListExt.Empty<IMasterReferenceGetter>();
        
        /// <inheritdoc />
        public ModKey CurrentMod { get; }

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

        /// <inheritdoc />
        public void SetTo(IEnumerable<IMasterReferenceGetter> masters)
        {
            this.Masters = new List<IMasterReferenceGetter>(masters);
            this._masterIndices.Clear();

            byte index = 0;
            foreach (var master in masters)
            {
                var modKey = master.Master;
                if (index >= 0xFE)
                {
                    throw new ArgumentException($"{CurrentMod} has too many masters on masters list");
                }
                if (modKey == this.CurrentMod)
                {
                    throw new ArgumentException($"Cannot add mod to itself as a master: {this.CurrentMod}");
                }
                // Don't care about duplicates too much, just skip
                if (!this._masterIndices.ContainsKey(modKey))
                {
                    this._masterIndices[modKey] = new ModIndex(index);
                }
                index++;
            }

            // Add current mod
            this._masterIndices[this.CurrentMod] = new ModIndex(index);
        }

        /// <inheritdoc />
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

        public static MasterReferenceReader FromPath(ModPath path, GameRelease release, IFileSystem? fileSystem = null)
        {
            var fs = fileSystem.GetOrDefault().FileStream.Create(path, FileMode.Open, FileAccess.Read);
            using var stream = new MutagenBinaryReadStream(fs, new ParsingBundle(release, masterReferences: null!)
            {
                ModKey = path.ModKey
            });
            return FromStream(stream);
        }

        public static MasterReferenceReader FromStream(Stream stream, ModKey modKey, GameRelease release, bool disposeStream = true)
        {
            using var interf = new MutagenInterfaceReadStream(
                new BinaryReadStream(stream, dispose: disposeStream), 
                new ParsingBundle(release, masterReferences: null!)
                {
                    ModKey = modKey
                });
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
