using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class MajorRecordModule : GenerationModule
    {
        public override async Task LoadWrapup(ObjectGeneration obj)
        {
            if (await obj.IsMajorRecord())
            {
                obj.BasicCtorPermission = PermissionLevel.@protected;
            }
            await base.LoadWrapup(obj);
        }

        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
        {
            if (!await obj.IsMajorRecord()) return;
            if (!maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)) return;
            //ToDo
            // Modify to getter interface after copy is refactored
            fg.AppendLine($"partial void PostDuplicate({obj.Name} obj, {obj.ObjectName} rhs, Func<FormKey> getNextFormKey, IList<({nameof(IMajorRecordCommon)} Record, FormKey OriginalFormKey)> duplicatedRecords);");
            fg.AppendLine();

            fg.AppendLine($"public{obj.FunctionOverride()}{nameof(IMajorRecordCommon)} Duplicate({nameof(IMajorRecordCommonGetter)} item, Func<FormKey> getNextFormKey, IList<({nameof(IMajorRecordCommon)} Record, FormKey OriginalFormKey)> duplicatedRecords)");
            using (new BraceWrapper(fg))
            {
                if (obj.Abstract)
                {
                    fg.AppendLine($"throw new {nameof(NotImplementedException)}();");
                }
                else
                {
                    fg.AppendLine($"var ret = new {obj.Name}(getNextFormKey());");
                    //ToDo
                    // Modify to getter interface after copy is refactored
                    fg.AppendLine($"ret.CopyFieldsFrom(({obj.ObjectName})item);");
                    fg.AppendLine("duplicatedRecords?.Add((ret, item.FormKey));");
                    fg.AppendLine($"PostDuplicate(ret, ({obj.ObjectName})item, getNextFormKey, duplicatedRecords);");
                    fg.AppendLine("return ret;");
                }
            }
            fg.AppendLine();
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);
            if (!await obj.IsMajorRecord()) return;
            fg.AppendLine($"public {obj.Name}(FormKey formKey)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("this.FormKey = formKey;");
                fg.AppendLine("CustomCtor();");
            }
            fg.AppendLine();

            fg.AppendLine($"public {obj.Name}(IMod mod)");
            using (new DepthWrapper(fg))
            {
                fg.AppendLine($": this(mod.{nameof(IMod.GetNextFormKey)}())");
            }
            using (new BraceWrapper(fg))
            {
            }
            fg.AppendLine();
        }

        public override async Task GenerateInCommonMixin(ObjectGeneration obj, FileGeneration fg)
        {
            if (!await obj.IsMajorRecord()) return;
            if (!obj.IsTopClass) return;
            using (var args = new FunctionWrapper(fg,
                $"public static {nameof(IMajorRecordCommon)} {nameof(IDuplicatable.Duplicate)}"))
            {
                //ToDo
                // Modify to getter interface after copy is refactored
                args.Add($"this {obj.ObjectName} item");
                args.Add("Func<FormKey> getNextFormKey");
                args.Add($"IList<({nameof(IMajorRecordCommon)} Record, FormKey OriginalFormKey)> duplicatedRecords = null");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                     $"return {obj.CommonClassInstance("item", LoquiInterfaceType.IGetter, CommonGenerics.Class, MaskType.Normal)}.{nameof(IDuplicatable.Duplicate)}"))
                {
                    args.AddPassArg("item");
                    args.AddPassArg("getNextFormKey");
                    args.AddPassArg("duplicatedRecords");
                }
            }
        }
    }
}
