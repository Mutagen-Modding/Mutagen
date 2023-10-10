using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MajorRecordPostProcessCustomization : PostProcessWhereIsACustomization<IMajorRecord>
{
    public MajorRecordPostProcessCustomization(GameRelease release)
        : base((x) => Process(x, release))
    {
    }

    private static void Process(IMajorRecord rec, GameRelease release)
    {
        rec.MajorRecordFlagsRaw = 0;
        if (rec is IFormVersionSetter formVersionSetter)
        {
            var formVersion = GameConstants.Get(release).DefaultFormVersion;
            if (formVersion != null)
            {
                formVersionSetter.FormVersion = formVersion.Value;
            }
        }
    }
}