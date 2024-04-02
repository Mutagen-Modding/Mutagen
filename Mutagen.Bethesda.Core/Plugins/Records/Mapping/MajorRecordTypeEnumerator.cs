using Loqui;
using Noggog;
namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public static class MajorRecordTypeEnumerator
{
	private static readonly Dictionary<GameCategory, List<ILoquiRegistration>> Registrations = new();

	private static List<ILoquiRegistration> GetRegistrations(GameCategory cat)
	{
		var gameNamespace = "Mutagen.Bethesda." + cat;

		return LoquiRegistration.StaticRegister.Registrations
			.Where(x =>
				string.Equals(x.ClassType.Namespace, gameNamespace, StringComparison.Ordinal)
			 && x.GetterType.IsAssignableTo(typeof(IMajorRecordGetter)))
			.ToList();
	}

	public static IEnumerable<ILoquiRegistration> GetMajorRecordRegistrationsFor(GameCategory cat)
	{
		return Registrations.GetOrAdd(cat, () => GetRegistrations(cat));
	}

	public static IEnumerable<Type> GetMajorRecordClassTypesFor(GameCategory cat)
	{
		return Registrations.GetOrAdd(cat, () => GetRegistrations(cat))
			.Select(x => x.ClassType);
	}

	public static IEnumerable<Type> GetMajorRecordGetterTypesFor(GameCategory cat)
	{
		return Registrations.GetOrAdd(cat, () => GetRegistrations(cat))
			.Select(x => x.GetterType);
	}

	public static IEnumerable<Type> GetMajorRecordSetterTypesFor(GameCategory cat)
	{
		return Registrations.GetOrAdd(cat, () => GetRegistrations(cat))
			.Select(x => x.SetterType);
	}
}
