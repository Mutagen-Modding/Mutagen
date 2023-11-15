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
    public partial class MagicEffectArchetype :
        AMagicEffectArchetype,
        IEquatable<IMagicEffectArchetypeGetter>,
        ILoquiObjectSetter<MagicEffectArchetype>,
        IMagicEffectArchetype
    {
        #region Ctor
        public MagicEffectArchetype()
        {
            CustomCtor();
        }
        partial void CustomCtor();
        #endregion

        #region Type
        public MagicEffectArchetype.TypeEnum Type { get; set; } = default;
        #endregion
        #region Association
        private readonly IFormLink<IStarfieldMajorRecordGetter> _Association = new FormLink<IStarfieldMajorRecordGetter>();
        public IFormLink<IStarfieldMajorRecordGetter> Association
        {
            get => _Association;
            set => _Association.SetTo(value);
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IFormLinkGetter<IStarfieldMajorRecordGetter> IMagicEffectArchetypeGetter.Association => this.Association;
        #endregion

        #region To String

        public override void Print(
            StructuredStringBuilder sb,
            string? name = null)
        {
            MagicEffectArchetypeMixIn.Print(
                item: this,
                sb: sb,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IMagicEffectArchetypeGetter rhs) return false;
            return ((MagicEffectArchetypeCommon)((IMagicEffectArchetypeGetter)this).CommonInstance()!).Equals(this, rhs, equalsMask: null);
        }

        public bool Equals(IMagicEffectArchetypeGetter? obj)
        {
            return ((MagicEffectArchetypeCommon)((IMagicEffectArchetypeGetter)this).CommonInstance()!).Equals(this, obj, equalsMask: null);
        }

        public override int GetHashCode() => ((MagicEffectArchetypeCommon)((IMagicEffectArchetypeGetter)this).CommonInstance()!).GetHashCode(this);

        #endregion

        #region Mask
        public new class Mask<TItem> :
            AMagicEffectArchetype.Mask<TItem>,
            IEquatable<Mask<TItem>>,
            IMask<TItem>
        {
            #region Ctors
            public Mask(TItem initialValue)
            : base(initialValue)
            {
                this.Type = initialValue;
                this.Association = initialValue;
            }

            public Mask(
                TItem Type,
                TItem Association)
            : base()
            {
                this.Type = Type;
                this.Association = Association;
            }

            #pragma warning disable CS8618
            protected Mask()
            {
            }
            #pragma warning restore CS8618

            #endregion

            #region Members
            public TItem Type;
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
                if (!object.Equals(this.Type, rhs.Type)) return false;
                if (!object.Equals(this.Association, rhs.Association)) return false;
                return true;
            }
            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(this.Type);
                hash.Add(this.Association);
                hash.Add(base.GetHashCode());
                return hash.ToHashCode();
            }

            #endregion

            #region All
            public override bool All(Func<TItem, bool> eval)
            {
                if (!base.All(eval)) return false;
                if (!eval(this.Type)) return false;
                if (!eval(this.Association)) return false;
                return true;
            }
            #endregion

            #region Any
            public override bool Any(Func<TItem, bool> eval)
            {
                if (base.Any(eval)) return true;
                if (eval(this.Type)) return true;
                if (eval(this.Association)) return true;
                return false;
            }
            #endregion

            #region Translate
            public new Mask<R> Translate<R>(Func<TItem, R> eval)
            {
                var ret = new MagicEffectArchetype.Mask<R>();
                this.Translate_InternalFill(ret, eval);
                return ret;
            }

            protected void Translate_InternalFill<R>(Mask<R> obj, Func<TItem, R> eval)
            {
                base.Translate_InternalFill(obj, eval);
                obj.Type = eval(this.Type);
                obj.Association = eval(this.Association);
            }
            #endregion

            #region To String
            public override string ToString() => this.Print();

            public string Print(MagicEffectArchetype.Mask<bool>? printMask = null)
            {
                var sb = new StructuredStringBuilder();
                Print(sb, printMask);
                return sb.ToString();
            }

            public void Print(StructuredStringBuilder sb, MagicEffectArchetype.Mask<bool>? printMask = null)
            {
                sb.AppendLine($"{nameof(MagicEffectArchetype.Mask<TItem>)} =>");
                using (sb.Brace())
                {
                    if (printMask?.Type ?? true)
                    {
                        sb.AppendItem(Type, "Type");
                    }
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
            public Exception? Type;
            public Exception? Association;
            #endregion

            #region IErrorMask
            public override object? GetNthMask(int index)
            {
                MagicEffectArchetype_FieldIndex enu = (MagicEffectArchetype_FieldIndex)index;
                switch (enu)
                {
                    case MagicEffectArchetype_FieldIndex.Type:
                        return Type;
                    case MagicEffectArchetype_FieldIndex.Association:
                        return Association;
                    default:
                        return base.GetNthMask(index);
                }
            }

            public override void SetNthException(int index, Exception ex)
            {
                MagicEffectArchetype_FieldIndex enu = (MagicEffectArchetype_FieldIndex)index;
                switch (enu)
                {
                    case MagicEffectArchetype_FieldIndex.Type:
                        this.Type = ex;
                        break;
                    case MagicEffectArchetype_FieldIndex.Association:
                        this.Association = ex;
                        break;
                    default:
                        base.SetNthException(index, ex);
                        break;
                }
            }

            public override void SetNthMask(int index, object obj)
            {
                MagicEffectArchetype_FieldIndex enu = (MagicEffectArchetype_FieldIndex)index;
                switch (enu)
                {
                    case MagicEffectArchetype_FieldIndex.Type:
                        this.Type = (Exception?)obj;
                        break;
                    case MagicEffectArchetype_FieldIndex.Association:
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
                if (Type != null) return true;
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
                    sb.AppendItem(Type, "Type");
                }
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
                ret.Type = this.Type.Combine(rhs.Type);
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
            public bool Type;
            public bool Association;
            #endregion

            #region Ctors
            public TranslationMask(
                bool defaultOn,
                bool onOverall = true)
                : base(defaultOn, onOverall)
            {
                this.Type = defaultOn;
                this.Association = defaultOn;
            }

            #endregion

            protected override void GetCrystal(List<(bool On, TranslationCrystal? SubCrystal)> ret)
            {
                base.GetCrystal(ret);
                ret.Add((Type, null));
                ret.Add((Association, null));
            }

            public static implicit operator TranslationMask(bool defaultOn)
            {
                return new TranslationMask(defaultOn: defaultOn, onOverall: defaultOn);
            }

        }
        #endregion

        #region Mutagen
        public override IEnumerable<IFormLinkGetter> EnumerateFormLinks() => MagicEffectArchetypeCommon.Instance.EnumerateFormLinks(this);
        public override void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping) => MagicEffectArchetypeSetterCommon.Instance.RemapLinks(this, mapping);
        #endregion

        #region Binary Translation
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override object BinaryWriteTranslator => MagicEffectArchetypeBinaryWriteTranslation.Instance;
        void IBinaryItem.WriteToBinary(
            MutagenWriter writer,
            TypedWriteParams translationParams = default)
        {
            ((MagicEffectArchetypeBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
                item: this,
                writer: writer,
                translationParams: translationParams);
        }
        #region Binary Create
        public new static MagicEffectArchetype CreateFromBinary(
            MutagenFrame frame,
            TypedParseParams translationParams = default)
        {
            var ret = new MagicEffectArchetype();
            ((MagicEffectArchetypeSetterCommon)((IMagicEffectArchetypeGetter)ret).CommonSetterInstance()!).CopyInFromBinary(
                item: ret,
                frame: frame,
                translationParams: translationParams);
            return ret;
        }

        #endregion

        public static bool TryCreateFromBinary(
            MutagenFrame frame,
            out MagicEffectArchetype item,
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
            ((MagicEffectArchetypeSetterCommon)((IMagicEffectArchetypeGetter)this).CommonSetterInstance()!).Clear(this);
        }

        internal static new MagicEffectArchetype GetNew()
        {
            return new MagicEffectArchetype();
        }

    }
    #endregion

    #region Interface
    public partial interface IMagicEffectArchetype :
        IAMagicEffectArchetype,
        IFormLinkContainer,
        ILoquiObjectSetter<IMagicEffectArchetype>,
        IMagicEffectArchetypeGetter
    {
        new MagicEffectArchetype.TypeEnum Type { get; set; }
        new IFormLink<IStarfieldMajorRecordGetter> Association { get; set; }
    }

    public partial interface IMagicEffectArchetypeGetter :
        IAMagicEffectArchetypeGetter,
        IBinaryItem,
        IFormLinkContainerGetter,
        ILoquiObject<IMagicEffectArchetypeGetter>
    {
        static new ILoquiRegistration StaticRegistration => MagicEffectArchetype_Registration.Instance;
        MagicEffectArchetype.TypeEnum Type { get; }
        IFormLinkGetter<IStarfieldMajorRecordGetter> Association { get; }

    }

    #endregion

    #region Common MixIn
    public static partial class MagicEffectArchetypeMixIn
    {
        public static void Clear(this IMagicEffectArchetype item)
        {
            ((MagicEffectArchetypeSetterCommon)((IMagicEffectArchetypeGetter)item).CommonSetterInstance()!).Clear(item: item);
        }

        public static MagicEffectArchetype.Mask<bool> GetEqualsMask(
            this IMagicEffectArchetypeGetter item,
            IMagicEffectArchetypeGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            return ((MagicEffectArchetypeCommon)((IMagicEffectArchetypeGetter)item).CommonInstance()!).GetEqualsMask(
                item: item,
                rhs: rhs,
                include: include);
        }

        public static string Print(
            this IMagicEffectArchetypeGetter item,
            string? name = null,
            MagicEffectArchetype.Mask<bool>? printMask = null)
        {
            return ((MagicEffectArchetypeCommon)((IMagicEffectArchetypeGetter)item).CommonInstance()!).Print(
                item: item,
                name: name,
                printMask: printMask);
        }

        public static void Print(
            this IMagicEffectArchetypeGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            MagicEffectArchetype.Mask<bool>? printMask = null)
        {
            ((MagicEffectArchetypeCommon)((IMagicEffectArchetypeGetter)item).CommonInstance()!).Print(
                item: item,
                sb: sb,
                name: name,
                printMask: printMask);
        }

        public static bool Equals(
            this IMagicEffectArchetypeGetter item,
            IMagicEffectArchetypeGetter rhs,
            MagicEffectArchetype.TranslationMask? equalsMask = null)
        {
            return ((MagicEffectArchetypeCommon)((IMagicEffectArchetypeGetter)item).CommonInstance()!).Equals(
                lhs: item,
                rhs: rhs,
                equalsMask: equalsMask?.GetCrystal());
        }

        public static void DeepCopyIn(
            this IMagicEffectArchetype lhs,
            IMagicEffectArchetypeGetter rhs,
            out MagicEffectArchetype.ErrorMask errorMask,
            MagicEffectArchetype.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ((MagicEffectArchetypeSetterTranslationCommon)((IMagicEffectArchetypeGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
            errorMask = MagicEffectArchetype.ErrorMask.Factory(errorMaskBuilder);
        }

        public static void DeepCopyIn(
            this IMagicEffectArchetype lhs,
            IMagicEffectArchetypeGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
            ((MagicEffectArchetypeSetterTranslationCommon)((IMagicEffectArchetypeGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: false);
        }

        public static MagicEffectArchetype DeepCopy(
            this IMagicEffectArchetypeGetter item,
            MagicEffectArchetype.TranslationMask? copyMask = null)
        {
            return ((MagicEffectArchetypeSetterTranslationCommon)((IMagicEffectArchetypeGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask);
        }

        public static MagicEffectArchetype DeepCopy(
            this IMagicEffectArchetypeGetter item,
            out MagicEffectArchetype.ErrorMask errorMask,
            MagicEffectArchetype.TranslationMask? copyMask = null)
        {
            return ((MagicEffectArchetypeSetterTranslationCommon)((IMagicEffectArchetypeGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: out errorMask);
        }

        public static MagicEffectArchetype DeepCopy(
            this IMagicEffectArchetypeGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            return ((MagicEffectArchetypeSetterTranslationCommon)((IMagicEffectArchetypeGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: errorMask);
        }

        #region Binary Translation
        public static void CopyInFromBinary(
            this IMagicEffectArchetype item,
            MutagenFrame frame,
            TypedParseParams translationParams = default)
        {
            ((MagicEffectArchetypeSetterCommon)((IMagicEffectArchetypeGetter)item).CommonSetterInstance()!).CopyInFromBinary(
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
    internal enum MagicEffectArchetype_FieldIndex
    {
        Type = 0,
        Association = 1,
    }
    #endregion

    #region Registration
    internal partial class MagicEffectArchetype_Registration : ILoquiRegistration
    {
        public static readonly MagicEffectArchetype_Registration Instance = new MagicEffectArchetype_Registration();

        public static ProtocolKey ProtocolKey => ProtocolDefinition_Starfield.ProtocolKey;

        public const ushort AdditionalFieldCount = 2;

        public const ushort FieldCount = 2;

        public static readonly Type MaskType = typeof(MagicEffectArchetype.Mask<>);

        public static readonly Type ErrorMaskType = typeof(MagicEffectArchetype.ErrorMask);

        public static readonly Type ClassType = typeof(MagicEffectArchetype);

        public static readonly Type GetterType = typeof(IMagicEffectArchetypeGetter);

        public static readonly Type? InternalGetterType = null;

        public static readonly Type SetterType = typeof(IMagicEffectArchetype);

        public static readonly Type? InternalSetterType = null;

        public const string FullName = "Mutagen.Bethesda.Starfield.MagicEffectArchetype";

        public const string Name = "MagicEffectArchetype";

        public const string Namespace = "Mutagen.Bethesda.Starfield";

        public const byte GenericCount = 0;

        public static readonly Type? GenericRegistrationType = null;

        public static readonly Type BinaryWriteTranslation = typeof(MagicEffectArchetypeBinaryWriteTranslation);
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
    internal partial class MagicEffectArchetypeSetterCommon : AMagicEffectArchetypeSetterCommon
    {
        public new static readonly MagicEffectArchetypeSetterCommon Instance = new MagicEffectArchetypeSetterCommon();

        partial void ClearPartial();
        
        public void Clear(IMagicEffectArchetype item)
        {
            ClearPartial();
            item.Type = default;
            item.Association.Clear();
            base.Clear(item);
        }
        
        public override void Clear(IAMagicEffectArchetype item)
        {
            Clear(item: (IMagicEffectArchetype)item);
        }
        
        #region Mutagen
        public void RemapLinks(IMagicEffectArchetype obj, IReadOnlyDictionary<FormKey, FormKey> mapping)
        {
            base.RemapLinks(obj, mapping);
            obj.Association.Relink(mapping);
        }
        
        #endregion
        
        #region Binary Translation
        public virtual void CopyInFromBinary(
            IMagicEffectArchetype item,
            MutagenFrame frame,
            TypedParseParams translationParams)
        {
            PluginUtilityTranslation.SubrecordParse(
                record: item,
                frame: frame,
                translationParams: translationParams,
                fillStructs: MagicEffectArchetypeBinaryCreateTranslation.FillBinaryStructs);
        }
        
        public override void CopyInFromBinary(
            IAMagicEffectArchetype item,
            MutagenFrame frame,
            TypedParseParams translationParams)
        {
            CopyInFromBinary(
                item: (MagicEffectArchetype)item,
                frame: frame,
                translationParams: translationParams);
        }
        
        #endregion
        
    }
    internal partial class MagicEffectArchetypeCommon : AMagicEffectArchetypeCommon
    {
        public new static readonly MagicEffectArchetypeCommon Instance = new MagicEffectArchetypeCommon();

        public MagicEffectArchetype.Mask<bool> GetEqualsMask(
            IMagicEffectArchetypeGetter item,
            IMagicEffectArchetypeGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            var ret = new MagicEffectArchetype.Mask<bool>(false);
            ((MagicEffectArchetypeCommon)((IMagicEffectArchetypeGetter)item).CommonInstance()!).FillEqualsMask(
                item: item,
                rhs: rhs,
                ret: ret,
                include: include);
            return ret;
        }
        
        public void FillEqualsMask(
            IMagicEffectArchetypeGetter item,
            IMagicEffectArchetypeGetter rhs,
            MagicEffectArchetype.Mask<bool> ret,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            ret.Type = item.Type == rhs.Type;
            ret.Association = item.Association.Equals(rhs.Association);
            base.FillEqualsMask(item, rhs, ret, include);
        }
        
        public string Print(
            IMagicEffectArchetypeGetter item,
            string? name = null,
            MagicEffectArchetype.Mask<bool>? printMask = null)
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
            IMagicEffectArchetypeGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            MagicEffectArchetype.Mask<bool>? printMask = null)
        {
            if (name == null)
            {
                sb.AppendLine($"MagicEffectArchetype =>");
            }
            else
            {
                sb.AppendLine($"{name} (MagicEffectArchetype) =>");
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
            IMagicEffectArchetypeGetter item,
            StructuredStringBuilder sb,
            MagicEffectArchetype.Mask<bool>? printMask = null)
        {
            AMagicEffectArchetypeCommon.ToStringFields(
                item: item,
                sb: sb,
                printMask: printMask);
            if (printMask?.Type ?? true)
            {
                sb.AppendItem(item.Type, "Type");
            }
            if (printMask?.Association ?? true)
            {
                sb.AppendItem(item.Association.FormKey, "Association");
            }
        }
        
        public static MagicEffectArchetype_FieldIndex ConvertFieldIndex(AMagicEffectArchetype_FieldIndex index)
        {
            switch (index)
            {
                default:
                    throw new ArgumentException($"Index is out of range: {index.ToStringFast()}");
            }
        }
        
        #region Equals and Hash
        public virtual bool Equals(
            IMagicEffectArchetypeGetter? lhs,
            IMagicEffectArchetypeGetter? rhs,
            TranslationCrystal? equalsMask)
        {
            if (!EqualsMaskHelper.RefEquality(lhs, rhs, out var isEqual)) return isEqual;
            if (!base.Equals((IAMagicEffectArchetypeGetter)lhs, (IAMagicEffectArchetypeGetter)rhs, equalsMask)) return false;
            if ((equalsMask?.GetShouldTranslate((int)MagicEffectArchetype_FieldIndex.Type) ?? true))
            {
                if (lhs.Type != rhs.Type) return false;
            }
            if ((equalsMask?.GetShouldTranslate((int)MagicEffectArchetype_FieldIndex.Association) ?? true))
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
                lhs: (IMagicEffectArchetypeGetter?)lhs,
                rhs: rhs as IMagicEffectArchetypeGetter,
                equalsMask: equalsMask);
        }
        
        public virtual int GetHashCode(IMagicEffectArchetypeGetter item)
        {
            var hash = new HashCode();
            hash.Add(item.Type);
            hash.Add(item.Association);
            hash.Add(base.GetHashCode());
            return hash.ToHashCode();
        }
        
        public override int GetHashCode(IAMagicEffectArchetypeGetter item)
        {
            return GetHashCode(item: (IMagicEffectArchetypeGetter)item);
        }
        
        #endregion
        
        
        public override object GetNew()
        {
            return MagicEffectArchetype.GetNew();
        }
        
        #region Mutagen
        public IEnumerable<IFormLinkGetter> EnumerateFormLinks(IMagicEffectArchetypeGetter obj)
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
    internal partial class MagicEffectArchetypeSetterTranslationCommon : AMagicEffectArchetypeSetterTranslationCommon
    {
        public new static readonly MagicEffectArchetypeSetterTranslationCommon Instance = new MagicEffectArchetypeSetterTranslationCommon();

        #region DeepCopyIn
        public void DeepCopyIn(
            IMagicEffectArchetype item,
            IMagicEffectArchetypeGetter rhs,
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
            if ((copyMask?.GetShouldTranslate((int)MagicEffectArchetype_FieldIndex.Type) ?? true))
            {
                item.Type = rhs.Type;
            }
            if ((copyMask?.GetShouldTranslate((int)MagicEffectArchetype_FieldIndex.Association) ?? true))
            {
                item.Association.SetTo(rhs.Association.FormKey);
            }
        }
        
        
        public override void DeepCopyIn(
            IAMagicEffectArchetype item,
            IAMagicEffectArchetypeGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy)
        {
            this.DeepCopyIn(
                item: (IMagicEffectArchetype)item,
                rhs: (IMagicEffectArchetypeGetter)rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: deepCopy);
        }
        
        #endregion
        
        public MagicEffectArchetype DeepCopy(
            IMagicEffectArchetypeGetter item,
            MagicEffectArchetype.TranslationMask? copyMask = null)
        {
            MagicEffectArchetype ret = (MagicEffectArchetype)((MagicEffectArchetypeCommon)((IMagicEffectArchetypeGetter)item).CommonInstance()!).GetNew();
            ((MagicEffectArchetypeSetterTranslationCommon)((IMagicEffectArchetypeGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: null,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            return ret;
        }
        
        public MagicEffectArchetype DeepCopy(
            IMagicEffectArchetypeGetter item,
            out MagicEffectArchetype.ErrorMask errorMask,
            MagicEffectArchetype.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            MagicEffectArchetype ret = (MagicEffectArchetype)((MagicEffectArchetypeCommon)((IMagicEffectArchetypeGetter)item).CommonInstance()!).GetNew();
            ((MagicEffectArchetypeSetterTranslationCommon)((IMagicEffectArchetypeGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                ret,
                item,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            errorMask = MagicEffectArchetype.ErrorMask.Factory(errorMaskBuilder);
            return ret;
        }
        
        public MagicEffectArchetype DeepCopy(
            IMagicEffectArchetypeGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            MagicEffectArchetype ret = (MagicEffectArchetype)((MagicEffectArchetypeCommon)((IMagicEffectArchetypeGetter)item).CommonInstance()!).GetNew();
            ((MagicEffectArchetypeSetterTranslationCommon)((IMagicEffectArchetypeGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
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
    public partial class MagicEffectArchetype
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => MagicEffectArchetype_Registration.Instance;
        public new static ILoquiRegistration StaticRegistration => MagicEffectArchetype_Registration.Instance;
        [DebuggerStepThrough]
        protected override object CommonInstance() => MagicEffectArchetypeCommon.Instance;
        [DebuggerStepThrough]
        protected override object CommonSetterInstance()
        {
            return MagicEffectArchetypeSetterCommon.Instance;
        }
        [DebuggerStepThrough]
        protected override object CommonSetterTranslationInstance() => MagicEffectArchetypeSetterTranslationCommon.Instance;

        #endregion

    }
}

#region Modules
#region Binary Translation
namespace Mutagen.Bethesda.Starfield
{
    public partial class MagicEffectArchetypeBinaryWriteTranslation :
        AMagicEffectArchetypeBinaryWriteTranslation,
        IBinaryWriteTranslator
    {
        public new static readonly MagicEffectArchetypeBinaryWriteTranslation Instance = new();

        public static void WriteEmbedded(
            IMagicEffectArchetypeGetter item,
            MutagenWriter writer)
        {
        }

        public void Write(
            MutagenWriter writer,
            IMagicEffectArchetypeGetter item,
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
                item: (IMagicEffectArchetypeGetter)item,
                writer: writer,
                translationParams: translationParams);
        }

        public override void Write(
            MutagenWriter writer,
            IAMagicEffectArchetypeGetter item,
            TypedWriteParams translationParams)
        {
            Write(
                item: (IMagicEffectArchetypeGetter)item,
                writer: writer,
                translationParams: translationParams);
        }

    }

    internal partial class MagicEffectArchetypeBinaryCreateTranslation : AMagicEffectArchetypeBinaryCreateTranslation
    {
        public new static readonly MagicEffectArchetypeBinaryCreateTranslation Instance = new MagicEffectArchetypeBinaryCreateTranslation();

        public static void FillBinaryStructs(
            IMagicEffectArchetype item,
            MutagenFrame frame)
        {
        }

    }

}
namespace Mutagen.Bethesda.Starfield
{
    #region Binary Write Mixins
    public static class MagicEffectArchetypeBinaryTranslationMixIn
    {
    }
    #endregion


}
namespace Mutagen.Bethesda.Starfield
{
}
#endregion

#endregion

