namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueTargetPartConditionData
{

    public object? Parameter1
    {
        get => ValueFunction.TargetPart;
        set
        {

        }
    }

    public object? Parameter2
    {
        get => Value;
        set => Value = value is ActorValue v ? v : throw new ArgumentException();
    }
}
