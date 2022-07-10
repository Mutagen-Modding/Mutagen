namespace Mutagen.Bethesda.Plugins.Internals;

internal static class Constants
{
    public static readonly sbyte HeaderLength = 4;
    public const string TriggeringRecordTypeMember = "TriggeringRecordType";
    public const string PartialFormMember = "IsPartialFormable";
    public const string SubgroupsMember = "SubgroupTypes";
    public const string GrupRecordTypeMember = "GrupRecordType";
    public const string EdidLinked = "edidLinked";
    public static readonly RecordType Group = new("GRUP");
    public const int LightMasterLimit = 2048;
    public const int MasterFlag = 0x0000_0001;
    public const int CompressedFlag = 0x0004_0000;
    public const int Localized = 0x0000_0080;
    public const int PartialFormFlag = 0x0000_4000;
    public const int DeletedFlag = 0x0000_0020;
    public const int InitiallyDisabled = 0x0000_0800;
    public const int Ignored = 0x0000_1000;
}