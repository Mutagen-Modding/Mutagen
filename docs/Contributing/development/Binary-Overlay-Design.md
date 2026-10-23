# Binary Overlay Design

Binary Overlays are the readonly import path described in [Importing](../../plugins/Importing.md#read-only).  Many of the choices in how they work are deliberate tradeoffs, chosen to be performant while staying usable for as many kinds of programs as possible.  A change that makes one access pattern faster can make another one worse, or take away control that some consumers depend on.

This page covers the principles overlays are built around, so that proposed changes can be weighed against them.

## Mutable vs Overlay
Mutagen offers two ways to read a mod, and they sit at opposite ends of the memory/speed tradeoff.

### Mutable Mods are the Cache
A mutable mod reads all of a plugin's data into normal C# objects.  Every field is parsed and stored, so once it's loaded all data is "instantly" accessible.  In a sense, it's the ultimate cache.

The costs are:

- **Load time**: everything is parsed up front, even when only one record is wanted.
- **Memory**: reading `Skyrim.esm` into a mutable mod uses several GB, and that memory stays in use for as long as the mod object is alive.  Often that's the whole program, since it can live in a load order on an environment object.

### Overlays are the Lightweight Alternative
Overlays exist to cover the readonly cases, so that mutable mods are only needed for editing.  They're lazy by nature, so reading a single record doesn't require parsing the whole file.  They also endeavor to not keep anything alive.  If a program reads a record from the NPC group and then moves on to processing weapons, nothing inside the overlay is holding onto the NPC group, and the GC can clean it up.

This is the core distinction.  If a change adds caching inside overlays, it starts turning them back into the mutable pattern, and removes the option of a low-footprint read that selectively fetches data and then forgets it.  A program that wants everything cached already has options: use a mutable mod, or cache the specific things it needs on its own side.

## Principles

### A Thin Translation Layer
An overlay is a translation from bytes to strongly typed fields.  It should have as little opinion as possible on threading, memory retention, and similar concerns, because those are decisions the consuming program is in the best position to make.

### Lazy by Access
Work is only done when a property is accessed.  The act of accessing a member is the only signal of interest; consumers don't have to declare up front which records or fields they care about.

Until something is accessed, the overlay only marks locations: where the groups are, where each record is within a group, where each subrecord is within a record.  Data is skipped over, not parsed.

### No Internal Retention
Overlays don't keep references to the records or data they've handed out.  The consumer holds the only reference, so once they're done with something it can be cleaned up.  How much memory to trade for speed is the consumer's choice, not the library's.

### Control Over Convenience
Mutagen is designed to be approachable, and many of its users are writing small Synthesis patchers without much attention to performance.  A change that helps those scripts at the cost of control for more involved programs is generally not a good trade.

For example, the Mutagen analyzers make heavy use of overlays, and rely on having a lightweight overlay whose memory usage they control.  Long-running tools, editors, and anything that scans many plugins selectively are in the same position.

The goal is a middle ground: overlays stay unopinionated, and simpler programs are served by [best practices](../../best-practices/Overlays-Single-Access.md) and by the defaults a host chooses (see [Simpler Programs and Host Defaults](#simpler-programs-and-host-defaults)).

## Where the Cost Goes
Before optimizing, it's worth knowing which phase is actually doing the work.  For a typical "loop all NPCs and print them":

| Phase | Work Done | Cost |
| --- | --- | --- |
| Create the mod overlay | Open the file, mark the mod header and each top level group's location, skipping their contents | Quick |
| Access `mod.Npcs` | Read the NPC group's bytes off disk, loop them to mark each record's location without parsing | One disk read, then a cheap marking loop |
| Loop the records | Return an overlay per record, pointing at a slice of the group's bytes | Near instant, slices are essentially free |
| Access fields | Mark subrecord locations within the record, then interpret the specific field's bytes | Where the real parsing happens, and only for the fields touched |

The most expensive operation is the disk read, which happens each time a group is accessed.  After that, the bulk of the work is in the consumer's own loop, parsing the fields it touches.

[:octicons-arrow-right-24: Concrete Example walkthrough](../../best-practices/Overlays-Single-Access.md#concrete-example)

## Threading
For overlays, threading is the consumer's job.  The phases Mutagen performs before handing records over are just location marking and byte slicing, which are cheap enough that threading them often costs more than it saves.  Parallelizing them also doesn't speed up the expensive part, which is the consumer's loop.  A program that wants parallelism can parallelize that loop itself.

Mutable mod import is different.  It doesn't return until the whole mod has been read, so finishing that as fast as possible is a win, and internal parallelism is a reasonable fit there.  If added, it should come with a way for consumers to limit or disable it.

## Simpler Programs and Host Defaults
Many programs will never follow the overlay best practices, such as accessing a group once and keeping the result.  Rather than building behavior for those programs into the overlay layer itself, the intended route is for hosts to choose defaults.  For example, Synthesis runs many unoptimized patchers, so it could reasonably turn on settings that a standalone tool would leave off.

Any such setting has a cost beyond its runtime behavior.  The overlay internals are already complex, and are generated for every game, so each new option has to be worth the added complexity.

## Case Studies

### Deferred Decompression
Compressed records are currently decompressed as soon as the record overlay is created, while looping a group.  Deferring decompression until a field is actually accessed means reading something like the FormKey doesn't pay for decompressing the whole record.

This fits the design well.  It does less work until a field is touched, doesn't retain anything new, and doesn't change behavior for consumers.

### Internal Group Caching
Accessing `mod.Npcs` reads the group's bytes off disk every time.  Programs that access a group repeatedly (often unknowingly) pay that disk read again and again, which can make some access patterns very slow.  Caching groups inside the overlay would fix that.

The tradeoffs:

- It raises the memory floor, even for programs that only want to touch each group once.
- It removes the option of reading a group and letting it be cleaned up, which is one of the main reasons overlays exist.
- Making it optional means adding a setting to already complex internals.

Possible middle grounds that have been raised:

- An opt-in setting, so only programs that want the cache get it.
- A setting that hosts like Synthesis default to on.
- Caching only the group's bytes read off disk, so a group is never read from disk more than once, while caching nothing below the group level.

Where this lands is still open.  Repeated disk reads are the largest cost a cache could remove, while caching parsed data below the group level is much less likely to be worth the complexity and memory.

## Known Improvement Directions
These are open issues that fit the design and would have a large impact:

- [#74 Make Overlay Decompressions Lazy](https://github.com/Mutagen-Modding/Mutagen/issues/74)
- [#229 Improve MutableModLinkCache performance](https://github.com/Mutagen-Modding/Mutagen/issues/229)
