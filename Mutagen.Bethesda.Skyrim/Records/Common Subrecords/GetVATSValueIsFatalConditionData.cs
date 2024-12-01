using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueIsFatalConditionData
{

    object? IConditionParameters.Parameter1
    {
        get => ValueFunction.IsFatal;
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