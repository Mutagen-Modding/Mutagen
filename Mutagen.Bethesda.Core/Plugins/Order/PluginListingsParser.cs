using System;
using System.Collections.Generic;
using System.IO;

namespace Mutagen.Bethesda.Plugins.Order
{
    /// <summary>
    /// Converts a stream into raw enumerable of ModListings
    /// </summary>
    public interface IPluginListingsParser
    {
        /// <summary>
        /// Parses a stream to retrieve all ModKeys in expected plugin file format
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns>List of ModKeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin stream is unexpected</exception>
        IEnumerable<IModListingGetter> Parse(Stream stream);
    }

    internal class PluginListingsParser : IPluginListingsParser
    {
        private readonly GameRelease _release;
        private readonly IModListingParser _listingParser;

        public PluginListingsParser(
            GameRelease release,
            IModListingParser listingParser)
        {
            _release = release;
            _listingParser = listingParser;
        }
        
        /// <inheritdoc />
        public IEnumerable<IModListingGetter> Parse(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            while (!streamReader.EndOfStream)
            {
                var str = streamReader.ReadLine().AsSpan();
                var commentIndex = str.IndexOf('#');
                if (commentIndex != -1)
                {
                    str = str.Slice(0, commentIndex);
                }
                if (MemoryExtensions.IsWhiteSpace(str) || str.Length == 0) continue;
                yield return _listingParser.FromString(str);
            }
        }
    }
}