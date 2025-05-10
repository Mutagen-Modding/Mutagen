using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Fallout4;

public partial class Flora
{
    IFormLinkNullableGetter<IHarvestTargetGetter> IHarvestableGetter.Ingredient => Ingredient;
    IFormLinkNullableGetter<ISoundDescriptorGetter> IHarvestableGetter.HarvestSound => HarvestSound;
}