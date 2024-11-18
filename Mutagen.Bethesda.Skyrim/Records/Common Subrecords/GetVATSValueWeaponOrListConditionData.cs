using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueWeaponOrListConditionData
{

    public object? Parameter1
    {
        get => ValueFunction.WeaponOrList;
        set
        {

        }
    }
    public Type? Parameter1Type => typeof(ValueFunction);

    public object? Parameter2
    {
        get => null;
        set => Value = value is IFormLink<IWeaponOrListGetter> v ? v : throw new ArgumentException();
    }
    public Type? Parameter2Type => typeof(IFormLink<IWeaponOrListGetter>);
}