namespace Mutagen.Bethesda.Starfield;

partial class Static
{
    [Flags]
    public enum MajorFlag
    {
        HeadingMarker = 0x0000_0004,
        NonOccluder = 0x0000_0010,
        HasTreeLod = 0x0000_0040,
        AddOnLodObject = 0x0000_0080,
        HiddenFromLocalMap = 0x0000_0200,
        HeadtrackMarker = 0x0000_0400,
        UsedAsPlatform = 0x0000_0800,
        PackInUseOnly = 0x0000_2000,
        HasDistantLod = 0x0000_8000,
        UsesHdLodTexture = 0x0002_0000,
        HasCurrents = 0x0008_0000,
        IsMarker = 0x0080_0000,
        Obstacle = 0x0200_0000,
        NavMeshGenerationFilter = 0x0400_0000,
        NavMeshGenerationBoundingBox = 0x0800_0000,
        /// <summary>
        /// Sky cell only
        /// </summary>
        ShowInWorldMap = 0x1000_0000,
        NavMeshGenerationGround = 0x4000_0000,
    }
}