using System;

namespace Mutagen.Bethesda.Fallout76;

partial class ImageSpace
{
    [Flags]
    public enum MajorFlag
    {
    }

    public enum SkyBlurRadius : ushort
    {
        None = 0,
        Radius0 = 16384,
        Radius1 = 16672,
        Radius2 = 16784,
        Radius3 = 16848,
        Radius4 = 16904,
        Radius5 = 16936,
        Radius6 = 16968,
        Radius7 = 17000,
        NoSkyRadius0 = 16576,
        NoSkyRadius1 = 16736,
        NoSkyRadius2 = 16816,
        NoSkyRadius3 = 16880,
        NoSkyRadius4 = 16920,
        NoSkyRadius5 = 16952,
        NoSkyRadius6 = 16984,
        NoSkyRadius7 = 17016,
    }
}
