using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Generation
{
    public class BreakType : TypeGeneration
    {
        public override bool IsEnumerable => throw new NotImplementedException();

        public override bool IsClass => throw new NotImplementedException();

        public override bool HasDefault => throw new NotImplementedException();

        public override bool CopyNeedsTryCatch => throw new NotImplementedException();

        public override bool Copy => true;

        public int Index;

        public BreakType()
        {
            IntegrateField = false;
        }

        public override string GenerateACopy(string rhsAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateClear(FileGeneration fg, Accessor accessorPrefix)
        {
            throw new NotImplementedException();
        }

        public override void GenerateForClass(FileGeneration fg)
        {
            throw new NotImplementedException();
        }

        public override void GenerateForCopy(FileGeneration fg, Accessor accessor, Accessor rhs, Accessor copyMaskAccessor, bool protectedMembers, bool deepCopy)
        {
            fg.AppendLine($"if ({rhs}.{VersioningModule.VersioningFieldName}.HasFlag({this.ObjectGen.ObjectName}.{VersioningModule.VersioningEnumName}.Break{Index})) return;");
        }

        public override void GenerateForEquals(FileGeneration fg, Accessor accessor, Accessor rhsAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateForEqualsMask(FileGeneration fg, Accessor accessor, Accessor rhsAccessor, string retAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateForHasBeenSetCheck(FileGeneration fg, Accessor accessor, string checkMaskAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateForHasBeenSetMaskGetter(FileGeneration fg, Accessor accessor, string retAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateForHash(FileGeneration fg, Accessor accessor, string hashResultAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateForInterface(FileGeneration fg, bool getter, bool internalInterface)
        {
            throw new NotImplementedException();
        }

        public override void GenerateGetNth(FileGeneration fg, Accessor identifier)
        {
            throw new NotImplementedException();
        }

        public override void GenerateSetNth(FileGeneration fg, Accessor accessor, Accessor rhs, bool internalUse)
        {
            throw new NotImplementedException();
        }

        public override void GenerateSetNthHasBeenSet(FileGeneration fg, Accessor identifier, string onIdentifier)
        {
            throw new NotImplementedException();
        }

        public override void GenerateToString(FileGeneration fg, string name, Accessor accessor, string fgAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateUnsetNth(FileGeneration fg, Accessor identifier)
        {
            throw new NotImplementedException();
        }

        public override string GetDuplicate(Accessor accessor)
        {
            throw new NotImplementedException();
        }

        public override bool IsNullable()
        {
            throw new NotImplementedException();
        }

        public override string SkipCheck(Accessor copyMaskAccessor, bool deepCopy)
        {
            throw new NotImplementedException();
        }

        public override string TypeName(bool getter)
        {
            throw new NotImplementedException();
        }
    }
}
