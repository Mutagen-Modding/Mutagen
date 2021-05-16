using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.WPF.Plugins.Order
{
    public class LoadOrderEntryVM : ModListingVM
    {
        public LoadOrderEntryVM(
            ILoadOrderVM loadOrderVM,
            IModListingGetter listing,
            string dataFolder) 
            : base(listing, dataFolder)
        {
        }
    }
}
