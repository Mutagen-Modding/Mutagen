using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

partial class AVirtualMachineAdapterBinaryCreateTranslation
{
    public static IEnumerable<ScriptEntry> ReadEntries(MutagenFrame frame, ushort objectFormat)
    {
        ushort count = frame.ReadUInt16();
        for (int i = 0; i < count; i++)
        {
            yield return ReadEntry(frame, objectFormat);
        }
    }

    public static ScriptEntry ReadEntry(MutagenFrame frame, ushort objectFormat)
    {
        var scriptName = StringBinaryTranslation.Instance.Parse(frame, stringBinaryType: StringBinaryType.PrependLengthUShort);
        var entry = new ScriptEntry()
        {
            Name = scriptName,
        };
        if (scriptName.IsNullOrEmpty()) return entry;
        entry.Flags = (ScriptEntry.Flag)frame.ReadUInt8();
        FillProperties(frame, objectFormat, entry);
        return entry;
    }

    public static partial void FillBinaryScriptsCustom(MutagenFrame frame, IAVirtualMachineAdapter item)
    {
        item.Scripts.AddRange(ReadEntries(frame, item.ObjectFormat));
    }

    public static ScriptProperty ParseProperty(MutagenFrame frame, ushort objectFormat)
    {
        var name = StringBinaryTranslation.Instance.Parse(frame, stringBinaryType: StringBinaryType.PrependLengthUShort);
        var type = (ScriptProperty.Type)frame.ReadUInt8();
        var flags = (ScriptProperty.Flag)frame.ReadUInt8();
        ScriptProperty prop = type switch
        {
            ScriptProperty.Type.None => new ScriptProperty(),
            ScriptProperty.Type.Object => new ScriptObjectProperty(),
            ScriptProperty.Type.String => new ScriptStringProperty(),
            ScriptProperty.Type.Int => new ScriptIntProperty(),
            ScriptProperty.Type.Float => new ScriptFloatProperty(),
            ScriptProperty.Type.Bool => new ScriptBoolProperty(),
            ScriptProperty.Type.Variable => new ScriptVariableProperty(),
            ScriptProperty.Type.Struct => new ScriptStructProperty(),
            ScriptProperty.Type.ArrayOfObject => new ScriptObjectListProperty(),
            ScriptProperty.Type.ArrayOfString => new ScriptStringListProperty(),
            ScriptProperty.Type.ArrayOfInt => new ScriptIntListProperty(),
            ScriptProperty.Type.ArrayOfFloat => new ScriptFloatListProperty(),
            ScriptProperty.Type.ArrayOfBool => new ScriptBoolListProperty(),
            ScriptProperty.Type.ArrayOfVariable => new ScriptVariableListProperty(),
            ScriptProperty.Type.ArrayOfStruct => new ScriptStructListProperty(),
            _ => throw new NotImplementedException(),
        };
        prop.Name = name;
        prop.Flags = flags;
        switch (prop)
        {
            case ScriptObjectProperty obj:
                FillObject(frame, obj, objectFormat);
                break;
            case ScriptObjectListProperty objList:
                var objListCount = frame.ReadUInt32();
                for (int j = 0; j < objListCount; j++)
                {
                    var subObj = new ScriptObjectProperty();
                    FillObject(frame, subObj, objectFormat);
                    objList.Objects.Add(subObj);
                }
                break;
            case ScriptVariableProperty varProp:
            case ScriptVariableListProperty varPropList:
                throw new NotImplementedException();
            case ScriptStructProperty subStructs:
                subStructs.Members.Add(GetScriptEntryStruct(frame, objectFormat));
                break;
            case ScriptStructListProperty structList:
                var structListCount = frame.ReadUInt32();
                for (int j = 0; j < structListCount; j++)
                {
                    var subStructs = new ScriptEntryStructs();
                    subStructs.Members.SetTo(
                        Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<ScriptProperty>.Instance.Parse(
                            amount: frame.ReadInt32(),
                            reader: frame,
                            transl: (MutagenFrame f, [MaybeNullWhen(false)] out ScriptProperty item, TypedParseParams p) =>
                            {
                                item = ParseProperty(f, objectFormat);
                                return true;
                            }));
                    structList.Structs.Add(subStructs);
                }
                break;
            default:
                prop.CopyInFromBinary(frame);
                break;
        }
        return prop;
    }

