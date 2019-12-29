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
            // ToDo
            // Refactor to use BinaryOverlay when linking capabilities are added to it
            IOblivionModGetter mod = await OblivionMod.CreateFromBinary(
                pathToMod,
                importMask: new GroupMask()
                {
                    NPCs = true,
                    Classes = true,
                });
            var links = new LinkingPackage<IOblivionModGetter>(mod, null);
            foreach (var npc in mod.NPCs.Items.Items)
            {
                // Not all NPCs have classes, so skip any that don't have one
                if (npc.Class == null) continue;

                // Access class directly, and get its name.
                // Also, we can still get access to the FormID (Or the more strongly typed FormKey) if desired through the property
                output($"{npc.EditorID} => {npc.Class.Resolve(links).Name} ({npc.Class.FormKey})");
            }
        }
    }
}
