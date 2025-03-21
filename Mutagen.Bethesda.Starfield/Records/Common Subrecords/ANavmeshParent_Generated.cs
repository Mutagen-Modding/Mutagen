/*
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * Autogenerated by Loqui.  Do not manually change.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*/
#region Usings
using Loqui;
using Loqui.Interfaces;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using RecordTypeInts = Mutagen.Bethesda.Starfield.Internals.RecordTypeInts;
using RecordTypes = Mutagen.Bethesda.Starfield.Internals.RecordTypes;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
#endregion

#nullable enable
namespace Mutagen.Bethesda.Starfield
{
    #region Class
    /// <summary>
    /// Implemented by: [WorldspaceNavmeshParent, CellNavmeshParent]
    /// </summary>
    public abstract partial class ANavmeshParent :
        IANavmeshParent,
        IEquatable<IANavmeshParentGetter>,
        ILoquiObjectSetter<ANavmeshParent>
    {
        #region Ctor
        public ANavmeshParent()
        {
            CustomCtor();
        }
        partial void CustomCtor();
        #endregion


        #region To String

        public virtual void Print(
            StructuredStringBuilder sb,
            string? name = null)
        {
            ANavmeshParentMixIn.Print(
                item: this,
                sb: sb,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IANavmeshParentGetter rhs) return false;
            return ((ANavmeshParentCommon)((IANavmeshParentGetter)this).CommonInstance()!).Equals(this, rhs, equalsMask: null);
        }

        public bool Equals(IANavmeshParentGetter? obj)
        {
            return ((ANavmeshParentCommon)((IANavmeshParentGetter)this).CommonInstance()!).Equals(this, obj, equalsMask: null);
        }

        public override int GetHashCode() => ((ANavmeshParentCommon)((IANavmeshParentGetter)this).CommonInstance()!).GetHashCode(this);

        #endregion

        #region Mask
        public class Mask<TItem> :
            IEquatable<Mask<TItem>>,
            IMask<TItem>
        {
            #region Ctors
            public Mask(TItem initialValue)
            {
            }


            #pragma warning disable CS8618
            protected Mask()
            {
            }
            #pragma warning restore CS8618

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
                return true;
            }
            public override int GetHashCode()
            {
                var hash = new HashCode();
                return hash.ToHashCode();
            }

            #endregion

            #region All
            public virtual bool All(Func<TItem, bool> eval)
            {
                return true;
            }
            #endregion

            #region Any
            public virtual bool Any(Func<TItem, bool> eval)
            {
                return false;
            }
            #endregion

            #region Translate
            public Mask<R> Translate<R>(Func<TItem, R> eval)
            {
                var ret = new ANavmeshParent.Mask<R>();
                this.Translate_InternalFill(ret, eval);
                return ret;
            }

            protected void Translate_InternalFill<R>(Mask<R> obj, Func<TItem, R> eval)
            {
            }
            #endregion

            #region To String
            public override string ToString() => this.Print();

            public string Print(ANavmeshParent.Mask<bool>? printMask = null)
            {
                var sb = new StructuredStringBuilder();
                Print(sb, printMask);
                return sb.ToString();
            }

            public void Print(StructuredStringBuilder sb, ANavmeshParent.Mask<bool>? printMask = null)
            {
                sb.AppendLine($"{nameof(ANavmeshParent.Mask<TItem>)} =>");
                using (sb.Brace())
                {
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
            #endregion

            #region IErrorMask
            public virtual object? GetNthMask(int index)
            {
                ANavmeshParent_FieldIndex enu = (ANavmeshParent_FieldIndex)index;
                switch (enu)
                {
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public virtual void SetNthException(int index, Exception ex)
            {
                ANavmeshParent_FieldIndex enu = (ANavmeshParent_FieldIndex)index;
                switch (enu)
                {
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public virtual void SetNthMask(int index, object obj)
            {
                ANavmeshParent_FieldIndex enu = (ANavmeshParent_FieldIndex)index;
                switch (enu)
                {
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public virtual bool IsInError()
            {
                if (Overall != null) return true;
                return false;
            }
            #endregion

            #region To String
            public override string ToString() => this.Print();

            public virtual void Print(StructuredStringBuilder sb, string? name = null)
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
            protected virtual void PrintFillInternal(StructuredStringBuilder sb)
            {
            }
            #endregion

            #region Combine
            public ErrorMask Combine(ErrorMask? rhs)
            {
                if (rhs == null) return this;
                var ret = new ErrorMask();
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
            #endregion

            #region Ctors
            public TranslationMask(
                bool defaultOn,
                bool onOverall = true)
            {
                this.DefaultOn = defaultOn;
                this.OnOverall = onOverall;
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

            protected virtual void GetCrystal(List<(bool On, TranslationCrystal? SubCrystal)> ret)
            {
            }

            public static implicit operator TranslationMask(bool defaultOn)
            {
                return new TranslationMask(defaultOn: defaultOn, onOverall: defaultOn);
            }

        }
        #endregion

        #region Mutagen
        public virtual IEnumerable<IFormLinkGetter> EnumerateFormLinks() => ANavmeshParentCommon.Instance.EnumerateFormLinks(this);
        public virtual void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping) => ANavmeshParentSetterCommon.Instance.RemapLinks(this, mapping);
        #endregion

        #region Binary Translation
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual object BinaryWriteTranslator => ANavmeshParentBinaryWriteTranslation.Instance;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IBinaryItem.BinaryWriteTranslator => this.BinaryWriteTranslator;
        void IBinaryItem.WriteToBinary(
            MutagenWriter writer,
            TypedWriteParams translationParams = default)
        {
            ((ANavmeshParentBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
                item: this,
                writer: writer,
                translationParams: translationParams);
        }
        #endregion

        void IPrintable.Print(StructuredStringBuilder sb, string? name) => this.Print(sb, name);

        void IClearable.Clear()
        {
            ((ANavmeshParentSetterCommon)((IANavmeshParentGetter)this).CommonSetterInstance()!).Clear(this);
        }

        internal static ANavmeshParent GetNew()
        {
            throw new ArgumentException("New called on an abstract class.");
        }

    }
    #endregion

    #region Interface
    /// <summary>
    /// Implemented by: [WorldspaceNavmeshParent, CellNavmeshParent]
    /// </summary>
    public partial interface IANavmeshParent :
        IANavmeshParentGetter,
        IFormLinkContainer,
        ILoquiObjectSetter<IANavmeshParent>
    {
    }

    /// <summary>
    /// Implemented by: [WorldspaceNavmeshParent, CellNavmeshParent]
    /// </summary>
    public partial interface IANavmeshParentGetter :
        ILoquiObject,
        IBinaryItem,
        IFormLinkContainerGetter,
        ILoquiObject<IANavmeshParentGetter>
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object? CommonSetterInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonSetterTranslationInstance();
        static ILoquiRegistration StaticRegistration => ANavmeshParent_Registration.Instance;

    }

    #endregion

    #region Common MixIn
    public static partial class ANavmeshParentMixIn
    {
        public static void Clear(this IANavmeshParent item)
        {
            ((ANavmeshParentSetterCommon)((IANavmeshParentGetter)item).CommonSetterInstance()!).Clear(item: item);
        }

        public static ANavmeshParent.Mask<bool> GetEqualsMask(
            this IANavmeshParentGetter item,
            IANavmeshParentGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            return ((ANavmeshParentCommon)((IANavmeshParentGetter)item).CommonInstance()!).GetEqualsMask(
                item: item,
                rhs: rhs,
                include: include);
        }

        public static string Print(
            this IANavmeshParentGetter item,
            string? name = null,
            ANavmeshParent.Mask<bool>? printMask = null)
        {
            return ((ANavmeshParentCommon)((IANavmeshParentGetter)item).CommonInstance()!).Print(
                item: item,
                name: name,
                printMask: printMask);
        }

        public static void Print(
            this IANavmeshParentGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            ANavmeshParent.Mask<bool>? printMask = null)
        {
            ((ANavmeshParentCommon)((IANavmeshParentGetter)item).CommonInstance()!).Print(
                item: item,
                sb: sb,
                name: name,
                printMask: printMask);
        }

        public static bool Equals(
            this IANavmeshParentGetter item,
            IANavmeshParentGetter rhs,
            ANavmeshParent.TranslationMask? equalsMask = null)
        {
            return ((ANavmeshParentCommon)((IANavmeshParentGetter)item).CommonInstance()!).Equals(
                lhs: item,
                rhs: rhs,
                equalsMask: equalsMask?.GetCrystal());
        }

        public static void DeepCopyIn(
            this IANavmeshParent lhs,
            IANavmeshParentGetter rhs)
        {
            ((ANavmeshParentSetterTranslationCommon)((IANavmeshParentGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: default,
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IANavmeshParent lhs,
            IANavmeshParentGetter rhs,
            ANavmeshParent.TranslationMask? copyMask = null)
        {
            ((ANavmeshParentSetterTranslationCommon)((IANavmeshParentGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IANavmeshParent lhs,
            IANavmeshParentGetter rhs,
            out ANavmeshParent.ErrorMask errorMask,
            ANavmeshParent.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ((ANavmeshParentSetterTranslationCommon)((IANavmeshParentGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
            errorMask = ANavmeshParent.ErrorMask.Factory(errorMaskBuilder);
        }

        public static void DeepCopyIn(
            this IANavmeshParent lhs,
            IANavmeshParentGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
            ((ANavmeshParentSetterTranslationCommon)((IANavmeshParentGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: false);
        }

        public static ANavmeshParent DeepCopy(
            this IANavmeshParentGetter item,
            ANavmeshParent.TranslationMask? copyMask = null)
        {
            return ((ANavmeshParentSetterTranslationCommon)((IANavmeshParentGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask);
        }

        public static ANavmeshParent DeepCopy(
            this IANavmeshParentGetter item,
            out ANavmeshParent.ErrorMask errorMask,
            ANavmeshParent.TranslationMask? copyMask = null)
        {
            return ((ANavmeshParentSetterTranslationCommon)((IANavmeshParentGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: out errorMask);
        }

        public static ANavmeshParent DeepCopy(
            this IANavmeshParentGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            return ((ANavmeshParentSetterTranslationCommon)((IANavmeshParentGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: errorMask);
        }

        #region Binary Translation
        public static void CopyInFromBinary(
            this IANavmeshParent item,
            MutagenFrame frame,
            TypedParseParams translationParams = default)
        {
            ((ANavmeshParentSetterCommon)((IANavmeshParentGetter)item).CommonSetterInstance()!).CopyInFromBinary(
                item: item,
                frame: frame,
                translationParams: translationParams);
        }

        #endregion

    }
    #endregion

}

namespace Mutagen.Bethesda.Starfield
{
    #region Field Index
    internal enum ANavmeshParent_FieldIndex
    {
    }
    #endregion

    #region Registration
    internal partial class ANavmeshParent_Registration : ILoquiRegistration
    {
        public static readonly ANavmeshParent_Registration Instance = new ANavmeshParent_Registration();

        public static ProtocolKey ProtocolKey => ProtocolDefinition_Starfield.ProtocolKey;

        public const ushort AdditionalFieldCount = 0;

        public const ushort FieldCount = 0;

        public static readonly Type MaskType = typeof(ANavmeshParent.Mask<>);

        public static readonly Type ErrorMaskType = typeof(ANavmeshParent.ErrorMask);

        public static readonly Type ClassType = typeof(ANavmeshParent);

        public static readonly Type GetterType = typeof(IANavmeshParentGetter);

        public static readonly Type? InternalGetterType = null;

        public static readonly Type SetterType = typeof(IANavmeshParent);

        public static readonly Type? InternalSetterType = null;

        public const string FullName = "Mutagen.Bethesda.Starfield.ANavmeshParent";

        public const string Name = "ANavmeshParent";

        public const string Namespace = "Mutagen.Bethesda.Starfield";

        public const byte GenericCount = 0;

        public static readonly Type? GenericRegistrationType = null;

        public static readonly Type BinaryWriteTranslation = typeof(ANavmeshParentBinaryWriteTranslation);
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
    internal partial class ANavmeshParentSetterCommon
    {
        public static readonly ANavmeshParentSetterCommon Instance = new ANavmeshParentSetterCommon();

        partial void ClearPartial();
        
        public virtual void Clear(IANavmeshParent item)
        {
            ClearPartial();
        }
        
        #region Mutagen
        public void RemapLinks(IANavmeshParent obj, IReadOnlyDictionary<FormKey, FormKey> mapping)
        {
        }
        
        #endregion
        
        #region Binary Translation
        public virtual void CopyInFromBinary(
            IANavmeshParent item,
            MutagenFrame frame,
            TypedParseParams translationParams)
        {
        }
        
        #endregion
        
    }
    internal partial class ANavmeshParentCommon
    {
        public static readonly ANavmeshParentCommon Instance = new ANavmeshParentCommon();

        public ANavmeshParent.Mask<bool> GetEqualsMask(
            IANavmeshParentGetter item,
            IANavmeshParentGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            var ret = new ANavmeshParent.Mask<bool>(false);
            ((ANavmeshParentCommon)((IANavmeshParentGetter)item).CommonInstance()!).FillEqualsMask(
                item: item,
                rhs: rhs,
                ret: ret,
                include: include);
            return ret;
        }
        
        public void FillEqualsMask(
            IANavmeshParentGetter item,
            IANavmeshParentGetter rhs,
            ANavmeshParent.Mask<bool> ret,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
        }
        
        public string Print(
            IANavmeshParentGetter item,
            string? name = null,
            ANavmeshParent.Mask<bool>? printMask = null)
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
            IANavmeshParentGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            ANavmeshParent.Mask<bool>? printMask = null)
        {
            if (name == null)
            {
                sb.AppendLine($"ANavmeshParent =>");
            }
            else
            {
                sb.AppendLine($"{name} (ANavmeshParent) =>");
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
            IANavmeshParentGetter item,
            StructuredStringBuilder sb,
            ANavmeshParent.Mask<bool>? printMask = null)
        {
        }
        
        #region Equals and Hash
        public virtual bool Equals(
            IANavmeshParentGetter? lhs,
            IANavmeshParentGetter? rhs,
            TranslationCrystal? equalsMask)
        {
            if (!EqualsMaskHelper.RefEquality(lhs, rhs, out var isEqual)) return isEqual;
            return true;
        }
        
        public virtual int GetHashCode(IANavmeshParentGetter item)
        {
            var hash = new HashCode();
            return hash.ToHashCode();
        }
        
        #endregion
        
        
        public virtual object GetNew()
        {
            return ANavmeshParent.GetNew();
        }
        
        #region Mutagen
        public IEnumerable<IFormLinkGetter> EnumerateFormLinks(IANavmeshParentGetter obj)
        {
            yield break;
        }
        
        #endregion
        
    }
    internal partial class ANavmeshParentSetterTranslationCommon
    {
        public static readonly ANavmeshParentSetterTranslationCommon Instance = new ANavmeshParentSetterTranslationCommon();

        #region DeepCopyIn
        public virtual void DeepCopyIn(
            IANavmeshParent item,
            IANavmeshParentGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy)
        {
            DeepCopyInCustom(
                item: item,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: deepCopy);
        }
        
        partial void DeepCopyInCustom(
            IANavmeshParent item,
            IANavmeshParentGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy);
        #endregion
        
        public ANavmeshParent DeepCopy(
            IANavmeshParentGetter item,
            ANavmeshParent.TranslationMask? copyMask = null)
        {
            ANavmeshParent ret = (ANavmeshParent)((ANavmeshParentCommon)((IANavmeshParentGetter)item).CommonInstance()!).GetNew();
            ((ANavmeshParentSetterTranslationCommon)((IANavmeshParentGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: null,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            return ret;
        }
        
        public ANavmeshParent DeepCopy(
            IANavmeshParentGetter item,
            out ANavmeshParent.ErrorMask errorMask,
            ANavmeshParent.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ANavmeshParent ret = (ANavmeshParent)((ANavmeshParentCommon)((IANavmeshParentGetter)item).CommonInstance()!).GetNew();
            ((ANavmeshParentSetterTranslationCommon)((IANavmeshParentGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                ret,
                item,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            errorMask = ANavmeshParent.ErrorMask.Factory(errorMaskBuilder);
            return ret;
        }
        
        public ANavmeshParent DeepCopy(
            IANavmeshParentGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            ANavmeshParent ret = (ANavmeshParent)((ANavmeshParentCommon)((IANavmeshParentGetter)item).CommonInstance()!).GetNew();
            ((ANavmeshParentSetterTranslationCommon)((IANavmeshParentGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
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

namespace Mutagen.Bethesda.Starfield
{
    public partial class ANavmeshParent
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => ANavmeshParent_Registration.Instance;
        public static ILoquiRegistration StaticRegistration => ANavmeshParent_Registration.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonInstance() => ANavmeshParentCommon.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonSetterInstance()
        {
            return ANavmeshParentSetterCommon.Instance;
        }
        [DebuggerStepThrough]
        protected virtual object CommonSetterTranslationInstance() => ANavmeshParentSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IANavmeshParentGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object IANavmeshParentGetter.CommonSetterInstance() => this.CommonSetterInstance();
        [DebuggerStepThrough]
        object IANavmeshParentGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

    }
}

#region Modules
#region Binary Translation
namespace Mutagen.Bethesda.Starfield
{
    public partial class ANavmeshParentBinaryWriteTranslation : IBinaryWriteTranslator
    {
        public static readonly ANavmeshParentBinaryWriteTranslation Instance = new();

        public virtual void Write(
            MutagenWriter writer,
            IANavmeshParentGetter item,
            TypedWriteParams translationParams)
        {
        }

        public virtual void Write(
            MutagenWriter writer,
            object item,
            TypedWriteParams translationParams = default)
        {
            Write(
                item: (IANavmeshParentGetter)item,
                writer: writer,
                translationParams: translationParams);
        }

    }

    internal partial class ANavmeshParentBinaryCreateTranslation
    {
        public static readonly ANavmeshParentBinaryCreateTranslation Instance = new ANavmeshParentBinaryCreateTranslation();

    }

}
namespace Mutagen.Bethesda.Starfield
{
    #region Binary Write Mixins
    public static class ANavmeshParentBinaryTranslationMixIn
    {
        public static void WriteToBinary(
            this IANavmeshParentGetter item,
            MutagenWriter writer,
            TypedWriteParams translationParams = default)
        {
            ((ANavmeshParentBinaryWriteTranslation)item.BinaryWriteTranslator).Write(
                item: item,
                writer: writer,
                translationParams: translationParams);
        }

    }
    #endregion


}
namespace Mutagen.Bethesda.Starfield
{
    internal abstract partial class ANavmeshParentBinaryOverlay :
        PluginBinaryOverlay,
        IANavmeshParentGetter
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => ANavmeshParent_Registration.Instance;
        public static ILoquiRegistration StaticRegistration => ANavmeshParent_Registration.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonInstance() => ANavmeshParentCommon.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonSetterTranslationInstance() => ANavmeshParentSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IANavmeshParentGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object? IANavmeshParentGetter.CommonSetterInstance() => null;
        [DebuggerStepThrough]
        object IANavmeshParentGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

        void IPrintable.Print(StructuredStringBuilder sb, string? name) => this.Print(sb, name);

        public virtual IEnumerable<IFormLinkGetter> EnumerateFormLinks() => ANavmeshParentCommon.Instance.EnumerateFormLinks(this);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual object BinaryWriteTranslator => ANavmeshParentBinaryWriteTranslation.Instance;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IBinaryItem.BinaryWriteTranslator => this.BinaryWriteTranslator;
        void IBinaryItem.WriteToBinary(
            MutagenWriter writer,
            TypedWriteParams translationParams = default)
        {
            ((ANavmeshParentBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
                item: this,
                writer: writer,
                translationParams: translationParams);
        }

        partial void CustomFactoryEnd(
            OverlayStream stream,
            int finalPos,
            int offset);

        partial void CustomCtor();
        protected ANavmeshParentBinaryOverlay(
            MemoryPair memoryPair,
            BinaryOverlayFactoryPackage package)
            : base(
                memoryPair: memoryPair,
                package: package)
        {
            this.CustomCtor();
        }


        #region To String

        public virtual void Print(
            StructuredStringBuilder sb,
            string? name = null)
        {
            ANavmeshParentMixIn.Print(
                item: this,
                sb: sb,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IANavmeshParentGetter rhs) return false;
            return ((ANavmeshParentCommon)((IANavmeshParentGetter)this).CommonInstance()!).Equals(this, rhs, equalsMask: null);
        }

        public bool Equals(IANavmeshParentGetter? obj)
        {
            return ((ANavmeshParentCommon)((IANavmeshParentGetter)this).CommonInstance()!).Equals(this, obj, equalsMask: null);
        }

        public override int GetHashCode() => ((ANavmeshParentCommon)((IANavmeshParentGetter)this).CommonInstance()!).GetHashCode(this);

        #endregion

    }

}
#endregion

#endregion

