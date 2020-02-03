using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public class RecordAccessThroughFormLinksCode
    {
        /// Class is not a part of NPC itself, but of the Class major record that the NPC points to via FormID.
        /// However, the FormID concepts are abstracted away so we can just reference the Class record each NPC is pointing to directly.
        public static async Task AccessRecords(string pathToMod, Action<string> output)
        {
            IOblivionModGetter mod = OblivionMod.CreateFromBinaryOverlay(pathToMod);
            var links = mod.CreateLinkCache();
            foreach (var npc in mod.NPCs.Records)
            {
                // Not all NPCs have classes, so skip any that don't have one
                if (npc.Class == null) continue;

                // Reolve to class major record, and get its name.
                // Also, we can still get access to the FormID (Or the more strongly typed FormKey) if desired through the property
                if (npc.Class.TryResolve(links, out var classRecord))
                {
                    output($"{npc.EditorID} => {classRecord.Name} ({npc.Class.FormKey})");
                }

                // Can also Resolve without using an if statement, as long as we handle the null potential
                //output($"{npc.EditorID} => {npc.Class.Resolve(links)?.Name} ({npc.Class.FormKey})");
            }
        }
    }
}
