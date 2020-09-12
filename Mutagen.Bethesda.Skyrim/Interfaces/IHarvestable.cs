using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Common interface for records that can be harvested
    /// </summary>
    public interface IHarvestable : ISkyrimMajorRecordInternal, IHarvestableGetter
    {
        new FormLinkNullable<IHarvestTargetGetter> Ingredient { get; set; }
        new FormLinkNullable<ISoundDescriptorGetter> HarvestSound { get; set; }
    }

    /// <summary>
    /// Common interface for records that can be harvested
    /// </summary>
    public interface IHarvestableGetter : ISkyrimMajorRecordGetter
    {
        FormLinkNullable<IHarvestTargetGetter> Ingredient { get; }
        FormLinkNullable<ISoundDescriptorGetter> HarvestSound { get; }
    }
}
