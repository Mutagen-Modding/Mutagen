/*
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * Autogenerated by Loqui.  Do not manually change.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*/
#region Usings
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Pex;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using System.Diagnostics;
#endregion

#nullable enable
namespace Mutagen.Bethesda.Pex
{
    #region Class
    public partial class PexObjectStructInfo :
        IEquatable<IPexObjectStructInfoGetter>,
        ILoquiObjectSetter<PexObjectStructInfo>,
        IPexObjectStructInfo
    {
        #region Ctor
        public PexObjectStructInfo()
        {
            CustomCtor();
        }
        partial void CustomCtor();
        #endregion

        #region Name
        public String? Name { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String? IPexObjectStructInfoGetter.Name => this.Name;
        #endregion
        #region Members
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ExtendedList<PexObjectStructInfoMember> _Members = new ExtendedList<PexObjectStructInfoMember>();
        public ExtendedList<PexObjectStructInfoMember> Members
        {
            get => this._Members;
            init => this._Members = value;
        }
        #region Interface Members
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IPexObjectStructInfoMemberGetter> IPexObjectStructInfoGetter.Members => _Members;
        #endregion

        #endregion

        #region To String

        public void Print(
            StructuredStringBuilder sb,
            string? name = null)
        {
            PexObjectStructInfoMixIn.Print(
                item: this,
                sb: sb,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IPexObjectStructInfoGetter rhs) return false;
            return ((PexObjectStructInfoCommon)((IPexObjectStructInfoGetter)this).CommonInstance()!).Equals(this, rhs, equalsMask: null);
        }

        public bool Equals(IPexObjectStructInfoGetter? obj)
        {
            return ((PexObjectStructInfoCommon)((IPexObjectStructInfoGetter)this).CommonInstance()!).Equals(this, obj, equalsMask: null);
        }

        public override int GetHashCode() => ((PexObjectStructInfoCommon)((IPexObjectStructInfoGetter)this).CommonInstance()!).GetHashCode(this);

        #endregion

        #region Mask
        public class Mask<TItem> :
            IEquatable<Mask<TItem>>,
            IMask<TItem>
        {
            #region Ctors
            public Mask(TItem initialValue)
            {
                this.Name = initialValue;
                this.Members = new MaskItem<TItem, IEnumerable<MaskItemIndexed<TItem, PexObjectStructInfoMember.Mask<TItem>?>>?>(initialValue, Enumerable.Empty<MaskItemIndexed<TItem, PexObjectStructInfoMember.Mask<TItem>?>>());
            }

            public Mask(
                TItem Name,
                TItem Members)
            {
                this.Name = Name;
                this.Members = new MaskItem<TItem, IEnumerable<MaskItemIndexed<TItem, PexObjectStructInfoMember.Mask<TItem>?>>?>(Members, Enumerable.Empty<MaskItemIndexed<TItem, PexObjectStructInfoMember.Mask<TItem>?>>());
            }

            #pragma warning disable CS8618
            protected Mask()
            {
            }
            #pragma warning restore CS8618

            #endregion

            #region Members
            public TItem Name;
            public MaskItem<TItem, IEnumerable<MaskItemIndexed<TItem, PexObjectStructInfoMember.Mask<TItem>?>>?>? Members;
            #endregion

            #region Equals
            public override bool Equals(object? obj)
            {
                if (!(obj is Mask<TItem> rhs)) return false;
                return Equals(rhs);
            }

            public bool Equals(Mask<TItem>? rhs)
            {
                if (rhs == null) return false;
                if (!object.Equals(this.Name, rhs.Name)) return false;
                if (!object.Equals(this.Members, rhs.Members)) return false;
                return true;
            }
            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(this.Name);
                hash.Add(this.Members);
                return hash.ToHashCode();
            }

            #endregion

            #region All
            public bool All(Func<TItem, bool> eval)
            {
                if (!eval(this.Name)) return false;
                if (this.Members != null)
                {
                    if (!eval(this.Members.Overall)) return false;
                    if (this.Members.Specific != null)
                    {
                        foreach (var item in this.Members.Specific)
                        {
                            if (!eval(item.Overall)) return false;
                            if (item.Specific != null && !item.Specific.All(eval)) return false;
                        }
                    }
                }
                return true;
            }
            #endregion

            #region Any
            public bool Any(Func<TItem, bool> eval)
            {
                if (eval(this.Name)) return true;
                if (this.Members != null)
                {
                    if (eval(this.Members.Overall)) return true;
                    if (this.Members.Specific != null)
                    {
                        foreach (var item in this.Members.Specific)
                        {
                            if (!eval(item.Overall)) return false;
                            if (item.Specific != null && !item.Specific.All(eval)) return false;
                        }
                    }
                }
                return false;
            }
            #endregion

            #region Translate
            public Mask<R> Translate<R>(Func<TItem, R> eval)
            {
                var ret = new PexObjectStructInfo.Mask<R>();
                this.Translate_InternalFill(ret, eval);
                return ret;
            }

            protected void Translate_InternalFill<R>(Mask<R> obj, Func<TItem, R> eval)
            {
                obj.Name = eval(this.Name);
                if (Members != null)
                {
                    obj.Members = new MaskItem<R, IEnumerable<MaskItemIndexed<R, PexObjectStructInfoMember.Mask<R>?>>?>(eval(this.Members.Overall), Enumerable.Empty<MaskItemIndexed<R, PexObjectStructInfoMember.Mask<R>?>>());
                    if (Members.Specific != null)
                    {
                        var l = new List<MaskItemIndexed<R, PexObjectStructInfoMember.Mask<R>?>>();
                        obj.Members.Specific = l;
                        foreach (var item in Members.Specific)
                        {
                            MaskItemIndexed<R, PexObjectStructInfoMember.Mask<R>?>? mask = item == null ? null : new MaskItemIndexed<R, PexObjectStructInfoMember.Mask<R>?>(item.Index, eval(item.Overall), item.Specific?.Translate(eval));
                            if (mask == null) continue;
                            l.Add(mask);
                        }
                    }
                }
            }
            #endregion

            #region To String
            public override string ToString() => this.Print();

            public string Print(PexObjectStructInfo.Mask<bool>? printMask = null)
            {
                var sb = new StructuredStringBuilder();
                Print(sb, printMask);
                return sb.ToString();
            }

            public void Print(StructuredStringBuilder sb, PexObjectStructInfo.Mask<bool>? printMask = null)
            {
                sb.AppendLine($"{nameof(PexObjectStructInfo.Mask<TItem>)} =>");
                using (sb.Brace())
                {
                    if (printMask?.Name ?? true)
                    {
                        sb.AppendItem(Name, "Name");
                    }
                    if ((printMask?.Members?.Overall ?? true)
                        && Members is {} MembersItem)
                    {
                        sb.AppendLine("Members =>");
                        using (sb.Brace())
                        {
                            sb.AppendItem(MembersItem.Overall);
                            if (MembersItem.Specific != null)
                            {
                                foreach (var subItem in MembersItem.Specific)
                                {
                                    using (sb.Brace())
                                    {
                                        subItem?.Print(sb);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

        }

        public class ErrorMask :
            IErrorMask,
            IErrorMask<ErrorMask>
        {
            #region Members
            public Exception? Overall { get; set; }
            private List<string>? _warnings;
            public List<string> Warnings
            {
                get
                {
                    if (_warnings == null)
                    {
                        _warnings = new List<string>();
                    }
                    return _warnings;
                }
            }
            public Exception? Name;
            public MaskItem<Exception?, IEnumerable<MaskItem<Exception?, PexObjectStructInfoMember.ErrorMask?>>?>? Members;
            #endregion

            #region IErrorMask
            public object? GetNthMask(int index)
            {
                PexObjectStructInfo_FieldIndex enu = (PexObjectStructInfo_FieldIndex)index;
                switch (enu)
                {
                    case PexObjectStructInfo_FieldIndex.Name:
                        return Name;
                    case PexObjectStructInfo_FieldIndex.Members:
                        return Members;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public void SetNthException(int index, Exception ex)
            {
                PexObjectStructInfo_FieldIndex enu = (PexObjectStructInfo_FieldIndex)index;
                switch (enu)
                {
                    case PexObjectStructInfo_FieldIndex.Name:
                        this.Name = ex;
                        break;
                    case PexObjectStructInfo_FieldIndex.Members:
                        this.Members = new MaskItem<Exception?, IEnumerable<MaskItem<Exception?, PexObjectStructInfoMember.ErrorMask?>>?>(ex, null);
                        break;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public void SetNthMask(int index, object obj)
            {
                PexObjectStructInfo_FieldIndex enu = (PexObjectStructInfo_FieldIndex)index;
                switch (enu)
                {
                    case PexObjectStructInfo_FieldIndex.Name:
                        this.Name = (Exception?)obj;
                        break;
                    case PexObjectStructInfo_FieldIndex.Members:
                        this.Members = (MaskItem<Exception?, IEnumerable<MaskItem<Exception?, PexObjectStructInfoMember.ErrorMask?>>?>)obj;
                        break;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public bool IsInError()
            {
                if (Overall != null) return true;
                if (Name != null) return true;
                if (Members != null) return true;
                return false;
            }
            #endregion

            #region To String
            public override string ToString() => this.Print();

            public void Print(StructuredStringBuilder sb, string? name = null)
            {
                sb.AppendLine($"{(name ?? "ErrorMask")} =>");
                using (sb.Brace())
                {
                    if (this.Overall != null)
                    {
                        sb.AppendLine("Overall =>");
                        using (sb.Brace())
                        {
                            sb.AppendLine($"{this.Overall}");
                        }
                    }
                    PrintFillInternal(sb);
                }
            }
            protected void PrintFillInternal(StructuredStringBuilder sb)
            {
                {
                    sb.AppendItem(Name, "Name");
                }
                if (Members is {} MembersItem)
                {
                    sb.AppendLine("Members =>");
                    using (sb.Brace())
                    {
                        sb.AppendItem(MembersItem.Overall);
                        if (MembersItem.Specific != null)
                        {
                            foreach (var subItem in MembersItem.Specific)
                            {
                                using (sb.Brace())
                                {
                                    subItem?.Print(sb);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Combine
            public ErrorMask Combine(ErrorMask? rhs)
            {
                if (rhs == null) return this;
                var ret = new ErrorMask();
                ret.Name = this.Name.Combine(rhs.Name);
                ret.Members = new MaskItem<Exception?, IEnumerable<MaskItem<Exception?, PexObjectStructInfoMember.ErrorMask?>>?>(Noggog.ExceptionExt.Combine(this.Members?.Overall, rhs.Members?.Overall), Noggog.ExceptionExt.Combine(this.Members?.Specific, rhs.Members?.Specific));
                return ret;
            }
            public static ErrorMask? Combine(ErrorMask? lhs, ErrorMask? rhs)
            {
                if (lhs != null && rhs != null) return lhs.Combine(rhs);
                return lhs ?? rhs;
            }
            #endregion

            #region Factory
            public static ErrorMask Factory(ErrorMaskBuilder errorMask)
            {
                return new ErrorMask();
            }
            #endregion

        }
        public class TranslationMask : ITranslationMask
        {
            #region Members
            private TranslationCrystal? _crystal;
            public readonly bool DefaultOn;
            public bool OnOverall;
            public bool Name;
            public PexObjectStructInfoMember.TranslationMask? Members;
            #endregion

            #region Ctors
            public TranslationMask(
                bool defaultOn,
                bool onOverall = true)
            {
                this.DefaultOn = defaultOn;
                this.OnOverall = onOverall;
                this.Name = defaultOn;
            }

            #endregion

            public TranslationCrystal GetCrystal()
            {
                if (_crystal != null) return _crystal;
                var ret = new List<(bool On, TranslationCrystal? SubCrystal)>();
                GetCrystal(ret);
                _crystal = new TranslationCrystal(ret.ToArray());
                return _crystal;
            }

            protected void GetCrystal(List<(bool On, TranslationCrystal? SubCrystal)> ret)
            {
                ret.Add((Name, null));
                ret.Add((Members == null ? DefaultOn : !Members.GetCrystal().CopyNothing, Members?.GetCrystal()));
            }

            public static implicit operator TranslationMask(bool defaultOn)
            {
                return new TranslationMask(defaultOn: defaultOn, onOverall: defaultOn);
            }

        }
        #endregion

        void IPrintable.Print(StructuredStringBuilder sb, string? name) => this.Print(sb, name);

        void IClearable.Clear()
        {
            ((PexObjectStructInfoSetterCommon)((IPexObjectStructInfoGetter)this).CommonSetterInstance()!).Clear(this);
        }

        internal static PexObjectStructInfo GetNew()
        {
            return new PexObjectStructInfo();
        }

    }
    #endregion

    #region Interface
    public partial interface IPexObjectStructInfo :
        ILoquiObjectSetter<IPexObjectStructInfo>,
        IPexObjectStructInfoGetter
    {
        new String? Name { get; set; }
        new ExtendedList<PexObjectStructInfoMember> Members { get; }
    }

    public partial interface IPexObjectStructInfoGetter :
        ILoquiObject,
        ILoquiObject<IPexObjectStructInfoGetter>
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object? CommonSetterInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonSetterTranslationInstance();
        static ILoquiRegistration StaticRegistration => PexObjectStructInfo_Registration.Instance;
        String? Name { get; }
        IReadOnlyList<IPexObjectStructInfoMemberGetter> Members { get; }

    }

    #endregion

    #region Common MixIn
    public static partial class PexObjectStructInfoMixIn
    {
        public static void Clear(this IPexObjectStructInfo item)
        {
            ((PexObjectStructInfoSetterCommon)((IPexObjectStructInfoGetter)item).CommonSetterInstance()!).Clear(item: item);
        }

        public static PexObjectStructInfo.Mask<bool> GetEqualsMask(
            this IPexObjectStructInfoGetter item,
            IPexObjectStructInfoGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            return ((PexObjectStructInfoCommon)((IPexObjectStructInfoGetter)item).CommonInstance()!).GetEqualsMask(
                item: item,
                rhs: rhs,
                include: include);
        }

        public static string Print(
            this IPexObjectStructInfoGetter item,
            string? name = null,
            PexObjectStructInfo.Mask<bool>? printMask = null)
        {
            return ((PexObjectStructInfoCommon)((IPexObjectStructInfoGetter)item).CommonInstance()!).Print(
                item: item,
                name: name,
                printMask: printMask);
        }

        public static void Print(
            this IPexObjectStructInfoGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            PexObjectStructInfo.Mask<bool>? printMask = null)
        {
            ((PexObjectStructInfoCommon)((IPexObjectStructInfoGetter)item).CommonInstance()!).Print(
                item: item,
                sb: sb,
                name: name,
                printMask: printMask);
        }

        public static bool Equals(
            this IPexObjectStructInfoGetter item,
            IPexObjectStructInfoGetter rhs,
            PexObjectStructInfo.TranslationMask? equalsMask = null)
        {
            return ((PexObjectStructInfoCommon)((IPexObjectStructInfoGetter)item).CommonInstance()!).Equals(
                lhs: item,
                rhs: rhs,
                equalsMask: equalsMask?.GetCrystal());
        }

        public static void DeepCopyIn(
            this IPexObjectStructInfo lhs,
            IPexObjectStructInfoGetter rhs)
        {
            ((PexObjectStructInfoSetterTranslationCommon)((IPexObjectStructInfoGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: default,
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IPexObjectStructInfo lhs,
            IPexObjectStructInfoGetter rhs,
            PexObjectStructInfo.TranslationMask? copyMask = null)
        {
            ((PexObjectStructInfoSetterTranslationCommon)((IPexObjectStructInfoGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IPexObjectStructInfo lhs,
            IPexObjectStructInfoGetter rhs,
            out PexObjectStructInfo.ErrorMask errorMask,
            PexObjectStructInfo.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ((PexObjectStructInfoSetterTranslationCommon)((IPexObjectStructInfoGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
            errorMask = PexObjectStructInfo.ErrorMask.Factory(errorMaskBuilder);
        }

        public static void DeepCopyIn(
            this IPexObjectStructInfo lhs,
            IPexObjectStructInfoGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
            ((PexObjectStructInfoSetterTranslationCommon)((IPexObjectStructInfoGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: false);
        }

        public static PexObjectStructInfo DeepCopy(
            this IPexObjectStructInfoGetter item,
            PexObjectStructInfo.TranslationMask? copyMask = null)
        {
            return ((PexObjectStructInfoSetterTranslationCommon)((IPexObjectStructInfoGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask);
        }

        public static PexObjectStructInfo DeepCopy(
            this IPexObjectStructInfoGetter item,
            out PexObjectStructInfo.ErrorMask errorMask,
            PexObjectStructInfo.TranslationMask? copyMask = null)
        {
            return ((PexObjectStructInfoSetterTranslationCommon)((IPexObjectStructInfoGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: out errorMask);
        }

        public static PexObjectStructInfo DeepCopy(
            this IPexObjectStructInfoGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            return ((PexObjectStructInfoSetterTranslationCommon)((IPexObjectStructInfoGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: errorMask);
        }

    }
    #endregion

}

namespace Mutagen.Bethesda.Pex
{
    #region Field Index
    internal enum PexObjectStructInfo_FieldIndex
    {
        Name = 0,
        Members = 1,
    }
    #endregion

    #region Registration
    internal partial class PexObjectStructInfo_Registration : ILoquiRegistration
    {
        public static readonly PexObjectStructInfo_Registration Instance = new PexObjectStructInfo_Registration();

        public static ProtocolKey ProtocolKey => ProtocolDefinition_Pex.ProtocolKey;

        public const ushort AdditionalFieldCount = 2;

        public const ushort FieldCount = 2;

        public static readonly Type MaskType = typeof(PexObjectStructInfo.Mask<>);

        public static readonly Type ErrorMaskType = typeof(PexObjectStructInfo.ErrorMask);

        public static readonly Type ClassType = typeof(PexObjectStructInfo);

        public static readonly Type GetterType = typeof(IPexObjectStructInfoGetter);

        public static readonly Type? InternalGetterType = null;

        public static readonly Type SetterType = typeof(IPexObjectStructInfo);

        public static readonly Type? InternalSetterType = null;

        public const string FullName = "Mutagen.Bethesda.Pex.PexObjectStructInfo";

        public const string Name = "PexObjectStructInfo";

        public const string Namespace = "Mutagen.Bethesda.Pex";

        public const byte GenericCount = 0;

        public static readonly Type? GenericRegistrationType = null;

        #region Interface
        ProtocolKey ILoquiRegistration.ProtocolKey => ProtocolKey;
        ushort ILoquiRegistration.FieldCount => FieldCount;
        ushort ILoquiRegistration.AdditionalFieldCount => AdditionalFieldCount;
        Type ILoquiRegistration.MaskType => MaskType;
        Type ILoquiRegistration.ErrorMaskType => ErrorMaskType;
        Type ILoquiRegistration.ClassType => ClassType;
        Type ILoquiRegistration.SetterType => SetterType;
        Type? ILoquiRegistration.InternalSetterType => InternalSetterType;
        Type ILoquiRegistration.GetterType => GetterType;
        Type? ILoquiRegistration.InternalGetterType => InternalGetterType;
        string ILoquiRegistration.FullName => FullName;
        string ILoquiRegistration.Name => Name;
        string ILoquiRegistration.Namespace => Namespace;
        byte ILoquiRegistration.GenericCount => GenericCount;
        Type? ILoquiRegistration.GenericRegistrationType => GenericRegistrationType;
        ushort? ILoquiRegistration.GetNameIndex(StringCaseAgnostic name) => throw new NotImplementedException();
        bool ILoquiRegistration.GetNthIsEnumerable(ushort index) => throw new NotImplementedException();
        bool ILoquiRegistration.GetNthIsLoqui(ushort index) => throw new NotImplementedException();
        bool ILoquiRegistration.GetNthIsSingleton(ushort index) => throw new NotImplementedException();
        string ILoquiRegistration.GetNthName(ushort index) => throw new NotImplementedException();
        bool ILoquiRegistration.IsNthDerivative(ushort index) => throw new NotImplementedException();
        bool ILoquiRegistration.IsProtected(ushort index) => throw new NotImplementedException();
        Type ILoquiRegistration.GetNthType(ushort index) => throw new NotImplementedException();
        #endregion

    }
    #endregion

    #region Common
    internal partial class PexObjectStructInfoSetterCommon
    {
        public static readonly PexObjectStructInfoSetterCommon Instance = new PexObjectStructInfoSetterCommon();

        partial void ClearPartial();
        
        public void Clear(IPexObjectStructInfo item)
        {
            ClearPartial();
            item.Name = default;
            item.Members.Clear();
        }
        
    }
    internal partial class PexObjectStructInfoCommon
    {
        public static readonly PexObjectStructInfoCommon Instance = new PexObjectStructInfoCommon();

        public PexObjectStructInfo.Mask<bool> GetEqualsMask(
            IPexObjectStructInfoGetter item,
            IPexObjectStructInfoGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            var ret = new PexObjectStructInfo.Mask<bool>(false);
            ((PexObjectStructInfoCommon)((IPexObjectStructInfoGetter)item).CommonInstance()!).FillEqualsMask(
                item: item,
                rhs: rhs,
                ret: ret,
                include: include);
            return ret;
        }
        
        public void FillEqualsMask(
            IPexObjectStructInfoGetter item,
            IPexObjectStructInfoGetter rhs,
            PexObjectStructInfo.Mask<bool> ret,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            ret.Name = string.Equals(item.Name, rhs.Name);
            ret.Members = item.Members.CollectionEqualsHelper(
                rhs.Members,
                (loqLhs, loqRhs) => loqLhs.GetEqualsMask(loqRhs, include),
                include);
        }
        
        public string Print(
            IPexObjectStructInfoGetter item,
            string? name = null,
            PexObjectStructInfo.Mask<bool>? printMask = null)
        {
            var sb = new StructuredStringBuilder();
            Print(
                item: item,
                sb: sb,
                name: name,
                printMask: printMask);
            return sb.ToString();
        }
        
        public void Print(
            IPexObjectStructInfoGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            PexObjectStructInfo.Mask<bool>? printMask = null)
        {
            if (name == null)
            {
                sb.AppendLine($"PexObjectStructInfo =>");
            }
            else
            {
                sb.AppendLine($"{name} (PexObjectStructInfo) =>");
            }
            using (sb.Brace())
            {
                ToStringFields(
                    item: item,
                    sb: sb,
                    printMask: printMask);
            }
        }
        
        protected static void ToStringFields(
            IPexObjectStructInfoGetter item,
            StructuredStringBuilder sb,
            PexObjectStructInfo.Mask<bool>? printMask = null)
        {
            if ((printMask?.Name ?? true)
                && item.Name is {} NameItem)
            {
                sb.AppendItem(NameItem, "Name");
            }
            if (printMask?.Members?.Overall ?? true)
            {
                sb.AppendLine("Members =>");
                using (sb.Brace())
                {
                    foreach (var subItem in item.Members)
                    {
                        using (sb.Brace())
                        {
                            subItem?.Print(sb, "Item");
                        }
                    }
                }
            }
        }
        
        #region Equals and Hash
        public virtual bool Equals(
            IPexObjectStructInfoGetter? lhs,
            IPexObjectStructInfoGetter? rhs,
            TranslationCrystal? equalsMask)
        {
            if (!EqualsMaskHelper.RefEquality(lhs, rhs, out var isEqual)) return isEqual;
            if ((equalsMask?.GetShouldTranslate((int)PexObjectStructInfo_FieldIndex.Name) ?? true))
            {
                if (!string.Equals(lhs.Name, rhs.Name)) return false;
            }
            if ((equalsMask?.GetShouldTranslate((int)PexObjectStructInfo_FieldIndex.Members) ?? true))
            {
                if (!lhs.Members.SequenceEqual(rhs.Members, (l, r) => ((PexObjectStructInfoMemberCommon)((IPexObjectStructInfoMemberGetter)l).CommonInstance()!).Equals(l, r, equalsMask?.GetSubCrystal((int)PexObjectStructInfo_FieldIndex.Members)))) return false;
            }
            return true;
        }
        
        public virtual int GetHashCode(IPexObjectStructInfoGetter item)
        {
            var hash = new HashCode();
            if (item.Name is {} Nameitem)
            {
                hash.Add(Nameitem);
            }
            hash.Add(item.Members);
            return hash.ToHashCode();
        }
        
        #endregion
        
        
        public object GetNew()
        {
            return PexObjectStructInfo.GetNew();
        }
        
    }
    internal partial class PexObjectStructInfoSetterTranslationCommon
    {
        public static readonly PexObjectStructInfoSetterTranslationCommon Instance = new PexObjectStructInfoSetterTranslationCommon();

        #region DeepCopyIn
        public void DeepCopyIn(
            IPexObjectStructInfo item,
            IPexObjectStructInfoGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy)
        {
            if ((copyMask?.GetShouldTranslate((int)PexObjectStructInfo_FieldIndex.Name) ?? true))
            {
                item.Name = rhs.Name;
            }
            if ((copyMask?.GetShouldTranslate((int)PexObjectStructInfo_FieldIndex.Members) ?? true))
            {
                errorMask?.PushIndex((int)PexObjectStructInfo_FieldIndex.Members);
                try
                {
                    item.Members.SetTo(
                        rhs.Members
                        .Select(r =>
                        {
                            return r.DeepCopy(
                                errorMask: errorMask,
                                default(TranslationCrystal));
                        }));
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
                finally
                {
                    errorMask?.PopIndex();
                }
            }
            DeepCopyInCustom(
                item: item,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: deepCopy);
        }
        
        partial void DeepCopyInCustom(
            IPexObjectStructInfo item,
            IPexObjectStructInfoGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy);
        #endregion
        
        public PexObjectStructInfo DeepCopy(
            IPexObjectStructInfoGetter item,
            PexObjectStructInfo.TranslationMask? copyMask = null)
        {
            PexObjectStructInfo ret = (PexObjectStructInfo)((PexObjectStructInfoCommon)((IPexObjectStructInfoGetter)item).CommonInstance()!).GetNew();
            ((PexObjectStructInfoSetterTranslationCommon)((IPexObjectStructInfoGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: null,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            return ret;
        }
        
        public PexObjectStructInfo DeepCopy(
            IPexObjectStructInfoGetter item,
            out PexObjectStructInfo.ErrorMask errorMask,
            PexObjectStructInfo.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            PexObjectStructInfo ret = (PexObjectStructInfo)((PexObjectStructInfoCommon)((IPexObjectStructInfoGetter)item).CommonInstance()!).GetNew();
            ((PexObjectStructInfoSetterTranslationCommon)((IPexObjectStructInfoGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                ret,
                item,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            errorMask = PexObjectStructInfo.ErrorMask.Factory(errorMaskBuilder);
            return ret;
        }
        
        public PexObjectStructInfo DeepCopy(
            IPexObjectStructInfoGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            PexObjectStructInfo ret = (PexObjectStructInfo)((PexObjectStructInfoCommon)((IPexObjectStructInfoGetter)item).CommonInstance()!).GetNew();
            ((PexObjectStructInfoSetterTranslationCommon)((IPexObjectStructInfoGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: true);
            return ret;
        }
        
    }
    #endregion

}

namespace Mutagen.Bethesda.Pex
{
    public partial class PexObjectStructInfo
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => PexObjectStructInfo_Registration.Instance;
        public static ILoquiRegistration StaticRegistration => PexObjectStructInfo_Registration.Instance;
        [DebuggerStepThrough]
        protected object CommonInstance() => PexObjectStructInfoCommon.Instance;
        [DebuggerStepThrough]
        protected object CommonSetterInstance()
        {
            return PexObjectStructInfoSetterCommon.Instance;
        }
        [DebuggerStepThrough]
        protected object CommonSetterTranslationInstance() => PexObjectStructInfoSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IPexObjectStructInfoGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object IPexObjectStructInfoGetter.CommonSetterInstance() => this.CommonSetterInstance();
        [DebuggerStepThrough]
        object IPexObjectStructInfoGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

    }
}

