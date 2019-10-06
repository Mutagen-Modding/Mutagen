using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public class RecordOverrideOrDuplicationCode : SimpleOutputVM
    {
        public RecordOverrideOrDuplicationCode(MainVM mvm) 
            : base(mvm)
        {
        }

        public override string Name => throw new NotImplementedException();

        public override string Description => throw new NotImplementedException();

        public static void DoSomeModifications(string pathToMod, string pathToExport, Action<string> output)
        {
            IOblivionModGetter mod = OblivionMod.CreateFromBinaryWrapper(pathToMod);
            var sourceArmor = mod.Armors.Items.Items.FirstOrDefault();
            if (sourceArmor == null) return;

            OblivionMod outgoingMod = new OblivionMod(new ModKey("Outgoing", master: false));

            // Can override by just making a copy, and modifying the copy. It will have the FormKey of the 
            // original, so when added to our outgoing mod it will be considered an override.
            //armor.Duplicate();
            //var armorOverride = new Armor();
            //armorOverride.CopyFieldsFrom(armor);
            //armorOverride.Name = $"Overridden {armor.Name}";

            //// By duplicating it, we create a copy with a new FormKey, originating from our outgoing mod.  
            //// This creates a record duplication, rather than an override
            //var armorDuplicate = new Armor();


            //outgoingMod.Armors.Major
            //var anotherArmorDuplicate = new Armor(outgoingMod);
        }

        protected override Task ToDo()
        {
            throw new NotImplementedException();
        }
    }
}
