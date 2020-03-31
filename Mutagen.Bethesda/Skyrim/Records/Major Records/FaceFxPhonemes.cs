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
                        switch (str)
                        {
                            case "Aah":
                                Add(targetAccumulation, Target.Aah);
                                SetLipMode(false);
                                break;
                            case "LipBigAah":
                                Add(targetAccumulation, Target.Aah);
                                SetLipMode(true);
                                break;
                            case "BigAah":
                                Add(targetAccumulation, Target.BigAah);
                                SetLipMode(false);
                                break;
                            case "LipDST":
                                Add(targetAccumulation, Target.BigAah);
                                SetLipMode(true);
                                break;
                            case "BMP":
                                Add(targetAccumulation, Target.BMP);
                                SetLipMode(false);
                                break;
                            case "LipEee":
                                Add(targetAccumulation, Target.BMP);
                                SetLipMode(true);
                                break;
                            case "ChJSh":
                                Add(targetAccumulation, Target.ChJSh);
                                SetLipMode(false);
                                break;
                            case "LipFV":
                                Add(targetAccumulation, Target.ChJSh);
                                SetLipMode(true);
                                break;
                            case "DST":
                                Add(targetAccumulation, Target.DST);
                                SetLipMode(false);
                                break;
                            case "LipK":
                                Add(targetAccumulation, Target.DST);
                                SetLipMode(true);
                                break;
                            case "Eee":
                                Add(targetAccumulation, Target.Eee);
                                SetLipMode(false);
                                break;
                            case "LipL":
                                Add(targetAccumulation, Target.Eee);
                                SetLipMode(true);
                                break;
                            case "Eh":
                                Add(targetAccumulation, Target.Eh);
                                SetLipMode(false);
                                break;
                            case "LipR":
                                Add(targetAccumulation, Target.Eh);
                                SetLipMode(true);
                                break;
                            case "FV":
                                Add(targetAccumulation, Target.FV);
                                SetLipMode(false);
                                break;
                            case "LipTh":
                                Add(targetAccumulation, Target.FV);
                                SetLipMode(true);
                                break;
                            case "I":
                                Add(targetAccumulation, Target.I);
                                break;
                            case "K":
                                Add(targetAccumulation, Target.K);
                                break;
                            case "N":
                                Add(targetAccumulation, Target.N);
                                break;
                            case "Oh":
                                Add(targetAccumulation, Target.Oh);
                                break;
                            case "OohQ":
                                Add(targetAccumulation, Target.OohQ);
                                break;
                            case "R":
                                Add(targetAccumulation, Target.R);
                                break;
                            case "Th":
                                Add(targetAccumulation, Target.Th);
                                break;
                            case "W":
                                Add(targetAccumulation, Target.W);
                                break;
                            default:
                                throw new ArgumentException($"Unknown Face String: {str}");
                        }
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
                if (!hasAll)
                {
                    foreach (var target in FaceFxPhonemesBinaryCreateTranslation.Targets)
                    {
                        IPhonemeGetter? phoneme = phonemes[(int)target];
                        using (HeaderExport.ExportSubrecordHeader(writer, Race_Registration.PHTN_HEADER))
                        {
                            writer.WriteZString(target.GetString(lipMode));
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
        }
    }
}
