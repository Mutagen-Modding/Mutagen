namespace Mutagen.Bethesda.FalloutNV;

public partial class MenuIcon
{
    Icons? IHasIcons.Icons
    {
        get => this.Icons;
        set => this.Icons = value ?? throw new NullReferenceException();
    }
}