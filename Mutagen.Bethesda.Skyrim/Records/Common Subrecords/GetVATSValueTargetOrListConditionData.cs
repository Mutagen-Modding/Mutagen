using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueTargetOrListConditionData
{

    object? IConditionParameters.Parameter1
    {
        get => ValueFunction.TargetOrList;
        set
        {

        }
    }
    Type? IConditionParametersGetter.Parameter1Type => typeof(ValueFunction);

    object? IConditionParameters.Parameter2
    {
        get => Value;
        set => Value = value is IFormLink<INpcOrListGetter> v ? v : throw new ArgumentException();
    }
    Type? IConditionParametersGetter.Parameter2Type => typeof(IFormLink<INpcOrListGetter>);
}
