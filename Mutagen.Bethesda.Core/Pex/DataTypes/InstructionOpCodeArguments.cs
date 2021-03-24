using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Mutagen.Bethesda.Core.Pex.Enums;

namespace Mutagen.Bethesda.Core.Pex.DataTypes
{
    [PublicAPI]
    public static class InstructionOpCodeArguments
    {
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public static IReadOnlyList<string> Arguments => new[]
        {
            "", // 00
            "SII",
            "SFF",
            "SII",
            "SFF", // 04
            "SII",
            "SFF",
            "SII",
            "SFF", // 08
            "SII",
            "SA",
            "SI",
            "SF", // 0C
            "SA",
            "SA",
            "SAA",
            "SAA", // 10
            "SAA",
            "SAA",
            "SAA",
            "L", // 14
            "AL",
            "AL",
            "NSS*",
            "NS*", // 18
            "NNS*",
            "A",
            "SQQ",
            "NSS", // 1C
            "NSA",
            "Su",
            "SS",
            "SSI", // 20
            "SIA",
            "SSII",
            "SSII",
            
            //F04
            //TODO: find the right argument types, only length is correct
            "SSS", // 24
            "S",
            "SS",
            "SSS",
            "SSSSS", // 28
            "SSSSS",
            "SSS",
            "SSS",
            "S", // 2C
            "SSS",
            "S"
        };

        public static string GetArguments(InstructionOpcode opcode)
        {
            var index = (byte) opcode;
            if (index >= Arguments.Count)
                throw new ArgumentOutOfRangeException(nameof(opcode), $"Out-of-range: {opcode} with index {index}");
            return Arguments[index];
        }
    }
}
