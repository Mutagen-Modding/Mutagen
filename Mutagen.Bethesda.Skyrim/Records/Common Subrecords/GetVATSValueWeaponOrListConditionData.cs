using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueWeaponOrListConditionData
{

    public override object? Parameter1
    {
        get => ValueFunction.WeaponOrList;
        set
        {

        }
    }
    public override Type? Parameter1Type => typeof(ValueFunction);

    public override object? Parameter2
    {
        get => null;
        set => Value = value is IFormLink<IWeaponOrListGetter> v ? v : throw new ArgumentException();
    }
    public override Type? Parameter2Type => typeof(IFormLink<IWeaponOrListGetter>);
}