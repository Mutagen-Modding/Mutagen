using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueWeaponConditionData
{

    public object? Parameter1
    {
        get => ValueFunction.WeaponIs;
        set
        {

        }
    }
    public Type? Parameter1Type => typeof(ValueFunction);

    public object? Parameter2
    {
        get => null;
        set => Value = value is IFormLink<IWeaponGetter> v ? v : throw new ArgumentException();
    }
    public Type? Parameter2Type => typeof(IFormLink<IWeaponGetter>);
}