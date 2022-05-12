using static Mutagen.Bethesda.Skyrim.Phoneme;

namespace Mutagen.Bethesda.Skyrim;

public partial class Phoneme
{
    /// <summary>
    /// Enum representing the different phonemes a weight set can be applied to
    /// </summary>
    public enum Slot
    {
        IY,
        IH,
        EH,
        EY,
        AE,
        AA,
        AW,
        AY,
        AH,
        AO,
        OY,
        OW,
        UH,
        UW,
        ER,
        AX,
        S,
        SH,
        Z,
        ZH,
        F,
        TH,
        V,
        DH,
        M,
        N,
        NG,
        L,
        R,
        W,
        Y,
        HH,
        B,
        D,
        JH,
        G,
        P,
        T,
        K,
        CH,
        SIL,
        SHOTSIL,
        FLAP,
    }
}

public partial class PhonemeMixIn
{
    public static void Set(this IPhoneme phoneme, Slot slot, float f)
    {
        switch (slot)
        {
            case Phoneme.Slot.IY:
                phoneme.IY = f;
                break;
            case Phoneme.Slot.IH:
                phoneme.IH = f;
                break;
            case Phoneme.Slot.EH:
                phoneme.EH = f;
                break;
            case Phoneme.Slot.EY:
                phoneme.EY = f;
                break;
            case Phoneme.Slot.AE:
                phoneme.AE = f;
                break;
            case Phoneme.Slot.AA:
                phoneme.AA = f;
                break;
            case Phoneme.Slot.AW:
                phoneme.AW = f;
                break;
            case Phoneme.Slot.AY:
                phoneme.AY = f;
                break;
            case Phoneme.Slot.AH:
                phoneme.AH = f;
                break;
            case Phoneme.Slot.AO:
                phoneme.AO = f;
                break;
            case Phoneme.Slot.OY:
                phoneme.OY = f;
                break;
            case Phoneme.Slot.OW:
                phoneme.OW = f;
                break;
            case Phoneme.Slot.UH:
                phoneme.UH = f;
                break;
            case Phoneme.Slot.UW:
                phoneme.UW = f;
                break;
            case Phoneme.Slot.ER:
                phoneme.ER = f;
                break;
            case Phoneme.Slot.AX:
                phoneme.AX = f;
                break;
            case Phoneme.Slot.S:
                phoneme.S = f;
                break;
            case Phoneme.Slot.SH:
                phoneme.SH = f;
                break;
            case Phoneme.Slot.Z:
                phoneme.Z = f;
                break;
            case Phoneme.Slot.ZH:
                phoneme.ZH = f;
                break;
            case Phoneme.Slot.F:
                phoneme.F = f;
                break;
            case Phoneme.Slot.TH:
                phoneme.TH = f;
                break;
            case Phoneme.Slot.V:
                phoneme.V = f;
                break;
            case Phoneme.Slot.DH:
                phoneme.DH = f;
                break;
            case Phoneme.Slot.M:
                phoneme.M = f;
                break;
            case Phoneme.Slot.N:
                phoneme.N = f;
                break;
            case Phoneme.Slot.NG:
                phoneme.NG = f;
                break;
            case Phoneme.Slot.L:
                phoneme.L = f;
                break;
            case Phoneme.Slot.R:
                phoneme.R = f;
                break;
            case Phoneme.Slot.W:
                phoneme.W = f;
                break;
            case Phoneme.Slot.Y:
                phoneme.Y = f;
                break;
            case Phoneme.Slot.HH:
                phoneme.HH = f;
                break;
            case Phoneme.Slot.B:
                phoneme.B = f;
                break;
            case Phoneme.Slot.D:
                phoneme.D = f;
                break;
            case Phoneme.Slot.JH:
                phoneme.JH = f;
                break;
            case Phoneme.Slot.G:
                phoneme.G = f;
                break;
            case Phoneme.Slot.P:
                phoneme.P = f;
                break;
            case Phoneme.Slot.T:
                phoneme.T = f;
                break;
            case Phoneme.Slot.K:
                phoneme.K = f;
                break;
            case Phoneme.Slot.CH:
                phoneme.CH = f;
                break;
            case Phoneme.Slot.SIL:
                phoneme.SIL = f;
                break;
            case Phoneme.Slot.SHOTSIL:
                phoneme.SHOTSIL = f;
                break;
            case Phoneme.Slot.FLAP:
                phoneme.FLAP = f;
                break;
            default:
                break;
        }
    }

