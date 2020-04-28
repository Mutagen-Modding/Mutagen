using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public interface IHarvestable : IMajorRecordCommon, IHarvestableGetter
    {
        new IFormLinkNullable<IHarvestTarget> Ingredient { get; }
        new IFormLinkNullable<SoundDescriptor> HarvestSound { get; }
    }

    public interface IHarvestableGetter : IMajorRecordCommonGetter
    {
        IFormLinkNullableGetter<IHarvestTargetGetter> Ingredient { get; }
        IFormLinkNullableGetter<ISoundDescriptorGetter> HarvestSound { get; }
    }
}
