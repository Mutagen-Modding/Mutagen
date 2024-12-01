using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueCripplePartConditionData
{

    object? IConditionParameters.Parameter1
    {
        get => ValueFunction.CripplePart;
        set
        {

        }
    }
    Type? IConditionParametersGetter.Parameter1Type => typeof(ValueFunction);

    object? IConditionParameters.Parameter2
    {
        get => null;
        set
        {

        }
    }
    Type? IConditionParametersGetter.Parameter2Type => null;
}