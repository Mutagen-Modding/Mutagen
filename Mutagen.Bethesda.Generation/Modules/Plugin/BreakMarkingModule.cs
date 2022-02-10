using System.Threading.Tasks;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;

namespace Mutagen.Bethesda.Generation.Modules.Plugin
{
    public class BreakMarkingModule : GenerationModule
    {
        public override async Task PostLoad(ObjectGeneration obj)
        {
            bool passedBreak = false;
            foreach (var field in obj.Fields)
            {
                field.GetFieldData().IsAfterBreak = passedBreak;
                if (field is BreakType)
                {
                    passedBreak = true;
                }
            }
        }
    }
}