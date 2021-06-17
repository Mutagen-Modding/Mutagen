using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface ILoadOrderWriter
    {
        void Write(
            FilePath path, 
            GameRelease release, 
            IEnumerable<IModListingGetter> loadOrder,
            bool removeImplicitMods = true);
    }

    public class LoadOrderWriter : ILoadOrderWriter
    {
        private readonly IFileSystem _fileSystem;

        public LoadOrderWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        /// <inheritdoc />
        public void Write(
            FilePath path, 
            GameRelease release, 
            IEnumerable<IModListingGetter> loadOrder,
            bool removeImplicitMods = true)
        {
            bool markers = PluginListings.HasEnabledMarkers(release);
            var loadOrderList = loadOrder.ToList();
            if (removeImplicitMods)
            {
                foreach (var implicitMod in Implicits.Get(release).Listings)
                {
                    if (loadOrderList.Count > 0
                        && loadOrderList[0].ModKey == implicitMod
                        && loadOrderList[0].Enabled)
                    {
                        loadOrderList.RemoveAt(0);
                    }
                }
            }
            _fileSystem.File.WriteAllLines(path,
                loadOrderList.Where(x =>
                    {
                        return (markers || x.Enabled);
                    })
                    .Select(x =>
                    {
                        if (x.Enabled && markers)
                        {
                            return $"*{x.ModKey.FileName}";
                        }
                        else
                        {
                            return x.ModKey.FileName.String;
                        }
                    }));
        }
    }
}