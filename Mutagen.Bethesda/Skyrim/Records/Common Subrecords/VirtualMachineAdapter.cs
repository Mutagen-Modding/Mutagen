using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim.Internals
{
    partial class VirtualMachineAdapterBinaryCreateTranslation
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

        static partial void FillBinaryScriptsCustom(MutagenFrame frame, IVirtualMachineAdapter item)
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
                    //ScriptProperty.Type.ArrayOfString => new ScriptStringListProperty(),
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
                        break;
                    case ScriptObjectListProperty objList:
                        throw new NotImplementedException();
                        break;
                    default:
                        prop.CopyInFromBinary(frame);
                        break;
                }
                item.Properties.Add(prop);
            }
        }
    }

    public partial class VirtualMachineAdapterBinaryWriteTranslation
    {
        static partial void WriteBinaryScriptsCustom(MutagenWriter writer, IVirtualMachineAdapterGetter item)
        {
            var scripts = item.Scripts;
            writer.Write(checked((ushort)scripts.Count));
            foreach (var entry in item.Scripts)
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
                        //ScriptStringListProperty _ => ScriptProperty.Type.ArrayOfString,
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
                            switch (item.ObjectFormat)
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
                            break;
                        default:
                            property.WriteToBinary(writer);
                            break;
                    }
                }
            }
        }
    }

    public partial class VirtualMachineAdapterBinaryOverlay
    {
        public IReadOnlyList<IScriptEntryGetter> Scripts
        {
            get
            {
                var frame = new MutagenFrame(new MutagenMemoryReadStream(_data, _package.MetaData))
                {
                    Position = 0x04
                };
                var ret = new ExtendedList<IScriptEntryGetter>();
                ret.AddRange(VirtualMachineAdapterBinaryCreateTranslation.ReadEntries(frame, this.ObjectFormat));
                return ret;
            }
        }
    }
}
