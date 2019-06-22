using Loqui;
using Mutagen.Bethesda.Tests;

namespace Loqui
{
    public class ProtocolDefinition_Tests : IProtocolRegistration
    {
        public readonly static ProtocolKey ProtocolKey = new ProtocolKey("Tests");
        void IProtocolRegistration.Register() => Register();
        public static void Register()
        {
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.TestingSettings_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.Target_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.PassthroughSettings_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.DataFolderLocations_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.TargetGroup_Registration.Instance);
            LoquiRegistration.Register(Mutagen.Bethesda.Tests.Internals.RecordInterest_Registration.Instance);
        }
    }
}
