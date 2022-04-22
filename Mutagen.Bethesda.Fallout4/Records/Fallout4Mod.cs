using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout4;

public partial class Fallout4Mod : AMod
{
    public const uint DefaultInitialNextFormID = 0x800;
    private uint GetDefaultInitialNextFormID() => DefaultInitialNextFormID;

    partial void CustomCtor()
    {
        this.ModHeader.FormVersion = GameRelease.Fallout4.GetDefaultFormVersion()!.Value;
    }
}