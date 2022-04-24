using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Fallout4.Internals;
using static Mutagen.Bethesda.Fallout4.FaceFxPhonemes;

namespace Mutagen.Bethesda.Fallout4;

public partial class FaceFxPhonemes
{
    public enum Target
    {
        Aah,
        BigAah,
        BMP,
        ChJSh,
        DST,
        Eee,
        Eh,
        FV,
        I,
        K,
        N,
        Oh,
        OohQ,
        R,
        Th,
        W,
    }
}

public partial class FaceFxPhonemesMixIn
{
    public static void Set(this IFaceFxPhonemes faceFx, Target target, Phoneme phoneme)
    {
        switch (target)
        {
            case FaceFxPhonemes.Target.Aah:
                faceFx.Aah_LipBigAah = phoneme;
                break;
            case FaceFxPhonemes.Target.BigAah:
                faceFx.BigAah_LipDST = phoneme;
                break;
            case FaceFxPhonemes.Target.BMP:
                faceFx.BMP_LipEee = phoneme;
                break;
            case FaceFxPhonemes.Target.ChJSh:
                faceFx.ChJSh_LipFV = phoneme;
                break;
            case FaceFxPhonemes.Target.DST:
                faceFx.DST_LipK = phoneme;
                break;
            case FaceFxPhonemes.Target.Eee:
                faceFx.Eee_LipL = phoneme;
                break;
            case FaceFxPhonemes.Target.Eh:
                faceFx.Eh_LipR = phoneme;
                break;
            case FaceFxPhonemes.Target.FV:
                faceFx.FV_LipTh = phoneme;
                break;
            case FaceFxPhonemes.Target.I:
                faceFx.I = phoneme;
                break;
            case FaceFxPhonemes.Target.K:
                faceFx.K = phoneme;
                break;
            case FaceFxPhonemes.Target.N:
                faceFx.N = phoneme;
                break;
            case FaceFxPhonemes.Target.Oh:
                faceFx.Oh = phoneme;
                break;
            case FaceFxPhonemes.Target.OohQ:
                faceFx.OohQ = phoneme;
                break;
            case FaceFxPhonemes.Target.R:
                faceFx.R = phoneme;
                break;
            case FaceFxPhonemes.Target.Th:
                faceFx.Th = phoneme;
                break;
            case FaceFxPhonemes.Target.W:
                faceFx.W = phoneme;
                break;
            default:
                faceFx.Unknowns.Add(phoneme);
                break;
        }
    }

    public static Phoneme? Get(this IFaceFxPhonemes faceFx, Target target)
    {
        switch (target)
        {
            case FaceFxPhonemes.Target.Aah:
                return faceFx.Aah_LipBigAah;
            case FaceFxPhonemes.Target.BigAah:
                return faceFx.BigAah_LipDST;
            case FaceFxPhonemes.Target.BMP:
                return faceFx.BMP_LipEee;
            case FaceFxPhonemes.Target.ChJSh:
                return faceFx.ChJSh_LipFV;
            case FaceFxPhonemes.Target.DST:
                return faceFx.DST_LipK;
            case FaceFxPhonemes.Target.Eee:
                return faceFx.Eee_LipL;
            case FaceFxPhonemes.Target.Eh:
                return faceFx.Eh_LipR;
            case FaceFxPhonemes.Target.FV:
                return faceFx.FV_LipTh;
            case FaceFxPhonemes.Target.I:
                return faceFx.I;
            case FaceFxPhonemes.Target.K:
                return faceFx.K;
            case FaceFxPhonemes.Target.N:
                return faceFx.N;
            case FaceFxPhonemes.Target.Oh:
                return faceFx.Oh;
            case FaceFxPhonemes.Target.OohQ:
                return faceFx.OohQ;
            case FaceFxPhonemes.Target.R:
                return faceFx.R;
            case FaceFxPhonemes.Target.Th:
                return faceFx.Th;
            case FaceFxPhonemes.Target.W:
                return faceFx.W;
            default:
                throw new NotImplementedException();
        }
    }

