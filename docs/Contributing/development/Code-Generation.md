# Code Generation
Most of Mutagen's record classes, interfaces, and binary translation code is generated rather than handwritten.

## Xml Definitions
Each record is defined by an xml file that sits next to its handwritten partial class, such as `Mutagen.Bethesda.Skyrim/Records/Major Records/DialogTopic.xml`.  The xml lists the record's fields, their types, and their record types, and the generator produces the matching `_Generated.cs` file.

To change a record's structure, edit its xml and regenerate.  The `_Generated.cs` files should never be edited by hand, as they are overwritten on the next run.

## Loqui
The generation is built on top of [Loqui](https://github.com/Noggog/Loqui), a code generation library that is only really used by Mutagen.  Loqui provides the xml schema and the base generation of the classes, interfaces, masks, equality, and copying.  `Mutagen.Bethesda.Generation` layers the Bethesda specific pieces on top, such as binary translation and FormLink handling.

## Running the Generator
`Mutagen.Bethesda.Generator.All` regenerates every game in one pass.  Run it after changing any xml definitions or generation code.

!!! note
    The generator locates the projects it writes to via paths relative to its working directory, so it must be run with its build output folder as the working directory.  IDEs do this by default.

## Debugging
When you want to modify a line in the generated code, the hard part is often finding which piece of code wrote it.  The generator has a built-in hook for this: you can list snippets of output text, and the debugger will break the moment a matching line is written.

### GenerationLines.txt
Create a file named `GenerationLines.txt` at the root of the repository, next to the solution files.  Each non-blank line is a snippet to watch for:

```
public partial class DialogTopicBinaryOverlay
SubtypeName =
```

Then whenever the generator writes a line that **contains** any of the snippets, it calls `Debugger.Break()` and pauses the run.  This allows you to then see the code involved, which gives you a starting point for modifying the code generator for the line you're interested in.

The `GenerationLines.txt` file is already in `.gitignore`, so it can be left in place between sessions with junk content.

Things To Know

- **Debug builds only.**  The hook is compiled under `#if DEBUG`, so a Release build will never break.
- **Edits are picked up live.**  The file is watched while the generator runs, so snippets can be added or removed mid-session without restarting.  An empty or missing file disables the check entirely.
- **Matches are substrings.**  Prefer a distinctive chunk of the line.  A short snippet like `Subtype` will match many lines and break constantly.
