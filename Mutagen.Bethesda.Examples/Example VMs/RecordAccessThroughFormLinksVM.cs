using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public class RecordAccessThroughFormLinksVM : SimpleOutputVM
    {
        public override string Name => "Record Access Through Form Links";

        public override string Description => "When accessing FormID records, link and access to the record objects they are pointing to.";

        public RecordAccessThroughFormLinksVM(MainVM mvm)
            : base(mvm)
        {
        }

        protected override Task ToDo()
        {
            return RecordAccessThroughFormLinksCode.AccessRecords(this.MainVM.ModFilePath, (s) => this.Output.Add(s));
        }
    }
}