    public static IPhonemeGetter? GetGetter(this IFaceFxPhonemesGetter faceFx, Target target)
    {
        switch (target)
        {
            case FaceFxPhonemes.Target.Aah:
                return faceFx.Aah_LipBigAah;
            case FaceFxPhonemes.Target.BigAah:
                return faceFx.BigAah_LipDST;
            case FaceFxPhonemes.Target.BMP:
                return faceFx.BMP_LipEee;
            case FaceFxPhonemes.Target.ChJSh:
                return faceFx.ChJSh_LipFV;
            case FaceFxPhonemes.Target.DST:
                return faceFx.DST_LipK;
            case FaceFxPhonemes.Target.Eee:
                return faceFx.Eee_LipL;
            case FaceFxPhonemes.Target.Eh:
                return faceFx.Eh_LipR;
            case FaceFxPhonemes.Target.FV:
                return faceFx.FV_LipTh;
            case FaceFxPhonemes.Target.I:
                return faceFx.I;
            case FaceFxPhonemes.Target.K:
                return faceFx.K;
            case FaceFxPhonemes.Target.N:
                return faceFx.N;
            case FaceFxPhonemes.Target.Oh:
                return faceFx.Oh;
            case FaceFxPhonemes.Target.OohQ:
                return faceFx.OohQ;
            case FaceFxPhonemes.Target.R:
                return faceFx.R;
            case FaceFxPhonemes.Target.Th:
                return faceFx.Th;
            case FaceFxPhonemes.Target.W:
                return faceFx.W;
            default:
                throw new NotImplementedException();
        }
    }

    public static string? TryGetString(this Target target, bool lipMode)
    {
        switch (target)
        {
            case Target.Aah:
                return lipMode ? "LipBigAah" : "Aah";
            case Target.BigAah:
                return lipMode ? "LipDST" : "BigAah";
            case Target.BMP:
                return lipMode ? "LipEee" : "BMP";
            case Target.ChJSh:
                return lipMode ? "LipFV" : "ChJSh";
            case Target.DST:
                return lipMode ? "LipK" : "DST";
            case Target.Eee:
                return lipMode ? "LipL" : "Eee";
            case Target.Eh:
                return lipMode ? "LipR" : "Eh";
            case Target.FV:
                return lipMode ? "LipTh" : "FV";
            case Target.I:
                return "I";
            case Target.K:
                return "K";
            case Target.N:
                return "N";
            case Target.Oh:
                return "Oh";
            case Target.OohQ:
                return "OohQ";
            case Target.R:
                return "R";
            case Target.Th:
                return "Th";
            case Target.W:
                return "W";
            default:
                return null;
        }
    }
}

partial class FaceFxPhonemesBinaryCreateTranslation
{
    public static readonly IReadOnlyList<Target> Targets = EnumExt.GetValues<Target>().ToExtendedList();
    public static readonly IReadOnlyList<(Target Target, string Name)> TargetWithNames;
    public static readonly int TargetSize = EnumExt.GetSize<Target>();
    public static readonly IEnumerable<Phoneme.Slot> Slots = EnumExt.GetValues<Phoneme.Slot>();
    public static readonly int SlotSize = EnumExt.GetSize<Phoneme.Slot>();

    static FaceFxPhonemesBinaryCreateTranslation()
    {
        TargetWithNames = Targets
            .Select(t =>
            {
                return (t, t.TryGetString(lipMode: false));
            })
            .Where(x => x.Item2 != null)
            .Select(x => (x.t, x.Item2!))
            .ToExtendedList();
    }

    public static bool IsTypical(string str)
    {
        switch (str)
        {
            case "Aah":
            case "BigAah":
            case "BMP":
            case "ChJSh":
            case "DST":
            case "Eee":
            case "Eh":
            case "FV":
            case "I":
            case "K":
            case "N":
            case "Oh":
            case "OohQ":
            case "R":
            case "Th":
            case "W":
                return true;
            default:
                return false;
        }
    }

