using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4;

public interface IEnchantable : IFallout4MajorRecordInternal, IEnchantableGetter
{
    new IFormLinkNullable<IObjectEffectGetter> ObjectEffect { get; }
    new ushort? EnchantmentAmount { get; set; }
}

public interface IEnchantableGetter : IFallout4MajorRecordGetter
{
    IFormLinkNullableGetter<IObjectEffectGetter> ObjectEffect { get; }
    ushort? EnchantmentAmount { get; }
}
