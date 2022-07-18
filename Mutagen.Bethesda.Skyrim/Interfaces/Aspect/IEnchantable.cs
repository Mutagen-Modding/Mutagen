using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

public interface IEnchantable : ISkyrimMajorRecordInternal, IEnchantableGetter
{
    new IFormLinkNullable<IEffectRecordGetter> ObjectEffect { get; }
    new ushort? EnchantmentAmount { get; set; }
}

public interface IEnchantableGetter : ISkyrimMajorRecordGetter
{
    IFormLinkNullableGetter<IEffectRecordGetter> ObjectEffect { get; }
    ushort? EnchantmentAmount { get; }
}