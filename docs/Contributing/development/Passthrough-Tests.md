# Passthrough Tests
Unit tests cover edge-case prone code, but they can only check the cases someone thought to write.  Bethesda's own plugins hold a large number of records, with every quirk the format allows.  Passthrough tests use those files to confirm Mutagen reads and writes every record correctly.

A passthrough reads a plugin into Mutagen's classes and immediately writes it back out.  If the output matches the input byte for byte, the plugin was able to "pass through" Mutagen without anything being lost or changed.

## Why Run Them
Run passthroughs whenever a change could affect how records are read or written, such as:

- Adding or changing record definitions in the generator XML
- Changing binary translation code, or the Binary Overlay
- Changing custom binary logic for a specific record

A passthrough catches mistakes that are easy to miss, like a subrecord that's silently dropped, a field that's read with the wrong length, or a flag that only some records use.  The unit tests won't notice these, because they only flex the records the test built.

Passthroughs need the games installed, so they don't run in CI.  Run them locally on the games your change touches before opening a PR.

## How It Works
Comparing the input and output directly would turn up many differences that don't matter.  Compressed records, floats with more than one valid encoding, and subrecords in a different order are all equally valid, but don't match byte for byte.

So the source plugin first goes through a series of preparation steps, producing a "processed" file that looks the way Mutagen would write it.  Then Mutagen imports the original plugin, writes it back out, and compares the result against the processed file.

### Preparation
Each step writes its output to the test's cache folder, and the next step reads from it.

