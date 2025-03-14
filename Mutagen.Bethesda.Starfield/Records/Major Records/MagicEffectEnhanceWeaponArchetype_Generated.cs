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
using Mutagen.Bethesda.Starfield;
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
    public partial class MagicEffectEnhanceWeaponArchetype :
        AMagicEffectArchetype,
        IEquatable<IMagicEffectEnhanceWeaponArchetypeGetter>,
        ILoquiObjectSetter<MagicEffectEnhanceWeaponArchetype>,
        IMagicEffectEnhanceWeaponArchetype
    {
        #region Association
        private readonly IFormLink<IObjectEffectGetter> _Association = new FormLink<IObjectEffectGetter>();
        public IFormLink<IObjectEffectGetter> Association
        {
            get => _Association;
            set => _Association.SetTo(value);
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IFormLinkGetter<IObjectEffectGetter> IMagicEffectEnhanceWeaponArchetypeGetter.Association => this.Association;
        #endregion

        #region To String

        public override void Print(
            StructuredStringBuilder sb,
            string? name = null)
        {
            MagicEffectEnhanceWeaponArchetypeMixIn.Print(
                item: this,
                sb: sb,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IMagicEffectEnhanceWeaponArchetypeGetter rhs) return false;
            return ((MagicEffectEnhanceWeaponArchetypeCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)this).CommonInstance()!).Equals(this, rhs, equalsMask: null);
        }

        public bool Equals(IMagicEffectEnhanceWeaponArchetypeGetter? obj)
        {
            return ((MagicEffectEnhanceWeaponArchetypeCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)this).CommonInstance()!).Equals(this, obj, equalsMask: null);
        }

        public override int GetHashCode() => ((MagicEffectEnhanceWeaponArchetypeCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)this).CommonInstance()!).GetHashCode(this);

        #endregion

        #region Mask
        public new class Mask<TItem> :
            AMagicEffectArchetype.Mask<TItem>,
            IEquatable<Mask<TItem>>,
            IMask<TItem>
        {
            #region Ctors
            public Mask(TItem Association)
            : base()
            {
                this.Association = Association;
            }

            #pragma warning disable CS8618
            protected Mask()
            {
            }
            #pragma warning restore CS8618

            #endregion

            #region Members
            public TItem Association;
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
                if (!base.Equals(rhs)) return false;
                if (!object.Equals(this.Association, rhs.Association)) return false;
                return true;
            }
            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(this.Association);
                hash.Add(base.GetHashCode());
                return hash.ToHashCode();
            }

            #endregion

            #region All
            public override bool All(Func<TItem, bool> eval)
            {
                if (!base.All(eval)) return false;
                if (!eval(this.Association)) return false;
                return true;
            }
            #endregion

            #region Any
            public override bool Any(Func<TItem, bool> eval)
            {
                if (base.Any(eval)) return true;
                if (eval(this.Association)) return true;
                return false;
            }
            #endregion

            #region Translate
            public new Mask<R> Translate<R>(Func<TItem, R> eval)
            {
                var ret = new MagicEffectEnhanceWeaponArchetype.Mask<R>();
                this.Translate_InternalFill(ret, eval);
                return ret;
            }

            protected void Translate_InternalFill<R>(Mask<R> obj, Func<TItem, R> eval)
            {
                base.Translate_InternalFill(obj, eval);
                obj.Association = eval(this.Association);
            }
            #endregion

            #region To String
            public override string ToString() => this.Print();

            public string Print(MagicEffectEnhanceWeaponArchetype.Mask<bool>? printMask = null)
            {
                var sb = new StructuredStringBuilder();
                Print(sb, printMask);
                return sb.ToString();
            }

            public void Print(StructuredStringBuilder sb, MagicEffectEnhanceWeaponArchetype.Mask<bool>? printMask = null)
            {
                sb.AppendLine($"{nameof(MagicEffectEnhanceWeaponArchetype.Mask<TItem>)} =>");
                using (sb.Brace())
                {
                    if (printMask?.Association ?? true)
                    {
                        sb.AppendItem(Association, "Association");
                    }
                }
            }
            #endregion

        }

        public new class ErrorMask :
            AMagicEffectArchetype.ErrorMask,
            IErrorMask<ErrorMask>
        {
            #region Members
            public Exception? Association;
            #endregion

            #region IErrorMask
            public override object? GetNthMask(int index)
            {
                MagicEffectEnhanceWeaponArchetype_FieldIndex enu = (MagicEffectEnhanceWeaponArchetype_FieldIndex)index;
                switch (enu)
                {
                    case MagicEffectEnhanceWeaponArchetype_FieldIndex.Association:
                        return Association;
                    default:
                        return base.GetNthMask(index);
                }
            }

            public override void SetNthException(int index, Exception ex)
            {
                MagicEffectEnhanceWeaponArchetype_FieldIndex enu = (MagicEffectEnhanceWeaponArchetype_FieldIndex)index;
                switch (enu)
                {
                    case MagicEffectEnhanceWeaponArchetype_FieldIndex.Association:
                        this.Association = ex;
                        break;
                    default:
                        base.SetNthException(index, ex);
                        break;
                }
            }

            public override void SetNthMask(int index, object obj)
            {
                MagicEffectEnhanceWeaponArchetype_FieldIndex enu = (MagicEffectEnhanceWeaponArchetype_FieldIndex)index;
                switch (enu)
                {
                    case MagicEffectEnhanceWeaponArchetype_FieldIndex.Association:
                        this.Association = (Exception?)obj;
                        break;
                    default:
                        base.SetNthMask(index, obj);
                        break;
                }
            }

            public override bool IsInError()
            {
                if (Overall != null) return true;
                if (Association != null) return true;
                return false;
            }
            #endregion

            #region To String
            public override string ToString() => this.Print();

            public override void Print(StructuredStringBuilder sb, string? name = null)
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
            protected override void PrintFillInternal(StructuredStringBuilder sb)
            {
                base.PrintFillInternal(sb);
                {
                    sb.AppendItem(Association, "Association");
                }
            }
            #endregion

            #region Combine
            public ErrorMask Combine(ErrorMask? rhs)
            {
                if (rhs == null) return this;
                var ret = new ErrorMask();
                ret.Association = this.Association.Combine(rhs.Association);
                return ret;
            }
            public static ErrorMask? Combine(ErrorMask? lhs, ErrorMask? rhs)
            {
                if (lhs != null && rhs != null) return lhs.Combine(rhs);
                return lhs ?? rhs;
            }
            #endregion

            #region Factory
            public static new ErrorMask Factory(ErrorMaskBuilder errorMask)
            {
                return new ErrorMask();
            }
            #endregion

        }
        public new class TranslationMask :
            AMagicEffectArchetype.TranslationMask,
            ITranslationMask
        {
            #region Members
            public bool Association;
            #endregion

            #region Ctors
            public TranslationMask(
                bool defaultOn,
                bool onOverall = true)
                : base(defaultOn, onOverall)
            {
                this.Association = defaultOn;
            }

            #endregion

            protected override void GetCrystal(List<(bool On, TranslationCrystal? SubCrystal)> ret)
            {
                base.GetCrystal(ret);
                ret.Add((Association, null));
            }

            public static implicit operator TranslationMask(bool defaultOn)
            {
                return new TranslationMask(defaultOn: defaultOn, onOverall: defaultOn);
            }

        }
        #endregion

        #region Mutagen
        public override IEnumerable<IFormLinkGetter> EnumerateFormLinks() => MagicEffectEnhanceWeaponArchetypeCommon.Instance.EnumerateFormLinks(this);
        public override void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping) => MagicEffectEnhanceWeaponArchetypeSetterCommon.Instance.RemapLinks(this, mapping);
        #endregion

        #region Binary Translation
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override object BinaryWriteTranslator => MagicEffectEnhanceWeaponArchetypeBinaryWriteTranslation.Instance;
        void IBinaryItem.WriteToBinary(
            MutagenWriter writer,
            TypedWriteParams translationParams = default)
        {
            ((MagicEffectEnhanceWeaponArchetypeBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
                item: this,
                writer: writer,
                translationParams: translationParams);
        }
        #region Binary Create
        public new static MagicEffectEnhanceWeaponArchetype CreateFromBinary(
            MutagenFrame frame,
            TypedParseParams translationParams = default)
        {
            var ret = new MagicEffectEnhanceWeaponArchetype();
            ((MagicEffectEnhanceWeaponArchetypeSetterCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)ret).CommonSetterInstance()!).CopyInFromBinary(
                item: ret,
                frame: frame,
                translationParams: translationParams);
            return ret;
        }

        #endregion

        public static bool TryCreateFromBinary(
            MutagenFrame frame,
            out MagicEffectEnhanceWeaponArchetype item,
            TypedParseParams translationParams = default)
        {
            var startPos = frame.Position;
            item = CreateFromBinary(
                frame: frame,
                translationParams: translationParams);
            return startPos != frame.Position;
        }
        #endregion

        void IPrintable.Print(StructuredStringBuilder sb, string? name) => this.Print(sb, name);

        void IClearable.Clear()
        {
            ((MagicEffectEnhanceWeaponArchetypeSetterCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)this).CommonSetterInstance()!).Clear(this);
        }

        internal static new MagicEffectEnhanceWeaponArchetype GetNew()
        {
            return new MagicEffectEnhanceWeaponArchetype();
        }

    }
    #endregion

    #region Interface
    public partial interface IMagicEffectEnhanceWeaponArchetype :
        IAMagicEffectArchetype,
        IFormLinkContainer,
        ILoquiObjectSetter<IMagicEffectEnhanceWeaponArchetype>,
        IMagicEffectEnhanceWeaponArchetypeGetter
    {
        new IFormLink<IObjectEffectGetter> Association { get; set; }
    }

    public partial interface IMagicEffectEnhanceWeaponArchetypeGetter :
        IAMagicEffectArchetypeGetter,
        IBinaryItem,
        IFormLinkContainerGetter,
        ILoquiObject<IMagicEffectEnhanceWeaponArchetypeGetter>
    {
        static new ILoquiRegistration StaticRegistration => MagicEffectEnhanceWeaponArchetype_Registration.Instance;
        IFormLinkGetter<IObjectEffectGetter> Association { get; }

    }

    #endregion

    #region Common MixIn
    public static partial class MagicEffectEnhanceWeaponArchetypeMixIn
    {
        public static void Clear(this IMagicEffectEnhanceWeaponArchetype item)
        {
            ((MagicEffectEnhanceWeaponArchetypeSetterCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonSetterInstance()!).Clear(item: item);
        }

        public static MagicEffectEnhanceWeaponArchetype.Mask<bool> GetEqualsMask(
            this IMagicEffectEnhanceWeaponArchetypeGetter item,
            IMagicEffectEnhanceWeaponArchetypeGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            return ((MagicEffectEnhanceWeaponArchetypeCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonInstance()!).GetEqualsMask(
                item: item,
                rhs: rhs,
                include: include);
        }

        public static string Print(
            this IMagicEffectEnhanceWeaponArchetypeGetter item,
            string? name = null,
            MagicEffectEnhanceWeaponArchetype.Mask<bool>? printMask = null)
        {
            return ((MagicEffectEnhanceWeaponArchetypeCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonInstance()!).Print(
                item: item,
                name: name,
                printMask: printMask);
        }

        public static void Print(
            this IMagicEffectEnhanceWeaponArchetypeGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            MagicEffectEnhanceWeaponArchetype.Mask<bool>? printMask = null)
        {
            ((MagicEffectEnhanceWeaponArchetypeCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonInstance()!).Print(
                item: item,
                sb: sb,
                name: name,
                printMask: printMask);
        }

        public static bool Equals(
            this IMagicEffectEnhanceWeaponArchetypeGetter item,
            IMagicEffectEnhanceWeaponArchetypeGetter rhs,
            MagicEffectEnhanceWeaponArchetype.TranslationMask? equalsMask = null)
        {
            return ((MagicEffectEnhanceWeaponArchetypeCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonInstance()!).Equals(
                lhs: item,
                rhs: rhs,
                equalsMask: equalsMask?.GetCrystal());
        }

        public static void DeepCopyIn(
            this IMagicEffectEnhanceWeaponArchetype lhs,
            IMagicEffectEnhanceWeaponArchetypeGetter rhs,
            out MagicEffectEnhanceWeaponArchetype.ErrorMask errorMask,
            MagicEffectEnhanceWeaponArchetype.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ((MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
            errorMask = MagicEffectEnhanceWeaponArchetype.ErrorMask.Factory(errorMaskBuilder);
        }

        public static void DeepCopyIn(
            this IMagicEffectEnhanceWeaponArchetype lhs,
            IMagicEffectEnhanceWeaponArchetypeGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
            ((MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: false);
        }

        public static MagicEffectEnhanceWeaponArchetype DeepCopy(
            this IMagicEffectEnhanceWeaponArchetypeGetter item,
            MagicEffectEnhanceWeaponArchetype.TranslationMask? copyMask = null)
        {
            return ((MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask);
        }

        public static MagicEffectEnhanceWeaponArchetype DeepCopy(
            this IMagicEffectEnhanceWeaponArchetypeGetter item,
            out MagicEffectEnhanceWeaponArchetype.ErrorMask errorMask,
            MagicEffectEnhanceWeaponArchetype.TranslationMask? copyMask = null)
        {
            return ((MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: out errorMask);
        }

        public static MagicEffectEnhanceWeaponArchetype DeepCopy(
            this IMagicEffectEnhanceWeaponArchetypeGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            return ((MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: errorMask);
        }

        #region Binary Translation
        public static void CopyInFromBinary(
            this IMagicEffectEnhanceWeaponArchetype item,
            MutagenFrame frame,
            TypedParseParams translationParams = default)
        {
            ((MagicEffectEnhanceWeaponArchetypeSetterCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonSetterInstance()!).CopyInFromBinary(
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
    internal enum MagicEffectEnhanceWeaponArchetype_FieldIndex
    {
        Association = 0,
    }
    #endregion

    #region Registration
    internal partial class MagicEffectEnhanceWeaponArchetype_Registration : ILoquiRegistration
    {
        public static readonly MagicEffectEnhanceWeaponArchetype_Registration Instance = new MagicEffectEnhanceWeaponArchetype_Registration();

        public static ProtocolKey ProtocolKey => ProtocolDefinition_Starfield.ProtocolKey;

        public const ushort AdditionalFieldCount = 1;

        public const ushort FieldCount = 1;

        public static readonly Type MaskType = typeof(MagicEffectEnhanceWeaponArchetype.Mask<>);

        public static readonly Type ErrorMaskType = typeof(MagicEffectEnhanceWeaponArchetype.ErrorMask);

        public static readonly Type ClassType = typeof(MagicEffectEnhanceWeaponArchetype);

        public static readonly Type GetterType = typeof(IMagicEffectEnhanceWeaponArchetypeGetter);

        public static readonly Type? InternalGetterType = null;

        public static readonly Type SetterType = typeof(IMagicEffectEnhanceWeaponArchetype);

        public static readonly Type? InternalSetterType = null;

        public const string FullName = "Mutagen.Bethesda.Starfield.MagicEffectEnhanceWeaponArchetype";

        public const string Name = "MagicEffectEnhanceWeaponArchetype";

        public const string Namespace = "Mutagen.Bethesda.Starfield";

        public const byte GenericCount = 0;

        public static readonly Type? GenericRegistrationType = null;

        public static readonly Type BinaryWriteTranslation = typeof(MagicEffectEnhanceWeaponArchetypeBinaryWriteTranslation);
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
    internal partial class MagicEffectEnhanceWeaponArchetypeSetterCommon : AMagicEffectArchetypeSetterCommon
    {
        public new static readonly MagicEffectEnhanceWeaponArchetypeSetterCommon Instance = new MagicEffectEnhanceWeaponArchetypeSetterCommon();

        partial void ClearPartial();
        
        public void Clear(IMagicEffectEnhanceWeaponArchetype item)
        {
            ClearPartial();
            item.Association.Clear();
            base.Clear(item);
        }
        
        public override void Clear(IAMagicEffectArchetype item)
        {
            Clear(item: (IMagicEffectEnhanceWeaponArchetype)item);
        }
        
        #region Mutagen
        public void RemapLinks(IMagicEffectEnhanceWeaponArchetype obj, IReadOnlyDictionary<FormKey, FormKey> mapping)
        {
            base.RemapLinks(obj, mapping);
            obj.Association.Relink(mapping);
        }
        
        #endregion
        
        #region Binary Translation
        public virtual void CopyInFromBinary(
            IMagicEffectEnhanceWeaponArchetype item,
            MutagenFrame frame,
            TypedParseParams translationParams)
        {
            PluginUtilityTranslation.SubrecordParse(
                record: item,
                frame: frame,
                translationParams: translationParams,
                fillStructs: MagicEffectEnhanceWeaponArchetypeBinaryCreateTranslation.FillBinaryStructs);
        }
        
        public override void CopyInFromBinary(
            IAMagicEffectArchetype item,
            MutagenFrame frame,
            TypedParseParams translationParams)
        {
            CopyInFromBinary(
                item: (MagicEffectEnhanceWeaponArchetype)item,
                frame: frame,
                translationParams: translationParams);
        }
        
        #endregion
        
    }
    internal partial class MagicEffectEnhanceWeaponArchetypeCommon : AMagicEffectArchetypeCommon
    {
        public new static readonly MagicEffectEnhanceWeaponArchetypeCommon Instance = new MagicEffectEnhanceWeaponArchetypeCommon();

        public MagicEffectEnhanceWeaponArchetype.Mask<bool> GetEqualsMask(
            IMagicEffectEnhanceWeaponArchetypeGetter item,
            IMagicEffectEnhanceWeaponArchetypeGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            var ret = new MagicEffectEnhanceWeaponArchetype.Mask<bool>(false);
            ((MagicEffectEnhanceWeaponArchetypeCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonInstance()!).FillEqualsMask(
                item: item,
                rhs: rhs,
                ret: ret,
                include: include);
            return ret;
        }
        
        public void FillEqualsMask(
            IMagicEffectEnhanceWeaponArchetypeGetter item,
            IMagicEffectEnhanceWeaponArchetypeGetter rhs,
            MagicEffectEnhanceWeaponArchetype.Mask<bool> ret,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            ret.Association = item.Association.Equals(rhs.Association);
            base.FillEqualsMask(item, rhs, ret, include);
        }
        
        public string Print(
            IMagicEffectEnhanceWeaponArchetypeGetter item,
            string? name = null,
            MagicEffectEnhanceWeaponArchetype.Mask<bool>? printMask = null)
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
            IMagicEffectEnhanceWeaponArchetypeGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            MagicEffectEnhanceWeaponArchetype.Mask<bool>? printMask = null)
        {
            if (name == null)
            {
                sb.AppendLine($"MagicEffectEnhanceWeaponArchetype =>");
            }
            else
            {
                sb.AppendLine($"{name} (MagicEffectEnhanceWeaponArchetype) =>");
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
            IMagicEffectEnhanceWeaponArchetypeGetter item,
            StructuredStringBuilder sb,
            MagicEffectEnhanceWeaponArchetype.Mask<bool>? printMask = null)
        {
            AMagicEffectArchetypeCommon.ToStringFields(
                item: item,
                sb: sb,
                printMask: printMask);
            if (printMask?.Association ?? true)
            {
                sb.AppendItem(item.Association.FormKey, "Association");
            }
        }
        
        public static MagicEffectEnhanceWeaponArchetype_FieldIndex ConvertFieldIndex(AMagicEffectArchetype_FieldIndex index)
        {
            switch (index)
            {
                default:
                    throw new ArgumentException($"Index is out of range: {index.ToStringFast()}");
            }
        }
        
        #region Equals and Hash
        public virtual bool Equals(
            IMagicEffectEnhanceWeaponArchetypeGetter? lhs,
            IMagicEffectEnhanceWeaponArchetypeGetter? rhs,
            TranslationCrystal? equalsMask)
        {
            if (!EqualsMaskHelper.RefEquality(lhs, rhs, out var isEqual)) return isEqual;
            if (!base.Equals((IAMagicEffectArchetypeGetter)lhs, (IAMagicEffectArchetypeGetter)rhs, equalsMask)) return false;
            if ((equalsMask?.GetShouldTranslate((int)MagicEffectEnhanceWeaponArchetype_FieldIndex.Association) ?? true))
            {
                if (!lhs.Association.Equals(rhs.Association)) return false;
            }
            return true;
        }
        
        public override bool Equals(
            IAMagicEffectArchetypeGetter? lhs,
            IAMagicEffectArchetypeGetter? rhs,
            TranslationCrystal? equalsMask)
        {
            return Equals(
                lhs: (IMagicEffectEnhanceWeaponArchetypeGetter?)lhs,
                rhs: rhs as IMagicEffectEnhanceWeaponArchetypeGetter,
                equalsMask: equalsMask);
        }
        
        public virtual int GetHashCode(IMagicEffectEnhanceWeaponArchetypeGetter item)
        {
            var hash = new HashCode();
            hash.Add(item.Association);
            hash.Add(base.GetHashCode());
            return hash.ToHashCode();
        }
        
        public override int GetHashCode(IAMagicEffectArchetypeGetter item)
        {
            return GetHashCode(item: (IMagicEffectEnhanceWeaponArchetypeGetter)item);
        }
        
        #endregion
        
        
        public override object GetNew()
        {
            return MagicEffectEnhanceWeaponArchetype.GetNew();
        }
        
        #region Mutagen
        public IEnumerable<IFormLinkGetter> EnumerateFormLinks(IMagicEffectEnhanceWeaponArchetypeGetter obj)
        {
            foreach (var item in base.EnumerateFormLinks(obj))
            {
                yield return item;
            }
            yield return FormLinkInformation.Factory(obj.Association);
            yield break;
        }
        
        #endregion
        
    }
    internal partial class MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon : AMagicEffectArchetypeSetterTranslationCommon
    {
        public new static readonly MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon Instance = new MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon();

        #region DeepCopyIn
        public void DeepCopyIn(
            IMagicEffectEnhanceWeaponArchetype item,
            IMagicEffectEnhanceWeaponArchetypeGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy)
        {
            base.DeepCopyIn(
                (IAMagicEffectArchetype)item,
                (IAMagicEffectArchetypeGetter)rhs,
                errorMask,
                copyMask,
                deepCopy: deepCopy);
            if ((copyMask?.GetShouldTranslate((int)MagicEffectEnhanceWeaponArchetype_FieldIndex.Association) ?? true))
            {
                item.Association.SetTo(rhs.Association.FormKey);
            }
            DeepCopyInCustom(
                item: item,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: deepCopy);
        }
        
        partial void DeepCopyInCustom(
            IMagicEffectEnhanceWeaponArchetype item,
            IMagicEffectEnhanceWeaponArchetypeGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy);
        
        public override void DeepCopyIn(
            IAMagicEffectArchetype item,
            IAMagicEffectArchetypeGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy)
        {
            this.DeepCopyIn(
                item: (IMagicEffectEnhanceWeaponArchetype)item,
                rhs: (IMagicEffectEnhanceWeaponArchetypeGetter)rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: deepCopy);
        }
        
        #endregion
        
        public MagicEffectEnhanceWeaponArchetype DeepCopy(
            IMagicEffectEnhanceWeaponArchetypeGetter item,
            MagicEffectEnhanceWeaponArchetype.TranslationMask? copyMask = null)
        {
            MagicEffectEnhanceWeaponArchetype ret = (MagicEffectEnhanceWeaponArchetype)((MagicEffectEnhanceWeaponArchetypeCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonInstance()!).GetNew();
            ((MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: null,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            return ret;
        }
        
        public MagicEffectEnhanceWeaponArchetype DeepCopy(
            IMagicEffectEnhanceWeaponArchetypeGetter item,
            out MagicEffectEnhanceWeaponArchetype.ErrorMask errorMask,
            MagicEffectEnhanceWeaponArchetype.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            MagicEffectEnhanceWeaponArchetype ret = (MagicEffectEnhanceWeaponArchetype)((MagicEffectEnhanceWeaponArchetypeCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonInstance()!).GetNew();
            ((MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                ret,
                item,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            errorMask = MagicEffectEnhanceWeaponArchetype.ErrorMask.Factory(errorMaskBuilder);
            return ret;
        }
        
        public MagicEffectEnhanceWeaponArchetype DeepCopy(
            IMagicEffectEnhanceWeaponArchetypeGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            MagicEffectEnhanceWeaponArchetype ret = (MagicEffectEnhanceWeaponArchetype)((MagicEffectEnhanceWeaponArchetypeCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)item).CommonInstance()!).GetNew();
            ((MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon)((IMagicEffectEnhanceWeaponArchetypeGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
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
    public partial class MagicEffectEnhanceWeaponArchetype
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => MagicEffectEnhanceWeaponArchetype_Registration.Instance;
        public new static ILoquiRegistration StaticRegistration => MagicEffectEnhanceWeaponArchetype_Registration.Instance;
        [DebuggerStepThrough]
        protected override object CommonInstance() => MagicEffectEnhanceWeaponArchetypeCommon.Instance;
        [DebuggerStepThrough]
        protected override object CommonSetterInstance()
        {
            return MagicEffectEnhanceWeaponArchetypeSetterCommon.Instance;
        }
        [DebuggerStepThrough]
        protected override object CommonSetterTranslationInstance() => MagicEffectEnhanceWeaponArchetypeSetterTranslationCommon.Instance;

        #endregion

    }
}

#region Modules
#region Binary Translation
namespace Mutagen.Bethesda.Starfield
{
    public partial class MagicEffectEnhanceWeaponArchetypeBinaryWriteTranslation :
        AMagicEffectArchetypeBinaryWriteTranslation,
        IBinaryWriteTranslator
    {
        public new static readonly MagicEffectEnhanceWeaponArchetypeBinaryWriteTranslation Instance = new();

        public static void WriteEmbedded(
            IMagicEffectEnhanceWeaponArchetypeGetter item,
            MutagenWriter writer)
        {
        }

        public void Write(
            MutagenWriter writer,
            IMagicEffectEnhanceWeaponArchetypeGetter item,
            TypedWriteParams translationParams)
        {
            WriteEmbedded(
                item: item,
                writer: writer);
        }

        public override void Write(
            MutagenWriter writer,
            object item,
            TypedWriteParams translationParams = default)
        {
            Write(
                item: (IMagicEffectEnhanceWeaponArchetypeGetter)item,
                writer: writer,
                translationParams: translationParams);
        }

        public override void Write(
            MutagenWriter writer,
            IAMagicEffectArchetypeGetter item,
            TypedWriteParams translationParams)
        {
            Write(
                item: (IMagicEffectEnhanceWeaponArchetypeGetter)item,
                writer: writer,
                translationParams: translationParams);
        }

    }

    internal partial class MagicEffectEnhanceWeaponArchetypeBinaryCreateTranslation : AMagicEffectArchetypeBinaryCreateTranslation
    {
        public new static readonly MagicEffectEnhanceWeaponArchetypeBinaryCreateTranslation Instance = new MagicEffectEnhanceWeaponArchetypeBinaryCreateTranslation();

        public static void FillBinaryStructs(
            IMagicEffectEnhanceWeaponArchetype item,
            MutagenFrame frame)
        {
        }

    }

}
namespace Mutagen.Bethesda.Starfield
{
    #region Binary Write Mixins
    public static class MagicEffectEnhanceWeaponArchetypeBinaryTranslationMixIn
    {
    }
    #endregion


}
namespace Mutagen.Bethesda.Starfield
{
}
#endregion

#endregion

