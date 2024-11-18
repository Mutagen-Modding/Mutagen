using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueProjectileTypeConditionData
{

    public object? Parameter1
    {
        get => ValueFunction.ProjectileTypeIs;
        set
        {

        }
    }
    public Type? Parameter1Type => typeof(ValueFunction);

    public object? Parameter2
    {
        get => null;
        set => Value = value is Projectile.TypeEnum v ? v : throw new ArgumentException();
    }
    public Type? Parameter2Type => typeof(Projectile.TypeEnum);
}