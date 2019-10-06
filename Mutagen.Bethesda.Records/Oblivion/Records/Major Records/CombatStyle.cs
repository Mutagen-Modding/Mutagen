using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class CombatStyle
    {
        [Flags]
        public enum Flag
        {
            Advanced = 0x0001,
            ChooseAttackUsingPercentChance = 0x0002,
            IgnoreAlliesInArea = 0x0004,
            WillYield = 0x0008,
            RejectsYields = 0x0010,
            FleeingDisabled = 0x0020,
            PrefersRanged = 0x0040,
            MeleeAlertOK = 0x0080,
            DoNotAcquire = 0x0100,
        }

        public static async Task<CombatStyle> CreateFromXmlFolder(
            XElement node,
            string path,
            ErrorMaskBuilder errorMask,
            TranslationCrystal translationMask)
        {
            var ret = CreateFromXml(
                node,
                errorMask: errorMask,
                translationMask: translationMask);
            var customEndingNode = node.Element("CustomEndingFlags");
            if (customEndingNode == null
                || !customEndingNode.TryGetAttribute<bool>("value", out var val)
                || val)
            {
                ret.CSTDDataTypeState &= ~CSTDDataType.Break4;
            }
            return ret;
        }

        public override async Task WriteToXmlFolder(
            DirectoryPath? dir,
            string name,
            XElement node,
            int counter,
            ErrorMaskBuilder errorMask)
        {
            var elem = new XElement(name ?? "Mutagen.Bethesda.Oblivion.CombatStyle");
            node.Add(elem);
            if (name != null)
            {
                elem.SetAttributeValue("type", "Mutagen.Bethesda.Oblivion.CombatStyle");
            }
            CombatStyleXmlWriteTranslation.WriteToNodeXml(
                item: this,
                node: elem,
                errorMask: errorMask,
                translationMask: null);
            if (!this.CSTDDataTypeState.HasFlag(CSTDDataType.Break4)) return;
            elem.Add(new XElement("CustomEndingFlags", new XAttribute("value", "false")));
        }
    }

    namespace Internals
    {
        public partial class CombatStyleBinaryCreateTranslation
        {
            static partial void FillBinarySecondaryFlagsCustom(MutagenFrame frame, CombatStyle item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                int flags = frame.ReadInt32();
                var otherFlag = (CombatStyle.Flag)(flags << 8);
                item.Flags = item.Flags | otherFlag;
            }
        }

        public partial class CombatStyleBinaryWriteTranslation
        {
            static partial void WriteBinarySecondaryFlagsCustom(MutagenWriter writer, ICombatStyleGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                int flags = (int)item.Flags;
                flags = flags >> 8;
                writer.Write(flags);
            }
        }

        public partial class CombatStyleBinaryWrapper
        {
            private bool GetFlagsIsSetCustom() => _CSTDLocation.HasValue;
            private CombatStyle.Flag GetFlagsCustom()
            {
                if (!_Flags_IsSet) return default;
                var ret = (CombatStyle.Flag)_data.Span.Slice(_FlagsLocation, 1)[0];
                if (!this.CSTDDataTypeState.HasFlag(CombatStyle.CSTDDataType.Break4))
                {
                    int flags = BinaryPrimitives.ReadInt32LittleEndian(_data.Span.Slice(_CSTDLocation.Value + 0x78));
                    var otherFlag = (CombatStyle.Flag)(flags << 8);
                    ret |= otherFlag;
                }
                return ret;
            }
        }
    }
}
