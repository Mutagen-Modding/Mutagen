using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueTargetConditionData
{
    object? IConditionParameters.Parameter1
    {
        get => ValueFunction.TargetIs;
        set
        {

        }
    }
    Type? IConditionParametersGetter.Parameter1Type => typeof(ValueFunction);

    object? IConditionParameters.Parameter2
    {
        get => null;
        set => Value = value is IFormLink<INpcGetter> v ? v : throw new ArgumentException();
    }
    Type? IConditionParametersGetter.Parameter2Type => typeof(IFormLink<INpcGetter>);
}
