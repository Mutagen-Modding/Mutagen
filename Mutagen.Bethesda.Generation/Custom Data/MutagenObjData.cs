using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class MutagenObjData
    {
        public ObjectGeneration ObjGen { get; private set; }
        public RecordType? RecordType;
        public bool FailOnUnknown;
        public ObjectType? ObjectType;
        public RecordType? MarkerType;
        public HashSet<RecordType> TriggeringRecordTypes = new HashSet<RecordType>();
        public HashSet<RecordType> CustomRecordTypeTriggers = new HashSet<RecordType>();
        public string TriggeringSource;
        public bool CustomBinaryEnd;
        public Task<IEnumerable<KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>>> GenerationTypes => GetGenerationTypes();
        public TaskCompletionSource WiringComplete = new TaskCompletionSource();
        public RecordTypeConverter BaseRecordTypeConverter;

        public MutagenObjData(ObjectGeneration objGen)
        {
            this.ObjGen = objGen;
        }

        private async Task<IEnumerable<KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>>> GetGenerationTypes()
        {
            List<KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>> ret = new List<KeyValuePair<IEnumerable<Bethesda.RecordType>, ObjectGeneration>>()
            {
                new KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>(
                    this.TriggeringRecordTypes,
                    this.ObjGen)
            };
            foreach (var subObjs in await this.ObjGen.InheritingObjects())
            {
                ret.Add(new KeyValuePair<IEnumerable<RecordType>, ObjectGeneration>(
                    await subObjs.GetTriggeringRecordTypes(),
                    subObjs));
            }
            return ret;
        }
    }
}
