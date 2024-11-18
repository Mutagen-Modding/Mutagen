using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueProjectileTypeConditionData
{

    public override object? Parameter1
    {
        get => ValueFunction.ProjectileTypeIs;
        set
        {

        }
    }
    public override Type? Parameter1Type => typeof(ValueFunction);

    public override object? Parameter2
    {
        get => null;
        set => Value = value is Projectile.TypeEnum v ? v : throw new ArgumentException();
    }
    public override Type? Parameter2Type => typeof(Projectile.TypeEnum);
}