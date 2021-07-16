using System;
using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Core.UnitTests.Placeholders
{
    public interface ITestMajorRecordGetter : IMajorRecordCommonGetter
    {
    }

    public interface ITestMajorRecord : ITestMajorRecordGetter, IMajorRecordCommon
    {
    }
    
    public class TestMajorRecord : ITestMajorRecord
    {
        public bool IsCompressed { get; set; }
        public bool IsDeleted { get; set; }
        public int MajorRecordFlagsRaw { get; set; }
        public bool Disable()
        {
            throw new System.NotImplementedException();
        }

        public ushort? FormVersion { get; }
        public IEnumerable<IFormLinkGetter> ContainedFormLinks => throw new NotImplementedException();
        public void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping)
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IFormLinkGetter? other)
        {
            throw new System.NotImplementedException();
        }

        public ILoquiRegistration Registration => throw new NotImplementedException();
        public FormKey FormKey { get; }
        public string? EditorID { get; }

        public TestMajorRecord(FormKey formKey)
        {
            FormKey = formKey;
        }

        public IFormLink<IOtherTestMajorRecordGetter> FormLink { get; } = new FormLink<IOtherTestMajorRecordGetter>();
    }
}