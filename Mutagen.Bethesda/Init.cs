using Loqui;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class Init
    {
        public static void SpinUp()
        {
            LoquiRegistrationSettings.AutomaticRegistration = false;
            ProtocolDefinition_Oblivion.Register();
            ProtocolDefinition_Skyrim.Register();
        }
    }
}
