using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class ObservableModModule : GenerationModule
    {
        public override async Task MiscellaneousGenerationActions(ObjectGeneration obj)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            FileGeneration fg = new FileGeneration();
            AddUsings(fg);
            fg.AppendLine($"namespace {obj.Namespace}");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"public class {obj.Name}_Observable");
                using (new BraceWrapper(fg))
                {
                    await GenerateMembers(obj, fg);
                }
            }

            var fileName = Path.Combine(obj.TargetDir.FullName, $"{obj.Name}_Observable.cs");
            fg.Generate(fileName);
            obj.ProtoGen.GeneratedFiles[fileName] = ProjItemType.Compile;
        }

        private static void AddUsings(FileGeneration fg)
        {
            fg.AppendLine("using Loqui;");
            fg.AppendLine("using Mutagen.Bethesda.Binary;");
            fg.AppendLine("using Mutagen.Bethesda.Oblivion.Internals;");
            fg.AppendLine("using Noggog;");
            fg.AppendLine("using System;");
            fg.AppendLine("using System.Collections.Generic;");
            fg.AppendLine("using System.Linq;");
            fg.AppendLine("using System.Reactive.Linq;");
            fg.AppendLine("using System.Threading.Tasks;");
            fg.AppendLine();
        }

        private async Task GenerateMembers(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var item in IterateLoqui(obj))
            {
                if (item.IsGroup)
                {
                    fg.AppendLine($"public IObservable<GroupObservable{item.Loqui.GenericTypes}> {item.Loqui.Name} {{ get; private set; }}");
                }
                else
                {
                    fg.AppendLine($"public IObservable<{item.Loqui.TypeName}> {item.Loqui.Name} {{ get; private set; }}");
                }
            }
        }

        private IEnumerable<(LoquiType Loqui, bool IsGroup)> IterateLoqui(ObjectGeneration obj)
        {
            foreach (var item in obj.IterateFields())
            {
                if (!(item is LoquiType loqui))
                {
                    throw new ArgumentException();
                }
                yield return (
                    loqui,
                    loqui.TargetObjectGeneration?.Name.Equals("Group") ?? false);
            }
        }
    }
}
