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
        public List<DataTypeRange> RangeIndices = new List<DataTypeRange>();
        public bool HasStateLogic => this.BreakIndices.Count > 0 || this.RangeIndices.Count > 0;
        public string EnumName => $"{this.GetFieldData().RecordType.Value.Type}DataType";
        public string StateName => $"{this.EnumName}State";

        public class DataTypeRange
        {
            public RangeInt32 Range { get; private set; }
            public int DataSetSizeMin { get; private set; }

            public DataTypeRange(
                RangeInt32 range,
                int dataSetSizeMin)
            {
                this.Range = range;
                this.DataSetSizeMin = dataSetSizeMin;
            }
        }

        public class DataTypeIteration
        {
            public int FieldIndex;
            public TypeGeneration Field;
            public int BreakIndex;
            public IEnumerable<int> EncounteredBreaks;
            public int RangeIndex;
            public DataTypeRange Range;
        }

        public override async Task Load(XElement node, bool requireName = true)
        {
            this.Node = node;
            var data = this.CustomData.TryCreateValue(Mutagen.Bethesda.Generation.Constants.DataKey, () => new MutagenFieldData(this)) as MutagenFieldData;
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
                            new DataTypeRange(
                                new RangeInt32(
                                    curIndex,
                                    this.SubFields.Count - 1),
                                fieldNode.GetAttribute<int>(MIN, throwException: true)));
                        continue;
                    }

                    typeGen = await this.ObjectGen.LoadField(fieldNode, true);
                    if (typeGen.Succeeded)
                    {
                        typeGen.Value.HasBeenSetProperty.Item = false;
                        if (typeGen.Value is LoquiType loqui
                            && loqui.SingletonType == SingletonLevel.None)
                        {
                            loqui.SingletonType = SingletonLevel.NotNull;
                        }
                        this.SubFields.Add(typeGen.Value);
                    }
                }

                foreach (var subField in this.IterateFieldsWithMeta())
                {
                    if (subField.Field is TypicalTypeGeneration typical)
                    {
                        typical.PreSetEvent += (fg) =>
                        {
                            fg.AppendLine($"this.{this.StateName} |= {this.EnumName}.Has;");
                            foreach (var b in subField.EncounteredBreaks)
                            {
                                fg.AppendLine($"this.{this.StateName} &= ~{this.EnumName}.Break{b};");
                            }
                            if (subField.Range != null)
                            {
                                fg.AppendLine($"this.{this.StateName} |= {this.EnumName}.Range{subField.RangeIndex};");
                            }
                        };
                    }
                }
            }
            this.HasBeenSetProperty.Set(false);
        }

        public IEnumerable<DataTypeIteration> IterateFieldsWithMeta()
        {
            HashSet<int> encounteredBreaks = new HashSet<int>();
            for (int i = 0; i < this.SubFields.Count; i++)
            {
                var breakIndex = this.BreakIndices.IndexOf(i);
                if (breakIndex != -1)
                {
                    encounteredBreaks.Add(breakIndex);
                }
                DataTypeRange range = null;
                var rangeIndex = this.RangeIndices.FindIndex((r) => r.Range.IsInRange(i));
                if (rangeIndex != -1)
                {
                    range = this.RangeIndices[rangeIndex];
                }
                yield return new DataTypeIteration()
                {
                    EncounteredBreaks = encounteredBreaks.ToList(),
                    Field = this.SubFields[i],
                    FieldIndex = i,
                    Range = range,
                    RangeIndex = rangeIndex,
                    BreakIndex = breakIndex
                };
            }
        }
    }
}
