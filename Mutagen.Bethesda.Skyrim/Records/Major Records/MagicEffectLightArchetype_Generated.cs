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
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using RecordTypeInts = Mutagen.Bethesda.Skyrim.Internals.RecordTypeInts;
using RecordTypes = Mutagen.Bethesda.Skyrim.Internals.RecordTypes;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
#endregion

#nullable enable
namespace Mutagen.Bethesda.Skyrim
{
    #region Class
    public partial class MagicEffectLightArchetype :
        AMagicEffectArchetype,
        IEquatable<IMagicEffectLightArchetypeGetter>,
        ILoquiObjectSetter<MagicEffectLightArchetype>,
        IMagicEffectLightArchetype
    {
        #region Association
        private readonly IFormLink<ILightGetter> _Association = new FormLink<ILightGetter>();
        public IFormLink<ILightGetter> Association
        {
            get => _Association;
            set => _Association.SetTo(value);
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IFormLinkGetter<ILightGetter> IMagicEffectLightArchetypeGetter.Association => this.Association;
        #endregion

        #region To String

        public override void Print(
            StructuredStringBuilder sb,
            string? name = null)
        {
            MagicEffectLightArchetypeMixIn.Print(
                item: this,
                sb: sb,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IMagicEffectLightArchetypeGetter rhs) return false;
            return ((MagicEffectLightArchetypeCommon)((IMagicEffectLightArchetypeGetter)this).CommonInstance()!).Equals(this, rhs, equalsMask: null);
        }

        public bool Equals(IMagicEffectLightArchetypeGetter? obj)
        {
            return ((MagicEffectLightArchetypeCommon)((IMagicEffectLightArchetypeGetter)this).CommonInstance()!).Equals(this, obj, equalsMask: null);
        }

        public override int GetHashCode() => ((MagicEffectLightArchetypeCommon)((IMagicEffectLightArchetypeGetter)this).CommonInstance()!).GetHashCode(this);

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
                this.Association = initialValue;
            }

            public Mask(
                TItem ActorValue,
                TItem Association)
            : base(ActorValue: ActorValue)
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
                var ret = new MagicEffectLightArchetype.Mask<R>();
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

            public string Print(MagicEffectLightArchetype.Mask<bool>? printMask = null)
            {
                var sb = new StructuredStringBuilder();
                Print(sb, printMask);
                return sb.ToString();
            }

            public void Print(StructuredStringBuilder sb, MagicEffectLightArchetype.Mask<bool>? printMask = null)
            {
                sb.AppendLine($"{nameof(MagicEffectLightArchetype.Mask<TItem>)} =>");
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
                MagicEffectLightArchetype_FieldIndex enu = (MagicEffectLightArchetype_FieldIndex)index;
                switch (enu)
                {
                    case MagicEffectLightArchetype_FieldIndex.Association:
                        return Association;
                    default:
                        return base.GetNthMask(index);
                }
            }

            public override void SetNthException(int index, Exception ex)
            {
                MagicEffectLightArchetype_FieldIndex enu = (MagicEffectLightArchetype_FieldIndex)index;
                switch (enu)
                {
                    case MagicEffectLightArchetype_FieldIndex.Association:
                        this.Association = ex;
                        break;
                    default:
                        base.SetNthException(index, ex);
                        break;
                }
            }

            public override void SetNthMask(int index, object obj)
            {
                MagicEffectLightArchetype_FieldIndex enu = (MagicEffectLightArchetype_FieldIndex)index;
                switch (enu)
                {
                    case MagicEffectLightArchetype_FieldIndex.Association:
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
        public override IEnumerable<IFormLinkGetter> EnumerateFormLinks() => MagicEffectLightArchetypeCommon.Instance.EnumerateFormLinks(this);
        public override void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping) => MagicEffectLightArchetypeSetterCommon.Instance.RemapLinks(this, mapping);
        #endregion

        #region Binary Translation
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override object BinaryWriteTranslator => MagicEffectLightArchetypeBinaryWriteTranslation.Instance;
        void IBinaryItem.WriteToBinary(
            MutagenWriter writer,
            TypedWriteParams translationParams = default)
        {
            ((MagicEffectLightArchetypeBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
                item: this,
                writer: writer,
                translationParams: translationParams);
        }
        #region Binary Create
        public new static MagicEffectLightArchetype CreateFromBinary(
            MutagenFrame frame,
            TypedParseParams translationParams = default)
        {
            var ret = new MagicEffectLightArchetype();
            ((MagicEffectLightArchetypeSetterCommon)((IMagicEffectLightArchetypeGetter)ret).CommonSetterInstance()!).CopyInFromBinary(
                item: ret,
                frame: frame,
                translationParams: translationParams);
            return ret;
        }

        #endregion

        public static bool TryCreateFromBinary(
            MutagenFrame frame,
            out MagicEffectLightArchetype item,
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
            ((MagicEffectLightArchetypeSetterCommon)((IMagicEffectLightArchetypeGetter)this).CommonSetterInstance()!).Clear(this);
        }

        internal static new MagicEffectLightArchetype GetNew()
        {
            return new MagicEffectLightArchetype();
        }

    }
    #endregion

    #region Interface
    public partial interface IMagicEffectLightArchetype :
        IAMagicEffectArchetype,
        IFormLinkContainer,
        ILoquiObjectSetter<IMagicEffectLightArchetype>,
        IMagicEffectLightArchetypeGetter
    {
        new IFormLink<ILightGetter> Association { get; set; }
    }

    public partial interface IMagicEffectLightArchetypeGetter :
        IAMagicEffectArchetypeGetter,
        IBinaryItem,
        IFormLinkContainerGetter,
        ILoquiObject<IMagicEffectLightArchetypeGetter>
    {
        static new ILoquiRegistration StaticRegistration => MagicEffectLightArchetype_Registration.Instance;
        IFormLinkGetter<ILightGetter> Association { get; }

    }

    #endregion

    #region Common MixIn
    public static partial class MagicEffectLightArchetypeMixIn
    {
        public static void Clear(this IMagicEffectLightArchetype item)
        {
            ((MagicEffectLightArchetypeSetterCommon)((IMagicEffectLightArchetypeGetter)item).CommonSetterInstance()!).Clear(item: item);
        }

        public static MagicEffectLightArchetype.Mask<bool> GetEqualsMask(
            this IMagicEffectLightArchetypeGetter item,
            IMagicEffectLightArchetypeGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            return ((MagicEffectLightArchetypeCommon)((IMagicEffectLightArchetypeGetter)item).CommonInstance()!).GetEqualsMask(
                item: item,
                rhs: rhs,
                include: include);
        }

        public static string Print(
            this IMagicEffectLightArchetypeGetter item,
            string? name = null,
            MagicEffectLightArchetype.Mask<bool>? printMask = null)
        {
            return ((MagicEffectLightArchetypeCommon)((IMagicEffectLightArchetypeGetter)item).CommonInstance()!).Print(
                item: item,
                name: name,
                printMask: printMask);
        }

        public static void Print(
            this IMagicEffectLightArchetypeGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            MagicEffectLightArchetype.Mask<bool>? printMask = null)
        {
            ((MagicEffectLightArchetypeCommon)((IMagicEffectLightArchetypeGetter)item).CommonInstance()!).Print(
                item: item,
                sb: sb,
                name: name,
                printMask: printMask);
        }

        public static bool Equals(
            this IMagicEffectLightArchetypeGetter item,
            IMagicEffectLightArchetypeGetter rhs,
            MagicEffectLightArchetype.TranslationMask? equalsMask = null)
        {
            return ((MagicEffectLightArchetypeCommon)((IMagicEffectLightArchetypeGetter)item).CommonInstance()!).Equals(
                lhs: item,
                rhs: rhs,
                equalsMask: equalsMask?.GetCrystal());
        }

        public static void DeepCopyIn(
            this IMagicEffectLightArchetype lhs,
            IMagicEffectLightArchetypeGetter rhs,
            out MagicEffectLightArchetype.ErrorMask errorMask,
            MagicEffectLightArchetype.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ((MagicEffectLightArchetypeSetterTranslationCommon)((IMagicEffectLightArchetypeGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
            errorMask = MagicEffectLightArchetype.ErrorMask.Factory(errorMaskBuilder);
        }

        public static void DeepCopyIn(
            this IMagicEffectLightArchetype lhs,
            IMagicEffectLightArchetypeGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
            ((MagicEffectLightArchetypeSetterTranslationCommon)((IMagicEffectLightArchetypeGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: false);
        }

        public static MagicEffectLightArchetype DeepCopy(
            this IMagicEffectLightArchetypeGetter item,
            MagicEffectLightArchetype.TranslationMask? copyMask = null)
        {
            return ((MagicEffectLightArchetypeSetterTranslationCommon)((IMagicEffectLightArchetypeGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask);
        }

        public static MagicEffectLightArchetype DeepCopy(
            this IMagicEffectLightArchetypeGetter item,
            out MagicEffectLightArchetype.ErrorMask errorMask,
            MagicEffectLightArchetype.TranslationMask? copyMask = null)
        {
            return ((MagicEffectLightArchetypeSetterTranslationCommon)((IMagicEffectLightArchetypeGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: out errorMask);
        }

        public static MagicEffectLightArchetype DeepCopy(
            this IMagicEffectLightArchetypeGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            return ((MagicEffectLightArchetypeSetterTranslationCommon)((IMagicEffectLightArchetypeGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: errorMask);
        }

        #region Binary Translation
        public static void CopyInFromBinary(
            this IMagicEffectLightArchetype item,
            MutagenFrame frame,
            TypedParseParams translationParams = default)
        {
            ((MagicEffectLightArchetypeSetterCommon)((IMagicEffectLightArchetypeGetter)item).CommonSetterInstance()!).CopyInFromBinary(
                item: item,
                frame: frame,
                translationParams: translationParams);
        }

        #endregion

    }
    #endregion

}

namespace Mutagen.Bethesda.Skyrim
{
    #region Field Index
    internal enum MagicEffectLightArchetype_FieldIndex
    {
        ActorValue = 0,
        Association = 1,
    }
    #endregion

    #region Registration
    internal partial class MagicEffectLightArchetype_Registration : ILoquiRegistration
    {
        public static readonly MagicEffectLightArchetype_Registration Instance = new MagicEffectLightArchetype_Registration();

        public static ProtocolKey ProtocolKey => ProtocolDefinition_Skyrim.ProtocolKey;

        public const ushort AdditionalFieldCount = 1;

        public const ushort FieldCount = 2;

        public static readonly Type MaskType = typeof(MagicEffectLightArchetype.Mask<>);

        public static readonly Type ErrorMaskType = typeof(MagicEffectLightArchetype.ErrorMask);

        public static readonly Type ClassType = typeof(MagicEffectLightArchetype);

        public static readonly Type GetterType = typeof(IMagicEffectLightArchetypeGetter);

        public static readonly Type? InternalGetterType = null;

        public static readonly Type SetterType = typeof(IMagicEffectLightArchetype);

        public static readonly Type? InternalSetterType = null;

        public const string FullName = "Mutagen.Bethesda.Skyrim.MagicEffectLightArchetype";

        public const string Name = "MagicEffectLightArchetype";

        public const string Namespace = "Mutagen.Bethesda.Skyrim";

        public const byte GenericCount = 0;

        public static readonly Type? GenericRegistrationType = null;

        public static readonly Type BinaryWriteTranslation = typeof(MagicEffectLightArchetypeBinaryWriteTranslation);
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
    internal partial class MagicEffectLightArchetypeSetterCommon : AMagicEffectArchetypeSetterCommon
    {
        public new static readonly MagicEffectLightArchetypeSetterCommon Instance = new MagicEffectLightArchetypeSetterCommon();

        partial void ClearPartial();
        
        public void Clear(IMagicEffectLightArchetype item)
        {
            ClearPartial();
            item.Association.Clear();
            base.Clear(item);
        }
        
        public override void Clear(IAMagicEffectArchetype item)
        {
            Clear(item: (IMagicEffectLightArchetype)item);
        }
        
        #region Mutagen
        public void RemapLinks(IMagicEffectLightArchetype obj, IReadOnlyDictionary<FormKey, FormKey> mapping)
        {
            base.RemapLinks(obj, mapping);
            obj.Association.Relink(mapping);
        }
        
        #endregion
        
        #region Binary Translation
        public virtual void CopyInFromBinary(
            IMagicEffectLightArchetype item,
            MutagenFrame frame,
            TypedParseParams translationParams)
        {
            PluginUtilityTranslation.SubrecordParse(
                record: item,
                frame: frame,
                translationParams: translationParams,
                fillStructs: MagicEffectLightArchetypeBinaryCreateTranslation.FillBinaryStructs);
        }
        
        public override void CopyInFromBinary(
            IAMagicEffectArchetype item,
            MutagenFrame frame,
            TypedParseParams translationParams)
        {
            CopyInFromBinary(
                item: (MagicEffectLightArchetype)item,
                frame: frame,
                translationParams: translationParams);
        }
        
        #endregion
        
    }
    internal partial class MagicEffectLightArchetypeCommon : AMagicEffectArchetypeCommon
    {
        public new static readonly MagicEffectLightArchetypeCommon Instance = new MagicEffectLightArchetypeCommon();

        public MagicEffectLightArchetype.Mask<bool> GetEqualsMask(
            IMagicEffectLightArchetypeGetter item,
            IMagicEffectLightArchetypeGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            var ret = new MagicEffectLightArchetype.Mask<bool>(false);
            ((MagicEffectLightArchetypeCommon)((IMagicEffectLightArchetypeGetter)item).CommonInstance()!).FillEqualsMask(
                item: item,
                rhs: rhs,
                ret: ret,
                include: include);
            return ret;
        }
        
        public void FillEqualsMask(
            IMagicEffectLightArchetypeGetter item,
            IMagicEffectLightArchetypeGetter rhs,
            MagicEffectLightArchetype.Mask<bool> ret,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            ret.Association = item.Association.Equals(rhs.Association);
            base.FillEqualsMask(item, rhs, ret, include);
        }
        
        public string Print(
            IMagicEffectLightArchetypeGetter item,
            string? name = null,
            MagicEffectLightArchetype.Mask<bool>? printMask = null)
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
            IMagicEffectLightArchetypeGetter item,
            StructuredStringBuilder sb,
            string? name = null,
            MagicEffectLightArchetype.Mask<bool>? printMask = null)
        {
            if (name == null)
            {
                sb.AppendLine($"MagicEffectLightArchetype =>");
            }
            else
            {
                sb.AppendLine($"{name} (MagicEffectLightArchetype) =>");
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
            IMagicEffectLightArchetypeGetter item,
            StructuredStringBuilder sb,
            MagicEffectLightArchetype.Mask<bool>? printMask = null)
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
        
        public static MagicEffectLightArchetype_FieldIndex ConvertFieldIndex(AMagicEffectArchetype_FieldIndex index)
        {
            switch (index)
            {
                case AMagicEffectArchetype_FieldIndex.ActorValue:
                    return (MagicEffectLightArchetype_FieldIndex)((int)index);
                default:
                    throw new ArgumentException($"Index is out of range: {index.ToStringFast()}");
            }
        }
        
        #region Equals and Hash
        public virtual bool Equals(
            IMagicEffectLightArchetypeGetter? lhs,
            IMagicEffectLightArchetypeGetter? rhs,
            TranslationCrystal? equalsMask)
        {
            if (!EqualsMaskHelper.RefEquality(lhs, rhs, out var isEqual)) return isEqual;
            if (!base.Equals((IAMagicEffectArchetypeGetter)lhs, (IAMagicEffectArchetypeGetter)rhs, equalsMask)) return false;
            if ((equalsMask?.GetShouldTranslate((int)MagicEffectLightArchetype_FieldIndex.Association) ?? true))
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
                lhs: (IMagicEffectLightArchetypeGetter?)lhs,
                rhs: rhs as IMagicEffectLightArchetypeGetter,
                equalsMask: equalsMask);
        }
        
        public virtual int GetHashCode(IMagicEffectLightArchetypeGetter item)
        {
            var hash = new HashCode();
            hash.Add(item.Association);
            hash.Add(base.GetHashCode());
            return hash.ToHashCode();
        }
        
        public override int GetHashCode(IAMagicEffectArchetypeGetter item)
        {
            return GetHashCode(item: (IMagicEffectLightArchetypeGetter)item);
        }
        
        #endregion
        
        
        public override object GetNew()
        {
            return MagicEffectLightArchetype.GetNew();
        }
        
        #region Mutagen
        public IEnumerable<IFormLinkGetter> EnumerateFormLinks(IMagicEffectLightArchetypeGetter obj)
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
    internal partial class MagicEffectLightArchetypeSetterTranslationCommon : AMagicEffectArchetypeSetterTranslationCommon
    {
        public new static readonly MagicEffectLightArchetypeSetterTranslationCommon Instance = new MagicEffectLightArchetypeSetterTranslationCommon();

        #region DeepCopyIn
        public void DeepCopyIn(
            IMagicEffectLightArchetype item,
            IMagicEffectLightArchetypeGetter rhs,
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
            if ((copyMask?.GetShouldTranslate((int)MagicEffectLightArchetype_FieldIndex.Association) ?? true))
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
            IMagicEffectLightArchetype item,
            IMagicEffectLightArchetypeGetter rhs,
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
                item: (IMagicEffectLightArchetype)item,
                rhs: (IMagicEffectLightArchetypeGetter)rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: deepCopy);
        }
        
        #endregion
        
        public MagicEffectLightArchetype DeepCopy(
            IMagicEffectLightArchetypeGetter item,
            MagicEffectLightArchetype.TranslationMask? copyMask = null)
        {
            MagicEffectLightArchetype ret = (MagicEffectLightArchetype)((MagicEffectLightArchetypeCommon)((IMagicEffectLightArchetypeGetter)item).CommonInstance()!).GetNew();
            ((MagicEffectLightArchetypeSetterTranslationCommon)((IMagicEffectLightArchetypeGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: null,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            return ret;
        }
        
        public MagicEffectLightArchetype DeepCopy(
            IMagicEffectLightArchetypeGetter item,
            out MagicEffectLightArchetype.ErrorMask errorMask,
            MagicEffectLightArchetype.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            MagicEffectLightArchetype ret = (MagicEffectLightArchetype)((MagicEffectLightArchetypeCommon)((IMagicEffectLightArchetypeGetter)item).CommonInstance()!).GetNew();
            ((MagicEffectLightArchetypeSetterTranslationCommon)((IMagicEffectLightArchetypeGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                ret,
                item,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            errorMask = MagicEffectLightArchetype.ErrorMask.Factory(errorMaskBuilder);
            return ret;
        }
        
        public MagicEffectLightArchetype DeepCopy(
            IMagicEffectLightArchetypeGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            MagicEffectLightArchetype ret = (MagicEffectLightArchetype)((MagicEffectLightArchetypeCommon)((IMagicEffectLightArchetypeGetter)item).CommonInstance()!).GetNew();
            ((MagicEffectLightArchetypeSetterTranslationCommon)((IMagicEffectLightArchetypeGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
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

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectLightArchetype
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => MagicEffectLightArchetype_Registration.Instance;
        public new static ILoquiRegistration StaticRegistration => MagicEffectLightArchetype_Registration.Instance;
        [DebuggerStepThrough]
        protected override object CommonInstance() => MagicEffectLightArchetypeCommon.Instance;
        [DebuggerStepThrough]
        protected override object CommonSetterInstance()
        {
            return MagicEffectLightArchetypeSetterCommon.Instance;
        }
        [DebuggerStepThrough]
        protected override object CommonSetterTranslationInstance() => MagicEffectLightArchetypeSetterTranslationCommon.Instance;

        #endregion

    }
}

#region Modules
#region Binary Translation
namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectLightArchetypeBinaryWriteTranslation :
        AMagicEffectArchetypeBinaryWriteTranslation,
        IBinaryWriteTranslator
    {
        public new static readonly MagicEffectLightArchetypeBinaryWriteTranslation Instance = new();

        public static void WriteEmbedded(
            IMagicEffectLightArchetypeGetter item,
            MutagenWriter writer)
        {
            AMagicEffectArchetypeBinaryWriteTranslation.WriteEmbedded(
                item: item,
                writer: writer);
        }

        public void Write(
            MutagenWriter writer,
            IMagicEffectLightArchetypeGetter item,
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
                item: (IMagicEffectLightArchetypeGetter)item,
                writer: writer,
                translationParams: translationParams);
        }

        public override void Write(
            MutagenWriter writer,
            IAMagicEffectArchetypeGetter item,
            TypedWriteParams translationParams)
        {
            Write(
                item: (IMagicEffectLightArchetypeGetter)item,
                writer: writer,
                translationParams: translationParams);
        }

    }

    internal partial class MagicEffectLightArchetypeBinaryCreateTranslation : AMagicEffectArchetypeBinaryCreateTranslation
    {
        public new static readonly MagicEffectLightArchetypeBinaryCreateTranslation Instance = new MagicEffectLightArchetypeBinaryCreateTranslation();

        public static void FillBinaryStructs(
            IMagicEffectLightArchetype item,
            MutagenFrame frame)
        {
            AMagicEffectArchetypeBinaryCreateTranslation.FillBinaryStructs(
                item: item,
                frame: frame);
        }

    }

}
namespace Mutagen.Bethesda.Skyrim
{
    #region Binary Write Mixins
    public static class MagicEffectLightArchetypeBinaryTranslationMixIn
    {
    }
    #endregion


}
namespace Mutagen.Bethesda.Skyrim
{
}
#endregion

#endregion

