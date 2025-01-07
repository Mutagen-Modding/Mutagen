using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueProjectileTypeConditionData
{

    object? IConditionParameters.Parameter1
    {
        get => ValueFunction.ProjectileTypeIs;
        set
        {

        }
    }
    Type? IConditionParametersGetter.Parameter1Type => typeof(ValueFunction);

    object? IConditionParameters.Parameter2
    {
        get => null;
        set => Value = value is Projectile.TypeEnum v ? v : throw new ArgumentException();
    }
    Type? IConditionParametersGetter.Parameter2Type => typeof(Projectile.TypeEnum);
}