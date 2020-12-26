using Loqui;
using Loqui.Generation;
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
                $"public static {obj.ObjectName} Duplicate{obj.GetGenericTypes(MaskType.Normal, MaskType.NormalGetter, MaskType.Translation)}"))
            {
                args.Wheres.AddRange(obj.GenericTypeMaskWheres(LoquiInterfaceType.ISetter, MaskType.Normal, MaskType.NormalGetter, MaskType.Translation));
                args.Add($"this {obj.Interface(obj.GetGenericTypes(MaskType.NormalGetter), getter: true, internalInterface: true)} item");
                args.Add($"{obj.Mask(MaskType.Translation)}? copyMask = null");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {obj.CommonClassInstance("item", LoquiInterfaceType.ISetter, CommonGenerics.Functions, MaskType.NormalGetter, MaskType.Translation)}.Duplicate{obj.GetGenericTypes(MaskType.Normal, MaskType.NormalGetter, MaskType.Translation)}"))
                {
                    args.AddPassArg("item");
                    args.AddPassArg("copyMask");
                }
            }
            fg.AppendLine();
        }

        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
        {
            await base.GenerateInCommon(obj, fg, maskTypes);
            if (!await obj.IsMajorRecord()) return;
            using (new RegionWrapper(fg, "Duplicate"))
            {
                using (var args = new FunctionWrapper(fg,
                    $"public{obj.Virtual()}void Duplicate{obj.GetGenericTypes(MaskType.Normal, MaskType.NormalGetter)}"))
                {
                    args.Wheres.AddRange(obj.GenericTypeMaskWheres(LoquiInterfaceType.ISetter, MaskType.Normal, MaskType.NormalGetter));
                    args.Add($"{obj.Interface(getter: false)} item");
                    args.Add($"{obj.Interface(obj.GetGenericTypes(MaskType.NormalGetter), getter: true)} rhs");
                    args.Add($"TranslationCrystal? copyMask");
                }
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("var ")
                }
                fg.AppendLine();

                foreach (var baseClass in obj.BaseClassTrail())
                {
                    if (baseClass.HasInternalGetInterface || baseClass.HasInternalSetInterface)
                    {
                        using (var args = new FunctionWrapper(fg,
                            $"public override void Duplicate{baseClass.GetGenericTypes(MaskType.Normal, MaskType.NormalGetter)}"))
                        {
                            args.Wheres.AddRange(baseClass.GenericTypeMaskWheres(LoquiInterfaceType.ISetter, MaskType.Normal, MaskType.NormalGetter));
                            args.Add($"{baseClass.Interface(getter: false, internalInterface: true)} item");
                            args.Add($"{baseClass.Interface(baseClass.GetGenericTypes(MaskType.NormalGetter), getter: true, internalInterface: true)} rhs");
                            args.Add($"TranslationCrystal? copyMask");
                        }
                        using (new BraceWrapper(fg))
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"this.Duplicate"))
                            {
                                args.Add($"item: ({obj.Interface(getter: false, internalInterface: true)})item");
                                args.Add($"rhs: ({obj.Interface(baseClass.GetGenericTypes(MaskType.NormalGetter), getter: true, internalInterface: true)})rhs");
                                args.AddPassArg("copyMask");
                            }
                        }
                    }
                    fg.AppendLine();

                    using (var args = new FunctionWrapper(fg,
                        $"public override void Duplicate{baseClass.GetGenericTypes(MaskType.Normal, MaskType.NormalGetter)}"))
                    {
                        args.Wheres.AddRange(baseClass.GenericTypeMaskWheres(LoquiInterfaceType.ISetter, MaskType.Normal, MaskType.NormalGetter));
                        args.Add($"{baseClass.Interface(getter: false)} item");
                        args.Add($"{baseClass.Interface(baseClass.GetGenericTypes(MaskType.NormalGetter), getter: true)} rhs");
                        args.Add($"TranslationCrystal? copyMask");
                    }
                    using (new BraceWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"this.Duplicate"))
                        {
                            args.Add($"item: ({obj.Interface(getter: false)})item");
                            args.Add($"rhs: ({obj.Interface(baseClass.GetGenericTypes(MaskType.NormalGetter), getter: true)})rhs");
                            args.AddPassArg("copyMask");
                        }
                    }
                    fg.AppendLine();
                }
            }
        }
    }
}
