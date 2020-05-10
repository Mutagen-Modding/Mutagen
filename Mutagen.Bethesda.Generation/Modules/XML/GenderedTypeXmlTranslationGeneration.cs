using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class GenderedTypeXmlTranslationGeneration : XmlTranslationGeneration
    {
        public override void GenerateCopyIn(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor nodeAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationMaskAccessor)
        {
            GenderedType gendered = typeGen as GenderedType;
            var gen = this.XmlMod.GetTypeGeneration(gendered.SubTypeGeneration.GetType());
            MaskGenerationUtility.WrapErrorFieldIndexPush(
                fg,
                () =>
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{itemAccessor} = new {typeGen.TypeName(getter: false)}"))
                    {
                        args.Add(subFg =>
                        {
                            gen.GenerateCopyIn(subFg, objGen, gendered.SubTypeGeneration, nodeAccessor, Accessor.ConstructorParam($"male"), errorMaskAccessor, translationMaskAccessor: null);
                        });
                        args.Add(subFg =>
                        {
                            gen.GenerateCopyIn(subFg, objGen, gendered.SubTypeGeneration, nodeAccessor, Accessor.ConstructorParam($"female"), errorMaskAccessor, translationMaskAccessor: null);
                        });
                    }
                },
                errorMaskAccessor,
                typeGen.HasIndex ? typeGen.IndexEnumInt : default(Accessor));
        }

        public override void GenerateForCommonXSD(XElement rootElement, TypeGeneration typeGen)
        {
            throw new NotImplementedException();
        }

        public override XElement GenerateForXSD(ObjectGeneration objGen, XElement rootElement, XElement choiceElement, TypeGeneration typeGen, string nameOverride)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrite(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor nameAccessor, Accessor translationMaskAccessor)
        {
            GenderedType gendered = typeGen as GenderedType;
            var gen = this.XmlMod.GetTypeGeneration(gendered.SubTypeGeneration.GetType());
            using (new BraceWrapper(fg))
            {
                gen.GenerateWrite(fg, objGen, gendered.SubTypeGeneration, writerAccessor, $"{itemAccessor}.Male", errorMaskAccessor, nameAccessor, translationMaskAccessor);
            }
            using (new BraceWrapper(fg))
            {
                gen.GenerateWrite(fg, objGen, gendered.SubTypeGeneration, writerAccessor, $"{itemAccessor}.Female", errorMaskAccessor, nameAccessor, translationMaskAccessor);
            }
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }
    }
}
