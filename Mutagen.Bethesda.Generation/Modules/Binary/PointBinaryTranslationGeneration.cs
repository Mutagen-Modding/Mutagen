using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Loqui.Generation;
using Noggog;

namespace Mutagen.Bethesda.Generation.Modules.Binary
{
    public class PointBinaryTranslationGeneration<T> : PrimitiveBinaryTranslationGeneration<T>
    {
        public PointBinaryTranslationGeneration(int? expectedLen)
            : base(expectedLen)
        {
            PreferDirectTranslation = false;
            this.AdditionalCopyInParams.Add(FlipTest);
            this.AdditionalCopyInRetParams.Add(FlipTest);
            this.AdditionalWriteParams.Add(FlipTest);
        }

        public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor, Accessor packageAccessor)
        {
            return $"{GetTranslatorInstance(typeGen, getter: true)}.Read({dataAccessor}{(NeedsFlip(typeGen) ? ", swapCoords: true" : null)})";
        }

        public override void Load(ObjectGeneration obj, TypeGeneration field, XElement node)
        {
            base.Load(obj, field, node);
            field.CustomData["Flip"] = node.GetAttribute<bool>("swapCoords", false);
        }

        public TryGet<string> FlipTest(ObjectGeneration o, TypeGeneration t)
        {
            return TryGet<string>.Create(NeedsFlip(t), "swapCoords: true");
        }

        public bool NeedsFlip(TypeGeneration t)
        {
            if (!(t is P2Int16Type p2)) return false;
            if (!p2.CustomData.TryGetValue("Flip", out var flip)) return false;
            if (!(bool)flip) return false;
            return true;
        }

        public override bool AllowDirectParse(ObjectGeneration objGen, TypeGeneration typeGen, bool squashedRepeatedList)
        {
            return !NeedsFlip(typeGen);
        }

        public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            return !NeedsFlip(typeGen);
        }
    }
}
