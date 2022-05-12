using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;

namespace Mutagen.Bethesda.Skyrim;

partial class QuestFragmentAliasBinaryCreateTranslation
{
    public static partial void FillBinaryPropertyCustom(MutagenFrame frame, IQuestFragmentAlias item)
    {
        // Preparse object format
        var pos = frame.Position;
        frame.Position += 10;
        var format = frame.ReadUInt16();
        frame.Position = pos;

        var obj = new ScriptObjectProperty();
        AVirtualMachineAdapterBinaryCreateTranslation.FillObject(frame, obj, format);
        item.Property = obj;
    }

    public static partial void FillBinaryScriptsCustom(MutagenFrame frame, IQuestFragmentAlias item)
    {
        item.Scripts.AddRange(AVirtualMachineAdapterBinaryCreateTranslation.ReadEntries(frame, item.ObjectFormat));
    }
}

partial class QuestFragmentAliasBinaryWriteTranslation
{
    public static partial void WriteBinaryScriptsCustom(MutagenWriter writer, IQuestFragmentAliasGetter item)
    {
        AVirtualMachineAdapterBinaryWriteTranslation.WriteScripts(writer, item.ObjectFormat, item.Scripts);
    }

    public static partial void WriteBinaryPropertyCustom(MutagenWriter writer, IQuestFragmentAliasGetter item)
    {
        AVirtualMachineAdapterBinaryWriteTranslation.WriteObject(writer, item.Property, item.ObjectFormat);
    }
}

partial class QuestFragmentAliasBinaryOverlay
{
    public IReadOnlyList<IScriptEntryGetter> Scripts { get; private set; } = null!;

    public partial IScriptObjectPropertyGetter GetPropertyCustom(int location) => throw new NotImplementedException();

    partial void CustomCtor()
    {
        var frame = new MutagenFrame(new MutagenMemoryReadStream(_data, _package.MetaData))
        {
            Position = PropertyEndingPos + 0x4
        };
        var ret = new ExtendedList<IScriptEntryGetter>();
        ret.AddRange(VirtualMachineAdapterBinaryCreateTranslation.ReadEntries(frame, this.ObjectFormat));
        this.Scripts = ret;
        this.ScriptsEndingPos = checked((int)frame.Position);
    }
}