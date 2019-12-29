using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public static class PrintContentCode
    {
        public static void PrintContent(string pathToMod, Action<string> output)
        {
            IOblivionModGetter mod = OblivionMod.CreateFromBinaryOverlay(pathToMod);
            foreach (var name in mod.NPCs.Items.Items
                .Select(npc => npc.Name)
                .Distinct()
                .Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                output(name);
            }
        }
    }
}
