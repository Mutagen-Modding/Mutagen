using Mutagen.Bethesda.Plugins;
using System;

namespace Mutagen.Bethesda.Fallout76;

partial class Race
{
    [Flags]
    public enum MajorFlag
    {
    }
}

partial class RaceBinaryOverlay
{
    public Boolean MNAM2 => false;
    public Boolean FNAM2 => false;
    public Boolean MNAM3 => false;
    public Boolean FNAM3 => false;
    public Boolean MNAM4 => false;
    public Boolean NAM02 => false;
    public Boolean FNAM4 => false;
    public IReadOnlyList<String> MovementTypeNames => Array.Empty<String>();
    public IReadOnlyList<String> BipedObjectNames => Array.Empty<String>();
    public IReadOnlyList<String> PhonemeTargetNames => Array.Empty<String>();
    public String ANAM2 => string.Empty;
    public String WMAP2 => string.Empty;
    public IReadOnlyList<IFormLinkGetter<IHeadPartGetter>> MaleHeadParts => Array.Empty<IFormLinkGetter<IHeadPartGetter>>();
    public IReadOnlyList<IFormLinkGetter<IFallout76MajorRecordGetter>> MaleRacePresets => Array.Empty<IFormLinkGetter<IFallout76MajorRecordGetter>>();
    public IReadOnlyList<IFormLinkGetter<IColorRecordGetter>> MaleHairColors => Array.Empty<IFormLinkGetter<IColorRecordGetter>>();
    public IReadOnlyList<IFormLinkGetter<ITextureSetGetter>> MaleFaceDetails => Array.Empty<IFormLinkGetter<ITextureSetGetter>>();
    public IReadOnlyList<IFormLinkGetter<IHeadPartGetter>> FemaleHeadParts => Array.Empty<IFormLinkGetter<IHeadPartGetter>>();
    public IReadOnlyList<IFormLinkGetter<IFallout76MajorRecordGetter>> FemaleRacePresets => Array.Empty<IFormLinkGetter<IFallout76MajorRecordGetter>>();
    public IReadOnlyList<IFormLinkGetter<IColorRecordGetter>> FemaleHairColors => Array.Empty<IFormLinkGetter<IColorRecordGetter>>();
    public IReadOnlyList<IFormLinkGetter<ITextureSetGetter>> FemaleFaceDetails => Array.Empty<IFormLinkGetter<ITextureSetGetter>>();
}
