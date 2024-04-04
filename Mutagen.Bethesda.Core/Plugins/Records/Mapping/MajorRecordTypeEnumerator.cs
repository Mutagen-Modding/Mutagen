using System.Collections.Concurrent;
using Loqui;
using Noggog;
namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public record RecordTypes(Type ClassType, Type GetterType, Type SetterType);

public static class MajorRecordTypeEnumerator
{
	private static readonly ConcurrentDictionary<GameCategory, List<ILoquiRegistration>> Registrations = new();

	private static List<ILoquiRegistration> GetRegistrations(GameCategory cat)
	{
		var categoryString = Enums<GameCategory>.ToStringFast((int) cat);

		return LoquiRegistration.StaticRegister.Registrations
			.Where(x =>
				x.ClassType.Namespace != null
			 && x.ClassType.Namespace.Contains(categoryString, StringComparison.Ordinal)
			 && x.GetterType.IsAssignableTo(typeof(IMajorRecordGetter)))
			.ToList();
	}

	public static IEnumerable<RecordTypes> GetMajorRecordTypesFor(GameCategory cat)
	{
		return Registrations.GetOrAdd(cat, () => GetRegistrations(cat))
			.Select(x => new RecordTypes(x.ClassType, x.GetterType, x.SetterType));
	}
}