    public static ParseResult ParseFaceFxPhonemes(MutagenFrame frame, IFaceFxPhonemes item)
    {
        var meta = frame.Reader.GetSubrecordHeader();
        IReadOnlyList<(Target Target, string Name)>? targets;

        if (meta.RecordType == RecordTypes.PHTN)
        {
            var targetAccumulation = new List<(Target Target, string Name)>();
            targets = targetAccumulation;
            var set = new HashSet<Target>();

            void Add(List<(Target Target, string Name)> targets, Target target, string name)
            {
                if (!set.Add(target))
                {
                    throw new ArgumentException("Already added target during listing parse.");
                }
                targets.Add((target, name));
            }

            var subFrame = frame.Reader.GetSubrecord();
            int i = 0;
            while (subFrame.RecordType == RecordTypes.PHTN)
            {
                var str = BinaryStringUtility.ProcessWholeToZString(subFrame.Content, frame.MetaData.Encodings.NonTranslated);
                Add(targetAccumulation, (Target)i++, str);
                frame.Position += subFrame.TotalLength;
                if (!frame.Reader.TryGetSubrecord(out subFrame)) break;
            }

            item.ForceNames = true;
        }
        else if (meta.RecordType == RecordTypes.PHWT)
        {
            targets = TargetWithNames;
        }
        else
        {
            throw new ArgumentException($"Unexpected header: {meta.RecordType}");
        }

        // Read in all the slots
        int? expectedSize = null;
        ReadOnlyMemorySlice<byte>[] slots = new ReadOnlyMemorySlice<byte>[SlotSize];
        for (int i = 0; i < SlotSize; i++)
        {
            var subMetaFrame = frame.Reader.ReadSubrecord(RecordTypes.PHWT);
            var content = subMetaFrame.Content;
            if (expectedSize == null)
            {
                expectedSize = content.Length;
            }
            else if (content.Length != expectedSize)
            {
                throw new ArgumentException($"Unexpected target size: {content.Length} < {expectedSize}");
            }
            slots[i] = subMetaFrame.Content;
        }
                
        // Loop over the targets outlined in listing, parse slots
        var expectedTargets = expectedSize / 4;
        for (int i = 0; i < expectedTargets; i++)
        {
            if (!targets.TryGet(i, out var target))
            {
                target = ((Target)i, string.Empty);
            }
            var phoneme = new Phoneme()
            {
                Name = target.Name
            };
            item.Set(target.Target, phoneme);
            foreach (var slot in Slots)
            {
                var mem = slots[(int)slot];
                phoneme.Set(slot, mem.Slice(i * 4, 4).Float());
            }
        }

        return null;
    }
}

partial class FaceFxPhonemesBinaryWriteTranslation
{
    public static void WriteFaceFxPhonemes(MutagenWriter writer, IFaceFxPhonemesGetter item)
    {
        IPhonemeGetter?[] phonemes = new IPhonemeGetter?[FaceFxPhonemesBinaryCreateTranslation.TargetSize + item.Unknowns.Count];
        foreach (var target in FaceFxPhonemesBinaryCreateTranslation.Targets)
        {
            phonemes[(int)target] = item.GetGetter(target);
        }

        for (int i = 0; i < item.Unknowns.Count; i++)
        {
            var unknownPhoneme = item.Unknowns[i];
            int target = i + FaceFxPhonemesBinaryCreateTranslation.TargetSize;
            phonemes[target] = unknownPhoneme;
        }
        if (phonemes.All(p => p == null)) return;
        var force = item.ForceNames;
        var hasAll = phonemes.All(p => p != null);
        if (!hasAll || force)
        {
            for (int i = 0; i < phonemes.Length; i++)
            {
                var phoneme = phonemes[i];
                if (phoneme == null) continue;
                using (HeaderExport.Subrecord(writer, RecordTypes.PHTN))
                {
                    writer.Write(phoneme.Name, StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);
                }
            }
        }
        foreach (var slot in FaceFxPhonemesBinaryCreateTranslation.Slots)
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.PHWT))
            {
                foreach (var phoneme in phonemes)
                {
                    if (phoneme == null) continue;
                    writer.Write(phoneme.Get(slot));
                }
            }
        }
    }
}

partial class FaceFxPhonemesBinaryOverlay
{
    public bool ForceNames => throw new NotImplementedException();

    public IPhonemeGetter? Aah_LipBigAah => throw new NotImplementedException();

    public IPhonemeGetter? BigAah_LipDST => throw new NotImplementedException();

    public IPhonemeGetter? BMP_LipEee => throw new NotImplementedException();

    public IPhonemeGetter? ChJSh_LipFV => throw new NotImplementedException();

    public IPhonemeGetter? DST_LipK => throw new NotImplementedException();

    public IPhonemeGetter? Eee_LipL => throw new NotImplementedException();

    public IPhonemeGetter? Eh_LipR => throw new NotImplementedException();

    public IPhonemeGetter? FV_LipTh => throw new NotImplementedException();

    public IPhonemeGetter? I => throw new NotImplementedException();

    public IPhonemeGetter? K => throw new NotImplementedException();

    public IPhonemeGetter? N => throw new NotImplementedException();

    public IPhonemeGetter? Oh => throw new NotImplementedException();

    public IPhonemeGetter? OohQ => throw new NotImplementedException();

    public IPhonemeGetter? R => throw new NotImplementedException();

    public IPhonemeGetter? Th => throw new NotImplementedException();

    public IPhonemeGetter? W => throw new NotImplementedException();
            
    public IReadOnlyList<IPhonemeGetter> Unknowns => throw new NotImplementedException();

    partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
    {
        throw new NotImplementedException();
    }
}