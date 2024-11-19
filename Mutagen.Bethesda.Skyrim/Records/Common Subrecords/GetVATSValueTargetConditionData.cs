using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueTargetConditionData
{

    public override object? Parameter1
    {
        get => ValueFunction.TargetIs;
        set
        {

        }
    }
    public override Type? Parameter1Type => typeof(ValueFunction);

    public override object? Parameter2
    {
        get => null;
        set => Value = value is IFormLink<INpcGetter> v ? v : throw new ArgumentException();
    }
    public override Type? Parameter2Type => typeof(IFormLink<INpcGetter>);
}
