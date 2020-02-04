using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public class RecordOverrideOrDuplicationCode
    {
        public static void DoSomeModifications(string pathToMod, string pathToExport, Action<string> output)
        {
            IArmorGetter sourceArmor;
            using (var mod = OblivionMod.CreateFromBinaryOverlay(pathToMod))
            {
                sourceArmor = mod.Armors.Records.FirstOrDefault();
            }
            if (sourceArmor == null) return;

            OblivionMod outgoingMod = new OblivionMod(new ModKey("Outgoing", master: false));

            // Can override by just making a copy, and modifying the copy. It will have the FormKey of the 
            // original, so when added to our outgoing mod it will be considered an override.
            // Note: a cast is required here currently, but will eventually be unnecessary
            var armorOverride = sourceArmor.DeepCopy() as IArmor;
            armorOverride.Name = $"Overridden {sourceArmor.Name}";

            // By duplicating it, we instead create a copy with a new FormKey, originating from our outgoing mod.  
            // This creates a record duplication, rather than an override
            var armorDuplicate = outgoingMod.Armors.AddNew();
            armorDuplicate.DeepCopyIn(sourceArmor);
            armorDuplicate.Name = $"Duplicated {sourceArmor.Name}";

            // Export both the overridden original record, as well as a duplicate new record
            outgoingMod.WriteToBinary(pathToExport);
        }
    }
}
