using System;
using System.Drawing;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class ColorBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<Color>
    {
        public override int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            switch (BinaryType(typeGen))
            {
                case ColorBinaryType.NoAlpha:
                    return 3;
                case ColorBinaryType.Alpha:
                    return 4;
                case ColorBinaryType.NoAlphaFloat:
                    return 12;
                default:
                    throw new NotImplementedException();
            }
        }

        public ColorBinaryTranslationGeneration()
            : base(expectedLen: null)
        {
            this.AdditionalWriteParams.Add(AdditionalParam);
            this.AdditionalCopyInParams.Add(AdditionalParam);
            this.AdditionalCopyInRetParams.Add(AdditionalParam);
            CustomRead = (fg, objGen, typeGen, reader, item) =>
            {
                var binaryType = BinaryType(typeGen);
                fg.AppendLine($"{item} = {reader}.ReadColor({nameof(ColorBinaryType)}.{binaryType});");
            };
        }

        private static TryGet<string> AdditionalParam(
           ObjectGeneration objGen,
           TypeGeneration typeGen)
        {
            var binaryType = BinaryType(typeGen);
            string param = $"binaryType: {nameof(ColorBinaryType)}.{binaryType}";
            return TryGet<string>.Create(successful: binaryType != ColorBinaryType.Alpha, val: param);
        }

        protected static ColorBinaryType BinaryType(TypeGeneration typeGen)
        {
            if (!typeGen.CustomData.TryGetValue(ColorTypeModule.BinaryTypeStr, out var obj)) return ColorBinaryType.Alpha;
            return (ColorBinaryType)obj;
        }

        public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor, Accessor packageAccessor)
        {
            var binaryType = BinaryType(typeGen);
            return $"{dataAccessor}.ReadColor({nameof(ColorBinaryType)}.{binaryType})";
        }
    }
}
