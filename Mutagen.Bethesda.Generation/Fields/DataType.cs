using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using System.Xml.Linq;
using Mutagen.Bethesda.Generation;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class DataType : SetMarkerType
    {
        public const string BREAK = "Break";
        public const string RANGE = "Range";
        public const string MIN = "Min";
        public List<int> BreakIndices = new List<int>();
        public List<(RangeInt32 FieldIndexRange, int DataSetSizeMin)> RangeIndices = new List<(RangeInt32 FieldIndexRange, int DataSetSizeMin)>();
        public bool HasCustomLogic => this.BreakIndices.Count > 0 || this.RangeIndices.Count > 0;
        public string EnumName => $"{this.GetFieldData().RecordType.Value.Type}DataType";
        public string StateName => $"{this.EnumName}State";

        public override async Task Load(XElement node, bool requireName = true)
        {
            this.Node = node;
            var data = this.CustomData.TryCreateValue(Mutagen.Bethesda.Generation.Constants.DATA_KEY, () => new MutagenFieldData(this)) as MutagenFieldData;
            if (node.TryGetAttribute("recordType", out var recType))
            {
                data.RecordType = new RecordType(recType.Value);
            }
            else
            {
                data.RecordType = new RecordType("DATA");
            }
            var fieldsNode = node.Element(XName.Get(Loqui.Generation.Constants.FIELDS, LoquiGenerator.Namespace));
            if (fieldsNode != null)
            {
                TryGet<TypeGeneration> typeGen;
                foreach (var fieldNode in fieldsNode.Elements())
                {
                    if (fieldNode.Name.LocalName.Equals(BREAK))
                    {
                        BreakIndices.Add(this.SubFields.Count);
                        continue;
                    }
                    if (fieldNode.Name.LocalName.Equals(RANGE))
                    {
                        var curIndex = this.SubFields.Count;
                        foreach (var subFieldNode in fieldNode.Elements())
                        {
                            typeGen = await this.ObjectGen.LoadField(subFieldNode, true);
                            if (typeGen.Succeeded)
                            {
                                this.SubFields.Add(typeGen.Value);
                            }
                        }
                        this.RangeIndices.Add(
                            (new RangeInt32(
                                curIndex,
                                this.SubFields.Count - 1),
                            fieldNode.GetAttribute<int>(MIN, throwException: true)));
                        continue;
                    }

                    typeGen = await this.ObjectGen.LoadField(fieldNode, true);
                    if (typeGen.Succeeded)
                    {
                        typeGen.Value.HasBeenSetProperty.Item = false;
                        this.SubFields.Add(typeGen.Value);
                    }
                }
            }
        }
    }
}
