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
    /// Implemented by: [NoInstanceNamingRules, ArmorInstanceNamingRules, FloraInstanceNamingRules, FurnitureInstanceNamingRules, WeaponInstanceNamingRules, ActorInstanceNamingRules, ContainerInstanceNamingRules]
    /// </summary>
    public abstract partial class AInstanceNamingRules :
        IAInstanceNamingRules,
        IEquatable<IAInstanceNamingRulesGetter>,
        ILoquiObjectSetter<AInstanceNamingRules>
    {
        #region Ctor
        public AInstanceNamingRules()
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
            AInstanceNamingRulesMixIn.Print(
                item: this,
                sb: sb,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IAInstanceNamingRulesGetter rhs) return false;
            return ((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)this).CommonInstance()!).Equals(this, rhs, equalsMask: null);
        }

        public bool Equals(IAInstanceNamingRulesGetter? obj)
        {
            return ((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)this).CommonInstance()!).Equals(this, obj, equalsMask: null);
        }

        public override int GetHashCode() => ((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)this).CommonInstance()!).GetHashCode(this);

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
                var ret = new AInstanceNamingRules.Mask<R>();
                this.Translate_InternalFill(ret, eval);
                return ret;
            }

            protected void Translate_InternalFill<R>(Mask<R> obj, Func<TItem, R> eval)
            {
            }
            #endregion

            #region To String
            public override string ToString() => this.Print();

            public string Print(AInstanceNamingRules.Mask<bool>? printMask = null)
            {
                var sb = new StructuredStringBuilder();
                Print(sb, printMask);
                return sb.ToString();
            }

            public void Print(StructuredStringBuilder sb, AInstanceNamingRules.Mask<bool>? printMask = null)
            {
                sb.AppendLine($"{nameof(AInstanceNamingRules.Mask<TItem>)} =>");
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
                AInstanceNamingRules_FieldIndex enu = (AInstanceNamingRules_FieldIndex)index;
                switch (enu)
                {
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public virtual void SetNthException(int index, Exception ex)
            {
                AInstanceNamingRules_FieldIndex enu = (AInstanceNamingRules_FieldIndex)index;
                switch (enu)
                {
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public virtual void SetNthMask(int index, object obj)
            {
                AInstanceNamingRules_FieldIndex enu = (AInstanceNamingRules_FieldIndex)index;
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
        public virtual IEnumerable<IFormLinkGetter> EnumerateFormLinks() => AInstanceNamingRulesCommon.Instance.EnumerateFormLinks(this);
        public virtual void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping) => AInstanceNamingRulesSetterCommon.Instance.RemapLinks(this, mapping);
        #endregion

        #region Binary Translation
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual object BinaryWriteTranslator => AInstanceNamingRulesBinaryWriteTranslation.Instance;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IBinaryItem.BinaryWriteTranslator => this.BinaryWriteTranslator;
        void IBinaryItem.WriteToBinary(
            MutagenWriter writer,
            TypedWriteParams translationParams = default)
        {
            ((AInstanceNamingRulesBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
                item: this,
                writer: writer,
                translationParams: translationParams);
        }
        #endregion

        void IPrintable.Print(StructuredStringBuilder sb, string? name) => this.Print(sb, name);

        void IClearable.Clear()
        {
            ((AInstanceNamingRulesSetterCommon)((IAInstanceNamingRulesGetter)this).CommonSetterInstance()!).Clear(this);
        }

        internal static AInstanceNamingRules GetNew()
        {
            throw new ArgumentException("New called on an abstract class.");
        }

    }
    #endregion

    #region Interface
    /// <summary>
    /// Implemented by: [NoInstanceNamingRules, ArmorInstanceNamingRules, FloraInstanceNamingRules, FurnitureInstanceNamingRules, WeaponInstanceNamingRules, ActorInstanceNamingRules, ContainerInstanceNamingRules]
    /// </summary>
    public partial interface IAInstanceNamingRules :
        IAInstanceNamingRulesGetter,
        IFormLinkContainer,
        ILoquiObjectSetter<IAInstanceNamingRules>
    {
    }

    /// <summary>
    /// Implemented by: [NoInstanceNamingRules, ArmorInstanceNamingRules, FloraInstanceNamingRules, FurnitureInstanceNamingRules, WeaponInstanceNamingRules, ActorInstanceNamingRules, ContainerInstanceNamingRules]
    /// </summary>
    public partial interface IAInstanceNamingRulesGetter :
        ILoquiObject,
        IBinaryItem,
        IFormLinkContainerGetter,
        ILoquiObject<IAInstanceNamingRulesGetter>
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object? CommonSetterInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonSetterTranslationInstance();
        static ILoquiRegistration StaticRegistration => AInstanceNamingRules_Registration.Instance;

    }

    #endregion

    #region Common MixIn
    public static partial class AInstanceNamingRulesMixIn
    {
        public static void Clear(this IAInstanceNamingRules item)
        {
            ((AInstanceNamingRulesSetterCommon)((IAInstanceNamingRulesGetter)item).CommonSetterInstance()!).Clear(item: item);
        }

        public static AInstanceNamingRules.Mask<bool> GetEqualsMask(
            this IAInstanceNamingRulesGetter item,
            IAInstanceNamingRulesGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            return ((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)item).CommonInstance()!).GetEqualsMask(
                item: item,
                rhs: rhs,
                include: include);
        }

        public static string Print(
            this IAInstanceNamingRulesGetter item,
            string? name = null,
            AInstanceNamingRules.Mask<bool>? printMask = null)
        {
            return ((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)item).CommonInstance()!).Print(
                item: item,
                name: name,
                printMask: printMask);
        }

        public static void Print(
            this IAInstanceNamingRulesGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            AInstanceNamingRules.Mask<bool>? printMask = null)
        {
            ((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)item).CommonInstance()!).Print(
                item: item,
                sb: sb,
                name: name,
                printMask: printMask);
        }

        public static bool Equals(
            this IAInstanceNamingRulesGetter item,
            IAInstanceNamingRulesGetter rhs,
            AInstanceNamingRules.TranslationMask? equalsMask = null)
        {
            return ((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)item).CommonInstance()!).Equals(
                lhs: item,
                rhs: rhs,
                equalsMask: equalsMask?.GetCrystal());
        }

        public static void DeepCopyIn(
            this IAInstanceNamingRules lhs,
            IAInstanceNamingRulesGetter rhs)
        {
            ((AInstanceNamingRulesSetterTranslationCommon)((IAInstanceNamingRulesGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: default,
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IAInstanceNamingRules lhs,
            IAInstanceNamingRulesGetter rhs,
            AInstanceNamingRules.TranslationMask? copyMask = null)
        {
            ((AInstanceNamingRulesSetterTranslationCommon)((IAInstanceNamingRulesGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IAInstanceNamingRules lhs,
            IAInstanceNamingRulesGetter rhs,
            out AInstanceNamingRules.ErrorMask errorMask,
            AInstanceNamingRules.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ((AInstanceNamingRulesSetterTranslationCommon)((IAInstanceNamingRulesGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
            errorMask = AInstanceNamingRules.ErrorMask.Factory(errorMaskBuilder);
        }

        public static void DeepCopyIn(
            this IAInstanceNamingRules lhs,
            IAInstanceNamingRulesGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
            ((AInstanceNamingRulesSetterTranslationCommon)((IAInstanceNamingRulesGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: false);
        }

        public static AInstanceNamingRules DeepCopy(
            this IAInstanceNamingRulesGetter item,
            AInstanceNamingRules.TranslationMask? copyMask = null)
        {
            return ((AInstanceNamingRulesSetterTranslationCommon)((IAInstanceNamingRulesGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask);
        }

        public static AInstanceNamingRules DeepCopy(
            this IAInstanceNamingRulesGetter item,
            out AInstanceNamingRules.ErrorMask errorMask,
            AInstanceNamingRules.TranslationMask? copyMask = null)
        {
            return ((AInstanceNamingRulesSetterTranslationCommon)((IAInstanceNamingRulesGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: out errorMask);
        }

        public static AInstanceNamingRules DeepCopy(
            this IAInstanceNamingRulesGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            return ((AInstanceNamingRulesSetterTranslationCommon)((IAInstanceNamingRulesGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: errorMask);
        }

        #region Binary Translation
        public static void CopyInFromBinary(
            this IAInstanceNamingRules item,
            MutagenFrame frame,
            TypedParseParams translationParams = default)
        {
            ((AInstanceNamingRulesSetterCommon)((IAInstanceNamingRulesGetter)item).CommonSetterInstance()!).CopyInFromBinary(
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
    internal enum AInstanceNamingRules_FieldIndex
    {
    }
    #endregion

    #region Registration
    internal partial class AInstanceNamingRules_Registration : ILoquiRegistration
    {
        public static readonly AInstanceNamingRules_Registration Instance = new AInstanceNamingRules_Registration();

        public static ProtocolKey ProtocolKey => ProtocolDefinition_Starfield.ProtocolKey;

        public const ushort AdditionalFieldCount = 0;

        public const ushort FieldCount = 0;

        public static readonly Type MaskType = typeof(AInstanceNamingRules.Mask<>);

        public static readonly Type ErrorMaskType = typeof(AInstanceNamingRules.ErrorMask);

        public static readonly Type ClassType = typeof(AInstanceNamingRules);

        public static readonly Type GetterType = typeof(IAInstanceNamingRulesGetter);

        public static readonly Type? InternalGetterType = null;

        public static readonly Type SetterType = typeof(IAInstanceNamingRules);

        public static readonly Type? InternalSetterType = null;

        public const string FullName = "Mutagen.Bethesda.Starfield.AInstanceNamingRules";

        public const string Name = "AInstanceNamingRules";

        public const string Namespace = "Mutagen.Bethesda.Starfield";

        public const byte GenericCount = 0;

        public static readonly Type? GenericRegistrationType = null;

        public static readonly RecordType TriggeringRecordType = RecordTypes.VNAM;
        public static RecordTriggerSpecs TriggerSpecs => _recordSpecs.Value;
        private static readonly Lazy<RecordTriggerSpecs> _recordSpecs = new Lazy<RecordTriggerSpecs>(() =>
        {
            var all = RecordCollection.Factory(RecordTypes.VNAM);
            return new RecordTriggerSpecs(allRecordTypes: all);
        });
        public static readonly Type BinaryWriteTranslation = typeof(AInstanceNamingRulesBinaryWriteTranslation);
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
    internal partial class AInstanceNamingRulesSetterCommon
    {
        public static readonly AInstanceNamingRulesSetterCommon Instance = new AInstanceNamingRulesSetterCommon();

        partial void ClearPartial();
        
        public virtual void Clear(IAInstanceNamingRules item)
        {
            ClearPartial();
        }
        
        #region Mutagen
        public void RemapLinks(IAInstanceNamingRules obj, IReadOnlyDictionary<FormKey, FormKey> mapping)
        {
        }
        
        #endregion
        
        #region Binary Translation
        public virtual void CopyInFromBinary(
            IAInstanceNamingRules item,
            MutagenFrame frame,
            TypedParseParams translationParams)
        {
        }
        
        #endregion
        
    }
    internal partial class AInstanceNamingRulesCommon
    {
        public static readonly AInstanceNamingRulesCommon Instance = new AInstanceNamingRulesCommon();

        public AInstanceNamingRules.Mask<bool> GetEqualsMask(
            IAInstanceNamingRulesGetter item,
            IAInstanceNamingRulesGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            var ret = new AInstanceNamingRules.Mask<bool>(false);
            ((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)item).CommonInstance()!).FillEqualsMask(
                item: item,
                rhs: rhs,
                ret: ret,
                include: include);
            return ret;
        }
        
        public void FillEqualsMask(
            IAInstanceNamingRulesGetter item,
            IAInstanceNamingRulesGetter rhs,
            AInstanceNamingRules.Mask<bool> ret,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
        }
        
        public string Print(
            IAInstanceNamingRulesGetter item,
            string? name = null,
            AInstanceNamingRules.Mask<bool>? printMask = null)
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
            IAInstanceNamingRulesGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            AInstanceNamingRules.Mask<bool>? printMask = null)
        {
            if (name == null)
            {
                sb.AppendLine($"AInstanceNamingRules =>");
            }
            else
            {
                sb.AppendLine($"{name} (AInstanceNamingRules) =>");
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
            IAInstanceNamingRulesGetter item,
            StructuredStringBuilder sb,
            AInstanceNamingRules.Mask<bool>? printMask = null)
        {
        }
        
        #region Equals and Hash
        public virtual bool Equals(
            IAInstanceNamingRulesGetter? lhs,
            IAInstanceNamingRulesGetter? rhs,
            TranslationCrystal? equalsMask)
        {
            if (!EqualsMaskHelper.RefEquality(lhs, rhs, out var isEqual)) return isEqual;
            return true;
        }
        
        public virtual int GetHashCode(IAInstanceNamingRulesGetter item)
        {
            var hash = new HashCode();
            return hash.ToHashCode();
        }
        
        #endregion
        
        
        public virtual object GetNew()
        {
            return AInstanceNamingRules.GetNew();
        }
        
        #region Mutagen
        public IEnumerable<IFormLinkGetter> EnumerateFormLinks(IAInstanceNamingRulesGetter obj)
        {
            yield break;
        }
        
        #endregion
        
    }
    internal partial class AInstanceNamingRulesSetterTranslationCommon
    {
        public static readonly AInstanceNamingRulesSetterTranslationCommon Instance = new AInstanceNamingRulesSetterTranslationCommon();

        #region DeepCopyIn
        public virtual void DeepCopyIn(
            IAInstanceNamingRules item,
            IAInstanceNamingRulesGetter rhs,
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
            IAInstanceNamingRules item,
            IAInstanceNamingRulesGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy);
        #endregion
        
        public AInstanceNamingRules DeepCopy(
            IAInstanceNamingRulesGetter item,
            AInstanceNamingRules.TranslationMask? copyMask = null)
        {
            AInstanceNamingRules ret = (AInstanceNamingRules)((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)item).CommonInstance()!).GetNew();
            ((AInstanceNamingRulesSetterTranslationCommon)((IAInstanceNamingRulesGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: null,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            return ret;
        }
        
        public AInstanceNamingRules DeepCopy(
            IAInstanceNamingRulesGetter item,
            out AInstanceNamingRules.ErrorMask errorMask,
            AInstanceNamingRules.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            AInstanceNamingRules ret = (AInstanceNamingRules)((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)item).CommonInstance()!).GetNew();
            ((AInstanceNamingRulesSetterTranslationCommon)((IAInstanceNamingRulesGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                ret,
                item,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            errorMask = AInstanceNamingRules.ErrorMask.Factory(errorMaskBuilder);
            return ret;
        }
        
        public AInstanceNamingRules DeepCopy(
            IAInstanceNamingRulesGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            AInstanceNamingRules ret = (AInstanceNamingRules)((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)item).CommonInstance()!).GetNew();
            ((AInstanceNamingRulesSetterTranslationCommon)((IAInstanceNamingRulesGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
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
    public partial class AInstanceNamingRules
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => AInstanceNamingRules_Registration.Instance;
        public static ILoquiRegistration StaticRegistration => AInstanceNamingRules_Registration.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonInstance() => AInstanceNamingRulesCommon.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonSetterInstance()
        {
            return AInstanceNamingRulesSetterCommon.Instance;
        }
        [DebuggerStepThrough]
        protected virtual object CommonSetterTranslationInstance() => AInstanceNamingRulesSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IAInstanceNamingRulesGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object IAInstanceNamingRulesGetter.CommonSetterInstance() => this.CommonSetterInstance();
        [DebuggerStepThrough]
        object IAInstanceNamingRulesGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

    }
}

#region Modules
#region Binary Translation
namespace Mutagen.Bethesda.Starfield
{
    public partial class AInstanceNamingRulesBinaryWriteTranslation : IBinaryWriteTranslator
    {
        public static readonly AInstanceNamingRulesBinaryWriteTranslation Instance = new();

        public virtual void Write(
            MutagenWriter writer,
            IAInstanceNamingRulesGetter item,
            TypedWriteParams translationParams)
        {
        }

        public virtual void Write(
            MutagenWriter writer,
            object item,
            TypedWriteParams translationParams = default)
        {
            Write(
                item: (IAInstanceNamingRulesGetter)item,
                writer: writer,
                translationParams: translationParams);
        }

    }

    internal partial class AInstanceNamingRulesBinaryCreateTranslation
    {
        public static readonly AInstanceNamingRulesBinaryCreateTranslation Instance = new AInstanceNamingRulesBinaryCreateTranslation();

    }

}
namespace Mutagen.Bethesda.Starfield
{
    #region Binary Write Mixins
    public static class AInstanceNamingRulesBinaryTranslationMixIn
    {
        public static void WriteToBinary(
            this IAInstanceNamingRulesGetter item,
            MutagenWriter writer,
            TypedWriteParams translationParams = default)
        {
            ((AInstanceNamingRulesBinaryWriteTranslation)item.BinaryWriteTranslator).Write(
                item: item,
                writer: writer,
                translationParams: translationParams);
        }

    }
    #endregion


}
namespace Mutagen.Bethesda.Starfield
{
    internal abstract partial class AInstanceNamingRulesBinaryOverlay :
        PluginBinaryOverlay,
        IAInstanceNamingRulesGetter
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => AInstanceNamingRules_Registration.Instance;
        public static ILoquiRegistration StaticRegistration => AInstanceNamingRules_Registration.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonInstance() => AInstanceNamingRulesCommon.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonSetterTranslationInstance() => AInstanceNamingRulesSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IAInstanceNamingRulesGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object? IAInstanceNamingRulesGetter.CommonSetterInstance() => null;
        [DebuggerStepThrough]
        object IAInstanceNamingRulesGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

        void IPrintable.Print(StructuredStringBuilder sb, string? name) => this.Print(sb, name);

        public virtual IEnumerable<IFormLinkGetter> EnumerateFormLinks() => AInstanceNamingRulesCommon.Instance.EnumerateFormLinks(this);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual object BinaryWriteTranslator => AInstanceNamingRulesBinaryWriteTranslation.Instance;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IBinaryItem.BinaryWriteTranslator => this.BinaryWriteTranslator;
        void IBinaryItem.WriteToBinary(
            MutagenWriter writer,
            TypedWriteParams translationParams = default)
        {
            ((AInstanceNamingRulesBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
                item: this,
                writer: writer,
                translationParams: translationParams);
        }

        partial void CustomFactoryEnd(
            OverlayStream stream,
            int finalPos,
            int offset);

        partial void CustomCtor();
        protected AInstanceNamingRulesBinaryOverlay(
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
            AInstanceNamingRulesMixIn.Print(
                item: this,
                sb: sb,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IAInstanceNamingRulesGetter rhs) return false;
            return ((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)this).CommonInstance()!).Equals(this, rhs, equalsMask: null);
        }

        public bool Equals(IAInstanceNamingRulesGetter? obj)
        {
            return ((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)this).CommonInstance()!).Equals(this, obj, equalsMask: null);
        }

        public override int GetHashCode() => ((AInstanceNamingRulesCommon)((IAInstanceNamingRulesGetter)this).CommonInstance()!).GetHashCode(this);

        #endregion

    }

}
#endregion

#endregion

