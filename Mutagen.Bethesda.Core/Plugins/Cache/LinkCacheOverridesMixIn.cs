using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Cache;

public static class LinkCacheOverridesMixIn
{
	#region GetPreviousOverrideSimpleContexts
	/// <summary>
	/// Gets all previous overrides of a record in the link cache, based on the specified override mod key and resolve target.
	/// </summary>
	/// <param name="cache">Link cache to search for previous overrides</param>
	/// <param name="record">The record for which to find previous overrides</param>
	/// <param name="overrideModKey">The mod key of the override to start searching from</param>
	/// <param name="target">Resolution target to look up previous overrides from</param>
	/// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
	/// <returns>Enumerable of all previous overrides of the record, based on the specified override mod key and resolve target</returns>
	public static IEnumerable<IModContext<TMajor>> GetPreviousOverrideSimpleContexts<TMajor>(this ILinkCache cache, TMajor record, ModKey overrideModKey, ResolveTarget target = ResolveTarget.Winner)
		where TMajor : class, IMajorRecordGetter
	{
		return target switch
		{
			ResolveTarget.Winner => cache.ResolveAllSimpleContexts(record)
				.SkipWhile(x => x.ModKey != overrideModKey)
				.Skip(1),
			_ => cache.ResolveAllSimpleContexts(record, ResolveTarget.Origin)
				.TakeWhile(x => x.ModKey != overrideModKey)
		};
	}

	/// <summary>
	/// Gets all previous overrides of a record in the link cache, based on the specified override mod key and resolve target.
	/// </summary>
	/// <param name="cache">Link cache to search for previous overrides</param>
	/// <param name="formKey">FormKey to find previous overrides for</param>
	/// <param name="overrideModKey">The mod key of the override to start searching from</param>
	/// <param name="target">Resolution target to look up previous overrides from</param>
	/// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
	/// <returns>Enumerable of all previous overrides of the record, based on the specified override mod key and resolve target</returns>
	public static IEnumerable<IModContext<TMajor>> GetPreviousOverrideSimpleContexts<TMajor>(this ILinkCache cache, FormKey formKey, ModKey overrideModKey, ResolveTarget target = ResolveTarget.Winner)
		where TMajor : class, IMajorRecordGetter
	{
		return target switch
		{
			ResolveTarget.Winner => cache.ResolveAllSimpleContexts<TMajor>(formKey)
				.SkipWhile(x => x.ModKey != overrideModKey)
				.Skip(1),
			_ => cache.ResolveAllSimpleContexts<TMajor>(formKey, ResolveTarget.Origin)
				.TakeWhile(x => x.ModKey != overrideModKey)
		};
	}

	/// <summary>
	/// Gets all previous overrides of a record in the link cache, based on the specified override mod key and resolve target.
	/// </summary>
	/// <param name="cache">Link cache to search for previous overrides</param>
	/// <param name="formLink">FormLink to find previous overrides for</param>
	/// <param name="overrideModKey">The mod key of the override to start searching from</param>
	/// <param name="target">Resolution target to look up previous overrides from</param>
	/// <returns>Enumerable of all previous overrides of the record, based on the specified override mod key and resolve target</returns>
	public static IEnumerable<IModContext<IMajorRecordGetter>> GetPreviousOverrideSimpleContexts(this ILinkCache cache, IFormLinkIdentifier formLink, ModKey overrideModKey, ResolveTarget target = ResolveTarget.Winner)
	{
		return target switch
		{
			ResolveTarget.Winner => cache.ResolveAllSimpleContexts(formLink)
				.SkipWhile(x => x.ModKey != overrideModKey)
				.Skip(1),
			_ => cache.ResolveAllSimpleContexts(formLink, ResolveTarget.Origin)
				.TakeWhile(x => x.ModKey != overrideModKey)
		};
	}

	/// <summary>
	/// Gets all previous overrides of a record in the link cache, based on the specified override mod key and resolve target.
	/// </summary>
	/// <param name="cache">Link cache to search for previous overrides</param>
	/// <param name="formKey">FormKey to find previous overrides for</param>
	/// <param name="type">The type of record to look up</param>
	/// <param name="overrideModKey">The mod key of the override to start searching from</param>
	/// <param name="target">Resolution target to look up previous overrides from</param>
	/// <returns>Enumerable of all previous overrides of the record, based on the specified override mod key and resolve target</returns>
	public static IEnumerable<IModContext<IMajorRecordGetter>> GetPreviousOverrideSimpleContexts(this ILinkCache cache, FormKey formKey, Type type, ModKey overrideModKey, ResolveTarget target = ResolveTarget.Winner)
	{
		return target switch
		{
			ResolveTarget.Winner => cache.ResolveAllSimpleContexts(formKey, type)
				.SkipWhile(x => x.ModKey != overrideModKey)
				.Skip(1),
			_ => cache.ResolveAllSimpleContexts(formKey, type, ResolveTarget.Origin)
				.TakeWhile(x => x.ModKey != overrideModKey)
		};
	}

