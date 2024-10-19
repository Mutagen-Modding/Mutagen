using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using Loqui;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public record RecordTypes(Type ClassType, Type GetterType, Type SetterType);

public static class MajorRecordTypeEnumerator
{
	private static readonly ConcurrentDictionary<GameCategory, ImmutableArray<RecordTypes>> MajorRecordRegistrations = new();
	private static readonly ConcurrentDictionary<GameCategory, ImmutableArray<RecordTypes>> TopLevelMajorRecordRegistrations = new();

	static MajorRecordTypeEnumerator()
	{
		Warmup.Init();
	}

	private static ImmutableArray<RecordTypes> GetMajorRecordRegistrations(GameCategory cat)
	{
		var categoryString = Enums<GameCategory>.ToStringFast((int) cat);

		return LoquiRegistration.StaticRegister.Registrations
			.Where(x =>
				x.ClassType.Namespace != null
			 && x.ClassType.Namespace.Contains(categoryString, StringComparison.Ordinal)
			 && x.GetterType.IsAssignableTo(typeof(IMajorRecordGetter)))
            .Select(x => new RecordTypes(x.ClassType, x.GetterType, x.SetterType))
            .ToImmutableArray();
	}

	public static IEnumerable<RecordTypes> GetMajorRecordTypesFor(GameCategory cat)
	{
		return MajorRecordRegistrations.GetOrAdd(cat, () => GetMajorRecordRegistrations(cat));
	}

	private static ImmutableArray<RecordTypes> GetTopLevelMajorRecordRegistrations(GameCategory cat)
	{
		return cat.ToModRegistration().GetterType
			.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
			.Where(p => p.PropertyType.IsGenericType && p.PropertyType.IsAssignableTo(typeof(IGroupGetter)))
			.Select(p =>
				LoquiRegistration.StaticRegister.Registrations
				.FirstOrDefault(r => r.GetterType == p.PropertyType.GetGenericArguments()[0]))
			.WhereNotNull()
			.Select(r => new RecordTypes(r.ClassType, r.GetterType, r.SetterType))
			.ToImmutableArray();
	}

	public static IEnumerable<RecordTypes> GetTopLevelMajorRecordTypesFor(GameCategory cat)
	{
		return TopLevelMajorRecordRegistrations.GetOrAdd(cat, () => GetTopLevelMajorRecordRegistrations(cat));
	}
}
