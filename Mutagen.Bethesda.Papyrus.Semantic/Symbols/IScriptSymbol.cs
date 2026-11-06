namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public interface IScriptSymbol : ISymbol
{
	IScriptSymbol? BaseScript { get; }
	IReadOnlyList<ISymbol> Members { get; }
}
