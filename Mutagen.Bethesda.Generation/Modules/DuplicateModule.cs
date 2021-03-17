using Loqui;
using Loqui.Generation;
using Loqui.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class DuplicateModule : GenerationModule
    {
        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);
            if (!await obj.IsMajorRecord()) return;
        }

        public override async Task GenerateInInterface(ObjectGeneration obj, FileGeneration fg, bool internalInterface, bool getter)
        {
            await base.GenerateInInterface(obj, fg, internalInterface, getter);
        }

        public override async Task GenerateInCommonMixin(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInCommonMixin(obj, fg);
            if (!await obj.IsMajorRecord()) return;
            using (var args = new FunctionWrapper(fg,
                $"public static {obj.ObjectName} Duplicate{obj.GetGenericTypes(MaskType.Normal, MaskType.NormalGetter)}"))
            {
                args.Wheres.AddRange(obj.GenericTypeMaskWheres(LoquiInterfaceType.IGetter, MaskType.Normal, MaskType.NormalGetter));
                args.Add($"this {obj.Interface(obj.GetGenericTypes(MaskType.NormalGetter), getter: true, internalInterface: true)} item");
                args.Add($"{nameof(FormKey)} formKey");
                args.Add($"{obj.Mask(MaskType.Translation)}? copyMask = null");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {obj.CommonClassInstance("item", LoquiInterfaceType.IGetter, CommonGenerics.Functions, MaskType.NormalGetter)}.Duplicate{obj.GetGenericTypes(MaskType.Normal, MaskType.NormalGetter, MaskType.Translation)}"))
                {
                    args.AddPassArg("item");
                    args.AddPassArg("formKey");
                    args.Add("copyMask: copyMask?.GetCrystal()");
                }
            }
            fg.AppendLine();
        }

        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
        {
            if (!maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class)) return;
            await base.GenerateInCommon(obj, fg, maskTypes);
            if (!await obj.IsMajorRecord()) return;
            using (new RegionWrapper(fg, "Duplicate"))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public{obj.Virtual()}{obj.Name} Duplicate{obj.GetGenericTypes(MaskType.Normal, MaskType.NormalGetter)}"))
                {
                    args.Wheres.AddRange(obj.GenericTypeMaskWheres(LoquiInterfaceType.IGetter, MaskType.Normal, MaskType.NormalGetter));
                    args.Add($"{obj.Interface(getter: true)} item");
                    args.Add($"{nameof(FormKey)} formKey");
                    args.Add($"TranslationCrystal? copyMask");
                }
                using (new BraceWrapper(fg))
                {
                    if (obj.Abstract)
                    {
                        fg.AppendLine("throw new NotImplementedException();");
                    }
                    else
                    {
                        fg.AppendLine($"var newRec = new {obj.Name}(formKey{(obj.GetObjectData().HasMultipleReleases ? $", item.FormVersion" : null)});");
                        fg.AppendLine($"newRec.DeepCopyIn(item, default({nameof(ErrorMaskBuilder)}?), copyMask);");
                        fg.AppendLine("return newRec;");
                    }
                }
                fg.AppendLine();

                foreach (var baseClass in obj.BaseClassTrail())
                {
                    using (var args = new FunctionWrapper(fg,
                        $"public override {baseClass.Name} Duplicate{baseClass.GetGenericTypes(MaskType.Normal, MaskType.NormalGetter)}"))
                    {
                        args.Wheres.AddRange(baseClass.GenericTypeMaskWheres(LoquiInterfaceType.IGetter, MaskType.Normal, MaskType.NormalGetter));
                        args.Add($"{baseClass.Interface(getter: true)} item");
                        args.Add($"{nameof(FormKey)} formKey");
                        args.Add($"TranslationCrystal? copyMask");
                    }
                    using (new BraceWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"return this.Duplicate"))
                        {
                            args.Add($"item: ({obj.Interface(getter: true)})item");
                            args.AddPassArg("formKey");
                            args.AddPassArg("copyMask");
                        }
                    }
                    fg.AppendLine();
                }
            }
        }
    }
}
