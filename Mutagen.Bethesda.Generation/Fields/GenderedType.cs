using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class GenderedType : WrapperType
    {
        public override bool HasDefault => false;
        public override bool CopyNeedsTryCatch => true;
        public override bool IsEnumerable => true;
        public override bool IsClass => true;
        public bool ItemHasBeenSet => false;

        public override void GenerateClear(FileGeneration fg, Accessor accessorPrefix)
        {
            if (this.HasBeenSet)
            {
                fg.AppendLine($"{accessorPrefix.DirectAccess} = null;");
            }
            else
            {
                SubTypeGeneration.GenerateClear(fg, $"{accessorPrefix.DirectAccess}.Male");
                SubTypeGeneration.GenerateClear(fg, $"{accessorPrefix.DirectAccess}.Female");
            }
        }

        public override void GenerateForClass(FileGeneration fg)
        {
            fg.AppendLine($"public GenderedItem<{SubTypeGeneration.TypeName(getter: false)}>{(this.HasBeenSet ? "?" : null)} {this.Name} {{ get; set; }}");
            fg.AppendLine($"IGenderedItemGetter<{SubTypeGeneration.TypeName(getter: true)}>{(this.HasBeenSet ? "?" : null)} {this.ObjectGen.Interface(getter: true, internalInterface: true)}.{this.Name} => this.{this.Name};");

        }

        public override string GenerateACopy(string rhsAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateForCopy(FileGeneration fg, Accessor accessor, string rhsAccessorPrefix, string copyMaskAccessor, bool protectedMembers, bool deepCopy)
        {
            if (!deepCopy)
            {
                throw new NotImplementedException();
            }
            if (this.HasBeenSet)
            {
                fg.AppendLine($"if ({rhsAccessorPrefix}.{this.Name} == null)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"{accessor} = null;");
                }
                fg.AppendLine("else");
            }
            using (new BraceWrapper(fg, doIt: this.HasBeenSet))
            {
                using (var args = new ArgsWrapper(fg,
                    $"{accessor} = new GenderedItem<{this.SubTypeGeneration.TypeName(getter: false)}>"))
                {
                    if (this.isLoquiSingle)
                    {
                        LoquiType loqui = this.SubTypeGeneration as LoquiType;
                        loqui.GenerateTypicalMakeCopy(
                            fg,
                            retAccessor: $"male: ",
                            rhsAccessor: $"{accessor}.Male",
                            copyMaskAccessor: $"{copyMaskAccessor}.Male",
                            deepCopy: deepCopy,
                            doTranslationMask: false);
                        loqui.GenerateTypicalMakeCopy(
                            fg,
                            retAccessor: $"female: ",
                            rhsAccessor: $"{accessor}.Female",
                            copyMaskAccessor: $"{copyMaskAccessor}.Female",
                            deepCopy: deepCopy,
                            doTranslationMask: false);
                    }
                    else
                    {
                        args.Add($"male: {this.SubTypeGeneration.GetDuplicate($"rhs.{this.Name}.Male")}");
                        args.Add($"female: {this.SubTypeGeneration.GetDuplicate($"rhs.{this.Name}.Female")}");
                    }
                }
            }
        }

        public override void GenerateForEquals(FileGeneration fg, Accessor accessor, Accessor rhsAccessor)
        {
            fg.AppendLine($"if (!Equals({accessor.DirectAccess}, {rhsAccessor.DirectAccess})) return false;");
        }

        public override void GenerateForEqualsMask(FileGeneration fg, Accessor accessor, Accessor rhsAccessor, string retAccessor)
        {
            if (this.ItemHasBeenSet)
            {
                throw new NotImplementedException();
            }

            string typeStr;
            LoquiType loqui = this.SubTypeGeneration as LoquiType;
            if (loqui != null)
            {
                typeStr = $"GenderedItem<{loqui.GetMaskString("bool")}>";
            }
            else
            {
                typeStr = $"GenderedItem<bool>";
            }

            if (this.HasBeenSet)
            {
                fg.AppendLine($"if ({accessor} == null || {rhsAccessor} == null)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"ret.{this.Name} = new MaskItem<bool, {typeStr}?>({accessor} == null && {rhsAccessor} == null, default);");
                }
            }
            fg.AppendLine("else");
            using (new BraceWrapper(fg))
            {
                if (loqui != null)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"ret.{this.Name} = new {typeStr}"))
                    {
                        args.Add($"male: {accessor.DirectAccess}.Male.{(loqui.TargetObjectGeneration == null ? nameof(IEqualsMask.GetEqualsIMask) : "GetEqualsMask")}({rhsAccessor.DirectAccess}.Male, include)");
                        args.Add($"female: {accessor.DirectAccess}.Female.{(loqui.TargetObjectGeneration == null ? nameof(IEqualsMask.GetEqualsIMask) : "GetEqualsMask")}({rhsAccessor.DirectAccess}.Female, include)");
                    }
                }
                else
                {
                    using (var args = new ArgsWrapper(fg,
                        $"var spec = new {typeStr}"))
                    {
                        args.Add($"male: {this.SubTypeGeneration.GenerateEqualsSnippet($"{accessor}.Male", $"{rhsAccessor}.Male")}");
                        args.Add($"female: {this.SubTypeGeneration.GenerateEqualsSnippet($"{accessor}.Female", $"{rhsAccessor}.Female")}");
                    }
                    fg.AppendLine($"ret.{this.Name} = new MaskItem<bool, {typeStr}?>(spec.Male && spec.Female, spec);");
                }
            }
        }

        public override void GenerateForHasBeenSetCheck(FileGeneration fg, Accessor accessor, string checkMaskAccessor)
        {
            if (this.HasBeenSet && !this.ItemHasBeenSet)
            {
                fg.AppendLine($"if ({checkMaskAccessor}?.Overall ?? false) return false;");
            }
            if (this.ItemHasBeenSet)
            {
                throw new NotImplementedException();
            }
        }

        public override void GenerateForHasBeenSetMaskGetter(FileGeneration fg, Accessor accessor, string retAccessor)
        {
            if (this.ItemHasBeenSet)
            {
                throw new NotImplementedException();
            }
            if (this.SubTypeGeneration is LoquiType loqui)
            {
                throw new NotImplementedException();
            }
            else if (this.HasBeenSet)
            {
                fg.AppendLine($"{retAccessor} = {accessor} == null ? null : new MaskItem<bool, GenderedItem<bool>?>(true, default);");
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override void GenerateForHash(FileGeneration fg, Accessor accessor, string hashResultAccessor)
        {
            if (this.HasBeenSet)
            {
                fg.AppendLine($"{hashResultAccessor} = ({accessor} != null ? HashHelper.GetHashCode({accessor}.Male, {accessor}.Female) : 0).CombineHashCode({hashResultAccessor});");
            }
            else
            {
                fg.AppendLine($"{hashResultAccessor} = HashHelper.GetHashCode({accessor}.Male, {accessor}.Female).CombineHashCode({hashResultAccessor});");
            }
        }

        public override void GenerateForInterface(FileGeneration fg, bool getter, bool internalInterface)
        {
            if (getter)
            {
                fg.AppendLine($"IGenderedItemGetter<{SubTypeGeneration.TypeName(getter: true)}>{(this.HasBeenSet ? "?" : null)} {this.Name} {{ get; }}");
            }
            else
            {
                fg.AppendLine($"new GenderedItem<{SubTypeGeneration.TypeName(getter: false)}>{(this.HasBeenSet ? "?" : null)} {this.Name} {{ get; set; }}");
            }
        }

        public override void GenerateGetNth(FileGeneration fg, Accessor identifier)
        {
            throw new NotImplementedException();
        }

        public override void GenerateSetNth(FileGeneration fg, string accessorPrefix, string rhsAccessorPrefix, bool internalUse)
        {
            throw new NotImplementedException();
        }

        public override void GenerateSetNthHasBeenSet(FileGeneration fg, Accessor identifier, string onIdentifier)
        {
            throw new NotImplementedException();
        }

        public override void GenerateToString(FileGeneration fg, string name, Accessor accessor, string fgAccessor)
        {
            fg.AppendLine($"if ({accessor} != null)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"{fgAccessor}.{nameof(FileGeneration.AppendLine)}(\"{name} =>\");");
                fg.AppendLine($"{fgAccessor}.{nameof(FileGeneration.AppendLine)}(\"[\");");
                fg.AppendLine($"using (new DepthWrapper({fgAccessor}))");
                using (new BraceWrapper(fg))
                {
                    this.SubTypeGeneration.GenerateToString(fg, "Male", $"{accessor.DirectAccess}.Male", fgAccessor);
                    this.SubTypeGeneration.GenerateToString(fg, "Female", $"{accessor.DirectAccess}.Female", fgAccessor);
                }
                fg.AppendLine($"{fgAccessor}.{nameof(FileGeneration.AppendLine)}(\"]\");");
            }
        }

        public override void GenerateUnsetNth(FileGeneration fg, Accessor identifier)
        {
            throw new NotImplementedException();
        }

        public override bool IsNullable() => true;

        public override string SkipCheck(string copyMaskAccessor, bool deepCopy)
        {
            throw new NotImplementedException();
        }

        public override string TypeName(bool getter)
        {
            return $"GenderedItem<{SubTypeGeneration.TypeName(getter)}>";
        }

        public override string GetDuplicate(Accessor accessor)
        {
            throw new NotImplementedException();
        }
    }
}
