using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.UnitTests.Placeholders;

public interface IOtherTestMajorRecordGetter : IMajorRecordGetter
{
}

public interface IOtherTestMajorRecord : ITestMajorRecordGetter, IMajorRecord
{
}