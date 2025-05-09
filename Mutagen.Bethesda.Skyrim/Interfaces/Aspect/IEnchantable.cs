using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

public interface IEnchantable : ISkyrimMajorRecordInternal, IEnchantableGetter
{
    new IFormLinkNullable<IObjectEffectGetter> ObjectEffect { get; }
    new ushort? EnchantmentAmount { get; set; }
}

public interface IEnchantableGetter : ISkyrimMajorRecordGetter
{
    IFormLinkNullableGetter<IObjectEffectGetter> ObjectEffect { get; }
    ushort? EnchantmentAmount { get; }
}
