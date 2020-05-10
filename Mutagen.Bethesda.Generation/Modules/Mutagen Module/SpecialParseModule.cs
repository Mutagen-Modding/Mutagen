using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class SpecialParseModule : GenerationModule
    {
        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
        {
            if (!maskTypes.Applicable(LoquiInterfaceType.ISetter, CommonGenerics.Class)) return;
            foreach (var field in obj.IterateFields(nonIntegrated: true, expandSets: SetMarkerType.ExpandSets.True))
            {
                if (!(field is SpecialParseType special)) continue;
                using (var args = new ArgsWrapper(fg,
                    $"static partial void SpecialParse_{field.Name}"))
                {
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                    args.Add($"{nameof(MutagenFrame)} frame");
                }
                using (var args = new ArgsWrapper(fg,
                    $"static partial void SpecialWrite_{field.Name}"))
                {
                    args.Add($"{obj.Interface(getter: true, internalInterface: true)} item");
                    args.Add($"{nameof(MutagenWriter)} writer");
                }
                using (var args = new FunctionWrapper(fg,
                    $"internal static void SpecialWrite_{field.Name}_Internal"))
                {
                    args.Add($"{obj.Interface(getter: true, internalInterface: true)} item");
                    args.Add($"{nameof(MutagenWriter)} writer");
                }
                using (new BraceWrapper(fg))
                {
                    using (var args = new ArgsWrapper(fg,
                        $"SpecialWrite_{field.Name}"))
                    {
                        args.Add($"item: item");
                        args.Add($"writer: writer");
                    }
                }
            }
            await base.GenerateInClass(obj, fg);
        }
    }
}
