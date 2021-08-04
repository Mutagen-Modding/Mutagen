using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface IModListingToStringConverter
    {
        string Convert(IModListingGetter getter);

        string Convert<TMod>(IModListingGetter<TMod> getter)
            where TMod : class, IModGetter;
    }

    public class ModListingToStringConverter : IModListingToStringConverter
    {
        public string Convert(IModListingGetter getter)
        {
            return IModListingGetter.ToString(getter);
        }
        
        public string Convert<TMod>(IModListingGetter<TMod> getter)
            where TMod : class, IModGetter
        {
            return IModListingGetter<TMod>.ToString(getter);
        }
    }
}