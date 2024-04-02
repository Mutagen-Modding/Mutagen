using Loqui;
using Noggog;
namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public static class MajorRecordTypeEnumerator
{
	private static readonly Dictionary<GameCategory, List<ILoquiRegistration>> Registrations = new();

	public static List<ILoquiRegistration> GetRegistrations(GameCategory cat)
	{
		var gameNamespace = "Mutagen.Bethesda." + cat;

		return LoquiRegistration.StaticRegister.Registrations
			.Where(x =>
				string.Equals(x.ClassType.Namespace, gameNamespace, StringComparison.Ordinal)
			 && x.GetterType.IsAssignableTo(typeof(IMajorRecordGetter)))
			.ToList();
	}

	IEnumerable<ILoquiRegistration> GetMajorRecordRegistrationsFor(GameCategory cat)
	{
		return Registrations.GetOrAdd(cat, () => GetRegistrations(cat));
	}

	IEnumerable<Type> GetMajorRecordClassTypesFor(GameCategory cat)
	{
		return Registrations.GetOrAdd(cat, () => GetRegistrations(cat))
			.Select(x => x.ClassType);
	}

	IEnumerable<Type> GetMajorRecordGetterTypesFor(GameCategory cat)
	{
		return Registrations.GetOrAdd(cat, () => GetRegistrations(cat))
			.Select(x => x.GetterType);
	}

	IEnumerable<Type> GetMajorRecordSetterTypesFor(GameCategory cat)
	{
		return Registrations.GetOrAdd(cat, () => GetRegistrations(cat))
			.Select(x => x.SetterType);
	}
}
