using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Mutagen.Bethesda.Pex.Enums
{
    [PublicAPI]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum InstructionOpcode
    {
        nop = 0x0,
        iadd = 0x01,
        fadd = 0x02,
        isub = 0x03,
        fsub = 0x04,
        imul = 0x05,
        fmul = 0x06,
        idiv = 0x07,
        fdiv = 0x08,
        imod = 0x09,
        not = 0x0A,
        ineg = 0x0B,
        fneg = 0x0C,
        assign = 0x0D,
        cast = 0x0E,
        cmp_eq = 0x0F,
        cmp_lt = 0x10,
        cmp_le = 0x11,
        cmp_gt = 0x12,
        cmp_ge = 0x13,
        jmp = 0x14,
        jmpt = 0x15,
        jmpf = 0x16,
        callmethod = 0x17,
        callparent = 0x18,
        callstatic = 0x19,
        return_ = 0x1A,
        strcat = 0x1B,
        propget = 0x1C,
        propset = 0x1D,
        array_create = 0x1E,
        array_length = 0x1F,
        array_getelement = 0x20,
        array_setelement = 0x21,
        array_findelement = 0x22,
        array_rfindelement = 0x23
    }
}
