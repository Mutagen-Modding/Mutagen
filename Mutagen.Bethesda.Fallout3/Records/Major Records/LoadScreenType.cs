namespace Mutagen.Bethesda.Fallout3;

public partial class LoadScreenType
{
    public enum LoadScreenTypeEnum : uint
    {
        None = 0,
        XPProgress = 1,
        Objective = 2,
        Tip = 3,
        Stats = 4,
    }

    /// <summary>
    /// Font slot index used by <see cref="LoadScreenType"/>'s font and stats fields.
    /// Mirrors xEdit's degenerate enum where index 0 is blank and 1-7 display as "2".."8".
    /// </summary>
    public enum LoadScreenItemEnum : uint
    {
        None = 0,
        Item2 = 1,
        Item3 = 2,
        Item4 = 3,
        Item5 = 4,
        Item6 = 5,
        Item7 = 6,
        Item8 = 7,
    }

    public enum LoadScreenAlignment : uint
    {
        None = 0,
        Left = 1,
        Center = 2,
        Right = 4,
    }
}
