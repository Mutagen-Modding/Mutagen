using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Skyrim;

public partial class GetVATSValueIsParalyzingPalmConditionData
{

    public override object? Parameter1
    {
        get => ValueFunction.IsParalyzingPalm;
        set
        {

        }
    }
    public override Type? Parameter1Type => typeof(ValueFunction);

    public override object? Parameter2
    {
        get => null;
        set
        {

        }
    }
    public override Type? Parameter2Type => null;
}