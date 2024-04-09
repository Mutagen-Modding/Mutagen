namespace Mutagen.Bethesda.Plugins.Exceptions;

public class LowerFormKeyRangeDisallowedException : Exception
{
    public FormKey ExampleKey { get; }

    public LowerFormKeyRangeDisallowedException(FormKey exampleKey)
    {
        ExampleKey = exampleKey;
    }
}
