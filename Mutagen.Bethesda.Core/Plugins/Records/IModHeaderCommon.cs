using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records;

public interface IModHeaderCommon : IBinaryItem
{
    IExtendedList<MasterReference> MasterReferences { get; }
    int RawFlags { get; set; }
    uint NumRecords { get; set; }
    uint NextFormID { get; set; }
    string? Author { get; set; }
    string? Description { get; set; }

    /// <summary>
    /// The version recorded in the mod header's stats
    /// </summary>
    float HeaderVersion { get; set; }

    /// <summary>
    /// Sets the listed overridden forms contained in the Mod Header
    /// </summary>
    /// <param name="formKeys">FormKeys to list</param>
    void SetOverriddenForms(IEnumerable<FormKey>? formKeys);
}