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
        public HashSet<int> BreakIndices = new HashSet<int>();
        public List<(RangeInt32, int)> RangeIndices = new List<(RangeInt32, int)>();

        public override async Task Load(XElement node, bool requireName = true)
        {
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
                    if (fieldNode.Name.LocalName.Equals("Break"))
                    {
                        BreakIndices.Add(this.SubFields.Count);
                        continue;
                    }
                    if (fieldNode.Name.LocalName.Equals("Range"))
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
                            fieldNode.GetAttribute<int>("Min", throwException: true)));
                        continue;
                    }

                    typeGen = await this.ObjectGen.LoadField(fieldNode, true);
                    if (typeGen.Succeeded)
                    {
                        this.SubFields.Add(typeGen.Value);
                    }
                }
            }
        }
    }
}
