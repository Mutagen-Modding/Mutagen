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
        new FormLinkNullable<IHarvestTarget> Ingredient { get; }
        new FormLinkNullable<SoundDescriptor> HarvestSound { get; }
    }

    /// <summary>
    /// Common interface for records that can be harvested
    /// </summary>
    public interface IHarvestableGetter : ISkyrimMajorRecordGetter
    {
        IFormLinkNullable<IHarvestTargetGetter> Ingredient { get; }
        IFormLinkNullable<ISoundDescriptorGetter> HarvestSound { get; }
    }
}
