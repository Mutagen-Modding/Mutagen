using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueTargetConditionData
{

    public object? Parameter1
    {
        get => ValueFunction.TargetIs;
        set
        {

        }
    }

    public object? Parameter2
    {
        get => null;
        set => Value = value is IFormLink<INpcGetter> v ? v : throw new ArgumentException();
    }
}
