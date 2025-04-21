using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

public interface IEnchantable : IStarfieldMajorRecordInternal, IEnchantableGetter
{
    new IFormLinkNullable<IObjectEffectGetter> ObjectEffect { get; }
    new ushort? EnchantmentAmount { get; set; }
}

public interface IEnchantableGetter : IStarfieldMajorRecordGetter
{
    IFormLinkNullableGetter<IObjectEffectGetter> ObjectEffect { get; }
    ushort? EnchantmentAmount { get; }
}
