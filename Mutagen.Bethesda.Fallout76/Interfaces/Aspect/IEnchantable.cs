using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout76;

public interface IEnchantable : IFallout76MajorRecordInternal, IEnchantableGetter
{
    new IFormLinkNullable<IObjectEffectGetter> ObjectEffect { get; }
    new ushort? EnchantmentAmount { get; set; }
}

public interface IEnchantableGetter : IFallout76MajorRecordGetter
{
    IFormLinkNullableGetter<IObjectEffectGetter> ObjectEffect { get; }
    ushort? EnchantmentAmount { get; }
}
