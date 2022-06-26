using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public class VirtualMachineAdapterAspect : AspectFieldInterfaceDefinition
{
    public VirtualMachineAdapterAspect()
        : base(
            "IHaveVirtualMachineAdapter",
            AspectSubInterfaceDefinition.Factory(
                "IHaveVirtualMachineAdapter",
                Registrations,
                (_, f) => Test(f)))
    {
        FieldActions = new()
        {
            new(LoquiInterfaceType.Direct, VirtualMachineMemberName, (o, tg, sb) =>
            {
                sb.AppendLine($"IAVirtualMachineAdapterGetter? IHaveVirtualMachineAdapterGetter.VirtualMachineAdapter => this.{VirtualMachineMemberName(o)};");
            }),
            new(LoquiInterfaceType.IGetter, VirtualMachineMemberName, (o, tg, sb) =>
            {
                sb.AppendLine($"IAVirtualMachineAdapterGetter? IHaveVirtualMachineAdapterGetter.VirtualMachineAdapter => this.{VirtualMachineMemberName(o)};");
            })
        };
    }

    public static IEnumerable<(string Name, bool Setter)> Registrations
    {
        get
        {
            // yield return ($"IHaveVirtualMachineAdapter", true);
            yield return ($"IHaveVirtualMachineAdapterGetter", false);
        }
    }

    public static bool Test(Dictionary<string, TypeGeneration> allFields)
        => allFields.Any(f =>
        {
            if (f.Value is not LoquiType loqui) return false;
            return loqui.TargetObjectGeneration?.BaseClass?.Name == "AVirtualMachineAdapter";
        });

    public static string VirtualMachineMemberName(ObjectGeneration o)
    {
        foreach (var field in o.IterateFields(includeBaseClass: true))
        {
            if (field is not LoquiType loqui) continue;
            if (loqui.TargetObjectGeneration?.BaseClass?.Name != "AVirtualMachineAdapter") continue;
            return field.Name;
        }

        throw new ArgumentException();
    }
}