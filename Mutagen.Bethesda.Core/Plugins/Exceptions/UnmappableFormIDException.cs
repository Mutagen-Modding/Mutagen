using System.Text;
using Mutagen.Bethesda.Plugins.Masters;

namespace Mutagen.Bethesda.Plugins.Exceptions;

public class UnmappableFormIDException : Exception
{
    public FormLinkInformation UnmappableFormKey { get; }
    public IReadOnlyList<ModKey> ListedMasters => MasterPackage
        .Raw.Masters
        .Select(x => x.Master)
        .ToArray();
    public IReadOnlySeparatedMasterPackage MasterPackage { get; }

    public UnmappableFormIDException(
        IFormLinkIdentifier unmappableFormKey, 
        IReadOnlySeparatedMasterPackage masterPackage)
        : base("Could not map FormKey to a master index")
    {
        UnmappableFormKey = FormLinkInformation.Factory(unmappableFormKey);
        MasterPackage = masterPackage;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(UnmappableFormKey);
        sb.AppendLine(" could not be located within:");
        PrintMastersList(sb, ListedMasters, "Raw");

        if (MasterPackage is SeparatedMasterPackage separated)
        {
            if (separated.Full.Count > 0)
            {
                PrintMastersList(sb, separated.Full, nameof(SeparatedMasterPackage.Full));
            }
            if (separated.Medium.Count > 0)
            {
                PrintMastersList(sb, separated.Medium, nameof(SeparatedMasterPackage.Medium));
            }
            if (separated.Light.Count > 0)
            {
                PrintMastersList(sb, separated.Light, nameof(SeparatedMasterPackage.Light));
            }
        }

        return sb.ToString();
    }

    private static void PrintMastersList(StringBuilder sb, IReadOnlyList<ModKey> masters, string name)
    {
        int i = 0;
        sb.AppendLine($"  {name}:");
        foreach (var modKey in masters)
        {
            sb.Append("    [");
            sb.Append(i++);
            sb.Append("] ");
            sb.Append(modKey);
            sb.AppendLine();
        }
    }
}