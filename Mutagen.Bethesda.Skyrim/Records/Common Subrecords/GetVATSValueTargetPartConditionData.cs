namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueTargetPartConditionData
{

    public override object? Parameter1
    {
        get => ValueFunction.TargetPart;
        set
        {

        }
    }
    public override Type? Parameter1Type => typeof(ValueFunction);

    public override object? Parameter2
    {
        get => Value;
        set => Value = value is ActorValue v ? v : throw new ArgumentException();
    }
    public override Type? Parameter2Type => typeof(ActorValue);
}
