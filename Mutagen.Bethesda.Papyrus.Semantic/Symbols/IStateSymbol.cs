namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public interface IStateSymbol : ISymbol
{
	bool IsAuto { get; }
	IReadOnlyList<ISymbol> Members { get; }
}
