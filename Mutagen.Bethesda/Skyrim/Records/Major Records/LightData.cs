using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class LightData
    {
        [Flags]
        public enum Flag
        {
            Dynamic = 0x0001,
            CanBeCarried = 0x0002,
            Negative = 0x0004,
            Flicker = 0x0008,
            OffByDefault = 0x0020,
            FlickerSlow = 0x0040,
            Pulse = 0x0080,
            PulseSlow = 0x0100,
            SpotLight = 0x0200,
            ShadowSpotlight = 0x0400,
            ShadowHemisphere = 0x0800,
            ShadowOmnidirectional = 0x1000,
            PortalStrict = 0x2000,
        }
    }
}
