using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueWeaponConditionData
{

    object? IConditionParameters.Parameter1
    {
        get => ValueFunction.WeaponIs;
        set
        {

        }
    }
    Type? IConditionParametersGetter.Parameter1Type => typeof(ValueFunction);

    object? IConditionParameters.Parameter2
    {
        get => null;
        set => Value = value is IFormLink<IWeaponGetter> v ? v : throw new ArgumentException();
    }
    Type? IConditionParametersGetter.Parameter2Type => typeof(IFormLink<IWeaponGetter>);
}