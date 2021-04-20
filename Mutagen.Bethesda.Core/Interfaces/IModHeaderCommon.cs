using Mutagen.Bethesda.Records.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Core
{
    public interface IModHeaderCommon : IBinaryItem
    {
        IExtendedList<MasterReference> MasterReferences { get; }
        int RawFlags { get; set; }
        uint NumRecords { get; set; }
        uint NextFormID { get; set; }
        uint MinimumCustomFormID { get; }
    }
}