#### Trimming
Optionally strips out record groups, so a run can focus on a few record types.  See [Focusing a Run](#focusing-a-run).

This step also removes the few specific records listed in a game's processor `TrimmedRecords`.  These are records that can't be passed through for reasons unrelated to Mutagen, such as being corrupted in the game's own files.

#### Decompression
Records are decompressed, so the comparison happens on the raw content.

#### Group Merging
Some plugins contain the same top-level group more than once.  Mutagen writes each group once, so duplicates are merged together.

#### Alignment
Subrecords usually come in a predictable order, but they are not required to.  In some records Bethesda shuffles them, so the content is the same but the order on disk differs.  This step moves subrecords into the order Mutagen writes them in, as described by each game's `GetAlignmentRules()` in its `PassthroughTest` class.

#### Processing
The last step handles the remaining differences, through each game's `Processor` class in `Mutagen.Bethesda.Tests/Processing`.  Some of what it does:

- **Float standardization**: Some float values can be stored in more than one way.  Zero, for example, is stored by Bethesda in two different formats.  These are rewritten into the format Mutagen writes, without changing the values they represent.
- **Strings file reindexing**: Mutagen doesn't keep the original keys of localized strings, since they aren't meaningful information.  The source strings files are reindexed to match what Mutagen writes, starting at 1 and counting up for each string.
- **Empty groups and deleted records**: Empty groups are removed, as is leftover content inside records marked deleted.
- **Record-specific fixes**: Many record types have their own small adjustments, registered with `AddDynamicProcessing` for that record type.

### The Passthrough Itself
Once the processed file is ready, the original plugin is imported and exported back out in one or more ways:

| Test | What it does |
| --- | --- |
| Normal | Imports into the normal mutable classes, then writes |
| Overlay | Imports as a [Binary Overlay](Binary-Overlay-Design.md), copies it into mutable classes, then writes |
| CopyIn | Imports as a Binary Overlay, uses `DeepCopyIn` to copy it into a new mod, then writes |

Each output is compared to the processed file.  If even one byte differs, the test fails.  Localized games also compare each strings file that was written.

There are a few other tests that can be turned on alongside these, such as `Import` (only imports, without comparing), `Equals` (checks that a mod equals itself), and `Pex` (parses the scripts in the game's archives).

## Running Them

### Helper UI
The easiest way to run passthroughs is the test runner UI, which lets you pick which tests to run, and lays out their parallel output so it's easier to digest:

- `Mutagen.Bethesda.Tests.GUI.Avalonia` runs on any platform
- `Mutagen.Bethesda.Tests.GUI` is the older WPF version, which only runs on Windows

On its first run, the UI looks for installed games, and fills in their data folders and base master files as targets.  This configuration is saved to `Mutagen.Bethesda.Tests/TestingSettings.json`, which is ignored by git, so each developer keeps their own.

![](https://i.imgur.com/YqFdom4.gif)

![](https://i.imgur.com/clKzSP0.gif)

### Command Line
`Mutagen.Bethesda.Tests` is a console app that can run the same tests.

To run a single plugin:
```
dotnet run --project Mutagen.Bethesda.Tests -f net10.0 -- passthrough -m "<path to Skyrim.esm>" -r SkyrimSE
```

It runs the Normal and Overlay tests.  Some useful options:

| Option | Effect |
| --- | --- |
| `--test-copyin` | Also run the CopyIn test |
| `--cache-all` | Reuse every cached preparation step.  Nothing is reused by default |
| `--cache-trim`, `--cache-decompress`, `--cache-merge`, `--cache-align`, `--cache-process` | Reuse only some of the cached steps |
| `--include NPC_,ARMO` | Only test these record types |
| `--exclude NAVM,LAND` | Test everything except these record types |
| `-t <folder>` | Where to keep the cache folder |
| `--json` | Print the results as JSON |

To run everything set up in a settings file, such as the one the UI saves:
```
dotnet run --project Mutagen.Bethesda.Tests -f net10.0 -- run-config -p Mutagen.Bethesda.Tests/TestingSettings.json
```

### Cache Folder
Each plugin gets its own cache folder, at `<temp folder>/Mutagen_Binary_Tests/<GameRelease>/<plugin name>`.  The temp folder is the system temp folder unless one is set.  Some games are large, and on systems where the temp folder lives in RAM it's worth pointing it at a disk instead.

The folder holds the output of every preparation step, and of every test:

| File | Contents |
| --- | --- |
| `_Trimmed`, `_Uncompressed`, `_Merged`, `_Aligned` | Output of each preparation step |
| `_Processed` | The file the outputs are compared against |
| `_NormalImport_NormalExport` | The Normal test's output |
| `_BinaryOverlay` | The Overlay test's output |
| `_CopyIn` | The CopyIn test's output |

Preparing a large plugin takes a while, so each step can reuse its cached output from the last run.  This is a big time saver, but be careful when changing the preparation code itself.  A step that reuses its cache won't pick up your change, so turn off reuse for the step you changed, and for every step after it.

### Focusing a Run
Passing through a whole game takes time.  When working on a few record types, use trimming to test only those, with `--include`/`--exclude` on the command line, or the include and skip lists in the UI.  Run the full game once more before finishing, since a change can affect records you weren't focused on.

## When a Passthrough Fails
A failure lists the positions where the output first differs from the processed file, in hex.  These are offsets into both files.

To see what lives at a position, the console app has some inspection commands:

| Command | Shows |
| --- | --- |
| `inspect-record -p <file> -r <release> --pos <offset>` | The record at an offset, and its subrecords |
| `inspect-grup -p <file> -r <release> --pos <offset>` | The group header at an offset |
| `inspect-bytes -p <file> --pos <offset>` | A hex dump |
| `list-grups -p <file> -r <release>` | Every top-level group in a file |

Running these against both the `_Processed` file and the output usually shows which record and subrecord went wrong.  A hex editor with a compare view is also very useful here.

Once you know what differs, decide which side is wrong:

- **Mutagen lost or changed information.**  This is a real bug.  Fix it in the record definitions or the binary translation code, and add a unit test that covers it.  See [Bug Fixes](Testing.md#bug-fixes).
- **The difference doesn't matter.**  Both forms mean the same thing, such as two encodings of the same float, or subrecords in a different order.  Teach the preparation steps to standardize it, with an alignment rule or a processor fix for that record type, so future runs don't trip on it.

Be strict about the second case.  Every processor fix hides a difference from the test, so it should only ever hide differences that truly carry no information.
