using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using static Mutagen.Bethesda.Fallout4.ScriptFragmentsBinaryCreateTranslation;

namespace Mutagen.Bethesda.Fallout4;

partial class ScriptFragmentsBinaryCreateTranslation
{
    [Flags]
    public enum Flag
    {
        OnBegin = 0x01,
        OnEnd = 0x02,
    }

    public static ScriptFragments ReadFragments(MutagenFrame frame, ushort objectFormat)
    {
        var ret = new ScriptFragments();
        FillFragments(frame, objectFormat, ret);
        return ret;
    }

    public static void FillFragments(MutagenFrame frame, ushort objectFormat, IScriptFragments ret)
    {
        ret.ExtraBindDataVersion = frame.ReadUInt8();
        var flag = (Flag)frame.ReadUInt8();
        ret.Script = AVirtualMachineAdapterBinaryCreateTranslation.ReadEntry(frame, objectFormat);
        if (flag.HasFlag(Flag.OnBegin))
        {
            ret.OnBegin = ScriptFragment.CreateFromBinary(frame);
        }
        if (flag.HasFlag(Flag.OnEnd))
        {
            ret.OnEnd = ScriptFragment.CreateFromBinary(frame);
        }
    }
}

partial class ScriptFragmentsBinaryWriteTranslation
{
    public static void WriteFragments(MutagenWriter writer, IScriptFragmentsGetter item, ushort objectFormat)
    {
        writer.Write(item.ExtraBindDataVersion);
        var begin = item.OnBegin;
        var end = item.OnEnd;
        Flag flag = default;
        if (begin != null)
        {
            flag |= Flag.OnBegin;
        }
        if (end != null)
        {
            flag |= Flag.OnEnd;
        }
        writer.Write((byte)flag);
        AVirtualMachineAdapterBinaryWriteTranslation.WriteEntry(writer, item.Script, objectFormat);
        begin?.WriteToBinary(writer);
        end?.WriteToBinary(writer);
    }
}

partial class ScriptFragmentsBinaryOverlay
{
    public IScriptEntryGetter Script => throw new NotImplementedException();
    public IScriptFragmentGetter? OnBegin => throw new NotImplementedException();
    public IScriptFragmentGetter? OnEnd => throw new NotImplementedException();
}