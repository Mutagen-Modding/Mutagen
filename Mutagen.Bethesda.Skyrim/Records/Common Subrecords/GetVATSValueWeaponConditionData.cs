using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueWeaponConditionData
{

    public override object? Parameter1
    {
        get => ValueFunction.WeaponIs;
        set
        {

        }
    }
    public override Type? Parameter1Type => typeof(ValueFunction);

    public override object? Parameter2
    {
        get => null;
        set => Value = value is IFormLink<IWeaponGetter> v ? v : throw new ArgumentException();
    }
    public override Type? Parameter2Type => typeof(IFormLink<IWeaponGetter>);
}