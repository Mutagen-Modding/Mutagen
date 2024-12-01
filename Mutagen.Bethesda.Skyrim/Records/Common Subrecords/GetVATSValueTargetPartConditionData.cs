namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueTargetPartConditionData
{

    object? IConditionParameters.Parameter1
    {
        get => ValueFunction.TargetPart;
        set
        {

        }
    }
    Type? IConditionParametersGetter.Parameter1Type => typeof(ValueFunction);

    object? IConditionParameters.Parameter2
    {
        get => Value;
        set => Value = value is ActorValue v ? v : throw new ArgumentException();
    }
    Type? IConditionParametersGetter.Parameter2Type => typeof(ActorValue);
}