	/// <summary>
	/// Gets all previous overrides of a record in the link cache, based on the specified override mod key and resolve target.
	/// </summary>
	/// <param name="cache">Link cache to search for previous overrides</param>
	/// <param name="formKey">FormKey to find previous overrides for</param>
	/// <param name="overrideModKey">The mod key of the override to start searching from</param>
	/// <param name="target">Resolution target to look up previous overrides from</param>
	/// <returns>Enumerable of all previous overrides of the record, based on the specified override mod key and resolve target</returns>
	[Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
	public static IEnumerable<IModContext<IMajorRecordGetter>> GetPreviousOverrideSimpleContexts(this ILinkCache cache, FormKey formKey, ModKey overrideModKey, ResolveTarget target = ResolveTarget.Winner)
	{
		return target switch
		{
			ResolveTarget.Winner => cache.ResolveAllSimpleContexts(formKey)
				.SkipWhile(x => x.ModKey != overrideModKey)
				.Skip(1),
			_ => cache.ResolveAllSimpleContexts(formKey, ResolveTarget.Origin)
				.TakeWhile(x => x.ModKey != overrideModKey)
		};
	}
	#endregion

	#region GetPreviousOverrides
	/// <summary>
	/// Gets all previous overrides of a record in the link cache, based on the specified override mod key and resolve target.
	/// </summary>
	/// <param name="cache">Link cache to search for previous overrides</param>
	/// <param name="record">The record for which to find previous overrides</param>
	/// <param name="overrideModKey">The mod key of the override to start searching from</param>
	/// <param name="target">Resolution target to look up previous overrides from</param>
	/// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
	/// <returns>Enumerable of all previous overrides of the record, based on the specified override mod key and resolve target</returns>
	public static IEnumerable<TMajor> GetPreviousOverrides<TMajor>(this ILinkCache cache, TMajor record, ModKey overrideModKey, ResolveTarget target = ResolveTarget.Winner)
		where TMajor : class, IMajorRecordGetter
	{
		return cache.GetPreviousOverrideSimpleContexts(record, overrideModKey, target)
			.Select(x => x.Record);
	}

	/// <summary>
	/// Gets all previous overrides of a record in the link cache, based on the specified override mod key and resolve target.
	/// </summary>
	/// <param name="cache">Link cache to search for previous overrides</param>
	/// <param name="formKey">FormKey to find previous overrides for</param>
	/// <param name="overrideModKey">The mod key of the override to start searching from</param>
	/// <param name="target">Resolution target to look up previous overrides from</param>
	/// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
	/// <returns>Enumerable of all previous overrides of the record, based on the specified override mod key and resolve target</returns>
	public static IEnumerable<TMajor> GetPreviousOverrides<TMajor>(this ILinkCache cache, FormKey formKey, ModKey overrideModKey, ResolveTarget target = ResolveTarget.Winner)
		where TMajor : class, IMajorRecordGetter
	{
		return cache.GetPreviousOverrideSimpleContexts<TMajor>(formKey, overrideModKey, target)
			.Select(x => x.Record);
	}

	/// <summary>
	/// Gets all previous overrides of a record in the link cache, based on the specified override mod key and resolve target.
	/// </summary>
	/// <param name="cache">Link cache to search for previous overrides</param>
	/// <param name="formLink">FormLink to find previous overrides for</param>
	/// <param name="overrideModKey">The mod key of the override to start searching from</param>
	/// <param name="target">Resolution target to look up previous overrides from</param>
	/// <returns>Enumerable of all previous overrides of the record, based on the specified override mod key and resolve target</returns>
	public static IEnumerable<IMajorRecordGetter> GetPreviousOverrides(this ILinkCache cache, IFormLinkIdentifier formLink, ModKey overrideModKey, ResolveTarget target = ResolveTarget.Winner)
	{
		return cache.GetPreviousOverrideSimpleContexts(formLink, overrideModKey, target)
			.Select(x => x.Record);
	}

	/// <summary>
	/// Gets all previous overrides of a record in the link cache, based on the specified override mod key and resolve target.
	/// </summary>
	/// <param name="cache">Link cache to search for previous overrides</param>
	/// <param name="formKey">FormKey to find previous overrides for</param>
	/// <param name="type">The type of record to look up</param>
	/// <param name="overrideModKey">The mod key of the override to start searching from</param>
	/// <param name="target">Resolution target to look up previous overrides from</param>
	/// <returns>Enumerable of all previous overrides of the record, based on the specified override mod key and resolve target</returns>
	public static IEnumerable<IMajorRecordGetter> GetPreviousOverrides(this ILinkCache cache, FormKey formKey, Type type, ModKey overrideModKey, ResolveTarget target = ResolveTarget.Winner)
	{
		return cache.GetPreviousOverrideSimpleContexts(formKey, type, overrideModKey, target)
			.Select(x => x.Record);
	}

	/// <summary>
	/// Gets all previous overrides of a record in the link cache, based on the specified override mod key and resolve target.
	/// </summary>
	/// <param name="cache">Link cache to search for previous overrides</param>
	/// <param name="formKey">FormKey to find previous overrides for</param>
	/// <param name="overrideModKey">The mod key of the override to start searching from</param>
	/// <param name="target">Resolution target to look up previous overrides from</param>
	/// <returns>Enumerable of all previous overrides of the record, based on the specified override mod key and resolve target</returns>
	[Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
	public static IEnumerable<IMajorRecordGetter> GetPreviousOverrides(this ILinkCache cache, FormKey formKey, ModKey overrideModKey, ResolveTarget target = ResolveTarget.Winner)
	{
		return cache.GetPreviousOverrideSimpleContexts(formKey, overrideModKey, target)
			.Select(x => x.Record);
	}
	#endregion
}
