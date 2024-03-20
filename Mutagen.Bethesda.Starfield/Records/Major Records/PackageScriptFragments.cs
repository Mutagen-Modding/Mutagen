using Mutagen.Bethesda.Plugins.Binary.Streams;
using static Mutagen.Bethesda.Starfield.PackageScriptFragmentsBinaryCreateTranslation;

namespace Mutagen.Bethesda.Starfield;

partial class PackageScriptFragmentsBinaryCreateTranslation
{
    [Flags]
    public enum Flag
    {
        OnBegin = 0x01,
        OnEnd = 0x02,
        OnChange = 0x04,
    }

    public static PackageScriptFragments ReadFragments(MutagenFrame frame, ushort objectFormat)
    {
        var ret = new PackageScriptFragments();
        FillFragments(frame, objectFormat, ret);
        return ret;
    }

    public static void FillFragments(MutagenFrame frame, ushort objectFormat, IPackageScriptFragments ret)
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
        if (flag.HasFlag(Flag.OnChange))
        {
            ret.OnChange = ScriptFragment.CreateFromBinary(frame);
        }
    }
}

partial class PackageScriptFragmentsBinaryWriteTranslation
{
    public static void WriteFragments(MutagenWriter writer, IPackageScriptFragmentsGetter item, ushort objectFormat)
    {
        writer.Write(item.ExtraBindDataVersion);
        var begin = item.OnBegin;
        var end = item.OnEnd;
        var change = item.OnChange;
        Flag flag = default;
        if (begin != null)
        {
            flag |= Flag.OnBegin;
        }
        if (end != null)
        {
            flag |= Flag.OnEnd;
        }
        if (change != null)
        {
            flag |= Flag.OnChange;
        }
        writer.Write((byte)flag);
        AVirtualMachineAdapterBinaryWriteTranslation.WriteEntry(writer, item.Script, objectFormat);
        begin?.WriteToBinary(writer);
        end?.WriteToBinary(writer);
        change?.WriteToBinary(writer);
    }
}

partial class PackageScriptFragmentsBinaryOverlay
{
    public IScriptEntryGetter Script => throw new NotImplementedException();
    public IScriptFragmentGetter? OnBegin => throw new NotImplementedException();
    public IScriptFragmentGetter? OnEnd => throw new NotImplementedException();
    public IScriptFragmentGetter? OnChange => throw new NotImplementedException();
}
