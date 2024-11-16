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

    public object? Parameter2
    {
        get => null;
        set => Value = value is WeaponAnimationType v ? v : throw new ArgumentException();
    }
}