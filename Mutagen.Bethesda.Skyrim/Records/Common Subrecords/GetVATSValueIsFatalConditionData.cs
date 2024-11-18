using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueIsFatalConditionData
{

    public object? Parameter1
    {
        get => ValueFunction.IsFatal;
        set
        {

        }
    }
    public Type? Parameter1Type => typeof(ValueFunction);

    public object? Parameter2
    {
        get => null;
        set
        {

        }
    }
    public Type? Parameter2Type => null;
}