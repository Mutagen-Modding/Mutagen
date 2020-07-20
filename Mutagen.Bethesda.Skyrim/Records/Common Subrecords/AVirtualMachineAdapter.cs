using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class AVirtualMachineAdapter
    {
        [Flags]
        public enum FragmentFlag
        {
            OnBegin = 0x01,
            OnEnd = 0x02,
        }

        [Flags]
        public enum FragmentChangeFlag
        {
            OnBegin = 0x01,
            OnEnd = 0x02,
            OnChange = 0x04,
        }
    }

    namespace Internals
    {
        partial class AVirtualMachineAdapterBinaryCreateTranslation
        {
            public static IEnumerable<ScriptEntry> ReadEntries(MutagenFrame frame, ushort objectFormat)
            {
                ushort count = frame.ReadUInt16();
                for (int i = 0; i < count; i++)
                {
                    var scriptName = StringBinaryTranslation.Instance.Parse(frame, stringBinaryType: StringBinaryType.PrependLengthUShort);
                    var scriptFlags = (ScriptEntry.Flag)frame.ReadUInt8();
                    var entry = new ScriptEntry()
                    {
                        Name = scriptName,
                        Flags = scriptFlags,
                    };
                    FillProperties(frame, objectFormat, entry);
                    yield return entry;
                }
            }

            static partial void FillBinaryScriptsCustom(MutagenFrame frame, IAVirtualMachineAdapter item)
            {
                item.Scripts.AddRange(ReadEntries(frame, item.ObjectFormat));
            }

            static void FillProperties(MutagenFrame frame, ushort objectFormat, IScriptEntry item)
            {
                var count = frame.ReadUInt16();
                for (int i = 0; i < count; i++)
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
                        ScriptProperty.Type.ArrayOfObject => new ScriptObjectListProperty(),
                        ScriptProperty.Type.ArrayOfString => new ScriptStringListProperty(),
                        ScriptProperty.Type.ArrayOfInt => new ScriptIntListProperty(),
                        ScriptProperty.Type.ArrayOfFloat => new ScriptFloatListProperty(),
                        ScriptProperty.Type.ArrayOfBool => new ScriptBoolListProperty(),
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
                        default:
                            prop.CopyInFromBinary(frame);
                            break;
                    }
                    item.Properties.Add(prop);
                }
            }

            public static void FillObject(MutagenFrame frame, IScriptObjectProperty obj, ushort objectFormat)
            {
                switch (objectFormat)
                {
                    case 2:
                        obj.Unused = frame.ReadUInt16();
                        obj.Alias = frame.ReadInt16();
                        obj.Object = Mutagen.Bethesda.Binary.FormLinkBinaryTranslation.Instance.Parse(
                            frame: frame,
                            defaultVal: FormKey.Null);
                        break;
                    case 1:
                        obj.Object = Mutagen.Bethesda.Binary.FormLinkBinaryTranslation.Instance.Parse(
                            frame: frame,
                            defaultVal: FormKey.Null);
                        obj.Alias = frame.ReadInt16();
                        obj.Unused = frame.ReadUInt16();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public partial class AVirtualMachineAdapterBinaryWriteTranslation
        {
            public static void WriteScripts(
                MutagenWriter writer,
                ushort objFormat,
                IReadOnlyList<IScriptEntryGetter> scripts)
            {
                writer.Write(checked((ushort)scripts.Count));
                foreach (var entry in scripts)
                {
                    writer.Write(entry.Name, StringBinaryType.PrependLengthUShort);
                    writer.Write((byte)entry.Flags);
                    var properties = entry.Properties;
                    writer.Write(checked((ushort)properties.Count));
                    foreach (var property in properties)
                    {
                        writer.Write(property.Name, StringBinaryType.PrependLengthUShort);
                        var type = property switch
                        {
                            ScriptObjectProperty _ => ScriptProperty.Type.Object,
                            ScriptStringProperty _ => ScriptProperty.Type.String,
                            ScriptIntProperty _ => ScriptProperty.Type.Int,
                            ScriptFloatProperty _ => ScriptProperty.Type.Float,
                            ScriptBoolProperty _ => ScriptProperty.Type.Bool,
                            ScriptObjectListProperty _ => ScriptProperty.Type.ArrayOfObject,
                            ScriptStringListProperty _ => ScriptProperty.Type.ArrayOfString,
                            ScriptIntListProperty _ => ScriptProperty.Type.ArrayOfInt,
                            ScriptFloatListProperty _ => ScriptProperty.Type.ArrayOfFloat,
                            ScriptBoolListProperty _ => ScriptProperty.Type.ArrayOfBool,
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
                            default:
                                property.WriteToBinary(writer);
                                break;
                        }
                    }
                }
            }

            public static void WriteObject(MutagenWriter writer, IScriptObjectPropertyGetter obj, ushort objFormat)
            {
                switch (objFormat)
                {
                    case 2:
                        writer.Write(obj.Unused);
                        writer.Write(obj.Alias);
                        writer.Write(writer.MetaData.MasterReferences!.GetFormID(obj.Object.FormKey).Raw);
                        break;
                    case 1:
                        writer.Write(writer.MetaData.MasterReferences!.GetFormID(obj.Object.FormKey).Raw);
                        writer.Write(obj.Alias);
                        writer.Write(obj.Unused);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            static partial void WriteBinaryScriptsCustom(MutagenWriter writer, IAVirtualMachineAdapterGetter item)
            {
                WriteScripts(writer, item.ObjectFormat, item.Scripts);
            }
        }

        public partial class AVirtualMachineAdapterBinaryOverlay
        {
            public IReadOnlyList<IScriptEntryGetter> Scripts { get; private set; } = null!;

            partial void CustomCtor()
            {
                var frame = new MutagenFrame(new MutagenMemoryReadStream(_data, _package.MetaData))
                {
                    Position = 0x04
                };
                var ret = new ExtendedList<IScriptEntryGetter>();
                ret.AddRange(VirtualMachineAdapterBinaryCreateTranslation.ReadEntries(frame, this.ObjectFormat));
                this.Scripts = ret;
                this.ScriptsEndingPos = checked((int)frame.Position);
            }
        }
    }
}
