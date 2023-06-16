The API and tools listed in the [[Plugin Record Suite]] are intended to expose Bethesda records in an organized, strongly typed, and (hopefully) less error prone fashion.  However, some tasks and some users require a less safe and more direct approach to get the job done.

This section is about some of the mechanics and tools under the hood, and are recommended for more advanced users.

# Reasoning and Typical Applications of Low Level Tooling
## Cross-game Processing
The [[Plugin Record Suite]] objects fall short when the task you are trying to accomplish is to be applied to multiple Bethesda games.  There are no halfway hybrid generated classes that can parse and contain both Skyrim and Oblivion NPCs, for example.

Interfaces can provide a small bit of relief, allowing some fields that are common to many records to be exposed.  `INamed` is an example of this.  Both Skyrim and Oblivion NPCs implement it, and so code that processed an `IEnumerable<INamed>` could handle both.

Low Level Tooling can be used instead of the generated records to write logic that can apply to multiple games where interfaces aren't good enough.  It comes at the cost of being a lot more error prone to use, losing a lot of the common functionality (Equals/ToString/etc), and requiring a lot more knowledge of binary details.

## Testing the Record Suite Itself
The generated records do the bulk of the parsing and data handling, but they cannot test themselves for correctness.  The manual granularity of the Low Level Tools allow for code and logic to be written for testing projects to do the work needed to confirm correctness of the generated records.
