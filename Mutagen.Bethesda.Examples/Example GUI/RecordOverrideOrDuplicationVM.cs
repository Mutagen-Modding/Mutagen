using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public class RecordOverrideOrDuplicationVM : SimpleOutputVM
    {
        public RecordOverrideOrDuplicationVM(MainVM mvm) 
            : base(mvm)
        {
        }

        public override string Name => "Record Override and Duplication";

        public override string Description => "How to override a record, or duplicate one as a new separate record.";

        protected override async Task ToDo()
        {
            //RecordOverrideOrDuplicationCode.DoSomeModifications(this.MainVM.ModFilePath, DelayedOutput.Add);
        }
    }
}
