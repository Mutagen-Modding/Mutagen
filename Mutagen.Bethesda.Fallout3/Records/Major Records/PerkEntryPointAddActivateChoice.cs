namespace Mutagen.Bethesda.Fallout3;

// Perk effects are parsed via PerkBinaryCreateTranslation.ParseEffect (returning mutable objects),
// so this overlay class is never instantiated.
partial class PerkEntryPointAddActivateChoiceBinaryOverlay
{
    public string? ButtonLabel => throw new NotImplementedException();

    public bool? RunImmediately => throw new NotImplementedException();

    public IScriptFieldsGetter Script => throw new NotImplementedException();
}
