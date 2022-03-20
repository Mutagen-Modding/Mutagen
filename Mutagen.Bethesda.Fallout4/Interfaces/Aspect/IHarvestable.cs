using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4
{
    /// <summary>
    /// Common interface for records that can be harvested
    /// </summary>
    public interface IHarvestable : IFallout4MajorRecordInternal, IHarvestableGetter
    {
        new IFormLinkNullable<IHarvestTargetGetter> Ingredient { get; }
        new IFormLinkNullable<ISoundDescriptorGetter> HarvestSound { get; }
    }

    /// <summary>
    /// Common interface for records that can be harvested
    /// </summary>
    public interface IHarvestableGetter : IFallout4MajorRecordGetter
    {
        IFormLinkNullableGetter<IHarvestTargetGetter> Ingredient { get; }
        IFormLinkNullableGetter<ISoundDescriptorGetter> HarvestSound { get; }
    }
}
