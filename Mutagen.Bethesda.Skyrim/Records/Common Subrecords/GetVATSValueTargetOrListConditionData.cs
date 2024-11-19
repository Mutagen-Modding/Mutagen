using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueTargetOrListConditionData
{

    public override object? Parameter1
    {
        get => ValueFunction.TargetOrList;
        set
        {

        }
    }
    public override Type? Parameter1Type => typeof(ValueFunction);

    public override object? Parameter2
    {
        get => Value;
        set => Value = value is IFormLink<INpcOrListGetter> v ? v : throw new ArgumentException();
    }
    public override Type? Parameter2Type => typeof(IFormLink<INpcOrListGetter>);
}
