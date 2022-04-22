using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Pex;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public enum InstructionOpcode
{
    NOP = 0x0,
    IADD = 0x01,
    FADD = 0x02,
    ISUB = 0x03,
    FSUB = 0x04,
    IMUL = 0x05,
    FMUL = 0x06,
    IDIV = 0x07,
    FDIV = 0x08,
    IMOD = 0x09,
    NOT = 0x0A,
    INEG = 0x0B,
    FNEG = 0x0C,
    ASSIGN = 0x0D,
    CAST = 0x0E,
    CMP_EQ = 0x0F,
    CMP_LT = 0x10,
    CMP_LTE = 0x11,
    CMP_GT = 0x12,
    CMP_GTE = 0x13,
    JMP = 0x14,
    JMPT = 0x15,
    JMPF = 0x16,
    CALLMETHOD = 0x17,
    CALLPARENT = 0x18,
    CALLSTATIC = 0x19,
    RETURN = 0x1A,
    STRCAT = 0x1B,
    PROPGET = 0x1C,
    PROPSET = 0x1D,
    ARRAY_CREATE = 0x1E,
    ARRAY_LENGTH = 0x1F,
    ARRAY_GETELEMENT = 0x20,
    ARRAY_SETELEMENT = 0x21,
    ARRAY_FINDELEMENT = 0x22,
    ARRAY_RFINDELEMENT = 0x23,
        
    //FO4 only
    IS = 0x24,
    STRUCT_CREATE = 0x25,
    STRUCT_GET = 0x26,
    STRUCT_SET = 0x27,
    ARRAY_FINDSTRUCT = 0x28,
    ARRAY_RFINDSTRUCT = 0x29,
    ARRAY_ADD = 0x2A,
    ARRAY_INSERT = 0x2B,
    ARRAY_REMOVELAST = 0x2C,
    ARRAY_REMOVE = 0x2D,
    ARRAY_CLEA = 0x2E
}