    public static float Get(this IPhonemeGetter phoneme, Slot slot)
    {
        switch (slot)
        {
            case Phoneme.Slot.IY:
                return phoneme.IY;
            case Phoneme.Slot.IH:
                return phoneme.IH;
            case Phoneme.Slot.EH:
                return phoneme.EH;
            case Phoneme.Slot.EY:
                return phoneme.EY;
            case Phoneme.Slot.AE:
                return phoneme.AE;
            case Phoneme.Slot.AA:
                return phoneme.AA;
            case Phoneme.Slot.AW:
                return phoneme.AW;
            case Phoneme.Slot.AY:
                return phoneme.AY;
            case Phoneme.Slot.AH:
                return phoneme.AH;
            case Phoneme.Slot.AO:
                return phoneme.AO;
            case Phoneme.Slot.OY:
                return phoneme.OY;
            case Phoneme.Slot.OW:
                return phoneme.OW;
            case Phoneme.Slot.UH:
                return phoneme.UH;
            case Phoneme.Slot.UW:
                return phoneme.UW;
            case Phoneme.Slot.ER:
                return phoneme.ER;
            case Phoneme.Slot.AX:
                return phoneme.AX;
            case Phoneme.Slot.S:
                return phoneme.S;
            case Phoneme.Slot.SH:
                return phoneme.SH;
            case Phoneme.Slot.Z:
                return phoneme.Z;
            case Phoneme.Slot.ZH:
                return phoneme.ZH;
            case Phoneme.Slot.F:
                return phoneme.F;
            case Phoneme.Slot.TH:
                return phoneme.TH;
            case Phoneme.Slot.V:
                return phoneme.V;
            case Phoneme.Slot.DH:
                return phoneme.DH;
            case Phoneme.Slot.M:
                return phoneme.M;
            case Phoneme.Slot.N:
                return phoneme.N;
            case Phoneme.Slot.NG:
                return phoneme.NG;
            case Phoneme.Slot.L:
                return phoneme.L;
            case Phoneme.Slot.R:
                return phoneme.R;
            case Phoneme.Slot.W:
                return phoneme.W;
            case Phoneme.Slot.Y:
                return phoneme.Y;
            case Phoneme.Slot.HH:
                return phoneme.HH;
            case Phoneme.Slot.B:
                return phoneme.B;
            case Phoneme.Slot.D:
                return phoneme.D;
            case Phoneme.Slot.JH:
                return phoneme.JH;
            case Phoneme.Slot.G:
                return phoneme.G;
            case Phoneme.Slot.P:
                return phoneme.P;
            case Phoneme.Slot.T:
                return phoneme.T;
            case Phoneme.Slot.K:
                return phoneme.K;
            case Phoneme.Slot.CH:
                return phoneme.CH;
            case Phoneme.Slot.SIL:
                return phoneme.SIL;
            case Phoneme.Slot.SHOTSIL:
                return phoneme.SHOTSIL;
            case Phoneme.Slot.FLAP:
                return phoneme.FLAP;
            default:
                throw new NotImplementedException();
        }
    }
}

partial class PhonemeBinaryOverlay
{
    public string Name => throw new NotImplementedException();

    public float IY => throw new NotImplementedException();

    public float IH => throw new NotImplementedException();

    public float EH => throw new NotImplementedException();

    public float EY => throw new NotImplementedException();

    public float AE => throw new NotImplementedException();

    public float AA => throw new NotImplementedException();

    public float AW => throw new NotImplementedException();

    public float AY => throw new NotImplementedException();

    public float AH => throw new NotImplementedException();

    public float AO => throw new NotImplementedException();

    public float OY => throw new NotImplementedException();

    public float OW => throw new NotImplementedException();

    public float UH => throw new NotImplementedException();

    public float UW => throw new NotImplementedException();

    public float ER => throw new NotImplementedException();

    public float AX => throw new NotImplementedException();

    public float S => throw new NotImplementedException();

    public float SH => throw new NotImplementedException();

    public float Z => throw new NotImplementedException();

    public float ZH => throw new NotImplementedException();

    public float F => throw new NotImplementedException();

    public float TH => throw new NotImplementedException();

    public float V => throw new NotImplementedException();

    public float DH => throw new NotImplementedException();

    public float M => throw new NotImplementedException();

    public float N => throw new NotImplementedException();

    public float NG => throw new NotImplementedException();

    public float L => throw new NotImplementedException();

    public float R => throw new NotImplementedException();

    public float W => throw new NotImplementedException();

    public float Y => throw new NotImplementedException();

    public float HH => throw new NotImplementedException();

    public float B => throw new NotImplementedException();

    public float D => throw new NotImplementedException();

    public float JH => throw new NotImplementedException();

    public float G => throw new NotImplementedException();

    public float P => throw new NotImplementedException();

    public float T => throw new NotImplementedException();

    public float K => throw new NotImplementedException();

    public float CH => throw new NotImplementedException();

    public float SIL => throw new NotImplementedException();

    public float SHOTSIL => throw new NotImplementedException();

    public float FLAP => throw new NotImplementedException();
}