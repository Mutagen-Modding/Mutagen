using Noggolloquy;
using BlackInk.Underdark;

namespace Noggolloquy
{
    public class ProtocolDefinition_bethesdaproc : IProtocolRegistration
    {
        public readonly ProtocolKey ProtocolKey = new ProtocolKey(1);

        public void Register()
        {
            NoggolloquyRegistration.Register(
                new ObjectKey(ProtocolKey, 1, 1),
                new NoggolloquyTypeRegister()
                {
                    Class = typeof(Header),
                    FullName = "Header",
                    GenericCount = 0,
                    ObjectKey = new ObjectKey(ProtocolKey, 1, 1)
                });
        }
    }
}
