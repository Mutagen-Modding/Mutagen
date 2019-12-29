using DynamicData;
using Mutagen.Bethesda.Oblivion;
using Noggog.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public class ImportComparisonVM : SimpleOutputVM
    {
        public override string Name => "Import Comparisons";

        public override string Description => "";

        public ImportComparisonVM(MainVM mvm) 
            : base(mvm)
        {
        }

        protected override async Task ToDo()
        {
            await TimeAndReport((s) => ImportComparisonCode.ImportIntoInMemoryObject(s), "Binary");
            await TimeAndReport(async (s) => ImportComparisonCode.ImportViaBinaryOverlay(s), "Binary Overlay");
        }

        private async Task TimeAndReport(Func<string, Task<IOblivionModGetter>> toDo, string jobName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var mod = await toDo(this.MainVM.ModFilePath);
            sw.Stop();
            this.Output.Add($"{jobName} has {mod.NPCs.Items.Count} NPCs. Took: {sw.ElapsedMilliseconds}ms");
        }
    }
}
