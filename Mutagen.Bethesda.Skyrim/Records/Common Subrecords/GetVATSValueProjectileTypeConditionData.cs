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

    public object? Parameter2
    {
        get => null;
        set => Value = value is Projectile.TypeEnum v ? v : throw new ArgumentException();
    }
}