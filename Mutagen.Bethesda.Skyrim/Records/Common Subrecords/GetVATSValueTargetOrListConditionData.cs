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

    public object? Parameter2
    {
        get => Value;
        set => Value = value is IFormLink<INpcOrListGetter> v ? v : throw new ArgumentException();
    }
}
