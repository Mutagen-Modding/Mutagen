using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Mutagen.Bethesda.Skyrim.FaceFxPhonemes;

namespace Mutagen.Bethesda.Skyrim
{
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
            W
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
                    throw new NotImplementedException();
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

        public static string GetString(this Target target, bool lipMode)
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
                    throw new NotImplementedException();
            }
        }

        public static Target GetTarget(string str, out bool lipMode)
        {
            switch (str)
            {
                case "Aah":
                    lipMode = false;
                    return Target.Aah;
                case "LipBigAah":
                    lipMode = true;
                    return Target.Aah;
                case "BigAah":
                    lipMode = false;
                    return Target.BigAah;
                case "LipDST":
                    lipMode = true;
                    return Target.BigAah;
                case "BMP":
                    lipMode = false;
                    return Target.BMP;
                case "LipEee":
                    lipMode = true;
                    return Target.BMP;
                case "ChJSh":
                    lipMode = false;
                    return Target.ChJSh;
                case "LipFV":
                    lipMode = true;
                    return Target.ChJSh;
                case "DST":
                    lipMode = false;
                    return Target.DST;
                case "LipK":
                    lipMode = true;
                    return Target.DST;
                case "Eee":
                    lipMode = false;
                    return Target.Eee;
                case "LipL":
                    lipMode = true;
                    return Target.Eee;
                case "Eh":
                    lipMode = false;
                    return Target.Eh;
                case "LipR":
                    lipMode = true;
                    return Target.Eh;
                case "FV":
                    lipMode = false;
                    return Target.FV;
                case "LipTh":
                    lipMode = true;
                    return Target.FV;
                case "I":
                    lipMode = false;
                    return Target.I;
                case "K":
                    lipMode = false;
                    return Target.K;
                case "N":
                    lipMode = false;
                    return Target.N;
                case "Oh":
                    lipMode = false;
                    return Target.Oh;
                case "OohQ":
                    lipMode = false;
                    return Target.OohQ;
                case "R":
                    lipMode = false;
                    return Target.R;
                case "Th":
                    lipMode = false;
                    return Target.Th;
                case "W":
                    lipMode = false;
                    return Target.W;
                default:
                    throw new ArgumentException($"Unknown Face String: {str}");
            }
        }
    }

    namespace Internals
    {
        public partial class FaceFxPhonemesBinaryCreateTranslation
        {
            public static readonly IReadOnlyList<Target> Targets = EnumExt.GetValues<Target>().ToExtendedList();
            public static readonly int TargetSize = EnumExt.GetSize<Target>();
            public static readonly IEnumerable<Phoneme.Slot> Slots = EnumExt.GetValues<Phoneme.Slot>();
            public static readonly int SlotSize = EnumExt.GetSize<Phoneme.Slot>();

            public static void ParseFaceFxPhonemes(MutagenFrame frame, IFaceFxPhonemes item)
            {
                var meta = frame.Reader.GetSubrecord();
                IReadOnlyList<Target>? targets;
                bool? lipMode = null;

                if (meta.RecordType == Race_Registration.PHTN_HEADER)
                {
                    var targetAccumulation = new List<Target>();
                    targets = targetAccumulation;
                    HashSet<Target> set = new HashSet<Target>();

                    void Add(List<Target> targets, Target target)
                    {
                        if (!set.Add(target))
                        {
                            throw new ArgumentException("Already added target during listing parse.");
                        }
                        targets.Add(target);
                    }

                    void SetLipMode(bool on)
                    {
                        if (lipMode == null)
                        {
                            lipMode = on;
                        }
                        else if (lipMode != on)
                        {
                            throw new ArgumentException("Conflicting Lip Modes.");
                        }
                    }

                    var subFrame = frame.Reader.GetSubrecordFrame();
                    while (subFrame.Header.RecordType == Race_Registration.PHTN_HEADER)
                    {
                        var str = BinaryStringUtility.ProcessWholeToZString(subFrame.Content);
                        Add(targetAccumulation, FaceFxPhonemesMixIn.GetTarget(str, out var isLip));
                        SetLipMode(isLip);
                        frame.Position += subFrame.Header.TotalLength;
                        if (!frame.Reader.TryGetSubrecordFrame(out subFrame)) break;
                    }
                }
                else if (meta.RecordType == Race_Registration.PHWT_HEADER)
                {
                    targets = Targets;
                }
                else
                {
                    throw new ArgumentException($"Unexpected header: {meta.RecordType}");
                }

                // Set lip mode
                item.LipMode = lipMode ?? false;

                // Read in all the slots
                var expectedSize = targets.Count * 4;
                ReadOnlyMemorySlice<byte>[] slots = new ReadOnlyMemorySlice<byte>[SlotSize];
                for (int i = 0; i < SlotSize; i++)
                {
                    var subMetaFrame = frame.Reader.ReadSubrecordMemoryFrame(Race_Registration.PHWT_HEADER);
                    var content = subMetaFrame.Content;
                    if (content.Length != expectedSize)
                    {
                        throw new ArgumentException($"Unexpected target size: {content.Length} != {expectedSize}");
                    }
                    slots[i] = subMetaFrame.Content;
                }

                // Loop over the targets outlined in listing, parse slots
                for (int i = 0; i < targets.Count; i++)
                {
                    var target = targets[i];
                    var phoneme = new Phoneme();
                    item.Set(target, phoneme);
                    foreach (var slot in Slots)
                    {
                        var mem = slots[(int)slot];
                        phoneme.Set(slot, SpanExt.GetFloat(mem.Slice(i * 4, 4)));
                    }
                }
            }
        }

        public partial class FaceFxPhonemesBinaryWriteTranslation
        {
            public static void WriteFaceFxPhonemes(MutagenWriter writer, IFaceFxPhonemesGetter item)
            {
                IPhonemeGetter?[] phonemes = new IPhonemeGetter?[FaceFxPhonemesBinaryCreateTranslation.TargetSize];
                foreach (var target in FaceFxPhonemesBinaryCreateTranslation.Targets)
                {
                    phonemes[(int)target] = item.GetGetter(target);
                }
                if (!phonemes.Any(p => p != null)) return;
                var lipMode = item.LipMode;
                var hasAll = phonemes.All(p => p != null);
                if (!hasAll || lipMode)
                {
                    for (int i = 0; i < phonemes.Length; i++)
                    {
                        if (phonemes[i] == null) continue;
                        var target = (Target)i;
                        using (HeaderExport.ExportSubrecordHeader(writer, Race_Registration.PHTN_HEADER))
                        {
                            writer.Write(target.GetString(lipMode), StringBinaryType.NullTerminate);
                        }
                    }
                }
                foreach (var slot in FaceFxPhonemesBinaryCreateTranslation.Slots)
                {
                    using (HeaderExport.ExportSubrecordHeader(writer, Race_Registration.PHWT_HEADER))
                    {
                        foreach (var target in FaceFxPhonemesBinaryCreateTranslation.Targets)
                        {
                            IPhonemeGetter? phoneme = phonemes[(int)target];
                            if (phoneme == null) continue;
                            writer.Write(phoneme.Get(slot));
                        }
                    }
                }
            }
        }

        public partial class FaceFxPhonemesBinaryOverlay
        {
            public bool LipMode => throw new NotImplementedException();

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

            partial void CustomCtor(IBinaryReadStream stream, int finalPos, int offset)
            {
                throw new NotImplementedException();
            }
        }
    }
}
