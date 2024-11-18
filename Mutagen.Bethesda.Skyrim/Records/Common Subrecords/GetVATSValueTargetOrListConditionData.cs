using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueTargetOrListConditionData
{

    public object? Parameter1
    {
        get => ValueFunction.TargetOrList;
        set
        {

        }
    }
    public Type? Parameter1Type => typeof(ValueFunction);

    public object? Parameter2
    {
        get => Value;
        set => Value = value is IFormLink<INpcOrListGetter> v ? v : throw new ArgumentException();
    }
    public Type? Parameter2Type => typeof(IFormLink<INpcOrListGetter>);
}
