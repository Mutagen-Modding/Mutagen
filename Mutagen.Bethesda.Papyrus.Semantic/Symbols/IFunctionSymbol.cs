namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public interface IFunctionSymbol : ISymbol
{
	TypeSymbol? ReturnType { get; }
	IReadOnlyList<IParameterSymbol> Parameters { get; }
	IReadOnlyList<IVariableSymbol> Locals { get; }
	bool IsNative { get; }
	bool IsGlobal { get; }
}
