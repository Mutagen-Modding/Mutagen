/*
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * Autogenerated by Loqui.  Do not manually change.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*/
#region Usings
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Pex.Internals;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
#endregion

#nullable enable
namespace Mutagen.Bethesda.Pex
{
    #region Class
    public partial class UserFlag :
        IEquatable<IUserFlagGetter>,
        ILoquiObjectSetter<UserFlag>,
        IUserFlag
    {
        #region Ctor
        public UserFlag()
        {
            CustomCtor();
        }
        partial void CustomCtor();
        #endregion

        #region Name
        public String? Name { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String? IUserFlagGetter.Name => this.Name;
        #endregion
        #region FlagIndex
        public Byte FlagIndex { get; set; } = default;
        #endregion

        #region To String

        public void ToString(
            FileGeneration fg,
            string? name = null)
        {
            UserFlagMixIn.ToString(
                item: this,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IUserFlagGetter rhs) return false;
            return ((UserFlagCommon)((IUserFlagGetter)this).CommonInstance()!).Equals(this, rhs, crystal: null);
        }

        public bool Equals(IUserFlagGetter? obj)
        {
            return ((UserFlagCommon)((IUserFlagGetter)this).CommonInstance()!).Equals(this, obj, crystal: null);
        }

        public override int GetHashCode() => ((UserFlagCommon)((IUserFlagGetter)this).CommonInstance()!).GetHashCode(this);

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
                this.FlagIndex = initialValue;
            }

            public Mask(
                TItem Name,
                TItem FlagIndex)
            {
                this.Name = Name;
                this.FlagIndex = FlagIndex;
            }

            #pragma warning disable CS8618
            protected Mask()
            {
            }
            #pragma warning restore CS8618

            #endregion

            #region Members
            public TItem Name;
            public TItem FlagIndex;
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
                if (!object.Equals(this.FlagIndex, rhs.FlagIndex)) return false;
                return true;
            }
            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(this.Name);
                hash.Add(this.FlagIndex);
                return hash.ToHashCode();
            }

            #endregion

            #region All
            public bool All(Func<TItem, bool> eval)
            {
                if (!eval(this.Name)) return false;
                if (!eval(this.FlagIndex)) return false;
                return true;
            }
            #endregion

            #region Any
            public bool Any(Func<TItem, bool> eval)
            {
                if (eval(this.Name)) return true;
                if (eval(this.FlagIndex)) return true;
                return false;
            }
            #endregion

            #region Translate
            public Mask<R> Translate<R>(Func<TItem, R> eval)
            {
                var ret = new UserFlag.Mask<R>();
                this.Translate_InternalFill(ret, eval);
                return ret;
            }

            protected void Translate_InternalFill<R>(Mask<R> obj, Func<TItem, R> eval)
            {
                obj.Name = eval(this.Name);
                obj.FlagIndex = eval(this.FlagIndex);
            }
            #endregion

            #region To String
            public override string ToString()
            {
                return ToString(printMask: null);
            }

            public string ToString(UserFlag.Mask<bool>? printMask = null)
            {
                var fg = new FileGeneration();
                ToString(fg, printMask);
                return fg.ToString();
            }

            public void ToString(FileGeneration fg, UserFlag.Mask<bool>? printMask = null)
            {
                fg.AppendLine($"{nameof(UserFlag.Mask<TItem>)} =>");
                fg.AppendLine("[");
                using (new DepthWrapper(fg))
                {
                    if (printMask?.Name ?? true)
                    {
                        fg.AppendItem(Name, "Name");
                    }
                    if (printMask?.FlagIndex ?? true)
                    {
                        fg.AppendItem(FlagIndex, "FlagIndex");
                    }
                }
                fg.AppendLine("]");
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
            public Exception? FlagIndex;
            #endregion

            #region IErrorMask
            public object? GetNthMask(int index)
            {
                UserFlag_FieldIndex enu = (UserFlag_FieldIndex)index;
                switch (enu)
                {
                    case UserFlag_FieldIndex.Name:
                        return Name;
                    case UserFlag_FieldIndex.FlagIndex:
                        return FlagIndex;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public void SetNthException(int index, Exception ex)
            {
                UserFlag_FieldIndex enu = (UserFlag_FieldIndex)index;
                switch (enu)
                {
                    case UserFlag_FieldIndex.Name:
                        this.Name = ex;
                        break;
                    case UserFlag_FieldIndex.FlagIndex:
                        this.FlagIndex = ex;
                        break;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public void SetNthMask(int index, object obj)
            {
                UserFlag_FieldIndex enu = (UserFlag_FieldIndex)index;
                switch (enu)
                {
                    case UserFlag_FieldIndex.Name:
                        this.Name = (Exception?)obj;
                        break;
                    case UserFlag_FieldIndex.FlagIndex:
                        this.FlagIndex = (Exception?)obj;
                        break;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public bool IsInError()
            {
                if (Overall != null) return true;
                if (Name != null) return true;
                if (FlagIndex != null) return true;
                return false;
            }
            #endregion

            #region To String
            public override string ToString()
            {
                var fg = new FileGeneration();
                ToString(fg, null);
                return fg.ToString();
            }

            public void ToString(FileGeneration fg, string? name = null)
            {
                fg.AppendLine($"{(name ?? "ErrorMask")} =>");
                fg.AppendLine("[");
                using (new DepthWrapper(fg))
                {
                    if (this.Overall != null)
                    {
                        fg.AppendLine("Overall =>");
                        fg.AppendLine("[");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine($"{this.Overall}");
                        }
                        fg.AppendLine("]");
                    }
                    ToString_FillInternal(fg);
                }
                fg.AppendLine("]");
            }
            protected void ToString_FillInternal(FileGeneration fg)
            {
                fg.AppendItem(Name, "Name");
                fg.AppendItem(FlagIndex, "FlagIndex");
            }
            #endregion

            #region Combine
            public ErrorMask Combine(ErrorMask? rhs)
            {
                if (rhs == null) return this;
                var ret = new ErrorMask();
                ret.Name = this.Name.Combine(rhs.Name);
                ret.FlagIndex = this.FlagIndex.Combine(rhs.FlagIndex);
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
            public bool FlagIndex;
            #endregion

            #region Ctors
            public TranslationMask(
                bool defaultOn,
                bool onOverall = true)
            {
                this.DefaultOn = defaultOn;
                this.OnOverall = onOverall;
                this.Name = defaultOn;
                this.FlagIndex = defaultOn;
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
                ret.Add((FlagIndex, null));
            }

            public static implicit operator TranslationMask(bool defaultOn)
            {
                return new TranslationMask(defaultOn: defaultOn, onOverall: defaultOn);
            }

        }
        #endregion

        void IPrintable.ToString(FileGeneration fg, string? name) => this.ToString(fg, name);

        void IClearable.Clear()
        {
            ((UserFlagSetterCommon)((IUserFlagGetter)this).CommonSetterInstance()!).Clear(this);
        }

        internal static UserFlag GetNew()
        {
            return new UserFlag();
        }

    }
    #endregion

    #region Interface
    public partial interface IUserFlag :
        ILoquiObjectSetter<IUserFlag>,
        IUserFlagGetter
    {
        new String? Name { get; set; }
        new Byte FlagIndex { get; set; }
    }

    public partial interface IUserFlagGetter :
        ILoquiObject,
        ILoquiObject<IUserFlagGetter>
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object? CommonSetterInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonSetterTranslationInstance();
        static ILoquiRegistration Registration => UserFlag_Registration.Instance;
        String? Name { get; }
        Byte FlagIndex { get; }

    }

    #endregion

    #region Common MixIn
    public static partial class UserFlagMixIn
    {
        public static void Clear(this IUserFlag item)
        {
            ((UserFlagSetterCommon)((IUserFlagGetter)item).CommonSetterInstance()!).Clear(item: item);
        }

        public static UserFlag.Mask<bool> GetEqualsMask(
            this IUserFlagGetter item,
            IUserFlagGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            return ((UserFlagCommon)((IUserFlagGetter)item).CommonInstance()!).GetEqualsMask(
                item: item,
                rhs: rhs,
                include: include);
        }

        public static string ToString(
            this IUserFlagGetter item,
            string? name = null,
            UserFlag.Mask<bool>? printMask = null)
        {
            return ((UserFlagCommon)((IUserFlagGetter)item).CommonInstance()!).ToString(
                item: item,
                name: name,
                printMask: printMask);
        }

        public static void ToString(
            this IUserFlagGetter item,
            FileGeneration fg,
            string? name = null,
            UserFlag.Mask<bool>? printMask = null)
        {
            ((UserFlagCommon)((IUserFlagGetter)item).CommonInstance()!).ToString(
                item: item,
                fg: fg,
                name: name,
                printMask: printMask);
        }

        public static bool Equals(
            this IUserFlagGetter item,
            IUserFlagGetter rhs,
            UserFlag.TranslationMask? equalsMask = null)
        {
            return ((UserFlagCommon)((IUserFlagGetter)item).CommonInstance()!).Equals(
                lhs: item,
                rhs: rhs,
                crystal: equalsMask?.GetCrystal());
        }

        public static void DeepCopyIn(
            this IUserFlag lhs,
            IUserFlagGetter rhs)
        {
            ((UserFlagSetterTranslationCommon)((IUserFlagGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: default,
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IUserFlag lhs,
            IUserFlagGetter rhs,
            UserFlag.TranslationMask? copyMask = null)
        {
            ((UserFlagSetterTranslationCommon)((IUserFlagGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IUserFlag lhs,
            IUserFlagGetter rhs,
            out UserFlag.ErrorMask errorMask,
            UserFlag.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ((UserFlagSetterTranslationCommon)((IUserFlagGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
            errorMask = UserFlag.ErrorMask.Factory(errorMaskBuilder);
        }

        public static void DeepCopyIn(
            this IUserFlag lhs,
            IUserFlagGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
            ((UserFlagSetterTranslationCommon)((IUserFlagGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: false);
        }

        public static UserFlag DeepCopy(
            this IUserFlagGetter item,
            UserFlag.TranslationMask? copyMask = null)
        {
            return ((UserFlagSetterTranslationCommon)((IUserFlagGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask);
        }

        public static UserFlag DeepCopy(
            this IUserFlagGetter item,
            out UserFlag.ErrorMask errorMask,
            UserFlag.TranslationMask? copyMask = null)
        {
            return ((UserFlagSetterTranslationCommon)((IUserFlagGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: out errorMask);
        }

        public static UserFlag DeepCopy(
            this IUserFlagGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            return ((UserFlagSetterTranslationCommon)((IUserFlagGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: errorMask);
        }

    }
    #endregion

}

namespace Mutagen.Bethesda.Pex.Internals
{
    #region Field Index
    public enum UserFlag_FieldIndex
    {
        Name = 0,
        FlagIndex = 1,
    }
    #endregion

    #region Registration
    public partial class UserFlag_Registration : ILoquiRegistration
    {
        public static readonly UserFlag_Registration Instance = new UserFlag_Registration();

        public static ProtocolKey ProtocolKey => ProtocolDefinition_Pex.ProtocolKey;

        public static readonly ObjectKey ObjectKey = new ObjectKey(
            protocolKey: ProtocolDefinition_Pex.ProtocolKey,
            msgID: 17,
            version: 0);

        public const string GUID = "0a3dee72-03b5-4908-bfd1-711612505d6c";

        public const ushort AdditionalFieldCount = 2;

        public const ushort FieldCount = 2;

        public static readonly Type MaskType = typeof(UserFlag.Mask<>);

        public static readonly Type ErrorMaskType = typeof(UserFlag.ErrorMask);

        public static readonly Type ClassType = typeof(UserFlag);

        public static readonly Type GetterType = typeof(IUserFlagGetter);

        public static readonly Type? InternalGetterType = null;

        public static readonly Type SetterType = typeof(IUserFlag);

        public static readonly Type? InternalSetterType = null;

        public const string FullName = "Mutagen.Bethesda.Pex.UserFlag";

        public const string Name = "UserFlag";

        public const string Namespace = "Mutagen.Bethesda.Pex";

        public const byte GenericCount = 0;

        public static readonly Type? GenericRegistrationType = null;

        #region Interface
        ProtocolKey ILoquiRegistration.ProtocolKey => ProtocolKey;
        ObjectKey ILoquiRegistration.ObjectKey => ObjectKey;
        string ILoquiRegistration.GUID => GUID;
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
    public partial class UserFlagSetterCommon
    {
        public static readonly UserFlagSetterCommon Instance = new UserFlagSetterCommon();

        partial void ClearPartial();
        
        public void Clear(IUserFlag item)
        {
            ClearPartial();
            item.Name = default;
            item.FlagIndex = default;
        }
        
    }
    public partial class UserFlagCommon
    {
        public static readonly UserFlagCommon Instance = new UserFlagCommon();

        public UserFlag.Mask<bool> GetEqualsMask(
            IUserFlagGetter item,
            IUserFlagGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            var ret = new UserFlag.Mask<bool>(false);
            ((UserFlagCommon)((IUserFlagGetter)item).CommonInstance()!).FillEqualsMask(
                item: item,
                rhs: rhs,
                ret: ret,
                include: include);
            return ret;
        }
        
        public void FillEqualsMask(
            IUserFlagGetter item,
            IUserFlagGetter rhs,
            UserFlag.Mask<bool> ret,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            if (rhs == null) return;
            ret.Name = string.Equals(item.Name, rhs.Name);
            ret.FlagIndex = item.FlagIndex == rhs.FlagIndex;
        }
        
        public string ToString(
            IUserFlagGetter item,
            string? name = null,
            UserFlag.Mask<bool>? printMask = null)
        {
            var fg = new FileGeneration();
            ToString(
                item: item,
                fg: fg,
                name: name,
                printMask: printMask);
            return fg.ToString();
        }
        
        public void ToString(
            IUserFlagGetter item,
            FileGeneration fg,
            string? name = null,
            UserFlag.Mask<bool>? printMask = null)
        {
            if (name == null)
            {
                fg.AppendLine($"UserFlag =>");
            }
            else
            {
                fg.AppendLine($"{name} (UserFlag) =>");
            }
            fg.AppendLine("[");
            using (new DepthWrapper(fg))
            {
                ToStringFields(
                    item: item,
                    fg: fg,
                    printMask: printMask);
            }
            fg.AppendLine("]");
        }
        
        protected static void ToStringFields(
            IUserFlagGetter item,
            FileGeneration fg,
            UserFlag.Mask<bool>? printMask = null)
        {
            if ((printMask?.Name ?? true)
                && item.Name.TryGet(out var NameItem))
            {
                fg.AppendItem(NameItem, "Name");
            }
            if (printMask?.FlagIndex ?? true)
            {
                fg.AppendItem(item.FlagIndex, "FlagIndex");
            }
        }
        
        #region Equals and Hash
        public virtual bool Equals(
            IUserFlagGetter? lhs,
            IUserFlagGetter? rhs,
            TranslationCrystal? crystal)
        {
            if (lhs == null && rhs == null) return false;
            if (lhs == null || rhs == null) return false;
            if ((crystal?.GetShouldTranslate((int)UserFlag_FieldIndex.Name) ?? true))
            {
                if (!string.Equals(lhs.Name, rhs.Name)) return false;
            }
            if ((crystal?.GetShouldTranslate((int)UserFlag_FieldIndex.FlagIndex) ?? true))
            {
                if (lhs.FlagIndex != rhs.FlagIndex) return false;
            }
            return true;
        }
        
        public virtual int GetHashCode(IUserFlagGetter item)
        {
            var hash = new HashCode();
            if (item.Name.TryGet(out var Nameitem))
            {
                hash.Add(Nameitem);
            }
            hash.Add(item.FlagIndex);
            return hash.ToHashCode();
        }
        
        #endregion
        
        
        public object GetNew()
        {
            return UserFlag.GetNew();
        }
        
    }
    public partial class UserFlagSetterTranslationCommon
    {
        public static readonly UserFlagSetterTranslationCommon Instance = new UserFlagSetterTranslationCommon();

        #region DeepCopyIn
        public void DeepCopyIn(
            IUserFlag item,
            IUserFlagGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy)
        {
            if ((copyMask?.GetShouldTranslate((int)UserFlag_FieldIndex.Name) ?? true))
            {
                item.Name = rhs.Name;
            }
            if ((copyMask?.GetShouldTranslate((int)UserFlag_FieldIndex.FlagIndex) ?? true))
            {
                item.FlagIndex = rhs.FlagIndex;
            }
        }
        
        #endregion
        
        public UserFlag DeepCopy(
            IUserFlagGetter item,
            UserFlag.TranslationMask? copyMask = null)
        {
            UserFlag ret = (UserFlag)((UserFlagCommon)((IUserFlagGetter)item).CommonInstance()!).GetNew();
            ((UserFlagSetterTranslationCommon)((IUserFlagGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: null,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            return ret;
        }
        
        public UserFlag DeepCopy(
            IUserFlagGetter item,
            out UserFlag.ErrorMask errorMask,
            UserFlag.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            UserFlag ret = (UserFlag)((UserFlagCommon)((IUserFlagGetter)item).CommonInstance()!).GetNew();
            ((UserFlagSetterTranslationCommon)((IUserFlagGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                ret,
                item,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            errorMask = UserFlag.ErrorMask.Factory(errorMaskBuilder);
            return ret;
        }
        
        public UserFlag DeepCopy(
            IUserFlagGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            UserFlag ret = (UserFlag)((UserFlagCommon)((IUserFlagGetter)item).CommonInstance()!).GetNew();
            ((UserFlagSetterTranslationCommon)((IUserFlagGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
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
    public partial class UserFlag
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => UserFlag_Registration.Instance;
        public static UserFlag_Registration Registration => UserFlag_Registration.Instance;
        [DebuggerStepThrough]
        protected object CommonInstance() => UserFlagCommon.Instance;
        [DebuggerStepThrough]
        protected object CommonSetterInstance()
        {
            return UserFlagSetterCommon.Instance;
        }
        [DebuggerStepThrough]
        protected object CommonSetterTranslationInstance() => UserFlagSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IUserFlagGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object IUserFlagGetter.CommonSetterInstance() => this.CommonSetterInstance();
        [DebuggerStepThrough]
        object IUserFlagGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

    }
}

