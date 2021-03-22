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
        new IFormLinkNullable<IHarvestTargetGetter> Ingredient { get; }
        new IFormLinkNullable<ISoundDescriptorGetter> HarvestSound { get; }
    }

    /// <summary>
    /// Common interface for records that can be harvested
    /// </summary>
    public interface IHarvestableGetter : ISkyrimMajorRecordGetter
    {
        IFormLinkNullableGetter<IHarvestTargetGetter> Ingredient { get; }
        IFormLinkNullableGetter<ISoundDescriptorGetter> HarvestSound { get; }
    }
}
