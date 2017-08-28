using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;

namespace Mutagen.Generation
{
    public class RecordTypeEmbedderModule : GenerationModule
    {
        public override string RegionString => "Record Types";

        public override void Generate(ObjectGeneration obj, FileGeneration fg)
        {
        }

        public override void Generate(ObjectGeneration obj)
        {
        }

        public override void GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            HashSet<RecordType> recordTypes = new HashSet<RecordType>();
            foreach (var field in obj.Fields)
            {
                if (!(field is IRecordTypeableField typable)
                    || !typable.RecordType.HasValue) continue;
                recordTypes.Add(typable.RecordType.Value);
            }
            foreach (var type in recordTypes)
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {type.HeaderName} = new {nameof(RecordType)}(\"{type.Type}\");");
            }
        }

        public override void GenerateInCommonExt(ObjectGeneration obj, FileGeneration fg)
        {
        }

        public override void GenerateInInterfaceGetter(ObjectGeneration obj, FileGeneration fg)
        {
        }

        public override IEnumerable<string> GetReaderInterfaces(ObjectGeneration obj)
        {
            yield break;
        }

        public override IEnumerable<string> GetWriterInterfaces(ObjectGeneration obj)
        {
            yield break;
        }

        public override IEnumerable<string> Interfaces(ObjectGeneration obj)
        {
            yield break;
        }

        public override void PreLoad(ObjectGeneration obj)
        {
            var record = obj.Node.GetAttribute("recordType");
            if (record != null)
            {
                obj.CustomData[nameof(RecordType)] = new RecordType(record);
            }
        }

        public override void Modify(LoquiGenerator gen)
        {
        }

        public override IEnumerable<string> RequiredUsingStatements()
        {
            yield break;
        }
    }
}
