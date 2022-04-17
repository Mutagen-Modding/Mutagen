using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Mutagen.Bethesda.Fallout4;

[Flags]
public enum BipedObjectFlag : uint
{
    HairTop = 0x0000_0001,
    HairLong = 0x0000_0002,
    FaceGenHead = 0x0000_0004,
    Body = 0x0000_0008,
    LeftHand = 0x0000_0010,
    RightHand = 0x0000_0020,
    TorsoUnderArmor = 0x0000_0040,
    LeftArmUnderArmor = 0x0000_0080,
    RightArmUnderArmor = 0x0000_0100,
    LeftLegUnderArmor = 0x0000_0200,
    RightLegUnderArmor = 0x0000_0400,
    TorsoArmor = 0x0000_0800,
    LeftArmArmor = 0x0000_1000,
    RightArmArmor = 0x0000_2000,
    LeftLegArmor = 0x0000_4000,
    RightLegArmor = 0x0000_8000,
    Headband = 0x0001_0000,
    Eyes = 0x0002_0000,
    Beard = 0x0004_0000,
    Mouth = 0x0008_0000,
    Neck = 0x0010_0000,
    Ring = 0x0020_0000,
    Scalp = 0x0040_0000,
    Decapitation = 0x0080_0000,
    Unnamed54 = 0x0100_0000,
    Unnamed55 = 0x0200_0000,
    Unnamed56 = 0x0400_0000,
    Unnamed57 = 0x0800_0000,
    Unnamed58 = 0x1000_0000,
    Shield = 0x2000_0000,
    Pipboy = 0x4000_0000,
    FX = 0x8000_0000
}

public enum BipedObject
{
    None = -1,
    HairTop = 0,
    HairLong = 1,
    FaceGenHead = 2,
    Body = 3,
    LeftHand = 4,
    RightHand = 5,
    TorsoUnderArmor = 6,
    LeftArmUnderArmor = 7,
    RightArmUnderArmor = 8,
    LeftLegUnderArmor = 9,
    RightLegUnderArmor = 10,
    TorsoArmor = 11,
    LeftArmArmor = 12,
    RightArmArmor = 13,
    LeftLegArmor = 14,
    RightLegArmor = 15,
    Headband = 16,
    Eyes = 17,
    Beard = 18,
    Mouth = 19,
    Neck = 20,
    Ring = 21,
    Scalp = 22,
    Decapitation = 23,
    Unnamed54 = 24,
    Unnamed55 = 25,
    Unnamed56 = 26,
    Unnamed57 = 27,
    Unnamed58 = 28,
    Shield = 29,
    Pipboy = 30,
    FX = 31
}

public static class BipedObjectConverter
{
    internal static Dictionary<int, (BipedObject, BipedObjectFlag)> ConversionList => new Dictionary<int, (BipedObject, BipedObjectFlag)>
    {
        { 30, (BipedObject.HairTop, BipedObjectFlag.HairTop) },
        { 31, (BipedObject.HairLong, BipedObjectFlag.HairLong) },
        { 32, (BipedObject.FaceGenHead, BipedObjectFlag.FaceGenHead) },
        { 33, (BipedObject.Body, BipedObjectFlag.Body) },
        { 34, (BipedObject.LeftHand, BipedObjectFlag.LeftHand) },
        { 35, (BipedObject.RightHand, BipedObjectFlag.RightHand) },
        { 36, (BipedObject.TorsoUnderArmor, BipedObjectFlag.TorsoUnderArmor) },
        { 37, (BipedObject.LeftArmUnderArmor, BipedObjectFlag.LeftArmUnderArmor) },
        { 38, (BipedObject.RightArmUnderArmor, BipedObjectFlag.RightArmUnderArmor) },
        { 39, (BipedObject.LeftLegUnderArmor, BipedObjectFlag.LeftLegUnderArmor) },
        { 40, (BipedObject.RightLegUnderArmor, BipedObjectFlag.RightLegUnderArmor) },
        { 41, (BipedObject.TorsoArmor, BipedObjectFlag.TorsoArmor) },
        { 42, (BipedObject.LeftArmArmor, BipedObjectFlag.LeftArmArmor) },
        { 43, (BipedObject.RightArmArmor, BipedObjectFlag.RightArmArmor) },
        { 44, (BipedObject.LeftLegArmor, BipedObjectFlag.LeftLegArmor) },
        { 45, (BipedObject.RightLegArmor, BipedObjectFlag.RightLegArmor) },
        { 46, (BipedObject.Headband, BipedObjectFlag.Headband) },
        { 47, (BipedObject.Eyes, BipedObjectFlag.Eyes) },
        { 48, (BipedObject.Beard, BipedObjectFlag.Beard) },
        { 49, (BipedObject.Mouth, BipedObjectFlag.Mouth) },
        { 50, (BipedObject.Neck, BipedObjectFlag.Neck) },
        { 51, (BipedObject.Ring, BipedObjectFlag.Ring) },
        { 52, (BipedObject.Scalp, BipedObjectFlag.Scalp) },
        { 53, (BipedObject.Decapitation, BipedObjectFlag.Decapitation) },
        { 54, (BipedObject.Unnamed54, BipedObjectFlag.Unnamed54) },
        { 55, (BipedObject.Unnamed55, BipedObjectFlag.Unnamed55) },
        { 56, (BipedObject.Unnamed56, BipedObjectFlag.Unnamed56) },
        { 57, (BipedObject.Unnamed57, BipedObjectFlag.Unnamed57) },
        { 58, (BipedObject.Unnamed58, BipedObjectFlag.Unnamed58) },
        { 59, (BipedObject.Shield, BipedObjectFlag.Shield) },
        { 60, (BipedObject.Pipboy, BipedObjectFlag.Pipboy) },
        { 61, (BipedObject.FX, BipedObjectFlag.FX) }
    };

