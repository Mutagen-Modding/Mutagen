namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public sealed class StateSymbol : IStateSymbol
{
	private readonly List<ISymbol> _members = [];

	internal StateSymbol(
		string name,
		bool isAuto,
		ScriptSymbol containingScript,
		DeclaringSyntaxReference declaringSyntaxReference)
	{
		Name = name;
		IsAuto = isAuto;
		ContainingSymbol = containingScript;
		DeclaringSyntaxReference = declaringSyntaxReference;
	}

	public string Name { get; }
	public SymbolKind Kind => SymbolKind.State;
	public ISymbol ContainingSymbol { get; }
	public DeclaringSyntaxReference DeclaringSyntaxReference { get; }
	public bool IsAuto { get; }
	public IReadOnlyList<ISymbol> Members => _members;

	internal void AddMember(ISymbol member) => _members.Add(member);

	public override string ToString() => Name;
}
