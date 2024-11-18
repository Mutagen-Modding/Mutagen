using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueWeaponTypeConditionData
{

    public object? Parameter1
    {
        get => ValueFunction.WeaponTypeIs;
        set
        {

        }
    }
    public Type? Parameter1Type => typeof(ValueFunction);

    public object? Parameter2
    {
        get => null;
        set => Value = value is WeaponAnimationType v ? v : throw new ArgumentException();
    }
    public Type? Parameter2Type => typeof(WeaponAnimationType);
}