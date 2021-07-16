using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Core.UnitTests.Placeholders
{
    public interface IOtherTestMajorRecordGetter : IMajorRecordCommonGetter
    {
    }

    public interface IOtherTestMajorRecord : ITestMajorRecordGetter, IMajorRecordCommon
    {
    }
}