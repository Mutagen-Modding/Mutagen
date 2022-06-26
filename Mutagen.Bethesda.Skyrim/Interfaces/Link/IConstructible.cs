namespace Mutagen.Bethesda.Skyrim;
// All items that implement IConstructible also implement IItem, except LLists

public partial interface IConstructible : IItem
{
}

public partial interface IConstructibleGetter : IItemGetter
{
}