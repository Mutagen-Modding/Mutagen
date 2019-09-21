using Noggog.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public abstract class ExampleVM : ViewModel
    {
        public MainVM MainVM { get; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public ExampleVM(MainVM mvm)
        {
            this.MainVM = mvm;
        }
    }
}
