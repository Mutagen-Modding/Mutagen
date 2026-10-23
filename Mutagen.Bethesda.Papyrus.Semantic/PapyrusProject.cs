using Mutagen.Bethesda.Papyrus.Syntax.Parser;

namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>
/// An immutable set of script documents that compile together. Editing is
/// fork-on-write: <c>With...</c> methods return a new project that shares the
/// unchanged documents, and the forked codebase reuses the semantic models of
/// those documents instead of rebuilding them. The codebase is built lazily on
/// first access and cached.
/// </summary>
public sealed class PapyrusProject
{
    private readonly Dictionary<string, PapyrusDocument> _documents;
    private readonly Lock _codebaseLock = new();
    private readonly PapyrusCodebase? _baseCodebase;

    private PapyrusProject(Dictionary<string, PapyrusDocument> documents, PapyrusCodebase? baseCodebase = null)
    {
        _documents = documents;
        _baseCodebase = baseCodebase;
        Documents = [.. documents.Values];
    }

    public static PapyrusProject FromDocuments(IEnumerable<PapyrusDocument> documents)
    {
        var map = new Dictionary<string, PapyrusDocument>(StringComparer.OrdinalIgnoreCase);
        foreach (var document in documents)
        {
            // Duplicate script names: last one wins, matching codebase behavior.
            map[document.Name] = document;
        }
        return new PapyrusProject(map);
    }

    /// <summary>Loads every .psc file directly inside the given directories.</summary>
    public static PapyrusProject FromDirectories(params string[] directories)
    {
        var documents = new List<PapyrusDocument>();
        foreach (var directory in directories)
        {
            if (!Directory.Exists(directory)) continue;
            foreach (var file in Directory.EnumerateFiles(directory, "*.psc", SearchOption.TopDirectoryOnly))
            {
                documents.Add(PapyrusDocument.Parse(File.ReadAllText(file), file));
            }
        }
        return FromDocuments(documents);
    }

    public static PapyrusProject FromFiles(params string[] files)
    {
        var documents = new List<PapyrusDocument>();
        foreach (var file in files)
        {
            if (!File.Exists(file)) continue;
            documents.Add(PapyrusDocument.Parse(File.ReadAllText(file), file));
        }
        return FromDocuments(documents);
    }

    public IReadOnlyList<PapyrusDocument> Documents { get; }

    public PapyrusDocument? GetDocument(string scriptName) => _documents.GetValueOrDefault(scriptName);

    /// <summary>The codebase of all documents; built on first access and cached.</summary>
    public PapyrusCodebase Codebase
    {
        get
        {
            if (field != null)
            {
                return field;
            }
            lock (_codebaseLock)
            {
                field ??= CreateCodebase();
                return field;
            }
        }
    }

    public PapyrusProject UpdateDocument(string scriptName, string newText)
    {
        var document = _documents.GetValueOrDefault(scriptName)
            ?? throw new ArgumentException($"No document named '{scriptName}'.", nameof(scriptName));
        return ReplaceDocument(document, PapyrusDocument.Parse(newText, document.FilePath));
    }

    public PapyrusProject AddDocument(PapyrusDocument document)
    {
        var map = CloneDocuments();
        map[document.Name] = document;
        return new PapyrusProject(map, Codebase);
    }

    public PapyrusProject RemoveDocument(string scriptName)
    {
        if (!_documents.ContainsKey(scriptName))
        {
            throw new ArgumentException($"No document named '{scriptName}'.", nameof(scriptName));
        }
        var map = CloneDocuments();
        map.Remove(scriptName);
        return new PapyrusProject(map, Codebase);
    }

    private PapyrusProject ReplaceDocument(PapyrusDocument oldDocument, PapyrusDocument newDocument)
    {
        var map = CloneDocuments();
        map[newDocument.Name] = newDocument;
        if (!string.Equals(newDocument.Name, oldDocument.Name, StringComparison.OrdinalIgnoreCase))
        {
            map.Remove(oldDocument.Name);
        }
        return new PapyrusProject(map, Codebase);
    }

    private Dictionary<string, PapyrusDocument> CloneDocuments() => new(_documents, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Builds the codebase for this project. The first build constructs every
    /// model (in parallel); a fork reuses the base codebase's model for every
    /// unchanged document — identified by the document's syntax tree instance — and
    /// builds only the models of changed or added documents (at most one per fork).
    /// </summary>
    private PapyrusCodebase CreateCodebase()
    {
        var trees = _documents.Values.Select(d => d.SyntaxTree).ToList();
        if (_baseCodebase is null)
        {
            return new PapyrusCodebase(trees);
        }

        // Tree identity is the "unchanged document" test: a forked project shares
        // the document instances it did not touch, so their trees are the same
        // objects the base codebase built models for.
        var reuse = new Dictionary<SyntaxTree, PapyrusSemanticModel>();
        foreach (var model in _baseCodebase.Models)
        {
            reuse[model.SyntaxTree] = model;
        }

        var models = new PapyrusSemanticModel[trees.Count];
        for (var i = 0; i < trees.Count; i++)
        {
            models[i] = reuse.TryGetValue(trees[i], out var reused)
                ? reused
                : new PapyrusSemanticModel(trees[i], buildIndexes: true);
        }
        return new PapyrusCodebase(models);
    }
}
