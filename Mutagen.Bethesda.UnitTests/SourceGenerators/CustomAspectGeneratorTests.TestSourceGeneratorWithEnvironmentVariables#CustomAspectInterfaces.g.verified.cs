//HintName: CustomAspectInterfaces.g.cs
using <global namespace>;

namespace SomeNamespace
{
    #region Wrappers
    public class IArmorWrapper : ISomeCustomAspect
    {
        private readonly IArmor _wrapped;
        public IFormLinkNullable<ISoundDescriptorGetter> PickUpSound
        {
            get => _wrapped.PickUpSound;
        }

        public IArmorWrapper(IArmor rhs)
        {
            _wrapped = rhs;
        }
    }


    #endregion

    #region Mix Ins
    public static class WrapperMixIns
    {
        public static IArmorWrapper AsISomeCustomAspect(this IArmor rhs)
        {
            return new IArmorWrapper(rhs);
        }

    }
    #endregion

}
