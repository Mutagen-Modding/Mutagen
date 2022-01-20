using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Core.UnitTests.Placeholders
{
    public interface IOtherTestMajorRecordGetter : IMajorRecordGetter
    {
    }

    public interface IOtherTestMajorRecord : ITestMajorRecordGetter, IMajorRecord
    {
    }
}