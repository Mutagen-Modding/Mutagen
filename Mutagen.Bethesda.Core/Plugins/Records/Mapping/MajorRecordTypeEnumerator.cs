using Loqui;
using Noggog;
namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public record RecordTypes(Type ClassType, Type GetterType, Type SetterType);

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

	public static IEnumerable<RecordTypes> GetMajorRecordTypesFor(GameCategory cat)
	{
		return Registrations.GetOrAdd(cat, () => GetRegistrations(cat))
			.Select(x => new RecordTypes(x.ClassType, x.GetterType, x.SetterType));
	}
}
