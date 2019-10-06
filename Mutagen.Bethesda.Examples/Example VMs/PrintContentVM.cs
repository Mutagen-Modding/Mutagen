using DynamicData;
using DynamicData.Binding;
using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public class PrintContentVM : SimpleOutputVM
    {
        public override string Name => "Print Content";

        public override string Description => "Print all unique names of NPCs";

        public PrintContentVM(MainVM mvm)
            : base(mvm)
        {
        }

        protected override async Task ToDo()
        {
            PrintContentCode.PrintContent(this.MainVM.ModFilePath, (s) => this.Output.Add(s));
        }
    }
}
