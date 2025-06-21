using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

public partial class Flora
{
    IFormLinkNullableGetter<IHarvestTargetGetter> IHarvestableGetter.Ingredient => Ingredient;
    IFormLinkNullableGetter<ISoundDescriptorGetter> IHarvestableGetter.HarvestSound => HarvestSound;
}