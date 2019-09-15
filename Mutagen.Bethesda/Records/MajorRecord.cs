using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda
{
    public partial interface IMajorRecord : IFormKey, IMajorRecordCommon
    {
        new FormKey FormKey { get; }
    }

    public partial interface IMajorRecordGetter : IDuplicatable
    {
    }

    [DebuggerDisplay("{GetType().Name} {this.EditorID?.ToString()} {this.FormKey.ToString()}")]
    public partial class MajorRecord : ILinkSubContainer
    {
        public MajorRecordFlag MajorRecordFlags
        {
            get => (MajorRecordFlag)this.MajorRecordFlagsRaw;
            set => this.MajorRecordFlagsRaw = (int)value;
        }

        [Flags]
        public enum MajorRecordFlag
        {
            Compressed = 0x00040000,
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string TitleString => $"{this.EditorID} - {this.FormKey.ToString()}";

        bool IMajorRecordCommon.IsCompressed
        {
            get => this.MajorRecordFlags.HasFlag(MajorRecordFlag.Compressed);
            set => this.MajorRecordFlags = this.MajorRecordFlags.SetFlag(MajorRecordFlag.Compressed, value);
        }

        bool IMajorRecordCommonGetter.IsCompressed
        {
            get => this.MajorRecordFlags.HasFlag(MajorRecordFlag.Compressed);
        }

        object IDuplicatable.Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)> duplicatedRecordTracker)
        {
            return this.Duplicate(getNextFormKey, duplicatedRecordTracker);
        }
    }
}

namespace Mutagen.Bethesda.Internals
{
    public delegate T MajorRecordActivator<T>(FormKey formKey) where T : IMajorRecordInternal;
    public static class MajorRecordInstantiator<T>
        where T : IMajorRecordInternal
    {
        public static readonly MajorRecordActivator<T> Activator;

        static MajorRecordInstantiator()
        {
            if (!LoquiRegistration.TryGetRegister(typeof(T), out var regis))
            {
                throw new ArgumentException();
            }

            var ctorInfo = regis.ClassType.GetConstructors()
                .Where(c => c.GetParameters().Length == 1)
                .Where(c => c.GetParameters()[0].ParameterType == typeof(FormKey))
                .First();
            var paramInfo = ctorInfo.GetParameters();
            ParameterExpression param = Expression.Parameter(typeof(FormKey), "formKey");
            NewExpression newExp = Expression.New(ctorInfo, param);
            LambdaExpression lambda = Expression.Lambda(typeof(MajorRecordActivator<T>), newExp, param);
            Activator = (MajorRecordActivator<T>)lambda.Compile();
        }
    }

    public partial class MajorRecordBinaryWrapper : IMajorRecordCommonGetter
    {
        public bool IsCompressed => ((MajorRecord.MajorRecordFlag)this.MajorRecordFlagsRaw).HasFlag(MajorRecord.MajorRecordFlag.Compressed);

        public Task WriteToXmlFolder(DirectoryPath? dir, string name, XElement node, int counter, ErrorMaskBuilder errorMask)
        {
            throw new NotImplementedException();
        }

        object IDuplicatable.Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)> duplicatedRecordTracker)
        {
            return ((MajorRecordCommon)this.CommonInstance()).Duplicate(this, getNextFormKey, duplicatedRecordTracker);
        }
    }
}