    /// <summary>
    /// returns the enum by providing the slot - e.g. 'Ring' when called with 51, 
    /// returns 'None' when called with -1,
    /// throws an exception if the slot does not represent an enum
    /// </summary>
    /// <param name="slot">the slot to convert</param>
    /// <returns>the enum by providing the slot - e.g. 'Ring' when called with 51, 'None' when called with -1</returns>
    public static BipedObject BipedObjectBySlot(int slot)
    {
        if (slot == -1)
            return BipedObject.None;

        if (!ConversionList.TryGetValue(slot, out (BipedObject, BipedObjectFlag) result))
            throw new ArgumentException($"Slot '{slot}' not found");

        return result.Item1;
    }

    /// <summary>
    /// returns the flag enum by providing the slot - e.g. 'Ring' when called with 51, 
    /// throws an exception if the slot does not represent a flag enum
    /// </summary>
    /// <param name="slot">the slot to convert</param>
    /// <returns>the enum by providing the slot - e.g. 'Ring' when called with 51</returns>
    public static BipedObjectFlag BipedObjectFlagBySlot(int slot)
    {
        if (!ConversionList.TryGetValue(slot, out (BipedObject, BipedObjectFlag) result))
            throw new ArgumentException($"Slot '{slot}' not found");

        return result.Item2;
    }
}

public static class BipedObjectExtensions
{
    /// <summary>
    /// converts the object enum to the corresponding flag enum, throws an exception when called with 'None'
    /// </summary>
    /// <param name="bipedObject"></param>
    /// <returns></returns>
    public static BipedObjectFlag ToFlagEnum(this BipedObject bipedObject)
    {
        if (bipedObject == BipedObject.None)
            throw new ArgumentException("Object enum 'None' does not exist as flag enum");
        return (BipedObjectFlag)Enum.Parse(typeof(BipedObjectFlag), bipedObject.ToString());
    }

    /// <summary>
    /// returns the slot number for the BipedObjectEnum - e.g. 51 for 'Ring'
    /// </summary>
    /// <param name="bipedObject">the enum to convert</param>
    /// <returns>the slot number for the BipedObjectEnum - e.g. 51 for 'Ring', -1 when called with "None"</returns>
    public static int ToSlot(this BipedObject bipedObject)
    {
        if (bipedObject == BipedObject.None)
            return -1;
        return BipedObjectConverter.ConversionList.First(itm => itm.Value.Item1 == bipedObject).Key;
    }
}

public static class BipedObjectFlagExtensions
{
    /// <summary>
    /// converts the flag enum to the corresponding object enum
    /// </summary>
    /// <param name="bipedObjectFlag"></param>
    /// <returns></returns>
    public static BipedObject ToObjectEnum(this BipedObjectFlag bipedObjectFlag)
    {
        return (BipedObject)Enum.Parse(typeof(BipedObject), bipedObjectFlag.ToString());
    }

    /// <summary>
    /// returns the slot number for the BipedObjectFlagEnum - e.g. 51 for 'Ring'
    /// </summary>
    /// <param name="bipedObjectFlag">the enum to convert</param>
    /// <returns>the slot number for the BipedObjectEnum - e.g. 51 for 'Ring'</returns>
    public static int ToSlot(this BipedObjectFlag bipedObjectFlag)
    {
        return BipedObjectConverter.ConversionList.First(itm => itm.Value.Item2 == bipedObjectFlag).Key;
    }
}