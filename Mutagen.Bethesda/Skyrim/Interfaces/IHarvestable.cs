using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Common interface for records that can be harvested
    /// </summary>
    public interface IHarvestable : IMajorRecordCommon, IHarvestableGetter
    {
        new IFormLinkNullable<IHarvestTarget> Ingredient { get; }
        new IFormLinkNullable<SoundDescriptor> HarvestSound { get; }
    }

    /// <summary>
    /// Common interface for records that can be harvested
    /// </summary>
    public interface IHarvestableGetter : IMajorRecordCommonGetter
    {
        IFormLinkNullableGetter<IHarvestTargetGetter> Ingredient { get; }
        IFormLinkNullableGetter<ISoundDescriptorGetter> HarvestSound { get; }
    }
}