    static void FillProperties(MutagenFrame frame, ushort objectFormat, IScriptEntry item, bool isStruct = false)
    {
        var count = isStruct ? frame.ReadUInt32() : frame.ReadUInt16();
        for (int i = 0; i < count; i++)
        {
            item.Properties.Add(ParseProperty(frame, objectFormat));
        }
    }

    static IEnumerable<ScriptProperty> ParseProperties(MutagenFrame frame, ushort objectFormat, bool isStruct = false)
    {
        var count = isStruct ? frame.ReadUInt32() : frame.ReadUInt16();
        for (int i = 0; i < count; i++)
        {
            yield return ParseProperty(frame, objectFormat);
        }
    }

    public static void FillObject(MutagenFrame frame, IScriptObjectProperty obj, ushort objectFormat)
    {
        switch (objectFormat)
        {
            case 2:
                obj.Unused = frame.ReadUInt16();
                obj.Alias = frame.ReadInt16();
                obj.Object.FormKey = FormLinkBinaryTranslation.Instance.Parse(
                    reader: frame,
                    defaultVal: FormKey.Null);
                break;
            case 1:
                obj.Object.FormKey = FormLinkBinaryTranslation.Instance.Parse(
                    reader: frame,
                    defaultVal: FormKey.Null);
                obj.Alias = frame.ReadInt16();
                obj.Unused = frame.ReadUInt16();
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static ScriptEntry GetScriptEntryStruct(MutagenFrame frame, ushort objectFormat)
    {
        var member = new ScriptEntry();
        FillProperties(frame, objectFormat, member, true);
        return member;
    }
}

partial class AVirtualMachineAdapterBinaryWriteTranslation
{
    public static void WriteProperty(MutagenWriter writer, IScriptPropertyGetter property, ushort objFormat)
    {
        writer.Write(property.Name, StringBinaryType.PrependLengthUShort, encoding: writer.MetaData.Encodings.NonTranslated);
        var type = property switch
        {
            ScriptObjectProperty _ => ScriptProperty.Type.Object,
            ScriptStringProperty _ => ScriptProperty.Type.String,
            ScriptIntProperty _ => ScriptProperty.Type.Int,
            ScriptFloatProperty _ => ScriptProperty.Type.Float,
            ScriptBoolProperty _ => ScriptProperty.Type.Bool,
            ScriptVariableProperty _ => ScriptProperty.Type.Variable,
            ScriptStructProperty _ => ScriptProperty.Type.Struct,
            ScriptObjectListProperty _ => ScriptProperty.Type.ArrayOfObject,
            ScriptStringListProperty _ => ScriptProperty.Type.ArrayOfString,
            ScriptIntListProperty _ => ScriptProperty.Type.ArrayOfInt,
            ScriptFloatListProperty _ => ScriptProperty.Type.ArrayOfFloat,
            ScriptBoolListProperty _ => ScriptProperty.Type.ArrayOfBool,
            ScriptVariableListProperty _ => ScriptProperty.Type.ArrayOfVariable,
            ScriptStructListProperty _ => ScriptProperty.Type.ArrayOfStruct,
            ScriptProperty _ => ScriptProperty.Type.None,
            _ => throw new NotImplementedException(),
        };
        writer.Write((byte)type);
        writer.Write((byte)property.Flags);
        switch (property)
        {
            case ScriptObjectProperty obj:
                WriteObject(writer, obj, objFormat);
                break;
            case ScriptObjectListProperty objList:
                var objsList = objList.Objects;
                writer.Write(objsList.Count);
                foreach (var subObj in objsList)
                {
                    WriteObject(writer, subObj, objFormat);
                }
                break;
            case ScriptVariableProperty varProp:
            case ScriptVariableListProperty varPropList:
                throw new NotImplementedException();
            case ScriptStructProperty subStructs:
                WriteScripts(writer, objFormat, (IReadOnlyList<IScriptEntryGetter>)subStructs.Members, true);
                break;
            case ScriptStructListProperty structList:
                var structsList = structList.Structs;
                writer.Write(checked((uint)structsList.Count));
                for (int i = 0; i < structsList.Count; ++i)
                {
                    var subStructs = structsList[i];
                    WriteProperties(writer, objFormat, (IReadOnlyList<IScriptPropertyGetter>)subStructs.Members);
                }

                break;
            default:
                property.WriteToBinary(writer);
                break;
        }
    }

    public static void WriteEntry(
        MutagenWriter writer,
        IScriptEntryGetter entry,
        ushort objFormat,
        bool isStruct = false)
    {
        var properties = entry.Properties;
        if (!isStruct)
        {
            var name = entry.Name;
            StringBinaryTranslation.Instance.Write(writer, name, StringBinaryType.PrependLengthUShort);
            if (!name.IsNullOrWhitespace())
            {
                writer.Write((byte)entry.Flags);
            }
            else
            {
                if (properties.Count > 0)
                {
                    throw new ArgumentException("Entry had no name, but had properties");
                }
                return;
            }
        }

        if (isStruct)
            writer.Write(checked((uint)properties.Count));
        else
            writer.Write(checked((ushort)properties.Count));

        foreach (var property in properties)
        {
            WriteProperty(writer, property, objFormat);
        }
    }

    public static void WriteEntry(
        MutagenWriter writer,
        IScriptEntryStructsGetter entry,
        ushort objFormat)
    {
        var properties = entry.Members;
        writer.Write(checked((uint)properties.Count));

        foreach (var property in properties)
        {
            WriteProperty(writer, property, objFormat);
        }
    }

    public static void WriteScripts(
        MutagenWriter writer,
        ushort objFormat,
        IReadOnlyList<IScriptEntryGetter> scripts,
        bool isStruct = false)
    {
        if (!isStruct)
            writer.Write(checked((ushort)scripts.Count));

        foreach (var entry in scripts)
        {
            WriteEntry(writer, entry, objFormat, isStruct: isStruct);
        }
    }

    public static void WriteProperties(
        MutagenWriter writer,
        ushort objFormat,
        IReadOnlyList<IScriptPropertyGetter> properties)
    {
        writer.Write(checked((uint)properties.Count));
        foreach (var property in properties)
        {
            WriteProperty(writer, property, objFormat);
        }
    }

    public static void WriteObject(MutagenWriter writer, IScriptObjectPropertyGetter obj, ushort objFormat)
    {
        switch (objFormat)
        {
            case 2:
                writer.Write(obj.Unused);
                writer.Write(obj.Alias);
                FormKeyBinaryTranslation.Instance.Write(writer, obj.Object);
                break;
            case 1:
                FormKeyBinaryTranslation.Instance.Write(writer, obj.Object);
                writer.Write(obj.Alias);
                writer.Write(obj.Unused);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static partial void WriteBinaryScriptsCustom(MutagenWriter writer, IAVirtualMachineAdapterGetter item)
    {
        WriteScripts(writer, item.ObjectFormat, item.Scripts);
    }
}
    
partial class AVirtualMachineAdapterBinaryOverlay
{
    public IReadOnlyList<IScriptEntryGetter> Scripts { get; private set; } = null!;

    partial void CustomCtor()
    {
        var frame = new MutagenFrame(new MutagenMemoryReadStream(_structData, _package.MetaData))
        {
            Position = 0x04
        };
        var ret = new ExtendedList<IScriptEntryGetter>();
        ret.AddRange(VirtualMachineAdapterBinaryCreateTranslation.ReadEntries(frame, this.ObjectFormat));
        this.Scripts = ret;
        this.ScriptsEndingPos = checked((int)frame.Position);
    }
}